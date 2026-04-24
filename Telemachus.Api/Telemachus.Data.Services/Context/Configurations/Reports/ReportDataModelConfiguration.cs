using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telemachus.Data.Models.Reports;

namespace Telemachus.Data.Services.Context.Configurations.Reports
{
    public class ReportDataModelConfiguration : IEntityTypeConfiguration<ReportDataModel>
    {
        public void Configure(EntityTypeBuilder<ReportDataModel> builder)
        {
            builder.ToTable("reports");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();
            builder.HasOne(a => a.Event).WithMany(a => a.Reports).HasForeignKey(a => a.EventId).OnDelete(DeleteBehavior.Cascade);
            builder.Property(_ => _.BusinessId)
            .HasValueGenerator<ReportValueGenerator>();
            builder.Property(s => s.BusinessId).IsRequired(true);
            builder.HasIndex(_ => _.BusinessId).IsUnique(true);
            builder.Property(r => r.CreatedDate).IsRequired(false).HasDefaultValue(null);

        }
    }
}
