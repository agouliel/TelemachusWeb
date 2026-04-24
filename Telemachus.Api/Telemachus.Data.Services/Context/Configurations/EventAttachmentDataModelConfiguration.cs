using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telemachus.Data.Models.Events;

namespace Telemachus.Data.Services.Context.Configurations
{
    public class EventAttachmentDataModelConfiguration : IEntityTypeConfiguration<EventAttachmentDataModel>
    {
        public void Configure(EntityTypeBuilder<EventAttachmentDataModel> builder)
        {
            builder.ToTable("event_attachments");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();
            builder.HasOne(a => a.Event).WithMany(a => a.Attachments).HasForeignKey(a => a.EventId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(a => a.Report).WithMany(a => a.Attachments).HasForeignKey(a => a.ReportId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(a => a.ReportField).WithMany().HasForeignKey(a => a.ReportFieldId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(a => a.DocumentType).WithMany().HasForeignKey(a => a.DocumentTypeId).OnDelete(DeleteBehavior.SetNull);
            builder.Property(_ => _.BusinessId)
            .HasValueGenerator<AttachmentValueGenerator>();
            builder.Property(s => s.BusinessId).IsRequired(true);
            builder.HasIndex(_ => _.BusinessId).IsUnique(true);
        }
    }
}
