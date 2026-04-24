# Telemachus REST API - Documentation

Authentication Type: **JWT**

**[POST] `/login/token`**

**Request Body:** `object`

| Property | Type     | Nullable | Required |
| :------- | :------- | :------- | :------- |
| UserName | `string` | false    | true     |
| Password | `string` | false    | true     |

**Response Body:** `object`

| Property | Type     | Nullable | Optional |
| :------- | :------- | :------- | :------- |
| Token    | `string` | false    | false    |
| Role     | `guid`   | false    | false    |
| UserName | `string` | false    | false    |

**[POST] `/Event/Facts?Page={number}&PageSize={number}`**

**Query params:**

| Property | Type     | Optional | Default |
| :------- | :------- | :------- | :------ |
| Page     | `number` | true     | 1       |
| PageSize | `number` | true     | 5       |

**Request Body:** `object`

| Property       | Type                             | Nullable |
| :------------- | :------------------------------- | :------- |
| EventTypeIds   | `number[]`                       | false    |
| EventStatuses  | [`number[]`](#event-status-enum) | false    |
| DateFrom       | `string`                         | false    |
| DateTo         | `string`                         | true     |
| EventIdsToSkip | `number[]`                       | false    |

## Event Status Enum

| Property   | Value |
| :--------- | :---- |
| InProgress | 1     |
| Completed  | 2     |
| Rejected   | 3     |
| Approved   | 4     |

**Response Body:** `object`

| Property   | Type                                       |
| :--------- | :----------------------------------------- |
| Items      | [`object`](#condition-events-object-model) |
| TotalCount | `number`                                   |

## Condition Events Object Model

| Property              | Type                              | Nullable |
| :-------------------- | :-------------------------------- | :------- |
| ConditionId           | `numbre`                          | false    |
| ConditionKey          | `guid`                            | false    |
| IsCurrentCondition    | `bit`                             | false    |
| VoyageId              | `number`                          | false    |
| InProgressEventsCount | `number`                          | false    |
| ConditionName         | `string`                          | false    |
| StartDate             | `string`                          | true     |
| EndDate               | `string`                          | true     |
| VesselName            | `string`                          | false    |
| Events                | [`object[]`](#event-object-model) | false    |

## Event Object Model

| Property             | Type                                         | Nullable |
| :------------------- | :------------------------------------------- | :------- |
| Id                   | `number`                                     | false    |
| Timestamp            | `string`                                     | true     |
| Terminal             | `string`                                     | false    |
| Cargo                | [`object`](#cargo-object-model)              | false    |
| Comment              | `string`                                     | false    |
| NumberOfAttachments  | `number`                                     | false    |
| Port                 | [`object`](#port-object-model)               | false    |
| StatusId             | `number`                                     | false    |
| EventTypeName        | `string`                                     | false    |
| EventTypeId          | `number`                                     | false    |
| ConditionId          | `number`                                     | false    |
| VoyageId             | `number`                                     | false    |
| Attachments          | [`object[]`](#event-attachment-object-model) | false    |
| CargoStatus          | [`number`](#cargo-status-enum)               | false    |
| BallastQuantity      | `decimal`                                    | true     |
| CargoDetails         | [`object[]`](#event-cargo-object-model)      | false    |
| ReportAvailable      | `bit`                                        | false    |
| ReportId             | `number`                                     | true     |
| IsAvailableForDelete | `bit`                                        | false    |

## Cargo Object Model

| Property | Type     | Nullable |
| :------- | :------- | :------- |
| Id       | `number` | false    |
| Name     | `string` | false    |

## Port Object Model

| Property | Type     | Nullable |
| :------- | :------- | :------- |
| Id       | `number` | false    |
| Name     | `string` | false    |

## Event Attachment Object Model

| Property | Type     | Nullable |
| :------- | :------- | :------- |
| Id       | `number` | false    |
| Url      | `string` | false    |

## Cargo Status Enum

| Property | Value |
| :------- | :---- |
| None     | 0     |
| Loaded   | 1     |
| Ballast  | 2     |

## Event Cargo Object Model

| Property       | Type      | Nullable |
| :------------- | :-------- | :------- |
| ParcelSize     | `decimal` | false    |
| ParcelQuantity | `decimal` | false    |

**[POST] `/Event/Facts/{'txt'|'pdf'}`**

**Query params:**

| Property | Type     | Optional | Default |
| :------- | :------- | :------- | :------ |
| Page     | `number` | true     | 1       |
| PageSize | `number` | true     | 5       |

**Request Body:** `object`

| Property       | Type                             | Nullable |
| :------------- | :------------------------------- | :------- |
| EventTypeIds   | `number[]`                       | false    |
| EventStatuses  | [`number[]`](#event-status-enum) | false    |
| DateFrom       | `string`                         | false    |
| DateTo         | `string`                         | true     |
| EventIdsToSkip | `number[]`                       | false    |

**Response Body:** `text|binary`

**[DELETE] `/Events/Fact/{eventId}`**

**[PATCH] `/Events/Fact/{eventId}`**

**Request Body:** `form`

| FieldName       | Type                                      | Nullable |
| :-------------- | :---------------------------------------- | :------- |
| Timestamp       | `string`                                  | true     |
| CargoId         | `number`                                  | true     |
| PortId          | `number`                                  | true     |
| Comment         | `string`                                  | false    |
| Files           | `file[]`                                  | false    |
| RemoveFileIds   | `number[]`                                | false    |
| CargoDetails    | [`object[]`](#cargo-details-object-model) | false    |
| CargoStatus     | [`number`](#cargo-status-enum)            | false    |
| BallastQuantity | `decimal`                                 | true     |

## Cargo Details Object Model

| Property       | Type      | Nullable |
| :------------- | :-------- | :------- |
| ParcelSize     | `decimal` | false    |
| ParcelQuantity | `decimal` | false    |

**[GET] `/Events/CurrentCondition`**

**Response Body:** `object`

| Property  | Type     |
| :-------- | :------- |
| Condition | `string` |

**[GET] `/Events/Current/{eventTypeId}`**

**Request Body:** `form`

| FieldName       | Type                                      | Nullable |
| :-------------- | :---------------------------------------- | :------- |
| Timestamp       | `string`                                  | true     |
| CargoId         | `number`                                  | true     |
| Comment         | `string`                                  | false    |
| PortId          | `number`                                  | true     |
| Terminal        | `string`                                  | false    |
| BallastQuantity | `decimal`                                 | true     |
| CargoDetails    | [`object[]`](#cargo-details-object-model) | false    |
| CargoStatus     | [`number`](#cargo-status-enum)            | false    |

**[GET] `/Events/Ports`**

**Response Body:** [`object[]`](#port-object-model)

**[GET] `/Events/Cargoes`**

**Response Body:** [`object[]`](#cargo-object-model)

**[GET] `/Events/Types`**

**Response Body:** [`object[]`](#event-type-object-model)

**[POST] `/Event/{typeId}/Voyage/{voyageId}/condition/{conditionId}/{conditionKey}`**

**[GET] `/EventType/All`**

**Response Body:** [`object[]`](#event-type-object-model)

**[GET] `/EventType/CurrentCondition`**

**Response Body:** [`object[]`](#event-type-object-model)

**[GET] `/EventType/Condition/{conditionId}/{conditionKey}/Voyage/{voyageId}`**

**Response Body:** [`object`](#event-type-object-model)

**[GET] `/Reports/History?Page={number}&PageSize={number}`**

**Query params:**

| Property | Type     | Optional | Default |
| :------- | :------- | :------- | :------ |
| Page     | `number` | true     | 1       |
| PageSize | `number` | true     | 5       |

**Response Body:** `object`

| Property   | Type                                        |
| :--------- | :------------------------------------------ |
| Items      | [`object[]`](#report-business-object-model) |
| TotalCount | `number`                                    |

## Report Business Object Model

| Property     | Type                                           | Nullable |
| :----------- | :--------------------------------------------- | :------- |
| Id           | `number`                                       | false    |
| Date         | `string`                                       | false    |
| Event        | [`object`](#event-object-model)                | false    |
| ReportFields | [`object[]`](#report-field-value-object-model) | false    |

**[GET] `/Reports/{eventId}/fields`**

**Response Body:** [`object[]`](#report-field-object-model)

**[GET] `/Reports/{reportId}`**

**Response Body:** [`object`](#report-object-model)

**[PUT] `/Reports/${reportId}`**

| Property    | Type                                           | Nullable |
| :---------- | :--------------------------------------------- | :------- |
| FieldValues | [`object[]`](#report-field-value-object-model) | false    |

**[POST] `/Reports/${eventId}`**

| Property    | Type                                           | Nullable |
| :---------- | :--------------------------------------------- | :------- |
| FieldValues | [`object[]`](#report-field-value-object-model) | false    |

**Request Body:** `object`

| Property       | Type                             | Nullable |
| :------------- | :------------------------------- | :------- |
| EventTypeIds   | `number[]`                       | false    |
| EventStatuses  | [`number[]`](#event-status-enum) | false    |
| DateFrom       | `string`                         | false    |
| DateTo         | `string`                         | true     |
| EventIdsToSkip | `number[]`                       | false    |

**[GET] `/Admin/Facts?Page={number}&PageSize={number}&UserId={string}`**

**Query params:**

| Property | Type     | Optional | Default |
| :------- | :------- | :------- | :------ |
| Page     | `number` | true     | 1       |
| PageSize | `number` | true     | 5       |
| UserId   | `string` | true     | null    |

**Response Body:** `object`

| Property   | Type                                         |
| :--------- | :------------------------------------------- |
| Items      | [`object[]`](#condition-events-object-model) |
| TotalCount | `number`                                     |

**[GET] `/Admin/Vessels`**

**Response Body:** [`object[]`](#user-object-model)

**[PUT] `/Admin/Event/{eventId}/${aprove|reject}`**

**[DELETE] `/Admin/Fact/{eventId}`**

## Event Type Object Model

| Property                  | Type                                                | Nullable |
| :------------------------ | :-------------------------------------------------- | :------- |
| Id                        | `number`                                            | false    |
| Name                      | `string`                                            | false    |
| Events                    | [`object[]`](#event-data-object-model)              | false    |
| EventTypesConditions      | [`object[]`](#event-type-condition-object-model)    | false    |
| PairedEventTypeId         | `number`                                            | true     |
| PairedEventType           | [`object`](#event-type-object-model)                | false    |
| NextConditionId           | `number`                                            | true     |
| Transit                   | `bit`                                               | false    |
| AvailableAfterEventTypeId | `number`                                            | true     |
| AvailableAfterEventType   | [`object`](#event-type-object-model)                | false    |
| EventType                 | `number`                                            | false    |
| NextCondition             | [`object`](#event-condition-object-model)           | false    |
| AvailableReportFields     | [`object[]`](#event-type-report-field-object-model) | false    |

## Event Data Object Model

| Property                  | Type                                      | Nullable |
| :------------------------ | :---------------------------------------- | :------- |
| Id                        | `number`                                  | false    |
| Timestamp                 | `string`                                  | true     |
| UserId                    | `string`                                  | false    |
| User                      | [`object`](#user-object-model)            | false    |
| Terminal                  | `string`                                  | false    |
| StatusId                  | `number`                                  | false    |
| Status                    | [`object`](#status-object-model)          | false    |
| EventTypeId               | `number`                                  | false    |
| EventType                 | [`object`](#event-type-object-model)      | false    |
| ConditionId               | `number`                                  | false    |
| EventCondition            | [`object`](#event-condition-object-model) | false    |
| VoyageId                  | `number`                                  | false    |
| Voyage                    | [`object`](#voayage-object-model)         | false    |
| CurrentVoyageConditionKey | `guid`                                    | false    |
| Comment                   | `string`                                  | false    |
| EventCargoes              | [`object[]`](#event-cargo-object-model)   | false    |
| EventPorts                | [`object[]`](#port-object-model)          | false    |
| Attachments               | [`object[]`](#attachment-object-model)    | false    |
| CargoDetails              | [`object[]`](#cargo-details-object-model) | false    |
| CargoStatus               | `number`                                  | false    |
| ConditionStartedDate      | `string`                                  | true     |
| BallastQuantity           | `decimal`                                 | true     |
| Reports                   | [`object[]`](#report-object-model)        | false    |
| ParentEventId             | `number`                                  | true     |
| ParentEvent               | [`object`](#event-data-object-model)      | false    |
| ChildrenEvents            | [`object[]`](#event-data-object-model)    | false    |

## Report Object Model

| Property    | Type                                           | Nullable |
| :---------- | :--------------------------------------------- | :------- |
| Id          | `number`                                       | false    |
| CreatedDate | `string`                                       | false    |
| EventId     | `number`                                       | false    |
| Event       | [`object`](#event-data-object-model)           | false    |
| FieldValues | [`object[]`](#report-field-value-object-model) | false    |

## Report Field Value Object Model

| Property      | Type                                   | Nullable |
| :------------ | :------------------------------------- | :------- |
| Id            | `number`                               | false    |
| Value         | `string`                               | false    |
| ReportId      | `number`                               | false    |
| ReportFieldId | `number`                               | false    |
| Report        | [`object`](#report-object-model)       | false    |
| ReportField   | [`object`](#report-field-object-model) | false    |

## Report Field Object Model

| Property              | Type                                                | Nullable |
| :-------------------- | :-------------------------------------------------- | :------- |
| Id                    | `number`                                            | false    |
| Name                  | `string`                                            | false    |
| Group                 | [`number`](#field-group-enum)                       | true     |
| IsSubgroupMain        | `bit`                                               | true     |
| Subgroup              | `number`                                            | true     |
| FieldValues           | [`object[]`](#report-field-value-object-model)      | false    |
| EventTypeReportFields | [`object[]`](#event-type-report-field-object-model) | false    |
| UserTankFields        | [`object[]`](#user-tank-object-model)               | false    |

## User Tank Object Model

| Property    | Type                                 | Nullable |
| :---------- | :----------------------------------- | :------- |
| Id          | `number`                             | false    |
| UserId      | `string`                             | false    |
| TankFieldId | `number`                             | false    |
| User        | [`object`](#user-object-model)       | false    |
| TankField   | [`object`](#tank-field-object-model) | false    |

## Tank Field Object Model

| Property              | Type                                                | Nullable |
| :-------------------- | :-------------------------------------------------- | :------- |
| Id                    | `number`                                            | false    |
| Name                  | `string`                                            | false    |
| Group                 | [`number`](#field-group-enum)                       | true     |
| IsSubgroupMain        | `bit`                                               | true     |
| Subgroup              | `number`                                            | true     |
| FieldValues           | [`object[]`](#report-field-value-object-model)      | false    |
| EventTypeReportFields | [`object[]`](#event-type-report-field-object-model) | false    |
| UserTankFields        | [`object[]`](#user-tank-field-object-model)         | false    |

## User Tank Field Object Model

| Property    | Type                                   | Nullable |
| :---------- | :------------------------------------- | :------- |
| Id          | `number`                               | false    |
| UserId      | `string`                               | false    |
| TankFieldId | `number`                               | false    |
| User        | [`object`](#user-object-model)         | false    |
| TankField   | [`object`](#report-field-object-model) | false    |

## Event Type Report Field Object Model

| Property      | Type                                   | Nullable |
| :------------ | :------------------------------------- | :------- |
| Id            | `number`                               | false    |
| EventTypeId   | `number`                               | false    |
| ReportFieldId | `number`                               | false    |
| EventType     | [`object`](#event-type-object-model)   | false    |
| ReportField   | [`object`](#report-field-object-model) | false    |

## Field Group Enum

| Property     | Value |
| :----------- | :---- |
| ROBHFOACTUAL | 0     |
| ROBHFOPOOL   | 1     |
| ROBMGOACTUAL | 2     |
| ROBMGOPOOL   | 3     |

## Attachment Object Model

| Property | Type                                 | Nullable |
| :------- | :----------------------------------- | :------- |
| Id       | `number`                             | false    |
| Path     | `string`                             | false    |
| EventId  | `number`                             | false    |
| Event    | [`object`](#event-data-object-model) | false    |

## Status Object Model

| Property | Type                                   | Nullable |
| :------- | :------------------------------------- | :------- |
| Id       | `number`                               | false    |
| Name     | `string`                               | false    |
| Events   | [`object[]`](#event-data-object-model) | false    |

## User Object Model

| Property   | Type                                   | Nullable |
| :--------- | :------------------------------------- | :------- |
| Events     | [`object[]`](#event-data-object-model) | false    |
| Voyages    | [`object[]`](#voayage-object-model)    | false    |
| TankFields | [`object[]`](#event-data-object-model) | false    |

## Voyage Object Model

| Property                  | Type                                      | Nullable |
| :------------------------ | :---------------------------------------- | :------- |
| Id                        | `number`                                  | false    |
| Events                    | [`object[]`](#event-data-object-model)    | false    |
| StartDate                 | `string`                                  | false    |
| EndDate                   | `string`                                  | true     |
| UserId                    | `string`                                  | false    |
| User                      | [`object`](#user-object-model)            | false    |
| CurrentCondition          | [`object`](#event-condition-object-model) | false    |
| IsFinished                | `bit`                                     | false    |
| CurrentConditionId        | `number`                                  | false    |
| CurrentVoyageConditionKey | `guid`                                    | false    |

## Event Condition Object Model

| Property             | Type                                             | Nullable |
| :------------------- | :----------------------------------------------- | :------- |
| Id                   | `number`                                         | false    |
| Name                 | `string`                                         | false    |
| Events               | [`object[]`](#event-data-object-model)           | false    |
| EventTypesConditions | [`object[]`](#event-type-condition-object-model) | false    |
| EventTypes           | [`object[]`](#event-type-object-model)           | false    |
| Voyages              | [`object[]`](#voyage-object-model)               | false    |

## Event Type Condition Object Model

| Property       | Type                                      | Nullable |
| :------------- | :---------------------------------------- | :------- |
| Id             | `number`                                  | false    |
| ConditionId    | `number`                                  | false    |
| EventTypeId    | `number`                                  | false    |
| EventCondition | [`object`](#event-condition-object-model) | false    |
| EventType      | [`object`](#event-type-object-model)      | false    |
