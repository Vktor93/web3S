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
    public class ProductoesController : Controller
    {
        private SSS_OPERACIONEntities db = new SSS_OPERACIONEntities();
        private SSS_COMPLEMENTOSEntities dbC = new SSS_COMPLEMENTOSEntities();
        private string myModulo = "Productos";
        private string mycatalogo = "Productos";
        private string myDat = "";
        public ConstantesP coP = new ConstantesP();
        public String tsp = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("pr_IdProd", typeof(int));

        public class DatosSearchResultModel
        {
            public IEnumerable<sp_Busqueda_Producto_Result> DatosResults { get; set; }
        }
        // GET: Productoes
        public ActionResult Index()
        {
            validad ac = new validad();
            var DatosSearch = new DatosSearchResultModel();
            try
            {
                ac.seteAccesos(mycatalogo, Session["perfil"].ToString());
                Session[myModulo] = ac.acc;
                myDat = "Busca productos / sp_Busqueda_Producto";
                if (Session["UserId"].ToString() != "")
                {
                    if (Session[myModulo].ToString().Substring(0, 1) == "1")
                    {
                        buscaCatalogos();
                        ViewBag.nuevo = Session[myModulo].ToString().Substring(1, 1);
                        ViewBag.editar = Session[myModulo].ToString().Substring(2, 1);
                        ViewBag.debaja = Session[myModulo].ToString().Substring(3, 1);
                        ViewBag.precios = Session[myModulo].ToString().Substring(11, 1);
                        ViewBag.reporte = Session[myModulo].ToString().Substring(19, 1);
                        DatosSearch.DatosResults = db.sp_Busqueda_Producto(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, null,null, error).ToList();
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Producto: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
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
                    PrecioSearch.PrecioResults = db.sp_Busqueda_PrecioProductoServicio(2, "", Convert.ToInt16(coP.cls_empresa), "PP", null, id, null, error).ToList();
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_PrecioProductoServicio: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), "PP", null, id, null, "-> R: " + validad.getResponse(error)));
                }
                else
                    throw new System.InvalidOperationException("-Ingreso a opción denegado!-", new Exception(""));
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Lista de precios"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Lista", myDat });
            }

            return View(PrecioSearch.PrecioResults.ToList());
        }

        public ActionResult Regresa()
        {
            return RedirectToAction("Index");
        }

        public void buscaCatalogos()
        {
            Session["MA"] = 0;

            ObjectResult resultadoT = dbC.sp_Busqueda_CabCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), null, "MarcaProducto", null, null, null, error);
            WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_CabCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, "MarcaProducto", null, null, null, "-> R: " + validad.getResponse(error)));
            foreach (sp_Busqueda_CabCatalogo_Result re in resultadoT)
            {
                Session["MA"] = re.IdCatalogo;
            }
        }


        // GET: Productoes/Create
        public ActionResult Create()
        {
            try
            {
                myDat = "Agrega nuevo producto";
                if (Session[myModulo].ToString().Substring(1, 1) == "0")
                    throw new System.InvalidOperationException("-No tiene acceso a crear nuevo registros-", new Exception(""));
                else
                {
                    ViewBag.MA = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MA"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MA"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    ViewBag.Tprod = new SelectList(db.sp_Busqueda_TipoProducto(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, error).ToList(), "TipoProducto", "NombreTipoProducto");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_TipoProducto: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea", myDat });
            } 
            return View();
        }

        // POST: Productoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "pr_empresa,pr_TipoProducto,pr_IdProd,pr_nombre,pr_descripcion,pr_CodigoBarras,pr_CodigoSAP,pr_IdMarca,pr_ImagenProd,pr_fechaing,pr_fechamod,pr_usuarioing,pr_usuariomod,pr_maquinaing,pr_maquinamod,pr_estado,pr_timestamp")] Producto producto)
        {
            try
            {
                myDat = "Crea nuevo producto / sp_ABC_Producto";
                if (ModelState.IsValid)
                {
                    producto.pr_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_Producto(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), producto.pr_TipoProducto, codigo, producto.pr_nombre,
                                                   producto.pr_descripcion, producto.pr_CodigoBarras, producto.pr_CodigoSAP, producto.pr_IdMarca, null, producto.pr_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Producto: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), producto.pr_TipoProducto, codigo.Value, producto.pr_nombre,
                                                   producto.pr_descripcion, producto.pr_CodigoBarras, producto.pr_CodigoSAP, producto.pr_IdMarca, null, producto.pr_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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

            return View(producto);
        }

        // GET: Productoes/Edit/5
        public ActionResult Edit(short? id)
        {
            Producto dato = new Producto();
            if (id == null)
            {
                return View("Error");
            }

            try
            {
                myDat = "Edita producto: " + id.ToString();
                if (Session[myModulo].ToString().Substring(2, 1) == "1")
                {
                    ViewBag.MA = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MA"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MA"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    ViewBag.Tprod = new SelectList(db.sp_Busqueda_TipoProducto(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, error).ToList(), "TipoProducto", "NombreTipoProducto");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_TipoProducto: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    dato = db.Producto.Where(a => a.pr_IdProd == id).First();
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
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Actualizar producto"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
            }
            return View(dato);
        }

        // POST: Productoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "pr_empresa,pr_TipoProducto,pr_IdProd,pr_nombre,pr_descripcion,pr_CodigoBarras,pr_CodigoSAP,pr_IdMarca,pr_ImagenProd,pr_fechaing,pr_fechamod,pr_usuarioing,pr_usuariomod,pr_maquinaing,pr_maquinamod,pr_estado,pr_timestamp")] Producto producto)
        {
            try
            {
                myDat = "Actualiza producto / sp_ABC_Producto";
                tsp = Convert.ToBase64String(producto.pr_timestamp as byte[]);
                codigo.Value = producto.pr_IdProd;
                int result = db.sp_ABC_Producto(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), producto.pr_TipoProducto, codigo, producto.pr_nombre,
                                                   producto.pr_descripcion, producto.pr_CodigoBarras, producto.pr_CodigoSAP, producto.pr_IdMarca, null, producto.pr_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Producto: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), producto.pr_TipoProducto, codigo.Value, producto.pr_nombre,
                                                   producto.pr_descripcion, producto.pr_CodigoBarras, producto.pr_CodigoSAP, producto.pr_IdMarca, null, producto.pr_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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

        // GET: Productoes/Delete/5
        public ActionResult Delete(short? id)
        {
            Producto dato = new Producto();
            if (id == null)
            {
                return View("Error");
            }

            try
            {
                myDat = "Da de baja producto: " + id.ToString();
                if (Session[myModulo].ToString().Substring(3, 1) == "1")
                {
                    dato = db.Producto.Where(a => a.pr_IdProd == id).First();
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
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "DeBaja", myDat });
            }
            return View(dato);
        }

        // POST: Productoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            Producto dato = new Producto();
            try
            {
                myDat = "Confirma de baja de producto: " + id.ToString() + " / sp_ABC_Producto";
                dato = db.Producto.Where(a => a.pr_IdProd == id).First();
                tsp = Convert.ToBase64String(dato.pr_timestamp as byte[]);
                dato.pr_usuarioing = Session["UserName"].ToString();
                codigo.Value = id;
                int result = db.sp_ABC_Producto(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), null, codigo, null, null, null, null, null, null, dato.pr_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Producto: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), null, codigo.Value, null, null, null, null, null, null, dato.pr_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                    string path = Path.Combine(Server.MapPath("~/Reports"), "Productos.rdlc");
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        return View("Index");
                    }
                    List<Producto> cm = new List<Producto>();
                    using (SSS_OPERACIONEntities dc = new Models.SSS_OPERACIONEntities())
                    {
                        cm = dc.Producto.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DProductos", cm);
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

                    myDat = "Listado de productos";
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
