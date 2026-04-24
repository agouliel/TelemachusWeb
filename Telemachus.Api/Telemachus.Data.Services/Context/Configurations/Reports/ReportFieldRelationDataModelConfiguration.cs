using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telemachus.Data.Models.Reports;

namespace Telemachus.Data.Services.Context.Configurations.Reports
{
    public class ReportFieldRelationDataModelConfiguration : IEntityTypeConfiguration<ReportFieldRelationDataModel>
    {
        public void Configure(EntityTypeBuilder<ReportFieldRelationDataModel> builder)
        {
            builder.ToTable("report_field_relations");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();
            builder.HasOne(a => a.ReportType).WithMany(a => a.AvailableReportFields).HasForeignKey(a => a.ReportTypeId).IsRequired(true).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(a => a.ReportField).WithMany(a => a.ReportRelatedFields).HasForeignKey(a => a.ReportFieldId).IsRequired(true).OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(_ => new { _.ReportFieldId, _.ReportTypeId });
        }
    }
}
