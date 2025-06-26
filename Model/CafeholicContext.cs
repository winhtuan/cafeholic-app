using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CAFEHOLIC.Model;

public partial class CafeholicContext : DbContext
{
    public CafeholicContext()
    {
    }

    public CafeholicContext(DbContextOptions<CafeholicContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Drink> Drinks { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoomType> RoomTypes { get; set; }

    public virtual DbSet<StudyRoom> StudyRooms { get; set; }

    public virtual DbSet<Suggestion> Suggestions { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=Cafeholic;Trusted_Connection=true;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccId).HasName("PK__Account__91CBC3986AE48249");

            entity.ToTable("Account");

            entity.Property(e => e.AccId).HasColumnName("AccID");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.RegistDate).HasColumnType("datetime");
            entity.Property(e => e.VerificationToken).HasMaxLength(255);

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Account__RoleId__4D94879B");

            entity.HasOne(d => d.User).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Account__UserId__4E88ABD4");
        });

        modelBuilder.Entity<Drink>(entity =>
        {
            entity.HasKey(e => e.DrinkId).HasName("PK__Drink__C094D3E8D03FA2DE");

            entity.ToTable("Drink");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__Menu__C99ED2302B0B8148");

            entity.ToTable("Menu");

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Drink).WithMany(p => p.Menus)
                .HasForeignKey(d => d.DrinkId)
                .HasConstraintName("FK__Menu__DrinkId__6E01572D");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__C3905BCF13136EA6");

            entity.ToTable("Order");

            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Order__UserId__60A75C0F");

            entity.HasOne(d => d.Voucher).WithMany(p => p.Orders)
                .HasForeignKey(d => d.VoucherId)
                .HasConstraintName("FK__Order__VoucherId__619B8048");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED068154BC7476");

            entity.ToTable("OrderItem");

            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Drink).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.DrinkId)
                .HasConstraintName("FK__OrderItem__Drink__656C112C");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderItem__Order__6477ECF3");
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Password__3214EC070E58EF60");

            entity.ToTable("PasswordResetToken");

            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.Token).HasMaxLength(255);

            entity.HasOne(d => d.Account).WithMany(p => p.PasswordResetTokens)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__PasswordR__Accou__5165187F");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A3838799FDC");

            entity.ToTable("Payment");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentType).HasMaxLength(50);

            entity.HasOne(d => d.Transaction).WithMany(p => p.Payments)
                .HasForeignKey(d => d.TransactionId)
                .HasConstraintName("FK__Payment__Transac__6B24EA82");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PK__Reservat__B7EE5F2475376C56");

            entity.ToTable("Reservation");

            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Room).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK__Reservati__RoomI__5BE2A6F2");

            entity.HasOne(d => d.User).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Reservati__UserI__5AEE82B9");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Review__74BC79CE2DFE60F6");

            entity.ToTable("Review");

            entity.Property(e => e.Comment).HasMaxLength(500);
            entity.Property(e => e.ReviewDate).HasColumnType("datetime");

            entity.HasOne(d => d.Drink).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.DrinkId)
                .HasConstraintName("FK__Review__DrinkId__71D1E811");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Review__UserId__70DDC3D8");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC0745C3ECCE");

            entity.ToTable("Role");

            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__RoomType__516F03B5CEA61183");

            entity.ToTable("RoomType");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<StudyRoom>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__StudyRoo__328639397C49C217");

            entity.ToTable("StudyRoom");

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.RoomType).WithMany(p => p.StudyRooms)
                .HasForeignKey(d => d.RoomTypeId)
                .HasConstraintName("FK__StudyRoom__RoomT__5812160E");
        });

        modelBuilder.Entity<Suggestion>(entity =>
        {
            entity.HasKey(e => e.SuggestionId).HasName("PK__Suggesti__94099508E16B15D6");

            entity.ToTable("Suggestion");

            entity.Property(e => e.Content).HasMaxLength(500);
            entity.Property(e => e.SuggestionDate).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Suggestions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Suggestio__UserI__74AE54BC");

            entity.HasMany(d => d.Drinks).WithMany(p => p.Suggestions)
                .UsingEntity<Dictionary<string, object>>(
                    "SuggestionItem",
                    r => r.HasOne<Drink>().WithMany()
                        .HasForeignKey("DrinkId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Suggestio__Drink__787EE5A0"),
                    l => l.HasOne<Suggestion>().WithMany()
                        .HasForeignKey("SuggestionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Suggestio__Sugge__778AC167"),
                    j =>
                    {
                        j.HasKey("SuggestionId", "DrinkId").HasName("PK__Suggesti__0800D836AEBCA49A");
                        j.ToTable("SuggestionItem");
                    });
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__55433A6B32A019DE");

            entity.Property(e => e.AmountPaid).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TransactionDate).HasColumnType("datetime");

            entity.HasOne(d => d.Order).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Transacti__Order__68487DD7");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07260B31A7");

            entity.ToTable("User");

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.VoucherId).HasName("PK__Voucher__3AEE7921004D6EF1");

            entity.ToTable("Voucher");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.VoucherCode).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
