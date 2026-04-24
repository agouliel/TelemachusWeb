using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telemachus.Data.Models.Events;

namespace Telemachus.Data.Services.Context.Configurations
{
    public class EventTypesConditionsDataModelConfiguration : IEntityTypeConfiguration<EventTypesConditionsDataModel>
    {
        public void Configure(EntityTypeBuilder<EventTypesConditionsDataModel> builder)
        {
            builder.ToTable("event_types_conditions");
            builder.HasKey(a => a.Id);
            builder.HasOne(a => a.EventType).WithMany(a => a.EventTypesConditions).HasForeignKey(a => a.EventTypeId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(a => a.EventCondition).WithMany(a => a.EventTypesConditions).OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(a => a.ConditionId);
        }
    }
}
