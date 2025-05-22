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
            entity.SetTableName(entity.GetTableName().ToLower());

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName().ToLower());
            }
        }
        
        modelBuilder.Entity<FriendConnection>()
            .HasOne(fc => fc.Friend)
            .WithMany(u => u.FriendOf)
            .HasForeignKey(fc => fc.FriendId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FriendConnection>()
            .HasOne(fc => fc.User)
            .WithMany(u => u.Friends)
            .HasForeignKey(fc => fc.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        base.OnModelCreating(modelBuilder);
        
        
    }
}