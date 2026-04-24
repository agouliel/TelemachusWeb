using System;
using System.Collections.Generic;
using static DocumentCreator.DocumentOperator;

namespace Telemachus.Business.Models.Events
{
    public class DocumentViewModel
    {
        public int? Id { get; set; }
        public bool Completed { get; set; }
        public Operators Operator { get; set; } = Operators.Ionia;
        public string VesselName { get; set; }
        public DateTimeOffset? Date { get; set; }
        public string FormattedDate
        {
            get
            {
                return Date?.ToString("dd/MM/yyyy");
            }
        }
        public string OperationGrade { get; set; }
        public string Voyage { get; set; }
        public int? PortId { get; set; }
        public string PortName { get; set; }
        public string Terminal { get; set; }
        public string Remarks { get; set; }
        public int? FirstId { get; set; }
        public int? LastId { get; set; }
        public string CharterParty { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public DateTime? LastEventDate { get; set; }
        public List<DocumentFactViewModel> Facts { get; set; }
        public List<EventMarker> Markers { get; set; } = new List<EventMarker>();

    }
    public class DocumentViewModelDTO
    {
        public bool Completed { get; set; }
        public DateTime? Date { get; set; }
        public string OperationGrade { get; set; }
        public string Voyage { get; set; }
        public int? PortId { get; set; }
        public string Terminal { get; set; }
        public string Remarks { get; set; }
        public string CharterParty { get; set; }
        public List<int> EventInclude { get; set; }
        public List<int> EventExclude { get; set; }
        public List<int> HiddenDates { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
    public class DocumentFactViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string CustomEventName { get; set; }
        public string DateFormatted
        {
            get
            {
                return Timestamp?.ToString("dd/MM/yy");
            }
        }
        public string TimeFormatted
        {
            get
            {
                return Timestamp?.ToString("HH:mm");
            }
        }
        public int StatusId { get; set; }
        public string StatusFormatted { get; set; }
        public int EventTypeId { get; set; }
        public string EventTypeFormatted { get; set; }
        public string Remarks { get; set; }
        public string Terminal { get; set; }
        public int? PortId { get; set; }
        public int VoyageId { get; set; }
        public bool Excluded { get; set; }
        public bool HiddenDate { get; set; }
        public string ConditionName { get; set; }
    }

    public class EventMarker
    {
        public int Id { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public int ConditionId { get; set; }
        public string ConditionName { get; set; }
        public int VoyageId { get; set; }

    }
}
