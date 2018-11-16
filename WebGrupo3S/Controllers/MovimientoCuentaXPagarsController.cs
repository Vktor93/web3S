using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebGrupo3S.Models;

namespace WebGrupo3S.Controllers
{
    public class MovimientoCuentaXPagarsController : Controller
    {
        private SSS_OPERACIONEntities db = new SSS_OPERACIONEntities();

        // GET: MovimientoCuentaXPagars
        public ActionResult Index()
        {
            return View(db.MovimientoCuentaXPagars.ToList());
        }

        // GET: MovimientoCuentaXPagars/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovimientoCuentaXPagar movimientoCuentaXPagar = db.MovimientoCuentaXPagars.Find(id);
            if (movimientoCuentaXPagar == null)
            {
                return HttpNotFound();
            }
            return View(movimientoCuentaXPagar);
        }

        // GET: MovimientoCuentaXPagars/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MovimientoCuentaXPagars/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "mp_empresa,mp_IdCuentaXPagar,mp_IdMovCuentaXPagar,mp_IdSucursal,mp_IdDoc,mp_Proveedor,mp_FechaMov,mp_MontoMov,mp_CreditoDebito,mp_Descripcion,mp_estMovCuentaXPagar,mp_fechaing,mp_fechamod,mp_usuarioing,mp_usuariomod,mp_maquinaing,mp_maquinamod,mp_estado,mp_timestamp")] MovimientoCuentaXPagar movimientoCuentaXPagar)
        {
            if (ModelState.IsValid)
            {
                db.MovimientoCuentaXPagars.Add(movimientoCuentaXPagar);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(movimientoCuentaXPagar);
        }

        // GET: MovimientoCuentaXPagars/Edit/5
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovimientoCuentaXPagar movimientoCuentaXPagar = db.MovimientoCuentaXPagars.Find(id);
            if (movimientoCuentaXPagar == null)
            {
                return HttpNotFound();
            }
            return View(movimientoCuentaXPagar);
        }

        // POST: MovimientoCuentaXPagars/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "mp_empresa,mp_IdCuentaXPagar,mp_IdMovCuentaXPagar,mp_IdSucursal,mp_IdDoc,mp_Proveedor,mp_FechaMov,mp_MontoMov,mp_CreditoDebito,mp_Descripcion,mp_estMovCuentaXPagar,mp_fechaing,mp_fechamod,mp_usuarioing,mp_usuariomod,mp_maquinaing,mp_maquinamod,mp_estado,mp_timestamp")] MovimientoCuentaXPagar movimientoCuentaXPagar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movimientoCuentaXPagar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movimientoCuentaXPagar);
        }

        // GET: MovimientoCuentaXPagars/Delete/5
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovimientoCuentaXPagar movimientoCuentaXPagar = db.MovimientoCuentaXPagars.Find(id);
            if (movimientoCuentaXPagar == null)
            {
                return HttpNotFound();
            }
            return View(movimientoCuentaXPagar);
        }

        // POST: MovimientoCuentaXPagars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            MovimientoCuentaXPagar movimientoCuentaXPagar = db.MovimientoCuentaXPagars.Find(id);
            db.MovimientoCuentaXPagars.Remove(movimientoCuentaXPagar);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
