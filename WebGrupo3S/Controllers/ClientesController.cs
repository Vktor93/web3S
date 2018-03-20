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
    public class ClientesController : Controller
    {
        private SSS_PERSONASEntities db = new SSS_PERSONASEntities();
        private SSS_COMPLEMENTOSEntities dbC = new SSS_COMPLEMENTOSEntities();
        private string myModulo = "Clientes";
        private string mycatalogo = "Mantenimiento clientes";
        private string myDat = "";
        public ConstantesP coP = new ConstantesP();
        public String tsp = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("cl_cliente", typeof(int));
        ObjectParameter codigoC = new ObjectParameter("cn_idcontacto", typeof(int));
        ObjectParameter codigoD = new ObjectParameter("di_idDireccion", typeof(int));

        public class ClientesSearchResultModel
        {
            public IEnumerable<sp_Busqueda_Cliente_Result> ClientesResults { get; set; }
        }
        // GET: Clientes
        public ActionResult Index()
        {
            validad ac = new validad();
            var ClientesSearch = new ClientesSearchResultModel();
            try
            {
                ac.seteAccesos(mycatalogo, Session["perfil"].ToString());
                Session[myModulo] = ac.acc;
                ViewData["nombre"] = Session["UserName"];
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
                        ClientesSearch.ClientesResults = db.sp_Busqueda_Cliente(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, null, null, null, null, null, null, error).ToList();
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Cliente: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
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
            return View(ClientesSearch.ClientesResults.ToList());
        }

        public void buscaCatalogos()
        {
            Session["TC"] = TraeIdCatalogo("TipoCliente");
            Session["CC"] = TraeIdCatalogo("ClasificacionCliente");
            Session["TI"] = TraeIdCatalogo("TipoIdentifica");
            Session["GE"] = TraeIdCatalogo("Genero");
            Session["OP"] = TraeIdCatalogo("Ocupacion");
            Session["EC"] = TraeIdCatalogo("EstadoCivil");
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
                if (Session[myModulo].ToString().Substring(7, 1) == "1")
                {
                    myDat = "Busca Direccion: " + id.ToString() + " - " + nom + " / sp_Busqueda_Direccion";
                    ViewBag.idcliente = id;
                    ViewBag.nombre = nom;
                    ViewData["Nombre"] = nom;
                    ViewBag.nuevo = Session[myModulo].ToString().Substring(31, 1);
                    ViewBag.editar = Session[myModulo].ToString().Substring(32, 1);
                    ViewBag.debaja = Session[myModulo].ToString().Substring(33, 1);
                    direccionSearch.direccionResults = db.sp_Busqueda_Direccion(2, "", Convert.ToInt16(coP.cls_empresa), null, "CL", id, null, null, null, null, null, null, null, error).ToList();
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Direccion: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, "CL", id, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
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

            ObjectResult resultado = dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DE"]), null,null, id, null, null, error);
            foreach (sp_Busqueda_DetCatalogo_Result re in resultado)
            {
                depto.Add(new SelectListItem { Text = re.Valor , Value = re.Codigo });
            }
            return Json(new SelectList(depto, "Value", "Text"));
        }

        public JsonResult GetMuni(string id)
        {
            List<SelectListItem> muni = new List<SelectListItem>();

            ObjectResult resultado = dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MU"]), null, null, id, null, null, error);
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
                    dato.di_TipoAsociado = "CL";
                    dato.di_pais = "502";
                    dato.di_departamento = "01";
                    dato.di_municipio = "01";
                    ViewBag.idcliente = id;
                    ViewBag.nombre = aso;
                    Session["asociado"] = aso;
                    ViewBag.DI = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DI"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    ViewBag.MU = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MU"]), null, null, "01", null, null, error).ToList(), "Codigo", "Valor");
                    ViewBag.DE = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DE"]), null, null, "502", null, null, error).ToList(), "Codigo", "Valor");
                    ViewBag.PA = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["PA"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                }
                else
                    throw new System.InvalidOperationException("No tiene acceso a la opcion de actualización");
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crear", myDat });
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
                    myDat = "Direcciones / sp_ABC_Direccion";
                    codigoD.Value = direccion.di_IdDireccion;
                    direccion.di_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_Direccion(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigoD, direccion.di_TipoAsociado, direccion.di_asociado, direccion.di_TipoDireccion, direccion.di_Direccion, direccion.di_Direccion2, direccion.di_zona,direccion.di_municipio,direccion.di_departamento,direccion.di_pais, direccion.di_usuarioing, tsp, error);
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
                    return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crear", myDat });
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
                myDat = "Edita Contacto: " + id.ToString() + " - " + aso + " - " + nom + "  / sp_Busqueda_Direccion";
                if (Session[myModulo].ToString().Substring(32, 1) == "1")
                {
                    ViewData["idcliente"] = aso;
                    ViewData["asociado"] = nom;
                    Session["asociado"] = nom;
                    ObjectResult resultado = db.sp_Busqueda_Direccion(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, null,null,null,null,error);
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
                        ViewBag.PA = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["PA"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                        ViewBag.MU = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MU"]), null, null,dato.di_departamento, null, null, error).ToList(), "Codigo", "Valor");
                        ViewBag.DE = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DE"]), null, null,dato.di_pais, null, null, error).ToList(), "Codigo", "Valor");
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
                myDat = "Dirección / sp_ABC_Direccion";
                tsp = Convert.ToBase64String(direccion.di_timestamp as byte[]);
                codigoD.Value = direccion.di_IdDireccion;
                direccion.di_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_Direccion(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigoD, direccion.di_TipoAsociado, direccion.di_asociado, direccion.di_TipoDireccion, direccion.di_Direccion, direccion.di_Direccion2, direccion.di_zona,direccion.di_municipio,direccion.di_departamento,direccion.di_pais, direccion.di_usuarioing, tsp, error);
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
                    ViewData["idcliente"] = aso;
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
                        ViewBag.PA = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["PA"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                        ViewBag.MU = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MU"]), null, null, dato.di_departamento, null, null, error).ToList(), "Codigo", "Valor");
                        ViewBag.DE = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DE"]), null, null, dato.di_pais, null, null, error).ToList(), "Codigo", "Valor");
                    }
                    if (dato == null)
                        throw new System.InvalidOperationException("Error al obtener registro", new Exception(""));
                }
                else
                    throw new System.InvalidOperationException("No tiene permitido dar de baja el registro");
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Debaja", myDat });
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
                myDat = "Confirmar de baja de direccion: " + id.ToString() + " - " + aso + " - " + nom + " / sp_Busqueda_Direccion";
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
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Debaja", myDat });
            }
            return RedirectToAction("Direcciones", new { id = dato.di_asociado, nom = Convert.ToString(Session["asociado"]) });
        }


        public ActionResult Contactos(short? id, string nom)
        {
            var contactoSearch = new contactosSearchResultModel();
            try
            {
                myDat = "Lista de perfiles de contacto: " + id.ToString() + " - " + nom + " / sp_Busqueda_Contacto";
                if (Session[myModulo].ToString().Substring(8, 1) == "1")
                {
                    ViewBag.idcliente = id;
                    ViewBag.nombre = nom;
                    ViewData["Nombre"] = nom;
                    ViewBag.nuevo = Session[myModulo].ToString().Substring(25, 1);
                    ViewBag.editar = Session[myModulo].ToString().Substring(26, 1);
                    ViewBag.debaja = Session[myModulo].ToString().Substring(27, 1);
                    contactoSearch.contactoResults = db.sp_Busqueda_Contacto(2, "", Convert.ToInt16(coP.cls_empresa), null, "CL", id, null, null, null, error).ToList();
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Contacto: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, "CL", id, null, null, null, "-> R: " + validad.getResponse(error)));
                }
                else
                    throw new System.InvalidOperationException("-Ingreso a opción denegada!-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Contactos", myDat });
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
                    dato.cn_tipoAsociado = "CL";

                    ViewBag.idcliente = id;
                    ViewBag.nombre = aso;
                    Session["asociado"] = aso;
                    ViewBag.CO = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["CO"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                }
                else
                    throw new System.InvalidOperationException("-Ingreso a opción denegada!-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea contaco", myDat });
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
                    myDat = "Asociado / sp_ABC_Contacto";
                    codigoC.Value = contacto.cn_idContacto;
                    contacto.cn_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_Contacto(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigoC, contacto.cn_tipoAsociado, contacto.cn_asociado , contacto.cn_tipoContacto,contacto.cn_Contacto,contacto.cn_Contacto2,contacto.cn_usuarioing, tsp, error);
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
                    return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crear", myDat });
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
                if (Session[myModulo].ToString().Substring(26, 1) == "1")
                {
                    myDat = "Edita contacto: " + id.ToString() + " - " + aso + " - " + nom + " / sp_Busqueda_Contacto";
                    ViewBag.CO = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["CO"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    ViewData["idcliente"] = aso;
                    ViewData["asociado"] = nom;
                    Session["asociado"] = nom;
                    ObjectResult resultado = db.sp_Busqueda_Contacto(2, "", Convert.ToInt16(coP.cls_empresa), id, null,null, null, null, null, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Contacto: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, tsp, "-> R: " + validad.getResponse(error)));
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
                myDat = "Dar de baja Contacto: " + id.ToString() + " - " + aso + " - " + nom + " / sp_Busqueda_Contacto";
                if (Session[myModulo].ToString().Substring(27, 1) == "1")
                {
                    ViewBag.idcon = id;
                    ViewData["idcliente"] = aso;
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
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Debaja", myDat });
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
                myDat = "Confirma de baja de contacto: " + id.ToString() + " - " + aso + " - " + nom + " / sp_Busqueda_Contacto";
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
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Confirmar dar de baja"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Debaja", myDat });
            }
            return RedirectToAction("Contactos", new { id = dato.cn_asociado, nom = Convert.ToString(Session["asociado"]) });
        }


        // GET: Clientes/Create
        public ActionResult Create()
        {
            try
            {
                if (Session[myModulo].ToString().Substring(1, 1) == "1")
                {
                    ViewBag.TC = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["TC"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    ViewBag.TI = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["TI"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    ViewBag.CC = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["CC"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    ViewBag.EC = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["EC"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    ViewBag.GE = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["GE"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    ViewBag.OP = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["OP"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea registro", myDat });

            }
            return View();
        }

        // POST: Clientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cl_empresa,cl_cliente,cl_tipoCliente,cl_ClasificacionCliente,cl_tipoident,cl_identifica,cl_nit,cl_nombre1,cl_nombre2,cl_nombre3,cl_apellido1,cl_apellido2,cl_apecasada,cl_nombrelargo,cl_genero,cl_fechanac,cl_ocupacion,cl_estadocivil,cl_RecibePublicidad,cl_fechaing,cl_fechamod,cl_usuarioing,cl_usuariomod,cl_maquinaing,cl_maquinamod,cl_estado,cl_timestamp")] Cliente cliente)
        {
            try
            {
                myDat = "Crear nuevo Cliente / sp_ABC_Cliente";
                if (ModelState.IsValid)
                {
                    cliente.cl_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_Cliente(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo, cliente.cl_tipoCliente, cliente.cl_ClasificacionCliente,
                                                   cliente.cl_tipoident, cliente.cl_identifica, cliente.cl_nit, cliente.cl_nombre1, cliente.cl_nombre2, cliente.cl_nombre3, cliente.cl_apellido1,
                                                   cliente.cl_apellido1, cliente.cl_apecasada, cliente.cl_genero, cliente.cl_fechanac, cliente.cl_ocupacion, cliente.cl_estadocivil, null, cliente.cl_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Cliente: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo.Value, cliente.cl_tipoCliente, cliente.cl_ClasificacionCliente,
                                                   cliente.cl_tipoident, cliente.cl_identifica, cliente.cl_nit, cliente.cl_nombre1, cliente.cl_nombre2, cliente.cl_nombre3, cliente.cl_apellido1,
                                                   cliente.cl_apellido1, cliente.cl_apecasada, cliente.cl_genero, cliente.cl_fechanac, cliente.cl_ocupacion, cliente.cl_estadocivil, null, cliente.cl_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crear", myDat });
            }

            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public ActionResult Edit(short? id)
        {
            Cliente cliente = new Cliente();
            if (id == null)
            {
                return View("Error");
            }

            try
            {
                myDat = "Actualizar datos de Cliente: " + id.ToString() + " / sp_Busqueda_DetCatalogo";
                if (Session[myModulo].ToString().Substring(2, 1) == "1")
                {
                    ViewBag.TC = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["TC"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    ViewBag.TI = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["TI"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    ViewBag.CC = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["CC"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    ViewBag.EC = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["EC"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    ViewBag.GE = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["GE"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    ViewBag.OP = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["OP"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    cliente = db.Cliente.Where(a => a.cl_cliente == id).First();
                    if (cliente == null)
                    {
                        throw new System.InvalidOperationException("-Error al obtener registro-", new Exception(""));
                    }
                }
                else
                    throw new System.InvalidOperationException("-No tiene acceso a esta opción-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Actualiza", myDat });
            }
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cl_empresa,cl_cliente,cl_tipoCliente,cl_ClasificacionCliente,cl_tipoident,cl_identifica,cl_nit,cl_nombre1,cl_nombre2,cl_nombre3,cl_apellido1,cl_apellido2,cl_apecasada,cl_nombrelargo,cl_genero,cl_fechanac,cl_ocupacion,cl_estadocivil,cl_RecibePublicidad,cl_fechaing,cl_fechamod,cl_usuarioing,cl_usuariomod,cl_maquinaing,cl_maquinamod,cl_estado,cl_timestamp")] Cliente cliente)
        {
            try
            {
                myDat = "Actualizar datos de cliente / sp_ABC_Cliente";
                tsp = Convert.ToBase64String(cliente.cl_timestamp as byte[]);
                codigo.Value = cliente.cl_cliente;
                cliente.cl_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_Cliente(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'),codigo, cliente.cl_tipoCliente, cliente.cl_ClasificacionCliente,
                                               cliente.cl_tipoident, cliente.cl_identifica, cliente.cl_nit, cliente.cl_nombre1, cliente.cl_nombre2, cliente.cl_nombre3, cliente.cl_apellido1,
                                               cliente.cl_apellido1, cliente.cl_apecasada, cliente.cl_genero, cliente.cl_fechanac,cliente.cl_ocupacion,cliente.cl_estadocivil, null, cliente.cl_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Cliente: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo.Value, cliente.cl_tipoCliente, cliente.cl_ClasificacionCliente,
                                               cliente.cl_tipoident, cliente.cl_identifica, cliente.cl_nit, cliente.cl_nombre1, cliente.cl_nombre2, cliente.cl_nombre3, cliente.cl_apellido1,
                                               cliente.cl_apellido1, cliente.cl_apecasada, cliente.cl_genero, cliente.cl_fechanac, cliente.cl_ocupacion, cliente.cl_estadocivil, null, cliente.cl_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Actualizar", myDat });
            }
            return RedirectToAction("Index");
        }

        // GET: Clientes/Delete/5
        public ActionResult Delete(short? id)
        {
            Cliente cliente = new Cliente();
            if (id == null)
            {
                return View("Error");
            }

            try
            {
                myDat = "Dar de baja a cliente: " + id.ToString();
                if (Session[myModulo].ToString().Substring(3, 1) == "1")
                {
                    cliente = db.Cliente.Where(a => a.cl_cliente == id).First();
                    if (cliente == null)
                    {
                        throw new System.InvalidOperationException("-Error al obtener registro-", new Exception(""));
                    }
                }
                else
                    throw new System.InvalidOperationException("-No tiene acceso a esta opción-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Debaja", myDat });
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            Cliente cliente = new Cliente();
            try
            {
                myDat = "Confirma dar de baja de cliente: " + id.ToString() + " / sp_Busqueda_Cliente";
                ObjectResult resultado = db.sp_Busqueda_Cliente(2, "",Convert.ToInt16(coP.cls_empresa), id,null,
                                         null, null, null, null, null, null, null, null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Cliente: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null,
                                         null, null, null, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_Cliente_Result re in resultado)
                {
                    cliente = CargaCliente(re);
                    tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                }

                codigo.Value = cliente.cl_cliente;
                cliente.cl_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_Cliente(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo, cliente.cl_tipoCliente, cliente.cl_ClasificacionCliente,
                               cliente.cl_tipoident, cliente.cl_identifica, cliente.cl_nit, cliente.cl_nombre1, cliente.cl_nombre2, cliente.cl_nombre3, cliente.cl_apellido1,
                               cliente.cl_apellido1, cliente.cl_apecasada, cliente.cl_genero, cliente.cl_fechanac, cliente.cl_ocupacion, cliente.cl_estadocivil, null, cliente.cl_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Cliente: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo.Value, cliente.cl_tipoCliente, cliente.cl_ClasificacionCliente,
                               cliente.cl_tipoident, cliente.cl_identifica, cliente.cl_nit, cliente.cl_nombre1, cliente.cl_nombre2, cliente.cl_nombre3, cliente.cl_apellido1,
                               cliente.cl_apellido1, cliente.cl_apecasada, cliente.cl_genero, cliente.cl_fechanac, cliente.cl_ocupacion, cliente.cl_estadocivil, null, cliente.cl_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Debaja", myDat });
            }
            return RedirectToAction("Index");
        }

        private Cliente CargaCliente(sp_Busqueda_Cliente_Result parResult)
        {
            Cliente pf = new Cliente();

            pf.cl_empresa = parResult.Empresa;
            pf.cl_cliente = parResult.Cliente;
            pf.cl_tipoCliente = parResult.TipoCliente;
            pf.cl_ClasificacionCliente = parResult.ClasificacionCliente;
            pf.cl_tipoident = parResult.TipoIdentificacion;
            pf.cl_nombrelargo = parResult.NombreLargo;
            pf.cl_timestamp = parResult.TimeStamp;
            return pf;
        }

        public ActionResult Report(string id)
        {
            try
            {
                myDat = "Generación de reporte";
                if (Session[myModulo].ToString().Substring(19, 1) == "1")
                {
                    LocalReport lr = new LocalReport();
                    string path = Path.Combine(Server.MapPath("~/Reports"), "Clientes.rdlc");
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        return View("Index");
                    }
                    List<Cliente> cm = new List<Cliente>();
                    using (SSS_PERSONASEntities dc = new Models.SSS_PERSONASEntities())
                    {
                        cm = dc.Cliente.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DClientes", cm);
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

                    myDat = "Reporte de Clientes";
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
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Report"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Report", myDat });
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
