using RegistroPago.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegistroPago.Repository
{
    public interface IPagoRepository
    {
        Task<List<Pago>> GetPagos();
        Task<int> AddPago(Pago pago);

        Task<Pago> GetPago(int? postId);

        Int32 GetPagoxCip(Decimal? cip);

        List<Pago> GetPagosExcel();

    }
}
