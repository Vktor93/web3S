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
    public class ApiSucursalController : ApiController
    {
        private SSS_OPERACIONEntities db = new SSS_OPERACIONEntities();

        // GET: api/ApiSucursal
        public IQueryable<Sucursal> GetSucursal()
        {
            return db.Sucursal;
        }

        // GET: api/ApiSucursal/5
        [ResponseType(typeof(Sucursal))]
        public IHttpActionResult GetSucursal(short id)
        {
            Sucursal sucursal = db.Sucursal.Find(id);
            if (sucursal == null)
            {
                return NotFound();
            }

            return Ok(sucursal);
        }

        // PUT: api/ApiSucursal/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSucursal(short id, Sucursal sucursal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sucursal.su_empresa)
            {
                return BadRequest();
            }

            db.Entry(sucursal).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SucursalExists(id))
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

        // POST: api/ApiSucursal
        [ResponseType(typeof(Sucursal))]
        public IHttpActionResult PostSucursal(Sucursal sucursal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Sucursal.Add(sucursal);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (SucursalExists(sucursal.su_empresa))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = sucursal.su_empresa }, sucursal);
        }

        // DELETE: api/ApiSucursal/5
        [ResponseType(typeof(Sucursal))]
        public IHttpActionResult DeleteSucursal(short id)
        {
            Sucursal sucursal = db.Sucursal.Find(id);
            if (sucursal == null)
            {
                return NotFound();
            }

            db.Sucursal.Remove(sucursal);
            db.SaveChanges();

            return Ok(sucursal);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SucursalExists(short id)
        {
            return db.Sucursal.Count(e => e.su_empresa == id) > 0;
        }
    }
}