using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telemachus.Data.Models.Reports;

namespace Telemachus.Data.Services.Context.Configurations.Reports
{
    public class ReportTypeDataModelConfiguration : IEntityTypeConfiguration<ReportTypeDataModel>
    {
        public void Configure(EntityTypeBuilder<ReportTypeDataModel> builder)
        {
            builder.ToTable("report_types");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.Name).HasColumnType("varchar(128)");

        }
    }
}
