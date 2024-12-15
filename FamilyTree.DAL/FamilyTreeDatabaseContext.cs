using FamilyTree.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace FamilyTree.DAL;

public class FamilyTreeDatabaseContext : DbContext
{
    public DbSet<Person> Persons { get; set; } = null!;
    public DbSet<Relation> Relations { get; set; } = null!;

    public FamilyTreeDatabaseContext(DbContextOptions<FamilyTreeDatabaseContext> options)
    : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Relation>(entity =>
        {
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.FromPersonId).IsRequired();
            entity.Property(e => e.ToPersonId).IsRequired();
        });
    }
}
