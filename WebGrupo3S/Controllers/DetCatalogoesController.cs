using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Data.Entity.Core.Objects;
using System.Web.Mvc;
using WebGrupo3S.Models;
using WebGrupo3S.Helpers;

namespace WebGrupo3S.Views
{
    public class DetCatalogoesController : Controller
    {
        private SSS_COMPLEMENTOSEntities db = new SSS_COMPLEMENTOSEntities();
        private string myModulo = "DetCatalogos";
        private string myDat = "";
        public ConstantesP coP = new ConstantesP();
        public String tsp = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("cc_IdCatalogo", typeof(int));


        public class CatalogosSearchResultModel
        {
            public IEnumerable<sp_Busqueda_DetCatalogo_Result> CatalogosResults { get; set; }
        }
        // GET: DetCatalogoes
        public ActionResult Index()
        {
            var CatalogosSearch = new CatalogosSearchResultModel();
            try
            {
                myDat = "Listado de detalle de catalogos / sp_Busqueda_DetCatalogo";
                CatalogosSearch.CatalogosResults = db.sp_Busqueda_DetCatalogo(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, error).ToList();
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Lista"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Lista", myDat });
            }

            return View(CatalogosSearch.CatalogosResults.ToList());
        }

        // GET: DetCatalogoes/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DetCatalogo detCatalogo = db.DetCatalogoes.Find(id);
            if (detCatalogo == null)
            {
                return HttpNotFound();
            }
            return View(detCatalogo);
        }

        // GET: DetCatalogoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DetCatalogoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cd_empresa,cd_IdCatalogo,cd_codigo,cd_valor,cd_valor1,cd_valor2,cd_valor3,cd_fechaing,cd_fechamod,cd_usuarioing,cd_usuariomod,cd_maquinaing,cd_maquinamod,cd_estado,cd_timestamp")] DetCatalogo detCatalogo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    myDat = "Crea detalle de catalogo / sp_ABC_DetCatalogo";
                    detCatalogo.cd_fechaing = System.DateTime.Now;
                    detCatalogo.cd_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_DetCatalogo(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'),0, detCatalogo.cd_codigo, detCatalogo.cd_valor, detCatalogo.cd_valor1, detCatalogo.cd_valor2,detCatalogo.cd_valor3, detCatalogo.cd_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_DetCatalogo: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), 0, detCatalogo.cd_codigo, detCatalogo.cd_valor, detCatalogo.cd_valor1, detCatalogo.cd_valor2, detCatalogo.cd_valor3, detCatalogo.cd_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                    if (error.Value.ToString() == "")
                    {
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                        throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
                }
                catch (Exception ex)
                {
                    //                    return View("Error", new HandleErrorInfo(ex, myModulo, "Crear"));
                    return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea", myDat });
                }
            }

            return View(detCatalogo);
        }

        // GET: DetCatalogoes/Edit/5
        public ActionResult Edit(short? id, String cod)
        {
            DetCatalogo detCatalogo = new DetCatalogo();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                myDat = "Editar detalle de catalogo: " + id.ToString() + " - " + cod + " / sp_Busqueda_DetCatalogo";
                //if (Convert.ToBoolean(Session["editar"]) == true)
                //{
                ObjectResult resultado = db.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa),id, cod, null, null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, cod, null, null, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_DetCatalogo_Result re in resultado)
                {
                    detCatalogo = CargaCatalogo(re);
                }
                if (detCatalogo == null)
                    throw new System.InvalidOperationException("Error al obtener registro", new Exception(""));
                //}
                //else
                //    throw new System.InvalidOperationException("No tiene acceso a la opcion de actualización");
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Edit"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita detalle", myDat });
            }
            return View(detCatalogo);
        }

        private DetCatalogo CargaCatalogo(sp_Busqueda_DetCatalogo_Result parResult)
        {
            DetCatalogo ca = new DetCatalogo();

            ca.cd_empresa = parResult.Empresa;
            ca.cd_IdCatalogo = parResult.IdCatalogo;
            ca.cd_codigo = parResult.Codigo;
            ca.cd_valor = parResult.Valor;
            ca.cd_valor1 = parResult.Valor1;
            ca.cd_valor2 = parResult.Valor2;
            ca.cd_valor3 = parResult.Valor3;
            ca.cd_timestamp = parResult.TimeStamp;
            return ca;
        }

        // POST: DetCatalogoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cd_empresa,cd_IdCatalogo,cd_codigo,cd_valor,cd_valor1,cd_valor2,cd_valor3,cd_fechaing,cd_fechamod,cd_usuarioing,cd_usuariomod,cd_maquinaing,cd_maquinamod,cd_estado,cd_timestamp")] DetCatalogo detCatalogo)
        {
            try
            {
                myDat = "Edita detalle de catalogo / sp_ABC_DetCatalogo";
                tsp = Convert.ToBase64String(detCatalogo.cd_timestamp as byte[]);
                codigo.Value = detCatalogo.cd_IdCatalogo;
                detCatalogo.cd_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_DetCatalogo(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), detCatalogo.cd_IdCatalogo, detCatalogo.cd_codigo, detCatalogo.cd_valor, detCatalogo.cd_valor1, detCatalogo.cd_valor2, detCatalogo.cd_valor3, detCatalogo.cd_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_DetCatalogo: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), detCatalogo.cd_IdCatalogo, detCatalogo.cd_codigo, detCatalogo.cd_valor, detCatalogo.cd_valor1, detCatalogo.cd_valor2, detCatalogo.cd_valor3, detCatalogo.cd_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                }
                else
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Edit"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita detalle", myDat });
            }
            return View(detCatalogo);
        }

        // GET: DetCatalogoes/Delete/5
        public ActionResult Delete(short? id, String cod)
        {
            DetCatalogo detCatalogo = new DetCatalogo();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                myDat = "Dar de baja detalle de catalogo: " + id.ToString() + " - " + cod + " / sp_Busqueda_DetCatalogo";
                //if (Convert.ToBoolean(Session["debaja"]) == true)
                //{
                    ObjectResult resultado = db.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), id, cod,null, null, null, null, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, cod, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_DetCatalogo_Result re in resultado)
                    {
                        detCatalogo = CargaCatalogo(re);
                    }
                //}
                //else
                //    throw new System.InvalidOperationException("No tiene permitido dar de baja el registro");
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Dar de baja"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Debaja", myDat });
            }
            return View(detCatalogo);
        }

        // POST: DetCatalogoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id, string cod)
        {
            DetCatalogo catalogo = new DetCatalogo();
            try
            {
                myDat = "Confirmacion de baja detalle de catalogo: " + id.ToString() + " - " + cod + " / sp_Busqueda_DetCatalogo";
                ObjectResult resultado = db.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), id, cod, null, null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, cod, null, null, null, null, "-> R: " + validad.getResponse(error)));

                foreach (sp_Busqueda_DetCatalogo_Result re in resultado)
                {
                    catalogo = CargaCatalogo(re);
                    tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                }

                codigo.Value = catalogo.cd_IdCatalogo;
                int result = db.sp_ABC_DetCatalogo(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), id, cod, catalogo.cd_valor, catalogo.cd_valor1, catalogo.cd_valor2, catalogo.cd_valor3, catalogo.cd_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_DetCatalogo: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), id, cod, catalogo.cd_valor, catalogo.cd_valor1, catalogo.cd_valor2, catalogo.cd_valor3, catalogo.cd_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                }
                else
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));

            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Confirmar dar de baja"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Debaja", myDat });
            }
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
