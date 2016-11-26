using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Swap.API.DAL;
using Swap.API.Models;
using Swap.API.Models.AddModels;

namespace Swap.API.Controllers
{
    public class ItemsController : ApiController
    {
        private SwapDbContext db = new SwapDbContext();

        // GET: api/Items
        public IEnumerable<Item> GetItems()
        {
            return db.Items.Include("User").ToList();
        }

        // GET: api/Items/MyItems
        [HttpGet]
        //[Route("MyItems/{fbId}")]
        [ActionName("MyItems")]
        public IEnumerable<ItemWithPictures> MyItems(string id)
        {
            var items = db.Items.Where(x => x.User.FaceBookUniqueId == id).ToList();
            var itemsWithPics = items.Select(i => new ItemWithPictures
            {
                ItemId = i.ItemId,
                DealType = (int)i.DealType,
                Description = i.Description,
                Value = (int)i.Value,
                PictureData = db.Pictures.FirstOrDefault(x => x.Item.ItemId == i.ItemId).FileData,
                MatchCount = db.Likes.Count(x => x.LikedItem.ItemId == i.ItemId),
            });
            return itemsWithPics;
        }

        // GET: api/Items/ItemsToLike
        [HttpGet]
        //[Route("ItemsToLike/{fbId}")]
        [ActionName("ItemsToLike")]
        public IEnumerable<ItemWithPictures> ItemsToLike(string id)
        {
            var allMyLikes = db.Likes.Where(x => x.LikingItem.User.FaceBookUniqueId == id).ToList();
            var itemsWithPics = db.Items.Where(i => i.User.FaceBookUniqueId != id && !allMyLikes.Any(x => x.LikedItem.ItemId == i.ItemId))
                  .Take(3)
                    .ToList()
                  .Select(i => new ItemWithPictures
                  {
                      ItemId = i.ItemId,
                      DealType = (int)i.DealType,
                      Description = i.Description,
                      Value = (int)i.Value,
                      PictureData = db.Pictures.FirstOrDefault(x => x.Item.ItemId == i.ItemId).FileData,
                  });
            return itemsWithPics;
        }

        // GET: api/Items/5
        [HttpGet]
        [ActionName("Get")]

        [ResponseType(typeof(ItemWithMatchedItemsViewModel))]
        public async Task<IHttpActionResult> GetItem(int id)
        {
            Item plainItem = await db.Items.FindAsync(id);
            var matches = db.Likes.Where(x => x.LikedItem.ItemId == id).ToList();
            ItemWithMatchedItemsViewModel item = new ItemWithMatchedItemsViewModel
            {
                ItemId = plainItem.ItemId,
                Description = plainItem.Description,
                PictureData = db.Pictures.First(x => x.Item.ItemId == plainItem.ItemId).FileData,
                Value = (int)plainItem.Value,
                DealType = (int)plainItem.DealType,
                MatchCount = matches.Count,
                Matches = matches.Select(x => new ItemWithPictures { ItemId = x.LikingItem.ItemId,
                    PictureData = db.Pictures.First(p => p.Item.ItemId == x.LikingItem.ItemId).FileData,

                }).ToArray(),
            };
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        // PUT: api/Items/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutItem(int id, Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != item.ItemId)
            {
                return BadRequest();
            }

            db.Entry(item).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Items
        [ResponseType(typeof(Item))]
        //[ActionName("Add")]
        public async Task<IHttpActionResult> PostItem(ItemAndPictureAddModel addableItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = db.Users.FirstOrDefault(x => x.FaceBookUniqueId == addableItem.UserId);
            var item = new Item
            {
                User = user,
                Description = addableItem.Description,
                Value = (Item.ItemValueLevel)Enum.Parse(typeof(Item.ItemValueLevel), addableItem.Value.ToString()),
                DealType = Item.ItemDealType.Default,
            };
            db.Items.Add(item);

            var pic = new Picture
            {
                FileData = addableItem.PictureData,
                Item = item,
            };
            db.Pictures.Add(pic);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = item.ItemId }, item);
        }

        //// POST: api/Items
        //[ResponseType(typeof(Item))]
        //public async Task<IHttpActionResult> PostItem(Item item)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Items.Add(item);
        //    await db.SaveChangesAsync();

        //    return CreatedAtRoute("DefaultApi", new { id = item.ItemId }, item);
        //}

        // DELETE: api/Items/5
        [ResponseType(typeof(Item))]
        public async Task<IHttpActionResult> DeleteItem(int id)
        {
            Item item = await db.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            db.Items.Remove(item);
            await db.SaveChangesAsync();

            return Ok(item);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ItemExists(int id)
        {
            return db.Items.Count(e => e.ItemId == id) > 0;
        }
    }
}