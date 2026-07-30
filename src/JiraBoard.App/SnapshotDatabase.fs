namespace JiraBoard.App

open System
open System.IO
open Microsoft.Data.Sqlite

[<RequireQualifiedAccess>]
module SnapshotDatabase =
    let private migrations =
        [ 1,
          """
INSERT INTO SnapshotSchemaVersion (Version) VALUES (1);
""" ]

    let private currentVersion (connection: SqliteConnection) =
        use command = connection.CreateCommand()
        command.CommandText <- "SELECT COALESCE(MAX(Version), 0) FROM SnapshotSchemaVersion;"
        Convert.ToInt32(command.ExecuteScalar())

    let initialize (databasePath: string) =
        let directory = Path.GetDirectoryName databasePath

        if String.IsNullOrWhiteSpace databasePath then
            Error "Snapshot database path is required."
        elif String.IsNullOrWhiteSpace directory || not (Directory.Exists directory) then
            Error $"Snapshot database directory does not exist: {directory}"
        else
            try
                let connectionString =
                    SqliteConnectionStringBuilder(DataSource = databasePath, Pooling = false).ToString()

                use connection = new SqliteConnection(connectionString)
                connection.Open()

                use tableCommand = connection.CreateCommand()
                tableCommand.CommandText <-
                    "CREATE TABLE IF NOT EXISTS SnapshotSchemaVersion (Version INTEGER NOT NULL CHECK (Version > 0));"
                tableCommand.ExecuteNonQuery() |> ignore

                let version = currentVersion connection

                for targetVersion, script in migrations do
                    if targetVersion > version then
                        use migrationCommand = connection.CreateCommand()
                        migrationCommand.CommandText <- script
                        migrationCommand.ExecuteNonQuery() |> ignore

                Ok(currentVersion connection)
            with :? SqliteException as error ->
                Error $"Snapshot database initialization failed: {error.Message}"