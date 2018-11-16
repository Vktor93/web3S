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
using System.IO;
using Newtonsoft.Json;

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
        ObjectParameter codigomv = new ObjectParameter("mp_idMovCuentaxPagar", typeof(int));
        ObjectParameter tsp2 = new ObjectParameter("mp_timestamp", typeof(String));
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
        public ActionResult Details(int? id, short? ids)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaXPagar cuentaXPagar = db.CuentaXPagars.Find(id, ids);
            if (cuentaXPagar == null)
            {
                return HttpNotFound();
            }
            return View(cuentaXPagar);
        }

        // GET: cuentaXPagars/Create
        public ActionResult Create()
        {
            ViewBag.CP = new SelectList(dbP.sp_Busqueda_Proveedor(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, error).ToList(), "Proveedor", "Nombre1");
            return View();
        }

        // POST: cuentaXPagars/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cp_empresa,cp_IdCuentaXPagar,cp_Proveedor,cp_Saldo,cp_fechaUltMov,cp_MontoUltMov,cp_estCuentaXPagar,cp_fechaing,cp_fechamod,cp_usuarioing,cp_usuariomod,cp_maquinaing,cp_maquinamod,cp_estado,cp_timestamp")] CuentaXPagar cuentaxpagar)
        {
            try
            {
                myDat = "Crear nuevo registro cuentas x pagar / sp_ABC_CuentaXPagar";
                if (ModelState.IsValid)
                {
                    cuentaxpagar.cp_usuarioing = Session["UserName"].ToString();
                    cuentaxpagar.cp_fechaUltMov = DateTime.Now;
                    int result = db.sp_ABC_CuentaXPagar(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo, cuentaxpagar.cp_Proveedor, cuentaxpagar.cp_Saldo,cuentaxpagar.cp_fechaUltMov, 0,"1", cuentaxpagar.cp_usuarioing, tsp, error);

                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_CuentaXPagar: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo.Value, cuentaxpagar.cp_Proveedor, cuentaxpagar.cp_Saldo, null, null, null, cuentaxpagar.cp_usuarioing, tsp, "-> R: " + validad.getResponse(error)));

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
                myDat = "Actualizar cuenta por pagar: " + id.ToString() + " / sp_Busqueda_Proveedor";
                ViewBag.CC = new SelectList(dbP.sp_Busqueda_Proveedor(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, error).ToList(), "pv_proveedor", "pv_nombre1");
                dato = db.CuentaXPagars.Where(a => a.cp_IdCuentaXPagar == id).First();
                if (dato == null)
                {
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Actualizar cuenta por pagar"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Actualizar", myDat });
            }
            return View(dato);
        }

        // POST: CuentaXPagars/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cp_empresa,cp_IdCuentaXPagar,cp_Proveedor,cp_Saldo,cp_fechaUltMov,cp_MontoUltMov,cp_estCuentaXPagar,cp_fechaing,cp_fechamod,cp_usuarioing,cp_usuariomod,cp_maquinaing,cp_maquinamod,cp_estado,cp_timestamp")] CuentaXPagar cuentaxpagar)
        {
            try
            {
                myDat = "Actualizar datos / sp_ABC_CuentaXPagar";
                tsp = Convert.ToBase64String(cuentaxpagar.cp_timestamp as byte[]);
                codigo.Value = cuentaxpagar.cp_IdCuentaXPagar;
                int result = db.sp_ABC_CuentaXPagar(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo, cuentaxpagar.cp_Proveedor, cuentaxpagar.cp_Saldo, null, null, null, cuentaxpagar.cp_usuarioing, tsp, error);

                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_CuentaXPagar: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo.Value, cuentaxpagar.cp_Proveedor, cuentaxpagar.cp_Saldo, null, null, null, cuentaxpagar.cp_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                myDat = "Dar de baja a cuenta por pagar: " + id.ToString();
                dato = db.CuentaXPagars.Where(a => a.cp_IdCuentaXPagar == id).First();
                if (dato == null)
                {
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Da de baja a cuenta por pagar"));
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
                myDat = "Confirma dar de baja cuenta por pagar: " + id.ToString() + " / sp_ABC_CuentaXPagar";
                dato = db.CuentaXPagars.Where(a => a.cp_IdCuentaXPagar == id).First();
                tsp = Convert.ToBase64String(dato.cp_timestamp as byte[]);
                dato.cp_usuarioing = Session["UserName"].ToString();
                codigo.Value = id;
                int result = db.sp_ABC_CuentaXPagar(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo, null, null, null, null, null, dato.cp_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_CuentaXPagar: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo.Value, null, null, null, null, null, dato.cp_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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

        //NUEVA SECCION PARA ALMAENAR CUNETA POR PAGAR
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
        public JsonResult bodyDoc()
        {
            try
            {
                var data = new
                {
                    status = 200,
                    message = "success",
                };

                int empre = Convert.ToInt16(coP.cls_empresa);

                ViewBag.SUC = new SelectList(db.sp_Busqueda_Sucursal(1, "", empre, null, null, null, null, null, error).ToList(), "CodigoSucursal", "NombreSucursal");
                //ViewBag.DOC = new SelectList(db.sp_Busqueda_Documento(1, "", empre, null, null, null, "PV", null, null, null, null, null, null, null, null, null, null, null, null, null, null, error).ToList(), "CodigoDocumento", "TipoDocumento");
                string viewContent = ConvertView("bodyDoc");
                return Json(new { PartialView = viewContent }, JsonConvert.SerializeObject(data));

            }
            catch (Exception ex)
            {
                var data = new
                {
                    status = 500,
                    message = "Excepción encontrada, ver consola",
                    excepcion = ex.ToString()
                };

                return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
            }

            //ViewBag.Ssd = new SelectList(dbP.sp_Busqueda_Cliente(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, null, null, null, null, null, null, error).ToList(), "Cliente", "NombreLargo");            
        }


        [HttpPost]
        public JsonResult getSaldoP(int? id)
        {
            try
            {
                CuentaXPagar dato = db.CuentaXPagars.Where(a => a.cp_Proveedor == id).First();
                return Json(dato, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var data = new
                {
                    status = 500,
                    message = "Excepción encontrada, ver consola",
                    excepcion = ex.ToString()
                };

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            //ObjectResult resultado =dbO.sp_Busqueda_Sucursal(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, error);                       
        }

        [HttpPost]
        public JsonResult saveCuenta([Bind(Include = "cp_empresa,cp_IdCuentaXPagar,cp_Proveedor,cp_Saldo,cp_fechaUltMov,cp_MontoUltMov,cp_estCuentaXPagar,cp_fechaing,cp_fechamod,cp_usuarioing,cp_usuariomod,cp_maquinaing,cp_maquinamod,cp_estado,cp_timestamp")] CuentaXPagar cuentaxpagar, [Bind(Include = "mp_empresa,mp_IdCuentaXPagar,mp_IdMovCuentaXPagar,mp_IdSucursal,mp_IdDoc,mp_Proveedor,mp_FechaMov,mp_MontoMov,mp_CreditoDebito,mp_Descripcion,mp_estMovCuentaXPagar,mp_fechaing,mp_fechamod,mp_usuarioing,mp_usuariomod,mp_maquinaing,mp_maquinamod,mp_estado,mp_timestamp")] MovimientoCuentaXPagar movimientoCuentaXPagar)
        {
            //NameValueCollection coll;
            //coll = Request.Form;

            try
            {

                myDat = "Crea Cuenta x Pagar / sp_ABC_CuentaXPagar";
                cuentaxpagar.cp_fechaing = System.DateTime.Now;
                cuentaxpagar.cp_usuarioing = Session["UserName"].ToString();
                cuentaxpagar.cp_fechaUltMov = System.DateTime.Now;
                var result = db.sp_ABC_CuentaXPagar(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo, Convert.ToInt16(Request.Form["CP"]), Convert.ToDecimal(Request.Form["cp_saldo"]), cuentaxpagar.cp_fechaUltMov, 0, "1", cuentaxpagar.cp_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_CuentaXPagar: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo.Value, cuentaxpagar.cp_Proveedor, cuentaxpagar.cp_Saldo, null, null, null, cuentaxpagar.cp_usuarioing, tsp, "-> R: " + validad.getResponse(error)));

                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                    int idCta = Convert.ToInt32(codigo.Value);
                    myDat = "Crea Movmiento Cuenta x Pagar / sp_ABC_MovimientoCuentaXPagar";
                    movimientoCuentaXPagar.mp_fechaing = System.DateTime.Now;
                    movimientoCuentaXPagar.mp_usuarioing = Session["UserName"].ToString();
                    movimientoCuentaXPagar.mp_FechaMov = System.DateTime.Now;
                    int resultM = db.sp_ABC_MovimientoCuentaXPagar(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), idCta, codigomv, Convert.ToInt16(Request.Form["SUC"]), 1, Convert.ToInt16(Request.Form["CP"]), movimientoCuentaXPagar.mp_FechaMov, Convert.ToDecimal(Request.Form["MontoMov"]), Convert.ToString(Request.Form["Credito"]), Convert.ToString(Request.Form["Descrip"]), "1", movimientoCuentaXPagar.mp_usuarioing, tsp2, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_MovimientoCuentaXPagar: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo.Value, cuentaxpagar.cp_Proveedor, cuentaxpagar.cp_Saldo, null, null, null, cuentaxpagar.cp_usuarioing, tsp, "-> R: " + validad.getResponse(error)));

                    if (error.Value.ToString() == "")
                    {
                        db.SaveChanges();
                        var data = new
                        {
                            status = 200,
                            message = "Cuenta y Movimiento, Agregada"
                        };
                        return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var data = new
                        {
                            status = 305,
                            message = "Operación Invalida, ver consola",
                            err = error.Value.ToString()
                        };

                        return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
                        throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
                    }


                }
                else
                {
                    var data = new
                    {
                        status = 305,
                        message = "Operación Invalida, ver consola",
                        err = error.Value.ToString()
                    };

                    return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
                }

            }
            catch (Exception ex)
            {
                var data = new
                {
                    status = 500,
                    message = "Excepción encontrada, ver consola",
                    excepcion = ex.ToString()
                };
                return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
            }
        }

    }
}