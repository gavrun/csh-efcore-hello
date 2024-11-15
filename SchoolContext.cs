using Microsoft.EntityFrameworkCore;

public class SchoolContext : DbContext
{
    public DbSet<Student> Students { get; set; }

    //add custom set
    public DbSet<StudentGroup> StudentGroups { get; set; }

    //table does not exist for this set
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StudentGroup>().HasNoKey();
        //modelBuilder.Entity<StudentGroup>().HasNoKey().ToView(null); //disable tracking
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=SchoolDB;Trusted_Connection=True;");
    }
}
