using Microsoft.EntityFrameworkCore;
using WebApi.Model;

namespace WebApi.Api.Common;

public class EfDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    
    public EfDbContext(DbContextOptions<EfDbContext> options, IConfiguration _configuration) : base(options)
    {
        this._configuration = _configuration;
        
        // TODO: find a fix for the problem that is described in this Stackoverflow post (it does the job but it appears to be a legacy variation).
        // https://stackoverflow.com/questions/69961449/net6-and-datetime-problem-cannot-write-datetime-with-kind-utc-to-postgresql-ty
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
    
    public DbSet<MeetUps> MeetUps { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Participation> Participations { get; set; }
    public DbSet<FriendConnection> FriendConnection { get; set; }
    
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
        }
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