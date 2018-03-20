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
    public class ServiciosController : Controller
    {
        private SSS_OPERACIONEntities db = new SSS_OPERACIONEntities();
        private string myModulo = "Servicios";
        private string mycatalogo = "Servicios";
        private string myDat = "";
        public ConstantesP coP = new ConstantesP();
        public String tsp = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("sv_IdServicio", typeof(int));

        public class DatosSearchResultModel
        {
            public IEnumerable<sp_Busqueda_Servicio_Result> DatosResults { get; set; }
        }

        // GET: Servicios
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
                        myDat = "Busqueda de servicios / sp_Busqueda_Servicio";
                        ViewBag.nuevo = Session[myModulo].ToString().Substring(1, 1);
                        ViewBag.editar = Session[myModulo].ToString().Substring(2, 1);
                        ViewBag.debaja = Session[myModulo].ToString().Substring(3, 1);
                        ViewBag.precios = Session[myModulo].ToString().Substring(11, 1);
                        ViewBag.reporte = Session[myModulo].ToString().Substring(19, 1);
                        DatosSearch.DatosResults = db.sp_Busqueda_Servicio(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, error).ToList();
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Servicio: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, "-> R: " + validad.getResponse(error)));
                    }
                    else
                        throw new System.InvalidOperationException("-Ingreso a opción denegado!-", new Exception(""));
                }
                else
                    throw new System.InvalidOperationException("-Ingrese al Sistema para poder Ingresar a esta Opción-", new Exception(""));
            }
            catch (Exception ex)
            {
//                return View("Error", new HandleErrorInfo(ex, myModulo, "Lista"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Lista", myDat });
            }

            return View(DatosSearch.DatosResults.ToList());
        }


        public class PrecioSearchResultModel
        {
            public IEnumerable<sp_Busqueda_PrecioProductoServicio_Result> PrecioResults { get; set; }
        }

        public ActionResult precio(short id, string nom)
        {
            var PrecioSearch = new PrecioSearchResultModel();
            try
            {
                myDat = "Lista de precios: " + id.ToString() + " - " + nom + " / sp_Busqueda_PrecioProductoServicio";
                if (Session[myModulo].ToString().Substring(11, 1) == "1")
                {
                    ViewData["Nombre"] = nom;
                    ViewBag.idpro = id;
                    ViewBag.nombre = nom;
                    ViewBag.nuevo = Session[myModulo].ToString().Substring(25, 1);
                    ViewBag.editar = Session[myModulo].ToString().Substring(26, 1);
                    ViewBag.debaja = Session[myModulo].ToString().Substring(27, 1);
                    PrecioSearch.PrecioResults = db.sp_Busqueda_PrecioProductoServicio(2, "", Convert.ToInt16(coP.cls_empresa), "PS", null, id, null, error).ToList();
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_PrecioProductoServicio: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), "PS", null, id, null, "-> R: " + validad.getResponse(error)));
                }
                else
                    throw new System.InvalidOperationException("-Ingreso a opción denegado!-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Lista", myDat });
            }

            return View(PrecioSearch.PrecioResults.ToList());
        }

        public ActionResult Regresa()
        {
            return RedirectToAction("Index");
        }


        // GET: Servicios/Create
        public ActionResult Create()
        {
            try
            {
                myDat = "Agrega nuevo servicio";
                if (Session[myModulo].ToString().Substring(1, 1) == "0")
                    throw new System.InvalidOperationException("-No tiene acceso a crear nuevo registros-", new Exception(""));
                else
                {
                    ViewBag.Tserv = new SelectList(db.sp_Busqueda_TipoServicio(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, error).ToList(), "TipoServicio", "NombreTipoServicio");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_TipoServicio: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea", myDat });
            }
            return View();
        }

        // POST: Servicios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "sv_empresa,sv_TipoServicio,sv_IdServicio,sv_nombre,sv_descripcion,sv_duracionServicio,sv_fechaing,sv_fechamod,sv_usuarioing,sv_usuariomod,sv_maquinaing,sv_maquinamod,sv_estado,sv_timestamp")] Servicio servicio)
        {
            try
            {
                myDat = "Crear nuevo servicio / sp_ABC_Servicio";
                if (ModelState.IsValid)
                {
                    servicio.sv_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_Servicio(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), servicio.sv_TipoServicio, codigo, servicio.sv_nombre,
                                                   servicio.sv_descripcion, servicio.sv_duracionServicio, servicio.sv_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Servicio: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), servicio.sv_TipoServicio, codigo.Value, servicio.sv_nombre,
                                                   servicio.sv_descripcion, servicio.sv_duracionServicio, servicio.sv_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Crear nuevo servicio"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea", myDat });
            }

            return View(servicio);
        }

        // GET: Servicios/Edit/5
        public ActionResult Edit(short? id)
        {
            Servicio dato = new Servicio();
            if (id == null)
            {
                return View("Error");
            }

            try
            {
                myDat = "Actualizar servicio: " + id.ToString();
                if (Session[myModulo].ToString().Substring(2, 1) == "1")
                {
                    ViewBag.Tserv = new SelectList(db.sp_Busqueda_TipoServicio(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, error).ToList(), "TipoServicio", "NombreTipoServicio");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_TipoServicio: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    dato = db.Servicio.Where(a => a.sv_IdServicio == id).First();
                    if (dato == null)
                    {
                        return View("Error");
                    }
                }
                else
                    throw new System.InvalidOperationException("-No tiene acceso a esta opción-", new Exception(""));
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Actualizar servicio"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
            }
            return View(dato);
        }

        // POST: Servicios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "sv_empresa,sv_TipoServicio,sv_IdServicio,sv_nombre,sv_descripcion,sv_duracionServicio,sv_fechaing,sv_fechamod,sv_usuarioing,sv_usuariomod,sv_maquinaing,sv_maquinamod,sv_estado,sv_timestamp")] Servicio servicio)
        {
            try
            {
                myDat = "Actualizar datos / sp_ABC_Servicio";
                tsp = Convert.ToBase64String(servicio.sv_timestamp as byte[]);
                codigo.Value = servicio.sv_IdServicio;
                int result = db.sp_ABC_Servicio(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), servicio.sv_TipoServicio, codigo, servicio.sv_nombre,
                                                   servicio.sv_descripcion, servicio.sv_duracionServicio, servicio.sv_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Servicio: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), servicio.sv_TipoServicio, codigo.Value, servicio.sv_nombre,
                                                   servicio.sv_descripcion, servicio.sv_duracionServicio, servicio.sv_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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

        // GET: Servicios/Delete/5
        public ActionResult Delete(short? id)
        {
            Servicio dato = new Servicio();
            if (id == null)
            {
                return View("Error");
            }

            try
            {
                myDat = "Dar de baja el servicio: " + id.ToString();
                if (Session[myModulo].ToString().Substring(3, 1) == "1")
                {
                    dato = db.Servicio.Where(a => a.sv_IdServicio == id).First();
                    if (dato == null)
                    {
                        return View("Error");
                    }
                }
                else
                    throw new System.InvalidOperationException("-No tiene permitido dar de baja el registro-", new Exception(""));
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Da de baja al servicio"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "DeBaja", myDat });
            }
            return View(dato);
        }

        // POST: Servicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            Servicio dato = new Servicio();
            try
            {
                myDat = "Confirma dar de baja el servicio: " + id.ToString() + " / sp_ABC_Servicio";
                dato = db.Servicio.Where(a => a.sv_IdServicio == id).First();
                tsp = Convert.ToBase64String(dato.sv_timestamp as byte[]);
                dato.sv_usuarioing = Session["UserName"].ToString();
                codigo.Value = id;
                int result = db.sp_ABC_Servicio(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), dato.sv_TipoServicio, codigo, null, null, null, dato.sv_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Servicio: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), dato.sv_TipoServicio, codigo.Value, null, null, null, dato.sv_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                    string path = Path.Combine(Server.MapPath("~/Reports"), "Servicios.rdlc");
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        return View("Index");
                    }
                    List<Servicio> cm = new List<Servicio>();
                    using (SSS_OPERACIONEntities dc = new Models.SSS_OPERACIONEntities())
                    {
                        cm = dc.Servicio.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DServicios", cm);
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

                    myDat = "Reporte de servicios";
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
