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
    
    public partial class FacturaFormaPago
    {
        public short fp_empresa { get; set; }
        public int fp_IdFacturaFormaPago { get; set; }
        public int fp_IdSucursal { get; set; }
        public int fp_IdDoc { get; set; }
        public Nullable<int> fp_IdMovDocPromo { get; set; }
        public string fp_IdFormaPago { get; set; }
        public Nullable<int> fp_IdAutorizacion { get; set; }
        public Nullable<decimal> fp_Cantidad { get; set; }
        public Nullable<int> fp_Unidades { get; set; }
        public Nullable<long> fp_NoDocumento { get; set; }
        public string fp_IdBanco { get; set; }
        public string fp_IdMarcaTarjeta { get; set; }
        public string fp_Comentario { get; set; }
        public string fp_Moneda { get; set; }
        public Nullable<decimal> fp_TasaCambio { get; set; }
        public Nullable<decimal> fp_CantidadOriginal { get; set; }
        public Nullable<int> fp_IdCierreCaja { get; set; }
        public Nullable<int> fp_ConfirmaCierreCaja { get; set; }
        public Nullable<decimal> fp_efectivoRecibido { get; set; }
        public Nullable<decimal> fp_efectivoVuelto { get; set; }
        public System.DateTime fp_fechaing { get; set; }
        public Nullable<System.DateTime> fp_fechamod { get; set; }
        public string fp_usuarioing { get; set; }
        public string fp_usuariomod { get; set; }
        public string fp_maquinaing { get; set; }
        public string fp_maquinamod { get; set; }
        public int fp_estado { get; set; }
        public byte[] fp_timestamp { get; set; }
    }
}
