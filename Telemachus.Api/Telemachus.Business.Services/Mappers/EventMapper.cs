using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Telemachus.Business.Models;
using Telemachus.Business.Models.Events;
using Telemachus.Business.Models.Events.Events;
using Telemachus.Data.Models;
using Telemachus.Data.Models.Cargo;
using Telemachus.Data.Models.DataTransferModels;
using Telemachus.Data.Models.Events;
using Telemachus.Data.Models.Ports;

namespace Telemachus.Business.Services.Mappers
{
    public static class EventMapper
    {
        private static string ConvertToTitleCase(string value)
        {
            if (value == null)
                return null;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(value.ToLower().Trim());
        }
        public static EventBusinessModel ToBusinessModel(this EventDataModel model)
        {
            if (model == null)
            {
                return null;
            }

            return new EventBusinessModel()
            {
                UserId = model.UserId,
                PairedEventTypeId = model.EventType?.PairedEventTypeId,
                ConditionName = model.EventCondition?.Name,
                ConditionBusinessId = model.EventCondition?.BusinessId.ToString(),
                EventTypeName = model.EventType?.Name,
                ReportTypeId = model.EventType?.ReportTypeId,
                Id = model.Id,
                StatusId = model.StatusId,
                StatusBusinessId = model.Status?.BusinessId.ToString(),
                Timestamp = model.Timestamp,
                VoyageId = model.VoyageId,
                VoyageBusinessId = model.Voyage?.BusinessId.ToString(),
                Comment = model.Comment,
                PortId = model.PortId,
                PortName = model.Port?.Name,
                PortCountry = model.Port?.Country?.Alpha2,
                PortIsEu = model.Port?.IsEuInt,
                PortBusinessId = model.Port?.BusinessId.ToString(),
                Terminal = model.Terminal,
                Attachments = model.Attachments?.ToBusinessModel(),
                EventTypeId = model.EventTypeId,
                BunkeringDataId = model.BunkeringDataId,
                BunkeringData = model.BunkeringData?.ToBusinessModel(),
                CustomEventName = model.CustomEventName,
                ReportId = model.Reports?.FirstOrDefault()?.Id,
                ParentEventId = model.ParentEventId,
                LongDegrees = model.LongDegrees,
                LongMinutes = model.LongMinutes,
                LongSeconds = model.LongSeconds,
                LatDegrees = model.LatDegrees,
                LatMinutes = model.LatMinutes,
                LatSeconds = model.LatSeconds,
                Lat = model.Lat,
                Lng = model.Lng,
                EventTypeBusinessId = model.EventType?.BusinessId,
                BusinessId = model.BusinessId,
                ParentEventBusinessId = model.ParentEvent?.BusinessId,
                CargoDetailId = model.CargoDetailId,
                CargoDetails = model.CargoDetail?.ToBusinessModel(),
                ParentEvent = model.ParentEvent?.ToBusinessModel(),
                Cargoes = model.Cargoes?.ToBusinessModel(),
                UserName = model.User?.UserName,
                StsOperation = model.StsOperation?.ToBusinessModel()
            };
        }

        public static CargoDetailModel ToCargoDetailsDataModel(this EventBaseBusinessModel model, string userId)
        {
            if ((!model.GradeId.HasValue || !model.Parcel.HasValue))
                return null;

            return new CargoDetailModel()
            {
                CargoId = model.CargoId ?? 0,
                Cargo = new CargoModel()
                {
                    Id = model.CargoId ?? 0,
                    UserId = userId,
                    Parcel = model.Parcel.Value,
                    GradeId = model.GradeId.Value
                }
            };
        }

        public static CargoDetailModel ToCargoDetailsDataModel(this EventUpdateBusinessModel model, string userId)
        {
            if (!model.CargoDetailsId.HasValue && !model.CargoId.HasValue)
                return null;

            return new CargoDetailModel()
            {
                Id = model.CargoDetailsId ?? 0,
                CargoId = model.CargoId.Value,
                Quantity = model.Quantity,
                Cargo = new CargoModel()
                {
                    Id = model.CargoId.Value,
                    UserId = userId,
                    Parcel = model.Parcel.Value,
                    GradeId = model.GradeId.Value
                }
            };
        }

        public static List<EventAttachmentBusinessModel> ToBusinessModel(this IEnumerable<EventAttachmentDataModel> models)
        {
            return models.Select(ToBusinessModel).ToList();
        }

        public static BunkeringDataBusinessModel ToBusinessModel(this BunkeringDataModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new BunkeringDataBusinessModel()
            {
                Id = model.Id,
                Bdn = model.Bdn?.ToUpper(),
                SulphurContent = model.SulphurContent,
                Density = model.Density,
                Viscosity = model.Viscosity,
                FuelType = model.FuelType,
                Supplier = model.Supplier?.ToUpper(),
                TotalAmount = model.TotalAmount,
                RobAmount = model.RobAmount,
                RobAmountDiff = model.RobAmountDiff,
                RobAmountDiffTimestamp = model.RobAmountDiffTimestamp,
                NamedAmount = model.NamedAmount,
                PortId = model.PortId,
                PortName = model.Port?.Name,
                PortCountry = model.Port?.Country?.Alpha2,
                Timestamp = model.Timestamp,
                BusinessId = model.BusinessId,
                UserId = model.UserId,
                Tanks = model.Tanks?.Select(a => a.ToBusinessModel()).ToList()
            };
        }

        public static BunkeringTankBusinessModel ToBusinessModel(this BunkeringTankDataModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new BunkeringTankBusinessModel()
            {
                Id = model.Id,
                BunkeringDataId = model.BunkeringDataId,
                TankId = model.TankId,
                Amount = model.Amount,
                ComminglingId = model.ComminglingId,
                BusinessId = model.BusinessId,
                ComminglingData = model.ComminglingData?.ToBusinessModel()
            };
        }

        public static EventAttachmentBusinessModel ToBusinessModel(this EventAttachmentDataModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new EventAttachmentBusinessModel()
            {
                Id = model.Id,
                FileName = model.FileName,
                MimeType = model.MimeType,
                FileSize = model.FileSize,
                BunkeringDataId = model.BunkeringDataId,
                EventId = model.EventId,
                ReportId = model.ReportId,
                ReportFieldId = model.ReportFieldId,
                DocumentTypeId = model.DocumentTypeId,
                DocumentType = model.DocumentType?.ToBusinessModel(),
                FilePath = model.FilePath
            };
        }

        public static DocumentTypeBusinessModel ToBusinessModel(this DocumentType model)
        {

            if (model == null)
            {
                return null;
            }
            return new DocumentTypeBusinessModel()
            {
                Id = model.Id,
                Name = model.Name,
                Code = model.Code
            };
        }

        public static EventConditionBusinessModel ToBusinessModel(this EventConditionDataModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new EventConditionBusinessModel()
            {
                Id = model.Id,
                BusinessId = model.BusinessId,
                Name = model.Name
            };
        }
        public static PortBusinessModel ToBusinessModel(this Port model)
        {
            if (model == null)
            {
                return null;
            }
            return new PortBusinessModel()
            {
                Id = model.Id,
                BusinessId = model.BusinessId.ToLower(),
                PortCode = model.Code,
                PortName = model.Name,
                AreaName = model.Country?.Region?.Area?.Name,
                RegionName = model.Country?.Region?.Name,
                CountryName = model.Country?.Name,
                CountryCode = model.Country?.Alpha2,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Distance = model.Distance
            };
        }

        public static DocumentFactViewModel AsDocumentViewModel(this EventDataModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new DocumentFactViewModel()
            {
                Id = model.Id,
                Name = model.EventType.Name,
                Remarks = model.Comment,
                Timestamp = model.Timestamp,
                Terminal = model.Terminal,
                VoyageId = model.Voyage.Id,
                EventTypeId = model.EventTypeId,
                StatusId = model.StatusId,
                EventTypeFormatted = model.EventType.Name,
                StatusFormatted = model.Status.Name,
                CustomEventName = model.CustomEventName,
                Excluded = model.ExcludeFromStatement,
                HiddenDate = model.HiddenDate,
                ConditionName = model.EventCondition.Name,
                PortId = model.PortId
            };
        }
        public static EventMarker AsEventMarker(this EventDataModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new EventMarker()
            {
                Id = model.Id,
                Timestamp = model.Timestamp,
                VoyageId = model.VoyageId,
                ConditionId = model.ConditionId,
                ConditionName = model.EventCondition.Name
            };
        }

        public static ConditionEventsBusinessModel ToBusinessModel(this ConditionEventsDataModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new ConditionEventsBusinessModel()
            {
                ConditionId = model.ConditionId,
                ConditionKey = model.ConditionKey,
                //IsCurrentCondition = model.IsCurrentCondition,
                //VoyageId = model.VoyageId,
                ConditionName = model.ConditionName,
                EndDate = model.EndDate,
                Events = model.Events?.Select(a => a.ToBusinessModel()).ToList(),
                //VesselName = model.VesselName,
                //InProgressEventsCount = model.InProgressEventsCount,
                StartDate = model.StartDate,
            };
        }

        public static StsOperation ToDataModel(this StsOperationBusinessModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new StsOperation()
            {
                Id = model.Id ?? default,
                EventId = model.EventId ?? default,
                ReverseLightering = model.ReverseLightering,
                CompanyParticipatingVesselId = model.CompanyParticipatingVesselId,
                ParticipatingVessel = model.ParticipatingVessel,
                SameSizeParticipatingVessel = model.SameSizeParticipatingVessel,
                Comments = model.Comments
            };
        }

        public static StsOperationBusinessModel ToBusinessModel(this StsOperation model)
        {
            if (model == null)
            {
                return null;
            }
            return new StsOperationBusinessModel()
            {
                Id = model.Id,
                EventId = model.EventId,
                ReverseLightering = model.ReverseLightering,
                CompanyParticipatingVesselId = model.CompanyParticipatingVesselId,
                ParticipatingVessel = model.ParticipatingVessel,
                SameSizeParticipatingVessel = model.SameSizeParticipatingVessel,
                Comments = model.Comments
            };
        }

        public static EventDataModel ToDataModel(this EventBaseBusinessModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new EventDataModel()
            {
                ParentEventId = model.ParentEventId,
                Timestamp = model.Timestamp,
                UserId = model.UserId,
                EventTypeId = model.EventTypeId,
                Comment = model.Comment,
                CustomEventName = model.CustomEventName,
                PortId = model.PortId,
                Terminal = model.Terminal,
                LongDegrees = model.LongDegrees,
                LongMinutes = model.LongMinutes,
                LongSeconds = model.LongSeconds,
                LatDegrees = model.LatDegrees,
                LatMinutes = model.LatMinutes,
                LatSeconds = model.LatSeconds,
                Lat = model.Lat,
                Lng = model.Lng,
                FuelType = model.FuelType,
                BunkeringDataId = model.BunkeringDataId,
                //StsOperation = model.StsOperation?.EventId > 0 ? model.StsOperation.ToDataModel() : null
            };
        }
    }
}
