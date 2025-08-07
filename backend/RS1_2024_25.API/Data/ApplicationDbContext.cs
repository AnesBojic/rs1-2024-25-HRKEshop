using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Extensions.DependencyInjection;
using RS1_2024_25.API.Data.Models.SharedTables;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul1_Auth;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Helper.BaseClasses;
using RS1_2024_25.API.Services;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

namespace RS1_2024_25.API.Data;

public class ApplicationDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : DbContext(options)
{
    
   
    public DbSet<Color> Colors { get; set; }
    public DbSet<AcademicYear> AcademicYears { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Municipality> Municipalities { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Role> Roles { get; set; }



    public DbSet<categories_products> Categories_ProductsAll { get; set; }
    public DbSet<Category> CategoryAll { get; set; }
    public DbSet<SizeType> SizeTypesAll { get; set; }
    public DbSet<AppUser> AppUsersAll { get; set; }
    public DbSet<MyAppUser> MyAppUsersAll { get; set; }
    public DbSet<MyAuthenticationToken> MyAuthenticationTokensAll { get; set; }
    public DbSet<Department> DepartmentsAll { get; set; }
    public DbSet<Faculty> FacultiesAll { get; set; }
    public DbSet<Professor> ProfessorsAll { get; set; }
    public DbSet<Student> StudentsAll { get; set; }

    public DbSet<Product> ProductsAll { get; set; }
    public DbSet<Brand> BrandsAll { get; set; }

    public DbSet<Image> ImagesAll { get; set; }

    public DbSet<Order> OrdersAll { get; set; }

    public DbSet<OrderItem> OrderItemsAll { get; set; }

    public DbSet<ProductSize> ProductsSizesAll { get; set; }

    public DbSet<Size> SizesAll { get; set; }

    public DbSet<Favorite> FavoritesAll { get; set; }

    public DbSet<ProductRating> ProductRatingsAll { get; set; }

    public DbSet<EmailVerificationToken> EmailVerificationTokensAll { get; set; }
    // IQueryable umjesto DbSet

    public IQueryable<categories_products> CategoriesProducts => Set<categories_products>().Where(e => e.TenantId == CurrentTenantIdThrowIfFail);
    public IQueryable<Category> Categories => Set<Category>().Where(e => e.TenantId == CurrentTenantIdThrowIfFail);
    public IQueryable<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>().Where(e => e.TenantId == CurrentTenantIdThrowIfFail);
    public IQueryable<ProductRating> ProductRatings => Set<ProductRating>().Where(e => e.TenantId == CurrentTenantIdThrowIfFail);
    public IQueryable<Favorite> Favorites => Set<Favorite>().Where(e => e.TenantId == CurrentTenantIdThrowIfFail);
    public IQueryable<SizeType> SizeTypes => Set<SizeType>().Where(e => e.TenantId == CurrentTenantIdThrowIfFail);
    public IQueryable<Size> Sizes => Set<Size>().Where(e => e.TenantId == CurrentTenantIdThrowIfFail);
    public IQueryable<ProductSize> ProductSizes => Set<ProductSize>().Where(e => e.TenantId == CurrentTenantIdThrowIfFail);
    public IQueryable<OrderItem> OrderItems => Set<OrderItem>().Where(e => e.TenantId == CurrentTenantIdThrowIfFail);
    public IQueryable<Order> Orders => Set<Order>().Where(e => e.TenantId == CurrentTenantIdThrowIfFail);
    public IQueryable<Image> Images => Set<Image>().Where(e => e.TenantId == CurrentTenantIdThrowIfFail);
    public IQueryable<Product> Products => Set<Product>().Where(e => e.TenantId == CurrentTenantIdThrowIfFail);
    public IQueryable<Brand> Brands => Set<Brand>().Where(e => e.TenantId == CurrentTenantIdThrowIfFail);
    public IQueryable<AppUser> AppUsers => Set<AppUser>().Where(e=> e.TenantId == CurrentTenantIdThrowIfFail);
    public IQueryable<MyAppUser> MyAppUsers => Set<MyAppUser>().Where(e => e.TenantId == CurrentTenantIdThrowIfFail);
    public IQueryable<MyAuthenticationToken> MyAuthenticationTokens => Set<MyAuthenticationToken>();
    public IQueryable<Department> Departments => Set<Department>().Where(e => e.TenantId == CurrentTenantIdThrowIfFail);
    public IQueryable<Faculty> Faculties => Set<Faculty>().Where(e => e.TenantId == CurrentTenantIdThrowIfFail);
    public IQueryable<Professor> Professors => Set<Professor>().Where(e => e.TenantId == CurrentTenantIdThrowIfFail);
    public IQueryable<Student> Students => Set<Student>().Where(e => e.TenantId == CurrentTenantIdThrowIfFail);

    #region METHODS
    public int? _CurrentTenantId = null;

    public int CurrentTenantIdThrowIfFail
    {
        get
        {
            var result = CurrentTenantId;
            if (result == null || result == 0)
            {
                //for appuser so it has defaul tenant when there are no claims
                var path = httpContextAccessor.HttpContext?.Request.Path.Value ?? "";
                if(path.StartsWith("/appusers/add",StringComparison.OrdinalIgnoreCase))
                {
                    return 1;
                }

                throw new UnauthorizedAccessException();
            }

            return result.Value;
        }
    }
    public int? CurrentTenantId
    {
        get
        {
            if (_CurrentTenantId == null)
            {
                //Changing use for claim
                /*MyAuthInfo myAuthInfo = MyAuthServiceHelper.GetAuthInfoFromRequest(this, httpContextAccessor);
                _CurrentTenantId = myAuthInfo.TenantId;*/

                var user = httpContextAccessor.HttpContext?.User;
                if(user == null || !user.Identity?.IsAuthenticated == true)
                {
                    return null;
                }

                var tenantClaim = user.Claims.FirstOrDefault(c=> c.Type == "tenant_id");
                if(tenantClaim == null)
                {
                    return null;
                }
                if(int.TryParse(tenantClaim.Value, out int tenantId))
                {
                    _CurrentTenantId = tenantId;
                }


            }
            return _CurrentTenantId;
        }
        set
        {
            _CurrentTenantId = value;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.NoAction;
        }

        // opcija kod nasljeđivanja
        // modelBuilder.Entity<NekaBaznaKlasa>().UseTpcMappingStrategy();

        // Iteracija kroz sve entitete u modelu
        // U EF-u defaultno naziv tabele je jednak nazivu dbseta.
        // S obzirom što smo izmjenili nazive dbsetova zbog tenanata i zbog dodatnih queryable
        // onda u narednoj petlji postavljamo da nazivi tabela budu nazivi atributa "table"
        // Ako nema atributa "table" onda se koristi naziv klase.

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            // Provjera da li postoji [Table("TblNekoIme")] atribut
            var tableAttribute = clrType.GetCustomAttributes(typeof(TableAttribute), inherit: false)
                                        .FirstOrDefault() as TableAttribute;

            if (tableAttribute == null)
            {
                // Ako nema TableAttribute, automatski pluralizuj naziv tabele
                var tableName = clrType.Name.Pluralize();
                modelBuilder.Entity(clrType).ToTable(tableName);
            }
            else
            {
                // Ako postoji TableAttribute, koristi navedeni naziv tabele
                modelBuilder.Entity(clrType).ToTable(tableAttribute.Name);
            }
        }
    }
    //Fix for updatedAt not working properly
    //fix
    private void UpdateAuditFields()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;

            }
            else if (entry.State == EntityState.Modified || (entry.State == EntityState.Unchanged && entry.Properties.Any(p => p.IsModified)))
            {
                
                entry.Entity.UpdatedAt = now;
            }

        }
    }
 
    public override int SaveChanges()
    {
        UpdateAuditFields();
        AddTenantIdToNewEntities();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        AddTenantIdToNewEntities();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void AddTenantIdToNewEntities()
    {
        // Iteracija kroz sve promjene u DbContext
        foreach (var entry in ChangeTracker.Entries()
                     .Where(e => e.State == EntityState.Added && e.Entity is TenantSpecificTable))
        {
            // Postavljanje TenantId za nove entitete
            var entity = (TenantSpecificTable)entry.Entity;

            //Postavljanje tenantId samo u slucaju ako nije hardkodiran, kako bi seedani testovi bili bolji ,
            if (entity.TenantId == 0)
            {
                entity.TenantId = CurrentTenantIdThrowIfFail;
            }
        }
    }

    public int GetUserIdThrow()
    {
        return int.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    }

    #endregion
}
