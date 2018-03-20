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
using Microsoft.Reporting.WebForms;
using System.IO;
using WebGrupo3S.Helpers;

namespace WebGrupo3S.Views
{
    public class TServiciosController : Controller
    {
        private SSS_OPERACIONEntities db = new SSS_OPERACIONEntities();
        private SSS_COMPLEMENTOSEntities dbC = new SSS_COMPLEMENTOSEntities();

        private string myModulo = "TipoServicios";
        private string myDat = "";
        private string mycatalogo = "Tipo servicios";
        public ConstantesP coP = new ConstantesP();
        public String tsp = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("ts_IdTipoServ", typeof(int));

        public class DatosSearchResultModel
        {
            public IEnumerable<sp_Busqueda_TipoServicio_Result> DatosResults { get; set; }
        }
        // GET: TServicios
        public ActionResult Index()
        {
            validad ac = new validad();
            var DatosSearch = new DatosSearchResultModel();
            try
            {
                ac.seteAccesos(mycatalogo, Session["perfil"].ToString());
                Session[myModulo] = ac.acc;
                if (Session["UserId"].ToString() != "")
                {
                    if (Session[myModulo].ToString().Substring(0, 1) == "1")
                    {
                        myDat = "Busqueda de Tipos de servicios / sp_Busqueda_TipoServicio";
                        buscaCatalogos();
                        ViewBag.nuevo = Session[myModulo].ToString().Substring(1, 1);
                        ViewBag.editar = Session[myModulo].ToString().Substring(2, 1);
                        ViewBag.debaja = Session[myModulo].ToString().Substring(3, 1);
                        ViewBag.reporte = Session[myModulo].ToString().Substring(19, 1);
                        DatosSearch.DatosResults = db.sp_Busqueda_TipoServicio(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, error).ToList();
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_TipoServicio: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    }
                    else
                        throw new System.InvalidOperationException("-Ingreso a opción denegado!-", new Exception(""));
                }
                else throw new System.InvalidOperationException("-Ingrese al Sistema para poder Ingresar a esta Opción-", new Exception(""));                
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Lista", myDat });
            }

            return View(DatosSearch.DatosResults.ToList());
        }

        public void buscaCatalogos()
        {
            try
            {
                Session["CO"] = 0;

                ObjectResult resultadoT = dbC.sp_Busqueda_CabCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), null, "Coordinaciones", null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_CabCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, "Coordinaciones", null, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_CabCatalogo_Result re in resultadoT)
                {
                    Session["CO"] = re.IdCatalogo;
                }
            }
            catch (Exception ex)
            {

            }

        }

        // GET: TServicios/Create
        public ActionResult Create()
        {
            try
            {
                myDat = "Agrega nuevo tipo de servicio";
                if (Session[myModulo].ToString().Substring(1, 1) == "0")
                    throw new System.InvalidOperationException("-No tiene acceso a crear nuevo registros-", new Exception(""));
                else
                {
                    ViewBag.CO = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["CO"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["CO"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea", myDat });
            }
            return View();
        }

        // POST: TServicios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ts_empresa,ts_IdTipoServicio,ts_nombre,ts_descripcion,ts_padre,ts_Coordinacion,ts_CuentaSAP,ts_fechaing,ts_fechamod,ts_usuarioing,ts_usuariomod,ts_maquinaing,ts_maquinamod,ts_estado,ts_timestamp")] TipoServicio tipoServicio)
        {
            try
            {
                myDat = "Crea nuevo tipo de servicio / sp_ABC_TipoServicio";
                if (ModelState.IsValid)
                {
                    tipoServicio.ts_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_TipoServicio(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo, tipoServicio.ts_nombre,
                                                   tipoServicio.ts_descripcion, tipoServicio.ts_padre, tipoServicio.ts_Coordinacion, tipoServicio.ts_CuentaSAP, tipoServicio.ts_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_TipoServicio: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo.Value, tipoServicio.ts_nombre,
                                                   tipoServicio.ts_descripcion, tipoServicio.ts_padre, tipoServicio.ts_Coordinacion, tipoServicio.ts_CuentaSAP, tipoServicio.ts_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                    if (error.Value.ToString() == "")
                    {
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea", myDat });
            }

            return View(tipoServicio);
        }

        // GET: TServicios/Edit/5
        public ActionResult Edit(short? id)
        {
            TipoServicio tipo = new TipoServicio();
            if (id == null)
            {
                return View("Error");
            }

            try
            {
                myDat = "Actualizar tipo de servicio: " + id.ToString();
                if (Session[myModulo].ToString().Substring(2, 1) == "1")
                {
                    ViewBag.CO = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["CO"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["CO"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    tipo = db.TipoServicio.Where(a => a.ts_IdTipoServicio == id).First();
                    if (tipo == null)
                    {
                        return View("Error");
                    }
                }
                else
                    throw new System.InvalidOperationException("-No tiene acceso a esta opción-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
            }
            return View(tipo);
        }

        // POST: TServicios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ts_empresa,ts_IdTipoServicio,ts_nombre,ts_descripcion,ts_padre,ts_Coordinacion,ts_CuentaSAP,ts_fechaing,ts_fechamod,ts_usuarioing,ts_usuariomod,ts_maquinaing,ts_maquinamod,ts_estado,ts_timestamp")] TipoServicio tipoServicio)
        {
            try
            {
                myDat = "Actualizar datos / sp_ABC_TipoServicio";
                tsp = Convert.ToBase64String(tipoServicio.ts_timestamp as byte[]);
                codigo.Value = tipoServicio.ts_IdTipoServicio;
                int result = db.sp_ABC_TipoServicio(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo, tipoServicio.ts_nombre,
                                                   tipoServicio.ts_descripcion, tipoServicio.ts_padre, tipoServicio.ts_Coordinacion, tipoServicio.ts_CuentaSAP, tipoServicio.ts_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_TipoServicio: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo.Value, tipoServicio.ts_nombre,
                                                   tipoServicio.ts_descripcion, tipoServicio.ts_padre, tipoServicio.ts_Coordinacion, tipoServicio.ts_CuentaSAP, tipoServicio.ts_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                }
                else
                {
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
                }
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Actualizar datos"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
            }
            return RedirectToAction("Index");
        }

        // GET: TServicios/Delete/5
        public ActionResult Delete(short? id)
        {
            TipoServicio tipo = new TipoServicio();
            if (id == null)
            {
                return View("Error");
            }

            try
            {
                myDat = "Dar de baja al tipo de servicio: " + id.ToString();
                if (Session[myModulo].ToString().Substring(3, 1) == "1")
                {
                    tipo = db.TipoServicio.Where(a => a.ts_IdTipoServicio == id).First();
                    if (tipo == null)
                    {
                        return View("Error");
                    }
                }
                else
                    throw new System.InvalidOperationException("-No tiene permitido dar de baja el registro-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "DeBaja", myDat });
            }
            return View(tipo);
        }

        // POST: TServicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            TipoServicio tipo = new TipoServicio();
            try
            {
                myDat = "Confirmacion dar de baja tipo de servicio: " + id.ToString() + " / sp_ABC_TipoServicio";
                tipo = db.TipoServicio.Where(a => a.ts_IdTipoServicio == id).First();
                tsp = Convert.ToBase64String(tipo.ts_timestamp as byte[]);
                tipo.ts_usuarioing = Session["UserName"].ToString();
                codigo.Value = id;
                int result = db.sp_ABC_TipoServicio(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo, null, null, null, null, null, tipo.ts_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_TipoServicio: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo.Value, null, null, null, null, null, tipo.ts_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                }
                else
                {
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
                }
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Confirma dar de baja"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "DeBaja", myDat });
            }
            return RedirectToAction("Index");
        }

        public ActionResult Report(string id)
        {
            try
            {
                if (Session[myModulo].ToString().Substring(19, 1) == "1")
                {
                    LocalReport lr = new LocalReport();
                    string path = Path.Combine(Server.MapPath("~/Reports"), "TipoServicio.rdlc");
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        return View("Index");
                    }
                    List<TipoServicio> cm = new List<TipoServicio>();
                    using (SSS_OPERACIONEntities dc = new Models.SSS_OPERACIONEntities())
                    {
                        cm = dc.TipoServicio.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DTServicios", cm);
                    lr.DataSources.Add(rd);
                    string reportType = id;
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    ReportParameter p1 = new ReportParameter("prUsuario", Session["UserName"].ToString());
                    ReportParameter p2 = new ReportParameter("prMaquina", Environment.MachineName.ToString());
                    lr.SetParameters(new ReportParameter[] { p1, p2 });
                    string deviceInfo =

                    "<DeviceInfo>" +
                    " <OutputFormat>" + id + "</OutputFormat>" +
                    "</DeviceInfo>";

                    Warning[] warnings;
                    string[] streams;


                    myDat = "Reporte de tipos de servicios";
                    byte[] renderedBytes = lr.Render(
                                                    reportType,
                                                    deviceInfo,
                                                    out mimeType,
                                                    out encoding,
                                                    out fileNameExtension,
                                                    out streams,
                                                    out warnings);
                    return File(renderedBytes, mimeType);
                }
                else throw new System.InvalidOperationException("-No tiene permiso para generar reporte!-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Reporte", myDat });
            }
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
