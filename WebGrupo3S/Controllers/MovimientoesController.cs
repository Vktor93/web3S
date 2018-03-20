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

namespace WebGrupo3S.Views
{
    public class MovimientoesController : Controller
    {
        private SSS_OPERACIONEntities db = new SSS_OPERACIONEntities();
        private SSS_COMPLEMENTOSEntities dbC = new SSS_COMPLEMENTOSEntities();
        private string myModulo = "Caja";
        private string myDat = "";
        public ConstantesP coP = new ConstantesP();
        public String tsp = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("mv_IdMov", typeof(int));
        public class DatosSearchResultModel
        {
            public IEnumerable<sp_busqueda_movimiento_result> DatosResults { get; set; }
        }


        // GET: Movimientoes
        public ActionResult Index()
        {
            var DatosSearch = new DatosSearchResultModel();
            try
            {
                myDat = "Lista de movimientos / sp_Busqueda_DetCatalogo";
                buscaCatalogos();
                ViewBag.TD = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["TD"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["TD"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                ViewBag.TM = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["TM"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["TM"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                //   DatosSearch.DatosResults = db.sp_Busqueda_Movimiento(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, null, null, null, null, null, null, null, null, null, null, error).ToList();
                return View(db.Movimiento.ToList());
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Lista"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Lista", myDat });
            }

           // return View(DatosSearch.DatosResults.ToList());
        }

        public void buscaCatalogos()
        {
            Session["TD"] = 0;
            Session["TM"] = 0;

            ObjectResult resultadoT = dbC.sp_Busqueda_CabCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), null, "TipoDocumento", null, null, null, error);
            WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_CabCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, "TipoDocumento", null, null, null, "-> R: " + validad.getResponse(error)));
            foreach (sp_Busqueda_CabCatalogo_Result re in resultadoT)
            {
                Session["TD"] = re.IdCatalogo;
            }
            ObjectResult resultadoM = dbC.sp_Busqueda_CabCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), null, "TipoMovimiento", null, null, null, error);
            WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_CabCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, "TipoMovimiento", null, null, null, "-> R: " + validad.getResponse(error)));
            foreach (sp_Busqueda_CabCatalogo_Result re in resultadoM)
            {
                Session["TM"] = re.IdCatalogo;
            }
        }

        // GET: Movimientoes/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movimiento movimiento = db.Movimiento.Find(id);
            if (movimiento == null)
            {
                return HttpNotFound();
            }
            return View(movimiento);
        }

        // GET: Movimientoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Movimientoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "mv_empresa,mv_IdSucursal,mv_IdMov,mv_ProdServ,mv_IdProdServ,mv_Cantidad,mv_ValorMov,mv_TipoMovimiento,mv_FechaMov,mv_IdDoc,mv_IdOrdenServicio,mv_IdDetalleOrdenServicio,mv_IdAutorizacion,mv_fechaing,mv_fechamod,mv_usuarioing,mv_usuariomod,mv_maquinaing,mv_maquinamod,mv_estado,mv_timestamp")] Movimiento movimiento)
        {
            if (ModelState.IsValid)
            {
                db.Movimiento.Add(movimiento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(movimiento);
        }

        // GET: Movimientoes/Edit/5
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movimiento movimiento = db.Movimiento.Find(id);
            if (movimiento == null)
            {
                return HttpNotFound();
            }
            return View(movimiento);
        }

        // POST: Movimientoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "mv_empresa,mv_IdSucursal,mv_IdMov,mv_ProdServ,mv_IdProdServ,mv_Cantidad,mv_ValorMov,mv_TipoMovimiento,mv_FechaMov,mv_IdDoc,mv_IdOrdenServicio,mv_IdDetalleOrdenServicio,mv_IdAutorizacion,mv_fechaing,mv_fechamod,mv_usuarioing,mv_usuariomod,mv_maquinaing,mv_maquinamod,mv_estado,mv_timestamp")] Movimiento movimiento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movimiento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movimiento);
        }

        // GET: Movimientoes/Delete/5
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movimiento movimiento = db.Movimiento.Find(id);
            if (movimiento == null)
            {
                return HttpNotFound();
            }
            return View(movimiento);
        }

        // POST: Movimientoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            Movimiento movimiento = db.Movimiento.Find(id);
            db.Movimiento.Remove(movimiento);
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
