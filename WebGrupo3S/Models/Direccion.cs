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
    using System.ComponentModel.DataAnnotations;

    public partial class Direccion
    {
        public short di_empresa { get; set; }
        public int di_IdDireccion { get; set; }
        public string di_TipoAsociado { get; set; }
        public int di_asociado { get; set; }
        [Required(ErrorMessage = "Tipo de dirección es requerida")]
        public string di_TipoDireccion { get; set; }
        [Required(ErrorMessage = "Dirección es requerido")]
        public string di_Direccion { get; set; }
        public string di_Direccion2 { get; set; }
        public string di_zona { get; set; }
        [Required(ErrorMessage = "Municipio es requerido")]
        public string di_municipio { get; set; }
        [Required(ErrorMessage = "Departamento es requerido")]
        public string di_departamento { get; set; }
        [Required(ErrorMessage = "País es requerido")]
        public string di_pais { get; set; }
        public System.DateTime di_fechaing { get; set; }
        public Nullable<System.DateTime> di_fechamod { get; set; }
        public string di_usuarioing { get; set; }
        public string di_usuariomod { get; set; }
        public string di_maquinaing { get; set; }
        public string di_maquinamod { get; set; }
        public int di_estado { get; set; }
        public byte[] di_timestamp { get; set; }
    }
}
