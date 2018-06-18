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
    public class ApiServicioController : ApiController
    {
        private SSS_OPERACIONEntities db = new SSS_OPERACIONEntities();

        // GET: api/ApiServicio
        public IQueryable<Servicio> GetServicio()
        {
            return db.Servicio;
        }

        // GET: api/ApiServicio/5
        [ResponseType(typeof(Servicio))]
        public IHttpActionResult GetServicio(short id)
        {
            Servicio servicio = db.Servicio.Find(id);
            if (servicio == null)
            {
                return NotFound();
            }

            return Ok(servicio);
        }

        // PUT: api/ApiServicio/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutServicio(short id, Servicio servicio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != servicio.sv_empresa)
            {
                return BadRequest();
            }

            db.Entry(servicio).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServicioExists(id))
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

        // POST: api/ApiServicio
        [ResponseType(typeof(Servicio))]
        public IHttpActionResult PostServicio(Servicio servicio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Servicio.Add(servicio);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (ServicioExists(servicio.sv_empresa))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = servicio.sv_empresa }, servicio);
        }

        // DELETE: api/ApiServicio/5
        [ResponseType(typeof(Servicio))]
        public IHttpActionResult DeleteServicio(short id)
        {
            Servicio servicio = db.Servicio.Find(id);
            if (servicio == null)
            {
                return NotFound();
            }

            db.Servicio.Remove(servicio);
            db.SaveChanges();

            return Ok(servicio);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ServicioExists(short id)
        {
            return db.Servicio.Count(e => e.sv_empresa == id) > 0;
        }
    }
}