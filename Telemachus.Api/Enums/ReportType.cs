using System;
using System.Collections.Generic;

namespace Enums
{
    public class ReportType
    {

        public static readonly Guid Hfo = new Guid("3D02E784-93D8-41D9-B240-0FC284C2067E");
        public static readonly Guid Mgo = new Guid("7F5F98E9-A61E-4C9F-A3F6-415A317C5030");
        public static readonly Guid RobHfoActualGroup = new Guid("07F5EA2B-526E-4CF1-849E-BC38166034E0");
        public static readonly Guid RobHfoPoolGroup = new Guid("FA1768BB-EE4A-47B6-B4EE-014CB04FB864");
        public static readonly Guid RobMgoActualGroup = new Guid("F9C48AA1-6AB2-468E-8437-300F592832B0");
        public static readonly Guid RobMgoPoolGroup = new Guid("4AD3DE20-CDCD-4CFA-AB72-64E7BEF4D3C1");
        public static readonly Guid RobHfoBunkerGroup = new Guid("81A85F35-5BAF-49FA-A98F-15AF7B8727B6");
        public static readonly Guid RobMgoBunkerGroup = new Guid("0ADBE48F-87FC-4D97-9BC6-9ADAE7A07C19");

        public static readonly List<Guid> ActualGroups = new List<Guid>()
        {
            RobHfoActualGroup,
            RobMgoActualGroup
        };
        public static readonly List<Guid> HfoGroups = new List<Guid>()
        {
            RobHfoActualGroup,
            RobHfoPoolGroup,
            RobHfoBunkerGroup
        };
        public static readonly List<Guid> MgoGroups = new List<Guid>()
        {
            RobMgoActualGroup,
            RobMgoPoolGroup,
            RobMgoBunkerGroup
        };
        public static readonly List<Guid> ExcludedEventTypes = new List<Guid>()
        {
            new Guid("A756A087-9124-4DD2-9CBB-98A4D86C2539"),
            new Guid("D0CB967F-BDAD-4CE4-AEA4-677907CAD6EE"),
            new Guid("D5E77BA7-2979-490A-9F62-8F6983D7E784"),
            new Guid("D796CBC0-C20D-4B0E-9C87-F1199FE3F400")
        };

        public static readonly Guid BunkerHfoGroup = new Guid("81A85F35-5BAF-49FA-A98F-15AF7B8727B6");
        public static readonly Guid BunkerMgoGroup = new Guid("0ADBE48F-87FC-4D97-9BC6-9ADAE7A07C19");

        public static readonly List<Guid> BunkerGroups = new List<Guid>()
        {
            BunkerHfoGroup,
            BunkerMgoGroup
        };

        public static readonly List<int> Rob = new List<int>()
        {
            1,3,4
        };
        public static readonly List<int> Performance = new List<int>()
        {
            1, 5
        };
        public static readonly List<int> Bunkering = new List<int>()
        {
            3
        };

        public static readonly List<string> ManagedFields = new List<string>()
            {
                "bdn", "density", "kinematicViscosity", "sulphurContent"
            };
    }
}
