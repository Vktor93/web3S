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
    
    public partial class sp_Busqueda_Horario_Result
    {
        public short Empresa { get; set; }
        public int IdHorario { get; set; }
        public int DiaSemana { get; set; }
        public string NombreHorario { get; set; }
        public string DescripcionHorario { get; set; }
        public string HoraEntrada { get; set; }
        public string HoraSalida { get; set; }
        public string HoraSalidaAlmuerzo { get; set; }
        public string HoraEntradaAlmuerzo { get; set; }
        public byte[] TimeStamp { get; set; }
    }
}
