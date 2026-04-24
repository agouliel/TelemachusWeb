using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Telemachus.Data.Models.Reports;

namespace Telemachus.Data.Services.Context.Configurations.Reports
{
    public class ReportFieldValueDataModelConfiguration : IEntityTypeConfiguration<ReportFieldValueDataModel>
    {
        public void Configure(EntityTypeBuilder<ReportFieldValueDataModel> builder)
        {
            builder.ToTable("report_field_values");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.Value).HasColumnType("varchar(512)");
            builder.HasOne(a => a.Report).WithMany(a => a.FieldValues).HasForeignKey(a => a.ReportId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(a => a.ReportField).WithMany(a => a.FieldValues).HasForeignKey(a => a.ReportFieldId).OnDelete(DeleteBehavior.Cascade);
            builder.Property(_ => _.BusinessId)
            .HasValueGenerator<ReportFieldValueGenerator>();
            builder.Property(s => s.BusinessId).IsRequired(true);
            builder.HasIndex(_ => _.BusinessId).IsUnique(true);
            builder.HasOne(f => f.ReportContext)
                .WithMany(f => f.ReportFieldValues)
                .HasForeignKey(f => f.ReportContextId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.ClientSetNull);

        }
    }
}
