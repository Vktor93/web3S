using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebGrupo3S.Models
{
    public class loginViewModel
    {
        [Required(ErrorMessage = "Usuario es requerido")]
        public string usuario { get; set; }
        [Required(ErrorMessage = "Contraseña es requeridad")]
        [DataType(DataType.Password)]
        public string password { get; set; }
        public int codigo { get; set; }
        public string nombre { get; set; }
        public int resetpass { get; set; }
    }

    public class RestableceViewModel
    {
        [Required(ErrorMessage = "Contraseña es requerida.")]
        [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        [Display(Name = "Contraseña")]
        public string password { get; set; }

        [Display(Name = "Confirmar contraseña")]
        [Compare("password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }
    }
}
