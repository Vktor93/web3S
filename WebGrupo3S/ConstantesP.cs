using System;
using System.Web.Mvc;

namespace WebGrupo3S
{

    public class ConstantesP
    {
        public String cls_usuario = "Admin";
        public String cls_empresa = "1";
        public String cls_sucursal = "1";
        public String cls_us_estado_activo = "1";
        public String cls_us_estado_baja = "0";
    }

    public class PermisosP
    {
        public Boolean cls_guardar = false;
        public Boolean cls_nuevo = false;
        public Boolean cls_debaja = false;
        public Boolean cls_editar = false;
        public Boolean cls_refrescar = false;
        public Boolean cls_ingreso = false;
    }
}