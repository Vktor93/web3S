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
    public class ProveedorsController : Controller
    {
        private SSS_PERSONASEntities db = new SSS_PERSONASEntities();
        private SSS_COMPLEMENTOSEntities dbC = new SSS_COMPLEMENTOSEntities();
        private string myModulo = "Proveedores";
        private string mycatalogo = "proveedores";
        private string myDat = "";
        public ConstantesP coP = new ConstantesP();
        public String tsp = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("pv_proveedor", typeof(int));
        ObjectParameter codigoC = new ObjectParameter("cn_idcontacto", typeof(int));
        ObjectParameter codigoD = new ObjectParameter("di_idDireccion", typeof(int));

        public class ProveedorSearchResultModel
        {
            public IEnumerable<sp_Busqueda_Proveedor_Result> ProveedorResults { get; set; }
        }
        // GET: Proveedors
        public ActionResult Index()
        {
            validad ac = new validad();
            var ProveedorSearch = new ProveedorSearchResultModel();
            try
            {
                ac.seteAccesos(mycatalogo, Session["perfil"].ToString());
                Session[myModulo] = ac.acc;
                myDat = "Busca proveedores / sp_Busqueda_Proveedor";
                if (Session["UserId"].ToString() != "")
                {
                    if (Session[myModulo].ToString().Substring(0, 1) == "1")
                    {
                        buscaCatalogos();
                        ViewBag.nuevo = Session[myModulo].ToString().Substring(1, 1);
                        ViewBag.editar = Session[myModulo].ToString().Substring(2, 1);
                        ViewBag.debaja = Session[myModulo].ToString().Substring(3, 1);
                        ViewBag.contactos = Session[myModulo].ToString().Substring(8, 1);
                        ViewBag.direccion = Session[myModulo].ToString().Substring(7, 1);
                        ViewBag.reporte = Session[myModulo].ToString().Substring(19, 1);
                        ProveedorSearch.ProveedorResults = db.sp_Busqueda_Proveedor(1, "", Convert.ToInt16(coP.cls_empresa), null, null,null, null, null, error).ToList();
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Proveedor: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
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

            return View(ProveedorSearch.ProveedorResults.ToList());
        }

        public void buscaCatalogos()
        {
            Session["TP"] = TraeIdCatalogo("TipoProveedor");
            Session["CO"] = TraeIdCatalogo("TipoContacto");
            Session["DI"] = TraeIdCatalogo("TipoDireccion");
            Session["MU"] = TraeIdCatalogo("Municipios");
            Session["DE"] = TraeIdCatalogo("Departamentos");
            Session["PA"] = TraeIdCatalogo("Paises");
        }

        public int TraeIdCatalogo(string catalogo)
        {
            int numero = 0;
            try
            {
                ObjectResult resultadoCO = dbC.sp_Busqueda_CabCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), null, catalogo, null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_CabCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, catalogo, null, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_CabCatalogo_Result re in resultadoCO)
                {
                    numero = re.IdCatalogo;
                }
            }
            catch (Exception)
            {
                numero = 0;
            }
            return numero;
        }

        public class contactosSearchResultModel
        {
            public IEnumerable<sp_Busqueda_Contacto_Result> contactoResults { get; set; }
        }

        public class direccionesSearchResultModel
        {
            public IEnumerable<sp_Busqueda_Direccion_Result> direccionResults { get; set; }
        }

        public class cataSearchResultModel
        {
            public IEnumerable<sp_Busqueda_DetCatalogo_Result> cataResults { get; set; }
        }

        public ActionResult Direcciones(short? id, string nom)
        {
            var direccionSearch = new direccionesSearchResultModel();
            try
            {
                myDat = "Lista de direcciones: " + id.ToString() + " - " + nom + " / sp_Busqueda_Direccion";
                if (Session[myModulo].ToString().Substring(7, 1) == "1")
                {
                    ViewBag.idproveedor = id;
                    ViewBag.nombre = nom;
                    ViewData["Nombre"] = nom;
                    ViewBag.nuevo = Session[myModulo].ToString().Substring(31, 1);
                    ViewBag.editar = Session[myModulo].ToString().Substring(32, 1);
                    ViewBag.debaja = Session[myModulo].ToString().Substring(33, 1);
                    direccionSearch.direccionResults = db.sp_Busqueda_Direccion(2, "", Convert.ToInt16(coP.cls_empresa), null, "PV", id, null, null, null, null, null, null, null, error).ToList();
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Direccion: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, "PV", id, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                }
                else
                    throw new System.InvalidOperationException("-Ingreso a opción denegada!-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Lista", myDat });
            }
            return View(direccionSearch.direccionResults.ToList());
        }


        public JsonResult GetDepto(string id)
        {
            List<SelectListItem> depto = new List<SelectListItem>();

            ObjectResult resultado = dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DE"]), null, null, id, null, null, error);
            WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DE"]), null, null, id, null, null, "-> R: " + validad.getResponse(error)));
            foreach (sp_Busqueda_DetCatalogo_Result re in resultado)
            {
                depto.Add(new SelectListItem { Text = re.Valor, Value = re.Codigo });
            }
            return Json(new SelectList(depto, "Value", "Text"));
        }

        public JsonResult GetMuni(string id)
        {
            List<SelectListItem> muni = new List<SelectListItem>();

            ObjectResult resultado = dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MU"]), null, null, id, null, null, error);
            WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MU"]), null, null, id, null, null, "-> R: " + validad.getResponse(error)));
            foreach (sp_Busqueda_DetCatalogo_Result re in resultado)
            {
                muni.Add(new SelectListItem { Text = re.Valor, Value = re.Codigo });
            }
            return Json(new SelectList(muni, "Value", "Text"));
        }

        public ActionResult CreaDireccion(int? id, string aso)
        {
            Direccion dato = new Direccion();
            try
            {
                if (Session[myModulo].ToString().Substring(31, 1) == "1")
                {
                    dato.di_asociado = Convert.ToInt16(id);
                    dato.di_TipoAsociado = "PV";
                    dato.di_pais = "502";
                    dato.di_departamento = "01";
                    dato.di_municipio = "01";
                    ViewBag.idproveedor = id;
                    ViewBag.nombre = aso;
                    Session["asociado"] = aso;
                    ViewBag.DI = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DI"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DI"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    ViewBag.MU = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MU"]), null, null, "01", null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MU"]), null, null, "01", null, null, "-> R: " + validad.getResponse(error)));
                    ViewBag.DE = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DE"]), null, null, "502", null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DE"]), null, null, "502", null, null, "-> R: " + validad.getResponse(error)));
                    ViewBag.PA = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["PA"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["PA"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                }
                else
                    throw new System.InvalidOperationException("No tiene acceso a esta opción", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea dirección", myDat });
            }
            return View(dato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreaDireccion([Bind(Include = "di_empresa,di_IdDireccion,di_TipoAsociado,di_asociado,di_TipoDireccion,di_Direccion,di_Direccion2,di_zona,di_municipio,di_departamento,di_pais,di_fechaing,di_fechamod,di_usuarioing,di_usuariomod,di_maquinaing,di_maquinamod,di_estado,di_timestamp")] Direccion direccion)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    myDat = "Crea Asociado / sp_ABC_Direccion";
                    codigoD.Value = direccion.di_IdDireccion;
                    direccion.di_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_Direccion(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigoD, direccion.di_TipoAsociado, direccion.di_asociado, direccion.di_TipoDireccion, direccion.di_Direccion, direccion.di_Direccion2, direccion.di_zona, direccion.di_municipio, direccion.di_departamento, direccion.di_pais, direccion.di_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Direccion: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigoD, direccion.di_TipoAsociado, direccion.di_asociado, direccion.di_TipoDireccion, direccion.di_Direccion, direccion.di_Direccion2, direccion.di_zona, direccion.di_municipio, direccion.di_departamento, direccion.di_pais, direccion.di_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                    if (error.Value.ToString() == "")
                    {
                        db.SaveChanges();
                        return RedirectToAction("Direcciones", new { id = direccion.di_asociado, nom = Convert.ToString(Session["asociado"]) });
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
            return View(direccion);
        }

        public ActionResult DEdit(short? id, string aso, string nom)
        {
            Direccion dato = new Direccion();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                myDat = "Edita contacto: " + id.ToString() + " - " + aso + " - " + nom + " / sp_Busqueda_Direccion";
                if (Session[myModulo].ToString().Substring(32, 1) == "1")
                {
                    ViewData["idproveedor"] = aso;
                    ViewData["asociado"] = nom;
                    Session["asociado"] = nom;
                    ObjectResult resultado = db.sp_Busqueda_Direccion(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, null, null, null, null, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Direccion: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_Direccion_Result re in resultado)
                    {
                        dato = CargaDireccion(re);
                    }
                    if (dato == null)
                        throw new System.InvalidOperationException("Error al obtener registro", new Exception(""));
                    else
                    {
                        ViewBag.DI = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DI"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DI"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                        ViewBag.PA = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["PA"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["PA"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                        ViewBag.MU = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MU"]), null, null, dato.di_departamento, null, null, error).ToList(), "Codigo", "Valor");
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MU"]), null, null, dato.di_departamento, null, null, "-> R: " + validad.getResponse(error)));
                        ViewBag.DE = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DE"]), null, null, dato.di_pais, null, null, error).ToList(), "Codigo", "Valor");
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DE"]), null, null, dato.di_pais, null, null, "-> R: " + validad.getResponse(error)));
                    }
                }
                else
                    throw new System.InvalidOperationException("No tiene acceso a la opcion de actualización");
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Edita contacto"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
            }
            return View(dato);
        }

        private Direccion CargaDireccion(sp_Busqueda_Direccion_Result parResult)
        {
            Direccion ca = new Direccion();

            ca.di_empresa = parResult.Empresa;
            ca.di_TipoAsociado = parResult.TipoAsociado;
            ca.di_IdDireccion = parResult.IdDireccion;
            ca.di_TipoDireccion = parResult.TipoDireccion;
            ca.di_asociado = parResult.Asociado;
            ca.di_pais = parResult.Pais;
            ca.di_departamento = parResult.Departamento;
            ca.di_municipio = parResult.Municipio;
            ca.di_Direccion = parResult.Direccion;
            ca.di_Direccion2 = parResult.Direccion2;
            ca.di_zona = parResult.Zona;
            ca.di_timestamp = parResult.TimeStamp;
            return ca;
        }

        // POST: DetCatalogoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DEdit([Bind(Include = "di_empresa,di_IdDireccion,di_TipoAsociado,di_asociado,di_TipoDireccion,di_Direccion,di_Direccion2,di_zona,di_municipio,di_departamento,di_pais,di_fechaing,di_fechamod,di_usuarioing,di_usuariomod,di_maquinaing,di_maquinamod,di_estado,di_timestamp")] Direccion direccion)
        {
            try
            {
                myDat = "Edita dirección / sp_ABC_Direccion";
                tsp = Convert.ToBase64String(direccion.di_timestamp as byte[]);
                codigoD.Value = direccion.di_IdDireccion;
                direccion.di_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_Direccion(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigoD, direccion.di_TipoAsociado, direccion.di_asociado, direccion.di_TipoDireccion, direccion.di_Direccion, direccion.di_Direccion2, direccion.di_zona, direccion.di_municipio, direccion.di_departamento, direccion.di_pais, direccion.di_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Direccion: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigoD, direccion.di_TipoAsociado, direccion.di_asociado, direccion.di_TipoDireccion, direccion.di_Direccion, direccion.di_Direccion2, direccion.di_zona, direccion.di_municipio, direccion.di_departamento, direccion.di_pais, direccion.di_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                    return RedirectToAction("Direcciones", new { id = direccion.di_asociado, nom = Convert.ToString(Session["asociado"]) });
                }
                else
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
            }
        }

        public ActionResult DDelete(short? id, string aso, string nom)
        {
            Direccion dato = new Direccion();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                myDat = "Dar de baja dirección: " + id.ToString() + " - " + aso + " - " + nom + " / sp_Busqueda_Direccion";
                if (Session[myModulo].ToString().Substring(33, 1) == "1")
                {
                    ViewData["idproveedor"] = aso;
                    ViewData["asociado"] = nom;
                    ObjectResult resultado = db.sp_Busqueda_Direccion(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, null, null, null, null, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Direccion: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_Direccion_Result re in resultado)
                    {
                        dato = CargaDireccion(re);
                    }
                    if (dato == null)
                        throw new System.InvalidOperationException("Error al obtener registro", new Exception(""));
                    else
                    {
                        ViewBag.DI = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DI"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DI"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                        ViewBag.PA = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["PA"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["PA"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                        ViewBag.MU = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MU"]), null, null, dato.di_departamento, null, null, error).ToList(), "Codigo", "Valor");
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MU"]), null, null, dato.di_departamento, null, null, "-> R: " + validad.getResponse(error)));
                        ViewBag.DE = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DE"]), null, null, dato.di_pais, null, null, error).ToList(), "Codigo", "Valor");
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DE"]), null, null, dato.di_pais, null, null, "-> R: " + validad.getResponse(error)));
                    }
                    if (dato == null)
                        throw new System.InvalidOperationException("Error al obtener registro", new Exception(""));
                }
                else
                    throw new System.InvalidOperationException("No tiene permitido dar de baja el registro");
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Dar de baja dirección"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "DeBaja", myDat });
            }
            return View(dato);
        }

        // POST: DetCatalogoes/Delete/5
        [HttpPost, ActionName("DDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DDeleteConfirmed(short? id, string aso, string nom)
        {
            Direccion dato = new Direccion();
            try
            {
                myDat = "Confirmar dar de baja dirección: " + id.ToString() + " - " + aso + " - " + nom + " / sp_Busqueda_Direccion";
                ObjectResult resultado = db.sp_Busqueda_Direccion(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, null, null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Direccion: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_Direccion_Result re in resultado)
                {
                    dato = CargaDireccion(re);
                    tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                }
                dato.di_usuarioing = Session["UserName"].ToString();
                codigoD.Value = dato.di_IdDireccion;
                int result = db.sp_ABC_Direccion(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigoD, dato.di_TipoAsociado, dato.di_asociado, dato.di_TipoDireccion, null, null, null, null, null, null, dato.di_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Direccion: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigoD, dato.di_TipoAsociado, dato.di_asociado, dato.di_TipoDireccion, null, null, null, null, null, null, dato.di_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "DeBaja", myDat });
            }
            return RedirectToAction("Direcciones", new { id = dato.di_asociado, nom = Convert.ToString(Session["asociado"]) });
        }


        public ActionResult Contactos(short? id, string nom)
        {
            var contactoSearch = new contactosSearchResultModel();
            try
            {
                myDat = "Lista de contactos: " + id.ToString() + " - " + nom + " / sp_Busqueda_Contacto";
                if (Session[myModulo].ToString().Substring(8, 1) == "1")
                {
                    ViewBag.idproveedor = id;
                    ViewBag.nombre = nom;
                    ViewData["Nombre"] = nom;
                    ViewBag.nuevo = Session[myModulo].ToString().Substring(25, 1);
                    ViewBag.editar = Session[myModulo].ToString().Substring(26, 1);
                    ViewBag.debaja = Session[myModulo].ToString().Substring(27, 1);
                    contactoSearch.contactoResults = db.sp_Busqueda_Contacto(2, "", Convert.ToInt16(coP.cls_empresa), null, "PV", id, null, null, null, error).ToList();
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Contacto: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, "PV", id, null, null, null, "-> R: " + validad.getResponse(error)));
                }
                else
                    throw new System.InvalidOperationException("-Ingreso a opción denegada!-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Lista", myDat });
            }
            return View(contactoSearch.contactoResults.ToList());
        }

        public ActionResult Regresa()
        {
            return RedirectToAction("Index");
        }

        public ActionResult CreaContacto(int? id, string aso)
        {
            Contacto dato = new Contacto();
            try
            {
                if (Session[myModulo].ToString().Substring(25, 1) == "1")
                {
                    dato.cn_asociado = Convert.ToInt16(id);
                    dato.cn_tipoAsociado = "PV";

                    ViewBag.idproveedor = id;
                    ViewBag.nombre = aso;
                    Session["asociado"] = aso;
                    ViewBag.CO = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["CO"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["CO"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                }
                else
                    throw new System.InvalidOperationException("-Ingreso a opción denegada!-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea Contacto", myDat });
            }
            return View(dato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreaContacto([Bind(Include = "cn_empresa,cn_idContacto,cn_tipoAsociado,cn_asociado,cn_tipoContacto,cn_Contacto,cn_Contacto2,cn_fechaing,cn_fechamod,cn_usuarioing,cn_usuariomod,cn_maquinaing,cn_maquinamod,cn_estado,cn_timestamp")] Contacto contacto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    myDat = "Crea Contacto / sp_ABC_Contacto";
                    codigoC.Value = contacto.cn_idContacto;
                    contacto.cn_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_Contacto(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigoC, contacto.cn_tipoAsociado, contacto.cn_asociado, contacto.cn_tipoContacto, contacto.cn_Contacto, contacto.cn_Contacto2, contacto.cn_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Contacto: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigoC, contacto.cn_tipoAsociado, contacto.cn_asociado, contacto.cn_tipoContacto, contacto.cn_Contacto, contacto.cn_Contacto2, contacto.cn_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                    if (error.Value.ToString() == "")
                    {
                        db.SaveChanges();
                        return RedirectToAction("Contactos", new { id = contacto.cn_asociado, nom = Convert.ToString(Session["asociado"]) });
                    }
                    else
                        throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
                }
                catch (Exception ex)
                {
                    return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea", myDat });
                }
            }
            return View(contacto);
        }

        public ActionResult CEdit(short? id, string aso, string nom)
        {
            Contacto dato = new Contacto();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                myDat = "Edita contacto: " + id.ToString() + " - " + aso + " - " + nom + " / sp_Busqueda_Contacto";
                if (Session[myModulo].ToString().Substring(26, 1) == "1")
                {
                    ViewBag.CO = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["CO"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["CO"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    ViewData["idproveedor"] = aso;
                    ViewData["asociado"] = nom;
                    Session["asociado"] = nom;
                    ObjectResult resultado = db.sp_Busqueda_Contacto(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Contacto: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_Contacto_Result re in resultado)
                    {
                        dato = CargaContactos(re);
                    }
                    if (dato == null)
                        throw new System.InvalidOperationException("Error al obtener registro", new Exception(""));
                }
                else
                    throw new System.InvalidOperationException("No tiene acceso a la opcion de actualización");
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Edita contacto"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
            }
            return View(dato);
        }

        private Contacto CargaContactos(sp_Busqueda_Contacto_Result parResult)
        {
            Contacto ca = new Contacto();

            ca.cn_empresa = parResult.Empresa;
            ca.cn_tipoAsociado = parResult.TipoAsociado;
            ca.cn_idContacto = parResult.IdContacto;
            ca.cn_tipoContacto = parResult.TipoContacto;
            ca.cn_asociado = parResult.Asociado;
            ca.cn_Contacto = parResult.Contacto;
            ca.cn_Contacto2 = parResult.Contacto2;
            ca.cn_timestamp = parResult.TimeStamp;
            return ca;
        }

        // POST: DetCatalogoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CEdit([Bind(Include = "cn_empresa,cn_idContacto,cn_tipoAsociado,cn_asociado,cn_tipoContacto,cn_Contacto,cn_Contacto2,cn_fechaing,cn_fechamod,cn_usuarioing,cn_usuariomod,cn_maquinaing,cn_maquinamod,cn_estado,cn_timestamp")] Contacto contacto)
        {
            try
            {
                myDat = "Edita contacto / sp_ABC_Contacto";
                tsp = Convert.ToBase64String(contacto.cn_timestamp as byte[]);
                codigoC.Value = contacto.cn_idContacto;
                contacto.cn_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_Contacto(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigoC, contacto.cn_tipoAsociado, contacto.cn_asociado, contacto.cn_tipoContacto, contacto.cn_Contacto, contacto.cn_Contacto2, contacto.cn_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Contacto: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigoC, contacto.cn_tipoAsociado, contacto.cn_asociado, contacto.cn_tipoContacto, contacto.cn_Contacto, contacto.cn_Contacto2, contacto.cn_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                    return RedirectToAction("Contactos", new { id = contacto.cn_asociado, nom = Convert.ToString(Session["asociado"]) });
                }
                else
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
            }
        }

        public ActionResult CDelete(short? id, string aso, string nom)
        {
            Contacto dato = new Contacto();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                myDat = "Dar de baja contacto: " + id.ToString() + " - " + aso + " - " + nom + " / sp_Busqueda_Contacto";
                if (Session[myModulo].ToString().Substring(27, 1) == "1")
                {
                    ViewBag.idcon = id;
                    ViewData["idproveedor"] = aso;
                    ViewData["asociado"] = nom;
                    ObjectResult resultado = db.sp_Busqueda_Contacto(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Contacto: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_Contacto_Result re in resultado)
                    {
                        dato = CargaContactos(re);
                    }
                    if (dato == null)
                        throw new System.InvalidOperationException("Error al obtener registro", new Exception(""));
                }
                else
                    throw new System.InvalidOperationException("No tiene permitido dar de baja el registro");
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "DeBaja", myDat });
            }
            return View(dato);
        }

        // POST: DetCatalogoes/Delete/5
        [HttpPost, ActionName("CDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult CDeleteConfirmed(short? id, string aso, string nom)
        {
            Contacto dato = new Contacto();
            try
            {
                myDat = "Confirmar dar de baja contacto: " + id.ToString() + " - " + aso + " - " + nom + " / sp_ABC_Contacto";
                ObjectResult resultado = db.sp_Busqueda_Contacto(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Contacto: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, "-> R: " + validad.getResponse(error)));

                foreach (sp_Busqueda_Contacto_Result re in resultado)
                {
                    dato = CargaContactos(re);
                    tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                }

                codigoC.Value = dato.cn_idContacto;
                int result = db.sp_ABC_Contacto(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigoC, dato.cn_tipoAsociado, dato.cn_asociado, dato.cn_tipoContacto, dato.cn_Contacto, dato.cn_Contacto2, dato.cn_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Contacto: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigoC, dato.cn_tipoAsociado, dato.cn_asociado, dato.cn_tipoContacto, dato.cn_Contacto, dato.cn_Contacto2, dato.cn_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                }
                else
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));

            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "DeBaja", myDat });
            }
            return RedirectToAction("Contactos", new { id = dato.cn_asociado, nom = Convert.ToString(Session["asociado"]) });
        }



        // GET: Proveedors/Create
        public ActionResult Create()
        {
            try
            {
                if (Session[myModulo].ToString().Substring(1, 1) == "1")
                {
                    ViewBag.TP = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["TP"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["TP"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea registro", myDat });
            }
            return View();
        }

        // POST: Proveedors/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "pv_empresa,pv_proveedor,pv_tipoProveedor,pv_nombre1,pv_nombre2,pv_nit,pv_fechaing,pv_fechamod,pv_usuarioing,pv_usuariomod,pv_maquinaing,pv_maquinamod,pv_estado,pv_timestamp")] Proveedor proveedor)
        {
            try
            {
                myDat = "Crear nuevo proveedor / sp_ABC_Proveedor";
                if (ModelState.IsValid)
                {
                    proveedor.pv_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_Proveedor(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo, proveedor.pv_tipoProveedor, proveedor.pv_nombre1,
                                                   proveedor.pv_nombre2, proveedor.pv_nit, proveedor.pv_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Proveedor: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo.Value, proveedor.pv_tipoProveedor, proveedor.pv_nombre1,
                                                   proveedor.pv_nombre2, proveedor.pv_nit, proveedor.pv_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Crear nuevo proveedor"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea", myDat });
            }

            return View(proveedor);
        }

        // GET: Proveedors/Edit/5
        public ActionResult Edit(short? id)
        {
            Proveedor proveedor = new Proveedor();
            if (id == null)
            {
                return View("Error");
            }

            try
            {
                myDat = "Actualizar datos de proveedor: " + id.ToString();
                if (Session[myModulo].ToString().Substring(2, 1) == "1")
                {
                    ViewBag.TP = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["TP"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["TP"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));

                    proveedor = db.Proveedor.Where(a => a.pv_proveedor == id).First();
                    if (proveedor == null)
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
            return View(proveedor);
        }

        // POST: Proveedors/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "pv_empresa,pv_proveedor,pv_tipoProveedor,pv_nombre1,pv_nombre2,pv_nit,pv_fechaing,pv_fechamod,pv_usuarioing,pv_usuariomod,pv_maquinaing,pv_maquinamod,pv_estado,pv_timestamp")] Proveedor proveedor)
        {
            try
            {
                myDat = "Editar proveedor / sp_ABC_Proveedor";
                tsp = Convert.ToBase64String(proveedor.pv_timestamp as byte[]);
                codigo.Value = proveedor.pv_proveedor;
                proveedor.pv_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_Proveedor(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo, proveedor.pv_tipoProveedor, proveedor.pv_nombre1,
                                               proveedor.pv_nombre2, proveedor.pv_nit, proveedor.pv_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Proveedor: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo.Value, proveedor.pv_tipoProveedor, proveedor.pv_nombre1,
                                               proveedor.pv_nombre2, proveedor.pv_nit, proveedor.pv_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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

        // GET: Proveedors/Delete/5
        public ActionResult Delete(short? id)
        {
            Proveedor proveedor = new Proveedor();
            if (id == null)
            {
                return View("Error");
            }

            try
            {
                myDat = "Dar de baja de proveedor: " + id.ToString();
                if (Session[myModulo].ToString().Substring(3, 1) == "1")
                {
                    proveedor = db.Proveedor.Where(a => a.pv_proveedor == id).First();
                    if (proveedor == null)
                    {
                        return View("Error");
                    }
                }
                else
                    throw new System.InvalidOperationException("-No tiene acceso a esta opción-", new Exception(""));
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Da de baja de proveedor"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "DeBaja", myDat });
            }
            return View(proveedor);
        }

        // POST: Proveedors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            Proveedor proveedor = new Proveedor();
            try
            {
                myDat = "Confirma dar de baja proveedor: " + id.ToString() + " / sp_Busqueda_Proveedor";
                ObjectResult resultado = db.sp_Busqueda_Proveedor(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Proveedor: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_Proveedor_Result re in resultado)
                {
                    proveedor = CargaProveedor(re);
                    tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                }

                codigo.Value = id;
                proveedor.pv_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_Proveedor(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo, proveedor.pv_tipoProveedor, proveedor.pv_nombre1,
                                               proveedor.pv_nombre2, proveedor.pv_nit, proveedor.pv_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Proveedor: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo.Value, proveedor.pv_tipoProveedor, proveedor.pv_nombre1,
                                               proveedor.pv_nombre2, proveedor.pv_nit, proveedor.pv_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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

        private Proveedor CargaProveedor(sp_Busqueda_Proveedor_Result parResult)
        {
            Proveedor pf = new Proveedor();

            pf.pv_empresa = parResult.Empresa;
            pf.pv_proveedor = parResult.Proveedor;
            pf.pv_tipoProveedor = parResult.TipoProveedor;
            pf.pv_timestamp = parResult.TimeStamp;
            return pf;
        }

        public ActionResult Report(string id)
        {
            try
            {
                if (Session[myModulo].ToString().Substring(19, 1) == "1")
                {
                    LocalReport lr = new LocalReport();
                    string path = Path.Combine(Server.MapPath("~/Reports"), "Proveedores.rdlc");
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        return View("Index");
                    }
                    List<Proveedor> cm = new List<Proveedor>();
                    using (SSS_PERSONASEntities dc = new Models.SSS_PERSONASEntities())
                    {
                        cm = dc.Proveedor.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DProveedores", cm);
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

                    myDat = "Reporte de proveedores";
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
