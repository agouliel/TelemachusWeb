class EventTypes {
  static CommenceMooring = '8945f8cf-5cdd-4ccb-9b6a-f082cead9e68'

  static CompleteMooring = '3fe72760-5a1e-456e-b928-9750eaa265c2'

  static Other = 'b8189512-c1a5-4c40-a847-94849e690fb7'

  static EOSP = 'd470ac75-50f5-4451-a6df-aa61b2187cb7'

  static COSP = '7e826887-6c05-4626-b908-ab461e2eb149'

  static CommenceLoadingParcel = '67b9ab9b-2919-438b-875e-dc643852dacb'

  static CompleteLoadingParcel = '1fc89ab3-9ef4-4bff-8fee-bb52adcc67e6'

  static CommenceDischargingParcel = '4ed1ed3f-5d53-4821-bd66-1ec0b1e9233c'

  static CompleteDischargingParcel = '69592c91-4302-4553-a93e-b777a1f3f8f4'

  static BunkeringPlan = 'd5e77ba7-2979-490a-9f62-8f6983d7e784'

  static BunkeringPlanProjected = 'd796cbc0-c20d-4b0e-9c87-f1199fe3f400'

  static CommenceBunkering = 'a756a087-9124-4dd2-9cbb-98a4d86c2539'

  static CompleteBunkering = '71b241f1-bfbc-4ee0-b105-363c527f0c7d'

  static CommenceInternalTransfer = 'd0cb967f-bdad-4ce4-aea4-677907cad6ee'

  static CompleteInternalTransfer = '71b453e4-67e1-4f5a-b042-742712d11dc1'

  static Noon = '42e15cd2-a7fc-412e-907c-f9249c70540f'

  // GROUPS

  static ParcelGroup = [
    EventTypes.CommenceLoadingParcel,
    EventTypes.CompleteLoadingParcel,
    EventTypes.CommenceDischargingParcel,
    EventTypes.CompleteDischargingParcel
  ]

  static BunkeringPlanGroup = [EventTypes.BunkeringPlan, EventTypes.BunkeringPlanProjected]

  static BunkeringGroup = [EventTypes.CommenceBunkering, EventTypes.CompleteBunkering]

  static BunkeringGroups = [...EventTypes.BunkeringPlanGroup, ...EventTypes.BunkeringGroup]
}

export const evenTypes = Object.entries(EventTypes)
  .filter(([_label, value]) => !Array.isArray(value))
  .map(([label, value]) => ({
    label,
    value
  }))

export const getEventTypeLabel = eventType =>
  evenTypes.find(e => e.value === eventType)?.label ?? null

export default EventTypes
