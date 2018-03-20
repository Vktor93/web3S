using System.ComponentModel.DataAnnotations;
using WebGrupo3S.ValidationHelper;

namespace WebGrupo3S.Models
{
    public class Addnode
    {
        [Required(ErrorMessage = "Node type is required.")]
        public string NodeTypeRbtn { get; set; }

        [Required(ErrorMessage = "Node Name is required.")]
        public string NodeName { get; set; }

        //[Required(ErrorMessage = "Parent Name is required.")]
        [requiredif("NodeTypeRbtn", "Cn", ErrorMessage = "Parent Node is required")]
        public int? ParentName { get; set; }
    }
}