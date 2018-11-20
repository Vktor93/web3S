using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using WebGrupo3S.Models;
using System.Data.Entity.Core.Objects;
using WebGrupo3S.Helpers;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Specialized;


namespace WebGrupo3S.Views
{
    public class CuentaXCobrarsController : Controller
    {
        private SSS_OPERACIONEntities db = new SSS_OPERACIONEntities();
        private SSS_PERSONASEntities dbP = new SSS_PERSONASEntities();
        private SSS_OPERACIONEntities dbO = new SSS_OPERACIONEntities();
        private string myModulo = "Cuentas por cobrar";
        private string myModuloMv = "Movimiento Cuentas por Cobrar";
        private string myDat = "";
        public ConstantesP coP = new ConstantesP();
        public String tsp = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("cc_idCuentaxCobrar", typeof(int));
        ObjectParameter codigomv = new ObjectParameter("mc_idMovCuentaxCobrar", typeof(int));
        ObjectParameter tsp2 = new ObjectParameter("mc_timestamp", typeof(String));

        public class DatosSearchResultModel
        {
            public IEnumerable<sp_Busqueda_CuentaXCobrar_Result> DatosResults { get; set; }
        }

        // GET: CuentaXCobrars
        public ActionResult Index()
        {
            var DatosSearch = new DatosSearchResultModel();
            try
            {
                myDat = "Busqueda / sp_Busqueda_CuentaXCobrar";
                DatosSearch.DatosResults = db.sp_Busqueda_CuentaXCobrar(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, error).ToList();
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_CuentaXCobrar: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
            }
            catch (Exception ex)
            {
                //                return View("Error", new HandleErrorInfo(ex, myModulo, "Lista"));
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Lista", myDat });
            }
            return View(DatosSearch.DatosResults.ToList());
        }


        // GET: CuentaXCobrars/Details/5
        public ActionResult Details(int? id ,short? ids)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaXCobrar cuentaXCobrar = db.CuentaXCobrar.Find(id, ids);
            if (cuentaXCobrar == null)
            {
                return HttpNotFound();
            }
            return View(cuentaXCobrar);
        }

        // GET: CuentaXCobrars/Create
        public ActionResult Create()
        {
            
            ViewBag.CC = new SelectList(dbP.sp_Busqueda_Cliente(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, null, null, null, null, null, null, error).ToList(), "Cliente", "NombreLargo");
            
            return View();
        }

        // POST: CuentaXCobrars/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cc_empresa,cc_IdCuentaXCobrar,cc_Cliente,cc_Saldo,cc_fechaUltMov,cc_MontoUltMov,cc_estCuentaXCobrar,cc_fechaing,cc_fechamod,cc_usuarioing,cc_usuariomod,cc_maquinaing,cc_maquinamod,cc_estado,cc_timestamp")] CuentaXCobrar cuentaxcobrar)
        {

            try
            {
                myDat = "Crear nuevo registro cuentas x cobrar / sp_ABC_CuentaXCobrar";
                if (ModelState.IsValid)
                {
                    cuentaxcobrar.cc_usuarioing = Session["UserName"].ToString();
                    cuentaxcobrar.cc_fechaUltMov = DateTime.Now;                    
                    var result = db.sp_ABC_CuentaXCobrar(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo, cuentaxcobrar.cc_Cliente, cuentaxcobrar.cc_Saldo, cuentaxcobrar.cc_fechaUltMov, 0, "1", cuentaxcobrar.cc_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_CuentaXCobrar: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo.Value, cuentaxcobrar.cc_Cliente, cuentaxcobrar.cc_Saldo, null, null, null, cuentaxcobrar.cc_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
            return View(cuentaxcobrar);
        }

        // GET: CuentaXCobrars/Edit/5
        public ActionResult Edit(short? id)
        {
            CuentaXCobrar dato = new CuentaXCobrar();
            if (id == null)
            {
                return View("Error");
            }

            try
            {
                myDat = "Actualizar cuenta por cobrar: " + id.ToString() + " / sp_Busqueda_Cliente";
                ViewBag.CC = new SelectList(dbP.sp_Busqueda_Cliente(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, null, null, null, null, null, null, error).ToList(), "cl_cliente", "cl_nombrelargo");
                dato = db.CuentaXCobrar.Where(a => a.cc_IdCuentaXCobrar == id).First();
                if (dato== null)
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

        // POST: CuentaXCobrars/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cc_empresa,cc_IdCuentaXCobrar,cc_Cliente,cc_Saldo,cc_fechaUltMov,cc_MontoUltMov,cc_estCuentaXCobrar,cc_fechaing,cc_fechamod,cc_usuarioing,cc_usuariomod,cc_maquinaing,cc_maquinamod,cc_estado,cc_timestamp")] CuentaXCobrar cuentaxcobrar)
        {
            try
            {
                myDat = "Actualizar datos / sp_ABC_CuentaXCobrar";
                tsp = Convert.ToBase64String(cuentaxcobrar.cc_timestamp as byte[]);
                codigo.Value = cuentaxcobrar.cc_IdCuentaXCobrar;
                var result = db.sp_ABC_CuentaXCobrar(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo, cuentaxcobrar.cc_Cliente, cuentaxcobrar.cc_Saldo, null, null, null, cuentaxcobrar.cc_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_CuentaXCobrar: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo.Value, cuentaxcobrar.cc_Cliente, cuentaxcobrar.cc_Saldo, null, null, null, cuentaxcobrar.cc_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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

        // GET: CuentaXCobrars/Delete/5
        public ActionResult Delete(int? id)
        {
            CuentaXCobrar dato = new CuentaXCobrar();
            if (id == null)
            {
                return View("Error");
            }

            try
            {
                myDat = "Dar de baja a cuenta por cobrar: " + id.ToString();
                dato = db.CuentaXCobrar.Where(a => a.cc_IdCuentaXCobrar == id).First();
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

        // POST: CuentaXCobrars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CuentaXCobrar dato = new CuentaXCobrar();
            try
            {
                myDat = "Confirma dar de baja cuenta por cobrar: " + id.ToString() + " / sp_ABC_CuentaXCobrar";
                dato = db.CuentaXCobrar.Where(a => a.cc_IdCuentaXCobrar == id).First();
                tsp = Convert.ToBase64String(dato.cc_timestamp as byte[]);
                dato.cc_usuarioing = Session["UserName"].ToString();
                codigo.Value = id;
                var result = db.sp_ABC_CuentaXCobrar(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo,null, null, null, null, null, dato.cc_usuarioing, tsp, error);

                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_CuentaXCobrar: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo.Value, null, null, null, null, null, dato.cc_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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

        //funcion para renderizar y retornar la vista crear cuerpo documento
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

                ViewBag.SUC = new SelectList(dbO.sp_Busqueda_Sucursal(1, "", empre, null, null, null, null, null, error).ToList(), "CodigoSucursal", "NombreSucursal");
                ViewBag.DOC = new SelectList(dbO.sp_Busqueda_Documento(1, "", empre, null, null, null, "CL", null, null, null, null, null, null, null,null,null,null,null,null,null,null,error).ToList(),"CodigoDocumento","TipoDocumento");
                string viewContent = ConvertView("bodyDoc");
                return Json(new {PartialView = viewContent }, JsonConvert.SerializeObject(data));

            }
            catch (Exception ex) {
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
        public JsonResult getSaldo(int? id)
        {
            try
            {
                CuentaXCobrar dato = db.CuentaXCobrar.Where(a => a.cc_Cliente == id).First();
                return Json(dato, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) {
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
        public JsonResult saveCuenta( [Bind(Include = "cc_empresa,cc_IdCuentaXCobrar,cc_Cliente,cc_Saldo,cc_fechaUltMov,cc_MontoUltMov,cc_estCuentaXCobrar,cc_fechaing,cc_fechamod,cc_usuarioing,cc_usuariomod,cc_maquinaing,cc_maquinamod,cc_estado,cc_timestamp")] CuentaXCobrar cuentaxcobrar, [Bind(Include = "mc_empresa,mc_IdCuentaXCobrar,mc_IdMovCuentaXCobrar,mc_IdSucursal,mc_IdDoc,mc_Cliente,mc_FechaMov,mc_MontoMov,mc_CreditoDebito,mc_Descripcion,mc_estMovCuentaXCobrar,mc_fechaing,mc_fechamod,mc_usuarioing,mc_usuariomod,mc_maquinaing,mc_maquinamod,mc_estado,mc_timestamp")] MovimientoCuentaXCobrar movimientoCuentaXCobrar)
        {
            //NameValueCollection coll;
            //coll = Request.Form;

            try
            {
                 
                myDat = "Crea Cuenta x Cobrar / sp_ABC_CuentaXCobrar";
                cuentaxcobrar.cc_fechaing = System.DateTime.Now;
                cuentaxcobrar.cc_usuarioing = Session["UserName"].ToString();
                cuentaxcobrar.cc_fechaUltMov = System.DateTime.Now;               
                var result = db.sp_ABC_CuentaXCobrar(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo, Convert.ToInt16(Request.Form["CC"]), Convert.ToDecimal(Request.Form["total_monto"]), cuentaxcobrar.cc_fechaUltMov, Convert.ToDecimal(Request.Form["Monto"].Last()), "1", cuentaxcobrar.cc_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_CuentaXCobrar: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo.Value, cuentaxcobrar.cc_Cliente, cuentaxcobrar.cc_Saldo, null, null, null, cuentaxcobrar.cc_usuarioing, tsp, "-> R: " + validad.getResponse(error)));

                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();                    
                    int idCta = Convert.ToInt32(codigo.Value);                    
                    myDat = "Crea Movmiento Cuenta x Cobrar / sp_ABC_MovimientoCuentaXCobrar";
                    int size = Request.Form["Cred"].Count();
                    movimientoCuentaXCobrar.mc_fechaing = System.DateTime.Now;
                    movimientoCuentaXCobrar.mc_usuarioing = Session["UserName"].ToString();
                    movimientoCuentaXCobrar.mc_FechaMov = System.DateTime.Now;

                    for(int i = 0; i < size; i++)
                    {
                        int resultM = db.sp_ABC_MovimientoCuentaXCobrar(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), idCta, codigomv, Convert.ToInt16(Request.Form["SUC"][i]), 1, Convert.ToInt16(Request.Form["CC"]), movimientoCuentaXCobrar.mc_FechaMov, Convert.ToDecimal(Request.Form["Monto"][i]), Convert.ToString(Request.Form["Cred"][i]), Convert.ToString(Request.Form["desc"][i]), "1", movimientoCuentaXCobrar.mc_usuarioing, tsp2, error);
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_MovimientoCuentaXCobrar: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo.Value, cuentaxcobrar.cc_Cliente, cuentaxcobrar.cc_Saldo, null, null, null, cuentaxcobrar.cc_usuarioing, tsp, "-> R: " + validad.getResponse(error)));

                        if (error.Value.ToString() == "")
                        {
                            db.SaveChanges();
                           
                        }
                        else
                        {
                            var datas = new
                            {
                                status = 305,
                                message = "Operación Invalida, ver consola",
                                err = error.Value.ToString()
                            };

                            return Json(JsonConvert.SerializeObject(datas), JsonRequestBehavior.AllowGet);
                            throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
                        }
                    }

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
            catch(Exception ex)
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
