using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telemachus.Data.Models.Events;

namespace Telemachus.Data.Services.Context.Configurations
{
    public class EventTypeDataModelConfiguration : IEntityTypeConfiguration<EventTypeDataModel>
    {
        public void Configure(EntityTypeBuilder<EventTypeDataModel> builder)
        {
            builder.ToTable("event_types");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Name).HasColumnType("varchar(128)");
            builder.Property(a => a.Transit).HasDefaultValue(false);
            builder.HasOne(a => a.PairedEventType).WithMany().HasForeignKey(a => a.PairedEventTypeId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(a => a.ReportType).WithMany().HasForeignKey(a => a.ReportTypeId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(a => a.NextCondition).WithMany(a => a.EventTypes).HasForeignKey(a => a.NextConditionId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(a => a.Prerequisites).WithOne(a => a.EventType).HasForeignKey(a => a.EventTypeId).HasPrincipalKey(a => a.Id).OnDelete(DeleteBehavior.NoAction);
            builder.Property(a => a.OnePairPerTime).HasDefaultValue(false);
        }
    }
}
