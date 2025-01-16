using Microsoft.EntityFrameworkCore;

namespace ExpenseApp.Models
{
    public class DBContext : DbContext
    {
        public DbSet<TreeItem> TreeItems { get; set; }
        public DbSet<DetailItem> DetailItems { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
#if ANDROID
            optionsBuilder.UseSqlite($"Data Source={path}\\ExpenseDB.db;");
#elif WINDOWS
            optionsBuilder.UseSqlServer("Server=.;Database=ExpenseDB;Trusted_Connection=True;TrustServerCertificate = True;");
#endif
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("Arabic_100_CI_AS");
            modelBuilder.Entity<DetailItem>(entity =>
            {
                entity.ToTable("DetailItem");

                entity.Property(e => e.ID)
                      .IsRequired(true)
                      .HasColumnName("id");
                entity.Property(e => e.ParentID)
                      .IsRequired(true)
                      .HasColumnName("ParentID");
                entity.Property(e => e.Date)
                      .IsRequired(true)
                      .HasColumnName("Date");
                entity.Property(e => e.Amount)
                      .IsRequired(true)
                      .HasColumnName("Amount");
                entity.Property(e => e.Note)
                      .HasMaxLength(100)
                      .HasColumnName("Note");
            });

            modelBuilder.Entity<TreeItem>(entity =>
            {
                entity.ToTable("TreeItem");

                entity.HasMany(tree => tree.Details) // TreeItem يحتوي على قائمة Details
                      .WithOne() // DetailItem ليس لديه Navigation إلى TreeItem
                      .HasForeignKey(detail => detail.ParentID) // المفتاح الخارجي في DetailItem
                      .OnDelete(DeleteBehavior.Cascade); // عند حذف TreeItem، يتم حذف Details المرتبطة.

                entity.Property(e => e.ID)
                      .IsRequired(true)
                      .HasColumnName("id");
                entity.Property(e => e.Title)
                      .IsRequired(true)
                      .HasMaxLength(20)
                      .HasColumnName("Title");
                entity.Property(e => e.color)
                        .IsRequired(true)
                        .HasColumnName("color");
                entity.HasData(
                    new TreeItem { ID = 1, Title = "طعام" ,color = "#213456"},
                    new TreeItem { ID = 2, Title = "مواصلات", color = "#f1e3e1" },
                    new TreeItem { ID = 3, Title = "صحة", color = "#b00b69" },
                    new TreeItem { ID = 4, Title = "رفاهية", color = "#a68105" },
                    new TreeItem { ID = 5, Title = "فواتير", color = "#1bd635" }
                );
            });

        }
    }
}
