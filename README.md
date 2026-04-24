# TelemachusWeb: Vessel reporting application

## Apply migrations

`dotnet ef migrations add [NAME] --project Telemachus.Data.Services -s Telemachus -c TelemachusContext`
`dotnet ef database update --project Telemachus.Data.Services -s Telemachus --context TelemachusContext`

### If you encounter migration issues
The project requires the .NET 3.1 runtime which is EOL and not installed. You might need to write the migration manually instead.

First the migration. Then update the snapshot to add the changes. Then create the designer file (base it on the previous designer file with the class name/migration ID changed and the same index additions).

Now to apply the migration, since `dotnet ef database update` won't work (same runtime problem),  run the SQL directly against the database.

Once applied, EF Core will record it in `__EFMigrationsHistory` — but since we're running it manually, we'll also need to insert the migration history row, for example:
```
INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
VALUES ('20260424000000_EventUserIdIndexes', '3.1.32');
```
Summary:
- 20260424000000_migrationName.cs — migration with Create/Drop calls
- 20260424000000_migrationName.Designer.cs — model snapshot at migration time
- TelemachusContextModelSnapshot.cs — updated with changes

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

## Passcode for reports ("in-house" detection)
`PasscodeAuthenticationMiddleware` and `AuthenticationService` vary on what constitutes in-house mode:
- AuthenticationService: `isInHouse = string.IsNullOrEmpty(vesselDetails?.Prefix)` — checks if Prefix is empty
- PasscodeAuthenticationMiddleware: `hasVesselSettings = _configuration.GetSection("VesselDetails").Exists()` — only checks if the section exists
                                                                                         
If `VesselDetails` is present in `appsettings.json` but `Prefix` is empty/missing, the auth service considers it in-house (so no passcode flow at login), but the middleware considers it a vessel install (so it requires `X-Passcode` and returns 401).

Now the middleware uses the same Prefix-based check as `AuthenticationService`. Previously, if `appsettings.json` had a `VesselDetails` section at all (even with an empty `Prefix`), the middleware would demand a passcode that was never created during login.
                                                                                                                           
Quick summary of the whole flow:
- Login hits AuthenticationService -> `isInHouse = string.IsNullOrEmpty(prefix)` -> if true, no passcode is issued
- Every request to `/api/reports` hits PasscodeAuthenticationMiddleware -> previously checked `.Exists()` instead of checking `Prefix`, so a `VesselDetails: {}` section in appsettings would silently flip it into vessel mode and block all reports requests with 401.

`VesselDetails__Prefix=GEA` means vessel mode (not in-house)   
`ASPNETCORE_ENVIRONMENT=Production` means no `isDevelopment` bypass, so the middleware actively enforces the passcode for `/api/reports`.

To get reports access in vessel mode, log out, then log back in with the "Secure access" toggle enabled and a Full Name filled in. That creates a `UserPasscodes` row in the DB and stores the UUID in `Telemachus-Passcode` (browser local storage). Subsequent `/api/reports` requests will then include the header and pass the middleware.

If you want to skip the passcode entirely while developing, change `ASPNETCORE_ENVIRONMENT=Production` to `ASPNETCORE_ENVIRONMENT=Development`. The middleware has an explicit `isDevelopment` bypass at the top.
