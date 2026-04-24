using System;

namespace Enums
{
    public class Status
    {
        public static readonly Guid InProgress = new Guid("7DCBAC44-63DC-4425-A41B-D41746E47AA0");
        public static readonly Guid Rejected = new Guid("F740E819-48CE-4FA6-93F3-BCE651865715");
        public static readonly Guid Approved = new Guid("40B413C4-C537-4DDE-A9BB-418815F76A8F");
        public static readonly Guid Completed = new Guid("114F49A8-AAC7-4ABA-8229-374C5AC142CB");
    }
}
