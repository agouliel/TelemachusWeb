using System.Linq;

using Enums;

using Helpers;

using Telemachus.Business.Models.Events;
using Telemachus.Business.Models.Reports;
using Telemachus.Data.Models;
using Telemachus.Data.Models.Reports;

namespace Telemachus.Business.Services.Mappers
{
    public static class ReportMapper
    {
        public static ReportFieldBusinessModel ToBusinessModel(this ReportFieldDataModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new ReportFieldBusinessModel()
            {
                FieldId = model.Id,
                Group = model.Group,
                FieldName = model.Name,
                IsSubgroupMainField = model.IsSubgroupMain,
                ValidationKey = model.ValidationKey,
                Description = model.Description,
                TankName = model.UserTank?.TankName ?? model.Tank?.Name,
                TankId = model.Tank?.Id,
                Storage = model.Tank?.Storage,
                Settling = model.Tank?.Settling,
                Serving = model.Tank?.Serving,
                TankDisplayOrder = model.UserTank?.DisplayOrder,
                MaxValue = model.UserTank?.MaxCapacity,
                TankCapacity = model.UserTank?.MaxCapacity,
            };
        }

        public static ReportBusinessModel ToBusinessModel(this ReportDataModel report)
        {
            if (report == null)
            {
                return null;
            }

            return new ReportBusinessModel()
            {
                Id = report.Id,
                Event = report.Event?.ToBusinessModel(),
                ReportFields = report.FieldValues?.OrderBy(_ => _.ReportFieldId).Select(ToBusinessModel).ToList()
            };
        }

        public static ReportBusinessModel ToBusinessModel(this ReportCustomDataModel report)
        {
            if (report == null)
            {
                return null;
            }

            return new ReportBusinessModel()
            {
                Id = report.ReportId,
                Event = new EventBusinessModel()
                {
                    VoyageId = report.VoyageId,
                    ConditionName = report.EventConditionName,
                    EventTypeName = report.EventTypeName,
                    EventTypeBusinessId = report.EventTypeBusinessId,
                    ReportTypeId = report.ReportTypeId,
                    Id = report.EventId,
                    StatusId = report.StatusId,
                    Timestamp = report.Timestamp,
                    PortName = report.PortName,
                    PortIsEu = report.PortIsEuInt,
                    PortBusinessId = report.PortBusinessId,
                    BunkeringData = report.BunkeringData?.ToBusinessModel(),
                    Lat = report.Lat,
                    Lng = report.Lng,
                    Cargoes = report.Cargoes?.ToBusinessModel()
                },
                ReportFields = report.FieldValues?.OrderBy(_ => _.ReportFieldId).Select(ToBusinessModel).ToList()
            };
        }

        public static ReportFieldValueBusinessModel ToBusinessModel(this ReportFieldValueDataModel model)
        {
            if (model == null)
            {
                return null;
            }

            return new ReportFieldValueBusinessModel()
            {
                ReportId = model.ReportId,
                Id = model.Id,
                FieldName = model.ReportField.Name,
                TankName = model.ReportField.UserTank?.TankName ?? model.ReportField.Tank?.Name,
                TankId = model.ReportField.Tank?.Id,
                TankDisplayOrder = model.ReportField.UserTank?.DisplayOrder,
                TankCapacity = model.ReportField.UserTank?.MaxCapacity,
                Storage = model.ReportField.Tank?.Storage,
                Settling = model.ReportField.Tank?.Settling,
                Serving = model.ReportField.Tank?.Serving,
                Group = model.ReportField.Group,
                FieldId = model.ReportFieldId,
                Value = model.Value,
                IsSubgroupMainField = model.ReportField.IsSubgroupMain,
                ValidationKey = model.ReportField.ValidationKey,
                Description = model.ReportField.Description,
            };
        }
        public static ReportBusinessExtensionModel ToBusinessExtensionModel(
            this ReportBusinessModel model)
        {
            return new ReportBusinessExtensionModel(model);
        }
        public static MrvMisDataModel ToMrvDataModel(
            this ReportBusinessModel report, string voyageBusinessId)
        {
            var extReport = report.ToBusinessExtensionModel();

            var tonnage = extReport.CargoTonage;

            var voyageDetails = new MrvMisDataModel();

            voyageDetails.Vessel = report.Event.UserName;
            voyageDetails.Voyage = voyageBusinessId;
            voyageDetails.LegId = voyageBusinessId;
            voyageDetails.EventId = report.Event.Id;
            voyageDetails.EventTimestamp = report.Event.Timestamp?.DateTime;
            voyageDetails.StatusId = report.Event.StatusId;
            voyageDetails.EventName = report.Event.EventTypeName;
            voyageDetails.ConditionName = report.Event.ConditionName;
            voyageDetails.CargoStatus = tonnage > 0 ? 1 : 0;
            voyageDetails.PortName = report.Event.PortName;
            voyageDetails.ReportId = report.Id;
            voyageDetails.Cargo = tonnage > 1 ? tonnage : 0;
            voyageDetails.PortBusinessId = report.Event.PortBusinessId;
            voyageDetails.IsEu = report.Event.PortIsEu;
            voyageDetails.Lat = (double?)report.Event.Lat;
            voyageDetails.Long = (double?)report.Event.Lng;
            voyageDetails.EventTimestampUtc = report.Event.Timestamp?.UtcDateTime;

            #region performance

            voyageDetails.MainEngineMaxPower = extReport.MainEngineMaxPower;
            voyageDetails.PitchPropeller = extReport.PitchPropeller;
            voyageDetails.TotalDistanceOverGround = extReport.TotalDistanceOverGround;
            voyageDetails.ActualConsVlsfo = extReport.ActualConsVLSFO;
            voyageDetails.ActualConsLsmgo = extReport.ActualConsLSMGO;
            voyageDetails.PoolConsVlsfo = extReport.PoolConsVLSFO;
            voyageDetails.PoolConsLsmgo = extReport.PoolConsLSMGO;

            voyageDetails.AirCoolerCoolingWaterInletTemp = extReport.AirCoolerCoolingWaterInletTemp.ToString();
            voyageDetails.AuxBlowerStatus = extReport.AuxBlowerStatus;
            voyageDetails.BarometricPressure = extReport.BarometricPressure.ToString();
            voyageDetails.BilgeRob = extReport.BilgeRob.ToString();
            voyageDetails.CylinderOil = extReport.CylinderOil.ToString();
            voyageDetails.DgOil = extReport.DgOil.ToString();
            voyageDetails.Dg1Load = extReport.Dg1Load.ToString();
            voyageDetails.Dg2Load = extReport.Dg2Load.ToString();
            voyageDetails.Dg3Load = extReport.Dg3Load.ToString();
            voyageDetails.Dg4Load = extReport.Dg4Load.ToString();
            voyageDetails.DistanceOverGround = extReport.DistanceOverGround.ToString();
            voyageDetails.DistanceToGo = extReport.DistanceToGo.ToString();
            voyageDetails.DistilledWater = extReport.DistilledWater.ToString();
            voyageDetails.DraftAft = extReport.DraftAft.ToString();
            voyageDetails.DraftFwd = extReport.DraftFwd.ToString();
            voyageDetails.ErTemp = extReport.ErTemp.ToString();
            voyageDetails.EgeSteamPress = extReport.EgeSteamPress.ToString();
            voyageDetails.ExhaustGasTempAfterTc = extReport.ExhaustGasTempAfterTc.ToString();
            voyageDetails.ExhaustGasTempBeforeTc = extReport.ExhaustGasTempBeforeTc.ToString();
            voyageDetails.ForecastWeather = extReport.ForecastWeather;
            voyageDetails.FreshWaterDomestic = extReport.FreshWaterDomestic.ToString();
            voyageDetails.FreshWaterPotable = extReport.FreshWaterPotable.ToString();
            voyageDetails.FreshWaterTankCleaning = extReport.FreshWaterTankCleaning.ToString();
            voyageDetails.FuelOilPumpIndex = extReport.FuelOilPumpIndex.ToString();
            voyageDetails.FwProduction = extReport.FwProduction.ToString();
            voyageDetails.InstructedChartererConsumption = extReport.InstructedChartererConsumption.ToString();
            voyageDetails.InstructedSpeed = extReport.InstructedSpeed.ToString();
            voyageDetails.LsmgoActualConsumptionBoiler1 = extReport.LsmgoActualConsumptionBoiler1.ToString();
            voyageDetails.LsmgoActualConsumptionBoiler2 = extReport.LsmgoActualConsumptionBoiler2.ToString();
            voyageDetails.LsmgoActualConsumptionCompositeBoiler = extReport.LsmgoActualConsumptionCompositeBoiler.ToString();
            voyageDetails.LsmgoActualConsumptionCummins = extReport.LsmgoActualConsumptionCummins.ToString();
            voyageDetails.LsmgoActualConsumptionDg1 = extReport.LsmgoActualConsumptionDg1.ToString();
            voyageDetails.LsmgoActualConsumptionDg2 = extReport.LsmgoActualConsumptionDg2.ToString();
            voyageDetails.LsmgoActualConsumptionDg3 = extReport.LsmgoActualConsumptionDg3.ToString();
            voyageDetails.LsmgoActualConsumptionDg4 = extReport.LsmgoActualConsumptionDg4.ToString();
            voyageDetails.LsmgoActualConsumptionIgg = extReport.LsmgoActualConsumptionIgg.ToString();
            voyageDetails.LsmgoActualConsumptionMe = extReport.LsmgoActualConsumptionMe.ToString();
            voyageDetails.LsmgoPoolConsumptionBoiler1 = extReport.LsmgoPoolConsumptionBoiler1.ToString();
            voyageDetails.LsmgoPoolConsumptionBoiler2 = extReport.LsmgoPoolConsumptionBoiler2.ToString();
            voyageDetails.LsmgoPoolConsumptionCompositeBoiler = extReport.LsmgoPoolConsumptionCompositeBoiler.ToString();
            voyageDetails.LsmgoPoolConsumptionCummins = extReport.LsmgoPoolConsumptionCummins.ToString();
            voyageDetails.LsmgoPoolConsumptionDg1 = extReport.LsmgoPoolConsumptionDg1.ToString();
            voyageDetails.LsmgoPoolConsumptionDg2 = extReport.LsmgoPoolConsumptionDg2.ToString();
            voyageDetails.LsmgoPoolConsumptionDg3 = extReport.LsmgoPoolConsumptionDg3.ToString();
            voyageDetails.LsmgoPoolConsumptionDg4 = extReport.LsmgoPoolConsumptionDg4.ToString();
            voyageDetails.LsmgoPoolConsumptionIgg = extReport.LsmgoPoolConsumptionIgg.ToString();
            voyageDetails.LsmgoPoolConsumptionMe = extReport.LsmgoPoolConsumptionMe.ToString();
            voyageDetails.MeOilOil = extReport.MeOilOil.ToString();
            voyageDetails.MainEngineRevolutionOutputCounter = extReport.MainEngineRevolutionOutputCounter.ToString();
            voyageDetails.ObservedWeather = extReport.ObservedWeather;
            voyageDetails.Oop = extReport.Oop.ToString();
            voyageDetails.RpmDeclared = extReport.RpmDeclared.ToString();
            voyageDetails.RunningHoursBoiler1Lsmgo = extReport.RunningHoursBoiler1Lsmgo.ToString();
            voyageDetails.RunningHoursBoiler1Vlsfo = extReport.RunningHoursBoiler1Vlsfo.ToString();
            voyageDetails.RunningHoursBoiler2Lsmgo = extReport.RunningHoursBoiler2Lsmgo.ToString();
            voyageDetails.RunningHoursBoiler2Vlsfo = extReport.RunningHoursBoiler2Vlsfo.ToString();
            voyageDetails.RunningHoursCargoCumminsLsmgo = extReport.RunningHoursCargoCumminsLsmgo.ToString();
            voyageDetails.RunningHoursCompositeBoilerLsmgo = extReport.RunningHoursCompositeBoilerLsmgo.ToString();
            voyageDetails.RunningHoursCompositeBoilerVlsfo = extReport.RunningHoursCompositeBoilerVlsfo.ToString();
            voyageDetails.RunningHoursDg1Lsmgo = extReport.RunningHoursDg1Lsmgo.ToString();
            voyageDetails.RunningHoursDg1Vlsfo = extReport.RunningHoursDg1Vlsfo.ToString();
            voyageDetails.RunningHoursDg2Lsmgo = extReport.RunningHoursDg2Lsmgo.ToString();
            voyageDetails.RunningHoursDg2Vlsfo = extReport.RunningHoursDg2Vlsfo.ToString();
            voyageDetails.RunningHoursDg3Lsmgo = extReport.RunningHoursDg3Lsmgo.ToString();
            voyageDetails.RunningHoursDg3Vlsfo = extReport.RunningHoursDg3Vlsfo.ToString();
            voyageDetails.RunningHoursDg4Lsmgo = extReport.RunningHoursDg4Lsmgo.ToString();
            voyageDetails.RunningHoursDg4Vlsfo = extReport.RunningHoursDg4Vlsfo.ToString();
            voyageDetails.RunningHoursIggLsmgoRunningHours = extReport.RunningHoursIggLsmgo.ToString();
            voyageDetails.ScavengeAirInletTemp = extReport.ScavengeAirInletTemp.ToString();
            voyageDetails.ScavengeAirPressure = extReport.ScavengeAirPressure.ToString();
            voyageDetails.SeaCurrentSpeed = extReport.SeaCurrentSpeed.ToString();
            voyageDetails.SeaState = extReport.SeaState;
            voyageDetails.ShaftPowerShapoli = extReport.ShaftPowerShapoli.ToString();
            voyageDetails.SlipDeclared = extReport.SlipDeclared.ToString();
            voyageDetails.Slops = extReport.Slops.ToString();
            voyageDetails.SludgeRob = extReport.SludgeRob.ToString();
            voyageDetails.SpeedlogDistance = extReport.SpeedlogDistance.ToString();
            voyageDetails.SteamingTime = extReport.SteamingTime.ToString();
            voyageDetails.SwellDirection = extReport.SwellDirection;
            voyageDetails.TcAirInletTemp = extReport.TcAirInletTemp.ToString();
            voyageDetails.TurboChargerRpm = extReport.TurboChargerRpm.ToString();
            voyageDetails.VesselCurrentList = extReport.VesselCurrentList.ToString();
            voyageDetails.VesselCurrentTrim = extReport.VesselCurrentTrim.ToString();
            voyageDetails.VlsfoActualConsumptionBoiler1 = extReport.VlsfoActualConsumptionBoiler1.ToString();
            voyageDetails.VlsfoActualConsumptionBoiler2 = extReport.VlsfoActualConsumptionBoiler2.ToString();
            voyageDetails.VlsfoActualConsumptionCompositeBoiler = extReport.VlsfoActualConsumptionCompositeBoiler.ToString();
            voyageDetails.VlsfoActualConsumptionDg1 = extReport.VlsfoActualConsumptionDg1.ToString();
            voyageDetails.VlsfoActualConsumptionDg2 = extReport.VlsfoActualConsumptionDg2.ToString();
            voyageDetails.VlsfoActualConsumptionDg3 = extReport.VlsfoActualConsumptionDg3.ToString();
            voyageDetails.VlsfoActualConsumptionDg4 = extReport.VlsfoActualConsumptionDg4.ToString();
            voyageDetails.VlsfoActualConsumptionMe = extReport.VlsfoActualConsumptionMe.ToString();
            voyageDetails.VlsfoPoolConsumptionBoiler1 = extReport.VlsfoPoolConsumptionBoiler1.ToString();
            voyageDetails.VlsfoPoolConsumptionBoiler2 = extReport.VlsfoPoolConsumptionBoiler2.ToString();
            voyageDetails.VlsfoPoolConsumptionCompositeBoiler = extReport.VlsfoPoolConsumptionCompositeBoiler.ToString();
            voyageDetails.VlsfoPoolConsumptionDg1 = extReport.VlsfoPoolConsumptionDg1.ToString();
            voyageDetails.VlsfoPoolConsumptionDg2 = extReport.VlsfoPoolConsumptionDg2.ToString();
            voyageDetails.VlsfoPoolConsumptionDg3 = extReport.VlsfoPoolConsumptionDg3.ToString();
            voyageDetails.VlsfoPoolConsumptionDg4 = extReport.VlsfoPoolConsumptionDg4.ToString();
            voyageDetails.VlsfoPoolConsumptionMe = extReport.VlsfoPoolConsumptionMe.ToString();
            voyageDetails.WindDirection = extReport.WindDirection;
            voyageDetails.WindForce = extReport.WindForce.ToString();
            voyageDetails.WindForceForecast = extReport.WindForceForecast.ToString();

            #endregion

            #region tanks

            voyageDetails.RobHfoActual = extReport.RobHfoActualWeightTotal;
            voyageDetails.RobHfoPool = extReport.RobHfoPoolWeightTotal;
            voyageDetails.RobMgoActual = extReport.RobMgoActualWeightTotal;
            voyageDetails.RobMgoPool = extReport.RobMgoPoolWeightTotal;
            voyageDetails.RobLfoActual = extReport.RobLfoActualWeightTotal;
            voyageDetails.RobLfoPool = extReport.RobLfoPoolWeightTotal;

            #endregion region

            #region processed fields

            voyageDetails.AverageSpeedThroughTheWater = extReport.AverageSpeedThroughTheWater;
            voyageDetails.AverageSpeedOverGround = extReport.AverageSpeedOverGround;
            voyageDetails.TotalConsumptionEstimated = extReport.TotalAllFuelActualDaily;
            voyageDetails.TotalConsumptionDeclared = extReport.TotalAllFuelPoolDaily;
            voyageDetails.VlsfoActualConsumptionThroughFlowMetersDiff = extReport.VlsfoActualTotalConsumptionThroughFlowMetersDiff;
            voyageDetails.VlsfoPoolConsumptionThroughFlowMetersDiff = extReport.VlsfoPoolTotalConsumptionThroughFlowMetersDiff;
            voyageDetails.LsmgoActualConsumptionThroughFlowMetersDiff = extReport.LsmgoActualTotalConsumptionThroughFlowMetersDiff;
            voyageDetails.LsmgoPoolConsumptionThroughFlowMetersDiff = extReport.LsmgoPoolTotalConsumptionThroughFlowMetersDiff;

            #endregion

            #region bunkering fields

            var bd = report.Event.BunkeringData;

            if (bd != null && EventType.CommenceBunkeringGroup.Contains(report.Event.EventTypeBusinessId.Value))
            {
                voyageDetails.Bdn = report.Event.BunkeringData?.Bdn;

                var totalAmount = bd.TotalAmount.ToDouble();

                if (bd.FuelType == 2)
                {
                    voyageDetails.BunkeringLsmgo = totalAmount;
                }
                else
                {
                    var viscosity = bd.Viscosity.ToDouble();
                    if (viscosity > 0 && viscosity < 180.00)
                    {
                        voyageDetails.BunkeringLfo = totalAmount;
                    }
                    else
                    {
                        voyageDetails.BunkeringHfo = totalAmount;
                    }
                }

                voyageDetails.SulphurContent = bd.SulphurContent;
                voyageDetails.Density = bd.Density;
                voyageDetails.KinematicViscosity = bd.Viscosity;

            }
            #endregion
            return voyageDetails;
        }
    }
}
