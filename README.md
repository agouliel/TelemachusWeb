# TelemachusWeb: Vessel reporting application

## Apply migrations

`dotnet ef migrations add [NAME] --project Telemachus.Data.Services -s Telemachus -c TelemachusContext`
`dotnet ef database update --project Telemachus.Data.Services -s Telemachus --context TelemachusContext`

## Add or Remove Vessel Tanks

### Remove (for Next)

Update the `tank_user_specs` table by setting the `IsActive` field to `0` for the tank to be removed.

### Create / Edit (for Next)

Select a tank from the `tanks` table and create a new row in the `tank_user_specs` table.
The tank name can be edited (rename able).

## Other notes
See `documentation` folder (`API.md` and `db_notes.md`)

## Sync procedure
1. Login returns `hasRemoteData: true` + `isInHouse: false` for the user
2. `useAuth.jsx:92-103` detects this and calls `POST /api/sync`
3. `SyncController.cs:56-57` calls `SyncMasterValues` / `SyncDataValues`, which makes an outbound HTTP request to the remote server address stored in the user's DB record (`RemoteAddress:RemotePort`)
