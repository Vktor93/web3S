//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebGrupo3S.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Horario
    {
        public short ho_empresa { get; set; }
        public int ho_IdHorario { get; set; }
        public int ho_diaSemana { get; set; }
        public string ho_Nombre { get; set; }
        public string ho_Descripcion { get; set; }
        public string ho_HoraEntrada { get; set; }
        public string ho_HoraSalida { get; set; }
        public string ho_HoraSalidaAlmuerzo { get; set; }
        public string ho_HoraEntradaAlmuerzo { get; set; }
        public System.DateTime ho_fechaing { get; set; }
        public Nullable<System.DateTime> ho_fechamod { get; set; }
        public string ho_usuarioing { get; set; }
        public string ho_usuariomod { get; set; }
        public string ho_maquinaing { get; set; }
        public string ho_maquinamod { get; set; }
        public int ho_estado { get; set; }
        public byte[] ho_timestamp { get; set; }
    }
}
