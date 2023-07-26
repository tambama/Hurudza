using Hurudza.Common.Services.Interfaces;
using Hurudza.Data.Models.Base;
using Hurudza.Data.Models.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Hurudza.Data.Context.Context;

public class HurudzaDbContext :
    IdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        string,
        ApplicationUserClaim,
        ApplicationUserRole,
        ApplicationUserLogin,
        ApplicationRoleClaim,
        ApplicationUserToken>,
    IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeService _dateTimeService;
    private readonly ILoggerFactory _loggerFactory;

    public HurudzaDbContext(DbContextOptions<HurudzaDbContext> options,
        ICurrentUserService currentUserService,
        IDateTimeService dateTimeService,
        ILoggerFactory loggerFactory)
        : base(options)
    {
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
        _loggerFactory = loggerFactory;
    }

    // User Management
    public DbSet<IdentityClaim> Claims { get; set; }

    // Administrative Structure
    public DbSet<Province> Provinces { get; set; }
    public DbSet<District> AdminDistricts { get; set; }
    public DbSet<Ward> Wards { get; set; }
    
    // System
    public DbSet<Farm> Farms { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    
    // Notifications
    public DbSet<SendGridTemplate>? SendGridTemplates { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        if (!string.IsNullOrEmpty(_currentUserService.UserId)) entry.Entity.CreatorId = _currentUserService.UserId;
                        entry.Entity.CreatedDate = _dateTimeService.Now;
                        break;
                    case EntityState.Modified:
                        if (!string.IsNullOrEmpty(_currentUserService.UserId)) entry.Entity.ModifiedBy = _currentUserService.UserId;
                        entry.Entity.ModifiedDate = _dateTimeService.Now;
                        break;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            throw;
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLoggerFactory(_loggerFactory);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(b =>
        {
            // Each User can have many UserClaims
            b.HasMany(e => e.Claims)
                .WithOne(e => e.User)
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();

            // Each User can have many UserLogins
            b.HasMany(e => e.Logins)
                .WithOne(e => e.User)
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();

            // Each User can have many UserTokens
            b.HasMany(e => e.Tokens)
                .WithOne(e => e.User)
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();

            // Each User can have many entries in the UserRole join table
            b.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        });

        builder.Entity<ApplicationRole>(b =>
        {
            // Each Role can have many entries in the UserRole join table
            b.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            // Each Role can have many associated RoleClaims
            b.HasMany(e => e.RoleClaims)
                .WithOne(e => e.Role)
                .HasForeignKey(rc => rc.RoleId)
                .IsRequired();
        });

        builder.Entity<IdentityClaim>(b =>
        {
            b.Property(p => p.Id)
                .ValueGeneratedOnAdd();
        });

        builder.Entity<Province>(b =>
        {
            b.Property(d => d.Id)
                .ValueGeneratedOnAdd();
        });

        builder.Entity<District>(b =>
        {
            b.Property(d => d.Id)
                .ValueGeneratedOnAdd();
        });

        builder.Entity<Ward>(b =>
        {
            b.Property(d => d.Id)
                .ValueGeneratedOnAdd();
        });
    }
}