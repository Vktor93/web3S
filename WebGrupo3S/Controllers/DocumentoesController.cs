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
    public class DocumentoesController : Controller
    {
        private SSS_OPERACIONEntities db = new SSS_OPERACIONEntities();
        private SSS_COMPLEMENTOSEntities dbC = new SSS_COMPLEMENTOSEntities();
        private string myModulo = "Documentos";
        private string myDat = "";
        public ConstantesP coP = new ConstantesP();
        public String tsp = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("dm_IdDoc", typeof(int));

        public class DatosSearchResultModel
        {
            public IEnumerable<sp_Busqueda_Documento_Result> DatosResults { get; set; }
        }

        //POST: Carga Cuerpo de Documento 
        [HttpPost]       
        public ActionResult CargaDoc()
        {

            return PartialView("BodyDoc");
        }

        // GET: Documentoes
        public ActionResult Index()
        {
            var DatosSearch = new DatosSearchResultModel();
            try
            {
                myDat = "Lista de documentos / sp_Busqueda_Documento";
                buscaCatalogos();
                ViewBag.TD = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["TD"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                DatosSearch.DatosResults = db.sp_Busqueda_Documento(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, null, "CL", null, null, null, null,null,null,null,null,null,null,null,null,null,null, error).ToList();
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Documento: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, null, "CL", null, null, null, null, null, null, null, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Lista"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Lista", myDat });
            }

            return View(DatosSearch.DatosResults.ToList());
        }

        public void buscaCatalogos()
        {
            Session["TD"] = 0;

            ObjectResult resultadoT = dbC.sp_Busqueda_CabCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), null, "TipoDocumento", null, null, null, error);
            foreach (sp_Busqueda_CabCatalogo_Result re in resultadoT)
            {
                Session["TD"] = re.IdCatalogo;
            }
        }

        // GET: Documentoes/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Documento documento = db.Documento.Find(id);
            if (documento == null)
            {
                return HttpNotFound();
            }
            return View(documento);
        }

        // GET: Documentoes/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: Documentoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "dm_empresa,dm_IdSucursal,dm_IdDoc,dm_TipoDocumento,dm_TipoAsociado,dm_asociado,dm_NombreAsociado,dm_DireccionAsociado,dm_Nit,dm_subtotal,dm_descuento,dm_total,dm_SerieDoc,dm_NoDocumento,dm_fechaDoc,dm_EstDocumento,dm_Terminal,dm_MotivoAnulacion,dm_MotivoNCND,dm_Comentario,dm_IdDocAnulado,dm_IdCierreDiario,dm_fechaing,dm_fechamod,dm_usuarioing,dm_usuariomod,dm_maquinaing,dm_maquinamod,dm_estado,dm_timestamp")] Documento documento)
        {
            if (ModelState.IsValid)
            {
                db.Documento.Add(documento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(documento);
        }

        // GET: Documentoes/Edit/5
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Documento documento = db.Documento.Find(id);
            if (documento == null)
            {
                return HttpNotFound();
            }
            return View(documento);
        }

        // POST: Documentoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "dm_empresa,dm_IdSucursal,dm_IdDoc,dm_TipoDocumento,dm_TipoAsociado,dm_asociado,dm_NombreAsociado,dm_DireccionAsociado,dm_Nit,dm_subtotal,dm_descuento,dm_total,dm_SerieDoc,dm_NoDocumento,dm_fechaDoc,dm_EstDocumento,dm_Terminal,dm_MotivoAnulacion,dm_MotivoNCND,dm_Comentario,dm_IdDocAnulado,dm_IdCierreDiario,dm_fechaing,dm_fechamod,dm_usuarioing,dm_usuariomod,dm_maquinaing,dm_maquinamod,dm_estado,dm_timestamp")] Documento documento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(documento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(documento);
        }

        // GET: Documentoes/Delete/5
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Documento documento = db.Documento.Find(id);
            if (documento == null)
            {
                return HttpNotFound();
            }
            return View(documento);
        }

        // POST: Documentoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            Documento documento = db.Documento.Find(id);
            db.Documento.Remove(documento);
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
