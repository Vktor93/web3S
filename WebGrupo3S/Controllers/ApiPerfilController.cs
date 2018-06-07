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
    public class ApiPerfilController : ApiController
    {
        private SSS_PERSONASEntities db = new SSS_PERSONASEntities();

        // GET: api/ApiPerfil
        public IQueryable<perfil> Getperfil()
        {
            return db.perfil;
        }

        // GET: api/ApiPerfil/5
        [Route("api/ApiPerfil/{id}/{id2}")]
        [ResponseType(typeof(perfil))]
        public IHttpActionResult Getperfil(int id, int id2)
        {
            perfil perfil = db.perfil.Find(id, id2);
            if (perfil == null)
            {
                return NotFound();
            }

            return Ok(perfil);
        }

        // PUT: api/ApiPerfil/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putperfil(int id, perfil perfil)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != perfil.pf_empresa)
            {
                return BadRequest();
            }

            db.Entry(perfil).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!perfilExists(id))
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

        // POST: api/ApiPerfil
        [ResponseType(typeof(perfil))]
        public IHttpActionResult Postperfil(perfil perfil)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.perfil.Add(perfil);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (perfilExists(perfil.pf_empresa))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = perfil.pf_empresa }, perfil);
        }

        // DELETE: api/ApiPerfil/5
        [ResponseType(typeof(perfil))]
        public IHttpActionResult Deleteperfil(int id)
        {
            perfil perfil = db.perfil.Find(id);
            if (perfil == null)
            {
                return NotFound();
            }

            db.perfil.Remove(perfil);
            db.SaveChanges();

            return Ok(perfil);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool perfilExists(int id)
        {
            return db.perfil.Count(e => e.pf_empresa == id) > 0;
        }
    }
}