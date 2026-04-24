using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telemachus.Data.Models.Reports;

namespace Telemachus.Data.Services.Context.Configurations.Reports
{
    public class EventTypeReportFieldDataModelConfiguration : IEntityTypeConfiguration<EventTypeReportFieldDataModel>
    {
        public void Configure(EntityTypeBuilder<EventTypeReportFieldDataModel> builder)
        {
            builder.ToTable("event_type_report_fields");
            builder.HasKey(a => a.Id);
        }
    }
}
