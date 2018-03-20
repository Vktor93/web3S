using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebGrupo3S.Models;

namespace WebGrupo3S.Views
{
    public class MovimientoCuentaXCobrarsController : Controller
    {
        private SSS_OPERACIONEntities db = new SSS_OPERACIONEntities();

        // GET: MovimientoCuentaXCobrars
        public ActionResult Index()
        {
            return View(db.MovimientoCuentaXCobrar.ToList());
        }

        // GET: MovimientoCuentaXCobrars/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovimientoCuentaXCobrar movimientoCuentaXCobrar = db.MovimientoCuentaXCobrar.Find(id);
            if (movimientoCuentaXCobrar == null)
            {
                return HttpNotFound();
            }
            return View(movimientoCuentaXCobrar);
        }

        // GET: MovimientoCuentaXCobrars/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MovimientoCuentaXCobrars/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "mc_empresa,mc_IdCuentaXCobrar,mc_IdMovCuentaXCobrar,mc_IdSucursal,mc_IdDoc,mc_Cliente,mc_FechaMov,mc_MontoMov,mc_CreditoDebito,mc_Descripcion,mc_estMovCuentaXCobrar,mc_fechaing,mc_fechamod,mc_usuarioing,mc_usuariomod,mc_maquinaing,mc_maquinamod,mc_estado,mc_timestamp")] MovimientoCuentaXCobrar movimientoCuentaXCobrar)
        {
            if (ModelState.IsValid)
            {
                db.MovimientoCuentaXCobrar.Add(movimientoCuentaXCobrar);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(movimientoCuentaXCobrar);
        }

        // GET: MovimientoCuentaXCobrars/Edit/5
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovimientoCuentaXCobrar movimientoCuentaXCobrar = db.MovimientoCuentaXCobrar.Find(id);
            if (movimientoCuentaXCobrar == null)
            {
                return HttpNotFound();
            }
            return View(movimientoCuentaXCobrar);
        }

        // POST: MovimientoCuentaXCobrars/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "mc_empresa,mc_IdCuentaXCobrar,mc_IdMovCuentaXCobrar,mc_IdSucursal,mc_IdDoc,mc_Cliente,mc_FechaMov,mc_MontoMov,mc_CreditoDebito,mc_Descripcion,mc_estMovCuentaXCobrar,mc_fechaing,mc_fechamod,mc_usuarioing,mc_usuariomod,mc_maquinaing,mc_maquinamod,mc_estado,mc_timestamp")] MovimientoCuentaXCobrar movimientoCuentaXCobrar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movimientoCuentaXCobrar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movimientoCuentaXCobrar);
        }

        // GET: MovimientoCuentaXCobrars/Delete/5
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovimientoCuentaXCobrar movimientoCuentaXCobrar = db.MovimientoCuentaXCobrar.Find(id);
            if (movimientoCuentaXCobrar == null)
            {
                return HttpNotFound();
            }
            return View(movimientoCuentaXCobrar);
        }

        // POST: MovimientoCuentaXCobrars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            MovimientoCuentaXCobrar movimientoCuentaXCobrar = db.MovimientoCuentaXCobrar.Find(id);
            db.MovimientoCuentaXCobrar.Remove(movimientoCuentaXCobrar);
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
