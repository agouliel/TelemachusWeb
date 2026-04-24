using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telemachus.Data.Models.Events;

namespace Telemachus.Data.Services.Context.Configurations
{
    public class VoyageDataModelConfiguration : IEntityTypeConfiguration<VoyageDataModel>
    {
        public void Configure(EntityTypeBuilder<VoyageDataModel> builder)
        {
            builder.ToTable("voyages");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.UserId).HasColumnType("nvarchar(450)");
            builder.HasOne(a => a.User).WithMany(a => a.Voyages).HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(a => a.CurrentCondition).WithMany(a => a.Voyages).HasForeignKey(a => a.CurrentConditionId).OnDelete(DeleteBehavior.Restrict);
            builder.Property(_ => _.BusinessId)
            .HasValueGenerator<VoyageValueGenerator>();
            builder.Property(s => s.BusinessId).IsRequired(true);
            builder.HasIndex(_ => _.BusinessId).IsUnique(true);
        }
    }
}
