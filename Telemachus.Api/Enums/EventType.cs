using System;
using System.Collections.Generic;

namespace Enums
{
    public static class EventType
    {
        public static readonly Guid AnchoringCompleted = new Guid("B0AAD731-4048-4EF0-A7E9-342C438432DD");
        public static readonly Guid AnchorUp = new Guid("552E2228-6AD9-4231-9921-800C000D4397");
        public static readonly Guid BunkeringPlan = new Guid("D5E77BA7-2979-490A-9F62-8F6983D7E784");
        public static readonly Guid BunkeringPlanProjected = new Guid("D796CBC0-C20D-4B0E-9C87-F1199FE3F400");
        public static readonly Guid CommenceBunkering = new Guid("A756A087-9124-4DD2-9CBB-98A4D86C2539");
        public static readonly Guid CommenceBunkeringComplete = new Guid("71B241F1-BFBC-4EE0-B105-363C527F0C7D");
        public static readonly Guid CommenceDischargingParcel = new Guid("4ED1ED3F-5D53-4821-BD66-1EC0B1E9233C");
        public static readonly Guid CommenceDrifting = new Guid("0EF55BE2-F032-4185-B8D7-FFD44B9C2690");
        public static readonly Guid CommenceHeavingAnchor = new Guid("0F936AB5-BF30-4463-A689-94153FACEEAF");
        public static readonly Guid CommenceHoses_LoadingArmConnection = new Guid("C1FAFFAF-0D3A-48C2-8BC0-B878C19AA4B4");
        public static readonly Guid CommenceHoses_LoadingArmDisconnection = new Guid("49F126FA-F747-43ED-A144-E0739626AECD");
        public static readonly Guid CommenceInternalTransfer = new Guid("D0CB967F-BDAD-4CE4-AEA4-677907CAD6EE");
        public static readonly Guid CommenceLoadingParcel = new Guid("67B9AB9B-2919-438B-875E-DC643852DACB");
        public static readonly Guid CommenceManeuvering = new Guid("C0ABD828-7737-4481-9299-05E4953FC2FA");
        public static readonly Guid CommenceMooring = new Guid("8945F8CF-5CDD-4CCB-9B6A-F082CEAD9E68");
        public static readonly Guid CommenceUnMooring = new Guid("0AB54BE7-5BB9-4590-9236-F4D295B63F91");
        public static readonly Guid CompleteDischargingParcel = new Guid("69592C91-4302-4553-A93E-B777A1F3F8F4");
        public static readonly Guid CompleteHoses_LoadingArmConnection = new Guid("2712BCD5-10FF-4C15-8916-FE47A5588ADD");
        public static readonly Guid CompleteHoses_LoadingArmDisconnection = new Guid("E35EDD0B-C7EE-4217-8644-8058BD111C3B");
        public static readonly Guid CompleteLoadingParcel = new Guid("1FC89AB3-9EF4-4BFF-8FEE-BB52ADCC67E6");
        public static readonly Guid CompleteMooring = new Guid("3FE72760-5A1E-456E-B928-9750EAA265C2");
        public static readonly Guid CompleteUnmooring = new Guid("0F68EB79-FC75-47AF-9876-9DB2EA1159C5");
        public static readonly Guid COSP = new Guid("7E826887-6C05-4626-B908-AB461E2EB149");
        public static readonly Guid Custom = new Guid("B8189512-C1A5-4C40-A847-94849E690FB7");
        public static readonly Guid DropAnchor = new Guid("A843D656-EEB2-44A8-8C6D-5E3DFCB82F92");
        public static readonly Guid EcaEntry = new Guid("1FD0655C-322B-448D-AECE-37043385AEA9");
        public static readonly Guid EOSP = new Guid("D470AC75-50F5-4451-A6DF-AA61B2187CB7");
        public static readonly Guid FWE = new Guid("F3F478F1-0ADE-4789-9DA4-F2AB22EF36F9");
        public static readonly Guid Noon = new Guid("42E15CD2-A7FC-412E-907C-F9249C70540F");
        public static readonly Guid SBE = new Guid("C02BE2F4-80CC-4EA1-8DB0-F144DB11B1D3");

        public static readonly List<Guid> ParcelGroup = new List<Guid>()
        {
            CommenceLoadingParcel,
            CompleteLoadingParcel,
            CommenceDischargingParcel,
            CompleteDischargingParcel
        };
        public static readonly List<Guid> ParcelCommenceGroup = new List<Guid>()
        {
            CommenceLoadingParcel,
            CommenceDischargingParcel
        };
        public static readonly List<Guid> UnMooringGroup = new List<Guid>()
        {
            CommenceUnMooring,
            CompleteUnmooring
        };

        public static readonly List<Guid> ChangeOverGroup = new List<Guid>()
        {
            new Guid("873F491A-FCC8-4245-8548-ECFD0ADEA69B"),
            new Guid("C7191E21-6330-45F5-8109-24E88F004A00"),
            new Guid("0FF21E47-0E33-4D44-87FF-3F3D9BE57535")
        };

        public static readonly List<Guid> CommenceBunkeringGroup = new List<Guid>()
        {
            CommenceBunkering,
            CommenceBunkeringComplete
        };

        public static readonly List<Guid> BunkeringPlanGroup = new List<Guid>()
        {
            BunkeringPlan,
            BunkeringPlanProjected
        };

        public static readonly List<Guid> BunkeringGroup = new List<Guid>()
        {
            BunkeringPlan,
            BunkeringPlanProjected,
            CommenceBunkering,
            CommenceBunkeringComplete
        };

        public static readonly List<Guid> BunkeringCompleteOrProjectedGroup = new List<Guid>()
        {
            BunkeringPlanProjected,
            CommenceBunkeringComplete
        };

    }
}
