using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebGrupo3S.Models;
using System.Data.Entity.Core.Objects;
using WebGrupo3S.Helpers;

namespace WebGrupo3S.Controllers
{
    public class CuentasXPagarsController : Controller
    {
        private SSS_OPERACIONEntities db = new SSS_OPERACIONEntities();
        private SSS_PERSONASEntities dbP = new SSS_PERSONASEntities();
        private string myModulo = "Cuentas por pagar";
        private string myDat = "";
        public ConstantesP coP = new ConstantesP();
        public String tsp = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("cp_idCuentaxPagar", typeof(int));

        CompleteString complete = new CompleteString();

        public class DatosSearchResultModel
        {
            public IEnumerable<sp_Busqueda_CuentaXPagar_Result> DatosResults { get; set; }
        }

        // GET: CuentaXPagars
        public ActionResult Index()
        {
            var DatosSearch = new DatosSearchResultModel();
            try
            {
                myDat = "Busqueda / sp_Busqueda_CuentaXPagar";
                DatosSearch.DatosResults = db.sp_Busqueda_CuentaXPagar(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, error).ToList();
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_CuentaXPagar: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Lista"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Lista", myDat });
            }
            return View(DatosSearch.DatosResults.ToList());
        }


        // GET: cuentaXPagars/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaXPagar cuentaXPagar = db.CuentaXPagars.Find(id);
            if (cuentaXPagar == null)
            {
                return HttpNotFound();
            }
            return View(cuentaXPagar);
        }

        // GET: cuentaXPagars/Create
        public ActionResult Create()
        {
            ViewBag.CP = new SelectList(dbP.sp_Busqueda_Proveedor(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, error).ToList(), "pv_proveedor", "pv_nombrelargo");
            return View();
        }

        // POST: cuentaXPagars/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cc_empresa,cc_IdCuentaXCobrar,cc_Cliente,cc_Saldo,cc_fechaUltMov,cc_MontoUltMov,cc_estCuentaXCobrar,cc_fechaing,cc_fechamod,cc_usuarioing,cc_usuariomod,cc_maquinaing,cc_maquinamod,cc_estado,cc_timestamp")] CuentaXPagar cuentaxpagar)
        {
            try
            {
                myDat = "Crear nuevo registro cuentas x cobrar / sp_ABC_CuentaXCobrar";
                if (ModelState.IsValid)
                {
                    cuentaxpagar.cp_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_CuentaXPagar(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo, cuentaxpagar.cp_Proveedor, cuentaxpagar.cp_Saldo, null, null, null, cuentaxpagar.cp_usuarioing, tsp, error);

                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_CuentaXCobrar: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo.Value, cuentaxpagar.cp_Proveedor, cuentaxpagar.cp_Saldo, null, null, null, cuentaxpagar.cp_usuarioing, tsp, "-> R: " + validad.getResponse(error)));

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
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Crear nuevo registro de cuentas x cobrar"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea", myDat });
            }
            return View(cuentaxpagar);
        }

        // GET: CuentaXPagars/Edit/5
        public ActionResult Edit(short? id)
        {
            CuentaXPagar dato = new CuentaXPagar();
            if (id == null)
            {
                return View("Error");
            }

            try
            {
                myDat = "Actualizar cuenta por cobrar: " + id.ToString() + " / sp_Busqueda_Cliente";
                ViewBag.CC = new SelectList(dbP.sp_Busqueda_Proveedor(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, error).ToList(), "cl_cliente", "cl_nombrelargo");
                dato = db.CuentaXPagars.Where(a => a.cp_IdCuentaXPagar == id).First();
                if (dato == null)
                {
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Actualizar cuenta por cobrar"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Actualizar", myDat });
            }
            return View(dato);
        }

        // POST: CuentaXPagars/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cc_empresa,cc_IdCuentaXCobrar,cc_Cliente,cc_Saldo,cc_fechaUltMov,cc_MontoUltMov,cc_estCuentaXCobrar,cc_fechaing,cc_fechamod,cc_usuarioing,cc_usuariomod,cc_maquinaing,cc_maquinamod,cc_estado,cc_timestamp")] CuentaXPagar cuentaxpagar)
        {
            try
            {
                myDat = "Actualizar datos / sp_ABC_CuentaXPagar";
                tsp = Convert.ToBase64String(cuentaxpagar.cp_timestamp as byte[]);
                codigo.Value = cuentaxpagar.cp_IdCuentaXPagar;
                int result = db.sp_ABC_CuentaXPagar(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo, cuentaxpagar.cp_Proveedor, cuentaxpagar.cp_Saldo, null, null, null, cuentaxpagar.cp_usuarioing, tsp, error);

                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_CuentaXCobrar: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo.Value, cuentaxpagar.cp_Proveedor, cuentaxpagar.cp_Saldo, null, null, null, cuentaxpagar.cp_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Actualiza", myDat });
            }
            return RedirectToAction("Index");
        }

        // GET: CuentaXPagars/Delete/5
        public ActionResult Delete(short? id)
        {
            CuentaXPagar dato = new CuentaXPagar();
            if (id == null)
            {
                return View("Error");
            }

            try
            {
                myDat = "Dar de baja a cuenta por cobrar: " + id.ToString();
                dato = db.CuentaXPagars.Where(a => a.cp_IdCuentaXPagar == id).First();
                if (dato == null)
                {
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Da de baja a cuenta por cobrar"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Debaja", myDat });
            }
            return View(dato);
        }

        // POST: CuentaXPagars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            CuentaXPagar dato = new CuentaXPagar();
            try
            {
                myDat = "Confirma dar de baja cuenta por cobrar: " + id.ToString() + " / sp_ABC_CuentaXCobrar";
                dato = db.CuentaXPagars.Where(a => a.cp_IdCuentaXPagar == id).First();
                tsp = Convert.ToBase64String(dato.cp_timestamp as byte[]);
                dato.cp_usuarioing = Session["UserName"].ToString();
                codigo.Value = id;
                int result = db.sp_ABC_CuentaXPagar(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo, null, null, null, null, null, dato.cp_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_CuentaXCobrar: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo.Value, null, null, null, null, null, dato.cp_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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