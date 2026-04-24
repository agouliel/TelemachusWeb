using System;
using System.Collections.Generic;
using System.Linq;
using Telemachus.Business.Models.Events;
using Telemachus.Data.Models.Events;

namespace Telemachus.Business.Services.Mappers
{
    public static class EventTypeMapper
    {
        public static EventTypeBusinessModel ToBusinessModel(this EventTypeDataModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new EventTypeBusinessModel()
            {
                Name = model.Name,
                Id = model.Id,
                Available = model.Available,
                IsPairedEvent = model.IsPairedEvent,
                Comment = model.Comment,
                BusinessId = model.BusinessId,
                Relevant = model.Relevant,
                Suggested = model.Suggested,
                NextConditionId = model.NextCondition?.BusinessId,
                NextConditionName = model.NextCondition?.Name,
                PairedEventNextConditionId = model.PairedEventType?.NextCondition?.BusinessId,
                PairedEventNextConditionName = model.PairedEventType?.NextCondition?.Name
            };
        }
        public static Models.Reports.Design.EventTypeBusinessModel ToBusinessDesignModel(this EventTypeDataModel t)
        {
            if (t == null)
            {
                return null;
            }
            return new Models.Reports.Design.EventTypeBusinessModel()
            {
                EventTypeId = t.Id,
                Name = t.Name,
                PairedEventTypeId = t.PairedEventTypeId,
                PairedEventType = t.PairedEventType != null ? t.PairedEventType.ToBusinessDesignModel() : null,
                NextConditionId = t.NextConditionId,
                Transit = t.Transit,
                ReportTypeId = t.ReportTypeId,
                EventTypesConditions = t.EventTypesConditions != null ? t.EventTypesConditions.Select(c => c.ConditionId).ToList() : null,
                Prerequisites = t.Prerequisites != null ? t.Prerequisites.ToBusinessModel() : null
            };
        }

        public static EventTypeDataModel ToDataModel(this Models.Reports.Design.EventTypeBusinessModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new EventTypeDataModel()
            {
                Id = model.EventTypeId ?? default,
                Name = model.Name,
                PairedEventTypeId = model.PairedEventTypeId,
                NextConditionId = model.PairedConditionChange == false ? model.NextConditionId : null,
                ReportTypeId = model.ReportTypeId,
                OnePairPerTime = model.PairedEventType != null ? model.OnePairPerTime.GetValueOrDefault() : false,
                PairedEventType = model.PairedEventType != null ? new EventTypeDataModel()
                {
                    Id = model.PairedEventType.EventTypeId ?? default,
                    Name = model.PairedEventType.Name,
                    NextConditionId = model.PairedConditionChange == true ? model.NextConditionId : null,
                    Transit = model.PairedEventType.Transit,
                    ReportTypeId = model.PairedEventType.ReportTypeId
                } : null
            };
        }

        public static List<EventTypeDataModel> ToDataModel(this IEnumerable<Models.Reports.Design.EventTypeBusinessModel> models)
        {
            return models.Select(ToDataModel).ToList();
        }
        public static List<EventTypeBusinessModel> ToBusinessModel(this IEnumerable<EventTypeDataModel> models)
        {
            return models.Select(ToBusinessModel).ToList();
        }
        public static EventTypePrerequisiteBusinessModel ToBusinessModel(this EventTypePrerequisiteDataModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new EventTypePrerequisiteBusinessModel()
            {
                Id = model.Id,
                EventTypeId = model.EventTypeId,
                AvailableAfterEventTypeId = model.AvailableAfterEventTypeId,
                Completed = model.Completed,
                Override = model.Override,
                BusinessId = model.BusinessId,
                Required = model.Required,
                RequiredForRepetition = model.RequiredForRepetition
            };
        }

        public static List<EventTypePrerequisiteBusinessModel> ToBusinessModel(this IEnumerable<EventTypePrerequisiteDataModel> models)
        {
            return models.Select(ToBusinessModel).ToList();
        }

        public static EventTypePrerequisiteDataModel ToDataModel(this EventTypePrerequisiteBusinessModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new EventTypePrerequisiteDataModel()
            {
                Id = model.Id ?? 0,
                EventTypeId = model.EventTypeId,
                AvailableAfterEventTypeId = model.AvailableAfterEventTypeId,
                Completed = model.Completed ?? false,
                Override = model.Override ?? false,
                Required = model.Required ?? false,
                RequiredForRepetition = model.RequiredForRepetition ?? false,
                BusinessId = model.BusinessId ?? Guid.NewGuid()
            };
        }
        public static List<EventTypePrerequisiteDataModel> ToDataModel(this IEnumerable<EventTypePrerequisiteBusinessModel> models)
        {
            return models.Select(ToDataModel).ToList();
        }


    }
}
