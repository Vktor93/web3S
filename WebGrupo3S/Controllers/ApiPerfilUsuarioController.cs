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
    public class ApiPerfilUsuarioController : ApiController
    {
        private SSS_PERSONASEntities db = new SSS_PERSONASEntities();

        // GET: api/ApiPerfilUsuario
        public IQueryable<perfilusuario> Getperfilusuario()
        {
            return db.perfilusuario;
        }

        // GET: api/ApiPerfilUsuario/5
        [ResponseType(typeof(perfilusuario))]
        public IHttpActionResult Getperfilusuario(int id)
        {
            perfilusuario perfilusuario = db.perfilusuario.Find(id);
            if (perfilusuario == null)
            {
                return NotFound();
            }

            return Ok(perfilusuario);
        }

        // PUT: api/ApiPerfilUsuario/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putperfilusuario(int id, perfilusuario perfilusuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != perfilusuario.pu_empresa)
            {
                return BadRequest();
            }

            db.Entry(perfilusuario).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!perfilusuarioExists(id))
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

        // POST: api/ApiPerfilUsuario
        [ResponseType(typeof(perfilusuario))]
        public IHttpActionResult Postperfilusuario(perfilusuario perfilusuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.perfilusuario.Add(perfilusuario);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (perfilusuarioExists(perfilusuario.pu_empresa))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = perfilusuario.pu_empresa }, perfilusuario);
        }

        // DELETE: api/ApiPerfilUsuario/5
        [ResponseType(typeof(perfilusuario))]
        public IHttpActionResult Deleteperfilusuario(int id)
        {
            perfilusuario perfilusuario = db.perfilusuario.Find(id);
            if (perfilusuario == null)
            {
                return NotFound();
            }

            db.perfilusuario.Remove(perfilusuario);
            db.SaveChanges();

            return Ok(perfilusuario);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool perfilusuarioExists(int id)
        {
            return db.perfilusuario.Count(e => e.pu_empresa == id) > 0;
        }
    }
}