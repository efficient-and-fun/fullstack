using Microsoft.EntityFrameworkCore;
using WebApi.Model;

namespace WebApi.Api.Common;

public class EfDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    
    public EfDbContext(DbContextOptions<EfDbContext> options, IConfiguration _configuration) : base(options)
    {
        this._configuration = _configuration;
    }
    
    public DbSet<MeetUps> MeetUps { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Participation> Participations { get; set; }
    
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Set table name to lowercase
            entity.SetTableName(entity.GetTableName().ToLower());

            foreach (var property in entity.GetProperties())
            {
                // Set column names to lowercase
                property.SetColumnName(property.GetColumnName().ToLower());
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}