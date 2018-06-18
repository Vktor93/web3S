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
    public class ApiCuentaXCobrarController : ApiController
    {
        private SSS_OPERACIONEntities db = new SSS_OPERACIONEntities();

        // GET: api/ApiCuentaXCobrar
        public IQueryable<CuentaXCobrar> GetCuentaXCobrar()
        {
            return db.CuentaXCobrar;
        }

        // GET: api/ApiCuentaXCobrar/5
        [ResponseType(typeof(CuentaXCobrar))]
        public IHttpActionResult GetCuentaXCobrar(short id)
        {
            CuentaXCobrar cuentaXCobrar = db.CuentaXCobrar.Find(id);
            if (cuentaXCobrar == null)
            {
                return NotFound();
            }

            return Ok(cuentaXCobrar);
        }

        // PUT: api/ApiCuentaXCobrar/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCuentaXCobrar(short id, CuentaXCobrar cuentaXCobrar)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cuentaXCobrar.cc_empresa)
            {
                return BadRequest();
            }

            db.Entry(cuentaXCobrar).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CuentaXCobrarExists(id))
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

        // POST: api/ApiCuentaXCobrar
        [ResponseType(typeof(CuentaXCobrar))]
        public IHttpActionResult PostCuentaXCobrar(CuentaXCobrar cuentaXCobrar)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CuentaXCobrar.Add(cuentaXCobrar);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CuentaXCobrarExists(cuentaXCobrar.cc_empresa))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = cuentaXCobrar.cc_empresa }, cuentaXCobrar);
        }

        // DELETE: api/ApiCuentaXCobrar/5
        [ResponseType(typeof(CuentaXCobrar))]
        public IHttpActionResult DeleteCuentaXCobrar(short id)
        {
            CuentaXCobrar cuentaXCobrar = db.CuentaXCobrar.Find(id);
            if (cuentaXCobrar == null)
            {
                return NotFound();
            }

            db.CuentaXCobrar.Remove(cuentaXCobrar);
            db.SaveChanges();

            return Ok(cuentaXCobrar);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CuentaXCobrarExists(short id)
        {
            return db.CuentaXCobrar.Count(e => e.cc_empresa == id) > 0;
        }
    }
}