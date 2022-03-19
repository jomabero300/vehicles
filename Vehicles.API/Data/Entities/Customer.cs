using System;
using System.ComponentModel.DataAnnotations;

namespace Vehicles.API.Data.Entities
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }

        [StringLength(30, ErrorMessage = "El campo {0} debe contener entre {2} y {1} caractere", MinimumLength = 3)]
        [Required(ErrorMessage = "¡El campo {0} es requerido!")]
        [Display(Name = "Nombres")]
        public string FirstName { get; set; }

        [StringLength(30, ErrorMessage = "El campo {0} debe contener entre {2} y {1} caractere", MinimumLength = 3)]
        [Required(ErrorMessage = "¡El campo {0} es requerido!")]
        [Display(Name = "Apellidos")]
        public string LastName { get; set; }

        [StringLength(30, ErrorMessage = "El campo {0} debe contener entre {2} y {1} caractere", MinimumLength = 3)]
        [Required(ErrorMessage = "¡El campo {0} es requerido!")]
        [Display(Name = "Dirección")]
        public string Address { get; set; }

        [Display(Name = "Celular")]
        [StringLength(30, ErrorMessage = "El campo {0} debe contener entre {2} y {1} caractere", MinimumLength = 3)]
        public string PhoneNumber { get; set; }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "¡El campo {0} es requerido!")]
        public bool Estate { get; set; }

        public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }

    }
}
