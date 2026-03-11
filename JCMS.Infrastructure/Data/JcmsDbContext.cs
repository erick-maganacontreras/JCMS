using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JCMS.Infrastructure.Entities;

namespace JCMS.Infrastructure.Data
{
    public class JcmsDbContext : DbContext
    {
        public JcmsDbContext(DbContextOptions<JcmsDbContext> options) 
            : base(options) 
        { 
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<JewelryItem> JewelryItems { get; set; }
        public DbSet<CleaningOrder> CleaningOrder { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CleaningHistory> CleaningHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraints
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<Staff>()
                .HasIndex(s => s.UserName)
                .IsUnique();

            // Charm bracelet - restrict
            modelBuilder.Entity<JewelryItem>()
                .HasOne(j => j.ParentItem)
                .WithMany(p => p.ChildItems)
                .HasForeignKey(j => j.ParentItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // OrderItem relationships - CRITICAL: explicitly restrict both sides
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.CleaningOrder)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.CleaningOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.JewelryItem)
                .WithMany(j => j.OrderItems)
                .HasForeignKey(oi => oi.JewelryItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customer relationships - restrict
            modelBuilder.Entity<JewelryItem>()
                .HasOne(j => j.Customer)
                .WithMany(c => c.JewelryItems)
                .HasForeignKey(j => j.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CleaningOrder>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.CleaningOrders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Staff relationships - restrict
            modelBuilder.Entity<CleaningOrder>()
                .HasOne(o => o.Staff)
                .WithMany(s => s.CleaningOrders)
                .HasForeignKey(o => o.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            // CleaningHistory relationships - restrict
            modelBuilder.Entity<CleaningHistory>()
                .HasOne(h => h.JewelryItem)
                .WithMany(j => j.CleaningHistoryEntries)
                .HasForeignKey(h => h.JewelryItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CleaningHistory>()
                .HasOne(h => h.Staff)
                .WithMany(s => s.CleaningHistoryEntries)  // Now matches the new property
                .HasForeignKey(h => h.StaffId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
