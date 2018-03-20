using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Core.Objects;
using Microsoft.Reporting.WebForms;
using System.IO;
using WebGrupo3S.Models;
using WebGrupo3S.Helpers;

namespace WebGrupo3S.Views
{
    public class CabCatalogoesController : Controller
    {
        private SSS_PERSONASEntities dbP = new SSS_PERSONASEntities();
        private SSS_COMPLEMENTOSEntities db = new SSS_COMPLEMENTOSEntities();
        private string myModulo = "Catalogos";
        private string mycatalogo = "Mantenimiento Catálogos";
        private string myDat = "";
        public ConstantesP coP = new ConstantesP();
        public String tsp = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("cc_IdCatalogo", typeof(int));

   
        public class CatalogosSearchResultModel
        {
            public IEnumerable<sp_Busqueda_CabCatalogo_Result> CatalogosResults { get; set; }
        }

        // GET: CabCatalogoes
        public ActionResult Index()
        {
            validad ac = new validad();
            var CatalogosSearch = new CatalogosSearchResultModel();
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
                            myDat = "Catalogos / sp_Busqueda_CabCatalogo";
                            ViewBag.nuevo = Session[myModulo].ToString().Substring(1, 1);
                            ViewBag.editar = Session[myModulo].ToString().Substring(2, 1);
                            ViewBag.debaja = Session[myModulo].ToString().Substring(3, 1);
                            ViewBag.detalle = Session[myModulo].ToString().Substring(10, 1);
                            ViewBag.reporte = Session[myModulo].ToString().Substring(19, 1);
                            CatalogosSearch.CatalogosResults = db.sp_Busqueda_CabCatalogo(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, error).ToList();
                            WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_CabCatalogo: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                        }
                        catch (Exception ex)
                        {
                            return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Lista", myDat });
                        }
                        return View(CatalogosSearch.CatalogosResults.ToList());
                    }
                    else
                        throw new System.InvalidOperationException("-Ingreso a opción denegado!-", new Exception(""));
                }
                else
                    throw new System.InvalidOperationException("-Ingrese al Sistema para poder Ingresar a esta Opción-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.Left(2048) : "", modulo = myModulo, opcion = "Lista", myDat });
            }
        }

        public ActionResult Regresa()
        {
            return RedirectToAction("Index");
        }

        public class DetSearchResultModel
        {
            public IEnumerable<sp_Busqueda_DetCatalogo_Result> detResults { get; set; }
        }

        public ActionResult DCreate(short? id, string cat, string val1)
        {
            try
            {
                myDat = "Crear nuevo codigo = " + id.ToString() + " - de catalogo: " + cat;
                DetCatalogo Catalogo = new DetCatalogo();

                    Catalogo.cd_IdCatalogo = Convert.ToInt16(id);
                    ViewBag.idcat = id;
                    ViewBag.catalogo = cat;
                    ViewBag.valor1 = val1;
                    ViewData["Catalogo"] = cat;
                    Session["Catalogo"] = cat;
                    return View(Catalogo);

            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Agregar", myDat });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DCreate([Bind(Include = "cd_empresa,cd_IdCatalogo,cd_codigo,cd_valor,cd_valor1,cd_valor2,cd_valor3,cd_fechaing,cd_fechamod,cd_usuarioing,cd_usuariomod,cd_maquinaing,cd_maquinamod,cd_estado,cd_timestamp")] DetCatalogo detCatalogo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    myDat = "Crear nuevo codigo de catalogo / sp_ABC_DetCatalogo";
                    DetCatalogo Catalogo = new DetCatalogo();
                    detCatalogo.cd_fechaing = System.DateTime.Now;
                    detCatalogo.cd_usuarioing = Session["UserName"].ToString();
                    codigo.Value = detCatalogo.cd_IdCatalogo;
                    int result = db.sp_ABC_DetCatalogo(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), detCatalogo.cd_IdCatalogo, detCatalogo.cd_codigo, detCatalogo.cd_valor, detCatalogo.cd_valor1, detCatalogo.cd_valor2, detCatalogo.cd_valor3, detCatalogo.cd_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_DetCatalogo: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), detCatalogo.cd_IdCatalogo, detCatalogo.cd_codigo, detCatalogo.cd_valor, detCatalogo.cd_valor1, detCatalogo.cd_valor2, detCatalogo.cd_valor3, detCatalogo.cd_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                    if (error.Value.ToString() == "")
                    {
                        db.SaveChanges();
                        return RedirectToAction("Details", new { id = detCatalogo.cd_IdCatalogo, cat = Convert.ToString(Session["Catalogo"]) });
                    }
                    else
                        throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
                }
                catch (Exception ex)
                {
                    //                    return View("Error", new HandleErrorInfo(ex, myModulo, "Crear nuevo codigo de catalago"));
                    return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Agregar", myDat });
                }
            }

            return View(detCatalogo);
        }

        public ActionResult DDelete(short? id, String cod, string ncat, string val1)
        {
            DetCatalogo detCatalogo = new DetCatalogo();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                myDat = "Dar de baja codigo: " + id.ToString() + " - de catalogo: " + ncat + " / sp_Busqueda_DetCatalogo";

                ViewBag.idcat = id;
                ViewBag.catalogo = ncat;
                ViewBag.valor1 = val1;
                ViewData["Catalogo"] = ncat;
                ObjectResult resultado = db.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), id, cod, null, null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, cod, null, null, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_DetCatalogo_Result re in resultado)
                {
                    detCatalogo = detCargaCatalogo(re);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Debaja", myDat });
            }
            return View(detCatalogo);
        }

        // POST: DetCatalogoes/Delete/5
        [HttpPost, ActionName("DDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DDeleteConfirmed(short id, string cod, string ncat)
        {
            DetCatalogo detCatalogo = new DetCatalogo();
            try
            {
                myDat = "Confirmar dar de baja catalogo: " + id.ToString() + " / sp_Busqueda_DetCatalogo";
                ObjectResult resultado = db.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), id, cod, null, null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, cod, null, null, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_DetCatalogo_Result re in resultado)
                {
                    detCatalogo = detCargaCatalogo(re);
                    tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                }

                codigo.Value = detCatalogo.cd_IdCatalogo;
                detCatalogo.cd_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_DetCatalogo(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), id, cod, detCatalogo.cd_valor, detCatalogo.cd_valor1, detCatalogo.cd_valor2, detCatalogo.cd_valor3, detCatalogo.cd_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_DetCatalogo: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), id, cod, detCatalogo.cd_valor, detCatalogo.cd_valor1, detCatalogo.cd_valor2, detCatalogo.cd_valor3, detCatalogo.cd_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                }
                else
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));

            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Confirmacion Debaja", myDat });
            }
            return RedirectToAction("Details", new { id = detCatalogo.cd_IdCatalogo, cat = ncat });
        }


        public ActionResult DEdit(short? id, string cod, string ncat, string val1)
        {
            DetCatalogo detCatalogo = new DetCatalogo();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                myDat = "Edita codigo: " + cod + ", catalogo: " + ncat + " / sp_Busqueda_DetCatalogo";

                    ViewBag.idcat = id;
                    ViewBag.catalogo = ncat;
                    ViewBag.valor1 = val1;
                    ViewData["Catalogo"] = ncat;
                    Session["Catalogo"] = ncat;
                    ObjectResult resultado = db.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), id, cod, null, null, null, null, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, cod, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_DetCatalogo_Result re in resultado)
                    {
                        detCatalogo = detCargaCatalogo(re);
                    }
                    if (detCatalogo == null)
                        throw new System.InvalidOperationException("Error al obtener registro", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Editar", myDat });
            }
            return View(detCatalogo);
        }

        // POST: DetCatalogoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DEdit([Bind(Include = "cd_empresa,cd_IdCatalogo,cd_codigo,cd_valor,cd_valor1,cd_valor2,cd_valor3,cd_fechaing,cd_fechamod,cd_usuarioing,cd_usuariomod,cd_maquinaing,cd_maquinamod,cd_estado,cd_timestamp")] DetCatalogo detCatalogo)
        {
            try
            {
                myDat = "Edita codigo de catalogo / sp_ABC_DetCatalogo";
                tsp = Convert.ToBase64String(detCatalogo.cd_timestamp as byte[]);
                codigo.Value = detCatalogo.cd_IdCatalogo;
                detCatalogo.cd_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_DetCatalogo(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), detCatalogo.cd_IdCatalogo, detCatalogo.cd_codigo, detCatalogo.cd_valor, detCatalogo.cd_valor1, detCatalogo.cd_valor2, detCatalogo.cd_valor3, detCatalogo.cd_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_DetCatalogo: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), detCatalogo.cd_IdCatalogo, detCatalogo.cd_codigo, detCatalogo.cd_valor, detCatalogo.cd_valor1, detCatalogo.cd_valor2, detCatalogo.cd_valor3, detCatalogo.cd_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = detCatalogo.cd_IdCatalogo, cat = Convert.ToString(Session["Catalogo"]) });
                }
                else
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
            }
            catch (Exception ex)
            {
//                return View("Error", new HandleErrorInfo(ex, myModulo, "Edita codigo de catalogo"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Editar", myDat });
            }
        }



        // GET: CabCatalogoes/Details/5
        public ActionResult Details(short? id, string cat, string val1, string val2, string val3)
        {
            var detSearch = new DetSearchResultModel();
            try
            {
                myDat = "Detalle de catalogo: " + id.ToString() + " - " + cat + " / sp_Busqueda_DetCatalogo";
                if (Session[myModulo].ToString().Substring(10, 1) == "1") { 
                    ViewData["nombre"] = Session["UserName"];
                    ViewBag.idcat = id;
                    ViewBag.catalogo = cat;
                    ViewBag.valor1 = val1;
                    ViewData["Catalogo"] = cat;
                    if (val1 != null)
                        ViewData["valor1"] = val1;
                    else
                        ViewData["valor1"] = "Valor1";

                    if (val2 != null)
                        ViewData["valor2"] = val2;
                    else
                        ViewData["valor2"] = "Valor2";

                    if (val3 != null)
                        ViewData["valor3"] = val3;
                    else
                        ViewData["valor3"] = "Valor3";
                    ViewBag.nuevo = Session[myModulo].ToString().Substring(25, 1);
                    ViewBag.editar = Session[myModulo].ToString().Substring(26, 1);
                    ViewBag.debaja = Session[myModulo].ToString().Substring(27, 1);
                    ViewBag.reporte = Session[myModulo].ToString().Substring(28, 1);
                    detSearch.detResults = db.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, error).ToList();
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
               }
                else
                    throw new System.InvalidOperationException("-Ingreso a opción denegada!-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Ver detalle", myDat });
            }
            return View(detSearch.detResults.ToList());
        }

        // GET: CabCatalogoes/Create
        public ActionResult Create()
        {
            try
            {
                myDat = "Crear nuevo codigo";
                if (Session[myModulo].ToString().Substring(1, 1) == "0")
                    throw new System.InvalidOperationException("-No tiene acceso a crear nuevo registros-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crear", myDat });
            }
            return View();
        }

        // POST: CabCatalogoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cc_empresa,cc_IdCatalogo,cc_nombre,cc_nomVal1,cc_nomVal2,cc_nomVal3,cc_fechaing,cc_fechamod,cc_usuarioing,cc_usuariomod,cc_maquinaing,cc_maquinamod,cc_estado,cc_timestamp")] CabCatalogo cabCatalogo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    myDat = "Crear nuevo / sp_ABC_CabCatalogo";
                    cabCatalogo.cc_fechaing = System.DateTime.Now;
                    cabCatalogo.cc_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_CabCatalogo(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo, cabCatalogo.cc_nombre, cabCatalogo.cc_nomVal1, cabCatalogo.cc_nomVal2, cabCatalogo.cc_nomVal3, cabCatalogo.cc_usuarioing,  tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_CabCatalogo: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo.Value, cabCatalogo.cc_nombre, cabCatalogo.cc_nomVal1, cabCatalogo.cc_nomVal2, cabCatalogo.cc_nomVal3, cabCatalogo.cc_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                    return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crear", myDat });
                }
            }

            return View(cabCatalogo);
        }

        // GET: CabCatalogoes/Edit/5
        public ActionResult Edit(short? id, string cat)
        {
            CabCatalogo catalogo = new CabCatalogo();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                myDat = "Editar catalogo: " + id.ToString() + " - " + cat + " / sp_Busqueda_CabCatalogo";
                if (Session[myModulo].ToString().Substring(2, 1) == "1")
                {
                    ObjectResult resultado = db.sp_Busqueda_CabCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), id,null, null, null, null, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_CabCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_CabCatalogo_Result re in resultado)
                    {
                        catalogo = CargaCatalogo(re);
                    }
                    if (catalogo == null)
                        throw new System.InvalidOperationException("-Error al obtener registro-", new Exception(""));
                }
                else
                    throw new System.InvalidOperationException("-No tiene acceso a la opcion de actualización-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Editar", myDat });
            }
            return View(catalogo);
        }

        private CabCatalogo CargaCatalogo(sp_Busqueda_CabCatalogo_Result parResult)
        {
            CabCatalogo ca = new CabCatalogo();

            ca.cc_empresa = parResult.Empresa;
            ca.cc_IdCatalogo = parResult.IdCatalogo;
            ca.cc_nombre = parResult.NombreCatalogo;
            ca.cc_nomVal1 = parResult.NombreValor1;
            ca.cc_nomVal2 = parResult.NombreValor2;
            ca.cc_nomVal3 = parResult.NombreValor3;
            ca.cc_timestamp = parResult.TimeStamp;
            return ca;
        }

        private DetCatalogo detCargaCatalogo(sp_Busqueda_DetCatalogo_Result parResult)
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

        // POST: CabCatalogoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cc_empresa,cc_IdCatalogo,cc_nombre,cc_nomVal1,cc_nomVal2,cc_nomVal3,cc_fechaing,cc_fechamod,cc_usuarioing,cc_usuariomod,cc_maquinaing,cc_maquinamod,cc_estado,cc_timestamp")] CabCatalogo cabCatalogo)
        {
            try
            {
                myDat = "Editar / sp_ABC_CabCatalogo";
                tsp = Convert.ToBase64String(cabCatalogo.cc_timestamp as byte[]);
                codigo.Value = cabCatalogo.cc_IdCatalogo;
                cabCatalogo.cc_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_CabCatalogo(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo, cabCatalogo.cc_nombre, cabCatalogo.cc_nomVal1, cabCatalogo.cc_nomVal2, cabCatalogo.cc_nomVal3, cabCatalogo.cc_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_CabCatalogo: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo.Value, cabCatalogo.cc_nombre, cabCatalogo.cc_nomVal1, cabCatalogo.cc_nomVal2, cabCatalogo.cc_nomVal3, cabCatalogo.cc_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                }
                else
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Editar", myDat });
            }

            return RedirectToAction("Index");
        }

        // GET: CabCatalogoes/Delete/5
        public ActionResult Delete(short? id, string cat)
        {
            CabCatalogo catalogo = new CabCatalogo();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                myDat = "Dar de baja catalogo: " + id.ToString() + " - " + cat + " / sp_Busqueda_CabCatalogo";
                if (Session[myModulo].ToString().Substring(3, 1) == "1")
                {
                    ObjectResult resultado = db.sp_Busqueda_CabCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), id, null,null, null, null, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_CabCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_CabCatalogo_Result re in resultado)
                    {
                        catalogo = CargaCatalogo(re);
                    }
                    if (catalogo == null)
                        throw new System.InvalidOperationException("-Error al obtener registro-", new Exception(""));
                }
                else
                    throw new System.InvalidOperationException("-No tiene permitido dar de baja el registro-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "DeBaja", myDat });
            }
            return View(catalogo);
        }

        // POST: CabCatalogoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id, string cat)
        {
            CabCatalogo catalogo = new CabCatalogo();
            try
            {
                myDat = "Confirmar dar de baja de catalogo: " + id.ToString() + " - " + cat + " / sp_Busqueda_CabCatalogo";

                ObjectResult resultado = db.sp_Busqueda_CabCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_CabCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, "-> R: " + validad.getResponse(error)));

                foreach (sp_Busqueda_CabCatalogo_Result re in resultado)
                {
                    catalogo = CargaCatalogo(re);
                    tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                }

                codigo.Value = catalogo.cc_IdCatalogo;
                catalogo.cc_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_CabCatalogo(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo, catalogo.cc_nombre, catalogo.cc_nomVal1, catalogo.cc_nomVal2, catalogo.cc_nomVal3, catalogo.cc_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_CabCatalogo: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo.Value, catalogo.cc_nombre, catalogo.cc_nomVal1, catalogo.cc_nomVal2, catalogo.cc_nomVal3, catalogo.cc_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                    string path = Path.Combine(Server.MapPath("~/Reports"), "Catalogos.rdlc");
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        return View("Index");
                    }
                    List<CabCatalogo> cm = new List<CabCatalogo>();
                    using (SSS_COMPLEMENTOSEntities dc = new SSS_COMPLEMENTOSEntities())
                    {
                        cm = dc.CabCatalogoes.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DCatalogos", cm);
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

                    myDat = "Reporte";
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
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Report", myDat });
            }
        }

        public ActionResult ReportD(string id, int cat, string nombre)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reports"), "Detalle de catalogo.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("Details");
            }
            List<DetCatalogo> cm = new List<DetCatalogo>();
            using (SSS_COMPLEMENTOSEntities dc = new SSS_COMPLEMENTOSEntities())
            {
                cm = dc.DetCatalogoes.Where(p => p.cd_IdCatalogo == cat).ToList();
            }

            ReportDataSource rd = new ReportDataSource("DDetalle", cm);
            lr.DataSources.Add(rd);
            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;

            ReportParameter p1 = new ReportParameter("prUsuario", Session["UserName"].ToString());
            ReportParameter p2 = new ReportParameter("prMaquina", Environment.MachineName.ToString());
            ReportParameter p3 = new ReportParameter("prCatalogo", nombre);
            lr.SetParameters(new ReportParameter[] { p1, p2, p3 });
            string deviceInfo =

            "<DeviceInfo>" +
            " <OutputFormat>" + id + "</OutputFormat>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;

            try
            {
                myDat = "Reporte";
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
