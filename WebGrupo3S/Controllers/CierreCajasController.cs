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
    public class CierreCajasController : Controller
    {
        private SSS_OPERACIONEntities db = new SSS_OPERACIONEntities();

        // GET: CierreCajas
        public ActionResult Index()
        {
            return View(db.CierreCaja.ToList());
        }

        // GET: CierreCajas/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CierreCaja cierreCaja = db.CierreCaja.Find(id);
            if (cierreCaja == null)
            {
                return HttpNotFound();
            }
            return View(cierreCaja);
        }

        // GET: CierreCajas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CierreCajas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cj_empresa,cj_IdCierreCaja,cj_Terminal,cj_IdSucursal,cj_FechaHoraCierre,cj_Moneda,cj_SaldoAnterior,cj_EntradasEfectivo,cj_SalidasEfectivo,cj_EfectivoTeorico,cj_EfectivoFisico,cj_AjusteEfectivo,cj_ChequeTeorico,cj_ChequeFisico,cj_AjusteCheque,cj_TarjetaTeorico,cj_TarjetaFisico,cj_AjusteTarjeta,cj_Descuadre,cj_RetiroEfectivoAdmin,cj_SaldoActual,cj_estCierreCaja,cj_TotalVenta,cj_TotalPropina,cj_fechaing,cj_fechamod,cj_usuarioing,cj_usuariomod,cj_maquinaing,cj_maquinamod,cj_estado,cj_timestamp")] CierreCaja cierreCaja)
        {
            if (ModelState.IsValid)
            {
                db.CierreCaja.Add(cierreCaja);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cierreCaja);
        }

        // GET: CierreCajas/Edit/5
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CierreCaja cierreCaja = db.CierreCaja.Find(id);
            if (cierreCaja == null)
            {
                return HttpNotFound();
            }
            return View(cierreCaja);
        }

        // POST: CierreCajas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cj_empresa,cj_IdCierreCaja,cj_Terminal,cj_IdSucursal,cj_FechaHoraCierre,cj_Moneda,cj_SaldoAnterior,cj_EntradasEfectivo,cj_SalidasEfectivo,cj_EfectivoTeorico,cj_EfectivoFisico,cj_AjusteEfectivo,cj_ChequeTeorico,cj_ChequeFisico,cj_AjusteCheque,cj_TarjetaTeorico,cj_TarjetaFisico,cj_AjusteTarjeta,cj_Descuadre,cj_RetiroEfectivoAdmin,cj_SaldoActual,cj_estCierreCaja,cj_TotalVenta,cj_TotalPropina,cj_fechaing,cj_fechamod,cj_usuarioing,cj_usuariomod,cj_maquinaing,cj_maquinamod,cj_estado,cj_timestamp")] CierreCaja cierreCaja)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cierreCaja).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cierreCaja);
        }

        // GET: CierreCajas/Delete/5
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CierreCaja cierreCaja = db.CierreCaja.Find(id);
            if (cierreCaja == null)
            {
                return HttpNotFound();
            }
            return View(cierreCaja);
        }

        // POST: CierreCajas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            CierreCaja cierreCaja = db.CierreCaja.Find(id);
            db.CierreCaja.Remove(cierreCaja);
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
