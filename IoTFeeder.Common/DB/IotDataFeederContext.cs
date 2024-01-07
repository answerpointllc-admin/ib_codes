using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IoTFeeder.Common.DB
{
    public partial class IotDataFeederContext : DbContext
    {
        public IotDataFeederContext()
        {
        }

        public IotDataFeederContext(DbContextOptions<IotDataFeederContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CommonSetting> CommonSettings { get; set; } = null!;
        public virtual DbSet<Enum> Enums { get; set; } = null!;
        public virtual DbSet<IotDevice> IotDevices { get; set; } = null!;
        public virtual DbSet<IotDeviceProperty> IotDeviceProperties { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=192.169.177.110,3666;Database=IotDataFeeder;UID=IotDataFeederUser;PWD=IotDataFeeder@123;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CommonSetting>(entity =>
            {
                entity.Property(e => e.ClientId)
                    .HasMaxLength(10)
                    .HasColumnName("clientId");

                entity.Property(e => e.ClientSecret)
                    .HasMaxLength(10)
                    .HasColumnName("clientSecret");

                entity.Property(e => e.DatabaseName)
                    .HasMaxLength(50)
                    .HasColumnName("databaseName");

                entity.Property(e => e.KustoUri)
                    .HasMaxLength(100)
                    .HasColumnName("kustoUri");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(100)
                    .HasColumnName("tenantId");
            });

            modelBuilder.Entity<Enum>(entity =>
            {
                entity.Property(e => e.EnumValue).HasMaxLength(100);
            });

            modelBuilder.Entity<IotDevice>(entity =>
            {
                entity.ToTable("IotDevice");

                entity.Property(e => e.DeviceName).HasMaxLength(100);
            });

            modelBuilder.Entity<IotDeviceProperty>(entity =>
            {
                entity.ToTable("IotDeviceProperty");

                entity.Property(e => e.MaxLength).HasMaxLength(50);

                entity.Property(e => e.MaxValue).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.MinValue).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.PropertyName).HasMaxLength(100);

                entity.HasOne(d => d.DataTypeNavigation)
                    .WithMany(p => p.IotDeviceProperties)
                    .HasForeignKey(d => d.DataType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IotDeviceProperty_Enums");

                entity.HasOne(d => d.IotDevice)
                    .WithMany(p => p.IotDeviceProperties)
                    .HasForeignKey(d => d.IotDeviceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IotDeviceProperty_IotDevice");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
