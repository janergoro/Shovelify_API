using Swap.API.DAL;
using Swap.API.Models;
using Swap.API.Models.AddModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Swap.API.Controllers
{
    public class LikesController : ApiController
    {
        private SwapDbContext db = new SwapDbContext();

        // POST: api/Items
        public async Task<IHttpActionResult> PostItem(LikeAddModel addedLike)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var like = db.Likes.Add(new Like
            {
                Created = DateTime.Now,
                Decision = addedLike.Decision,
                LikedItem = db.Items.First(x => x.ItemId == addedLike.LikedItemId),
                LikingItem = db.Items.First(x => x.ItemId == addedLike.MyItemId),
            }); ;

            db.Likes.Add(like);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = like.LikeId}, like);
        }


    }
}
