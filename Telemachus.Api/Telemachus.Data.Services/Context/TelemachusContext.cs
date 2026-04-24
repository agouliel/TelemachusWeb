using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Telemachus.Data.Models;
using Telemachus.Data.Models.Authentication;
using Telemachus.Data.Models.Cargo;
using Telemachus.Data.Models.Events;
using Telemachus.Data.Models.Ports;
using Telemachus.Data.Models.Reports;
using Telemachus.Data.Services.Context.Configurations;
using Telemachus.Data.Services.Context.Configurations.Reports;

namespace Telemachus.Data.Services.Context
{
    public class TelemachusContext : IdentityDbContext<User>
    {
        public TelemachusContext(DbContextOptions<TelemachusContext> options) : base(options)
        { }
        public DbSet<EventDataModel> Events { get; set; }
        public DbSet<EventStatusDataModel> EventStatuses { get; set; }
        public DbSet<EventTypeDataModel> EventTypes { get; set; }
        public DbSet<EventConditionDataModel> EventConditions { get; set; }
        public DbSet<VoyageDataModel> Voyages { get; set; }
        public DbSet<EventTypesConditionsDataModel> EventTypesConditions { get; set; }
        public DbSet<StatementOfFact> StatementOfFact { get; set; }
        public DbSet<EventAttachmentDataModel> EventAttachments { get; set; }
        public DbSet<GradeModel> Grades { get; set; }
        public DbSet<CargoModel> Cargoes { get; set; }
        public DbSet<CargoDetailModel> CargoDetails { get; set; }
        public DbSet<EventTypePrerequisiteDataModel> EventTypePrerequisites { get; set; }
        public DbSet<MrvMisDataModel> MrvMisData { get; set; }
        public DbSet<StsOperation> StsOperations { get; set; }
        public DbSet<ReportContextDataModel> ReportContext { get; set; }

        #region Reports

        public DbSet<BunkeringDataModel> BunkeringData { get; set; }
        public DbSet<BunkeringTankDataModel> BunkeringDataTanks { get; set; }
        public DbSet<ReportDataModel> Reports { get; set; }
        public DbSet<ReportTypeDataModel> ReportTypes { get; set; }
        public DbSet<ReportFieldDataModel> ReportFields { get; set; }
        public DbSet<ReportFieldRelationDataModel> ReportFieldRelations { get; set; }
        public DbSet<ReportFieldValueDataModel> ReportFieldValues { get; set; }
        public DbSet<ReportFieldGroupDataModel> ReportFieldGroups { get; set; }
        public DbSet<TankDataModel> Tanks { get; set; }
        public DbSet<TankUserSpecsDataModel> TankUserSpecs { get; set; }
        public DbSet<UserPasscode> UserPasscodes { get; set; }
        public DbSet<Area> Area { get; set; }
        public DbSet<AreaCoordinate> AreaCoordinate { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Port> Port { get; set; }
        public DbSet<Region> Region { get; set; }
        public DbSet<DocumentType> DocumentType { get; set; }
        public DbSet<FuelTypeDataModel> FuelTypes { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<VoyageConditionQueryDataModel>().HasNoKey().ToView("view_name_that_doesnt_exist");
            builder.ApplyConfiguration(new ReportDataModelConfiguration());
            builder.ApplyConfiguration(new ReportTypeDataModelConfiguration());
            builder.ApplyConfiguration(new ReportFieldDataModelConfiguration());
            builder.ApplyConfiguration(new ReportFieldValueDataModelConfiguration());
            builder.ApplyConfiguration(new ReportFieldRelationDataModelConfiguration());
            builder.ApplyConfiguration(new EventAttachmentDataModelConfiguration());
            builder.ApplyConfiguration(new EventTypesConditionsDataModelConfiguration());
            builder.ApplyConfiguration(new VoyageDataModelConfiguration());
            builder.ApplyConfiguration(new EventConditionDataModelConfiguration());
            builder.ApplyConfiguration(new EventDataModelConfiguration());
            builder.ApplyConfiguration(new EventTypeDataModelConfiguration());
            builder.ApplyConfiguration(new EventStatusDataModelConfiguration());
            builder.ApplyConfiguration(new ReportFieldGroupDataModelConfiguration());
            builder.ApplyConfiguration(new FuelTypeDataModelConfiguration());
            builder.Entity<BunkeringDataModel>().Property(_ => _.BusinessId)
                .HasValueGenerator<BunkeringValueGenerator>();
            builder.Entity<BunkeringTankDataModel>().Property(_ => _.BusinessId)
                .HasValueGenerator<BunkeringTankValueGenerator>();
            builder.Entity<TankDataModel>().HasIndex(_ => new { _.Id, _.Name }).IsUnique(true);
            builder.Entity<TankUserSpecsDataModel>().HasIndex(_ => new { _.UserId, _.TankId }).IsUnique(true);
            builder.Entity<StatementOfFact>().HasOne(_ => _.User)
                .WithMany(_ => _.Statements).HasForeignKey(_ => _.UserId).HasPrincipalKey(_ => _.Id).IsRequired(true);
            builder.Entity<StatementOfFact>().Property(_ => _.FromDate).HasColumnType("Date").IsRequired(true);
            builder.Entity<StatementOfFact>().Property(_ => _.ToDate).HasColumnType("Date").IsRequired(true);
            builder.Entity<StatementOfFact>().Property(a => a.Completed).IsRequired(true).HasDefaultValue(false);
            builder.Entity<StatementOfFact>().Property(_ => _.Date).HasColumnType("Date").IsRequired(false);
            builder.Entity<User>().Property(_ => _.Prefix).HasMaxLength(3).IsRequired(true);
            builder.Entity<User>().HasIndex(_ => _.Prefix).IsUnique(true);
            builder.Entity<StatementOfFact>().Property(_ => _.BusinessId)
            .HasValueGenerator<SofValueGenerator>();
            builder.Entity<StatementOfFact>().Property(s => s.BusinessId).IsRequired(true);
            builder.Entity<StatementOfFact>().HasIndex(_ => _.BusinessId).IsUnique(true);
            builder.Entity<DocumentType>().Property(_ => _.BusinessId)
            .HasValueGenerator<SofValueGenerator>();
            builder.Entity<StsOperation>().Property(_ => _.BusinessId)
            .HasValueGenerator<StsOperationValueGenerator>();
            builder.Entity<DocumentType>().Property(s => s.BusinessId).IsRequired(true);
            builder.Entity<DocumentType>().HasIndex(_ => _.BusinessId).IsUnique(true);
            builder.Entity<StsOperation>()
                .HasIndex(s => s.EventId).IsUnique(true);
            builder.Entity<StsOperation>()
                .HasOne(s => s.Event)
                .WithOne(e => e.StsOperation)
                .HasForeignKey<StsOperation>(s => s.EventId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<StsOperation>()
                .HasOne(s => s.CompanyParticipatingVessel)
                .WithMany()
                .HasForeignKey(s => s.CompanyParticipatingVesselId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<Area>()
                .HasMany(_ => _.AreaCoordinates)
                .WithOne(_ => _.Area)
                .HasForeignKey(_ => _.AreaId)
                .HasPrincipalKey(_ => _.Id);
            builder.Entity<AreaCoordinate>()
                .HasOne(_ => _.Area)
                .WithMany(_ => _.AreaCoordinates)
                .HasForeignKey(_ => _.AreaId)
                .HasPrincipalKey(_ => _.Id);
            builder.Entity<Country>()
                .HasOne(_ => _.Region)
                .WithMany(_ => _.Countries)
                .HasForeignKey(_ => _.RegionId)
                .HasPrincipalKey(_ => _.Id);
            builder.Entity<Country>()
                .HasMany(_ => _.Ports)
                .WithOne(_ => _.Country)
                .HasForeignKey(_ => _.CountryId)
                .HasPrincipalKey(_ => _.Id);
            builder.Entity<Region>()
                .HasMany(_ => _.Countries)
                .WithOne(_ => _.Region)
                .HasForeignKey(_ => _.RegionId)
                .HasPrincipalKey(_ => _.Id);
            builder.Entity<Region>()
                .HasMany(_ => _.Ports)
                .WithOne(_ => _.Region)
                .HasForeignKey(_ => _.RegionId)
                .HasPrincipalKey(_ => _.Id);
            builder.Entity<Region>()
                .HasOne(_ => _.Area)
                .WithMany(a => a.Regions)
                .HasForeignKey(_ => _.AreaId)
                .HasPrincipalKey(_ => _.Id);
            builder.Entity<Area>().Property(_ => _.DateModified)
                .IsRequired(true);
            builder.Entity<AreaCoordinate>().Property(_ => _.DateModified)
                .IsRequired(true);
            builder.Entity<Country>().Property(_ => _.DateModified)
                .IsRequired(true);
            builder.Entity<Port>().Property(_ => _.DateModified)
                .IsRequired(true);
            builder.Entity<Region>().Property(_ => _.DateModified)
                .IsRequired(true);

            builder.Entity<Area>().Property(_ => _.BusinessId)
                .IsRequired(true);
            builder.Entity<AreaCoordinate>().Property(_ => _.BusinessId)
                .IsRequired(true);
            builder.Entity<Country>().Property(_ => _.BusinessId)
                .IsRequired(true);
            builder.Entity<Port>().Property(_ => _.BusinessId)
                .IsRequired(true);
            builder.Entity<Region>().Property(_ => _.BusinessId)
                .IsRequired(true);

            builder.Entity<Area>()
                .HasIndex(_ => _.BusinessId)
                .IsUnique(true);
            builder.Entity<AreaCoordinate>()
                .HasIndex(_ => _.BusinessId)
                .IsUnique(true);
            builder.Entity<Country>()
                .HasIndex(_ => _.BusinessId)
                .IsUnique(true);
            builder.Entity<Port>()
                .HasIndex(_ => _.BusinessId)
                .IsUnique(true);
            builder.Entity<Region>()
                .HasIndex(_ => _.BusinessId)
                .IsUnique(true);

            builder.Entity<Area>()
                .HasIndex(_ => _.DateModified)
                .IsUnique(false);
            builder.Entity<AreaCoordinate>()
                .HasIndex(_ => _.DateModified)
                .IsUnique(false);
            builder.Entity<Country>()
                .HasIndex(_ => _.DateModified)
                .IsUnique(false);
            builder.Entity<Port>()
                .HasIndex(_ => _.DateModified)
                .IsUnique(false);
            builder.Entity<Region>()
                .HasIndex(_ => _.DateModified)
                .IsUnique(false);

            builder.Entity<StatementOfFact>().HasOne(_ => _.Port).WithMany(_ => _.StatementOfFacts).HasForeignKey(_ => _.PortId).OnDelete(DeleteBehavior.SetNull);


            builder.Entity<Port>().Property(e => e.Latitude)
                .HasColumnType("DECIMAL(9,6)");
            builder.Entity<Port>().Property(e => e.Longitude)
                .HasColumnType("DECIMAL(9,6)");


            builder.Entity<BunkeringTankDataModel>().HasOne(a => a.BunkeringData).WithMany(a => a.Tanks).HasForeignKey(_ => _.BunkeringDataId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<BunkeringTankDataModel>().HasIndex(_ => new { _.BunkeringDataId, _.TankId }).IsUnique(true);
            builder.Entity<BunkeringTankDataModel>().HasOne(_ => _.Tank).WithMany().HasForeignKey(_ => _.TankId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<BunkeringTankDataModel>().HasOne(_ => _.ComminglingData).WithMany().HasForeignKey(_ => _.ComminglingId).OnDelete(DeleteBehavior.NoAction);


            builder.Entity<BunkeringDataModel>().HasMany(b => b.Events).WithOne(b => b.BunkeringData)
                .HasForeignKey(b => b.BunkeringDataId)
                .HasPrincipalKey(b => b.Id)
                .OnDelete(DeleteBehavior.Restrict);



            builder.Entity<Port>().Property(p => p.Point)
                .HasColumnType("geography")
                .HasComputedColumnSql("CASE WHEN [Latitude] IS NULL OR [Longitude] IS NULL THEN NULL ELSE GEOGRAPHY::Point([Latitude], [Longitude], 4326) END PERSISTED");


            builder.Entity<CargoModel>().Property(a => a.BusinessId).HasValueGenerator<CargoValueGenerator>();
            builder.Entity<CargoDetailModel>().Property(a => a.BusinessId).HasValueGenerator<CargoDetailValueGenerator>();
            builder.Entity<CargoModel>().Property(s => s.BusinessId).IsRequired(true);
            builder.Entity<CargoDetailModel>().HasIndex(_ => _.BusinessId).IsUnique(true);
            builder.Entity<CargoModel>().HasIndex(a => a.StartedOn).IsUnique(false);
            builder.Entity<CargoModel>().HasIndex(a => a.CompletedOn).IsUnique(false);
            builder.Entity<CargoDetailModel>().HasIndex(a => a.Timestamp).IsUnique(false);

            builder.Entity<TankUserSpecsDataModel>().Property(a => a.IsActive).HasDefaultValue(true);


            builder.Entity<CargoModel>()
                .HasMany(a => a.CargoDetails)
                .WithOne(a => a.Cargo)
                .HasForeignKey(_ => _.CargoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CargoDetailModel>()
                .HasOne(cd => cd.Cargo)
                .WithMany(c => c.CargoDetails)
                .HasForeignKey(cd => cd.CargoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CargoDetailModel>()
                .HasOne(cd => cd.Event)
                .WithOne(e => e.CargoDetail)
                .HasForeignKey<EventDataModel>(e => e.CargoDetailId)
                .HasPrincipalKey<CargoDetailModel>(e => e.Id)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<TankDataModel>()
                .HasMany(t => t.ReportFields)
                .WithOne(t => t.Tank)
                .HasForeignKey(t => t.TankId)
                .HasPrincipalKey(t => t.Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EventTypePrerequisiteDataModel>().HasIndex(_ => new { _.EventTypeId, _.AvailableAfterEventTypeId }).IsUnique(true);

            builder.Entity<EventAttachmentDataModel>().Property("FileSize").HasDefaultValue(false);
            builder.Entity<EventDataModel>().Property("HiddenDate").HasDefaultValue(false);
            builder.Entity<TankUserSpecsDataModel>().Property("DisplayOrder").HasDefaultValue(false);
            builder.Entity<TankDataModel>().Property("Storage").HasDefaultValue(true);
            builder.Entity<TankDataModel>().Property("Settling").HasDefaultValue(false);
            builder.Entity<TankDataModel>().Property("Serving").HasDefaultValue(false);

            builder.Entity<TankDataModel>().HasOne(g => g.FuelType)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey(g => g.FuelTypeId)
                .HasPrincipalKey(g => g.Id)
                .OnDelete(DeleteBehavior.Restrict);

            var entityTypes = builder.Model.GetEntityTypes();

            foreach (var entityType in entityTypes)
            {
                var entityBuilder = builder.Entity(entityType.ClrType);

                if (typeof(EntityBase).IsAssignableFrom(entityType.ClrType))
                {


                    entityBuilder.HasIndex("IsDeleted");
                    entityBuilder.HasIndex("DateModified");

                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, "IsDeleted");
                    var filter = Expression.Lambda(Expression.Not(property), parameter);
                    entityBuilder.HasQueryFilter(filter);

                    entityBuilder.Property("DateModified")
                        .HasDefaultValueSql("GETUTCDATE()");
                    entityBuilder.Property("IsDeleted")
                        .HasDefaultValue(false);
                }


                builder.Entity<ReportContextDataModel>()
                    .HasOne(t => t.Report)
                    .WithMany(t => t.ReportContext)
                    .IsRequired(true)
                    .OnDelete(DeleteBehavior.NoAction)
                    ;

                //builder.Entity<ReportContextDataModel>()
                //    .HasIndex(r => new { r.ReportId, r.TankId, r.GroupId })
                //    .IsUnique(true);

                builder.Entity<ReportContextDataModel>()
                    .HasIndex("DateModified")
                    ;

                builder.Entity<ReportContextDataModel>()
                    .Property("DateModified")
                    .HasDefaultValueSql("GETUTCDATE()");

                if (typeof(EntityMaster).IsAssignableFrom(entityType.ClrType))
                {

                    entityBuilder.Property("BusinessId")
                        .HasDefaultValueSql("newid()");
                }

                var businessIdProperty = entityType.FindProperty("BusinessId");
                if (businessIdProperty != null)
                {
                    entityBuilder.HasIndex("BusinessId").IsUnique();
                }
            }



            base.OnModelCreating(builder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SoftDeleteInterceptor interceptor = new SoftDeleteInterceptor(SoftDeleteEnabled, UpdateTimestamps);
            interceptor.OnSavingChanges(this);
            return base.SaveChangesAsync(cancellationToken);

        }
        public override int SaveChanges()
        {
            SoftDeleteInterceptor interceptor = new SoftDeleteInterceptor(SoftDeleteEnabled, UpdateTimestamps);
            interceptor.OnSavingChanges(this);
            return base.SaveChanges();
        }

        public bool UpdateTimestamps { get; set; } = true;
        public bool SoftDeleteEnabled { get; set; } = true;
    }
}
