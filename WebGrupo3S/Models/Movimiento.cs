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
    
    public partial class Movimiento
    {
        public short mv_empresa { get; set; }
        public int mv_IdSucursal { get; set; }
        public int mv_IdMov { get; set; }
        public string mv_ProdServ { get; set; }
        public int mv_IdProdServ { get; set; }
        public int mv_Cantidad { get; set; }
        public decimal mv_ValorMov { get; set; }
        public string mv_TipoMovimiento { get; set; }
        public System.DateTime mv_FechaMov { get; set; }
        public int mv_IdDoc { get; set; }
        public Nullable<int> mv_IdOrdenServicio { get; set; }
        public Nullable<int> mv_IdDetalleOrdenServicio { get; set; }
        public Nullable<int> mv_IdAutorizacion { get; set; }
        public System.DateTime mv_fechaing { get; set; }
        public Nullable<System.DateTime> mv_fechamod { get; set; }
        public string mv_usuarioing { get; set; }
        public string mv_usuariomod { get; set; }
        public string mv_maquinaing { get; set; }
        public string mv_maquinamod { get; set; }
        public int mv_estado { get; set; }
        public byte[] mv_timestamp { get; set; }
    }
}
