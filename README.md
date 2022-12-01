# NordigenSQLite
[![.NET Core Desktop](https://github.com/BorisGerretzen/NordigenSQLite/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/BorisGerretzen/NordigenSQLite/actions/workflows/dotnet-desktop.yml)

Application to load transactions from the Nordigen API into a SQLite database.

## How to use
- Click on the `.NET Core Desktop` status badge and open the latest run with a green checkmark. Scroll all the way down and download the `NordigenService` artifact.
- Extract the contents of the zip file somewhere
- Fill in the fields in `example.appsettings.json`. 
  - `RetrievalMode` can be set to three things
    - `All`, does not ask for a date range from the Nordigen API, always retrieves the latest number of transactions
    - `Range`, asks for a specified date range from the Nordigen API, specify this with `DateFrom` and `DateTo`.
    - `Dynamic`, first requests all transactions, then every subsequent run only retrieves the timespan since the last run.
  - `TimeoutMinutes` is how long the application waits after retrieving the transactions.
- Rename `example.appsettings.json` to `appsettings.json`. 
- To run the service, run the executable file as administrator.
- To install the service as a Windows service, run the executable from an elevated terminal window with the flag `--install`.
  - To uninstall, run with `--uninstall`.
- The SQLite3 database will be created in the folder `NordigenService` that is created in your Program Files.
  - Access to the database requires administrator rights, I am working on this.

## Todo
- 'Installer' is not very intuitive to use
- Database access requires administrator rights, so Excel needs to be launched as administrator to read from the db.
- Some fields are missing because I do not know what kind of structure they have. 
