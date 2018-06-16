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
    public class ApiDetCatalogoController : ApiController
    {
        private SSS_COMPLEMENTOSEntities db = new SSS_COMPLEMENTOSEntities();

        // GET: api/ApiDetCatalogo
        public IQueryable<DetCatalogo> GetDetCatalogoes()
        {
            return db.DetCatalogoes;
        }

        // GET: api/ApiDetCatalogo/5
        [ResponseType(typeof(DetCatalogo))]
        public IHttpActionResult GetDetCatalogo(short id)
        {
            DetCatalogo detCatalogo = db.DetCatalogoes.Find(id);
            if (detCatalogo == null)
            {
                return NotFound();
            }

            return Ok(detCatalogo);
        }

        // PUT: api/ApiDetCatalogo/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDetCatalogo(short id, DetCatalogo detCatalogo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != detCatalogo.cd_empresa)
            {
                return BadRequest();
            }

            db.Entry(detCatalogo).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DetCatalogoExists(id))
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

        // POST: api/ApiDetCatalogo
        [ResponseType(typeof(DetCatalogo))]
        public IHttpActionResult PostDetCatalogo(DetCatalogo detCatalogo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DetCatalogoes.Add(detCatalogo);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (DetCatalogoExists(detCatalogo.cd_empresa))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = detCatalogo.cd_empresa }, detCatalogo);
        }

        // DELETE: api/ApiDetCatalogo/5
        [ResponseType(typeof(DetCatalogo))]
        public IHttpActionResult DeleteDetCatalogo(short id)
        {
            DetCatalogo detCatalogo = db.DetCatalogoes.Find(id);
            if (detCatalogo == null)
            {
                return NotFound();
            }

            db.DetCatalogoes.Remove(detCatalogo);
            db.SaveChanges();

            return Ok(detCatalogo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DetCatalogoExists(short id)
        {
            return db.DetCatalogoes.Count(e => e.cd_empresa == id) > 0;
        }
    }
}