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
using WebGrupo3S.Helpers;

namespace WebGrupo3S.Views
{
    public class SucursalsController : Controller
    {
        private SSS_OPERACIONEntities db = new SSS_OPERACIONEntities();
        private string myModulo = "Sucursales";
        private string mycatalogo = "Mantenimientos Sucursal";
        private string myDat = "";
        public ConstantesP coP = new ConstantesP();
        public String tsp = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("su_IdSucursal", typeof(int));

        public class DatosSearchResultModel
        {
            public IEnumerable<sp_Busqueda_Sucursal_Result> DatosResults { get; set; }
        }

        // GET: Sucursals
        public ActionResult Index()
        {
            validad ac = new validad();
            var DatosSearch = new DatosSearchResultModel();
            try
            {
                ac.seteAccesos(mycatalogo, Session["perfil"].ToString());
                Session[myModulo] = ac.acc;
                if (Session["UserId"].ToString() != "")
                {
                    if (Session[myModulo].ToString().Substring(0, 1) == "1")
                    {
                        myDat = "Busqueda de sucursales / sp_Busqueda_Sucursal";
                        ViewBag.nuevo = Session[myModulo].ToString().Substring(1, 1);
                        ViewBag.editar = Session[myModulo].ToString().Substring(2, 1);
                        ViewBag.debaja = Session[myModulo].ToString().Substring(3, 1);
                        ViewBag.horarios = Session[myModulo].ToString().Substring(6, 1);
                        ViewBag.reporte = Session[myModulo].ToString().Substring(19, 1);
                        DatosSearch.DatosResults = db.sp_Busqueda_Sucursal(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, error).ToList();
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Sucursal: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
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


        // GET: Sucursals/Create
        public ActionResult Create()
        {
            try
            {
                myDat = "Agrega nueva sucursal";
                if (Session[myModulo].ToString().Substring(1, 1) == "0")
                    throw new System.InvalidOperationException("-No tiene acceso a crear nuevo registros-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea", myDat });
            }
            return View();
        }

        // POST: Sucursals/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "su_empresa,su_IdSucursal,su_Nombre,su_Encargado,su_Direccion,su_Telefono,su_fechaing,su_fechamod,su_usuarioing,su_usuariomod,su_maquinaing,su_maquinamod,su_estado,su_timestamp")] Sucursal sucursal)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    myDat = "Crear nueva sucursal / sp_ABC_Sucursal";
                    sucursal.su_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_Sucursal(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo, sucursal.su_Nombre,
                                                   sucursal.su_Encargado, sucursal.su_Direccion, sucursal.su_Telefono, sucursal.su_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Sucursal: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo.Value, sucursal.su_Nombre,
                                                   sucursal.su_Encargado, sucursal.su_Direccion, sucursal.su_Telefono, sucursal.su_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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

            return View(sucursal);
        }

        // GET: Sucursals/Edit/5
        public ActionResult Edit(short? id)
        {
            Sucursal dato = new Sucursal();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                myDat = "Actualizar sucursal: " + id.ToString();
                if (Session[myModulo].ToString().Substring(2, 1) == "1")
                {
                    dato = db.Sucursal.Where(a => a.su_IdSucursal == id).First();
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
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
            }
            return View(dato);
        }

        // POST: Sucursals/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "su_empresa,su_IdSucursal,su_Nombre,su_Encargado,su_Direccion,su_Telefono,su_fechaing,su_fechamod,su_usuarioing,su_usuariomod,su_maquinaing,su_maquinamod,su_estado,su_timestamp")] Sucursal sucursal)
        {
            try
            {
                myDat = "Actualizar datos / sp_ABC_Sucursal";
                tsp = Convert.ToBase64String(sucursal.su_timestamp as byte[]);
                codigo.Value = sucursal.su_IdSucursal;
                int result = db.sp_ABC_Sucursal(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo, sucursal.su_Nombre,
                                                   sucursal.su_Encargado, sucursal.su_Direccion,sucursal.su_Telefono, sucursal.su_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Sucursal: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo.Value, sucursal.su_Nombre,
                                                   sucursal.su_Encargado, sucursal.su_Direccion, sucursal.su_Telefono, sucursal.su_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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

        // GET: Sucursals/Delete/5
        public ActionResult Delete(short? id)
        {
            Sucursal dato = new Sucursal();
            if (id == null)
            {
                return View("Error");
            }

            try
            {
                myDat = "Dar de baja la sucursal: " + id.ToString();
                if (Session[myModulo].ToString().Substring(3, 1) == "1")
                {
                    dato = db.Sucursal.Where(a => a.su_IdSucursal == id).First();
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
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Da de baja a la sucursal"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "DeBaja", myDat });
            }
            return View(dato);
        }

        // POST: Sucursals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            Sucursal dato = new Sucursal();
            try
            {
                myDat = "Confirmacion dar de baja sucursal: " + id.ToString() + " / sp_ABC_Sucursal";
                dato = db.Sucursal.Where(a => a.su_IdSucursal == id).First();
                tsp = Convert.ToBase64String(dato.su_timestamp as byte[]);
                dato.su_usuarioing = Session["UserName"].ToString();
                codigo.Value = id;
                int result = db.sp_ABC_Sucursal(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo, null, null, null, null, dato.su_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Sucursal: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo.Value, null, null, null, null, dato.su_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
