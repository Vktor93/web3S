using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity.Core.Objects;
using WebGrupo3S.Models;
using Microsoft.Reporting.WebForms;
using System.IO;
using WebGrupo3S.Helpers;
using Newtonsoft.Json;

namespace WebGrupo3S.Views
{
    public class perfilsController : Controller
    {
        private SSS_PERSONASEntities db = new SSS_PERSONASEntities();
        private string myModulo = "Perfiles";
        private string mycatalogo = "Catalogo de perfiles";
        private string myDat = "";
        public ConstantesP coP = new ConstantesP();
        public string siPermisos = "";
        public String tsp = "";
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codigo = new ObjectParameter("pf_codPerfil", typeof(int));
        ObjectParameter codigoP = new ObjectParameter("pe_IdPermiso", typeof(int));

        public class PerfilSearchResultModel
        {
            public IEnumerable<sp_Busqueda_Perfil_Result> perfilResults { get; set; }
        }

  
        // GET: perfils
        public ActionResult Index()
        {
            validad ac = new validad();
            var PerfilSearch = new PerfilSearchResultModel();
            try
            {
                ac.seteAccesos(mycatalogo, Session["perfil"].ToString());
                Session[myModulo] = ac.acc;
                ViewData["nombre"] = Session["UserName"];
                if (Session["UserId"].ToString() != "")
                {
                    if (Session[myModulo].ToString().Substring(0, 1) == "1")
                    {
                        ViewBag.nuevo = Session[myModulo].ToString().Substring(1, 1);
                        ViewBag.editar = Session[myModulo].ToString().Substring(2, 1);
                        ViewBag.debaja = Session[myModulo].ToString().Substring(3, 1);
                        ViewBag.permisos = Session[myModulo].ToString().Substring(9, 1);
                        ViewBag.reporte = Session[myModulo].ToString().Substring(19, 1);
                        PerfilSearch.perfilResults = db.sp_Busqueda_Perfil(1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, error).ToList();
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Perfil: " + string.Join(",", 1, "", Convert.ToInt16(coP.cls_empresa), null, null, null, "-> R: " + validad.getResponse(error)));
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

            return View(PerfilSearch.perfilResults.ToList());
        }

        public class PermisosSearchResultModel
        {
            public IEnumerable<sp_Busqueda_Permiso_Result> permisosResults { get; set; }
        }


        public ActionResult Opciontree(int per, string nom)
        {
            try
            {
                if (Session[myModulo].ToString().Substring(9, 1) == "1")
                {
                    Session["per"] = per;
                    ViewData["Perfil"] = nom;
                    GetHierarchy();
                    return View();
                }
                else
                    throw new System.InvalidOperationException("-No tiene acceso a modificar permisos!-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Busqueda", myDat });
            }

        }


        public JsonResult GetHierarchy()
        {
            List<opcion> hdList;
            var PermisoSearch = new PermisosSearchResultModel();
            List<TreeOpcion> records;
            int per = Convert.ToInt32(Session["per"].ToString());

            using (SSS_PERSONASEntities context = new SSS_PERSONASEntities())
            {
                PermisoSearch.permisosResults = db.sp_Busqueda_Permiso(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), per, null, null, error).ToList();
                WriteLogMessages.WriteFile(Session["LogonName"], "Validación" + "-> ejecutando db.sp_Busqueda_Permiso: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), per, null, null, "-> R: " + validad.getResponse(error)));
                hdList = context.opcion.Where(p => p.op_PadreOpcion > 0).ToList();

                records = hdList.Where(l => l.op_PadreOpcion == 1 && l.op_Estado == 1)
                    .Select(l => new TreeOpcion
                    {
                        Id = l.op_CodigoOpcion,
                        text = l.op_NombreOpcion,
                        @checked = traePermiso(l.op_CodigoOpcion, hdList, PermisoSearch.permisosResults),
                        perentId = 1,
                        children = GetChildren(hdList, l.op_CodigoOpcion, PermisoSearch.permisosResults)
                    }).ToList();
            }
            siPermisos = "";
            foreach (sp_Busqueda_Permiso_Result re in PermisoSearch.permisosResults)
            {
                if (siPermisos == "")
                    siPermisos = re.CodigoOpcion.ToString();
                else
                    siPermisos = siPermisos + "," + re.CodigoOpcion.ToString();
            }
            Session["siper"] = siPermisos;
            return this.Json(records, JsonRequestBehavior.AllowGet);
        }


        private List<TreeOpcion> GetChildren(List<opcion> hdList, int PadreOpcion, IEnumerable<sp_Busqueda_Permiso_Result> lpe)
        {
            return hdList.Where(l => l.op_PadreOpcion == PadreOpcion && l.op_Estado == 1)
                .Select(l => new TreeOpcion
                {
                    Id = l.op_CodigoOpcion,
                    text = l.op_NombreOpcion,
                    @checked = traePermiso(l.op_CodigoOpcion, hdList, lpe),
                    perentId = PadreOpcion,
                    children = GetChildren(hdList, l.op_CodigoOpcion, lpe)
                }).ToList();
        }

        private bool traePermiso(int opc, List<opcion> hdList, IEnumerable<sp_Busqueda_Permiso_Result> lpe)
        {
            Boolean blnResult = false;
            var PermisoSearch = new PermisosSearchResultModel();
            try
            {
                if (lpe.Where(p => p.CodigoOpcion == opc).Count() > 0)
                {
                    if (hdList.Where(l => l.op_PadreOpcion == opc && l.op_Estado == 1).Count() == 0)
                    {
                        if (siPermisos == "")
                            siPermisos = opc.ToString();
                        else
                            siPermisos = siPermisos + "," + opc.ToString();
                        blnResult = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return blnResult;
        }

        public ActionResult GetOpciones(string datos)
        {
            opcion op = new opcion();
            permiso pe = new permiso();
            var registros = 0;
            List<permiso> lpe = new List<permiso>();
            int padre = 0;
            string[] opci = datos.Split(',');
            try
            {
                siPermisos = Session["siper"].ToString();
                int per = Convert.ToInt32(Session["per"].ToString());
                myDat = "Actualiza permisos del perfil= " + per.ToString() + " / sp_ABC_Permiso";
                foreach (string dat in opci)
                {
                    if (siPermisos.IndexOf(dat) == -1)
                    {
                        /*
                        ObjectResult resultado = db.sp_Busqueda_Permiso(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), per, Convert.ToInt32(dat), null, error);
                        foreach (sp_Busqueda_Permiso_Result re in resultado)
                        {
                            tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                        }
                        */
                        var ts_array = GenerateRandomBytes(8);
                        tsp = Convert.ToBase64String(ts_array);
                        registros = db.sp_ABC_Permiso(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), Convert.ToInt16(coP.cls_sucursal), codigoP, per, Convert.ToInt32(dat), Session["UserName"].ToString(), tsp, error);
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "->ejecutando sp_ABC_Permiso: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), Convert.ToInt16(coP.cls_sucursal), codigoP.Value.ToString(), per, Convert.ToInt32(dat), Session["UserName"].ToString(), tsp, "-> R: " + validad.getResponse(error)));
                    }
                }
                string[] nopci = siPermisos.Split(',');
                foreach (string dat in nopci)
                {
                    if (datos.IndexOf(dat) == -1)
                    {
                        ObjectResult resultado = db.sp_Busqueda_Permiso(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), per, Convert.ToInt32(dat), null, error);
                        foreach (sp_Busqueda_Permiso_Result re in resultado)
                        {
                            tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                        }
                        registros = db.sp_ABC_Permiso(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), Convert.ToInt16(coP.cls_sucursal), codigoP, per, Convert.ToInt32(dat), Session["UserName"].ToString(), tsp, error);
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "->ejecutando sp_ABC_Permiso: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), Convert.ToInt16(coP.cls_sucursal), codigoP.Value.ToString(), per, Convert.ToInt32(dat), Session["UserName"].ToString(), tsp, "-> R: " + validad.getResponse(error)));
                    }
                }
                db.SaveChanges();
                /*
                foreach (string dat in opci)
                {
                    int valor = Convert.ToInt32(dat);
                    pe = new permiso();
                    op = db.opcion.Where(a => a.op_CodigoOpcion == valor && a.op_Estado == 1).First();
                    if (!db.permiso.Where(a => a.pe_CodPerfil == per && a.pe_CodigoOpcion == valor && a.pe_estado == 1).Any())
                    {
                        pe.pe_CodPerfil = per;
                        pe.pe_CodigoOpcion = valor;
                        pe.pe_UsuarioIng = Session["userid"].ToString();
                        pe.pe_estado = 1;
                        codigoP = new ObjectParameter("pe_IdPermiso", typeof(int));
                        tsp = "";
                        int result = db.sp_ABC_Permiso(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), Convert.ToInt16(coP.cls_sucursal), codigoP, pe.pe_CodPerfil, pe.pe_CodigoOpcion, pe.pe_UsuarioIng, tsp, error);
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Permiso: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), Convert.ToInt16(coP.cls_sucursal), codigoP.Value.ToString(), pe.pe_CodPerfil, pe.pe_CodigoOpcion, pe.pe_UsuarioIng, tsp, "-> R: " + validad.getResponse(error)));

                        if (error.Value.ToString() == "")
                        {
                            db.SaveChanges();
                        }
                    }
                    if (op != null)
                    {
                        padre = Convert.ToInt16(op.op_PadreOpcion);
                        if (padre > 1)
                        {
                            lpe = db.permiso.Where(a => a.pe_CodPerfil == per && a.pe_CodigoOpcion == padre).ToList();
                            if (lpe.Count == 0)
                            {
                                pe.pe_CodPerfil = per;
                                pe.pe_CodigoOpcion = padre;
                                pe.pe_UsuarioIng = Session["userid"].ToString();
                                pe.pe_estado = 1;
                                int result = db.sp_ABC_Permiso(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), Convert.ToInt16(coP.cls_sucursal), codigoP, pe.pe_CodPerfil, pe.pe_CodigoOpcion, pe.pe_UsuarioIng, tsp, error);                               
                                if (error.Value.ToString() == "")
                                {
                                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Permiso  para insertar padre nuevo: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), Convert.ToInt16(coP.cls_sucursal), codigoP.Value.ToString(), pe.pe_CodPerfil, pe.pe_CodigoOpcion, pe.pe_UsuarioIng, tsp, "-> R: " + validad.getResponse(error)));
                                }
                            }
                            else
                            {
                                if (db.permiso.Where(a => a.pe_CodPerfil == per && a.pe_CodigoOpcion == padre && a.pe_estado == 0).Any())
                                {
                                    ObjectResult resultado = db.sp_Busqueda_Permiso(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), per, padre, null, error);
                                    foreach (sp_Busqueda_Permiso_Result re in resultado)
                                    {
                                        tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                                    }
                                    registros = db.sp_ABC_Permiso(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), Convert.ToInt16(coP.cls_sucursal), codigoP, per, padre, Session["UserName"].ToString(), tsp, error);
                                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "->ejecutando sp_ABC_Permiso para activar padre: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), Convert.ToInt16(coP.cls_sucursal), codigoP.Value.ToString(), per, padre, Session["UserName"].ToString(), tsp, "-> R: " + validad.getResponse(error)));
                                }
                            }
                        }
                    }
                }
                */
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { error = ex.Message, inner = (ex.InnerException != null) ? ex.InnerException.Message : "", modulo = myModulo, opcion = "Actualizando permisos de perfil", myDat });
            }
            return RedirectToAction("Index");
        }

        private byte[] GenerateRandomBytes(int length)
        {
            // Create a buffer
            byte[] randBytes;

            if (length >= 1)
            {
                randBytes = new byte[length];
            }
            else
            {
                randBytes = new byte[1];
            }

            // Create a new RNGCryptoServiceProvider.
            System.Security.Cryptography.RNGCryptoServiceProvider rand =
                 new System.Security.Cryptography.RNGCryptoServiceProvider();

            // Fill the buffer with random bytes.
            rand.GetBytes(randBytes);

            // return the bytes.
            return randBytes;
        }

        public class OpcionSearchResultModel
        {
            public IEnumerable<sp_Busqueda_Opcion_Result> opcionResults { get; set; }
        }


        // GET: perfils/Create
        public ActionResult Create()
        {
            try
            {
                myDat = "Agrega nuevo perfil";
                if (Session[myModulo].ToString().Substring(1, 1) == "0")
                   throw new System.InvalidOperationException("-No tiene acceso a crear nuevo registros-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea", myDat });
            }
            return View();
        }

        // POST: perfils/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "pf_empresa,pf_codPerfil,pf_nomPerfil,pf_descPerfil,pf_fechaing,pf_fechamod,pf_usuarioing,pf_usuariomod,pf_maquinaing,pf_maquinamod,pf_estado,pf_timestamp")] perfil perfil)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    myDat = "Crea perfil / sp_ABC_Perfil";
                    perfil.pf_fechaing = System.DateTime.Now;
                    perfil.pf_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_Perfil(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo, perfil.pf_nomPerfil, perfil.pf_descPerfil,perfil.pf_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo+"-> ejecutando sp_ABC_Perfil: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo.Value, perfil.pf_nomPerfil, perfil.pf_descPerfil,
                        perfil.pf_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                    return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea", myDat });
                }
            }
            return View(perfil);
        }

        // GET: perfils/Edit/5
        public ActionResult Edit(int? id)
        {
            perfil Perfil = new perfil();
            if (id == null)
            {
                throw new System.InvalidOperationException("-Error al obtener registro-", new Exception(""));
            }
            try
            {
                myDat = "Edita perfil: " + id.ToString() + " / sp_Busqueda_Perfil";
                if (Session[myModulo].ToString().Substring(2, 1) == "1")
                {
                    ObjectResult resultado = db.sp_Busqueda_Perfil(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo+"-> ejecutando sp_Busqueda_Perfil: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_Perfil_Result re in resultado)
                    {
                        Perfil = CargaPerfil(re);
                    }
                    if (Perfil == null)
                        throw new System.InvalidOperationException("-Error al obtener registro-", new Exception(""));
                }
                else                   
                   throw new System.InvalidOperationException("-No tiene acceso a esta opción-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
            }
            return View(Perfil);
        }


        private perfil CargaPerfil(sp_Busqueda_Perfil_Result parResult)
        {
            perfil pf = new perfil();

            pf.pf_empresa = parResult.Empresa;
            pf.pf_codPerfil = parResult.CodigoPerfil;
            pf.pf_descPerfil = parResult.DescripcionPerfil;
            pf.pf_nomPerfil = parResult.NombrePerfil;
            pf.pf_timestamp = parResult.TimeStamp;
            return pf;
        }

        // POST: perfils/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "pf_empresa,pf_codPerfil,pf_nomPerfil,pf_descPerfil,pf_fechaing,pf_fechamod,pf_usuarioing,pf_usuariomod,pf_maquinaing,pf_maquinamod,pf_estado,pf_timestamp")] perfil perfil)
        {
            try
            {
                myDat = "Modifica perfil / sp_ABC_Perfil";
                tsp = Convert.ToBase64String(perfil.pf_timestamp as byte[]);
                codigo.Value = perfil.pf_codPerfil;
                perfil.pf_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_Perfil(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo, perfil.pf_nomPerfil, perfil.pf_descPerfil, perfil.pf_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo+"-> ejecutando sp_ABC_Perfil: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo.Value, perfil.pf_nomPerfil, perfil.pf_descPerfil, perfil.pf_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
            }

            return RedirectToAction("Index");
        }

        // GET: perfils/Delete/5
        public ActionResult Delete(int? id)
        {
            perfil Perfil = new perfil();
            if (id == null)
            {
                return View("Error");
            }
            try
            {
                myDat = "Dar de baja perfil: " + id.ToString() + " / sp_Busqueda_Perfil";
                if (Session[myModulo].ToString().Substring(3, 1) == "1")
                {
                    ObjectResult resultado = db.sp_Busqueda_Perfil(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo+"-> ejecutando sp_Busqueda_Perfil: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_Perfil_Result re in resultado)
                    {
                        Perfil = CargaPerfil(re);
                    }
                }
                else
                    throw new System.InvalidOperationException("-No tiene permitido dar de baja el registro-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Debaja", myDat });
            }
            return View(Perfil);
        }

        // POST: perfils/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            perfil Perfil = new perfil();
            try
            {
                myDat = "Confirma dar de baja perfil: " + id.ToString() + " / sp_Busqueda_Perfil";
                ObjectResult resultado = db.sp_Busqueda_Perfil(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo+"-> ejecutando sp_Busqueda_Perfil: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_Perfil_Result re in resultado)
                {
                    Perfil = CargaPerfil(re);
                    tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                }

                codigo.Value = Perfil.pf_codPerfil;
                Perfil.pf_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_Perfil(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo, Perfil.pf_nomPerfil, Perfil.pf_descPerfil, Perfil.pf_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo+"-> ejecutando sp_ABC_Perfil: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo.Value, Perfil.pf_nomPerfil, Perfil.pf_descPerfil, Perfil.pf_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
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
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Debaja", myDat });
            }
            return RedirectToAction("Index");
        }

        public ActionResult Report(string id)
        {
            try
            {
                if (Session[myModulo].ToString().Substring(19, 1) == "1")
                {
                    LocalReport lr = new LocalReport();
                    string path = Path.Combine(Server.MapPath("~/Reports"), "Perfiles.rdlc");
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        return View("Index");
                    }
                    List<perfil> cm = new List<perfil>();
                    using (SSS_PERSONASEntities dc = new SSS_PERSONASEntities())
                    {
                        cm = dc.perfil.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DPerfil", cm);
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

 
                    myDat = "Reporte de perfiles";
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


        //POST ajax, crear perfil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SavePerfil([Bind(Include = "pf_empresa,pf_codPerfil,pf_nomPerfil,pf_descPerfil,pf_fechaing,pf_fechamod,pf_usuarioing,pf_usuariomod,pf_maquinaing,pf_maquinamod,pf_estado,pf_timestamp")] perfil perfil) {
            if (ModelState.IsValid)
            {
                try
                {
                    myDat = "Crea perfil / sp_ABC_Perfil";
                    perfil.pf_fechaing = System.DateTime.Now;
                    perfil.pf_usuarioing = Session["UserName"].ToString();
                    int result = db.sp_ABC_Perfil(Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo, perfil.pf_nomPerfil, perfil.pf_descPerfil, perfil.pf_usuarioing, tsp, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Perfil: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('A'), codigo.Value, perfil.pf_nomPerfil, perfil.pf_descPerfil,
                        perfil.pf_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                    if (error.Value.ToString() == "")
                    {
                        db.SaveChanges();

                        var data = new {
                            status = 200,
                            message = "success",
                        };
                                                
                        return Json(JsonConvert.SerializeObject(data),JsonRequestBehavior.AllowGet);
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

            var datas = new
            {
                status = 400,
                message = "Datos no válidos"              
            };

            return Json(datas, JsonRequestBehavior.AllowGet);

        }

        //POST ajax, vista para crear perfil
        [HttpPost]
        public JsonResult perfilView()
        {

            string viewContent = ConvertView("perfilView");
                        
            return Json( new { PartialView = viewContent });
        }

        //funcion para renderizar y retornar la vista crear perfil
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

        //POST ajax, vista editar perfil
        [HttpPost]        
        public JsonResult updateView(int? id)
        {

            perfil Perfil = new perfil();
            if (id == null)
            {
                var data = new
                {
                    status = 500,
                    message = "id nulo"                    
                };

                return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
                throw new System.InvalidOperationException("-Error al obtener registro-", new Exception(""));
            }
            try
            {
                myDat = "Edita perfil: " + id.ToString() + " / sp_Busqueda_Perfil";
                if (Session[myModulo].ToString().Substring(2, 1) == "1")
                {
                    ObjectResult resultado = db.sp_Busqueda_Perfil(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Perfil: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_Perfil_Result re in resultado)
                    {
                        Perfil = CargaPerfil(re);
                    }
                    if (Perfil == null)
                        
                        throw new System.InvalidOperationException("-Error al obtener registro-", new Exception(""));
                }
                else
                    throw new System.InvalidOperationException("-No tiene acceso a esta opción-", new Exception(""));
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
                //return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
            }
            //return View(Perfil);
            string viewContent = ConvertViewUpdate("updateView", Perfil);
            return Json(new { PartialView = viewContent });
        }

        //FUNCION PARA RETORNAR LA VISTA DE ACTUALIZAR PERFIL RENDERIZADA
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

        //FUNCION PARA ACTUALIZAR LOS DATOS VIA AJAX
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult updatePerfil([Bind(Include = "pf_empresa,pf_codPerfil,pf_nomPerfil,pf_descPerfil,pf_fechaing,pf_fechamod,pf_usuarioing,pf_usuariomod,pf_maquinaing,pf_maquinamod,pf_estado,pf_timestamp")] perfil perfil)
        {
            try
            {
                myDat = "Modifica perfil / sp_ABC_Perfil";
                tsp = Convert.ToBase64String(perfil.pf_timestamp as byte[]);
                codigo.Value = perfil.pf_codPerfil;
                perfil.pf_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_Perfil(Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo, perfil.pf_nomPerfil, perfil.pf_descPerfil, perfil.pf_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Perfil: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('C'), codigo.Value, perfil.pf_nomPerfil, perfil.pf_descPerfil, perfil.pf_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                    String Noregistro = codigo.Value.ToString();

                    var data = new
                    {
                        status = 200,
                        message = "success",
                        registro = Noregistro,
                    };

                    return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);

                }
                else {
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
                //return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Edita", myDat });
            }          

        }


        //FUNCION PARA CARGAR LA VISTA DE DAR DE BAJA PERFIL

        [HttpPost]
        public JsonResult deleteView(int? id)
        {
            perfil Perfil = new perfil();
            if (id == null)
            {
                var data = new
                {
                    status = 400,
                    message = "Datos no válidos, ver consola"                    
                };
                return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
                //return View("Error");
            }
            try
            {
                myDat = "Dar de baja perfil: " + id.ToString() + " / sp_Busqueda_Perfil";
                if (Session[myModulo].ToString().Substring(3, 1) == "1")
                {
                    ObjectResult resultado = db.sp_Busqueda_Perfil(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, error);
                    WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Perfil: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, "-> R: " + validad.getResponse(error)));
                    foreach (sp_Busqueda_Perfil_Result re in resultado)
                    {
                        Perfil = CargaPerfil(re);
                    }
                }
                else {
                    var data = new
                    {
                        status = 305,
                        message = "permisos insuficientes"
                    };

                    return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
                    throw new System.InvalidOperationException("-No tiene permitido dar de baja el registro-", new Exception(""));
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
                //return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Debaja", myDat });
            }
            //return View(Perfil);

            string viewContent = ConvertViewDelete("deleteView", Perfil);
            return Json(new { PartialView = viewContent });
        }

        //FUNCION PARA RENDERIZAR LA VISTA PARA DAR DE BAJA EL PERFIL
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
        [ValidateAntiForgeryToken]
        public JsonResult deletePerfil(int id)
        {
            perfil Perfil = new perfil();
            try
            {
                myDat = "Confirma dar de baja perfil: " + id.ToString() + " / sp_Busqueda_Perfil";
                ObjectResult resultado = db.sp_Busqueda_Perfil(2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Busqueda_Perfil: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), id, null, null, "-> R: " + validad.getResponse(error)));
                foreach (sp_Busqueda_Perfil_Result re in resultado)
                {
                    Perfil = CargaPerfil(re);
                    tsp = Convert.ToBase64String(re.TimeStamp as byte[]);
                }

                codigo.Value = Perfil.pf_codPerfil;
                Perfil.pf_usuarioing = Session["UserName"].ToString();
                int result = db.sp_ABC_Perfil(Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo, Perfil.pf_nomPerfil, Perfil.pf_descPerfil, Perfil.pf_usuarioing, tsp, error);
                WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_ABC_Perfil: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), Convert.ToString('B'), codigo.Value, Perfil.pf_nomPerfil, Perfil.pf_descPerfil, Perfil.pf_usuarioing, tsp, "-> R: " + validad.getResponse(error)));
                if (error.Value.ToString() == "")
                {
                    db.SaveChanges();
                    String Noregistro = codigo.Value.ToString();
                    var data = new
                    {
                        status = 200,
                        message = "success",
                        registro = Noregistro,
                    };

                    return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
                }
                else {
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
                //return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Debaja", myDat });
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
