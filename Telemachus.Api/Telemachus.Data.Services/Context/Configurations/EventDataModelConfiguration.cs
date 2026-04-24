using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telemachus.Data.Models.Events;

namespace Telemachus.Data.Services.Context.Configurations
{
    public class EventDataModelConfiguration : IEntityTypeConfiguration<EventDataModel>
    {
        public void Configure(EntityTypeBuilder<EventDataModel> builder)
        {
            builder.ToTable("events");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.UserId).HasColumnType("nvarchar(450)");
            builder.Property(a => a.Terminal).HasColumnType("varchar(128)");
            builder.HasOne(a => a.User).WithMany(a => a.Events).HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(a => a.EventType).WithMany(a => a.Events).HasForeignKey(a => a.EventTypeId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(a => a.Status).WithMany(a => a.Events).HasForeignKey(a => a.StatusId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(a => a.ParentEvent).WithMany(a => a.ChildrenEvents).HasForeignKey(a => a.ParentEventId).OnDelete(DeleteBehavior.NoAction);
            builder.Property(a => a.CustomEventName).HasColumnType("varchar(128)").IsRequired(false);
            builder.Property(a => a.ExcludeFromStatement).IsRequired(true).HasDefaultValue(false);
            builder.Property(_ => _.BusinessId)
                .HasValueGenerator<EventValueGenerator>();
            builder.Property(s => s.BusinessId).IsRequired(true);
            builder.HasIndex(_ => _.BusinessId).IsUnique(true);
            builder.HasIndex(_ => _.Timestamp);
            builder.HasIndex(_ => _.StatusId);
            builder.HasIndex(_ => _.EventTypeId);
            builder.HasIndex(_ => _.CurrentVoyageConditionKey);
            builder.HasIndex(_ => new { _.UserId, _.Timestamp });
            builder.HasIndex(_ => new { _.UserId, _.CurrentVoyageConditionKey });
            builder.HasOne(_ => _.Port).WithMany(_ => _.Events).HasForeignKey(_ => _.PortId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(e => e.CargoDetail)
                .WithOne(cd => cd.Event)
                .HasForeignKey<EventDataModel>(e => e.CargoDetailId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Property(e => e.Lat)
            .HasColumnType("DECIMAL(9,6)");
            builder.Property(e => e.Lng)
                .HasColumnType("DECIMAL(9,6)");
            builder.HasOne(e => e.BunkeringData).WithMany(e => e.Events)
                .HasForeignKey(e => e.BunkeringDataId)
                .HasPrincipalKey(e => e.Id)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(p => p.Point)
                .HasColumnType("geography")
                .HasComputedColumnSql("CASE WHEN [Lat] IS NULL OR [Lng] IS NULL THEN NULL ELSE GEOGRAPHY::Point([Lat], [Lng], 4326) END PERSISTED");


        }
    }
}
