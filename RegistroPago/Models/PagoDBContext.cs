using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace RegistroPago.Models
{
    public partial class PagoDBContext : DbContext
    {
        public PagoDBContext()
        {
        }

        public PagoDBContext(DbContextOptions<PagoDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Pago> Pagos { get; set; }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseSqlServer("Server=DESKTOP-PAHTF05;Database=PagoDB;Trusted_Connection=True;MultipleActiveResultSets=True;");
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pago>(entity =>
            {
                //entity.Property(e => e.PagoId).ValueGeneratedNever();
                entity.Property(e => e.FechaVoucher).HasColumnType("datetime");

                entity.Property(e => e.Cip)
                 .IsRequired();

                entity.Property(e => e.ImageName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NombreApellido)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Estado)
                 .IsRequired();

                entity.Property(e => e.FechaRegistro).HasColumnType("datetime");

            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
