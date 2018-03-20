using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGrupo3S.Models
{
    public class TreeOpcion
    {
        public int Id { get; set; }
        public string text { get; set; }
        public bool @checked { get; set; }
        public int? perentId { get; set; }
        public virtual List<TreeOpcion> children { get; set; }

    }
}