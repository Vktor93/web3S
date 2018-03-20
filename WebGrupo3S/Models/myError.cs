namespace WebGrupo3S.Models
{ 
    using System;
    using System.Collections.Generic;

    public partial class myError
    {
        public string message { get; set; }
        public string messageusuario { get; set; }
        public string excepcion { get; set; }
        public string usuario { get; set; }
        public string nombre { get; set; }
        public string modulo { get; set; }
        public string opcion { get; set; }
        public string ip { get; set; }
        public string browser { get; set; }
        public string source { get; set; }
        public string targetsite { get; set; }
        public string data { get; set; }
    }
}