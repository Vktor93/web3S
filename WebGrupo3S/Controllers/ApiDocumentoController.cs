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
    public class ApiDocumentoController : ApiController
    {
        private SSS_OPERACIONEntities db = new SSS_OPERACIONEntities();

        // GET: api/ApiDocumento
        public IQueryable<Documento> GetDocumento()
        {
            return db.Documento;
        }

        // GET: api/ApiDocumento/5
        [ResponseType(typeof(Documento))]
        public IHttpActionResult GetDocumento(short id)
        {
            Documento documento = db.Documento.Find(id);
            if (documento == null)
            {
                return NotFound();
            }

            return Ok(documento);
        }

        // PUT: api/ApiDocumento/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDocumento(short id, Documento documento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != documento.dm_empresa)
            {
                return BadRequest();
            }

            db.Entry(documento).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentoExists(id))
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

        // POST: api/ApiDocumento
        [ResponseType(typeof(Documento))]
        public IHttpActionResult PostDocumento(Documento documento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Documento.Add(documento);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (DocumentoExists(documento.dm_empresa))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = documento.dm_empresa }, documento);
        }

        // DELETE: api/ApiDocumento/5
        [ResponseType(typeof(Documento))]
        public IHttpActionResult DeleteDocumento(short id)
        {
            Documento documento = db.Documento.Find(id);
            if (documento == null)
            {
                return NotFound();
            }

            db.Documento.Remove(documento);
            db.SaveChanges();

            return Ok(documento);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DocumentoExists(short id)
        {
            return db.Documento.Count(e => e.dm_empresa == id) > 0;
        }
    }
}