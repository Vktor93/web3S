using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity.Core.Objects;
using WebGrupo3S.Models;
using Microsoft.Reporting.WebForms;
using System.IO;
using WebGrupo3S.Helpers;

namespace WebGrupo3S.Controllers
{
    public class EmpleadoesController : Controller
    {
        private SSS_PERSONASEntities dbP = new SSS_PERSONASEntities();
        private SSS_PLANILLAEntities db = new SSS_PLANILLAEntities();
        private SSS_COMPLEMENTOSEntities dbC = new SSS_COMPLEMENTOSEntities();
        private string mycatalogo = "Mantenimiento Empleados";
        private string myModulo = "Empleados";
        public ConstantesP coP = new ConstantesP();
        public String tsp = "";
        public String myDat = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("em_IdEmpleado", typeof(int));
        ObjectParameter codigoC = new ObjectParameter("cn_idcontacto", typeof(int));
        ObjectParameter codigoD = new ObjectParameter("di_idDireccion", typeof(int));


        public class DatosSearchResultModel
        {
            public IEnumerable<sp_Busqueda_Empleado_Result> DatosResults { get; set; }
        }

        // GET: Empleados
        public ActionResult Index()
        {
            validad ac = new validad();

            var DatosSearch = new DatosSearchResultModel();
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
                            ViewBag.nuevo = Session[myModulo].ToString().Substring(1,1);
                            ViewBag.editar = Session[myModulo].ToString().Substring(2, 1);
                            ViewBag.debaja = Session[myModulo].ToString().Substring(3, 1);
                            ViewBag.contactos = Session[myModulo].ToString().Substring(8, 1);
                            ViewBag.direccion = Session[myModulo].ToString().Substring(7, 1);
                            ViewBag.horarios = Session[myModulo].ToString().Substring(6, 1);
                            ViewBag.reporte = Session[myModulo].ToString().Substring(19, 1);

                            myDat = "Generación de lista de empleados / sp_Busqueda_Empleado";
                            buscaCatalogos();
                            DatosSearch.DatosResults = db.sp_Busqueda_Empleado(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, error).ToList();
                            WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Empleado: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                        }
                        catch (Exception ex)
                        {
                            return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Ingreso", myDat });
                        }
                        return View(DatosSearch.DatosResults.ToList());
                    }
                    else
                        throw new System.InvalidOperationException("-Ingreso a opción denegado!-", new Exception(""));
                }
                else
                    throw new System.InvalidOperationException("-Ingrese al Sistema para poder Ingresar a esta Opción-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Ingreso a empleados", myDat});
            }
        }


        public void buscaCatalogos()
        {
            Session["TE"] = TraeIdCatalogo("TipoEmpleado");
            Session["TI"] = TraeIdCatalogo("TipoIdentifica");
            Session["GE"] = TraeIdCatalogo("Genero");
            Session["OP"] = TraeIdCatalogo("Ocupacion");
            Session["EC"] = TraeIdCatalogo("EstadoCivil");
            Session["CO"] = TraeIdCatalogo("TipoContacto");
            Session["DI"] = TraeIdCatalogo("TipoDireccion");
            Session["MU"] = TraeIdCatalogo("Municipios");
            Session["DE"] = TraeIdCatalogo("Departamentos");
            Session["PA"] = TraeIdCatalogo("Paises");
        }

        public int TraeIdCatalogo(string catalogo)
        {
            int numero = 0;
            try
            {
                ObjectResult resultadoCO = dbC.sp_Busqueda_CabCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), null, catalogo, null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_CabCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, catalogo, null, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_CabCatalogo_Result re in resultadoCO)
                {
                    numero = re.IdCatalogo;
                }
            }
            catch (Exception)
            {
                numero = 0;
            }
            return numero;
        }

        public JsonResult AsignaHEmpleado(int id)
        {
            HorarioEmpleado dato = new HorarioEmpleado();
            List<SelectListItem> hempl = new List<SelectListItem>();

            try
            {
                int idempleado = Convert.ToInt32(Session["idempleado"].ToString());
                ObjectResult resultado = db.sp_Busqueda_Horario(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Horario: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_Horario_Result re in resultado)
                {
                    dato.he_IdHorario = re.IdHorario;
                    dato.he_diaSemana = re.DiaSemana;
                }

                dato.he_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_HorarioEmpleado(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), dato.he_IdHorario, dato.he_diaSemana, idempleado, dato.he_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_HorarioEmpleado: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), dato.he_IdHorario, dato.he_diaSemana, idempleado, dato.he_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                }
                else
                {
                    ViewBag.Message = error.Value.ToString();
                }
                resultado = db.sp_Busqueda_HorarioEmpleado(2, "", Convert.ToInt16(coP.cls_empresa), null, null, idempleado, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_HorarioEmpleado: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, null, idempleado, tsp, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_HorarioEmpleado_Result re in resultado)
                {
                    hempl.Add(new SelectListItem { Text = re.DescripcionHorario, Value = re.IdHorario.ToString() });
                }
            }
            catch (Exception ex)
            {

            }
            return Json(new SelectList(hempl, "Value", "Text"));
        }
        public JsonResult BajaHEmpleado(int id)
        {
            HorarioEmpleado dato = new HorarioEmpleado();
            List<SelectListItem> hempl = new List<SelectListItem>();
            try
            {
                int idempleado = Convert.ToInt32(Session["idempleado"].ToString());
                ObjectResult resultado = db.sp_Busqueda_Horario(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Horario: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_Horario_Result re in resultado)
                {
                    dato.he_IdHorario = re.IdHorario;
                    dato.he_diaSemana = re.DiaSemana;
                }

                resultado = db.sp_Busqueda_HorarioEmpleado(2, "", Convert.ToInt16(coP.cls_empresa), id, dato.he_diaSemana,idempleado, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_HorarioEmpleado: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, dato.he_diaSemana, idempleado, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_HorarioEmpleado_Result re in resultado)
                {
                    dato.he_IdHorario = re.IdHorario;
                    dato.he_diaSemana = re.DiaSemana;
                    dato.he_CodHorarioEmpleado = re.CodHorarioEmpleado;
                    tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                }
                
                dato.he_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_HorarioEmpleado(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), id, dato.he_diaSemana, idempleado, dato.he_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_HorarioEmpleado: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), dato.he_CodHorarioEmpleado, id, dato.he_diaSemana, idempleado, dato.he_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                    db.SaveChanges();
                else
                    throw new System.InvalidOperationException(error.Value.ToString());

                resultado = db.sp_Busqueda_HorarioEmpleado(2, "", Convert.ToInt16(coP.cls_empresa), null, null, idempleado, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_HorarioEmpleado: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, null, idempleado, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_HorarioEmpleado_Result re in resultado)
                {
                    hempl.Add(new SelectListItem { Text = re.DescripcionHorario, Value = re.IdHorario.ToString() });
                }
            }
            catch (Exception ex)
            {

            }
            return Json(new SelectList(hempl, "Value", "Text"));
        }

        public ActionResult RTodos()
        {
            String mierror = "";
            HorarioEmpleado dato = new HorarioEmpleado();
            int idempleado = 0;
            try
            {
                idempleado = Convert.ToInt32(Session["idempleado"].ToString());
                ObjectResult resultado = db.sp_Busqueda_Horario(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Horario: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_Horario_Result re in resultado)
                {
                    dato.he_IdEmpleado = idempleado;
                    dato.he_IdHorario = re.IdHorario;
                    dato.he_diaSemana = re.DiaSemana;
                    dato.he_usuarioing = Session["UserName"].ToString();
                    if (!db.HorarioEmpleadoes.Where(a => a.he_IdEmpleado == idempleado && a.he_IdHorario == re.IdHorario && a.he_estado == 1).Any())
                    {
                        myDat = "Crea horario del empleado = " + dato.he_IdEmpleado.ToString() + " - horario:" + dato.he_IdHorario.ToString() + " / sp_ABC_HorarioEmpleado";
                        mierror = "";
                        GrabaHorarioE(dato, ref mierror);
                        if (mierror != "")
                            throw new System.InvalidOperationException(mierror, new Exception(""));
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Agregando horarios a empleado", myDat });
            }

            return RedirectToAction("Horarios", new { id = idempleado });
        }

        public JsonResult GetRSelec(string datos)
        {
            List<SelectListItem> hempl = new List<SelectListItem>();
            HorarioEmpleado dato = new HorarioEmpleado();
            string[] opci = datos.Split(',');
            try
            {
                int idempleado = Convert.ToInt32(Session["idempleado"].ToString());
                foreach (string dat in opci)
                {
                    Int16 coHor = Convert.ToInt16(dat);
                    ObjectResult resultado = db.sp_Busqueda_Horario(2, "", Convert.ToInt16(coP.cls_empresa), coHor, null, null, null, null, null, null, null, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Horario: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), coHor, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_Horario_Result re in resultado)
                    {
                        dato.he_IdHorario = re.IdHorario;
                        dato.he_diaSemana = re.DiaSemana;
                    }

                    dato.he_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_HorarioEmpleado(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), dato.he_IdHorario, dato.he_diaSemana, idempleado, dato.he_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_HorarioEmpleado: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), dato.he_IdHorario, dato.he_diaSemana, idempleado, dato.he_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                    if (error.Value.ToString() == "")
                    {
                        db.SaveChanges();
                    }

                }
                ObjectResult resulta1 = db.sp_Busqueda_HorarioEmpleado(2, "", Convert.ToInt16(coP.cls_empresa), null, null, idempleado, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_HorarioEmpleado: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, null, idempleado, tsp, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_HorarioEmpleado_Result re in resulta1)
                {
                    hempl.Add(new SelectListItem { Text = re.DescripcionHorario, Value = re.IdHorario.ToString() });
                }
            }
            catch (Exception ex)
            {

            }
            return Json(new SelectList(hempl, "Value", "Text"));
        }

        public JsonResult GetLSelec(string datos)
        {
            List<SelectListItem> hempl = new List<SelectListItem>();
            HorarioEmpleado dato = new HorarioEmpleado();
            string[] opci = datos.Split(',');
            try
            {
                int idempleado = Convert.ToInt32(Session["idempleado"].ToString());
                foreach (string dat in opci)
                {
                    Int16 coHor = Convert.ToInt16(dat);
                    ObjectResult resultado = db.sp_Busqueda_Horario(2, "", Convert.ToInt16(coP.cls_empresa), coHor, null, null, null, null, null, null, null, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Horario: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), coHor, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_Horario_Result re in resultado)
                    {
                        dato.he_IdHorario = re.IdHorario;
                        dato.he_diaSemana = re.DiaSemana;
                    }

                    resultado = db.sp_Busqueda_HorarioEmpleado(2, "", Convert.ToInt16(coP.cls_empresa), coHor, dato.he_diaSemana, idempleado, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_HorarioEmpleado: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), coHor, dato.he_diaSemana, idempleado, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_HorarioEmpleado_Result re in resultado)
                    {
                        dato.he_IdHorario = re.IdHorario;
                        dato.he_diaSemana = re.DiaSemana;
                        dato.he_CodHorarioEmpleado = re.CodHorarioEmpleado;
                        tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                    }

                    dato.he_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_HorarioEmpleado(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), dato.he_IdHorario, dato.he_diaSemana, idempleado, dato.he_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_HorarioEmpleado: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), dato.he_CodHorarioEmpleado, dato.he_IdHorario, dato.he_diaSemana, idempleado, dato.he_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                    if (error.Value.ToString() == "")
                    {
                        db.SaveChanges();
                    }

                }
                ObjectResult resulta1 = db.sp_Busqueda_HorarioEmpleado(2, "", Convert.ToInt16(coP.cls_empresa), null, null, idempleado, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_HorarioEmpleado: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, null, idempleado, tsp, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_HorarioEmpleado_Result re in resulta1)
                {
                    hempl.Add(new SelectListItem { Text = re.DescripcionHorario, Value = re.IdHorario.ToString() });
                }
            }
            catch (Exception ex)
            {

            }
            return Json(new SelectList(hempl, "Value", "Text"));
        }

        public JsonResult TraeHorarios()
        {
            HorarioEmpleado dato = new HorarioEmpleado();
            List<SelectListItem> perf = new List<SelectListItem>();
            try
            {
                int id = Convert.ToInt16(Session["idempleado"]);
                ObjectResult resultado = db.sp_Busqueda_HorarioEmpleado(3, "", Convert.ToInt16(coP.cls_empresa), null, null, id, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando db.sp_Busqueda_HorarioEmpleado: " + string.Join(",", 3, "", Convert.ToInt16(coP.cls_empresa), null, null, id, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_HorarioEmpleado_Result re in resultado)
                {
                    perf.Add(new SelectListItem { Text = re.DescripcionHorario, Value = re.IdHorario.ToString() });
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
            HorarioEmpleado dato = new HorarioEmpleado();
            int idempleado = 0;
            try
            {
                idempleado = Convert.ToInt32(Session["idempleado"].ToString());
                ObjectResult resultado = db.sp_Busqueda_HorarioEmpleado(2, "", Convert.ToInt16(coP.cls_empresa),null, null, idempleado, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_HorarioEmpleado: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, null, idempleado, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_HorarioEmpleado_Result re in resultado)
                {
                    myDat = "Baja a horario del empleado= " + dato.he_IdEmpleado.ToString() + " - horario:" + dato.he_IdHorario.ToString() + " / sp_ABC_HorarioEmpleado";
                    dato.he_IdEmpleado = idempleado;
                    dato.he_IdHorario = re.IdHorario;
                    dato.he_diaSemana = re.DiaSemana;
                    dato.he_timestamp = re.TimeStamp;
                    dato.he_usuarioing = Session["UserName"].ToString();
                    mierror = "";
                    BajaHorarioE(dato, ref mierror);
                    if (mierror != "")
                        throw new System.InvalidOperationException(mierror, new Exception(""));
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Da de baja a horarios de empleado", myDat });
            }

            return RedirectToAction("Horarios", new { id = idempleado });
        }

        public Boolean GrabaHorarioE(HorarioEmpleado dato, ref string err)
        {
            try
            {
                SSS_PLANILLAEntities dbh = new SSS_PLANILLAEntities();
                int result = dbh.sp_ABC_HorarioEmpleado(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), dato.he_IdHorario, dato.he_diaSemana,dato.he_IdEmpleado, dato.he_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_HorarioEmpleado: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), dato.he_IdHorario, dato.he_diaSemana, dato.he_IdEmpleado, dato.he_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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


        public Boolean BajaHorarioE(HorarioEmpleado dato, ref string err)
        {
            try
            {
                SSS_PLANILLAEntities dbh = new SSS_PLANILLAEntities();
                int idempleado = Convert.ToInt32(Session["idempleado"].ToString());

                ObjectResult resultado = db.sp_Busqueda_HorarioEmpleado(2, "", Convert.ToInt16(coP.cls_empresa), dato.he_IdHorario, dato.he_diaSemana, idempleado, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_HorarioEmpleado: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), dato.he_IdHorario, dato.he_diaSemana, idempleado, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_HorarioEmpleado_Result re in resultado)
                {
                    dato.he_IdHorario = re.IdHorario;
                    dato.he_diaSemana = re.DiaSemana;
                    dato.he_CodHorarioEmpleado = re.CodHorarioEmpleado;
                    tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                }

                myDat = "Da de baja horario del empleado= " + dato.he_IdEmpleado.ToString() + " - horario:" + dato.he_IdHorario.ToString() + " / sp_ABC_HorarioEmpleado";
                tsp = Convert.ToBase64String(dato.he_timestamp as byte[]);
                int result = dbh.sp_ABC_HorarioEmpleado(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), dato.he_IdHorario, dato.he_diaSemana, dato.he_IdEmpleado, dato.he_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_HorarioEmpleado: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), dato.he_CodHorarioEmpleado, dato.he_IdHorario, dato.he_diaSemana, dato.he_IdEmpleado, dato.he_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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


        public ActionResult Horarios(short? id, string nom)
        {
            ViewData["nombre"] = nom;
            try
            {
                Session["idempleado"] = id;
                myDat = "Carga horarios generales y del empleado = " + id.ToString() + " / sp_Busqueda_Horario, sp_Busqueda_HorarioEmpleado";
                if (Session[myModulo].ToString().Substring(6, 1) == "1")
                {
                    ViewBag.HO = new SelectList(db.sp_Busqueda_HorarioEmpleado(3, "", Convert.ToInt16(coP.cls_empresa), null, null, id, error).ToList(), "IdHorario", "DescripcionHorario");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_HorarioEmpleado: " + string.Join(",", 3, "", Convert.ToInt16(coP.cls_empresa), null, null, id, "-> R: " + validad.getResponse(error)));
                    ViewBag.HE = new SelectList(db.sp_Busqueda_HorarioEmpleado(2, "", Convert.ToInt16(coP.cls_empresa), null, null, id, error).ToList(), "IdHorario", "DescripcionHorario");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_HorarioEmpleado: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, null, id, "-> R: " + validad.getResponse(error)));
                }
                else
                    throw new System.InvalidOperationException("-Ingreso a opción denegada!-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Horarios de empleado", myDat });
            }
            return View();
        }


        public class contactosSearchResultModel
        {
            public IEnumerable<sp_Busqueda_Contacto_Result> contactoResults { get; set; }
        }

        public class direccionesSearchResultModel
        {
            public IEnumerable<sp_Busqueda_Direccion_Result> direccionResults { get; set; }
        }

        public class cataSearchResultModel
        {
            public IEnumerable<sp_Busqueda_DetCatalogo_Result> cataResults { get; set; }
        }

        public ActionResult Direcciones(short? id, string nom)
        {
            var direccionSearch = new direccionesSearchResultModel();
            try
            {
                myDat = "Generando listado de direcciones del empleado= " + id.ToString() + "-" + nom + " / sp_Busqueda_Direccion";
                if (Session[myModulo].ToString().Substring(7, 1) == "1")
                {
                    ViewBag.idempleado = id;
                    ViewBag.nombre = nom;
                    ViewData["Nombre"] = nom;
                    ViewBag.nuevo = Session[myModulo].ToString().Substring(31, 1);
                    ViewBag.editar = Session[myModulo].ToString().Substring(32, 1);
                    ViewBag.debaja = Session[myModulo].ToString().Substring(33, 1);
                    direccionSearch.direccionResults = dbP.sp_Busqueda_Direccion(2, "", Convert.ToInt16(coP.cls_empresa), null, "EM", id, null, null, null, null, null, null, null, error).ToList();
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Direccion: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, "EM", id, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                }
                else
                    throw new System.InvalidOperationException("-Ingreso a opción denegada!-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Direcciones de empleado", myDat });
            }
            return View(direccionSearch.direccionResults.ToList());
        }


        public JsonResult GetDepto(string id)
        {
            List<SelectListItem> depto = new List<SelectListItem>();
            try
            {
                ObjectResult resultado = dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DE"]), null, null, id, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DE"]), null, null, id, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_DetCatalogo_Result re in resultado)
                {
                    depto.Add(new SelectListItem { Text = re.Valor, Value = re.Codigo });
                }
            }
            catch (Exception ex)
            {

            }
            return Json(new SelectList(depto, "Value", "Text"));
        }

        public JsonResult GetMuni(string id)
        {
            List<SelectListItem> muni = new List<SelectListItem>();
            try
            {
                ObjectResult resultado = dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MU"]), null, null, id, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MU"]), null, null, id, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_DetCatalogo_Result re in resultado)
                {
                    muni.Add(new SelectListItem { Text = re.Valor, Value = re.Codigo });
                }
            }
            catch (Exception ex)
            {

            }

            return Json(new SelectList(muni, "Value", "Text"));
        }

        public ActionResult CreaDireccion(int? id, string aso)
        {
            Direccion dato = new Direccion();
            try
            {
                if (Session[myModulo].ToString().Substring(31, 1) == "1")
                {
                    dato.di_asociado = Convert.ToInt16(id);
                    dato.di_TipoAsociado = "EM";
                    dato.di_pais = "502";
                    dato.di_departamento = "01";
                    dato.di_municipio = "01";
                    ViewBag.idempleado = id;
                    ViewBag.nombre = aso;
                    Session["asociado"] = aso;
                    ViewBag.DI = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DI"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DI"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    ViewBag.MU = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MU"]), null, null, "01", null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MU"]), null, null, "01", null, null, "-> R: " + validad.getResponse(error)));
                    ViewBag.DE = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DE"]), null, null, "502", null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DE"]), null, null, "502", null, null, "-> R: " + validad.getResponse(error)));
                    ViewBag.PA = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["PA"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["PA"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                }
                else
                    throw new System.InvalidOperationException("No tiene acceso a esta opción", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea dirección", myDat });
            }

            return View(dato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreaDireccion([Bind(Include = "di_empresa,di_IdDireccion,di_TipoAsociado,di_asociado,di_TipoDireccion,di_Direccion,di_Direccion2,di_zona,di_municipio,di_departamento,di_pais,di_fechaing,di_fechamod,di_usuarioing,di_usuariomod,di_maquinaing,di_maquinamod,di_estado,di_timestamp")] Direccion direccion)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    codigoD.Value = direccion.di_IdDireccion;
                    direccion.di_usuarioing = Session["UserName"].ToString();
                    myDat = "Crea dirección del empleado= " + direccion.di_asociado.ToString() + " - " + direccion.di_Direccion + " / sp_ABC_Direccion";
                    int result = dbP.sp_ABC_Direccion(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigoD, direccion.di_TipoAsociado, direccion.di_asociado, direccion.di_TipoDireccion, direccion.di_Direccion, direccion.di_Direccion2, direccion.di_zona, direccion.di_municipio, direccion.di_departamento, direccion.di_pais, direccion.di_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Direccion: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigoD, direccion.di_TipoAsociado, direccion.di_asociado, direccion.di_TipoDireccion, direccion.di_Direccion, direccion.di_Direccion2, direccion.di_zona, direccion.di_municipio, direccion.di_departamento, direccion.di_pais, direccion.di_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                    if (error.Value.ToString() == "")
                    {
                        db.SaveChanges();
                        return RedirectToAction("Direcciones", new { id = direccion.di_asociado, nom = Convert.ToString(Session["asociado"]) });
                    }
                    else
                        throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
                }
                catch (Exception ex)
                {
                    return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea dirección", myDat });
                }
            }
            return View(direccion);
        }

        public ActionResult DEdit(short? id, string aso, string nom)
        {
            Direccion dato = new Direccion();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                if (Session[myModulo].ToString().Substring(32, 1) == "1")
                {
                    ViewData["idempleado"] = aso;
                    ViewData["asociado"] = nom;
                    Session["asociado"] = nom;
                    myDat = "Proceso de modificación de la dirección = " + id.ToString() + " del asociado  = " + nom + " / sp_Busqueda_Direccion";
                    ObjectResult resultado = dbP.sp_Busqueda_Direccion(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, null, null, null, null, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Direccion: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_Direccion_Result re in resultado)
                    {
                        dato = CargaDireccion(re);
                    }
                    if (dato == null)
                        throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
                    else
                    {
                        ViewBag.DI = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DI"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DI"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                        ViewBag.PA = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["PA"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["PA"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                        ViewBag.MU = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MU"]), null, null, dato.di_departamento, null, null, error).ToList(), "Codigo", "Valor");
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MU"]), null, null, dato.di_departamento, null, null, "-> R: " + validad.getResponse(error)));
                        ViewBag.DE = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DE"]), null, null, dato.di_pais, null, null, error).ToList(), "Codigo", "Valor");
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DE"]), null, null, dato.di_pais, null, null, "-> R: " + validad.getResponse(error)));
                    }
                }
                else
                   throw new System.InvalidOperationException("No tiene acceso a esta opción", new Exception(""));                
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita dirección", myDat });
            }
            return View(dato);
        }

        private Direccion CargaDireccion(sp_Busqueda_Direccion_Result parResult)
        {
            Direccion ca = new Direccion();

            ca.di_empresa = parResult.Empresa;
            ca.di_TipoAsociado = parResult.TipoAsociado;
            ca.di_IdDireccion = parResult.IdDireccion;
            ca.di_TipoDireccion = parResult.TipoDireccion;
            ca.di_asociado = parResult.Asociado;
            ca.di_pais = parResult.Pais;
            ca.di_departamento = parResult.Departamento;
            ca.di_municipio = parResult.Municipio;
            ca.di_Direccion = parResult.Direccion;
            ca.di_Direccion2 = parResult.Direccion2;
            ca.di_zona = parResult.Zona;
            ca.di_timestamp = parResult.TimeStamp;
            return ca;
        }

        // POST: DetCatalogoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DEdit([Bind(Include = "di_empresa,di_IdDireccion,di_TipoAsociado,di_asociado,di_TipoDireccion,di_Direccion,di_Direccion2,di_zona,di_municipio,di_departamento,di_pais,di_fechaing,di_fechamod,di_usuarioing,di_usuariomod,di_maquinaing,di_maquinamod,di_estado,di_timestamp")] Direccion direccion)
        {
            try
            {
                myDat = "Actualizando dirección del empleado= " + direccion.di_asociado.ToString() + " - " + direccion.di_Direccion + " / sp_ABC_Direccion";
                tsp = Convert.ToBase64String(direccion.di_timestamp as byte[]);
                codigoD.Value = direccion.di_IdDireccion;
                direccion.di_usuarioing = Session["UserName"].ToString();
                int result = dbP.sp_ABC_Direccion(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigoD, direccion.di_TipoAsociado, direccion.di_asociado, direccion.di_TipoDireccion, direccion.di_Direccion, direccion.di_Direccion2, direccion.di_zona, direccion.di_municipio, direccion.di_departamento, direccion.di_pais, direccion.di_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Direccion: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigoD, direccion.di_TipoAsociado, direccion.di_asociado, direccion.di_TipoDireccion, direccion.di_Direccion, direccion.di_Direccion2, direccion.di_zona, direccion.di_municipio, direccion.di_departamento, direccion.di_pais, direccion.di_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                    return RedirectToAction("Direcciones", new { id = direccion.di_asociado, nom = Convert.ToString(Session["asociado"]) });
                }
                else
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita dirección", myDat});
            }
        }

        public ActionResult DDelete(short? id, string aso, string nom)
        {
            Direccion dato = new Direccion();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                if (Session[myModulo].ToString().Substring(33, 1) == "1")
                {
                    ViewData["idempleado"] = aso;
                    ViewData["asociado"] = nom;
                    myDat = "Proceso de baja dirección del empleado= " + nom + " - id dirección = " + id.ToString() + " / sp_Busqueda_Direccion";
                    ObjectResult resultado = dbP.sp_Busqueda_Direccion(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, null, null, null, null, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Direccion: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_Direccion_Result re in resultado)
                    {
                        dato = CargaDireccion(re);
                    }
                    if (dato == null)
                        throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
                    else
                    {
                        ViewBag.DI = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DI"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DI"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                        ViewBag.PA = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["PA"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["PA"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                        ViewBag.MU = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MU"]), null, null, dato.di_departamento, null, null, error).ToList(), "Codigo", "Valor");
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["MU"]), null, null, dato.di_departamento, null, null, "-> R: " + validad.getResponse(error)));
                        ViewBag.DE = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DE"]), null, null, dato.di_pais, null, null, error).ToList(), "Codigo", "Valor");
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["DE"]), null, null, dato.di_pais, null, null, "-> R: " + validad.getResponse(error)));
                    }
                }
                else
                   throw new System.InvalidOperationException("No tiene acceso a esta opción", new Exception(""));
                }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Da de baja a dirección", myDat });
            }
            return View(dato);
        }

        // POST: DetCatalogoes/Delete/5
        [HttpPost, ActionName("DDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DDeleteConfirmed(short? id, string aso, string nom)
        {
            Direccion dato = new Direccion();
            try
            {
                myDat = "Proceso de baja dirección del empleado= " + nom + " - id dirección = " + id.ToString() + " / sp_Busqueda_Direccion";
                ObjectResult resultado = dbP.sp_Busqueda_Direccion(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, null, null, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Direccion: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_Direccion_Result re in resultado)
                {
                    dato = CargaDireccion(re);
                    tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                }
                dato.di_usuarioing = Session["UserName"].ToString();
                codigoD.Value = dato.di_IdDireccion;
                myDat = "Dando de baja dirección de empleado= " + nom + " - id dirección = " + id.ToString() + " / sp_ABC_Direccion";
                int result = dbP.sp_ABC_Direccion(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigoD, dato.di_TipoAsociado, dato.di_asociado, dato.di_TipoDireccion, null, null, null, null, null, null, dato.di_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Direccion: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigoD, dato.di_TipoAsociado, dato.di_asociado, dato.di_TipoDireccion, null, null, null, null, null, null, dato.di_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                    db.SaveChanges();
                else
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));

            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo,  opcion = "Da de baja dirección", myDat });
            }
            return RedirectToAction("Direcciones", new { id = dato.di_asociado, nom = Convert.ToString(Session["asociado"]) });
        }


        public ActionResult Contactos(short? id, string nom)
        {
            var contactoSearch = new contactosSearchResultModel();
            try
            {
                myDat = "Generando listado de contactos del empleado= " + nom + " / sp_Busqueda_Direccion";
                if (Session[myModulo].ToString().Substring(8, 1) == "1")
                {
                    ViewBag.idempleado = id;
                    ViewBag.nombre = nom;
                    ViewData["Nombre"] = nom;
                    ViewBag.nuevo = Session[myModulo].ToString().Substring(25, 1);
                    ViewBag.editar = Session[myModulo].ToString().Substring(26, 1);
                    ViewBag.debaja = Session[myModulo].ToString().Substring(27, 1);
                    contactoSearch.contactoResults = dbP.sp_Busqueda_Contacto(2, "", Convert.ToInt16(coP.cls_empresa), null, "EM", id, null, null, null, error).ToList();
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Contacto: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), null, "EM", id, null, null, null, "-> R: " + validad.getResponse(error)));
                }
                else
                    throw new System.InvalidOperationException("-Ingreso a opción denegada!-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Contactos", myDat });
            }
            return View(contactoSearch.contactoResults.ToList());
        }

        public ActionResult Regresa()
        {
            return RedirectToAction("Index");
        }

        public ActionResult CreaContacto(int? id, string aso)
        {
            Contacto dato = new Contacto();
            dato.cn_asociado = Convert.ToInt16(id);
            dato.cn_tipoAsociado = "EM";
            try
            {
                if (Session[myModulo].ToString().Substring(25, 1) == "1")
                {
                    ViewBag.idempleado = id;
                    ViewBag.nombre = aso;
                    Session["asociado"] = aso;
                    ViewBag.CO = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["CO"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["CO"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                }
                else
                    throw new System.InvalidOperationException("-Ingreso a opción denegada!-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea Contacto", myDat });
            }
            return View(dato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreaContacto([Bind(Include = "cn_empresa,cn_idContacto,cn_tipoAsociado,cn_asociado,cn_tipoContacto,cn_Contacto,cn_Contacto2,cn_fechaing,cn_fechamod,cn_usuarioing,cn_usuariomod,cn_maquinaing,cn_maquinamod,cn_estado,cn_timestamp")] Contacto contacto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    codigoC.Value = contacto.cn_idContacto;
                    contacto.cn_usuarioing = Session["UserName"].ToString();
                    myDat = "Actualiza contacto de empleado = " + contacto.cn_asociado + " -  contacto = " + contacto.cn_Contacto + " / sp_ABC_Contacto";
                    int result = dbP.sp_ABC_Contacto(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigoC, contacto.cn_tipoAsociado, contacto.cn_asociado, contacto.cn_tipoContacto, contacto.cn_Contacto, contacto.cn_Contacto2, contacto.cn_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Contacto: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigoC, contacto.cn_tipoAsociado, contacto.cn_asociado, contacto.cn_tipoContacto, contacto.cn_Contacto, contacto.cn_Contacto2, contacto.cn_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                    if (error.Value.ToString() == "")
                    {
                        db.SaveChanges();
                        return RedirectToAction("Contactos", new { id = contacto.cn_asociado, nom = Convert.ToString(Session["asociado"]) });
                    }
                    else
                        throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
                }
                catch (Exception ex)
                {
                    return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea contacto", myDat });
                }
            }
            return View(contacto);
        }

        public ActionResult CEdit(short? id, string aso, string nom)
        {
            Contacto dato = new Contacto();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                if (Session[myModulo].ToString().Substring(26, 1) == "1")
                {
                    ViewBag.CO = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["CO"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    ViewData["idempleado"] = aso;
                    ViewData["asociado"] = nom;
                    Session["asociado"] = nom;
                    myDat = "En proceso de Actualiza contacto de empleado = " + nom + " -  contacto = " + id.ToString() + " / sp_Busqueda_Contacto";
                    ObjectResult resultado = dbP.sp_Busqueda_Contacto(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, error);
                    foreach (sp_Busqueda_Contacto_Result re in resultado)
                    {
                        dato = CargaContactos(re);
                    }
                    if (dato == null)
                        throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
                }
                else
                    throw new System.InvalidOperationException("No tiene acceso a esta opción", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita contacto", myDat });
            }
            return View(dato);
        }

        private Contacto CargaContactos(sp_Busqueda_Contacto_Result parResult)
        {
            Contacto ca = new Contacto();

            ca.cn_empresa = parResult.Empresa;
            ca.cn_tipoAsociado = parResult.TipoAsociado;
            ca.cn_idContacto = parResult.IdContacto;
            ca.cn_tipoContacto = parResult.TipoContacto;
            ca.cn_asociado = parResult.Asociado;
            ca.cn_Contacto = parResult.Contacto;
            ca.cn_Contacto2 = parResult.Contacto2;
            ca.cn_timestamp = parResult.TimeStamp;
            return ca;
        }

        // POST: DetCatalogoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CEdit([Bind(Include = "cn_empresa,cn_idContacto,cn_tipoAsociado,cn_asociado,cn_tipoContacto,cn_Contacto,cn_Contacto2,cn_fechaing,cn_fechamod,cn_usuarioing,cn_usuariomod,cn_maquinaing,cn_maquinamod,cn_estado,cn_timestamp")] Contacto contacto)
        {
            try
            {
                tsp = Convert.ToBase64String(contacto.cn_timestamp as byte[]);
                codigoC.Value = contacto.cn_idContacto;
                contacto.cn_usuarioing = Session["UserName"].ToString();
                myDat = "Actualiza contacto de empleado = " + contacto.cn_asociado + " -  contacto = " + contacto.cn_Contacto + " / sp_ABC_Contacto"; 
                int result = dbP.sp_ABC_Contacto(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigoC, contacto.cn_tipoAsociado, contacto.cn_asociado, contacto.cn_tipoContacto, contacto.cn_Contacto, contacto.cn_Contacto2, contacto.cn_usuarioing, tsp, error);
                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                    return RedirectToAction("Contactos", new { id = contacto.cn_asociado, nom = Convert.ToString(Session["asociado"]) });
                }
                else
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita contacto", myDat });
            }
        }

        public ActionResult CDelete(short? id, string aso, string nom)
        {
            Contacto dato = new Contacto();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                if (Session[myModulo].ToString().Substring(27, 1) == "1")
                {
                    ViewBag.idcon = id;
                    ViewData["idempleado"] = aso;
                    ViewData["asociado"] = nom;
                    myDat = "Proceso de baja de contacto del empleado = " + nom + " -  contacto = " + id.ToString() + " / sp_Busqueda_Contacto";
                    ObjectResult resultado = dbP.sp_Busqueda_Contacto(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, error);
                    foreach (sp_Busqueda_Contacto_Result re in resultado)
                    {
                        dato = CargaContactos(re);
                    }
                    if (dato == null)
                        throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
                }
                else
                    throw new System.InvalidOperationException("No tiene acceso a esta opción", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Da de baja contacto", myDat });
            }
            return View(dato);
        }

        // POST: DetCatalogoes/Delete/5
        [HttpPost, ActionName("CDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult CDeleteConfirmed(short? id, string aso, string nom)
        {
            Contacto dato = new Contacto();
            try
            {
                myDat = "Proceso de baja de contacto del empleado = " + nom + " -  contacto = " + id.ToString() + " / sp_Busqueda_Contacto";
                ObjectResult resultado = dbP.sp_Busqueda_Contacto(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, null, null, null, error);

                foreach (sp_Busqueda_Contacto_Result re in resultado)
                {
                    dato = CargaContactos(re);
                    tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                }
                myDat = "Actualiza contacto de empleado = " + nom + " -  contacto = " + dato.cn_Contacto + " / sp_ABC_Contacto";
                codigoC.Value = dato.cn_idContacto;
                int result = dbP.sp_ABC_Contacto(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigoC, dato.cn_tipoAsociado, dato.cn_asociado, dato.cn_tipoContacto, dato.cn_Contacto, dato.cn_Contacto2, dato.cn_usuarioing, tsp, error);
                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                }
                else
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Da de baja contacto", myDat });
            }
            return RedirectToAction("Contactos", new { id = dato.cn_asociado, nom = Convert.ToString(Session["asociado"]) });
        }

        // GET: Empleados/Create
        public ActionResult Create()
        {
            try
            {
                if (Session[myModulo].ToString().Substring(0, 1) == "0")                
                    throw new System.InvalidOperationException("No tiene acceso a crear nuevo registros");
                else
                {
                    myDat = "Proceso de crear empleado / sp_Busqueda_DetCatalogo";
                    ViewBag.TE = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["TE"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    ViewBag.TI = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["TI"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    ViewBag.EC = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["EC"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    ViewBag.GE = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["GE"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    ViewBag.OP = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["OP"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Creando empleado", myDat });

            }
            return View();
        }

        // POST: Empleados/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "em_empresa,em_IdEmpleado,em_tipoEmpleado,em_tipoIdentifica,em_Identifica,em_Nit,em_Nombre1,em_Nombre2,em_Nombre3,em_Apellido1,em_Apellido2,em_ApeCasada,em_nombrelargo,em_Alias,em_genero,em_FechaNac,em_ocupacion,em_estadoCivil,em_estEmpleado,em_FechaIngreso,em_FechaSalida,em_SueldoBase,em_Bonificacion,em_fechaing,em_fechamod,em_usuarioing,em_usuariomod,em_maquinaing,em_maquinamod,em_estado,em_timestamp")] Empleado empleado)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    myDat = "Crea empleado = " + empleado.em_Nombre1  + empleado.em_Apellido1 +  empleado.em_Identifica + "  / sp_ABC_Empleado";
                    empleado.em_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_Empleado(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo, empleado.em_tipoEmpleado, empleado.em_tipoIdentifica, empleado.em_Identifica,
                                                   empleado.em_Nit, empleado.em_Nombre1, empleado.em_Nombre2, empleado.em_Nombre3, empleado.em_Apellido1, empleado.em_Apellido2, empleado.em_ApeCasada,
                                                    empleado.em_Alias, empleado.em_genero, empleado.em_FechaNac, empleado.em_ocupacion, empleado.em_estadoCivil, null, empleado.em_FechaIngreso,
                                                    empleado.em_FechaSalida, empleado.em_SueldoBase, empleado.em_Bonificacion, empleado.em_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Empleado: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo.Value, empleado.em_tipoEmpleado, empleado.em_tipoIdentifica, empleado.em_Identifica,
                                                   empleado.em_Nit, empleado.em_Nombre1, empleado.em_Nombre2, empleado.em_Nombre3, empleado.em_Apellido1, empleado.em_Apellido2, empleado.em_ApeCasada,
                                                    empleado.em_Alias, empleado.em_genero, empleado.em_FechaNac, empleado.em_ocupacion, empleado.em_estadoCivil, null, empleado.em_FechaIngreso,
                                                    empleado.em_FechaSalida, empleado.em_SueldoBase, empleado.em_Bonificacion, empleado.em_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Creando empleado", myDat });
            }

            return View(empleado);
        }

        // GET: Empleados/Edit/5
        public ActionResult Edit(short? id)
        {
            Empleado dato = new Empleado();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                if (Session[myModulo].ToString().Substring(2, 1) == "1")
                {
                    myDat = "Proceso de actualización del empleado = " + id.ToString() + "  / Empleadoes.Where(a => a.em_IdEmpleado == id).First()";
                    ViewBag.TE = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["TE"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["TE"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    ViewBag.TI = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["TI"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["TI"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    ViewBag.EC = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["EC"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["EC"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    ViewBag.GE = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["GE"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["GE"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    ViewBag.OP = new SelectList(dbC.sp_Busqueda_DetCatalogo(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["OP"]), null, null, null, null, null, error).ToList(), "Codigo", "Valor");
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_DetCatalogo: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(Session["OP"]), null, null, null, null, null, "-> R: " + validad.getResponse(error)));
                    dato = db.Empleadoes.Where(a => a.em_IdEmpleado == id).First();
                    if (dato == null)
                        throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
                }
                else
                    throw new System.InvalidOperationException("-No tiene acceso a esta opción-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Modifica empleado", myDat });
            }
            return View(dato);
        }

        // POST: Empleados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "em_empresa,em_IdEmpleado,em_tipoEmpleado,em_tipoIdentifica,em_Identifica,em_Nit,em_Nombre1,em_Nombre2,em_Nombre3,em_Apellido1,em_Apellido2,em_ApeCasada,em_nombrelargo,em_Alias,em_genero,em_FechaNac,em_ocupacion,em_estadoCivil,em_estEmpleado,em_FechaIngreso,em_FechaSalida,em_SueldoBase,em_Bonificacion,em_fechaing,em_fechamod,em_usuarioing,em_usuariomod,em_maquinaing,em_maquinamod,em_estado,em_timestamp")] Empleado empleado)
        {
            try
            {
                myDat = "Actualización de datos del empleado = " + empleado.em_Nombre1 + empleado.em_Apellido1 + empleado.em_Identifica + "  / sp_ABC_Empleado";
                tsp = Convert.ToBase64String(empleado.em_timestamp as byte[]);
                codigo.Value = empleado.em_IdEmpleado;
                empleado.em_usuarioing = Session["UserName"].ToString();
                empleado.em_estEmpleado = empleado.em_estado.ToString();
                int result = db.sp_ABC_Empleado(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo, empleado.em_tipoEmpleado, empleado.em_tipoIdentifica, empleado.em_Identifica,
                                                   empleado.em_Nit, empleado.em_Nombre1, empleado.em_Nombre2, empleado.em_Nombre3, empleado.em_Apellido1, empleado.em_Apellido2, empleado.em_ApeCasada,
                                                    empleado.em_Alias, empleado.em_genero, empleado.em_FechaNac, empleado.em_ocupacion, empleado.em_estadoCivil,empleado.em_estEmpleado, empleado.em_FechaIngreso,
                                                    empleado.em_FechaSalida, empleado.em_SueldoBase, empleado.em_Bonificacion, empleado.em_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Empleado: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo.Value, empleado.em_tipoEmpleado, empleado.em_tipoIdentifica, empleado.em_Identifica,
                                                   empleado.em_Nit, empleado.em_Nombre1, empleado.em_Nombre2, empleado.em_Nombre3, empleado.em_Apellido1, empleado.em_Apellido2, empleado.em_ApeCasada,
                                                    empleado.em_Alias, empleado.em_genero, empleado.em_FechaNac, empleado.em_ocupacion, empleado.em_estadoCivil, null, empleado.em_FechaIngreso,
                                                    empleado.em_FechaSalida, empleado.em_SueldoBase, empleado.em_Bonificacion, empleado.em_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                    db.SaveChanges();
                else
                    throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Modifica empleado", myDat});
            }
            return RedirectToAction("Index");
        }

        // GET: Empleados/Delete/5
        public ActionResult Delete(short? id)
        {
            Empleado dato = new Empleado();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                if (Session[myModulo].ToString().Substring(3, 1) == "1")
                {
                    myDat = "Proceso de baja del empleado = " + id.ToString() + "  / Empleadoes.Where(a => a.em_IdEmpleado == id).First()";
                    dato = db.Empleadoes.Where(a => a.em_IdEmpleado == id).First();
                    if (dato == null)
                        throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
                }
                else
                    throw new System.InvalidOperationException("-No tiene acceso a esta opción-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Da de baja a empleado"});
            }
            return View(dato);
        }

        // POST: Empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            Empleado dato = new Empleado();
            try
            {
                dato = db.Empleadoes.Where(a => a.em_IdEmpleado == id).First();
                tsp = Convert.ToBase64String(dato.em_timestamp as byte[]);
                dato.em_usuarioing = Session["UserName"].ToString();
                codigo.Value = id;
                myDat = "Baja del empleado = " + dato.em_Nombre1 + dato.em_Apellido1 + dato.em_Identifica + "  / sp_ABC_Empleado";
                int result = db.sp_ABC_Empleado(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, dato.em_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Empleado: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo.Value, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, dato.em_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Da de baja a empleado", myDat});
            }
            return RedirectToAction("Index");
        }

        public ActionResult Report(string id)
        {
            myDat = "Generacíon de reporte tipo = " + id + " / Empleados.rdlc";
            try
            {
                if (Session[myModulo].ToString().Substring(19, 1) == "1")
                {
                    LocalReport lr = new LocalReport();
                    string path = Path.Combine(Server.MapPath("~/Reports"), "Empleados.rdlc");
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        return View("Index");
                    }
                    List<Empleado> cm = new List<Empleado>();
                    using (SSS_PLANILLAEntities dc = new SSS_PLANILLAEntities())
                    {
                        cm = dc.Empleadoes.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("Empleados", cm);
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
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Generando reporte", myDat });
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