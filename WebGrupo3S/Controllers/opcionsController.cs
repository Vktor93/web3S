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

namespace WebGrupo3S.Views
{
    public class opcionsController : Controller
    {
        private SSS_PERSONASEntities db = new SSS_PERSONASEntities();
        private string myModulo = "Opciones";
        private string mycatalogo = "CATALOGO PERMISOS";
        public String myDat = "";
        public ConstantesP coP = new ConstantesP();
        public String tsp = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("op_CodigoOpcion", typeof(int));

        // GET: opcions
        public ActionResult Index()
        {
            try
            {
                Session["idApadre"] = "0";
                int[] myacceso = new int[20];
                accesos ac = new accesos();

                for (int i = 0; i < 20; i++)
                {
                    myacceso[i] = 0;
                }
                ViewData["nombre"] = Session["UserName"];
                myDat = "Lista de opciones / sp_CargaPermiso";
                ObjectResult resultado = db.sp_CargaPermisos(Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["UserId"]), error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_CargaPermisos: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["UserId"]), "-> R: " + validad.getResponse(error)));
                foreach (sp_CargaPermisos_Result re in resultado)
                {
                    if (myacceso[0] != 0)
                        ac.CargaPerP(re.op_nombreopcion, re.CodPadre, ref myacceso);
                    else
                        myacceso[0] = ac.BuscaPadre(re.op_nombreopcion, re.op_codigoopcion, mycatalogo);
                }
                for (int i = 1; i < 20; i++)
                {
                    ac.acc = ac.acc + myacceso[i].ToString();
                }
                Session[myModulo] = ac.acc;


                Session[myModulo] = "11111111111111111111111111111111111111111111";


                if (Session["UserId"].ToString() != "")
                {
                    if (Session[myModulo].ToString().Substring(0, 1) == "1")
                    {
                        myDat = "Ingreso de opciones";
                        List<opcion> opciones = new List<opcion>();
                        opciones = db.opcion.Where(a => a.op_PadreOpcion <= 1 && a.op_Estado == 1).ToList();
                        ViewData["Opcion"] = "Menu principal";
                        return View(opciones);
                    }
                    else
                        throw new System.InvalidOperationException("-Ingreso a opción denegado!-", new Exception(""));
                }
                else
                    throw new System.InvalidOperationException("-Ingrese al Sistema para poder Ingresar a esta Opción-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Ingreso", myDat });
            }
        }

        public class OpcionSearchResultModel
        {
            public IEnumerable<sp_Busqueda_Opcion_Result> OpcionResults { get; set; }
        }

        public ActionResult PCreate(short? id, string nom)
        {
            opcion dato = new opcion();
            try
            {
                myDat = "Crea opción: " + id.ToString() + " - " + nom;
                dato.op_PadreOpcion = Convert.ToInt16(id);
                ViewBag.idpadre = id;
                ViewBag.nombre = nom;
                ViewData["nombre"] = nom;                
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crear opción", myDat });
            }
            return View(dato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PCreate([Bind(Include = "op_Empresa,op_CodigoOpcion,op_NombreOpcion,op_DescOpcion,op_PadreOpcion,op_Autenticar,op_FechaIng,op_FechaMod,op_UsuarioIng,op_UsuarioMod,op_MaquinaIng,op_MaquinaMod,op_Estado,op_timestamp")] opcion op)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    myDat = "Crea nueva opcion / sp_ABC_Opcion";
                    op.op_FechaIng = System.DateTime.Now;
                    op.op_UsuarioIng = Session["UserName"].ToString();
                    int result = db.sp_ABC_Opcion(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo, op.op_NombreOpcion, op.op_DescOpcion, op.op_PadreOpcion, op.op_Autenticar, op.op_UsuarioIng, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Opcion: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo.Value, op.op_NombreOpcion, op.op_DescOpcion, op.op_PadreOpcion, op.op_Autenticar, op.op_UsuarioIng, tsp, "-> R: " + validad.getResponse(error)));
                    if (error.Value.ToString() == "")
                    {
                        db.SaveChanges();
                        /*if (Session["idApadre"].ToString() != "")
                            return RedirectToAction("Popcion", new { id = op.op_PadreOpcion, idp = Convert.ToInt16(Session["idApadre"].ToString()), nom = Convert.ToString(Session["nombreO"]) });
                        else*/
                            return RedirectToAction("Details", new { id = op.op_PadreOpcion, idp = Convert.ToInt16(Session["idApadre"].ToString()), nom = Convert.ToString(Session["nombre"]) });
                    }
                    else
                        throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
                }
                catch (Exception ex)
                {
                    return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crear nueva opción", myDat });
                }
            }
            return View(op);
        }

        public ActionResult PEdit(short? id, short? idp, string nom)
        {
            opcion dato = new opcion();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                myDat = "Actualización de opciones: " + id.ToString() + " - " + nom + " / sp_Busqueda_Opcion";
                ViewBag.idpadre = idp;
                ViewBag.nombre = nom;
                ViewData["nombre"] = nom;
                ObjectResult resultado = db.sp_Busqueda_Opcion(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Opcion: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_Opcion_Result re in resultado)
                {
                    dato = CargaOpcion(re);
                }
                if (dato == null)
                    throw new System.InvalidOperationException("-Error al obtener registro-", new Exception(""));

            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Actualiza opción", myDat });
            }
            return View(dato);
        }


        // POST: opcions/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PEdit([Bind(Include = "op_Empresa,op_CodigoOpcion,op_NombreOpcion,op_DescOpcion,op_PadreOpcion,op_Autenticar,op_FechaIng,op_FechaMod,op_UsuarioIng,op_UsuarioMod,op_MaquinaIng,op_MaquinaMod,op_Estado,op_timestamp")] opcion op)
        {
            try
            {
                myDat = "Actualiza opción / sp_ABC_Opcion";
                tsp = Convert.ToBase64String(op.op_timestamp as byte[]);
                codigo.Value = op.op_CodigoOpcion;
                op.op_UsuarioIng = Session["UserName"].ToString();
                int result = db.sp_ABC_Opcion(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo, op.op_NombreOpcion, op.op_DescOpcion, op.op_PadreOpcion, op.op_Autenticar, op.op_UsuarioIng, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Opcion: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo.Value, op.op_NombreOpcion, op.op_DescOpcion, op.op_PadreOpcion, op.op_Autenticar, op.op_UsuarioIng, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                }
                else
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Actualiza opción", myDat });
            }
            /*if (Session["idApadre"].ToString() != "")
                return RedirectToAction("Popcion", new { id = op.op_PadreOpcion, idp = Convert.ToInt16(Session["idApadre"].ToString()), nom = Convert.ToString(Session["nombreO"]) });
            else*/
                return RedirectToAction("Details", new { id = op.op_PadreOpcion, idp = Convert.ToInt16(Session["idApadre"].ToString()), nom = Convert.ToString(Session["nombre"]) });
        }

        public ActionResult PDelete(short? id, short? idp, string nom)
        {
            opcion op = new opcion();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                myDat = "Dando de baja a opción: " + id.ToString() + " - " + nom + " / sp_Busqueda_Opcion";
                ViewData["nombre"] = nom;
                ViewBag.idpadre = idp;
                ViewBag.nombre = nom;

                ObjectResult resultado = db.sp_Busqueda_Opcion(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Opcion: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_Opcion_Result re in resultado)
                {
                    op = CargaOpcion(re);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "".ToString(), modulo = myModulo, opcion = "Da de baja a opción", myDat });
            }
            return View(op);
        }

        // POST: opcions/Delete/5
        [HttpPost, ActionName("PDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult PDeleteConfirmed(short id)
        {
            opcion op = new opcion();
            try
            {
                myDat = "Confirma de baja a opción: " + id.ToString() + " / sp_Busqueda_Opcion";
                ObjectResult resultado = db.sp_Busqueda_Opcion(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Opcion: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, "-> R: " + validad.getResponse(error)));

                foreach (sp_Busqueda_Opcion_Result re in resultado)
                {
                    op = CargaOpcion(re);
                    tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                }

                codigo.Value = op.op_CodigoOpcion;
                op.op_UsuarioIng = Session["UserName"].ToString();
                int result = db.sp_ABC_Opcion(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo, op.op_NombreOpcion, op.op_DescOpcion, op.op_PadreOpcion, op.op_Autenticar, op.op_UsuarioIng, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Opcion: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo.Value, op.op_NombreOpcion, op.op_DescOpcion, op.op_PadreOpcion, op.op_Autenticar, op.op_UsuarioIng, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                }
                else
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));

            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Confirma baja de opción", myDat });
            }
            /*if (Session["idApadre"].ToString() != "")
                return RedirectToAction("Popcion", new { id = op.op_PadreOpcion, idp = Convert.ToInt16(Session["idApadre"].ToString()), nom = Convert.ToString(Session["nombreO"]) });
            else*/
                return RedirectToAction("Details", new { id = op.op_PadreOpcion, idp = Convert.ToInt16(Session["idApadre"].ToString()), nom = Convert.ToString(Session["nombre"]) });
        }

        public ActionResult Regresa()
        {
            return RedirectToAction("Index");
        }

        // GET: opcions/Details/5
        public ActionResult Details(short? id, short? idp, string nom)
        {
            try
            {
                myDat = "Lista de opciones: " + id.ToString() + " - " + nom;
                if (Session[myModulo].ToString().Substring(10, 1) == "1")
                {
                    if (idp == null) idp = Convert.ToInt16(Session["idApadre"].ToString());
                    if (id == 0) return RedirectToAction("Index");
                    List<opcion> opciones = new List<opcion>();
                    opciones = db.opcion.Where(a => a.op_PadreOpcion == id && a.op_Estado==1).ToList();
                    ViewData["Opcion"] = nom;
                    Session["nombre"] = nom;
                    Session["idApadre"] = idp;
                    ViewBag.idpadre = id;
                    ViewBag.nombre = nom;
                    return View(opciones);
                }
                else
                    throw new System.InvalidOperationException("-No tiene acceso a esta opción!-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "List", myDat });
            }
        }

        // GET: opcions/Create
        public ActionResult Create()
        {
            try
            {
                myDat = "Crear nueva opción";
                if (Session[myModulo].ToString().Substring(1, 1) == "0")
                    throw new System.InvalidOperationException("-No tiene acceso a crear nuevo registros-", new Exception(""));
                else
                {
                    ViewBag.PadreId = new SelectList(db.opcion.Where(a => a.op_PadreOpcion <= 1).ToList(), "op_CodigoOpcion", "op_DescOpcion");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crear nueva opción", myDat });
            }
            return View();
        }

        // POST: opcions/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "op_Empresa,op_CodigoOpcion,op_NombreOpcion,op_DescOpcion,op_PadreOpcion,op_Autenticar,op_FechaIng,op_FechaMod,op_UsuarioIng,op_UsuarioMod,op_MaquinaIng,op_MaquinaMod,op_Estado,op_timestamp")] opcion opcion)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    myDat = "Crear nueva opción / sp_ABC_Opcion";
                    opcion.op_FechaIng = System.DateTime.Now;
                    opcion.op_UsuarioIng = Session["UserName"].ToString();
                    int result = db.sp_ABC_Opcion(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo, opcion.op_NombreOpcion, opcion.op_DescOpcion, opcion.op_PadreOpcion, opcion.op_Autenticar, opcion.op_UsuarioIng, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Opcion: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo.Value, opcion.op_NombreOpcion, opcion.op_DescOpcion, opcion.op_PadreOpcion, opcion.op_Autenticar, opcion.op_UsuarioIng, tsp, "-> R: " + validad.getResponse(error)));
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
                    return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crear nueva opción", myDat });
                }
            }
            return View(opcion);
        }
       
        // GET: opcions/Edit/5
        public ActionResult Edit(short? id)
        {
            opcion dato = new opcion();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                if (Session[myModulo].ToString().Substring(2, 1) == "1")
                {
                    myDat = "Actualiza opción: " + id.ToString() + " / sp_Busqueda_Opcion";
                    ViewBag.PadreId = new SelectList(db.opcion.Where(a => a.op_CodigoOpcion <= 1).ToList(), "op_CodigoOpcion", "op_DescOpcion");
                    ObjectResult resultado = db.sp_Busqueda_Opcion(2, "", Convert.ToInt16(coP.cls_empresa), id, null,null, null, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Opcion: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_Opcion_Result re in resultado)
                    {
                        dato = CargaOpcion(re);
                    }
                    if (dato == null)
                        throw new System.InvalidOperationException("-Error al obtener registro-", new Exception(""));
                }
                else
                    throw new System.InvalidOperationException("-No tiene acceso a la opcion de actualización-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Actualiza opción", myDat });
            }
            return View(dato);
        }

        private opcion CargaOpcion(sp_Busqueda_Opcion_Result parResult)
        {
            opcion pf = new opcion();

            pf.op_Empresa = parResult.Empresa;
            pf.op_CodigoOpcion = parResult.CodigoOpcion;
            pf.op_DescOpcion = parResult.DescripcionOpcion;
            pf.op_NombreOpcion = parResult.NombreOpcion;
            pf.op_PadreOpcion = parResult.PadreOpcion;
            pf.op_timestamp = parResult.TimeStamp;
            return pf;
        }

        // POST: opcions/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "op_Empresa,op_CodigoOpcion,op_NombreOpcion,op_DescOpcion,op_PadreOpcion,op_Autenticar,op_FechaIng,op_FechaMod,op_UsuarioIng,op_UsuarioMod,op_MaquinaIng,op_MaquinaMod,op_Estado,op_timestamp")] opcion opcion)
        {
            try
            {
                myDat = "Actualiza opción / sp_ABC_Opcion";
                tsp = Convert.ToBase64String(opcion.op_timestamp as byte[]);
                codigo.Value = opcion.op_CodigoOpcion;
                opcion.op_UsuarioIng = Session["UserName"].ToString();
                int result = db.sp_ABC_Opcion(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo, opcion.op_NombreOpcion, opcion.op_DescOpcion, opcion.op_PadreOpcion, opcion.op_Autenticar, opcion.op_UsuarioIng, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Opcion: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo.Value, opcion.op_NombreOpcion, opcion.op_DescOpcion, opcion.op_PadreOpcion, opcion.op_Autenticar, opcion.op_UsuarioIng, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                    String Noregistro = codigo.Value.ToString();
                }
                else
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Actualiza opción", myDat });
            }

            return RedirectToAction("Index");
        }

        // GET: opcions/Delete/5
        public ActionResult Delete(short? id)
        {
            opcion opcion = new opcion();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                myDat = "Da de baja la opción: " + id.ToString() + " / sp_Busqueda_Opcion";
                if (Session[myModulo].ToString().Substring(3, 1) == "1")
                {
                    ObjectResult resultado = db.sp_Busqueda_Opcion(2, "", Convert.ToInt16(coP.cls_empresa), id,null,null, null, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Opcion: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_Opcion_Result re in resultado)
                    {
                        opcion = CargaOpcion(re);
                    }
                }
                else
                    throw new System.InvalidOperationException("-No tiene permitido dar de baja el registro-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Da de baja a opción", myDat });
            }
            return View(opcion);
        }

        // POST: opcions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            opcion opcion = new opcion();
            try
            {
                myDat = "Confirma dar de baja opción: " + id.ToString() + " / sp_Busqueda_Opcion";
                ObjectResult resultado = db.sp_Busqueda_Opcion(2, "", Convert.ToInt16(coP.cls_empresa), id, null,null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Opcion: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, "-> R: " + validad.getResponse(error)));

                foreach (sp_Busqueda_Opcion_Result re in resultado)
                {
                    opcion = CargaOpcion(re);
                    tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                }

                codigo.Value = opcion.op_CodigoOpcion;
                opcion.op_UsuarioIng = Session["UserName"].ToString();
                int result = db.sp_ABC_Opcion(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo, opcion.op_NombreOpcion, opcion.op_DescOpcion, opcion.op_PadreOpcion, opcion.op_Autenticar, opcion.op_UsuarioIng, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Opcion: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo.Value, opcion.op_NombreOpcion, opcion.op_DescOpcion, opcion.op_PadreOpcion, opcion.op_Autenticar, opcion.op_UsuarioIng, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                    String Noregistro = codigo.Value.ToString();
                }
                else
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));

            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Comfirma baja de opción", myDat });
            }
            return RedirectToAction("Index");
        }


        public ActionResult RegresaDos(short? id, string nom)
        {
            Session["idApadre"] = "";
            return RedirectToAction("Details", new { id = id, nom = Session["Nombre"].ToString() });
        }

        // GET: opcions/Details/5
        public ActionResult Popcion(short? id, short? idp, string nom)
        {
            try
            {
                myDat = "Lista de sub opciones: " + id.ToString() + " - " + nom;
                if (Session[myModulo].ToString().Substring(10, 1) == "1")
                {
                    List<opcion> opciones = new List<opcion>();
                    opciones = db.opcion.Where(a => a.op_PadreOpcion == id && a.op_Estado == 1).ToList();
                    ViewData["Opcion"] = nom;
                    ViewBag.idpadre = id;
                    Session["idApadre"] = idp;
                    Session["nombreO"] = nom;
                    return View(opciones);
                }
                else
                    throw new System.InvalidOperationException("-No tiene acceso a esta opción!-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "List", myDat });
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
