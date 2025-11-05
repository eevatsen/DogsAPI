using dogs.Models;
using Microsoft.EntityFrameworkCore;

namespace dogs.Data;

public class DogsDbContext : DbContext
{
    public DogsDbContext(DbContextOptions<DogsDbContext> options) : base(options)
    {
    }

    public DbSet<Dog> Dogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // configure dogs table
        modelBuilder.Entity<Dog>(entity =>
        {
            entity.ToTable("dogs");
            entity.HasKey(e => e.Id);

            // make name unique
            entity.HasIndex(e => e.Name).IsUnique();

            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Color).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TailLength).IsRequired();
            entity.Property(e => e.Weight).IsRequired();
        });

        // add some default dogs
        modelBuilder.Entity<Dog>().HasData(
            new Dog
            {
                Id = 1,
                Name = "Neo",
                Color = "red&amber",
                TailLength = 22,
                Weight = 32
            },
            new Dog
            {
                Id = 2,
                Name = "Jessy",
                Color = "black&white",
                TailLength = 7,
                Weight = 14
            }
        );
    }
}