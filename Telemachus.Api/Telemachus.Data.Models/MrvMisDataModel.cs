namespace Telemachus.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("voyage_details2")]
    public class MrvMisDataModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("vessel")]
        public string Vessel { get; set; }

        [Column("voyage")]
        public string Voyage { get; set; }

        [Column("event_id")]
        public int? EventId { get; set; }

        [Column("event_timestamp")]
        public DateTime? EventTimestamp { get; set; }

        [Column("status_id")]
        public int? StatusId { get; set; }

        [Column("event_name")]
        public string EventName { get; set; }

        [Column("condition_name")]
        public string ConditionName { get; set; }

        [Column("cargo_status")]
        public int? CargoStatus { get; set; }

        [Column("port_name")]
        public string PortName { get; set; }

        [Column("report_id")]
        public int? ReportId { get; set; }

        [Column("instructedSpeed")]
        public string InstructedSpeed { get; set; }

        [Column("instructedChartererConsumption")]
        public string InstructedChartererConsumption { get; set; }

        [Column("ROB_HFO_ACTUAL")]
        public double? RobHfoActual { get; set; }

        [Column("ROB_HFO_POOL")]
        public double? RobHfoPool { get; set; }

        [Column("ROB_MGO_ACTUAL")]
        public double? RobMgoActual { get; set; }

        [Column("ROB_MGO_POOL")]
        public double? RobMgoPool { get; set; }

        [Column("distanceToGo")]
        public string DistanceToGo { get; set; }

        [Column("distanceOverGround")]
        public string DistanceOverGround { get; set; }

        [Column("steamingTime")]
        public string SteamingTime { get; set; }

        [Column("vlsfo_actual_consumption_me")]
        public string VlsfoActualConsumptionMe { get; set; }

        [Column("vlsfo_pool_consumption_me")]
        public string VlsfoPoolConsumptionMe { get; set; }

        [Column("lsmgo_actual_consumption_me")]
        public string LsmgoActualConsumptionMe { get; set; }

        [Column("lsmgo_pool_consumption_me")]
        public string LsmgoPoolConsumptionMe { get; set; }

        [Column("windForceForecast")]
        public string WindForceForecast { get; set; }

        [Column("oop")]
        public string Oop { get; set; }

        [Column("airCoolerCoolingWaterInletTemp")]
        public string AirCoolerCoolingWaterInletTemp { get; set; }

        [Column("auxBlowerStatus")]
        public string AuxBlowerStatus { get; set; }

        [Column("barometricPressure")]
        public string BarometricPressure { get; set; }

        [Column("bdn")]
        public string Bdn { get; set; }

        [Column("bilgeRob")]
        public string BilgeRob { get; set; }

        [Column("commingling")]
        public string Commingling { get; set; }

        [Column("cylinderOil")]
        public string CylinderOil { get; set; }

        [Column("dgOil")]
        public string DgOil { get; set; }

        [Column("density")]
        public string Density { get; set; }

        [Column("dg1Load")]
        public string Dg1Load { get; set; }

        [Column("dg2Load")]
        public string Dg2Load { get; set; }

        [Column("dg3Load")]
        public string Dg3Load { get; set; }

        [Column("dg4Load")]
        public string Dg4Load { get; set; }

        [Column("distilledWater")]
        public string DistilledWater { get; set; }

        [Column("draftAft")]
        public string DraftAft { get; set; }

        [Column("draftFwd")]
        public string DraftFwd { get; set; }

        [Column("erTemp")]
        public string ErTemp { get; set; }

        [Column("egeSteamPress")]
        public string EgeSteamPress { get; set; }

        [Column("exhaustGasTempAfterTc")]
        public string ExhaustGasTempAfterTc { get; set; }

        [Column("exhaustGasTempBeforeTc")]
        public string ExhaustGasTempBeforeTc { get; set; }

        [Column("forecastWeather")]
        public string ForecastWeather { get; set; }

        [Column("freshWaterDomestic")]
        public string FreshWaterDomestic { get; set; }

        [Column("freshWaterPotable")]
        public string FreshWaterPotable { get; set; }

        [Column("freshWaterTankCleaning")]
        public string FreshWaterTankCleaning { get; set; }

        [Column("fuelOilPumpIndex")]
        public string FuelOilPumpIndex { get; set; }

        [Column("fwProduction")]
        public string FwProduction { get; set; }

        [Column("gsv")]
        public string Gsv { get; set; }

        [Column("kinematicViscosity")]
        public string KinematicViscosity { get; set; }

        [Column("lowerCalorifer")]
        public string LowerCalorifer { get; set; }

        [Column("lsmgo_actual_consumption_boiler_1")]
        public string LsmgoActualConsumptionBoiler1 { get; set; }

        [Column("lsmgo_actual_consumption_boiler_2")]
        public string LsmgoActualConsumptionBoiler2 { get; set; }

        [Column("lsmgo_actual_consumption_composite_boiler")]
        public string LsmgoActualConsumptionCompositeBoiler { get; set; }

        [Column("lsmgo_actual_consumption_cummins")]
        public string LsmgoActualConsumptionCummins { get; set; }

        [Column("lsmgo_actual_consumption_dg1")]
        public string LsmgoActualConsumptionDg1 { get; set; }

        [Column("lsmgo_actual_consumption_dg2")]
        public string LsmgoActualConsumptionDg2 { get; set; }

        [Column("lsmgo_actual_consumption_dg3")]
        public string LsmgoActualConsumptionDg3 { get; set; }

        [Column("lsmgo_actual_consumption_dg4")]
        public string LsmgoActualConsumptionDg4 { get; set; }

        [Column("lsmgo_actual_consumption_igg")]
        public string LsmgoActualConsumptionIgg { get; set; }

        [Column("lsmgo_pool_consumption_boiler_1")]
        public string LsmgoPoolConsumptionBoiler1 { get; set; }

        [Column("lsmgo_pool_consumption_boiler_2")]
        public string LsmgoPoolConsumptionBoiler2 { get; set; }

        [Column("lsmgo_pool_consumption_composite_boiler")]
        public string LsmgoPoolConsumptionCompositeBoiler { get; set; }

        [Column("lsmgo_pool_consumption_cummins")]
        public string LsmgoPoolConsumptionCummins { get; set; }

        [Column("lsmgo_pool_consumption_dg1")]
        public string LsmgoPoolConsumptionDg1 { get; set; }

        [Column("lsmgo_pool_consumption_dg2")]
        public string LsmgoPoolConsumptionDg2 { get; set; }

        [Column("lsmgo_pool_consumption_dg3")]
        public string LsmgoPoolConsumptionDg3 { get; set; }

        [Column("lsmgo_pool_consumption_dg4")]
        public string LsmgoPoolConsumptionDg4 { get; set; }

        [Column("lsmgo_pool_consumption_igg")]
        public string LsmgoPoolConsumptionIgg { get; set; }

        [Column("meOilOil")]
        public string MeOilOil { get; set; }

        [Column("mainEngineRevolutionOutputCounter")]
        public string MainEngineRevolutionOutputCounter { get; set; }

        [Column("observedWeather")]
        public string ObservedWeather { get; set; }

        [Column("rpmDeclared")]
        public string RpmDeclared { get; set; }

        [Column("runningHours_Boiler_1_LSMGO")]
        public string RunningHoursBoiler1Lsmgo { get; set; }

        [Column("runningHours_Boiler_1_VLSFO")]
        public string RunningHoursBoiler1Vlsfo { get; set; }

        [Column("runningHours_Boiler_2_LSMGO")]
        public string RunningHoursBoiler2Lsmgo { get; set; }

        [Column("runningHours_Boiler_2_VLSFO")]
        public string RunningHoursBoiler2Vlsfo { get; set; }

        [Column("runningHours_Cargo_Cummins_LSMGO")]
        public string RunningHoursCargoCumminsLsmgo { get; set; }

        [Column("runningHours_Composite_Boiler_LSMGO")]
        public string RunningHoursCompositeBoilerLsmgo { get; set; }

        [Column("runningHours_Composite_Boiler_VLSFO")]
        public string RunningHoursCompositeBoilerVlsfo { get; set; }

        [Column("runningHours_DG1_LSMGO")]
        public string RunningHoursDg1Lsmgo { get; set; }

        [Column("runningHours_DG1_VLSFO")]
        public string RunningHoursDg1Vlsfo { get; set; }

        [Column("runningHours_DG2_LSMGO")]
        public string RunningHoursDg2Lsmgo { get; set; }

        [Column("runningHours_DG2_VLSFO")]
        public string RunningHoursDg2Vlsfo { get; set; }

        [Column("runningHours_DG3_LSMGO")]
        public string RunningHoursDg3Lsmgo { get; set; }

        [Column("runningHours_DG3_VLSFO")]
        public string RunningHoursDg3Vlsfo { get; set; }

        [Column("runningHours_DG4_LSMGO")]
        public string RunningHoursDg4Lsmgo { get; set; }

        [Column("runningHours_DG4_VLSFO")]
        public string RunningHoursDg4Vlsfo { get; set; }

        [Column("runningHours_IGG_LSMGO_Running_Hours")]
        public string RunningHoursIggLsmgoRunningHours { get; set; }

        [Column("scavengeAirInletTemp")]
        public string ScavengeAirInletTemp { get; set; }

        [Column("scavengeAirPressure")]
        public string ScavengeAirPressure { get; set; }

        [Column("seaCurrentSpeed")]
        public string SeaCurrentSpeed { get; set; }

        [Column("seaState")]
        public string SeaState { get; set; }

        [Column("shaftPowerShapoli")]
        public string ShaftPowerShapoli { get; set; }

        [Column("slipDeclared")]
        public string SlipDeclared { get; set; }

        [Column("slops")]
        public string Slops { get; set; }

        [Column("sludgeRob")]
        public string SludgeRob { get; set; }

        [Column("sounding")]
        public string Sounding { get; set; }

        [Column("speedlogDistance")]
        public string SpeedlogDistance { get; set; }

        [Column("sulphurContent")]
        public string SulphurContent { get; set; }

        [Column("swellDirection")]
        public string SwellDirection { get; set; }

        [Column("tcAirInletTemp")]
        public string TcAirInletTemp { get; set; }

        [Column("tankTemperature")]
        public string TankTemperature { get; set; }

        [Column("turboChargerRpm")]
        public string TurboChargerRpm { get; set; }

        [Column("vcf")]
        public string Vcf { get; set; }

        [Column("vesselCurrentList")]
        public string VesselCurrentList { get; set; }

        [Column("vesselCurrentTrim")]
        public string VesselCurrentTrim { get; set; }

        [Column("vlsfo_actual_consumption_boiler_1")]
        public string VlsfoActualConsumptionBoiler1 { get; set; }

        [Column("vlsfo_actual_consumption_boiler_2")]
        public string VlsfoActualConsumptionBoiler2 { get; set; }

        [Column("vlsfo_actual_consumption_composite_boiler")]
        public string VlsfoActualConsumptionCompositeBoiler { get; set; }

        [Column("vlsfo_actual_consumption_dg1")]
        public string VlsfoActualConsumptionDg1 { get; set; }

        [Column("vlsfo_actual_consumption_dg2")]
        public string VlsfoActualConsumptionDg2 { get; set; }

        [Column("vlsfo_actual_consumption_dg3")]
        public string VlsfoActualConsumptionDg3 { get; set; }

        [Column("vlsfo_actual_consumption_dg4")]
        public string VlsfoActualConsumptionDg4 { get; set; }

        [Column("vlsfo_pool_consumption_boiler_1")]
        public string VlsfoPoolConsumptionBoiler1 { get; set; }

        [Column("vlsfo_pool_consumption_boiler_2")]
        public string VlsfoPoolConsumptionBoiler2 { get; set; }

        [Column("vlsfo_pool_consumption_composite_boiler")]
        public string VlsfoPoolConsumptionCompositeBoiler { get; set; }

        [Column("vlsfo_pool_consumption_dg1")]
        public string VlsfoPoolConsumptionDg1 { get; set; }

        [Column("vlsfo_pool_consumption_dg2")]
        public string VlsfoPoolConsumptionDg2 { get; set; }

        [Column("vlsfo_pool_consumption_dg3")]
        public string VlsfoPoolConsumptionDg3 { get; set; }

        [Column("vlsfo_pool_consumption_dg4")]
        public string VlsfoPoolConsumptionDg4 { get; set; }

        [Column("volume")]
        public string Volume { get; set; }

        [Column("wcf")]
        public string Wcf { get; set; }

        [Column("weight")]
        public string Weight { get; set; }

        [Column("windDirection")]
        public string WindDirection { get; set; }

        [Column("windForce")]
        public string WindForce { get; set; }

        [Column("cargo")]
        public int? Cargo { get; set; }

        [Column("port_business_id")]
        public string PortBusinessId { get; set; }

        [Column("averageSpeedThroughTheWater")]
        public double? AverageSpeedThroughTheWater { get; set; }

        [Column("averageSpeedOverGround")]
        public double? AverageSpeedOverGround { get; set; }

        [Column("total_consumption_estimated")]
        public double? TotalConsumptionEstimated { get; set; }

        [Column("total_consumption_declared")]
        public double? TotalConsumptionDeclared { get; set; }

        [Column("VLSFOActualConsumptionThroughFlowMetersDiff")]
        public double? VlsfoActualConsumptionThroughFlowMetersDiff { get; set; }

        [Column("VLSFOPoolConsumptionThroughFlowMetersDiff")]
        public double? VlsfoPoolConsumptionThroughFlowMetersDiff { get; set; }

        [Column("LSMGOActualConsumptionThroughFlowMetersDiff")]
        public double? LsmgoActualConsumptionThroughFlowMetersDiff { get; set; }

        [Column("LSMGOPoolConsumptionThroughFlowMetersDiff")]
        public double? LsmgoPoolConsumptionThroughFlowMetersDiff { get; set; }

        [Column("mainEngineMaxPower")]
        public double? MainEngineMaxPower { get; set; }

        [Column("pitchPropeller")]
        public double? PitchPropeller { get; set; }

        [Column("totalDistanceOverGround")]
        public double? TotalDistanceOverGround { get; set; }

        [Column("actualConsVLSFO")]
        public double? ActualConsVlsfo { get; set; }

        [Column("actualConsLSMGO")]
        public double? ActualConsLsmgo { get; set; }

        [Column("poolConsVLSFO")]
        public double? PoolConsVlsfo { get; set; }

        [Column("poolConsLSMGO")]
        public double? PoolConsLsmgo { get; set; }

        [Column("event_timestamp_utc")]
        public DateTime? EventTimestampUtc { get; set; }

        [Column("lat")]
        public double? Lat { get; set; }

        [Column("long")]
        public double? Long { get; set; }

        [Column("Bunkering_LSMGO")]
        public double? BunkeringLsmgo { get; set; }

        [Column("isEU")]
        public int? IsEu { get; set; }

        [Column("ROB_LFO_ACTUAL")]
        public double? RobLfoActual { get; set; }

        [Column("ROB_LFO_POOL")]
        public double? RobLfoPool { get; set; }

        [Column("Bunkering_LFO")]
        public double? BunkeringLfo { get; set; }

        [Column("Bunkering_HFO")]
        public double? BunkeringHfo { get; set; }

        [Column("leg_id")]
        public string LegId { get; set; }

        [Column("ROB_HFO_ACTUAL_TANKS")]
        public string RobHfoActualTanks { get; set; }

        [Column("ROB_HFO_POOL_TANKS")]
        public string RobHfoPoolTanks { get; set; }

        [Column("ROB_MGO_ACTUAL_TANKS")]
        public string RobMgoActualTanks { get; set; }

        [Column("ROB_MGO_POOL_TANKS")]
        public string RobMgoPoolTanks { get; set; }

        [Column("port_name_future_if_opl")]
        public string PortNameFutureIfOpl { get; set; }

        [Column("port_business_id_future_if_opl")]
        public string PortBusinessIdFutureIfOpl { get; set; }

        [Column("isEU_future_if_opl")]
        public int? IsEuFutureIfOpl { get; set; }

        [Column("port_name_past_if_opl")]
        public string PortNamePastIfOpl { get; set; }

        [Column("port_business_id_past_if_opl")]
        public string PortBusinessIdPastIfOpl { get; set; }

        [Column("isEU_past_if_opl")]
        public int? IsEuPastIfOpl { get; set; }
    }

}
