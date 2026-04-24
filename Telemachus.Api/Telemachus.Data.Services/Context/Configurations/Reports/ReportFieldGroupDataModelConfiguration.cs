using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Telemachus.Data.Models.Reports;

namespace Telemachus.Data.Services.Context.Configurations.Reports
{
    public class ReportFieldGroupDataModelConfiguration : IEntityTypeConfiguration<ReportFieldGroupDataModel>
    {
        public void Configure(EntityTypeBuilder<ReportFieldGroupDataModel> builder)
        {
            builder.ToTable("report_field_groups");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.FieldGroupName).HasColumnType("varchar(512)").IsRequired(true);
            builder.HasOne(g => g.FuelType)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey(g => g.FuelTypeId)
                .HasPrincipalKey(g => g.Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
