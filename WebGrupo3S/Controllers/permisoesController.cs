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
    public class permisoesController : Controller
    {
        private SSS_PERSONASEntities db = new SSS_PERSONASEntities();
        private string myModulo = "Perfiles";
        private string myDat = "";
        public ConstantesP coP = new ConstantesP();
        public PermisosP perP = new PermisosP();
        public String tsp = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("pe_IdPermiso", typeof(int));

        public class PermisosSearchResultModel
        {
            public IEnumerable<sp_Busqueda_Permiso_Result> permisosResults { get; set; }
        }

        // GET: permisoes
        public ActionResult Index()
        {
            var PermisosSearch = new PermisosSearchResultModel();
            try
            {
                myDat = "Lista de permisos";
              //  PermisosSearch.permisosResults = db.sp_Busqueda_Permiso(1, "", Convert.ToInt16(coP.cls_empresa), 1, null, null, error).ToList();
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, "Permisos", "Lista"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Lista", myDat });
            }

            return View(PermisosSearch.permisosResults.ToList());
        }

        // GET: permisoes/Create
        public ActionResult Create()
        {
            ViewBag.PerfilId = new SelectList(db.sp_Busqueda_Perfil(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, error).ToList(), "CodigoPerfil", "NombrePerfil");
            WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Perfil: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, "-> R: " + validad.getResponse(error)));
            ViewBag.OpcionId = new SelectList(db.sp_Busqueda_Opcion(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, error).ToList(), "CodigoOpcion", "NombreOpcion");
            WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Opcion: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, "-> R: " + validad.getResponse(error)));
            return View();
        }

        // POST: permisoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "pe_empresa,pe_IdSucursal,pe_IdPermiso,pe_CodPerfil,pe_CodigoOpcion,pe_FechaIng,pe_FechaMod,pe_UsuarioIng,pe_UsuarioMod,pe_MaquinaIng,pe_MaquinaMod,pe_estado,pe_timestamp")] permiso permiso)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    myDat = "Crea permisos / sp_ABC_Permiso";
                    permiso.pe_FechaIng = System.DateTime.Now;
                    permiso.pe_UsuarioIng = Session["UserName"].ToString();
                    int result = db.sp_ABC_Permiso(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'),Convert.ToInt16(coP.cls_sucursal),codigo,permiso.pe_CodPerfil,permiso.pe_CodigoOpcion, permiso.pe_UsuarioIng, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Permiso: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), Convert.ToInt16(coP.cls_sucursal), codigo.Value, permiso.pe_CodPerfil, permiso.pe_CodigoOpcion, permiso.pe_UsuarioIng, tsp, "-> R: " + validad.getResponse(error)));
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
                catch (Exception ex)
                {
                    //                    return View("Error", new HandleErrorInfo(ex, "Permisos","Crear"));
                    return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea", myDat });
                }
            }
            return View(permiso);
        }

        // GET: permisoes/Edit/5
        public ActionResult Edit(short? id, short? opcion)
        {
            permiso Permiso = new permiso();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.PerfilId = new SelectList(db.sp_Busqueda_Perfil(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, error).ToList(), "CodigoPerfil", "NombrePerfil");
            WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Perfil: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, "-> R: " + validad.getResponse(error)));
            ViewBag.OpcionId = new SelectList(db.sp_Busqueda_Opcion(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, error).ToList(), "CodigoOpcion", "NombreOpcion");
            WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Opcion: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, "-> R: " + validad.getResponse(error)));
            try
            {
                myDat = "Actualiza permisos: " + id.ToString() + " - " + opcion.ToString();
            //    ObjectResult resultado = db.sp_Busqueda_Permiso(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), id, opcion, error);
                //foreach (sp_Busqueda_Permiso_Result re in resultado)
                //{
                //    Permiso = CargaPermiso(re);
                //}
                //if (Permiso == null)
                //{
                //    return View("Error");
                //}
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, "Permisos", "Actualización"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
            }
            return View(Permiso);
        }

        // POST: permisoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "pe_empresa,pe_IdSucursal,pe_IdPermiso,pe_CodPerfil,pe_CodigoOpcion,pe_FechaIng,pe_FechaMod,pe_UsuarioIng,pe_UsuarioMod,pe_MaquinaIng,pe_MaquinaMod,pe_estado,pe_timestamp")] permiso permiso)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    myDat = "Actualización permisos / sp_ABC_Permiso";
                    tsp = Convert.ToBase64String(permiso.pe_timestamp as byte[]);
                    codigo.Value = permiso.pe_IdPermiso;
                    int result = db.sp_ABC_Permiso(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), Convert.ToInt16(coP.cls_sucursal), codigo, permiso.pe_CodPerfil, permiso.pe_CodigoOpcion, permiso.pe_UsuarioIng, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Permiso: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), Convert.ToInt16(coP.cls_sucursal), codigo.Value, permiso.pe_CodPerfil, permiso.pe_CodigoOpcion, permiso.pe_UsuarioIng, tsp, "-> R: " + validad.getResponse(error)));
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
                    //                    return View("Error", new HandleErrorInfo(ex, "Permisos", "Actualización"));
                    return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
                }
            }
            return View(permiso);
        }

        // GET: permisoes/Delete/5
        public ActionResult Delete(short? id, short? opcion)
        {
            permiso Permiso = new permiso();
            if (id == null)
            {
                return View("Error");
            }

            try
            {
                myDat = "Da de baja permisos: " + id.ToString() + " - " + opcion.ToString();
                //ObjectResult resultado = db.sp_Busqueda_Permiso(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), id, opcion, error);

                //foreach (sp_Busqueda_Permiso_Result re in resultado)
                //{
                //    Permiso = CargaPermiso(re);
                //}

            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, "Permisos", "Dar de Baja"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "DeBaja", myDat });
            }
            return View(Permiso);
        }

        private permiso CargaPermiso(sp_Busqueda_Permiso_Result parResult)
        {
            permiso pf = new permiso();

            pf.pe_empresa = parResult.Empresa;
            pf.pe_CodPerfil = parResult.CodigoPerfil;
            //pf.pe_DescPerfil = parResult.Perfil;
            pf.pe_CodigoOpcion = parResult.CodigoOpcion;
            //pf.pe_DescOpcion = parResult.Opcion;
            pf.pe_timestamp = parResult.TimeStamp;
            return pf;
        } 

        // POST: permisoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id, short opcion)
        {
            permiso Permiso = new permiso();
            try
            {
                myDat = "Confirmación de baja de permisos: " + id.ToString() + " - " + opcion.ToString() + " / sp_ABC_Permiso";
                //ObjectResult resultado = db.sp_Busqueda_Permiso(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), id, opcion, error);
                //foreach (sp_Busqueda_Permiso_Result re in resultado)
                //{
                //    Permiso = CargaPermiso(re);
                //    tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                //}

                codigo.Value = Permiso.pe_IdPermiso;
                int result = db.sp_ABC_Permiso(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), Convert.ToInt16(coP.cls_sucursal), codigo, Permiso.pe_CodPerfil, Permiso.pe_CodigoOpcion, Convert.ToString(Session["UserId"]), tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Permiso: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), Convert.ToInt16(coP.cls_sucursal), codigo.Value, Permiso.pe_CodPerfil, Permiso.pe_CodigoOpcion, Convert.ToString(Session["UserId"]), tsp, "-> R: " + validad.getResponse(error)));
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
                //                return View("Error", new HandleErrorInfo(ex, "Permisos", "Comfirmación de Baja"));
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
