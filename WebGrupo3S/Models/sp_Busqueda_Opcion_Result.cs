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

    public partial class sp_Busqueda_Opcion_Result
    {
        public short Empresa { get; set; }
        public int CodigoOpcion { get; set; }
        public string NombreOpcion { get; set; }
        public string DescripcionOpcion { get; set; }
        public int PadreOpcion { get; set; }
        public bool Autenticar { get; set; }
        public byte[] TimeStamp { get; set; }
        public virtual List<sp_Busqueda_Opcion_Result> children { get; set; }
    }
}
