using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegistroPago.Models
{
    public class PaginacionViewModel
    {
        public IEnumerable<Pago> Pagos { get; set; }
        public int PaginaActual { get; set; }
        public int TotalElementos { get; set; }
        public int ElementosPorPagina { get; set; }
        public int TotalPaginas { get; set; }
        public string TerminoBusqueda { get; set; }
    }
}
