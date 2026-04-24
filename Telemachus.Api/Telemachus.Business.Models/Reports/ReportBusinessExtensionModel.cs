using System;
using System.Linq;

using Helpers;

namespace Telemachus.Business.Models.Reports
{
    public class ReportBusinessExtensionModel
    {
        public ReportBusinessModel Model { get; }

        public ReportBusinessExtensionModel(ReportBusinessModel model)
        {
            Model = model;
        }

        public double MainEngineMaxPower => Model.Performance?.MainEngineMaxPower ?? 0;
        public double PitchPropeller => Model.Performance?.PitchPropeller ?? 0;
        public double TotalDistanceOverGround => Model.Performance?.TotalDistanceOverGround ?? 0;
        public double ActualConsVLSFO => Model.Performance?.TotalConsumption.ActualConsVLSFO ?? 0;
        public double ActualConsLSMGO => Model.Performance?.TotalConsumption.ActualConsLSMGO ?? 0;
        public double PoolConsVLSFO => Model.Performance?.TotalConsumption.PoolConsVLSFO ?? 0;
        public double PoolConsLSMGO => Model.Performance?.TotalConsumption.PoolConsLSMGO ?? 0;
        public double AirCoolerCoolingWaterInletTemp => Model.ReportFields.Where(rf => rf.ValidationKey == "airCoolerCoolingWaterInletTemp").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public string AuxBlowerStatus => Model.ReportFields.Where(rf => rf.ValidationKey == "auxBlowerStatus").FirstOrDefault()?.Value ?? "";
        public double BarometricPressure => Model.ReportFields.Where(rf => rf.ValidationKey == "barometricPressure").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double BilgeRob => Model.ReportFields.Where(rf => rf.ValidationKey == "bilgeRob").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double CylinderOil => Model.ReportFields.Where(rf => rf.ValidationKey == "cylinderOil").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double DgOil => Model.ReportFields.Where(rf => rf.ValidationKey == "d/gOil").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double Dg1Load => Model.ReportFields.Where(rf => rf.ValidationKey == "dg1Load").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double Dg2Load => Model.ReportFields.Where(rf => rf.ValidationKey == "dg2Load").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double Dg3Load => Model.ReportFields.Where(rf => rf.ValidationKey == "dg3Load").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double Dg4Load => Model.ReportFields.Where(rf => rf.ValidationKey == "dg4Load").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double DistanceOverGround => Model.ReportFields.Where(rf => rf.ValidationKey == "distanceOverGround").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double DistanceToGo => Model.ReportFields.Where(rf => rf.ValidationKey == "distanceToGo").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double DistilledWater => Model.ReportFields.Where(rf => rf.ValidationKey == "distilledWater").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double DraftAft => Model.ReportFields.Where(rf => rf.ValidationKey == "draftAft").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double DraftFwd => Model.ReportFields.Where(rf => rf.ValidationKey == "draftFwd").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double ErTemp => Model.ReportFields.Where(rf => rf.ValidationKey == "e/rTemp").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double EgeSteamPress => Model.ReportFields.Where(rf => rf.ValidationKey == "egeSteamPress").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double ExhaustGasTempAfterTc => Model.ReportFields.Where(rf => rf.ValidationKey == "exhaustGasTempAfterT/c").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double ExhaustGasTempBeforeTc => Model.ReportFields.Where(rf => rf.ValidationKey == "exhaustGasTempBeforeT/c").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public string ForecastWeather => Model.ReportFields.Where(rf => rf.ValidationKey == "forecastWeather").FirstOrDefault()?.Value ?? "";
        public double FreshWaterDomestic => Model.ReportFields.Where(rf => rf.ValidationKey == "freshWaterDomestic").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double FreshWaterPotable => Model.ReportFields.Where(rf => rf.ValidationKey == "freshWaterPotable").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double FreshWaterTankCleaning => Model.ReportFields.Where(rf => rf.ValidationKey == "freshWaterTankCleaning").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double FuelOilPumpIndex => Model.ReportFields.Where(rf => rf.ValidationKey == "fuelOilPumpIndex").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double FwProduction => Model.ReportFields.Where(rf => rf.ValidationKey == "fwProduction").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double InstructedChartererConsumption => Model.ReportFields.Where(rf => rf.ValidationKey == "instructedChartererConsumption").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double InstructedSpeed => Model.ReportFields.Where(rf => rf.ValidationKey == "instructedSpeed").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoActualConsumptionBoiler1 => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_actual_consumption_boiler 1").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoActualConsumptionBoiler2 => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_actual_consumption_boiler 2").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoActualConsumptionCompositeBoiler => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_actual_consumption_composite boiler").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoActualConsumptionCummins => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_actual_consumption_cummins").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoActualConsumptionDg1 => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_actual_consumption_dg1").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoActualConsumptionDg2 => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_actual_consumption_dg2").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoActualConsumptionDg3 => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_actual_consumption_dg3").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoActualConsumptionDg4 => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_actual_consumption_dg4").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoActualConsumptionDge => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_actual_consumption_dge").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoActualConsumptionIgg => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_actual_consumption_igg").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoActualConsumptionInc => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_actual_consumption_inc").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoActualConsumptionMe => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_actual_consumption_me").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoPoolConsumptionBoiler1 => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_pool_consumption_boiler 1").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoPoolConsumptionBoiler2 => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_pool_consumption_boiler 2").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoPoolConsumptionCompositeBoiler => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_pool_consumption_composite boiler").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoPoolConsumptionCummins => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_pool_consumption_cummins").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoPoolConsumptionDg1 => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_pool_consumption_dg1").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoPoolConsumptionDg2 => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_pool_consumption_dg2").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoPoolConsumptionDg3 => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_pool_consumption_dg3").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoPoolConsumptionDg4 => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_pool_consumption_dg4").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoPoolConsumptionDge => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_pool_consumption_dge").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoPoolConsumptionIgg => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_pool_consumption_igg").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoPoolConsumptionInc => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_pool_consumption_inc").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoPoolConsumptionMe => Model.ReportFields.Where(rf => rf.ValidationKey == "lsmgo_pool_consumption_me").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double MeOilOil => Model.ReportFields.Where(rf => rf.ValidationKey == "m/eOilOil").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double MainEngineRevolutionOutputCounter => Model.ReportFields.Where(rf => rf.ValidationKey == "mainEngineRevolutionOutputCounter").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public string ObservedWeather => Model.ReportFields.Where(rf => rf.ValidationKey == "observedWeather").FirstOrDefault()?.Value ?? "";
        public double Oop => Model.ReportFields.Where(rf => rf.ValidationKey == "oop").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double RpmDeclared => Model.ReportFields.Where(rf => rf.ValidationKey == "rpmDeclared").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double RunningHoursBoiler1Lsmgo => Model.ReportFields.Where(rf => rf.ValidationKey == "runningHours_Boiler_1_LSMGO").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double RunningHoursBoiler1Vlsfo => Model.ReportFields.Where(rf => rf.ValidationKey == "runningHours_Boiler_1_VLSFO").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double RunningHoursBoiler2Lsmgo => Model.ReportFields.Where(rf => rf.ValidationKey == "runningHours_Boiler_2_LSMGO").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double RunningHoursBoiler2Vlsfo => Model.ReportFields.Where(rf => rf.ValidationKey == "runningHours_Boiler_2_VLSFO").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double RunningHoursCargoCumminsLsmgo => Model.ReportFields.Where(rf => rf.ValidationKey == "runningHours_Cargo_Cummins_LSMGO").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double RunningHoursCompositeBoilerLsmgo => Model.ReportFields.Where(rf => rf.ValidationKey == "runningHours_Composite_Boiler_LSMGO").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double RunningHoursCompositeBoilerVlsfo => Model.ReportFields.Where(rf => rf.ValidationKey == "runningHours_Composite_Boiler_VLSFO").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double RunningHoursDg1Lsmgo => Model.ReportFields.Where(rf => rf.ValidationKey == "runningHours_DG1_LSMGO").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double RunningHoursDg1Vlsfo => Model.ReportFields.Where(rf => rf.ValidationKey == "runningHours_DG1_VLSFO").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double RunningHoursDg2Lsmgo => Model.ReportFields.Where(rf => rf.ValidationKey == "runningHours_DG2_LSMGO").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double RunningHoursDg2Vlsfo => Model.ReportFields.Where(rf => rf.ValidationKey == "runningHours_DG2_VLSFO").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double RunningHoursDg3Lsmgo => Model.ReportFields.Where(rf => rf.ValidationKey == "runningHours_DG3_LSMGO").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double RunningHoursDg3Vlsfo => Model.ReportFields.Where(rf => rf.ValidationKey == "runningHours_DG3_VLSFO").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double RunningHoursDg4Lsmgo => Model.ReportFields.Where(rf => rf.ValidationKey == "runningHours_DG4_LSMGO").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double RunningHoursDg4Vlsfo => Model.ReportFields.Where(rf => rf.ValidationKey == "runningHours_DG4_VLSFO").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double RunningHoursIggLsmgo => Model.ReportFields.Where(rf => rf.ValidationKey == "runningHours_IGG_LSMGO_Running_Hours").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double ScavengeAirInletTemp => Model.ReportFields.Where(rf => rf.ValidationKey == "scavengeAirInletTemp").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double ScavengeAirPressure => Model.ReportFields.Where(rf => rf.ValidationKey == "scavengeAirPressure").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double SeaCurrentSpeed => Model.ReportFields.Where(rf => rf.ValidationKey == "seaCurrentSpeed").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public string SeaState => Model.ReportFields.Where(rf => rf.ValidationKey == "seaState").FirstOrDefault()?.Value ?? "";
        public double ShaftPowerShapoli => Model.ReportFields.Where(rf => rf.ValidationKey == "shaftPowerShapoli").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double SlipDeclared => Model.ReportFields.Where(rf => rf.ValidationKey == "slipDeclared").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double Slops => Model.ReportFields.Where(rf => rf.ValidationKey == "slops").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double SludgeRob => Model.ReportFields.Where(rf => rf.ValidationKey == "sludgeRob").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double SpeedlogDistance => Model.ReportFields.Where(rf => rf.ValidationKey == "speedlogDistance").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double SteamingTime => TotalSteamingTime;
        public string SwellDirection => Model.ReportFields.Where(rf => rf.ValidationKey == "swellDirection").FirstOrDefault()?.Value ?? "";
        public double TcAirInletTemp => Model.ReportFields.Where(rf => rf.ValidationKey == "t/cAirInletTemp").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double TurboChargerRpm => Model.ReportFields.Where(rf => rf.ValidationKey == "turboChargerRpm").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double VesselCurrentList => Model.ReportFields.Where(rf => rf.ValidationKey == "vesselCurrentList").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double VesselCurrentTrim => Model.ReportFields.Where(rf => rf.ValidationKey == "vesselCurrentTrim").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double VlsfoActualConsumptionBoiler1 => Model.ReportFields.Where(rf => rf.ValidationKey == "vlsfo_actual_consumption_boiler 1").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double VlsfoActualConsumptionBoiler2 => Model.ReportFields.Where(rf => rf.ValidationKey == "vlsfo_actual_consumption_boiler 2").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double VlsfoActualConsumptionCompositeBoiler => Model.ReportFields.Where(rf => rf.ValidationKey == "vlsfo_actual_consumption_composite boiler").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double VlsfoActualConsumptionDg1 => Model.ReportFields.Where(rf => rf.ValidationKey == "vlsfo_actual_consumption_dg1").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double VlsfoActualConsumptionDg2 => Model.ReportFields.Where(rf => rf.ValidationKey == "vlsfo_actual_consumption_dg2").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double VlsfoActualConsumptionDg3 => Model.ReportFields.Where(rf => rf.ValidationKey == "vlsfo_actual_consumption_dg3").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double VlsfoActualConsumptionDg4 => Model.ReportFields.Where(rf => rf.ValidationKey == "vlsfo_actual_consumption_dg4").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double VlsfoActualConsumptionMe => Model.ReportFields.Where(rf => rf.ValidationKey == "vlsfo_actual_consumption_me").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double VlsfoPoolConsumptionBoiler1 => Model.ReportFields.Where(rf => rf.ValidationKey == "vlsfo_pool_consumption_boiler 1").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double VlsfoPoolConsumptionBoiler2 => Model.ReportFields.Where(rf => rf.ValidationKey == "vlsfo_pool_consumption_boiler 2").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double VlsfoPoolConsumptionCompositeBoiler => Model.ReportFields.Where(rf => rf.ValidationKey == "vlsfo_pool_consumption_composite boiler").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double VlsfoPoolConsumptionDg1 => Model.ReportFields.Where(rf => rf.ValidationKey == "vlsfo_pool_consumption_dg1").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double VlsfoPoolConsumptionDg2 => Model.ReportFields.Where(rf => rf.ValidationKey == "vlsfo_pool_consumption_dg2").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double VlsfoPoolConsumptionDg3 => Model.ReportFields.Where(rf => rf.ValidationKey == "vlsfo_pool_consumption_dg3").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double VlsfoPoolConsumptionDg4 => Model.ReportFields.Where(rf => rf.ValidationKey == "vlsfo_pool_consumption_dg4").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double VlsfoPoolConsumptionMe => Model.ReportFields.Where(rf => rf.ValidationKey == "vlsfo_pool_consumption_me").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public string WindDirection => Model.ReportFields.Where(rf => rf.ValidationKey == "windDirection").FirstOrDefault()?.Value ?? "";
        public double WindForce => Model.ReportFields.Where(rf => rf.ValidationKey == "windForce").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double WindForceForecast => Model.ReportFields.Where(rf => rf.ValidationKey == "windForceForecast").FirstOrDefault()?.Value?.ToDouble() ?? 0;
        public double LsmgoActualTotalConsumption => LsmgoActualConsumptionMe + LsmgoActualConsumptionDg1 + LsmgoActualConsumptionDg2 + LsmgoActualConsumptionDg3 + LsmgoActualConsumptionDg4;
        public double LsmgoPoolTotalConsumption => LsmgoPoolConsumptionMe + LsmgoPoolConsumptionDg1 + LsmgoPoolConsumptionDg2 + LsmgoPoolConsumptionDg3 + LsmgoPoolConsumptionDg4;
        public double VlsfoActualTotalConsumption => VlsfoActualConsumptionMe + VlsfoActualConsumptionDg1 + VlsfoActualConsumptionDg2 + VlsfoActualConsumptionDg3 + VlsfoActualConsumptionDg4;
        public double VlsfoPoolTotalConsumption => VlsfoPoolConsumptionMe + VlsfoPoolConsumptionDg1 + VlsfoPoolConsumptionDg2 + VlsfoPoolConsumptionDg3 + VlsfoPoolConsumptionDg4;
        public double AverageSpeedThroughTheWater => TotalSteamingTime > 0 ? SpeedlogDistance / TotalSteamingTime : 0;
        public double AverageSpeedOverGround => TotalSteamingTime > 0 ? DistanceOverGround / TotalSteamingTime : 0;
        public int? CargoTonage => Model.Event.Cargoes?.Sum(c => c.CargoTonnage);
        public double TotalAllFuelActualDaily => TotalSteamingTime > 0 ? ((LsmgoActualTotalConsumption + VlsfoActualTotalConsumption) * 24) / TotalSteamingTime : 0;
        public double TotalAllFuelPoolDaily => TotalSteamingTime > 0 ? ((LsmgoPoolTotalConsumption + VlsfoPoolTotalConsumption) * 24) / TotalSteamingTime : 0;
        public double TotalSteamingTime => Model.Performance != null ? Model.Performance.SteamingTime.Sum(t => t.EventId != Model.Event.Id ? 0 : t.SteamingTime) : 0;
        public double VlsfoActualTotalConsumptionThroughFlowMetersDiff => Model.Performance != null ? Math.Abs(VlsfoActualTotalConsumption - Model.Performance.TotalConsumption.ActualConsVLSFO) : 0;
        public double VlsfoPoolTotalConsumptionThroughFlowMetersDiff => Model.Performance != null ? Math.Abs(VlsfoPoolTotalConsumption - Model.Performance.TotalConsumption.PoolConsVLSFO) : 0;
        public double LsmgoActualTotalConsumptionThroughFlowMetersDiff => Model.Performance != null ? Math.Abs(LsmgoActualTotalConsumption - Model.Performance.TotalConsumption.ActualConsLSMGO) : 0;
        public double LsmgoPoolTotalConsumptionThroughFlowMetersDiff => Model.Performance != null ? Math.Abs(LsmgoPoolTotalConsumption - Model.Performance.TotalConsumption.PoolConsLSMGO) : 0;

        public double RobHfoActualWeightTotal =>
            Model.ReportFields
                .Where(rf => rf.Group != null && rf.Group.IsRobHfoActualGroup())
                .GroupBy(rf => rf.TankId)
                .Where(g => g.Any(x => x.IsViscosityField() && x.Value.ToDouble() > 80))
                .SelectMany(g => g.Where(x => x.IsWeightField()))
                .Sum(rf => rf.Value.ToDouble() ?? 0);

        public double RobHfoPoolWeightTotal =>
            Model.ReportFields
                .Where(rf => rf.Group != null && rf.Group.IsRobHfoPoolGroup())
                .GroupBy(rf => rf.TankId)
                .Where(g => g.Any(x => x.IsViscosityField() && x.Value.ToDouble() > 80))
                .SelectMany(g => g.Where(x => x.IsWeightField()))
                .Sum(rf => rf.Value.ToDouble() ?? 0);

        public double RobLfoActualWeightTotal =>
            Model.ReportFields
                .Where(rf => rf.Group != null && rf.Group.IsRobHfoActualGroup())
                .GroupBy(rf => rf.TankId)
                .Where(g => g.Any(x => x.IsViscosityField() && x.Value.ToDouble() <= 80))
                .SelectMany(g => g.Where(x => x.IsWeightField()))
                .Sum(rf => rf.Value.ToDouble() ?? 0);

        public double RobLfoPoolWeightTotal =>
            Model.ReportFields
                .Where(rf => rf.Group != null && rf.Group.IsRobHfoPoolGroup())
                .GroupBy(rf => rf.TankId)
                .Where(g => g.Any(x => x.IsViscosityField() && x.Value.ToDouble() <= 80))
                .SelectMany(g => g.Where(x => x.IsWeightField()))
                .Sum(rf => rf.Value.ToDouble() ?? 0);

        public double RobMgoActualWeightTotal =>
            Model.ReportFields
                .Where(rf => rf.Group != null && rf.Group.IsRobMgoActualGroup())
                .GroupBy(rf => rf.TankId)
                .SelectMany(g => g.Where(x => x.IsWeightField()))
                .Sum(rf => rf.Value.ToDouble() ?? 0);

        public double RobMgoPoolWeightTotal =>
            Model.ReportFields
                .Where(rf => rf.Group != null && rf.Group.IsRobMgoPoolGroup())
                .GroupBy(rf => rf.TankId)
                .SelectMany(g => g.Where(x => x.IsWeightField()))
                .Sum(rf => rf.Value.ToDouble() ?? 0);


    }
}
