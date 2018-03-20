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

    public class perfilusuariosController : Controller
    {
        private SSS_PERSONASEntities db = new SSS_PERSONASEntities();
        private string myModulo = "PerfilesUsuario";
        private string myDat = "";
        public ConstantesP coP = new ConstantesP();
        public PermisosP perP = new PermisosP();
        public String tsp = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("pu_CodPerfilUsuario", typeof(int));

        public class PerfilSearchResultModel
        {
            public IEnumerable<sp_Busqueda_PerfilUsuario_Result> perfilResults { get; set; }
        }
        // GET: perfilusuarios
        public ActionResult Index()
        {

            var PerfilSearch = new PerfilSearchResultModel();
            try
            {
                myDat = "Lista de perfiles de usuario / sp_Busqueda_PerfilUsuario";
                PerfilSearch.perfilResults = db.sp_Busqueda_PerfilUsuario(1, "", Convert.ToInt16(coP.cls_empresa),1, null, null, error).ToList();
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_PerfilUsuario: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), 1, null, null, "-> R: " + validad.getResponse(error)));
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Lista"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Lista", myDat });
            }

            return View(PerfilSearch.perfilResults.ToList());
        }

        // GET: perfilusuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            perfilusuario perfilusuario = db.perfilusuario.Find(id);
            if (perfilusuario == null)
            {
                return HttpNotFound();
            }
            return View(perfilusuario);
        }

        // GET: perfilusuarios/Create
        public ActionResult Create()
        {
            ViewBag.PerfilId = new SelectList(db.sp_Busqueda_Perfil(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, error).ToList(), "CodigoPerfil", "NombrePerfil");
            WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Perfil: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, "-> R: " + validad.getResponse(error)));
            ViewBag.UserId = new SelectList(db.sp_Busqueda_Usuario(1, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, null, null, null, null, error).ToList(), "CodigoUsuario", "NombreUsuario");
            WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Usuario: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
            return View();
        }

        // POST: perfilusuarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "pu_empresa,pu_IdSucursal,pu_CodPerfilUsuario,pu_codPerfil,pu_codUsuario,pu_fechaing,pu_fechamod,pu_usuarioing,pu_usuariomod,pu_maquinaing,pu_maquinamod,pu_estado,pu_timestamp")] perfilusuario perfilusuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    myDat = "Asigna perfiles a usuarios / sp_ABC_PerfilUsuario";
                    perfilusuario.pu_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_PerfilUsuario(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), Convert.ToInt16(coP.cls_sucursal),codigo, perfilusuario.pu_codPerfil, perfilusuario.pu_codUsuario, perfilusuario.pu_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_PerfilUsuario: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), Convert.ToInt16(coP.cls_sucursal), codigo.Value, perfilusuario.pu_codPerfil, perfilusuario.pu_codUsuario, perfilusuario.pu_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                    return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea", myDat });
                }
            }

            return View(perfilusuario);
        }

        // GET: perfilusuarios/Edit/5
        public ActionResult Edit(int? id, int? usu)
        {
            perfilusuario Perfil = new perfilusuario();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                myDat = "Edita perfiles de usuario: " + id.ToString() + " - " + usu.ToString() + " / sp_Busqueda_PerfilUsuario";
                perP.cls_editar = true; //temporal

                if (perP.cls_editar == true)
                {
                    ViewBag.PerfilId = new SelectList(db.sp_Busqueda_Perfil(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, error).ToList(), "CodigoPerfil", "NombrePerfil");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Perfil: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, "-> R: " + validad.getResponse(error)));
                    ViewBag.UserId = new SelectList(db.sp_Busqueda_Usuario(1, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, null, null, null, null, error).ToList(), "CodigoUsuario", "NombreUsuario");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Usuario: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), null, null, null, null, null, "-> R: " + validad.getResponse(error)));

                    ObjectResult resultado = db.sp_Busqueda_PerfilUsuario(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), id, usu, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_PerfilUsuario: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), id, usu, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_PerfilUsuario_Result re in resultado)
                    {
                        Perfil = CargaPerfil(re);
                    }
                    if (Perfil == null)
                        throw new System.InvalidOperationException("Error al obtener registro", new Exception(""));
                }
                else
                    throw new System.InvalidOperationException("No tiene acceso a la opcion de actualización", new Exception(""));
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Edit"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
            }
            return View(Perfil);
        }

        private perfilusuario CargaPerfil(sp_Busqueda_PerfilUsuario_Result parResult)
        {
            perfilusuario pf = new perfilusuario();

            pf.pu_empresa = parResult.Empresa;
            pf.pu_CodPerfilUsuario = Convert.ToInt16(parResult.CodigoPerfilUsuario);
            pf.pu_codPerfil = parResult.CodigoPerfil;
            pf.pu_codUsuario = parResult.CodigoUsuario;
            pf.pu_NombrePerfil = parResult.NombrePerfil;
            pf.pu_NombreUsuario = parResult.NombreUsuario;
            return pf;
        }

        // POST: perfilusuarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "pu_empresa,pu_IdSucursal,pu_CodPerfilUsuario,pu_codPerfil,pu_codUsuario,pu_fechaing,pu_fechamod,pu_usuarioing,pu_usuariomod,pu_maquinaing,pu_maquinamod,pu_estado,pu_timestamp")] perfilusuario perfilusuario)
        {
            try
            {
                myDat = "Edita permisos del usuario / sp_ABC_PerfilUsuario";
                tsp = Convert.ToBase64String(perfilusuario.pu_timestamp as byte[]);
                perfilusuario.pu_usuarioing = Session["UserName"].ToString();
                codigo.Value = perfilusuario.pu_CodPerfilUsuario;
                int result = db.sp_ABC_PerfilUsuario(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), Convert.ToInt16(coP.cls_sucursal), codigo, perfilusuario.pu_codPerfil, perfilusuario.pu_codUsuario, perfilusuario.pu_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_PerfilUsuario: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), Convert.ToInt16(coP.cls_sucursal), codigo.Value, perfilusuario.pu_codPerfil, perfilusuario.pu_codUsuario, perfilusuario.pu_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                }
                else
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Edit"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
            }
            return View(perfilusuario);
        }

        // GET: perfilusuarios/Delete/5
        public ActionResult Delete(int? id, int? usu)
        {
            perfilusuario Perfil = new perfilusuario();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                myDat = "Dar de baja perfiles de usuarios: " + id.ToString() + " - " + usu.ToString() + " / sp_Busqueda_PerfilUsuario";
                perP.cls_debaja = true;

                if (perP.cls_debaja == true)
                {
                    ObjectResult resultado = db.sp_Busqueda_PerfilUsuario(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), id, usu, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_PerfilUsuario: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), id, usu, "-> R: " + validad.getResponse(error)));

                    foreach (sp_Busqueda_PerfilUsuario_Result re in resultado)
                    {
                        Perfil = CargaPerfil(re);
                    }

                }
                else
                    throw new System.InvalidOperationException("No tiene permitido dar de baja el registro", new Exception(""));
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Dar de baja"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Debaja", myDat });
            }
            return View(Perfil);
        }

        // POST: perfilusuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int usu)
        {
            perfilusuario Perfil = new perfilusuario();
            try
            {
                myDat = "Confirmacion dar de baja permiso de usuario: " + id.ToString() + " - " + usu.ToString() + " / sp_Busqueda_PerfilUsuario";
                ObjectResult resultado = db.sp_Busqueda_PerfilUsuario(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), id, usu, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_PerfilUsuario: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), id, usu, "-> R: " + validad.getResponse(error)));

                foreach (sp_Busqueda_PerfilUsuario_Result re in resultado)
                {
                    Perfil = CargaPerfil(re);
                    tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                }

                codigo.Value = Perfil.pu_CodPerfilUsuario;
                int result = db.sp_ABC_PerfilUsuario(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), Convert.ToInt16(coP.cls_sucursal), codigo, Perfil.pu_codPerfil, Perfil.pu_codUsuario, coP.cls_usuario, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_PerfilUsuario: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), Convert.ToInt16(coP.cls_sucursal), codigo.Value, Perfil.pu_codPerfil, Perfil.pu_codUsuario, coP.cls_usuario, tsp, "-> R: " + validad.getResponse(error)));
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
            return RedirectToAction("Index");
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
