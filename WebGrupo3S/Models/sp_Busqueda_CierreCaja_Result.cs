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
    
    public partial class sp_Busqueda_CierreCaja_Result
    {
        public short Empresa { get; set; }
        public int CodigoCierreCaja { get; set; }
        public string IdCaja { get; set; }
        public int CodigoSucursal { get; set; }
        public string NombreSucursal { get; set; }
        public System.DateTime FechaHoraCierre { get; set; }
        public string Moneda { get; set; }
        public string DescripcionMoneda { get; set; }
        public Nullable<decimal> SaldoAnterior { get; set; }
        public Nullable<decimal> EntradasEfectivo { get; set; }
        public Nullable<decimal> SalidasEfectivo { get; set; }
        public Nullable<decimal> EfectivoTeorico { get; set; }
        public Nullable<decimal> EfectivoFisico { get; set; }
        public Nullable<decimal> AjusteEfectivo { get; set; }
        public Nullable<decimal> ChequeTeorico { get; set; }
        public Nullable<decimal> ChequeFisico { get; set; }
        public Nullable<decimal> AjusteCheque { get; set; }
        public Nullable<decimal> TarjetaTeorico { get; set; }
        public Nullable<decimal> TarjetaFisico { get; set; }
        public Nullable<decimal> AjusteTarjeta { get; set; }
        public Nullable<decimal> Descuadre { get; set; }
        public Nullable<decimal> RetiroEfectivoAdmin { get; set; }
        public Nullable<decimal> SaldoActual { get; set; }
        public string EstatusCierreCaja { get; set; }
        public Nullable<decimal> TotalVenta { get; set; }
        public Nullable<decimal> TotalPropina { get; set; }
        public byte[] TimeStamp { get; set; }
    }
}
