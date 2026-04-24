export const requiredTabs = ['auxTanks', 'distance', 'me', 'weather']
const performanceTabs = [
  {
    key: 'auxTanks',
    label: '',
    tables: [
      {
        key: 1,
        cols: [
          { label: 'Fresh water domestic', name: 'freshWaterDomestic' },
          { label: 'Fresh water potable', name: 'freshWaterPotable' },
          { label: 'Fresh water tank cleaning', name: 'freshWaterTankCleaning' },
          { label: 'Distilled water', name: 'distilledWater' },
          { label: 'Sludge', name: 'sludgeRob' },
          { label: 'Bilge water', name: 'bilgeRob' },
          { label: 'Slops', name: 'slops' },
          { label: 'Cylinder oil', name: 'cylinderOil' },
          { label: 'M/E oil', name: 'm/eOilOil' },
          { label: 'D/G oil', name: 'd/gOil' },
          { label: 'FW Production', name: 'fwProduction' }
        ]
      }
    ],
    subValues: [
      {
        name: 'auxTanks',
        label: 'Aux Tanks'
      }
    ]
  },
  {
    key: 'runningHours',
    label: 'Running Hours',
    subValues: [
      {
        name: 'steamingTimeVLSFO',
        label: 'VLSFO Steaming Time',
        tables: [
          {
            key: 1,
            label: 'Machinery Running Hours (VLSFO)',
            cols: [
              { label: 'DG1', name: 'runningHours_DG1_VLSFO', min: 0 },
              { label: 'DG2', name: 'runningHours_DG2_VLSFO', min: 0 },
              { label: 'DG3', name: 'runningHours_DG3_VLSFO', min: 0 },
              { label: 'DG4', name: 'runningHours_DG4_VLSFO', min: 0 },
              { label: 'Boiler 1', name: 'runningHours_Boiler_1_VLSFO', min: 0 },
              { label: 'Boiler 2', name: 'runningHours_Boiler_2_VLSFO', min: 0 },
              {
                label: 'Composite Boiler',
                name: 'runningHours_Composite_Boiler_VLSFO',
                min: 0
              }
            ]
          }
        ]
      },
      {
        name: 'steamingTimeLSMGO',
        label: 'MGO Steaming Time',
        tables: [
          {
            key: 1,
            label: 'Machinery Running Hours (LSMGO)',
            cols: [
              { label: 'DG1', name: 'runningHours_DG1_LSMGO', min: 0 },
              { label: 'DG2', name: 'runningHours_DG2_LSMGO', min: 0 },
              { label: 'DG3', name: 'runningHours_DG3_LSMGO', min: 0 },
              { label: 'DG4', name: 'runningHours_DG4_LSMGO', min: 0 },
              { label: 'Boiler 1', name: 'runningHours_Boiler_1_LSMGO', min: 0 },
              { label: 'Boiler 2', name: 'runningHours_Boiler_2_LSMGO', min: 0 },
              {
                label: 'Composite Boiler',
                name: 'runningHours_Composite_Boiler_LSMGO',
                min: 0
              },
              {
                label: 'IGG',
                name: 'runningHours_IGG_LSMGO_Running_Hours',
                min: 0
              },
              { label: 'Cargo Cummins', name: 'runningHours_Cargo_Cummins_LSMGO', min: 0 }
            ]
          }
        ]
      },
      { name: 'steamingTime', label: 'Total Hours from last Event' },
      { name: 'steamingTimeTotal', label: 'Total Hours from last COSP' }
    ]
  },
  {
    label: 'Distance',
    key: 'distance',
    subValues: [
      {
        name: 'distanceOverGround',
        label: 'Distance Over Ground',
        tables: [
          {
            key: 1,
            cols: [
              {
                label: 'Distance over ground',
                name: 'distanceOverGround',
                min: 0
              },
              {
                label: 'Distance to go',
                name: 'distanceToGo'
              },
              {
                label: 'SpeedLog Distance',
                name: 'speedlogDistance'
              },
              {
                label: 'Total Distance over ground from last COSP',
                name: 'totalDistanceOverGroundLastCosp'
              }
            ]
          }
        ]
      },
      {
        name: 'averageSpeedOverGround',
        label: 'Average Speed Over Ground',
        tables: [
          {
            key: 1,
            cols: [
              {
                label: 'Instructed Speed',
                name: 'instructedSpeed',
                min: 10,
                max: 15,
                step: '0.5'
              },
              {
                label: 'Pool Index',
                name: 'instructedChartererConsumption'
              },

              {
                label: 'Average Speed Over Ground',
                name: 'averageSpeedOverGround'
              },
              {
                label: 'Average Speed Through The Water',
                name: 'averageSpeedThroughTheWater'
              }
            ]
          }
        ]
      }
    ]
  },
  {
    key: 'me',
    label: 'Machinery',
    tables: [
      {
        key: 1,
        cols: [
          { label: 'Rev. Output Counter (since COSP)', name: 'mainEngineRevolutionOutputCounter' },
          { label: 'Shaft Power Shapoli avg since last report', name: 'shaftPowerShapoli' },
          { label: 'Turbo Charger RPM', name: 'turboChargerRpm', min: 0, max: 16000 },
          {
            name: 'engineDistance',
            label: 'Engine Distance'
          },
          {
            name: 'engineSpeed',
            label: 'Engine Speed'
          },
          { label: 'RPM Estimated', name: 'averageRpm', min: 0, hidden: true },
          { label: 'RPM Declared', name: 'rpmDeclared', min: 0 },
          { label: 'Slip % Estimated', name: 'slip', hidden: true },
          { label: 'Slip % Declared', name: 'slipDeclared' },
          { label: 'Fuel Oil Pump Index', name: 'fuelOilPumpIndex', min: 0, max: 100 },
          { label: 'Scavenge Air Pressure', name: 'scavengeAirPressure', min: 0, max: 100 },
          { label: 'Scavenge Air Inlet Temp', name: 'scavengeAirInletTemp', min: 0, max: 100 },
          { label: 'T/C Air Inlet Temp', name: 't/cAirInletTemp', min: 0, max: 100 },
          {
            label: 'Air Cooler Cooling Water Inlet Temp',
            name: 'airCoolerCoolingWaterInletTemp',
            min: 0,
            max: 100
          },
          {
            label: 'Exhaust Gas Temp Before T/C',
            name: 'exhaustGasTempBeforeT/c',
            min: 0,
            max: 600
          },
          {
            label: 'Exhaust Gas Temp After T/C',
            name: 'exhaustGasTempAfterT/c',
            min: 0,
            max: 400
          },
          { label: 'E/R Temp', name: 'e/rTemp', min: 0, max: 100 },
          { label: 'EGE Steam Press	', name: 'egeSteamPress', min: 0, max: 10 },
          {
            label: 'Aux Blower Status',
            name: 'auxBlowerStatus',
            values: ['On', 'Off'].map(v => ({ value: v, label: v }))
          },
          { label: 'DG1 Load', name: 'dg1Load' },
          { label: 'DG2 Load', name: 'dg2Load' },
          { label: 'DG3 Load', name: 'dg3Load' },
          { label: 'DG4 Load', name: 'dg4Load' },
          { label: 'Barometric Pressure', name: 'barometricPressure', min: 900, max: 1100 }
        ]
      }
    ],
    subValues: [
      {
        key: 1,
        name: 'averageRpm',
        hidden: true,
        label: 'RPM Estimated'
      },
      {
        key: 2,
        name: 'rpmDeclared',
        label: 'RPM Declared'
      },
      { key: 3, name: 'slip', label: 'Slip % Estimated', hidden: true },
      { key: 4, name: 'slipDeclared', label: 'Slip % Declared' }
    ]
  },
  {
    key: 'totalConsumptions',
    label: 'Total Consumptions',
    subValues: [
      {
        name: 'actualTotalConsumption_hfo',
        label: 'VLSFO Estimated',
        hidden: true,
        tables: [
          {
            key: 1,
            cols: [
              {
                label: 'VLSFO Actual Difference',
                name: 'VLSFOActualConsumptionThroughFlowMetersDiff'
              },
              {
                label: 'VLSFO Estimated (Survey)',
                name: 'actualTotalConsumption_hfo'
              },
              {
                label: 'VLSFO Estimated (Cons)',
                name: 'actualTotalConsumptionDeclared_hfo'
              },
              { name: 'vlsfo_actual_consumption_me', label: 'VLSFO Estimated ME	' },
              { name: 'vlsfo_actual_consumption_dg1', label: 'VLSFO Estimated DG1	' },
              { name: 'vlsfo_actual_consumption_dg2', label: 'VLSFO Estimated DG2	' },
              { name: 'vlsfo_actual_consumption_dg3', label: 'VLSFO Estimated DG3	' },
              { name: 'vlsfo_actual_consumption_dg4', label: 'VLSFO Estimated DG4	' },
              { name: 'vlsfo_actual_consumption_boiler 1', label: 'VLSFO Estimated Boiler 1	' },
              { name: 'vlsfo_actual_consumption_boiler 2', label: 'VLSFO Estimated Boiler 2	' },
              {
                name: 'vlsfo_actual_consumption_composite boiler',
                label: 'VLSFO Actual Composite Boiler	'
              }
            ]
          }
        ]
      },
      {
        name: 'poolTotalConsumption_hfo',
        label: 'VLSFO Declared',
        tables: [
          {
            key: 1,
            cols: [
              {
                label: 'VLSFO Declared Difference',
                name: 'VLSFOPoolConsumptionThroughFlowMetersDiff'
              },
              {
                label: 'VLSFO Declared (Survey)',
                name: 'poolTotalConsumption_hfo'
              },
              {
                label: 'VLSFO Declared (Cons)',
                name: 'poolTotalConsumptionDeclared_hfo'
              },
              { name: 'vlsfo_pool_consumption_me', label: 'VLSFO Declared ME ' },
              { name: 'vlsfo_pool_consumption_dg1', label: 'VLSFO Declared DG1 ' },
              { name: 'vlsfo_pool_consumption_dg2', label: 'VLSFO Declared DG2 ' },
              { name: 'vlsfo_pool_consumption_dg3', label: 'VLSFO Declared DG3 ' },
              { name: 'vlsfo_pool_consumption_dg4', label: 'VLSFO Declared DG4 ' },
              { name: 'vlsfo_pool_consumption_boiler 1', label: 'VLSFO Declared Boiler 1 ' },
              { name: 'vlsfo_pool_consumption_boiler 2', label: 'VLSFO Declared Boiler 2 ' },
              {
                name: 'vlsfo_pool_consumption_composite boiler',
                label: 'VLSFO Declared Composite Boiler	'
              }
            ]
          }
        ]
      },
      {
        name: 'actualTotalConsumption_mgo',
        label: 'LSMGO Estimated',
        hidden: true,
        tables: [
          {
            key: 1,
            cols: [
              {
                label: 'LSMGO Estimated Difference',
                name: 'LSMGOActualConsumptionThroughFlowMetersDiff'
              },
              {
                label: 'LSMGO Estimated (Survey)',
                name: 'actualTotalConsumption_mgo'
              },
              {
                label: 'LSMGO Estimated (Cons)',
                name: 'actualTotalConsumptionDeclared_mgo'
              },
              { name: 'lsmgo_actual_consumption_me', label: 'LSMGO Estimated ME	' },
              { name: 'lsmgo_actual_consumption_dg1', label: 'LSMGO Estimated DG1	' },
              { name: 'lsmgo_actual_consumption_dg2', label: 'LSMGO Estimated DG2	' },
              { name: 'lsmgo_actual_consumption_dg3', label: 'LSMGO Estimated DG3	' },
              { name: 'lsmgo_actual_consumption_dg4', label: 'LSMGO Estimated DG4	' },
              { name: 'lsmgo_actual_consumption_boiler 1', label: 'LSMGO Estimated Boiler 1	' },
              { name: 'lsmgo_actual_consumption_boiler 2', label: 'LSMGO Estimated Boiler 2	' },
              { name: 'lsmgo_actual_consumption_dge', label: 'LSMGO Estimated Emergency DG	' },
              {
                name: 'lsmgo_actual_consumption_composite boiler',
                label: 'LSMGO Estimated Composite Boiler	'
              },
              { name: 'lsmgo_actual_consumption_inc', label: 'LSMGO Estimated Incinerator ' },
              { name: 'lsmgo_actual_consumption_igg', label: 'LSMGO Estimated IGG	' },
              { name: 'lsmgo_actual_consumption_cummins', label: 'LSMGO Estimated Cummins	' }
            ]
          }
        ]
      },
      {
        name: 'poolTotalConsumption_mgo',
        label: 'LSMGO Declared',
        tables: [
          {
            key: 1,
            cols: [
              {
                label: 'LSMGO Declared Difference',
                name: 'LSMGOPoolConsumptionThroughFlowMetersDiff'
              },
              {
                label: 'LSMGO Declared (Survey)',
                name: 'poolTotalConsumption_mgo'
              },
              {
                label: 'LSMGO Declared (Cons)',
                name: 'poolTotalConsumptionDeclared_mgo'
              },
              { name: 'lsmgo_pool_consumption_me', label: 'LSMGO Declared ME	' },
              { name: 'lsmgo_pool_consumption_dg1', label: 'LSMGO Declared DG1	' },
              { name: 'lsmgo_pool_consumption_dg2', label: 'LSMGO Declared DG2	' },
              { name: 'lsmgo_pool_consumption_dg3', label: 'LSMGO Declared DG3	' },
              { name: 'lsmgo_pool_consumption_dg4', label: 'LSMGO Declared DG4	' },
              { name: 'lsmgo_pool_consumption_boiler 1', label: 'LSMGO Declared Boiler 1	' },
              { name: 'lsmgo_pool_consumption_boiler 2', label: 'LSMGO Declared Boiler 2	' },
              { name: 'lsmgo_pool_consumption_dge', label: 'LSMGO Declared Emergency DG	' },
              {
                name: 'lsmgo_pool_consumption_composite boiler',
                label: 'LSMGO Declared Composite Boiler	'
              },
              { name: 'lsmgo_pool_consumption_inc', label: 'LSMGO Declared Incinerator ' },
              { name: 'lsmgo_pool_consumption_igg', label: 'LSMGO Declared IGG	' },
              { name: 'lsmgo_pool_consumption_cummins', label: 'LSMGO Declared Cummins	' }
            ]
          }
        ]
      },
      {
        label: 'Total All Fuel Estimated/24',
        hidden: true,
        name: 'totalAllFuelActualSteamingConsumptionDaily'
      },
      {
        label: 'Total All Fuel Declared/24',
        name: 'totalAllFuelPoolSteamingConsumptionDaily'
      }
    ]
  },
  {
    key: 'weather',
    label: 'Weather',
    tables: [
      {
        key: 1,
        cols: [
          {
            label: 'Forecast Weather',
            name: 'forecastWeather',
            values: [
              'CLOUDY SKY',
              'OVERCAST SKY',
              'CLEAR SKY',
              'PARTLY CLOUDY',
              'MOSTLY CLOUDY',
              'LIGHT RAIN',
              'HEAVY RAIN',
              'THUNDERSTORMS',
              'LIGHT SNOW',
              'HEAVY SNOW',
              'FOG',
              'MIST',
              'HAZE',
              'WINDY',
              'CALM',
              'SCATTERED SHOWERS',
              'ISOLATED THUNDERSTORMS',
              'DRIZZLE',
              'FREEZING RAIN',
              'SLEET',
              'HAIL',
              'STORMY',
              'HUMID',
              'DRY'
            ].map(v => ({ value: v, label: v }))
          },
          {
            label: 'Observed Weather',
            name: 'observedWeather'
          },
          {
            label: 'OOP',
            name: 'oop'
          },
          {
            label: 'Wind Force Observed',
            name: 'windForce',
            values: ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12'].map(v => ({
              value: v,
              label: v
            }))
          },
          {
            label: 'Wind Force Forecast',
            name: 'windForceForecast',
            values: ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12'].map(v => ({
              value: v,
              label: v
            }))
          },
          {
            label: 'Wind Direction',
            name: 'windDirection',
            values: ['N', 'NW', 'W', 'SW', 'S', 'SE', 'E', 'NE', 'N'].map(v => ({
              value: v,
              label: v
            }))
          },
          {
            label: 'Sea State',
            name: 'seaState',
            values: [
              'CALM (GLASSY)',
              'CALM (RIPPLED)',
              'SMOOTH',
              'SLIGHT',
              'MODERATE',
              'ROUGH',
              'VERY ROUGH',
              'HIGH',
              'VERY HIGH',
              'PHENOMENAL',
              'NOT APPLICABLE'
            ].map(v => ({ value: v, label: v }))
          },
          {
            label: 'Swell Direction',
            name: 'swellDirection',
            values: [
              'N/NW',
              'NW',
              'W/NW',
              'W',
              'W/SW',
              'SW',
              'S/SW',
              'S',
              'S/SE',
              'SE',
              'E/SE',
              'E',
              'E/NE',
              'NE',
              'N/NE',
              'N'
            ].map(v => ({ value: v, label: v }))
          },
          {
            label: 'Sea Current Speed',
            name: 'seaCurrentSpeed'
          }
        ]
      }
    ],
    subValues: [
      { name: 'windForce', label: 'Wind Force Observed' },
      { name: 'windForceForecast', label: 'Wind Force Forecast' },
      { name: 'oop', label: 'OOP' }
    ]
  },
  {
    label: '',
    tables: [
      {
        key: 1,
        cols: [
          { label: 'Draft Fwd(m)', name: 'draftFwd' },
          { label: 'Draft Aft(m)', name: 'draftAft' },
          { label: "Vessel's current Trim", name: 'vesselCurrentTrim' },
          { label: "Vessel's current List", name: 'vesselCurrentList' }
        ]
      }
    ],
    subValues: [
      {
        label: 'Draft Survey',
        name: 'draftSurvey'
      }
    ]
  }
]

export const performanceFields = performanceTabs.flatMap(tab =>
  (tab.tables?.flatMap(table => table.cols ?? []) ?? []).concat(
    tab.subValues?.flatMap(subValue => subValue.tables?.flatMap(table => table.cols ?? []) ?? []) ??
      []
  )
)

export default performanceTabs
