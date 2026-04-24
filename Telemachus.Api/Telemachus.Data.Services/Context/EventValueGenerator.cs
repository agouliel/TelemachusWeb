using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Telemachus.Data.Services.Context
{

    public class EventValueGenerator : ValueGenerator<string>
    {
        public override bool GeneratesTemporaryValues => false;

        public override string Next(EntityEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }
            var context = (TelemachusContext)entry.Context;
            var userId = entry.CurrentValues.GetValue<string>("UserId");
            var userPrefix = context.Users.Where(_ => _.Id == userId).Select(_ => _.Prefix).Single().ToUpper();
            if (userPrefix == null)
            {
                throw new ArgumentNullException(nameof(userPrefix));
            }
            var lastRecord = context.Events.Where(_ => _.BusinessId.StartsWith(userPrefix)).OrderBy(_ => _.BusinessId.Length).ThenBy(_ => _.BusinessId).IgnoreQueryFilters().LastOrDefault();
            var lastId = lastRecord?.BusinessId;
            string id = lastId == null ?
            userPrefix + "-1"
            : Regex.Replace(lastId, "\\d+", m => (long.Parse(m.Value) + 1).ToString());
            return id;
        }

        public override async ValueTask<string> NextAsync(EntityEntry entry, CancellationToken token = default(CancellationToken))
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }
            var context = (TelemachusContext)entry.Context;
            var userId = entry.CurrentValues.GetValue<string>("UserId");
            var userPrefix = await context.Users.Where(_ => _.Id == userId).Select(_ => _.Prefix).SingleAsync();
            if (userPrefix == null)
            {
                throw new ArgumentNullException(nameof(userPrefix));
            }
            var lastRecord = await context.Events.Where(_ => _.BusinessId.StartsWith(userPrefix!.ToUpper())).OrderBy(_ => _.BusinessId.Length).ThenBy(_ => _.BusinessId).IgnoreQueryFilters().LastOrDefaultAsync();
            var lastId = lastRecord?.BusinessId;
            string id = lastId == null ?
            userPrefix.ToUpper() + "-1"
            : Regex.Replace(lastId, "\\d+", m => (long.Parse(m.Value) + 1).ToString());
            return id;
        }
    }
}
