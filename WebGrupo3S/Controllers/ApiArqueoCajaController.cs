using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebGrupo3S.Models;

namespace WebGrupo3S.Controllers
{
    public class ApiArqueoCajaController : ApiController
    {
        private SSS_OPERACIONEntities db = new SSS_OPERACIONEntities();

        // GET: api/ApiArqueoCaja
        public IQueryable<ArqueoCaja> GetArqueoCaja()
        {
            return db.ArqueoCaja;
        }

        // GET: api/ApiArqueoCaja/5
        [ResponseType(typeof(ArqueoCaja))]
        public IHttpActionResult GetArqueoCaja(short id)
        {
            ArqueoCaja arqueoCaja = db.ArqueoCaja.Find(id);
            if (arqueoCaja == null)
            {
                return NotFound();
            }

            return Ok(arqueoCaja);
        }

        // PUT: api/ApiArqueoCaja/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutArqueoCaja(short id, ArqueoCaja arqueoCaja)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != arqueoCaja.aq_empresa)
            {
                return BadRequest();
            }

            db.Entry(arqueoCaja).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArqueoCajaExists(id))
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

        // POST: api/ApiArqueoCaja
        [ResponseType(typeof(ArqueoCaja))]
        public IHttpActionResult PostArqueoCaja(ArqueoCaja arqueoCaja)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ArqueoCaja.Add(arqueoCaja);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (ArqueoCajaExists(arqueoCaja.aq_empresa))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = arqueoCaja.aq_empresa }, arqueoCaja);
        }

        // DELETE: api/ApiArqueoCaja/5
        [ResponseType(typeof(ArqueoCaja))]
        public IHttpActionResult DeleteArqueoCaja(short id)
        {
            ArqueoCaja arqueoCaja = db.ArqueoCaja.Find(id);
            if (arqueoCaja == null)
            {
                return NotFound();
            }

            db.ArqueoCaja.Remove(arqueoCaja);
            db.SaveChanges();

            return Ok(arqueoCaja);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ArqueoCajaExists(short id)
        {
            return db.ArqueoCaja.Count(e => e.aq_empresa == id) > 0;
        }
    }
}