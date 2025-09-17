using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class RestaurantDbContext : DbContext
    {
        public DbSet<Dish> Dish { get; set; }
        public DbSet<DeliveryType> DeliveryType { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
        public DbSet<Status> Status { get; set; }
        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dish>(entity =>
            {
                entity.Property(d => d.DishId);

                entity.Property(d => d.Name)
                .HasColumnType("varchar(255)")
                .IsRequired();

                entity.Property(d => d.Description)
                .HasColumnType("varchar(MAX)")
                .IsRequired();

                entity.Property(d => d.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

                entity.Property(d => d.Available)
                .HasColumnType("BIT")
                .IsRequired();

                entity.Property(d => d.ImageUrl)
                .HasColumnType("varchar(MAX)")
                .IsRequired();

                entity.Property(d => d.CreateDate)
                .IsRequired();

                entity.Property(d => d.UpdateDate)
                .IsRequired();

                entity.HasOne(d => d.Category)
                .WithMany(c => c.Dishes)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            });
            modelBuilder.Entity<DeliveryType>(entity =>
            {
                entity.HasKey(de => de.Id);
                entity.Property(de => de.Id)
                .ValueGeneratedOnAdd();

                entity.Property(de => de.Name)
                .HasColumnType("nvarchar(25)")
                .IsRequired();

                //precargo los datos
                entity.HasData(
                    new DeliveryType { Id = 1, Name = "Delivery" },
                    new DeliveryType { Id = 2, Name = "Take away" },
                    new DeliveryType { Id = 3, Name = "Dine in" }
                );

            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id)
                .ValueGeneratedOnAdd();

                entity.Property(c => c.Name)
                .HasColumnType("varchar(25)")
                .IsRequired();

                entity.Property(c => c.Description)
                .HasColumnType("varchar(255)")
                .IsRequired();

                entity.Property(c => c.Order)
                .HasColumnType("int")
                .IsRequired();

                //precargo los datos
                entity.HasData(
                    new Category { Id = 1, Name = "Entradas", Description = "Pequeñas porciones para abrir el apetito antes del plato principal.", Order = 1 },
                    new Category { Id = 2, Name = "Ensaladas", Description = "Opciones frescas y livianas, ideales como acompañamiento o plato principal.", Order = 2 },
                    new Category { Id = 3, Name = "Minutas", Description = "Platos rápidos y clásicos de bodegón: milanesas, tortillas, revueltos.", Order = 3 },
                    new Category { Id = 4, Name = "Pastas", Description = "Variedad de pastas caseras y salsas tradicionales.", Order = 5 },
                    new Category { Id = 5, Name = "Parrilla", Description = "Cortes de carne asados a la parrilla, servidos con guarniciones.", Order = 4 },
                    new Category { Id = 6, Name = "Pizzas", Description = "Pizzas artesanales con masa casera y variedad de ingredientes.", Order = 7 },
                    new Category { Id = 7, Name = "Sandwiches", Description = "Sandwiches y lomitos completos preparados al momento.", Order = 6 },
                    new Category { Id = 8, Name = "Bebidas", Description = "Gaseosas, jugos, aguas y opciones sin alcohol.", Order = 8 },
                    new Category { Id = 9, Name = "Cerveza Artesanal", Description = "Cervezas de producción artesanal, rubias, rojas y negras", Order = 9 },
                    new Category { Id = 10, Name = "Postres", Description = "Clásicos dulces caseros para cerrar la comida.", Order = 10 }
                );

            });
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.OrderId);
                entity.Property(o => o.OrderId)
                .ValueGeneratedOnAdd();

                entity.Property(o => o.DeliveryTo)
                .HasColumnType("varchar(255)")
                .IsRequired();

                entity.Property(o => o.Notes)
                .HasColumnType("varchar(MAX)")
                .IsRequired();

                entity.Property(o => o.Price)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

                entity.Property(o => o.CreateDate)
                .IsRequired();

                entity.Property(o => o.UpdateDate)
                .IsRequired();

                entity.HasOne(o => o.DeliveryType)
                .WithMany(dt => dt.Orders)
                .HasForeignKey(o => o.DeliveryTypeId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.OverallStatus)
                .WithMany(st => st.Orders)
                .HasForeignKey(o => o.OverallStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            });
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.OrderItemId);
                entity.Property(oi => oi.OrderItemId)
                .ValueGeneratedOnAdd();

                entity.Property(oi => oi.Quantity)
                .HasColumnType("int")
                .IsRequired();

                entity.Property(oi => oi.Notes)
                .HasColumnType("varchar(MAX)")
                .IsRequired();

                entity.Property(oi => oi.CreateDate)
                .IsRequired();

                entity.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(oi => oi.Dish)
                .WithMany(d => d.OrderItems)
                .HasForeignKey(oi => oi.DishId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(oi => oi.Status)
                .WithMany(st => st.OrderItems)
                .HasForeignKey(oi => oi.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            });
            modelBuilder.Entity<Status>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Id)
                .ValueGeneratedOnAdd();

                entity.Property(s => s.Name)
                .HasColumnType("varchar(25)")
                .IsRequired();

                entity.HasData(
                    new Status { Id = 1, Name = "Pending" },
                    new Status { Id = 2, Name = "In progress" },
                    new Status { Id = 3, Name = "Ready" },
                    new Status { Id = 4, Name = "Delivery" },
                    new Status { Id = 5, Name = "Closed" }
                );
            });
        }
    }
}
