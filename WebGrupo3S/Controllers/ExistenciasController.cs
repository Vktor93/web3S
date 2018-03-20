using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Core.Objects;
using WebGrupo3S.Models;
using WebGrupo3S.Helpers;

namespace WebGrupo3S.Views
{
    public class ExistenciasController : Controller
    {
        private SSS_OPERACIONEntities db = new SSS_OPERACIONEntities();
        private string myModulo = "Existencias";
        private string myDat = "";
        public ConstantesP coP = new ConstantesP();
        public String tsp = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("ex_IdExistencia", typeof(int));

        public class DatosSearchResultModel
        {
            public IEnumerable<sp_Busqueda_Existencia_Result> DatosResults { get; set; }
        }
        // GET: Existencias
        public ActionResult Index()
        {
            var DatosSearch = new DatosSearchResultModel();
            try
            {
                myDat = "Listado de existencias / sp_Busqueda_Existencia";
                ViewBag.reporte = Session[myModulo].ToString().Substring(19, 1);
                DatosSearch.DatosResults = db.sp_Busqueda_Existencia(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, null, error).ToList();
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Existencia: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Lista"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Lista", myDat });
            }

            return View(DatosSearch.DatosResults.ToList());
        }

        // GET: Existencias/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Existencia existencia = db.Existencia.Find(id);
            if (existencia == null)
            {
                return HttpNotFound();
            }
            return View(existencia);
        }

        // GET: Existencias/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Existencias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ex_empresa,ex_IdSucursal,ex_IdExistencia,ex_TipoProducto,ex_IdProd,ex_cantidad,ex_precioUnitario,ex_FechaCaducidad,ex_fechaing,ex_fechamod,ex_usuarioing,ex_usuariomod,ex_maquinaing,ex_maquinamod,ex_estado,ex_timestamp")] Existencia existencia)
        {
            if (ModelState.IsValid)
            {
                db.Existencia.Add(existencia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(existencia);
        }

        // GET: Existencias/Edit/5
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Existencia existencia = db.Existencia.Find(id);
            if (existencia == null)
            {
                return HttpNotFound();
            }
            return View(existencia);
        }

        // POST: Existencias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ex_empresa,ex_IdSucursal,ex_IdExistencia,ex_TipoProducto,ex_IdProd,ex_cantidad,ex_precioUnitario,ex_FechaCaducidad,ex_fechaing,ex_fechamod,ex_usuarioing,ex_usuariomod,ex_maquinaing,ex_maquinamod,ex_estado,ex_timestamp")] Existencia existencia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(existencia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(existencia);
        }

        // GET: Existencias/Delete/5
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Existencia existencia = db.Existencia.Find(id);
            if (existencia == null)
            {
                return HttpNotFound();
            }
            return View(existencia);
        }

        // POST: Existencias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            Existencia existencia = db.Existencia.Find(id);
            db.Existencia.Remove(existencia);
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
