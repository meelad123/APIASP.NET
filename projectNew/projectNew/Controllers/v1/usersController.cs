using projectNew.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Microsoft.AspNet.Identity.EntityFramework;

namespace projectNew.Controllers.v1
{
    [Authorize]
    public class UsersController : ApiController
    {
        #region variables
        private UserManager<IdentityUser> _userManager;
        #endregion

        #region requests
        [AcceptVerbs("GET")]
        // GET: api/v1/users/{id}
        public IHttpActionResult GetUserByUserName(string id)
        {
            User user;
            using (var _db = new DBContext())
            {
                user = _db.MyUsers.FirstOrDefault(u => u.UserName.Equals(id));
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
        }

        [AcceptVerbs("PUT")]
        //PUT: Updates the user with username <username>.
        public IHttpActionResult UpdateUser(string id, User user)
        {
            using (var _db = new DBContext())
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity.Claims.Any(u => u.Value == id))
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    if (id != user.UserName)
                    {
                        return BadRequest();
                    }

                    _db.Entry(user).State = System.Data.Entity.EntityState.Modified;

                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!(_db.MyUsers.Count(e => e.UserName == id) > 0))
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
                else
                    return StatusCode(HttpStatusCode.Unauthorized);
            }
        }

        [AcceptVerbs("PATCH")]
        public IHttpActionResult PratiallyUpdateUser(string id, User user)
        {
            using (var _db = new DBContext())
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity.Claims.Any(u => u.Value == id))
                {
                    if (!ModelState.IsValid)
                        return BadRequest(ModelState);
                    User userUpdate = _db.MyUsers.FirstOrDefault(u => u.UserName.Equals(id));

                    if (userUpdate == null)
                        return NotFound();
                    if (user.Email != null)
                        userUpdate.Email = user.Email;
                    if (user.FirstName != null)
                        userUpdate.FirstName = user.FirstName;
                    if (user.LastName != null)
                        userUpdate.LastName = user.LastName;
                    if (user.UserName != null)
                        userUpdate.UserName = user.UserName;

                    _db.Entry(userUpdate).State = System.Data.Entity.EntityState.Modified;
                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!(_db.MyUsers.Count(e => e.UserName == id) > 0))
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
                else
                    return StatusCode(HttpStatusCode.Unauthorized);
            }
        }
        [AcceptVerbs("DELETE")]
        public IHttpActionResult deleteUser(string id)
        {
            using (var _db = new DBContext())
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity.Claims.Any(u => u.Value == id))
                {
                    User userToremove = _db.MyUsers.Include("Friends").FirstOrDefault(u => u.UserName.Equals(id));
                    IdentityUser uSer = _db.Users.FirstOrDefault(u => u.UserName.Equals(id));
                    if (userToremove == null)
                        return NotFound();
                    userToremove.Friends.Clear();
                    _db.MyUsers.Remove(userToremove);
                    _db.Users.Remove(uSer);
                    _db.SaveChanges();

                    return Ok();
                }
                else
                    return StatusCode(HttpStatusCode.Unauthorized);
            }
        }

        [AcceptVerbs("GET")]
        public IHttpActionResult friends(string id, string modifier)
        {
            using (var _db = new DBContext())
            {

                if (modifier == "friends")
                {
                    User user = _db.MyUsers.Include("Friends").FirstOrDefault(u => u.UserName.Equals(id));

                    if (user == null)
                        return NotFound();

                    ICollection<User> friends = (ICollection<User>)user.Friends;
                    foreach (User u in friends)
                    {
                        u.Friends = null;
                    }
                    return Ok(friends);
                }
                else
                {
                    var identity = User.Identity as ClaimsIdentity;
                    var usrName = identity.Claims.ElementAt(0).Value;
                    User user = _db.MyUsers.Include("Vacations").Include("Friends").FirstOrDefault(u => u.UserName.Equals(id));

                    if ((usrName == id) || (user.Friends.Any(f => f.UserName == usrName)))
                    {
                        if (user == null)
                        {
                            return NotFound();
                        }

                        ICollection<Vacation> vacations = user.Vacations;
                        foreach (Vacation vac in vacations)
                        {
                            vac.User = null;
                        }

                        return Ok(vacations);
                    }
                    else
                        return StatusCode(HttpStatusCode.Unauthorized);
                }
            }
        }

        [AcceptVerbs("POST")]
        public IHttpActionResult friends(string id, User newFriend, string modifier)
        {
            using (var _db = new DBContext())
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity.Claims.Any(u => u.Value == id))
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    if (!(_db.MyUsers.Count(e => e.UserName == newFriend.UserName) > 0))
                    {
                        return NotFound();
                    }
                    User user = _db.MyUsers.FirstOrDefault(u => u.UserName.Equals(id));
                    User friendToAdd = _db.MyUsers.FirstOrDefault(u => u.UserName.Equals(newFriend.UserName));
                    user.Friends = user.Friends ?? new List<User>();
                    user.Friends.Add(friendToAdd);

                    _db.Entry(user).State = System.Data.Entity.EntityState.Modified;

                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!(_db.MyUsers.Count(e => e.UserName == id) > 0))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    return Ok(newFriend);
                }
                else
                    return StatusCode(HttpStatusCode.Unauthorized);
            }

        }

        [AcceptVerbs("DELETE")]
        public IHttpActionResult deleteFriend(string id, string modifier, string secondModifer)
        {
            using (var _db = new DBContext())
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity.Claims.Any(u => u.Value == id))
                {
                    User user = _db.MyUsers.Include("Friends").FirstOrDefault(u => u.UserName.Equals(id));
                    User friendToRemove = user.Friends.FirstOrDefault(u => u.UserName == secondModifer);
                    if (user == null || friendToRemove == null)
                        return NotFound();
                    user.Friends.Remove(friendToRemove);
                    _db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    return Ok();
                }
                else
                    return StatusCode(HttpStatusCode.Unauthorized);
            }
        }
        #endregion
        

    }
}
