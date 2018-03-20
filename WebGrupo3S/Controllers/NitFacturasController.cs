using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web.Mvc;
using WebGrupo3S.Models;
using Microsoft.Reporting.WebForms;
using System.IO;
using WebGrupo3S.Helpers;

namespace WebGrupo3S.Views
{
    public class NitFacturasController : Controller
    {
        private SSS_PERSONASEntities db = new SSS_PERSONASEntities();
        private string myModulo = "NitFacturas";
        private string mycatalogo = "Nit facturas";
        private string myDat = "";
        public ConstantesP coP = new ConstantesP();
        public String tsp = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("nf_nit", typeof(string));

        // GET: NitFacturas
        public class NitSearchResultModel
        {
            public IEnumerable<sp_Busqueda_NitFactura_Result> NitResults { get; set; }
        }
        // GET: NitFacturas
        public ActionResult Index()
        {
            validad ac = new validad();
            try
            {
                ac.seteAccesos(mycatalogo, Session["perfil"].ToString());
                Session[myModulo] = ac.acc;
                ViewData["nombre"] = Session["UserName"];
                if (Session["UserId"].ToString() != "")
                {
                    if (Session[myModulo].ToString().Substring(0, 1) == "1")
                    {
                        myDat = "Listado de Nit";
                        ViewBag.editar = Session[myModulo].ToString().Substring(2, 1);
                        ViewBag.debaja = Session[myModulo].ToString().Substring(3, 1);
                        ViewBag.reporte = Session[myModulo].ToString().Substring(19, 1);
                        return View(db.NitFactura.Where(a => a.nf_estado == 1).ToList());
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

        // GET: NitFacturas/Edit/5
        public ActionResult Edit(string id)
        {
            NitFactura dato = new NitFactura();
            if (id == null)
            {
                return View("Error");
            }

            try
            {
                myDat = "Actualiza Nit: " + id.ToString();
                if (Session[myModulo].ToString().Substring(2, 1) == "1")
                {
                    ViewData["Nit"] = id;
                    dato = db.NitFactura.Where(a => a.nf_Nit == id).First();
                    if (dato == null)
                    {
                        return View("Error");
                    }
                }
                else
                    throw new System.InvalidOperationException("-Ingreso a opción denegada!-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
            }
            return View(dato);
        }

        // POST: NitFacturas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "nf_empresa,nf_Nit,nf_IdSucursal,nf_Nombre,nf_Direccion,nf_actCentral,nf_fechaing,nf_fechamod,nf_usuarioing,nf_usuariomod,nf_maquinaing,nf_maquinamod,nf_estado,nf_timestamp")] NitFactura nitFactura)
        {
            try
            {
                myDat = "Actualiza Nit / sp_ABC_NitFactura";
                tsp = Convert.ToBase64String(nitFactura.nf_timestamp as byte[]);
                codigo.Value = nitFactura.nf_Nit;
                nitFactura.nf_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_NitFactura(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo, Convert.ToInt16(coP.cls_sucursal), nitFactura.nf_Nombre,
                                                   nitFactura.nf_Direccion, null, nitFactura.nf_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_NitFactura: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo.Value, Convert.ToInt16(coP.cls_sucursal), nitFactura.nf_Nombre,
                                                   nitFactura.nf_Direccion, null, nitFactura.nf_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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

        // GET: NitFacturas/Delete/5
        public ActionResult Delete(string id)
        {
            NitFactura dato = new NitFactura();
            if (id == null)
            {
                return View("Error");
            }

            try
            {
                if (Session[myModulo].ToString().Substring(3, 1) == "1")
                {
                    myDat = "Dar de baja Nit: " + id.ToString();
                    dato = db.NitFactura.Where(a => a.nf_Nit == id).First();
                    if (dato == null)
                    {
                        return View("Error");
                    }
                }
                else
                    throw new System.InvalidOperationException("-Ingreso a opción denegada!-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Debaja", myDat });
            }
            return View(dato);
        }

        // POST: NitFacturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            NitFactura dato = new NitFactura();
            try
            {
                myDat = "Confirma de dar de baja de Nit:" + id.ToString() + " / sp_ABC_NitFactura";
                dato = db.NitFactura.Where(a => a.nf_Nit == id).First();
                tsp = Convert.ToBase64String(dato.nf_timestamp as byte[]);
                dato.nf_usuarioing = Session["UserName"].ToString();
                codigo.Value = id;
                int result = db.sp_ABC_NitFactura(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo, Convert.ToInt16(coP.cls_sucursal), null, null, null, dato.nf_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_NitFactura: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo.Value, Convert.ToInt16(coP.cls_sucursal), null, null, null, dato.nf_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                    string path = Path.Combine(Server.MapPath("~/Reports"), "NitFacturas.rdlc");
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        return View("Index");
                    }
                    List<NitFactura> cm = new List<NitFactura>();
                    using (SSS_PERSONASEntities dc = new SSS_PERSONASEntities())
                    {
                        cm = dc.NitFactura.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DNit", cm);
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

                    myDat = "Reporte de Nit";
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
