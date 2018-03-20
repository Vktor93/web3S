using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using WebGrupo3S.Models;
using System.Data.Entity.Core.Objects;
using System.Web.UI;

namespace WebGrupo3S.Helpers
{
    public class validad
    {
        private int[] ma = new int[40];
        private int cant = 40;
        private SSS_PERSONASEntities db = new SSS_PERSONASEntities();
        public string acc = "";
        public ConstantesP coP = new ConstantesP();
        ObjectParameter error = new ObjectParameter("error", typeof(String));

        public validad()
        {

        }

        public class PermisosSearchResultModel
        {
            public IEnumerable<sp_Busqueda_Permiso_Result> permisoResults { get; set; }
        }


        private int traPermiso(int opcion, string perfiles, IEnumerable<sp_Busqueda_Permiso_Result> lpe)
        {
            //var PermisoSearch = new PermisosSearchResultModel();
            int intResult = 0;
            int p = 0;
            //string[] perf = perfiles.Split(',');
            try
            {
                if (lpe.Where(l => l.CodigoOpcion == opcion).Count() > 0)
                {
                    //WriteLogMessages.WriteFile("traePermiso", "Validación" + "-> ejecutando db.sp_Busqueda_Permiso: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), p, opcion, null, "-> R: " + validad.getResponse(error)));
                    intResult = 1;
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return intResult;
        }


        public void seteMenu(string pe)
        {
            string[] cat = ("Catalogo de perfiles,Catalogo Usuarios,Mantenimiento horarios,Mantenimiento Empleados,Nit facturas,Mantenimiento clientes,proveedores,Cuentas por cobrar,Cuentas por pagar,Tipo Producto,Productos,Tipo servicios,Servicios,Mantenimientos Sucursal,CATALOGO PERMISOS,Secuencial,Mantenimiento Catálogos,Bitacora").Split(',');
            var PermisoSearch = new PermisosSearchResultModel();
            List<opcion> opc = new List<opcion>();
            try
            {
                int i = 0;
                int padre = 0;
                PermisoSearch.permisoResults = db.sp_Busqueda_Permiso(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), Convert.ToInt16(pe), null, null, error).ToList();
                WriteLogMessages.WriteFile("valida", "Validación" + "-> ejecutando db.sp_Busqueda_Permiso: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), pe, null, null, "-> R: " + validad.getResponse(error)));
                foreach (string dat in cat)
                {
                    opc = db.opcion.Where(a => a.op_NombreOpcion == dat && a.op_Estado == 1).ToList();
                    if (opc != null)
                    {
                        foreach (opcion re in opc)
                        {
                            padre = Convert.ToInt16(re.op_CodigoOpcion.ToString());
                            ma[i] = traPermiso(padre, pe, PermisoSearch.permisoResults);
                        }
                    }
                    else throw new System.InvalidOperationException("No puede resolver la opción de menú - " + dat + "!-", new Exception(""));
                    i++;
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            for (int i = 0; i < cant; i++)
            {
                acc = acc + ma[i].ToString();
            }
        }


        public void seteAccesos(string catalogo, string pe)
        {
            List<opcion> opc = new List<opcion>();
            List<opcion> opc1 = new List<opcion>();
            var PermisoSearch = new PermisosSearchResultModel();
            opcion op = new opcion();

            catalogo = catalogo.ToUpper();
            for (int i = 0; i < cant; i++)
            {
                ma[i] = 0;
            }

            try
            {
                string[] pes = pe.Split(',');
                if (db.opcion.Where(a => a.op_NombreOpcion == catalogo && a.op_Estado == 1).Any())
                {
                    op = db.opcion.Where(a => a.op_NombreOpcion == catalogo && a.op_Estado == 1).First();
                    if (op != null)
                    {
                        foreach (string perfil_data in pes)
                        {
                            int padre = Convert.ToInt16(op.op_CodigoOpcion.ToString());
                            PermisoSearch.permisoResults = db.sp_Busqueda_Permiso(2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), Convert.ToInt16(perfil_data), null, null, error).ToList();
                            WriteLogMessages.WriteFile("valida", "Validación" + "-> ejecutando db.sp_Busqueda_Permiso: " + string.Join(",", 2, "", Convert.ToInt16(coP.cls_empresa), Convert.ToInt16(coP.cls_sucursal), perfil_data, null, null, "-> R: " + validad.getResponse(error)));
                            opc = db.opcion.Where(a => a.op_PadreOpcion == padre && a.op_Estado == 1).ToList();
                            foreach (opcion re in opc)
                            {
                                switch (re.op_NombreOpcion.ToUpper())
                                {
                                    case "CONSULTA":
                                        ma[0] = traPermiso(re.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                        break;
                                    case "NUEVO":
                                        ma[1] = traPermiso(re.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                        break;
                                    case "EDITAR":
                                        ma[2] = traPermiso(re.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                        break;
                                    case "DEBAJA":
                                        ma[3] = traPermiso(re.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                        break;
                                    case "PERFILES":
                                        ma[4] = traPermiso(re.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                        break;
                                    case "RESETEA":
                                        ma[5] = traPermiso(re.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                        break;
                                    case "HORARIOS":
                                        ma[6] = traPermiso(re.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                        break;
                                    case "DIRECCION":
                                        ma[7] = traPermiso(re.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                        opc1 = db.opcion.Where(a => a.op_PadreOpcion == re.op_CodigoOpcion && a.op_Estado == 1).ToList();
                                        foreach (opcion re1 in opc1)
                                        {
                                            switch (re1.op_NombreOpcion.ToUpper())
                                            {
                                                case "NUEVO":
                                                    ma[31] = traPermiso(re1.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                                    break;
                                                case "EDITAR":
                                                    ma[32] = traPermiso(re1.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                                    break;
                                                case "DEBAJA":
                                                    ma[33] = traPermiso(re1.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                                    break;
                                                case "REPORTE":
                                                    ma[34] = traPermiso(re1.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        break;
                                    case "CONTACTOS":
                                        ma[8] = traPermiso(re.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                        opc1 = db.opcion.Where(a => a.op_PadreOpcion == re.op_CodigoOpcion && a.op_Estado == 1).ToList();
                                        foreach (opcion re1 in opc1)
                                        {
                                            switch (re1.op_NombreOpcion.ToUpper())
                                            {
                                                case "NUEVO":
                                                    ma[25] = traPermiso(re1.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                                    break;
                                                case "EDITAR":
                                                    ma[26] = traPermiso(re1.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                                    break;
                                                case "DEBAJA":
                                                    ma[27] = traPermiso(re1.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                                    break;
                                                case "REPORTE":
                                                    ma[28] = traPermiso(re1.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        break;
                                    case "PERMISOS":
                                        ma[9] = traPermiso(re.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                        break;
                                    case "DETALLE":
                                        ma[10] = traPermiso(re.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                        opc1 = db.opcion.Where(a => a.op_PadreOpcion == re.op_CodigoOpcion && a.op_Estado == 1).ToList();
                                        foreach (opcion re1 in opc1)
                                        {
                                            switch (re1.op_NombreOpcion.ToUpper())
                                            {
                                                case "NUEVO":
                                                    ma[25] = traPermiso(re1.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                                    break;
                                                case "EDITAR":
                                                    ma[26] = traPermiso(re1.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                                    break;
                                                case "DEBAJA":
                                                    ma[27] = traPermiso(re1.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                                    break;
                                                case "REPORTE":
                                                    ma[28] = traPermiso(re1.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        break;
                                    case "PRECIO":
                                        ma[11] = traPermiso(re.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                        opc1 = db.opcion.Where(a => a.op_PadreOpcion == re.op_CodigoOpcion && a.op_Estado == 1).ToList();
                                        foreach (opcion re1 in opc1)
                                        {
                                            switch (re1.op_NombreOpcion.ToUpper())
                                            {
                                                case "NUEVO":
                                                    ma[25] = traPermiso(re1.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                                    break;
                                                case "EDITAR":
                                                    ma[26] = traPermiso(re1.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                                    break;
                                                case "DEBAJA":
                                                    ma[27] = traPermiso(re1.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                                    break;
                                                case "REPORTE":
                                                    ma[28] = traPermiso(re1.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        break;
                                    case "REPORTE":
                                        ma[19] = traPermiso(re.op_CodigoOpcion, pe, PermisoSearch.permisoResults);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            for (int i = 0; i < cant; i++)
            {
                acc = acc + ma[i].ToString();
            }
        }
        public static string getResponse(ObjectParameter error)
        {
            string str_validate = "";
            if (error.Value != null) str_validate = error.Value.ToString();
            return (str_validate == "" ? "Ok" : str_validate);
        }

        public enum MessageType
        {
            Success,
            Info,
            Warning,
            Danger
        }
        public static void ShowMessage(Control control, string Message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(control, control.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
        }
    }
}