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

namespace WebGrupo3S.Controllers
{
    public class HorariosController : Controller
    {
        private SSS_PERSONASEntities dbP = new SSS_PERSONASEntities();
        private SSS_PLANILLAEntities db = new SSS_PLANILLAEntities();
        private SSS_COMPLEMENTOSEntities dbC = new SSS_COMPLEMENTOSEntities();
        private string mycatalogo = "MANTENIMIENTO HORARIOS";
        private string myModulo = "Horarios";
        private string myDat = "";
        public ConstantesP coP = new ConstantesP();
        public String tsp = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("ho_IdHorario", typeof(int));

        public class DatosSearchResultModel
        {
            public IEnumerable<sp_Busqueda_Horario_Result> DatosResults { get; set; }
        }
        // GET: Horarios
        public ActionResult Index()
        {
            validad ac = new validad();
            var DatosSearch = new DatosSearchResultModel();
            try
            {
                ac.seteAccesos(mycatalogo, Session["perfil"].ToString());
                Session[myModulo] = ac.acc;
                ViewData["nombre"] = Session["UserName"];
                if (Session["UserId"].ToString() != "")
                {
                    if (Session[myModulo].ToString().Substring(0, 1) == "1")
                    {
                        try
                        {
                            ViewBag.nuevo = Session[myModulo].ToString().Substring(1, 1);
                            ViewBag.editar = Session[myModulo].ToString().Substring(2, 1);
                            ViewBag.debaja = Session[myModulo].ToString().Substring(3, 1);
                            ViewBag.reporte = Session[myModulo].ToString().Substring(19, 1);
                            //DatosSearch.DatosResults = db.sp_Busqueda_Horario(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, null, null, error);
                            DatosSearch.DatosResults = db.sp_Busqueda_Horario(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, null, null, error).ToList();
                            WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Horario: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                        }
                        catch (Exception ex)
                        {
                            return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Lista", myDat });
                        }

                        return View(DatosSearch.DatosResults.ToList());
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
        }

        // GET: Horarios/Create
        public ActionResult Create()
        {
            try
            {
                myDat = "Crear horarios";
                if (Session[myModulo].ToString().Substring(1, 1) == "0")
                    throw new System.InvalidOperationException("-No tiene acceso a crear nuevo registros-", new Exception(""));
                else
                {
                    List<SelectListItem> items = new List<SelectListItem>();
                    items.Add(new SelectListItem { Text = "Lunes", Value = "1" });
                    items.Add(new SelectListItem { Text = "Martes", Value = "2" });
                    items.Add(new SelectListItem { Text = "Miercoles", Value = "3" });
                    items.Add(new SelectListItem { Text = "Jueves", Value = "4" });
                    items.Add(new SelectListItem { Text = "Viernes", Value = "5" });
                    items.Add(new SelectListItem { Text = "Sabado", Value = "6" });
                    items.Add(new SelectListItem { Text = "Domingo", Value = "7" });
                    ViewBag.dias = items;
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea", myDat });
            }
            return View();
        }

        // POST: Horarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ho_empresa,ho_IdHorario,ho_diaSemana,ho_Nombre,ho_Descripcion,ho_HoraEntrada,ho_HoraSalida,ho_HoraSalidaAlmuerzo,ho_HoraEntradaAlmuerzo,ho_fechaing,ho_fechamod,ho_usuarioing,ho_usuariomod,ho_maquinaing,ho_maquinamod,ho_estado,ho_timestamp")] Horario Horario)
        {
            try
            {
                myDat = "Crea horario / sp_ABC_Horario";
                if (ModelState.IsValid)
                {
                    Horario.ho_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_Horario(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo, Horario.ho_diaSemana, Horario.ho_Nombre, Horario.ho_Descripcion,
                                                   Horario.ho_HoraEntrada, Horario.ho_HoraSalida, Horario.ho_HoraSalidaAlmuerzo, Horario.ho_HoraEntradaAlmuerzo, Horario.ho_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Horario: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo.Value, Horario.ho_diaSemana, Horario.ho_Nombre, Horario.ho_Descripcion,
                                                   Horario.ho_HoraEntrada, Horario.ho_HoraSalida, Horario.ho_HoraSalidaAlmuerzo, Horario.ho_HoraEntradaAlmuerzo, Horario.ho_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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

            return View(Horario);
        }

        // GET: Horarios/Edit/5
        public ActionResult Edit(short? id)
        {
            Horario dato = new Horario();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                myDat = "Actualiza horario: " + id.ToString();
                if (Session[myModulo].ToString().Substring(2, 1) == "1")
                {
                    List<SelectListItem> items = new List<SelectListItem>();
                    items.Add(new SelectListItem { Text = "Lunes", Value = "1" });
                    items.Add(new SelectListItem { Text = "Martes", Value = "2" });
                    items.Add(new SelectListItem { Text = "Miercoles", Value = "3" });
                    items.Add(new SelectListItem { Text = "Jueves", Value = "4" });
                    items.Add(new SelectListItem { Text = "Viernes", Value = "5" });
                    items.Add(new SelectListItem { Text = "Sabado", Value = "6" });
                    items.Add(new SelectListItem { Text = "Domingo", Value = "7" });
                    ViewBag.dias = items;
                    dato = db.Horarios.Where(a => a.ho_IdHorario == id).First();
                    if (dato == null)
                        throw new System.InvalidOperationException("-Error al obtener registro-", new Exception(""));
                }
                else
                    throw new System.InvalidOperationException("-No tiene acceso a la opcion de actualización-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
            }
            return View(dato);
        }

        // POST: Horarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ho_empresa, ho_IdHorario, ho_diaSemana, ho_Nombre, ho_Descripcion, ho_HoraEntrada, ho_HoraSalida, ho_HoraSalidaAlmuerzo, ho_HoraEntradaAlmuerzo, ho_fechaing, ho_fechamod, ho_usuarioing, ho_usuariomod, ho_maquinaing, ho_maquinamod, ho_estado, ho_timestamp")] Horario Horario)
        {
            try
            {
                myDat = "Actualiza horarios / sp_ABC_Horario";
                tsp = Convert.ToBase64String(Horario.ho_timestamp as byte[]);
                codigo.Value = Horario.ho_IdHorario;
                Horario.ho_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_Horario(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo, Horario.ho_diaSemana, Horario.ho_Nombre, Horario.ho_Descripcion,
                                               Horario.ho_HoraEntrada, Horario.ho_HoraSalida, Horario.ho_HoraSalidaAlmuerzo, Horario.ho_HoraEntradaAlmuerzo, Horario.ho_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Horario: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo.Value, Horario.ho_diaSemana, Horario.ho_Nombre, Horario.ho_Descripcion,
                                               Horario.ho_HoraEntrada, Horario.ho_HoraSalida, Horario.ho_HoraSalidaAlmuerzo, Horario.ho_HoraEntradaAlmuerzo, Horario.ho_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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

        // GET: Horarios/Delete/5
        public ActionResult Delete(short? id)
        {
            Horario dato = new Horario();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                myDat = "Dar de baja horario: " + id.ToString();
                if (Session[myModulo].ToString().Substring(3, 1) == "1")
                {
                    dato = db.Horarios.Where(a => a.ho_IdHorario == id).First();
                    if (dato == null)
                        throw new System.InvalidOperationException("-Error al obtener registro-", new Exception(""));
                }
                else
                    throw new System.InvalidOperationException("-No tiene permitido dar de baja el registro-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Debaja", myDat });
            }
            return View(dato);
        }

        // POST: Horarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            Horario dato = new Horario();
            try
            {
                myDat = "Confirmacion dar de baja horario: " + id.ToString();
                dato = db.Horarios.Where(a => a.ho_IdHorario == id).First();
                tsp = Convert.ToBase64String(dato.ho_timestamp as byte[]);
                dato.ho_usuarioing = Session["UserName"].ToString();
                codigo.Value = id;
                int result = db.sp_ABC_Horario(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo, null, null, null, null, null, null, null, dato.ho_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Horario: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo.Value, null, null, null, null, null, null, null, dato.ho_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Debaja", myDat });
            }
            return RedirectToAction("Index");
        }

        public ActionResult Report(string id)
        {
            try
            {
                myDat = "Generación de reporte";
                if (Session[myModulo].ToString().Substring(19, 1) == "1")
                {
                    LocalReport lr = new LocalReport();
                    string path = Path.Combine(Server.MapPath("~/Reports"), "Horarios.rdlc");
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        return View("Index");
                    }
                    List<Horario> cm = new List<Horario>();
                    using (SSS_PLANILLAEntities dc = new SSS_PLANILLAEntities())
                    {
                        cm = dc.Horarios.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DHorarios", cm);
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
                    myDat = "Reporte horarios";
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