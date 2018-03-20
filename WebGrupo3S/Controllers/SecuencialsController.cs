using System;
using System.Linq;
using System.Web.Mvc;
using WebGrupo3S.Models;
using System.Data.Entity.Core.Objects;
using WebGrupo3S.Helpers;

namespace WebGrupo3S.Views
{
    public class SecuencialsController : Controller
    {
        private SSS_COMPLEMENTOSEntities db = new SSS_COMPLEMENTOSEntities();
        ObjectParameter error = new ObjectParameter("error", typeof(String));
        ObjectParameter codusuario = new ObjectParameter("sc_codTabla", typeof(int));
        private string myModulo = "Secuencial";
        private string mycatalogo = "Secuencial";
        public ConstantesP coP = new ConstantesP();
        public PermisosP perP = new PermisosP();
        public String myDat = "";
        public String tsp = "";


        // GET: Secuencials
        public ActionResult Index()
        {
            validad ac = new validad();
            try
            {
                ac.seteAccesos(mycatalogo, Session["perfil"].ToString());
                Session[myModulo] = ac.acc;
                myDat = "Generando lista de secuencias";
                if (Session["UserId"].ToString() != "")
                {
                    if (Session[myModulo].ToString().Substring(0, 1) == "1")
                    {
                        ViewBag.nuevo = Session[myModulo].ToString().Substring(1, 1);
                        return View(db.Secuencials.ToList());
                    }
                    else
                        throw new System.InvalidOperationException("-Ingreso a opción denegado!-", new Exception(""));
                }
                else
                    throw new System.InvalidOperationException("-Ingrese al Sistema para poder Ingresar a esta Opción-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Listado de secuencias", myDat });
            }         
            
        }

        // GET: Secuencials/Create
        public ActionResult Create()
        {
            try
            {
                myDat = "Agrega nuevo secuencia de datos";
                if (Session[myModulo].ToString().Substring(1, 1) == "0")
                    throw new System.InvalidOperationException("-No tiene acceso a crear nuevo registros-", new Exception(""));
            }
            catch (Exception ex)
            {
                return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Crea", myDat });
            }
            return View();
        }

        // POST: Secuencials/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "sc_empresa,sc_codTabla,sc_tabla,sc_secuencial,sc_DataBase,sc_Centralizado,sc_fechaing,sc_fechamod,sc_usuarioing,sc_usuariomod,sc_maquinaing,sc_maquinamod,sc_estado,sc_timestamp")] Secuencial secuencial)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        secuencial.sc_usuarioing = Session["UserName"].ToString();
                        myDat = "Alta de secuencia= " + secuencial.sc_tabla + " , " + secuencial.sc_DataBase  + " / sp_Altas_Secuencial";
                        int result = db.sp_Altas_Secuencial(Convert.ToInt16(coP.cls_empresa), secuencial.sc_tabla, secuencial.sc_DataBase, secuencial.sc_Centralizado,secuencial.sc_usuarioing, error);
                        WriteLogMessages.WriteFile(Session["LogonName"], myModulo + "-> ejecutando sp_Altas_Secuencial: " + string.Join(",", Convert.ToInt16(coP.cls_empresa), secuencial.sc_tabla, secuencial.sc_DataBase, secuencial.sc_Centralizado, secuencial.sc_usuarioing, "-> R: " + validad.getResponse(error)));
                        if (error.Value.ToString() == "")
                        {
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        else
                            throw new System.InvalidOperationException(error.Value.ToString(), new Exception(""));
                    }
                }
                catch (Exception ex)
                {
                    return RedirectToAction("miError", "Account", new { message = ex.Message, error = ex.ToString().Left(2048), inner = (ex.InnerException != null) ? ex.InnerException.Message.ToString().Left(2048) : "", modulo = myModulo, opcion = "Creando secuencia", myDat });
                }
            }

            return View(secuencial);
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
