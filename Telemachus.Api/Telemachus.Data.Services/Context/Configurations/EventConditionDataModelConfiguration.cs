using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telemachus.Data.Models.Events;

namespace Telemachus.Data.Services.Context.Configurations
{
    public class EventConditionDataModelConfiguration : IEntityTypeConfiguration<EventConditionDataModel>
    {
        public void Configure(EntityTypeBuilder<EventConditionDataModel> builder)
        {
            builder.ToTable("event_conditions");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Name).HasColumnType("varchar(256)").IsRequired();
            builder.HasMany(a => a.Events).WithOne(a => a.EventCondition).HasForeignKey(a => a.ConditionId).OnDelete(DeleteBehavior.Cascade);


        }
    }
}
