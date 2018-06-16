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
    public class ApiHorarioController : ApiController
    {
        private SSS_PLANILLAEntities db = new SSS_PLANILLAEntities();

        // GET: api/ApiHorario
        public IQueryable<Horario> GetHorarios()
        {
            return db.Horarios;
        }

        // GET: api/ApiHorario/5
        [ResponseType(typeof(Horario))]
        public IHttpActionResult GetHorario(short id)
        {
            Horario horario = db.Horarios.Find(id);
            if (horario == null)
            {
                return NotFound();
            }

            return Ok(horario);
        }

        // PUT: api/ApiHorario/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutHorario(short id, Horario horario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != horario.ho_empresa)
            {
                return BadRequest();
            }

            db.Entry(horario).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HorarioExists(id))
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

        // POST: api/ApiHorario
        [ResponseType(typeof(Horario))]
        public IHttpActionResult PostHorario(Horario horario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Horarios.Add(horario);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (HorarioExists(horario.ho_empresa))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = horario.ho_empresa }, horario);
        }

        // DELETE: api/ApiHorario/5
        [ResponseType(typeof(Horario))]
        public IHttpActionResult DeleteHorario(short id)
        {
            Horario horario = db.Horarios.Find(id);
            if (horario == null)
            {
                return NotFound();
            }

            db.Horarios.Remove(horario);
            db.SaveChanges();

            return Ok(horario);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HorarioExists(short id)
        {
            return db.Horarios.Count(e => e.ho_empresa == id) > 0;
        }
    }
}