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
using System.Web.Http.Description;

namespace Swap.API.Controllers
{
    public class UsersController : ApiController
    {
        private SwapDbContext db = new SwapDbContext();


        // GET: api/Users
        public IQueryable<User> GetItems()
        {
                return db.Users;
        }


        // POST: api/Users
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> PostUser(UserAddModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //return new System.Web.Http.Results.OkResult(this);

            var u = new User
            {
                Email = user.Email,
                FaceBookUniqueId = user.FaceBookUniqueId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNo = user.PhoneNo,
            };
            db.Users.Add(u);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = u.UserId }, u);
        }
    }
}
