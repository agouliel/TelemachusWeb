using System.Collections.Generic;

using Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Telemachus.Data.Models.Reports;

namespace Telemachus.Data.Services.Context.Configurations.Reports
{
    public class FuelTypeDataModelConfiguration : IEntityTypeConfiguration<FuelTypeDataModel>
    {
        public void Configure(EntityTypeBuilder<FuelTypeDataModel> builder)
        {
            builder.ToTable("fuel_types");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.Name).HasColumnType("varchar(512)").IsRequired(true);
            builder.HasData(new List<FuelTypeDataModel>()
            {
                new FuelTypeDataModel()
                {
                    Id = 1,
                    Name = "HFO",
                    BusinessId = ReportType.Hfo
                },
                new FuelTypeDataModel()
                {
                    Id = 2,
                    Name = "MGO",
                    BusinessId = ReportType.Mgo
                }
            });
        }
    }
}
