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
    public class TProductoesController : Controller
    {
        private SSS_OPERACIONEntities db = new SSS_OPERACIONEntities();
        private SSS_COMPLEMENTOSEntities dbC = new SSS_COMPLEMENTOSEntities();
        private string myModulo = "TipoProductos";
        private string mycatalogo = "Tipo Producto";
        private string myDat = "";
        public ConstantesP coP = new ConstantesP();
        public String tsp = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("tp_IdTipoProducto", typeof(int));

        public class DatosSearchResultModel
        {
            public IEnumerable<sp_Busqueda_TipoProducto_Result> DatosResults { get; set; }
        }
        // GET: TProductoes
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
                        myDat = "Busqueda de tipos de productos / sp_Busqueda_TipoProducto";
                        buscaCatalogos();
                        ViewBag.nuevo = Session[myModulo].ToString().Substring(1, 1);
                        ViewBag.editar = Session[myModulo].ToString().Substring(2, 1);
                        ViewBag.debaja = Session[myModulo].ToString().Substring(3, 1);
                        ViewBag.reporte = Session[myModulo].ToString().Substring(19, 1);

                        DatosSearch.DatosResults = db.sp_Busqueda_TipoProducto(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, error).ToList();
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_TipoProducto: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    }
                    else
                        throw new System.InvalidOperationException("-Ingreso a opción denegado!-", new Exception(""));
                }
                else
                    throw new System.InvalidOperationException("-Ingrese al Sistema para poder Ingresar a esta Opción-", new Exception(""));
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
                Session["CT"] = 0;

                ObjectResult resultadoT = dbC.sp_Busqueda_CabCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), null, "ClasificacionTP", null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_CabCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, "ClasificacionTP", null, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_CabCatalogo_Result re in resultadoT)
                {
                    Session["CT"] = re.IdCatalogo;
                }
            }
            catch (Exception ex)
            {

            }
        }

        // GET: TProductoes/Create
        public ActionResult Create()
        {
            try
            {
                myDat = "Agrega nuevo tipo de producto";
                if (Session[myModulo].ToString().Substring(1, 1) == "0")
                    throw new System.InvalidOperationException("-No tiene acceso a crear nuevo registros-", new Exception(""));
                else
                {
                    ViewBag.CT = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["CT"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["CT"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea", myDat });
            }
            return View();
        }

        // POST: TProductoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "tp_empresa,tp_IdTipoProducto,tp_ClasificacionTP,tp_nombre,tp_descripcion,tp_padre,tp_fechaing,tp_fechamod,tp_usuarioing,tp_usuariomod,tp_maquinaing,tp_maquinamod,tp_estado,tp_timestamp")] TipoProducto tipoProducto)
        {
            try
            {
                myDat = "Crear nuevo tipo de producto / sp_ABC_TipoProducto";
                if (ModelState.IsValid)
                {
                    tipoProducto.tp_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_TipoProducto(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo,  tipoProducto.tp_ClasificacionTP,
                                                   tipoProducto.tp_nombre, tipoProducto.tp_descripcion, tipoProducto.tp_padre, tipoProducto.tp_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_TipoProducto: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo.Value, tipoProducto.tp_ClasificacionTP,
                                                   tipoProducto.tp_nombre, tipoProducto.tp_descripcion, tipoProducto.tp_padre, tipoProducto.tp_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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

            return View(tipoProducto);
        }

        // GET: TProductoes/Edit/5
        public ActionResult Edit(short? id)
        {
            TipoProducto tipo = new TipoProducto();
            if (id == null)
            {
                throw new System.InvalidOperationException("-Error al obtener registro-", new Exception(""));
            }

            try
            {
                myDat = "Actualiza tipo de producto: " + id.ToString();
                if (Session[myModulo].ToString().Substring(2, 1) == "1")
                {
                    ViewBag.CT = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["CT"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["CT"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    tipo = db.TipoProducto.Where(a => a.tp_IdTipoProducto == id).First();
                    if (tipo == null)
                        throw new System.InvalidOperationException("-Error al obtener registro-", new Exception(""));
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

        // POST: TProductoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "tp_empresa,tp_IdTipoProducto,tp_ClasificacionTP,tp_nombre,tp_descripcion,tp_padre,tp_fechaing,tp_fechamod,tp_usuarioing,tp_usuariomod,tp_maquinaing,tp_maquinamod,tp_estado,tp_timestamp")] TipoProducto tipoProducto)
        {
            try
            {
                myDat = "Actualiza datos / sp_ABC_TipoProducto";
                tsp = Convert.ToBase64String(tipoProducto.tp_timestamp as byte[]);
                codigo.Value = tipoProducto.tp_IdTipoProducto;
                int result = db.sp_ABC_TipoProducto(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo, tipoProducto.tp_ClasificacionTP,
                                                   tipoProducto.tp_nombre, tipoProducto.tp_descripcion, tipoProducto.tp_padre, tipoProducto.tp_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_TipoProducto: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo.Value, tipoProducto.tp_ClasificacionTP,
                                                   tipoProducto.tp_nombre, tipoProducto.tp_descripcion, tipoProducto.tp_padre, tipoProducto.tp_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
            }
            return RedirectToAction("Index");
        }

        // GET: TProductoes/Delete/5
        public ActionResult Delete(short? id)
        {
            TipoProducto tipo = new TipoProducto();
            if (id == null)
            {
                throw new System.InvalidOperationException("-Error al obtener registro-", new Exception(""));
            }

            try
            {
                myDat = "Dar de baja tipo de producto: " + id.ToString();
                if (Session[myModulo].ToString().Substring(3, 1) == "1")
                {
                    tipo = db.TipoProducto.Where(a => a.tp_IdTipoProducto == id).First();
                    if (tipo == null)
                    {
                        throw new System.InvalidOperationException("-Error al obtener registro-", new Exception(""));
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

        // POST: TProductoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            TipoProducto tipo = new TipoProducto();
            try
            {
                myDat = "Confirma dar de baja tipo de producto: " + id.ToString() + " / sp_ABC_TipoProducto";
                tipo = db.TipoProducto.Where(a => a.tp_IdTipoProducto == id).First();
                tsp = Convert.ToBase64String(tipo.tp_timestamp as byte[]);
                tipo.tp_usuarioing = Session["UserName"].ToString();
                codigo.Value = id;
                int result = db.sp_ABC_TipoProducto(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo, null, null, null, null, tipo.tp_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_TipoProducto: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo.Value, null, null, null, null, tipo.tp_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                    string path = Path.Combine(Server.MapPath("~/Reports"), "TipoProducto.rdlc");
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        return View("Index");
                    }
                    List<TipoProducto> cm = new List<TipoProducto>();
                    using (SSS_OPERACIONEntities dc = new Models.SSS_OPERACIONEntities())
                    {
                        cm = dc.TipoProducto.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DTProducto", cm);
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

                    myDat = "Reporte de tipos de producto";
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
