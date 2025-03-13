using Hurudza.Common.Services.Interfaces;
using Hurudza.Data.Enums.Enums;
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
        
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    // User Management
    public DbSet<IdentityClaim> Claims { get; set; }

    // Administrative Structure
    public DbSet<Province> Provinces { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<LocalAuthority> LocalAuthorities { get; set; }
    public DbSet<Ward> Wards { get; set; }
    
    // System
    public DbSet<Farm> Farms { get; set; }
    public DbSet<Field> Fields { get; set; }
    public DbSet<Location> Locations { get; set; }
    
    // Farms Ownership
    public DbSet<Entity> Entities { get; set; }
    public DbSet<FarmOwner> FarmOwners { get; set; }
    
    // Crops
    public DbSet<Crop> Crops { get; set; }
    public DbSet<FieldCrop> FieldCrops { get; set; }
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
                        entry.Entity.UpdatedAt = _dateTimeService.Now;
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

            b.HasMany(p => p.Districts)
                .WithOne(p => p.Province)
                .IsRequired(true)
                .HasForeignKey(p => p.ProvinceId)
                .OnDelete(DeleteBehavior.NoAction);
            
            b.HasMany(p => p.Wards)
                .WithOne(p => p.Province)
                .IsRequired(true)
                .HasForeignKey(p => p.ProvinceId)
                .OnDelete(DeleteBehavior.NoAction);
            
            b.HasMany(p => p.Farms)
                .WithOne(p => p.Province)
                .IsRequired(true)
                .HasForeignKey(p => p.ProvinceId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<District>(b =>
        {
            b.Property(d => d.Id)
                .ValueGeneratedOnAdd();
            
            b.Property(d => d.Id)
                .ValueGeneratedOnAdd();

            b.HasMany(d => d.Wards)
                .WithOne(w => w.District)
                .IsRequired(true)
                .HasForeignKey(w => w.DistrictId)
                .OnDelete(DeleteBehavior.NoAction);

            b.HasMany(l => l.LocalAuthorities)
                .WithOne(l => l.District)
                .HasForeignKey(l => l.DistrictId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            b.HasMany(f => f.Farms)
                .WithOne(f => f.District)
                .HasForeignKey(f => f.DistrictId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<LocalAuthority>(b =>
        {
            b.Property(d => d.Id)
                .ValueGeneratedOnAdd();
            
            b.HasMany(a => a.Farms)
                .WithOne(a => a.LocalAuthority)
                .IsRequired(false)
                .HasForeignKey(a => a.LocalAuthorityId)
                .OnDelete(DeleteBehavior.NoAction);

            b.HasMany(w => w.Wards)
                .WithOne(w => w.LocalAuthority)
                .HasForeignKey(w => w.LocalAuthorityId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Ward>(b =>
        {
            b.Property(d => d.Id)
                .ValueGeneratedOnAdd();

            b.HasMany(f => f.Farms)
                .WithOne(f => f.Ward)
                .IsRequired(false)
                .HasForeignKey(f => f.WardId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Farm>(b =>
        {
            b.Property(f => f.Id)
                .ValueGeneratedOnAdd();

            b.Property(f => f.Region)
                .HasDefaultValue(Region.I);

            b.HasMany(f => f.Fields)
                .WithOne(f => f.Farm)
                .HasForeignKey(f => f.FarmId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            b.HasMany(f => f.Locations)
                .WithOne(l => l.Farm)
                .HasForeignKey(l => l.FarmId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Field>(b =>
        {
            b.Property(f => f.Id)
                .ValueGeneratedOnAdd();
            
            b.HasMany(f => f.Locations)
                .WithOne(l => l.Field)
                .HasForeignKey(l => l.FieldId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            b.HasMany(f => f.Crops)
                .WithOne(f => f.Field)
                .HasForeignKey(f => f.FieldId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

        });

        builder.Entity<Location>(b =>
        {
            b.Property(l => l.Id)
                .ValueGeneratedOnAdd();
        });

        builder.Entity<Crop>(b =>
        {
            b.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            b.HasMany(c => c.Fields)
                .WithOne(c => c.Crop)
                .HasForeignKey(c => c.CropId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<SendGridTemplate>(b =>
        {
            b.Property(t => t.Id)
                .ValueGeneratedOnAdd();
        });

        builder.Entity<Entity>(b =>
        {
            b.Property(t => t.Id).ValueGeneratedOnAdd();
        });

        builder.Entity<FarmOwner>(b =>
        {
            b.Property(f => f.Id).ValueGeneratedOnAdd();

            b.HasOne(f => f.Farm).WithMany(f => f.Owners).HasForeignKey(f => f.FarmId).IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);
            b.HasOne(f => f.Entity).WithMany(f => f.FarmOwners).HasForeignKey(f => f.EntityId).IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
}