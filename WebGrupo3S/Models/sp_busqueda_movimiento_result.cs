using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGrupo3S.Models
{
    public class sp_busqueda_movimiento_result
    {
        public short Empresa { get; set; }
        public short CodigoSucursal { get; set; }
        public string NombreSucursal { get; set; }
        public short CodigoMovimiento { get; set; }
        public string TipoMovimiento { get; set; }
        public string NombreTipoDocumento { get; set; }

    }
}