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
    public class ApiCuentaXPagarController : ApiController
    {
        private SSS_OPERACIONEntities db = new SSS_OPERACIONEntities();

        // GET: api/ApiCuentaXPagar
        public IQueryable<CuentaXPagar> GetCuentaXPagars()
        {
            return db.CuentaXPagars;
        }

        // GET: api/ApiCuentaXPagar/5
        [ResponseType(typeof(CuentaXPagar))]
        public IHttpActionResult GetCuentaXPagar(short id)
        {
            CuentaXPagar cuentaXPagar = db.CuentaXPagars.Find(id);
            if (cuentaXPagar == null)
            {
                return NotFound();
            }

            return Ok(cuentaXPagar);
        }

        // PUT: api/ApiCuentaXPagar/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCuentaXPagar(short id, CuentaXPagar cuentaXPagar)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cuentaXPagar.cp_empresa)
            {
                return BadRequest();
            }

            db.Entry(cuentaXPagar).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CuentaXPagarExists(id))
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

        // POST: api/ApiCuentaXPagar
        [ResponseType(typeof(CuentaXPagar))]
        public IHttpActionResult PostCuentaXPagar(CuentaXPagar cuentaXPagar)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CuentaXPagars.Add(cuentaXPagar);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CuentaXPagarExists(cuentaXPagar.cp_empresa))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = cuentaXPagar.cp_empresa }, cuentaXPagar);
        }

        // DELETE: api/ApiCuentaXPagar/5
        [ResponseType(typeof(CuentaXPagar))]
        public IHttpActionResult DeleteCuentaXPagar(short id)
        {
            CuentaXPagar cuentaXPagar = db.CuentaXPagars.Find(id);
            if (cuentaXPagar == null)
            {
                return NotFound();
            }

            db.CuentaXPagars.Remove(cuentaXPagar);
            db.SaveChanges();

            return Ok(cuentaXPagar);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CuentaXPagarExists(short id)
        {
            return db.CuentaXPagars.Count(e => e.cp_empresa == id) > 0;
        }
    }
}