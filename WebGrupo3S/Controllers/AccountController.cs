using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Core.Objects;
using WebGrupo3S.Models;
using WebGrupo3S.ValidationHelper;
using WebGrupo3S.Helpers;
using System.Collections.Generic;

namespace WebGrupo3S.Controllers
{

    public class AccountController : Controller
    {       
        private SSS_PERSONASEntities db = new SSS_PERSONASEntities();
        public ConstantesP coP = new ConstantesP();
        String myModulo="Logon";
        ObjectParameter dt = new ObjectParameter("error", typeof(String));
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codusuario = new ObjectParameter("us_codUsuario", typeof(int));
        ObjectParameter lg_nombreUsuario = new ObjectParameter("lg_nombreusuario", typeof(String));
        public List<string> groups = new List<string>();

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        public class PerfilSearchResultModel
        {
            public IEnumerable<sp_Busqueda_PerfilUsuario_Result> perfilResults { get; set; }
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "usuario,password")] loginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                validad va = new validad();
                loginViewModel datos = new loginViewModel();
                var PerfilSearch = new PerfilSearchResultModel();
                error.Value = "";
                ObjectResult resultado = db.sp_Login(Convert.ToInt16(coP.cls_empresa), login.usuario, login.password, error);
                foreach (sp_Login_Result re in resultado)
                {
                    Cargalogin(re, datos);
                }
                WriteLogMessages.WriteFile("", myModulo + "-> ejecutado sp_Login: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), login.usuario, "".PadRight(login.password.Length, '*'), "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    if (datos.codigo != 0)
                    {
                        Session["UserId"] = datos.codigo;
                        Session["UserName"] = datos.nombre;
                        Session["LogonName"] = login.usuario;
                        ViewData["nombre"] = datos.nombre;
                        ViewData["nombreusu"] = datos.nombre;
                        if (datos.resetpass == 1)
                        {
                            ViewData["nombre"] = datos.nombre;
                            return View("~/Views/Account/ResetPassword.cshtml");
                        }
                        else
                        {
                            error.Value = "";
                            PerfilSearch.perfilResults = db.sp_Busqueda_PerfilUsuario(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null,  datos.codigo, error);
                            WriteLogMessages.WriteFile("", myModulo + "-> ejecutando sp_Busqueda_PerfilUsuario: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, datos.codigo, "-> R: " + validad.getResponse(error)));
                            string per = "";
                            foreach (sp_Busqueda_PerfilUsuario_Result re in PerfilSearch.perfilResults)
                            {
                                if (per == "")
                                    per = re.CodigoPerfil.ToString();
                                else
                                    per = per + "," + re.CodigoPerfil.ToString();
                            }
                            if (per == "")
                            {
                                string message = "Usuario no tiene perfiles de seguridad configurados.";
                                ModelState.AddModelError("", message);
                                WriteLogMessages.WriteFile("", myModulo + "-> resultado: " + message);
                                return View();
                            }
                            else Session["perfil"] = per;

                            for (int a = 0; a < 27; a++)
                            {
                                Session["M" + a.ToString()] = "0";
                            }

                            va.seteMenu(per);
                            int i = 1;
                            foreach(char valor in va.acc)
                            {
                                Session["M" + i.ToString()] = valor.ToString();
                                i++;
                            }
                            return View("~/Views/Home/Index.cshtml");
                        }
                    }
                    else
                    {
                        string message = "Usuario invalido o contraseña incorrecta.";
                        ModelState.AddModelError("", message);
                        WriteLogMessages.WriteFile("", myModulo + "-> mensaje: " + message);
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", error.Value.ToString());
                    WriteLogMessages.WriteFile("", myModulo + "-> mensaje: " + error.Value.ToString());
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al procesar su acceso al sistema, notifique al administrador.");
                string message = ex.ToString();
                WriteLogMessages.WriteFile("", myModulo + "-> excepción: " + message);
                return View();
            }
        }

        private loginViewModel Cargalogin(sp_Login_Result parResult, loginViewModel pf)
        {
            pf.codigo = parResult.codigo;
            pf.nombre = parResult.nombre;
            pf.resetpass = Convert.ToInt16(parResult.resetpass);
            return pf;
        }


        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword([Bind(Include = "password, ConfirmPassword")] RestableceViewModel cambio)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                if (cambio.password == cambio.ConfirmPassword)
                {
                    Usuario usuario = new Usuario();
                    ObjectResult resultado = db.sp_Busqueda_Usuario(2, "", 1, null, Convert.ToInt32(Session["UserId"]), null, null, null, null, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Usuario: " + string.Join(",", 2, "", 1, null, Convert.ToInt32(Session["UserId"]), null, null, null, null, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_Usuario_Result re in resultado)
                    {
                        usuario = CargaUsuario(re);
                        dt.Value = Convert.ToBase64String(re.TimeStamp as byte[]);
                    }

                    codusuario.Value = usuario.us_codUsuario;
                    int result = db.sp_ABC_Usuario(Convert.ToInt16(coP.cls_empresa), Convert.ToString('D'), Convert.ToInt16(coP.cls_sucursal), codusuario, usuario.us_usuario,
                                                   usuario.us_nombre, cambio.password, usuario.us_estUsuario, usuario.us_IdEmpleado, usuario.us_usuarioing,
                                                   0, dt.Value.ToString(), error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo+"-> ejecutando sp_ABC_Usuario: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('D'), Convert.ToInt16(coP.cls_sucursal), codusuario.Value.ToString(), usuario.us_usuario,
                                                   usuario.us_nombre, "".PadRight(cambio.password.Length, '*'), usuario.us_estUsuario, usuario.us_IdEmpleado, usuario.us_usuarioing,
                                                   0, dt.Value.ToString(), "-> R: " + validad.getResponse(error)));
                    if (error.Value.ToString() == "")
                    {
                        db.SaveChanges();
                        return View("~/Views/Account/Login.cshtml");
                    }
                    else
                    {
                        ModelState.AddModelError("", error.Value.ToString());
                        return View();
                    }
                }
                else
                {
                    string message = "Contraseñas no coinciden.";
                    ModelState.AddModelError("", message);
                    WriteLogMessages.WriteFile("", myModulo + "-> mensaje: " + message);
                    return View();
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error en ingreso de contraseña.");
                string message = ex.Message;
                WriteLogMessages.WriteFile("", myModulo + "-> excepción, Error en ingreso de contraseña. " + message);
                return View();
            }
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

        public ActionResult miError(string message, string error, string inner, string modulo, string opcion, string datos)
        {
            Logs miLogs = new Logs("logs","c:\\");

            myError er = new myError();
            if (error == "Ingreso")
            {
                er.messageusuario = "No tiene acceso para ingresar a : " + modulo;
            }
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");
            else
            {
                er.message = error;
                er.modulo = modulo;
                er.excepcion = inner;
                er.opcion = opcion;
                if (Session["UserId"] == null)
                {
                    er.usuario = "Unknown";
                    er.nombre = "Unknown";
                }
                else
                {
                    er.usuario = Session["UserId"].ToString();
                    er.nombre = Session["UserName"].ToString();
                }
                miLogs.Add("Usuario: " + er.usuario + "/" + er.nombre + "  -  Control: " + er.modulo + "/" + er.opcion + "  -  Error= " + er.message + "/" + er.excepcion + "  -  Proceso o datos: " + datos);
                if (error.Substring(34, 1) != "-")
                {
                    //er.messageusuario = "Error en proceso de generación pagina, por favor notificar al administrador del sistema";
                    er.messageusuario = "Error: " + message;
                }
                else
                {
                    int p = error.IndexOf("-", 35, (error.Length - 35));
                    er.messageusuario = error.Substring(35, (p - 35));
                }
            }
            return View("Error", er);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {

        //    }

        //    // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
        //    return View(model);
        //}

 
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session["UserId"] = "";
            Session["UserName"] = "";
            return RedirectToAction("Login", "Account");
        }
    }
}