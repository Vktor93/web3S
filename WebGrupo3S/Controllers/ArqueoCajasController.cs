using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebGrupo3S.Models;
using System.Data.Entity.Core.Objects;
using WebGrupo3S.Helpers;

namespace WebGrupo3S.Controllers
{
    public class ArqueoCajasController : Controller
    {
        private SSS_OPERACIONEntities db = new SSS_OPERACIONEntities();
        private string myModulo = "Arqueo Caja";
        private string myDat = "";
        public ConstantesP coP = new ConstantesP();
        public String tsp = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("aq_IdArqueoCaja", typeof(int));

        CompleteString complete = new CompleteString();

        public class DatosSearchResultModel
        {
            public IEnumerable<sp_Busqueda_ArqueoCaja_Result> DatosResults { get; set; }
        }

        // GET: ArqueoCajas
        public ActionResult Index()
        {
            var DatosSearch = new DatosSearchResultModel();

            try
            {
                myDat = "Busqueda / sp_Busqueda_CierreCaja";
                DatosSearch.DatosResults = db.sp_Busqueda_ArqueoCaja(1, "",Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, null, error).ToList();
            }
            catch (Exception ex) {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", myModulo, opcion = "Lista", myDat });
            }

            return View(DatosSearch.DatosResults.ToList());
        }

        // GET: ArqueoCajas/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArqueoCaja arqueoCaja = db.ArqueoCaja.Find(id);
            if (arqueoCaja == null)
            {
                return HttpNotFound();
            }
            return View(arqueoCaja);
        }

        // GET: ArqueoCajas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ArqueoCajas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "aq_empresa,aq_IdCierreCaja,aq_IdArqueoCaja,aq_Moneda,aq_PresentacionMoneda,aq_Denominacion,aq_Cantidad,aq_MontoTotal,aq_fechaing,aq_fechamod,aq_usuarioing,aq_usuariomod,aq_maquinaing,aq_maquinamod,aq_estado,aq_timestamp")] ArqueoCaja arqueoCaja)
        {
            if (ModelState.IsValid)
            {
                db.ArqueoCaja.Add(arqueoCaja);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(arqueoCaja);
        }

        // GET: ArqueoCajas/Edit/5
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArqueoCaja arqueoCaja = db.ArqueoCaja.Find(id);
            if (arqueoCaja == null)
            {
                return HttpNotFound();
            }
            return View(arqueoCaja);
        }

        // POST: ArqueoCajas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "aq_empresa,aq_IdCierreCaja,aq_IdArqueoCaja,aq_Moneda,aq_PresentacionMoneda,aq_Denominacion,aq_Cantidad,aq_MontoTotal,aq_fechaing,aq_fechamod,aq_usuarioing,aq_usuariomod,aq_maquinaing,aq_maquinamod,aq_estado,aq_timestamp")] ArqueoCaja arqueoCaja)
        {
            if (ModelState.IsValid)
            {
                db.Entry(arqueoCaja).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(arqueoCaja);
        }

        // GET: ArqueoCajas/Delete/5
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArqueoCaja arqueoCaja = db.ArqueoCaja.Find(id);
            if (arqueoCaja == null)
            {
                return HttpNotFound();
            }
            return View(arqueoCaja);
        }

        // POST: ArqueoCajas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            ArqueoCaja arqueoCaja = db.ArqueoCaja.Find(id);
            db.ArqueoCaja.Remove(arqueoCaja);
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
