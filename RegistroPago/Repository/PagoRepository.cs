using Microsoft.EntityFrameworkCore;
using RegistroPago.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegistroPago.Repository
{
    public class PagoRepository : IPagoRepository
    {
        PagoDBContext db;
        public PagoRepository(PagoDBContext _db)
        {
            db = _db;
        }



        public async Task<int> AddPago(Pago pago)
        {
            try
            {
                if (db != null)
                {
                    await db.Pagos.AddAsync(pago);
                    await db.SaveChangesAsync();

                    return pago.PagoId;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
              

            return 0;
        }

        public Task<Pago> GetPago(int? postId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Pago>> GetPagos()
        {
            if (db != null)
            {
                return await db.Pagos.ToListAsync();
            }

            return null;
        }

        public List<Pago> GetPagosExcel()
        {
            if (db != null)
            {
                return  db.Pagos.ToList();
            }

            return null;
        }


        public Int32 GetPagoxCip(decimal? cip)
        {
            try
            {
                if (db != null)
                {
                    // Realizar la consulta LINQ para buscar por CIP
                    var usuariosEncontrados = db.Pagos.Where(u => u.Cip == cip).ToList();
                    if (usuariosEncontrados.Count>0)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }


            return 0;
        }
    }
}
