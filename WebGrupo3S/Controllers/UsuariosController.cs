using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebGrupo3S.Models;
using System.Data.Entity.Core.Objects;
using Microsoft.Reporting.WebForms;
using System.IO;
using WebGrupo3S.Helpers;
using System.Collections;

namespace WebGrupo3S.Controllers
{
    public class UsuariosController : Controller
    {
        
        private SSS_PERSONASEntities db = new SSS_PERSONASEntities();
        private SSS_PLANILLAEntities dbE = new SSS_PLANILLAEntities();
        private string myModulo = "Usuarios";
        private string mycatalogo = "Catalogo Usuarios";
        private string myDat = "";
        public ConstantesP coP = new ConstantesP();
        public PermisosP perP = new PermisosP();
        public String tsp = "";
        String dt = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codusuario = new ObjectParameter("us_codUsuario", typeof(int));
        ObjectParameter codigo = new ObjectParameter("pu_CodPerfilUsuario", typeof(int));

        
        // GET: Usuarios
        public ActionResult Index()
        {
            validad ac = new validad();
            try
            {
                ac.seteAccesos(mycatalogo, Session["perfil"].ToString());
                Session[myModulo] = ac.acc;
                ViewData["nombre"] = Session["UserName"];
                myDat = "Lista de Usuarios / sp_CargaPermiso";
                if (Session["UserId"].ToString() != "")
                {
                    if (Session[myModulo].ToString().Substring(0, 1) == "1")
                    {
                        var UsuariosSearch = new UsuariosSearchResultModel();
                        try
                        {
                            ViewBag.nuevo = Session[myModulo].ToString().Substring(1, 1);
                            ViewBag.editar = Session[myModulo].ToString().Substring(2, 1);
                            ViewBag.debaja = Session[myModulo].ToString().Substring(3, 1);
                            ViewBag.perfiles = Session[myModulo].ToString().Substring(4, 1);
                            ViewBag.resetea = Session[myModulo].ToString().Substring(5, 1);
                            ViewBag.reporte = Session[myModulo].ToString().Substring(19, 1);
                            UsuariosSearch.UsuariosResults = db.sp_Busqueda_Usuario(1, "", Convert.ToInt32(coP.cls_empresa), null, null, null, null, null, null, error).ToList();
                            WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Usuario: " + string.Join(",", 1, "", Convert.ToInt32(coP.cls_empresa), null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                        }
                        catch (Exception ex)
                        {
                            return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = "Usuarios", opcion = "Lista", myDat });
                        }
                        return View("Index", UsuariosSearch.UsuariosResults.ToList());
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

        public class UsuariosSearchResultModel
        {
            public IEnumerable<sp_Busqueda_Usuario_Result> UsuariosResults { get; set; }
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            try
            {
                myDat = "Crea Usuario";
                if (Session[myModulo].ToString().Substring(1, 1) == "1")
                {
                    ViewBag.EmpleId = new SelectList(dbE.sp_Busqueda_Empleado(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, error).ToList(), "Empleado", "NombreLargo");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Empleado: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                }
                else
                    throw new System.InvalidOperationException("-No tiene acceso a crear nuevo registros-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea registro", myDat });

            }
            return View();
        }

        // POST: Usuarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "us_empresa,us_IdSucursal,us_codUsuario,us_usuario,us_nombre,us_password,us_estUsuario,us_IdEmpleado,us_resetPass,us_fechaing,us_fechamod,us_usuarioing,us_usuariomod,us_maquinaing,us_maquinamod,us_estado,us_timestamp")] Usuario usuario)
        {
            try
            {
                myDat = "Crea nuevo usuarios / db.sp_ABC_Usuario";
                if (ModelState.IsValid)
                {
                    dt = "";
                    usuario.us_estUsuario = coP.cls_us_estado_activo;
                    usuario.us_fechaing = System.DateTime.Now;
                    usuario.us_usuarioing = Session["UserName"].ToString()  ;
                    int result = db.sp_ABC_Usuario(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), Convert.ToInt16(coP.cls_sucursal), codusuario, usuario.us_usuario,
                                 usuario.us_nombre, usuario.us_password, usuario.us_estUsuario, usuario.us_IdEmpleado, usuario.us_usuarioing, usuario.us_resetPass, dt, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Usuario: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), Convert.ToInt16(coP.cls_sucursal), codusuario.Value.ToString(), usuario.us_usuario,
                                 usuario.us_nombre, "".PadRight(usuario.us_password.Length, '*'), usuario.us_estUsuario, usuario.us_IdEmpleado, usuario.us_usuarioing, usuario.us_resetPass, dt, "-> R: " + validad.getResponse(error)));
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

            return View(usuario);
        }

        public JsonResult TraePerfil()
        {
            perfilusuario dato = new perfilusuario();
            List<SelectListItem> perf = new List<SelectListItem>();
            try
            {
                int idusuario = Convert.ToInt32(Session["idusuario"].ToString());
                ObjectResult resultado = db.sp_Busqueda_PerfilUsuario(3, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, idusuario, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_PerfilUsuario: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, idusuario, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_PerfilUsuario_Result re in resultado)
                {
                    perf.Add(new SelectListItem { Text = re.NombrePerfil, Value = re.CodigoPerfil.ToString() });
                }
            }
            catch (Exception ex)
            {

            }
            return Json(new SelectList(perf, "Value", "Text"));
        }

        public JsonResult AsignaPerfil(int id)
        {
            perfilusuario dato = new perfilusuario();
            List<SelectListItem> perf = new List<SelectListItem>();
            try
            {
                int idusuario = Convert.ToInt32(Session["idusuario"].ToString());
                ObjectResult resultado = db.sp_Busqueda_Perfil(2, "", Convert.ToInt16(coP.cls_empresa),id, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Perfil: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_Perfil_Result re in resultado)
                {
                    dato.pu_codPerfil = re.CodigoPerfil;
                    dato.pu_NombrePerfil = re.NombrePerfil;
                }

                dato.pu_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_PerfilUsuario(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), Convert.ToInt16(coP.cls_sucursal),codigo, dato.pu_codPerfil,idusuario,dato.pu_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo+" Agrega: " + dato.pu_NombrePerfil + "-> ejecutando sp_ABC_PerfilUsuario: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), Convert.ToInt16(coP.cls_sucursal), codigo.Value, dato.pu_codPerfil, idusuario, dato.pu_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                    db.SaveChanges();

                resultado = db.sp_Busqueda_PerfilUsuario(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, idusuario, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_PerfilUsuario: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, idusuario, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_PerfilUsuario_Result re in resultado)
                {
                    perf.Add(new SelectListItem { Text = re.NombrePerfil, Value = re.CodigoPerfil.ToString() });
                }
            }
            catch (Exception ex)
            {

            }
            return Json(new SelectList(perf, "Value", "Text"));
        }
        public JsonResult BajaPerfil(int id)
        {
            perfilusuario dato = new perfilusuario();
            List<SelectListItem> perf = new List<SelectListItem>();
            try
            {
                int idusuario = Convert.ToInt32(Session["idusuario"].ToString());
                ObjectResult resultado = db.sp_Busqueda_Perfil(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Perfil: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_Perfil_Result re in resultado)
                {
                    dato.pu_codPerfil = re.CodigoPerfil;
                    dato.pu_NombrePerfil = re.NombrePerfil;
                }

                resultado = db.sp_Busqueda_PerfilUsuario(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), dato.pu_codPerfil, idusuario, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_PerfilUsuario: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), dato.pu_codPerfil, idusuario, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_PerfilUsuario_Result re in resultado)
                {
                    dato.pu_codPerfil = re.CodigoPerfil;
                    dato.pu_CodPerfilUsuario = Convert.ToInt16(re.CodigoPerfilUsuario);
                    tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                }

                codigo.Value = dato.pu_CodPerfilUsuario;
                dato.pu_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_PerfilUsuario(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), Convert.ToInt16(coP.cls_sucursal), codigo, dato.pu_codPerfil, idusuario, dato.pu_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo+ " Debaja: " + dato.pu_NombrePerfil + "-> ejecutando sp_ABC_PerfilUsuario: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), Convert.ToInt16(coP.cls_sucursal), codigo.Value, dato.pu_codPerfil, idusuario, dato.pu_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                    db.SaveChanges();
                else
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));

                resultado = db.sp_Busqueda_PerfilUsuario(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, idusuario, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_PerfilUsuario: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, idusuario, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_PerfilUsuario_Result re in resultado)
                {
                    perf.Add(new SelectListItem { Text = re.NombrePerfil, Value = re.CodigoPerfil.ToString() });
                }
            }
            catch (Exception ex)
            {

            }
            return Json(new SelectList(perf, "Value", "Text"));
        }

        public ActionResult RTodos()
        {
            String mierror = "";
            int idusuario = 0;
            perfilusuario dato = new perfilusuario();
            try
            {
                string sUsuario = Session["idusuario"] == null ? "0" : Session["idusuario"].ToString();
                idusuario = Convert.ToInt32(sUsuario);
                ObjectResult resultado = db.sp_Busqueda_Perfil(2, "", Convert.ToInt16(coP.cls_empresa), null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo+"-> ejecutando sp_Busqueda_Perfil: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_Perfil_Result re in resultado)
                {
                    dato.pu_codUsuario = idusuario;
                    dato.pu_codPerfil = re.CodigoPerfil;
                    dato.pu_usuarioing =  Session["UserName"].ToString();
                    if (!db.perfilusuario.Where(a => a.pu_codUsuario == idusuario && a.pu_codPerfil == re.CodigoPerfil && a.pu_estado == 1).Any())
                    {
                        myDat = "Crea perfil de usuario = " + dato.pu_codUsuario.ToString() + " - perfil:" + dato.pu_codPerfil.ToString() + " / sp_ABC_PerfilUsuario";
                        mierror = "";
                        GrabaPerfilU(dato, ref mierror);
                        if (mierror != "")
                            throw new System.InvalidOperationException(mierror, new Exception(""));
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Agregando perfiles a usuario", myDat });
            }

            return RedirectToAction("Perfil", new { id = idusuario });
        }


        public JsonResult GetRSelec(string datos)
        {
            String mierror = "";
            List<SelectListItem> perf = new List<SelectListItem>();
            perfilusuario dato = new perfilusuario();
            string[] opci = datos.Split(',');
            try
            {
                string sUsuario = Session["idusuario"] == null ? "0" : Session["idusuario"].ToString();
                int idusuario = Convert.ToInt32(sUsuario);
                foreach (string dat in opci)
                {
                    Int16 coPer = Convert.ToInt16(dat);
                    if (!db.perfilusuario.Where(a => a.pu_codUsuario == idusuario && a.pu_codPerfil == coPer && a.pu_estado == 1).Any())
                    {
                        myDat = "Crea perfil de usuario = " + dato.pu_codUsuario.ToString() + " - perfil:" + dato.pu_codPerfil.ToString() + " / sp_ABC_PerfilUsuario";
                        dato.pu_codUsuario = idusuario;
                        dato.pu_codPerfil = Convert.ToInt16(dat);
                        dato.pu_usuarioing = Session["UserName"].ToString();
                        mierror = "";
                        GrabaPerfilU(dato, ref mierror);
                        if (mierror != "")
                            throw new System.InvalidOperationException(mierror, new Exception(""));
                    }
                }
                ObjectResult resultado = db.sp_Busqueda_PerfilUsuario(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, idusuario, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_PerfilUsuario: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, idusuario, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_PerfilUsuario_Result re in resultado)
                {
                    perf.Add(new SelectListItem { Text = re.NombrePerfil, Value = re.CodigoPerfil.ToString() });
                }
            }
            catch (Exception ex)
            {
                
            }
            return Json(new SelectList(perf, "Value", "Text"));
        }

        public JsonResult GetLSelec(string datos)
        {
            List<SelectListItem> perf = new List<SelectListItem>();
            perfilusuario dato = new perfilusuario();
            string[] opci = datos.Split(',');
            try
            {
                string sUsuario = Session["idusuario"] == null ? "0" : Session["idusuario"].ToString();
                int idusuario = Convert.ToInt32(sUsuario);
                foreach (string dat in opci)
                {
                    Int16 coPer = Convert.ToInt16(dat);
                    ObjectResult resultado = db.sp_Busqueda_PerfilUsuario(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), coPer, idusuario, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_PerfilUsuario: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), dato.pu_codPerfil, idusuario, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_PerfilUsuario_Result re in resultado)
                    {
                        dato.pu_codPerfil = re.CodigoPerfil;
                        dato.pu_NombrePerfil = re.NombrePerfil;
                        dato.pu_CodPerfilUsuario = Convert.ToInt16(re.CodigoPerfilUsuario);
                        tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                    }

                    codigo.Value = dato.pu_CodPerfilUsuario;
                    dato.pu_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_PerfilUsuario(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), Convert.ToInt16(coP.cls_sucursal), codigo, dato.pu_codPerfil, idusuario, dato.pu_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + " De baja : " + dato.pu_NombrePerfil + " -> ejecutando sp_ABC_PerfilUsuario: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), Convert.ToInt16(coP.cls_sucursal), codigo.Value, dato.pu_codPerfil, idusuario, dato.pu_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                    if (error.Value.ToString() == "")
                        db.SaveChanges();
                    else
                        throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
                }
                ObjectResult resulta1 = db.sp_Busqueda_PerfilUsuario(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, idusuario, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_PerfilUsuario: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, idusuario, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_PerfilUsuario_Result re in resulta1)
                {
                    perf.Add(new SelectListItem { Text = re.NombrePerfil, Value = re.CodigoPerfil.ToString() });
                }
            }
            catch (Exception ex)
            {

            }
            return Json(new SelectList(perf, "Value", "Text"));
        }


        public ActionResult LTodos()
        {
            String mierror = "";
            int idusuario = 0;
            perfilusuario dato = new perfilusuario();
            try
            {
                idusuario = Convert.ToInt32(Session["idusuario"].ToString());
                ObjectResult resultado = db.sp_Busqueda_PerfilUsuario(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, idusuario, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo+"-> ejecutando sp_Busqueda_Perfil: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, idusuario, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_PerfilUsuario_Result re in resultado)
                {
                    myDat = "Baja de perfil de usuario = " + dato.pu_codUsuario.ToString() + " - perfil:" + dato.pu_codPerfil.ToString() + " / sp_ABC_PerfilUsuario";
                    dato.pu_codUsuario = idusuario;
                    dato.pu_codPerfil = re.CodigoPerfil;
                    dato.pu_NombrePerfil = re.NombrePerfil;
                    dato.pu_CodPerfilUsuario = Convert.ToInt16(re.CodigoPerfilUsuario);
                    dato.pu_usuarioing = coP.cls_usuario;
                    dato.pu_timestamp = re.TimeStamp;
                    mierror = "";
                    BajaPerfilU(dato, ref mierror);
                    if (mierror != "")
                        throw new System.InvalidOperationException(mierror, new Exception(""));
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Da de baja de perfiles a usuario", myDat });
            }

            return RedirectToAction("Perfil", new { id = idusuario });
        }

        public Boolean GrabaPerfilU(perfilusuario dato, ref string err)
        {
            try
            {
                SSS_PERSONASEntities dbh = new SSS_PERSONASEntities();
                dato.pu_usuarioing = Session["UserName"].ToString();
                int result = dbh.sp_ABC_PerfilUsuario(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), Convert.ToInt16(coP.cls_sucursal), codigo, dato.pu_codPerfil, dato.pu_codUsuario, dato.pu_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo+ " Agrega: " + dato.pu_NombrePerfil + "-> ejecutando sp_ABC_PerfilUsuario: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), Convert.ToInt16(coP.cls_sucursal), codigo.Value, dato.pu_codPerfil, dato.pu_codUsuario, dato.pu_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    dbh.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                err = ex.ToString();
                return false;
            }
            return true;
        }


        public Boolean BajaPerfilU(perfilusuario dato, ref string err)
        {
            try
            {
                SSS_PERSONASEntities dbh = new SSS_PERSONASEntities();

                tsp = Convert.ToBase64String(dato.pu_timestamp as byte[]);
                codigo.Value = dato.pu_CodPerfilUsuario;
                dato.pu_usuarioing = Session["UserName"].ToString();
                int result = dbh.sp_ABC_PerfilUsuario(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), Convert.ToInt16(coP.cls_sucursal), codigo, dato.pu_codPerfil, dato.pu_codUsuario, dato.pu_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo+" De baja : " + dato.pu_NombrePerfil + " -> ejecutando sp_ABC_PerfilUsuario: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), Convert.ToInt16(coP.cls_sucursal), codigo.Value, dato.pu_codPerfil, dato.pu_codUsuario, dato.pu_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    dbh.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                err = ex.ToString();
                return false;
            }
            return true;
        }


        public ActionResult Perfil(short? id, string nom)
        {
            ViewData["nombre"] = nom;
            try
            {
                Session["idusuario"] = id;
                if (Session[myModulo].ToString().Substring(4, 1) == "1")
                {
                    myDat = "Horarios de empleado: " + id.ToString() + " - " + nom + " / sp_Busqueda_Perfil - sp_Busqueda_PerfilUsuario";
                    error.Value = "";
                    ViewBag.PE = new SelectList(db.sp_Busqueda_PerfilUsuario(3, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, id, error).ToList(), "CodigoPerfil", "NombrePerfil");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Perfil: " + string.Join(",", 3, "", Convert.ToInt16(coP.cls_empresa), null, null, null, "-> R: " + validad.getResponse(error)));
                    ViewBag.PU = new SelectList(db.sp_Busqueda_PerfilUsuario(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, id, error).ToList(), "CodigoPerfil", "NombrePerfil");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_PerfilUsuario: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, id, "-> R: " + validad.getResponse(error)));
                }
                else
                    throw new System.InvalidOperationException("-Usuario no tiene permitido asignar perfiles!-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Lista", myDat });
            }
            return View();
        }

        public ActionResult Regresa()
        {
            return RedirectToAction("Index");
        }

        public ActionResult Resetea(short? id, int? empl)
        {
            Usuario usuario = new Usuario();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                if (Session[myModulo].ToString().Substring(5, 1) == "1")
                {
                    myDat = "Resetea clave: " + id.ToString() + " - " + empl.ToString() + " / sp_Busqueda_Usuario";
                    ObjectResult resultado = db.sp_Busqueda_Usuario(2, "", Convert.ToInt16(coP.cls_empresa), null, id, null, null, null, empl, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Usuario: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, id, null, null, null, empl, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_Usuario_Result re in resultado)
                    {
                        usuario = CargaUsuario(re);
                        dt = Convert.ToBase64String(re.TimeStamp as byte[]);
                    }
                    if (usuario == null)
                    {
                        return View("Error");
                    }
                    ViewData["Nombre"] = usuario.us_nombre;
                }
                else
                    throw new System.InvalidOperationException("-Usuario no tiene permitido resetear claves!-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
            }

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Resetea([Bind(Include = "us_empresa,us_IdSucursal,us_codUsuario,us_usuario,us_nombre,us_password,us_estUsuario,us_IdEmpleado,us_resetPass,us_fechaing,us_fechamod,us_usuarioing,us_usuariomod,us_maquinaing,us_maquinamod,us_estado,us_timestamp")] Usuario usuario)
        {
            try
            {
                myDat = "Resetea clave / sp_ABC_Usuario";
                dt = Convert.ToBase64String(usuario.us_timestamp as byte[]);
                codusuario.Value = usuario.us_codUsuario;
                usuario.us_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_Usuario(Convert.ToInt16(coP.cls_empresa), Convert.ToString('D'), Convert.ToInt16(coP.cls_sucursal), codusuario, usuario.us_usuario,
                                               usuario.us_nombre, usuario.us_password, usuario.us_estUsuario, usuario.us_IdEmpleado, usuario.us_usuarioing, 1, dt, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Usuario: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('D'), Convert.ToInt16(coP.cls_sucursal), codusuario.Value.ToString(), usuario.us_usuario,
                                               usuario.us_nombre, "".PadRight(usuario.us_password.Length, '*'), usuario.us_estUsuario, usuario.us_IdEmpleado, usuario.us_usuarioing, 1, dt, "-> R: " + validad.getResponse(error)));
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


        // GET: Usuarios/Edit/5
        public ActionResult Edit(short? id,int? empl)
        {
            Usuario usuario = new Usuario();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                myDat = "Actualiza datos: " + id.ToString() + " - " + empl.ToString() + " / sp_Busqueda_Usuario";
                if (Session[myModulo].ToString().Substring(2, 1) == "1")
                {
                    ViewBag.EmpleId = new SelectList(dbE.sp_Busqueda_Empleado(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, error).ToList(), "Empleado", "NombreLargo");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Empleado: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    ObjectResult resultado = db.sp_Busqueda_Usuario(2, "", Convert.ToInt16(coP.cls_empresa), null, id, null, null, null, empl, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Usuario: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, id, null, null, null, empl, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_Usuario_Result re in resultado)
                    {
                        usuario = CargaUsuario(re);
                        dt = Convert.ToBase64String(re.TimeStamp as byte[]);
                    }
                    if (usuario == null)
                    {
                        throw new System.InvalidOperationException("-Error al obtener registro-", new Exception(""));
                    }
                }
                else
                    throw new System.InvalidOperationException("-No tiene acceso a esta opción-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
            }

            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "us_empresa,us_IdSucursal,us_codUsuario,us_usuario,us_nombre,us_password,us_estUsuario,us_IdEmpleado,us_resetPass,us_fechaing,us_fechamod,us_usuarioing,us_usuariomod,us_maquinaing,us_maquinamod,us_estado,us_timestamp")] Usuario usuario)
        {
            try
            {
                myDat = "Actualizar datos / sp_ABC_Usuario";
                dt = Convert.ToBase64String(usuario.us_timestamp as byte[]);
                codusuario.Value = usuario.us_codUsuario;
                usuario.us_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_Usuario(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), Convert.ToInt16(coP.cls_sucursal), codusuario, usuario.us_usuario,
                                               usuario.us_nombre, usuario.us_password, usuario.us_estUsuario, usuario.us_IdEmpleado, usuario.us_usuarioing, usuario.us_resetPass, dt, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Usuario: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), Convert.ToInt16(coP.cls_sucursal), codusuario.Value.ToString(), usuario.us_usuario,
                                               usuario.us_nombre, "".PadRight((usuario.us_password==null?"":usuario.us_password).Length, '*'), usuario.us_estUsuario, usuario.us_IdEmpleado, usuario.us_usuarioing, usuario.us_resetPass, dt, "-> R: " + validad.getResponse(error)));
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

        // GET: Usuarios/Delete/5
        public ActionResult Delete(short? id)
        {
            Usuario usuario = new Usuario();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                myDat = "Dar de baja usuario: " + id.ToString() + " / sp_Busqueda_Usuario";
                if (Session[myModulo].ToString().Substring(3, 1) == "1")
                {
                    ObjectResult resultado = db.sp_Busqueda_Usuario(2, "", Convert.ToInt32(coP.cls_empresa), null, id, null, null, null, null, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Usuario: " + string.Join(",", 2, "", Convert.ToInt32(coP.cls_empresa), null, id, null, null, null, null, "-> R: " + validad.getResponse(error)));

                    foreach (sp_Busqueda_Usuario_Result re in resultado)
                    {
                        usuario = CargaUsuario(re);
                    }
                }
                else
                    throw new System.InvalidOperationException("-No tiene acceso a esta opción-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "DeBaja", myDat });
            }
            return View(usuario);
        }

        private Usuario CargaUsuario(sp_Busqueda_Usuario_Result parResult)
        {
            Usuario usu = new Usuario();

            usu.us_empresa = parResult.Empresa;
            usu.us_codUsuario = parResult.CodigoUsuario;
            usu.us_usuario = parResult.Usuario;
            usu.us_nombre = parResult.NombreUsuario;
            usu.us_estUsuario = parResult.EstadoUsuario;
            usu.us_IdEmpleado = parResult.CodigoEmpleado;
            usu.us_timestamp = parResult.TimeStamp; 
            return usu;
        }


        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            Usuario usuario = new Usuario();
            try
            {
                myDat = "Confirma dar de baja usuario: " + id.ToString() + " / sp_Busqueda_Usuario";
                ObjectResult resultado = db.sp_Busqueda_Usuario(2, "", 1, null, id, null, null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Usuario: " + string.Join(",", 2, "", 1, null, id, null, null, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_Usuario_Result re in resultado)
                {
                    usuario = CargaUsuario(re);
                    dt = Convert.ToBase64String(re.TimeStamp as byte[]);
                }

                codusuario.Value = usuario.us_codUsuario;
                usuario.us_usuarioing = Session["UserName"].ToString();
                usuario.us_password = (usuario.us_password == null ? "": usuario.us_password);
                int result = db.sp_ABC_Usuario(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), Convert.ToInt16(coP.cls_sucursal), codusuario, usuario.us_usuario,
                      usuario.us_nombre, usuario.us_password, usuario.us_estUsuario, usuario.us_IdEmpleado, usuario.us_usuarioing, usuario.us_resetPass, dt, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Usuario: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), Convert.ToInt16(coP.cls_sucursal), codusuario.Value.ToString(), usuario.us_usuario,
                      usuario.us_nombre, "".PadRight(usuario.us_password.Length, '*'), usuario.us_estUsuario, usuario.us_IdEmpleado, usuario.us_usuarioing, usuario.us_resetPass, dt, "-> R: " + validad.getResponse(error)));
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
                    string path = Path.Combine(Server.MapPath("~/Reports"), "Usuarios.rdlc");
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        return View("Index");
                    }
                    List<Usuario> cm = new List<Usuario>();
                    using (SSS_PERSONASEntities dc = new SSS_PERSONASEntities())
                    {
                        cm = dc.Usuario.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DsUsuarios", cm);
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


                    myDat = "Reporte de usuarios";
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
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Reportea", myDat });
            }
        }

        [HttpPost]
        public JsonResult createView()
        {
            return Json("");

        }

        //funcion para renderizar y retornar la vista crear usuario
        private string ConvertView(string viewName)
        {
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);

                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }

        [HttpPost]
        public JsonResult saveUsuario()
        {
            return Json("");
        }

        [HttpPost]
        public JsonResult updateView(int id)
        {
            return Json("");
        }

        //FUNCION PARA RETORNAR LA VISTA DE ACTUALIZAR USUARIO RENDERIZADA
        private string ConvertViewUpdate(string viewName, object model)
        {
            ViewData.Model = model;

            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);

                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }

        [HttpPost]
        public JsonResult updateUsuario()
        {
            return Json("");
        }


        [HttpPost]
        public JsonResult deleteView(int id)
        {
            return Json("");
        }

        //FUNCION PARA RENDERIZAR LA VISTA PARA DAR DE BAJA EL USUARIO
        private string ConvertViewDelete(string viewName, object model)
        {
            ViewData.Model = model;

            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);

                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }

        [HttpPost]
        public JsonResult deleteUsuario()
        {
            return Json("");
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
