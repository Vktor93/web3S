//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebGrupo3S.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Sucursal
    {
        public short su_empresa { get; set; }
        public int su_IdSucursal { get; set; }
        public string su_Nombre { get; set; }
        public int su_Encargado { get; set; }
        public string su_Direccion { get; set; }
        public string su_Telefono { get; set; }
        public System.DateTime su_fechaing { get; set; }
        public Nullable<System.DateTime> su_fechamod { get; set; }
        public string su_usuarioing { get; set; }
        public string su_usuariomod { get; set; }
        public string su_maquinaing { get; set; }
        public string su_maquinamod { get; set; }
        public int su_estado { get; set; }
        public byte[] su_timestamp { get; set; }
    }
}
