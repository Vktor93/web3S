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
    public class ApiCabCatalogoController : ApiController
    {
        private SSS_COMPLEMENTOSEntities db = new SSS_COMPLEMENTOSEntities();

        // GET: api/ApiCabCatalogo
        public IQueryable<CabCatalogo> GetCabCatalogoes()
        {
            return db.CabCatalogoes;
        }

        // GET: api/ApiCabCatalogo/5
        [ResponseType(typeof(CabCatalogo))]
        public IHttpActionResult GetCabCatalogo(short id)
        {
            CabCatalogo cabCatalogo = db.CabCatalogoes.Find(id);
            if (cabCatalogo == null)
            {
                return NotFound();
            }

            return Ok(cabCatalogo);
        }

        // PUT: api/ApiCabCatalogo/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCabCatalogo(short id, CabCatalogo cabCatalogo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cabCatalogo.cc_empresa)
            {
                return BadRequest();
            }

            db.Entry(cabCatalogo).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CabCatalogoExists(id))
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

        // POST: api/ApiCabCatalogo
        [ResponseType(typeof(CabCatalogo))]
        public IHttpActionResult PostCabCatalogo(CabCatalogo cabCatalogo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CabCatalogoes.Add(cabCatalogo);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CabCatalogoExists(cabCatalogo.cc_empresa))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = cabCatalogo.cc_empresa }, cabCatalogo);
        }

        // DELETE: api/ApiCabCatalogo/5
        [ResponseType(typeof(CabCatalogo))]
        public IHttpActionResult DeleteCabCatalogo(short id)
        {
            CabCatalogo cabCatalogo = db.CabCatalogoes.Find(id);
            if (cabCatalogo == null)
            {
                return NotFound();
            }

            db.CabCatalogoes.Remove(cabCatalogo);
            db.SaveChanges();

            return Ok(cabCatalogo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CabCatalogoExists(short id)
        {
            return db.CabCatalogoes.Count(e => e.cc_empresa == id) > 0;
        }
    }
}