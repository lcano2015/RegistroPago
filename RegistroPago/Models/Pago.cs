using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistroPago.Models
{
    public partial class Pago
    {
        [Key]
        public int PagoId { get; set; }


        [Required(ErrorMessage = "El campo CIP es requerido.")]
        public Decimal Cip { get; set; }

        [Required(ErrorMessage = "El campo Apellidos y Nombre es requerido.")]
        public string NombreApellido { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "El campo Fecha es requerido.")]
        public DateTime FechaVoucher { get; set; }
        public string ImageName { get; set; }
      
        [NotMapped]
        public IFormFile ImageFile { get; set; }

       
        public int Estado { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaRegistro { get; set; }

    }
}
