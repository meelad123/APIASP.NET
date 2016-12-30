using projectNew.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace projectNew.Controllers.v1
{
    [Authorize]
    public class VacationsController : ApiController
    {

        [AcceptVerbs("GET")]
        public IEnumerable<Vacation> vacations()
        {
            IEnumerable<Vacation> v;          
            using (var _db = new DBContext())
            {
                v = _db.MyVacations.ToList();
            }
            return v;
        }
        [AcceptVerbs("POST")]
        public IHttpActionResult newVacation(Vacation vacation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var identity = User.Identity as ClaimsIdentity;
            var userName = identity.Claims.ElementAt(0).Value;
            using (var _db = new DBContext()) 
            {
                 vacation.User = _db.MyUsers.FirstOrDefault(u => u.UserName == userName);
                _db.MyVacations.Add(vacation);
                _db.SaveChanges();
            }

            vacation.User = null;
            return Ok(vacation);
        }

        [AcceptVerbs("GET")]
        //GET: api/v1/vacations/{id} 
        public IHttpActionResult getVacationById(int id)
        {
            Vacation vacation;
            using (var _db = new DBContext())
            {
                vacation = _db.MyVacations.FirstOrDefault(vac => vac.VacationId == id);
            }
           
            if (vacation == null)
                return NotFound();
            return Ok(vacation);
        }
        [AcceptVerbs("PUT")]
        public IHttpActionResult PutVacation(int id, Vacation vacation)
        {
            var identity = User.Identity as ClaimsIdentity;
            var usrName = identity.Claims.ElementAt(0).Value;
            using (var _db = new DBContext())
            {
                var usr = _db.MyUsers.FirstOrDefault(u => u.UserName == usrName);
                var vac = _db.MyVacations.FirstOrDefault(v => v.VacationId == id);
                if (vac.UserId == usr.UserId)
                { 
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    if (id != vacation.VacationId)
                    {
                        return BadRequest();
                    }

                    _db.Entry(vacation).State = System.Data.Entity.EntityState.Modified;

                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!(_db.MyVacations.Count(e => e.VacationId == id) > 0))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    return Ok();
                }
                else
                    return StatusCode(HttpStatusCode.Unauthorized);
            }

        }
        [AcceptVerbs("PATCH")]
        public IHttpActionResult PatchVacation(int id, Vacation vacation)
        {
            var identity = User.Identity as ClaimsIdentity;
            var usrName = identity.Claims.ElementAt(0).Value;
            using (var _db = new DBContext())
            {
                var usr = _db.MyUsers.FirstOrDefault(u => u.UserName == usrName);
                var vac = _db.MyVacations.FirstOrDefault(v => v.VacationId == id);
                if (vac.UserId == usr.UserId)
                {
                    Vacation vacationToUpdate = _db.MyVacations.FirstOrDefault(v => v.VacationId == id);
                    if (vacationToUpdate == null)
                    {
                        return NotFound();
                    }

                    if (vacation.Description != null)
                        vacationToUpdate.Description = vacation.Description;
                    if (vacation.End != null)
                        vacationToUpdate.End = vacation.End;
                    if (vacation.Place != null)
                        vacationToUpdate.Place = vacation.Place;
                    if (vacation.Start != null)
                        vacationToUpdate.Start = vacation.Start;
                    if (vacation.Title != null)
                        vacationToUpdate.Title = vacation.Title;


                    _db.Entry(vacationToUpdate).State = System.Data.Entity.EntityState.Modified;

                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!(_db.MyVacations.Count(e => e.VacationId == id) > 0))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    return Ok();
                }
                else
                    return StatusCode(HttpStatusCode.Unauthorized);
            }
        }
        [AcceptVerbs("DELETE")]
        public IHttpActionResult deleteVacation(int id)
        {
            using (var _db = new DBContext())
            {
                var identity = User.Identity as ClaimsIdentity;
                var usrName = identity.Claims.ElementAt(0).Value;
                var usr = _db.MyUsers.FirstOrDefault(u => u.UserName == usrName);
                var vaca = _db.MyVacations.Include("Memories").FirstOrDefault(v => v.VacationId == id);
                if (vaca == null)
                {
                    return NotFound();
                }
                if (vaca.UserId == usr.UserId)
                {
                    _db.MyVacations.Remove(vaca);
                    _db.SaveChanges();

                    return Ok();
                }
                else
                    return StatusCode(HttpStatusCode.Unauthorized);
            }
        }
        //api/v1/vacations/<id>/memories
        [AcceptVerbs("GET")]
        public IHttpActionResult memories(int id, string modifier)
        {
            using (var _db = new DBContext())
            {
                var identity = User.Identity as ClaimsIdentity;
                var usrName = identity.Claims.ElementAt(0).Value;
                var usr = _db.MyUsers.FirstOrDefault(u => u.UserName == usrName);
                var vaca = _db.MyVacations.Include("Memories").FirstOrDefault(v => v.VacationId == id);
                if (vaca == null)
                    return NotFound();
                if (vaca.UserId == usr.UserId)
                {
                    ICollection<Memory> memories = (ICollection<Memory>)vaca.Memories;
                    foreach (Memory mem in memories)
                    {
                        mem.Vacation = null;
                    }

                    return Ok(memories);
                }
                else
                    return StatusCode(HttpStatusCode.Unauthorized);
            }
        }
        [AcceptVerbs("POST")]
        public IHttpActionResult memories(int id, string modifier, Memory memory)
        {
            using (var _db = new DBContext())
            {
                var identity = User.Identity as ClaimsIdentity;
                var usrName = identity.Claims.ElementAt(0).Value;
                var usr = _db.MyUsers.Include("Vacations").FirstOrDefault(u => u.UserName == usrName);
                if (usr.Vacations.Any(v => v.VacationId == id))
                {
                    Vacation vac = _db.MyVacations.Include("Memories").FirstOrDefault(u => u.VacationId.Equals(id));

                    if (!ModelState.IsValid || vac == null)
                        return BadRequest(ModelState);
                    memory.Vacation = vac;
                    vac.Memories.Add(memory);
                    _db.SaveChanges();

                    memory.Vacation = null;
                    return Ok(memory);
                }
                else
                    return StatusCode(HttpStatusCode.Unauthorized);
            }

        }
    }
}
