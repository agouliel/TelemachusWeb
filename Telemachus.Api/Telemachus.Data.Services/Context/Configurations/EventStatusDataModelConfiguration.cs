using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telemachus.Data.Models.Events;

namespace Telemachus.Data.Services.Context.Configurations
{
    public class EventStatusDataModelConfiguration : IEntityTypeConfiguration<EventStatusDataModel>
    {
        public void Configure(EntityTypeBuilder<EventStatusDataModel> builder)
        {
            builder.ToTable("event_statuses");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Name).HasColumnType("varchar(128)");
        }
    }
}
