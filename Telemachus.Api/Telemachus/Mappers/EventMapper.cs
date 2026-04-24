using System.Collections.Generic;
using System.IO;

using Helpers;

using Microsoft.AspNetCore.Http;

using Telemachus.Business.Models.Events.Events;
using Telemachus.Data.Models;
using Telemachus.Models.Events;

namespace Telemachus.Mappers
{
    public static class EventMapper
    {

        public static EventBaseBusinessModel ToBusinessModel(this EventCreateBaseViewModel model, int eventTypeId, string userId = null)
        {
            if (model == null)
            {
                return null;
            }
            return new EventBaseBusinessModel()
            {
                UserId = userId,
                Timestamp = model.Timestamp,
                EventTypeId = eventTypeId,
                Comment = model.Comment,
                PortId = model.PortId,
                Terminal = model.Terminal,
                CustomEventName = Converters.ConvertToTitleCase(model.CustomEventName),
                LatDegrees = model.LatDegrees,
                LatMinutes = model.LatMinutes,
                LatSeconds = model.LatSeconds,
                LongDegrees = model.LongDegrees,
                LongMinutes = model.LongMinutes,
                LongSeconds = model.LongSeconds,
                FuelType = model.FuelType,
                BunkeringDataId = model.BunkeringDataId,
                Lat = model.Lat,
                Lng = model.Lng,
                GradeId = model.GradeId,
                Parcel = model.Parcel,
                CargoId = model.CargoId,
                StsOperation = model.StsOperation
            };
        }
        private static List<FileViewModel> GetFiles(IFormFileCollection files)
        {
            if (files == null)
            {
                return new List<FileViewModel>();
            }
            var result = new List<FileViewModel>();
            foreach (var file in files)
            {
                byte[] data = null;
                if (file == null)
                {
                    continue;
                }

                if (file.Length < 1)
                {
                    continue;
                }
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    data = ms.ToArray();
                }
                result.Add(new FileViewModel()
                {
                    FileName = file.FileName,
                    Data = data
                });
            }

            return result;
        }

        public static EventUpdateBusinessModel ToBusinessModel(this EventUpdateViewModel model, int id)
        {
            if (model == null)
            {
                return null;
            }
            var coords = Converters.DMSToDecimalDegrees(model.LatDegrees, model.LatMinutes, model.LatSeconds, model.LongDegrees, model.LongMinutes, model.LongSeconds);
            return new EventUpdateBusinessModel()
            {
                Id = id,
                Comment = model.Comment,
                PortId = model.PortId,
                Files = GetFiles(model.Files),
                CustomEventName = Converters.ConvertToTitleCase(model.CustomEventName),
                RemoveFileIds = model.RemoveFileIds ?? new List<int>(),
                Timestamp = model.Timestamp,
                LatDegrees = model.LatDegrees,
                LatMinutes = model.LatMinutes,
                LatSeconds = model.LatSeconds,
                LongDegrees = model.LongDegrees,
                LongMinutes = model.LongMinutes,
                LongSeconds = model.LongSeconds,
                Lat = coords?[0],
                Lng = coords?[1],
                GradeId = model.GradeId,
                Parcel = model.Parcel,
                Quantity = model.Quantity,
                CargoId = model.CargoId,
                CargoDetailsId = model.CargoDetailsId,
                StsOperation = model.StsOperation
            };
        }
    }
}
