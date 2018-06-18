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
    public class ApiCierreCajaController : ApiController
    {
        private SSS_OPERACIONEntities db = new SSS_OPERACIONEntities();

        // GET: api/ApiCierreCaja
        public IQueryable<CierreCaja> GetCierreCaja()
        {
            return db.CierreCaja;
        }

        // GET: api/ApiCierreCaja/5
        [ResponseType(typeof(CierreCaja))]
        public IHttpActionResult GetCierreCaja(short id)
        {
            CierreCaja cierreCaja = db.CierreCaja.Find(id);
            if (cierreCaja == null)
            {
                return NotFound();
            }

            return Ok(cierreCaja);
        }

        // PUT: api/ApiCierreCaja/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCierreCaja(short id, CierreCaja cierreCaja)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cierreCaja.cj_empresa)
            {
                return BadRequest();
            }

            db.Entry(cierreCaja).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CierreCajaExists(id))
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

        // POST: api/ApiCierreCaja
        [ResponseType(typeof(CierreCaja))]
        public IHttpActionResult PostCierreCaja(CierreCaja cierreCaja)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CierreCaja.Add(cierreCaja);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CierreCajaExists(cierreCaja.cj_empresa))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = cierreCaja.cj_empresa }, cierreCaja);
        }

        // DELETE: api/ApiCierreCaja/5
        [ResponseType(typeof(CierreCaja))]
        public IHttpActionResult DeleteCierreCaja(short id)
        {
            CierreCaja cierreCaja = db.CierreCaja.Find(id);
            if (cierreCaja == null)
            {
                return NotFound();
            }

            db.CierreCaja.Remove(cierreCaja);
            db.SaveChanges();

            return Ok(cierreCaja);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CierreCajaExists(short id)
        {
            return db.CierreCaja.Count(e => e.cj_empresa == id) > 0;
        }
    }
}