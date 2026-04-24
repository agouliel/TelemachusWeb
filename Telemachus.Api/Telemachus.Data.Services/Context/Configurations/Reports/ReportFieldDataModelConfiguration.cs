using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Telemachus.Data.Models.Reports;

namespace Telemachus.Data.Services.Context.Configurations.Reports
{
    public class ReportFieldDataModelConfiguration : IEntityTypeConfiguration<ReportFieldDataModel>
    {
        public void Configure(EntityTypeBuilder<ReportFieldDataModel> builder)
        {
            builder.ToTable("report_fields");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Name).HasColumnType("varchar(128)");
            builder.Property(a => a.GroupId).HasColumnName("Group").IsRequired(false);
            builder.HasOne(a => a.Group).WithMany().HasForeignKey(a => a.GroupId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);


        }
    }
}
