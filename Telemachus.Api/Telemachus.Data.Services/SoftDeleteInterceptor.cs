using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Telemachus.Data.Models;

namespace Telemachus.Data.Services
{
    public class SoftDeleteInterceptor
    {
        private bool UpdateTimestamps { get; set; } = true;
        private bool Enabled { get; set; } = true;
        public SoftDeleteInterceptor(bool enabled = true, bool updateTimestamps = true)
        {
            UpdateTimestamps = updateTimestamps;
            Enabled = enabled;
        }
        public void OnSavingChanges(DbContext context)
        {
            var filters = new List<EntityState>() { EntityState.Deleted, EntityState.Modified, EntityState.Added };
            var entries = context.ChangeTracker.Entries().Where(e => filters.Contains(e.State) && e.Entity is EntityBase).ToList();

            foreach (var entry in entries)
            {
                if (Enabled && entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.CurrentValues["IsDeleted"] = true;
                    entry.CurrentValues["DateModified"] = DateTime.UtcNow;
                    //foreach (var navigationEntry in entry.Navigations.Where(n => !n.Metadata.IsDependentToPrincipal()))
                    //{
                    //    if (navigationEntry is CollectionEntry collectionEntry)
                    //    {
                    //        if (collectionEntry.CurrentValue == null) continue;
                    //        foreach (var dependentEntry in collectionEntry.CurrentValue)
                    //        {
                    //            EntityEntry e = (EntityEntry)dependentEntry;
                    //            e.CurrentValues["IsDeleted"] = true;
                    //            e.CurrentValues["DateModified"] = DateTime.UtcNow;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (navigationEntry.CurrentValue == null) continue;
                    //        var dependentEntry = navigationEntry.CurrentValue;
                    //        EntityEntry e = (EntityEntry)dependentEntry;
                    //        e.CurrentValues["IsDeleted"] = true;
                    //        e.CurrentValues["DateModified"] = DateTime.UtcNow;
                    //    }
                    //}
                }
                else if (UpdateTimestamps && (entry.State == EntityState.Added || entry.State == EntityState.Modified))
                {
                    entry.CurrentValues["DateModified"] = DateTime.UtcNow;
                }

            }
        }
    }
}
