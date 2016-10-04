using Dapper;
using DatabaseDefinitionDumper.Core.Domain;
using DatabaseDefinitionDumper.Core.Domain.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace DatabaseDefinitionDumper.Core.Data
{
    public class SQLServerDatabaseDataSource : IDataSource
    {
        private string connectionString;
        public SQLServerDatabaseDataSource(ConnectionSettings ConnectionSettings)
        {
            this.connectionString = ConnectionSettings.ToConnectionString();
        }

        public IObservable<bool> TestConnection()
        {
            return Observable.Create<bool>(observer => {
                using (var con = new SqlConnection(connectionString))
                {
                    try
                    {
                        con.Open();
                        observer.OnNext(con.State == ConnectionState.Open);
                        observer.OnCompleted();
                    }
                    catch (SqlException e)
                    {
                        observer.OnError(e);
                    }
                    return Disposable.Empty;
                }
            });
        }

        public List<Database> LoadDatabases()
        {
            using (var con = new SqlConnection(connectionString))
            {
                return con.Query<Database>("select database_id, name from sys.databases;").ToList();
            }
        }

        public List<Table> LoadTables(Database Database)
        {
            using (var con = new SqlConnection(connectionString))
            {
                return con.Query<dynamic>($@"use [{Database.Name}];
SELECT
    tables.object_id AS TableId,
    tables.name AS Name,
    exd.value AS Comment,
    tables.create_date AS CreatedAt,
    tables.modify_date AS UpdatedAt
FROM sys.tables
LEFT OUTER JOIN sys.extended_properties AS exd
    ON exd.major_id = tables.object_id AND exd.minor_id = 0 AND exd.class = 1
ORDER BY tables.Name;")
                .Select(x => { return new Table(Database, x.TableId, x.Name, x.Comment?.ToString(), x.CreatedAt, x.UpdatedAt); })
                .ToList();
            }
        }

        public List<TableTrigger> LoadTriggers(Database Database, Table Table)
        {

            using (var con = new SqlConnection(connectionString))
            {
                return con.Query<dynamic>($@"use [{Database.Name}];
SELECT
    triggers.object_id AS TriggerId,
    triggers.name AS Name,
    trigger_events.Type AS Type,
    trigger_events.type_desc AS TypeName
FROM sys.tables
INNER JOIN sys.triggers ON triggers.parent_id = tables.object_id
INNER JOIN sys.trigger_events ON trigger_events.object_id = triggers.object_id
WHERE tables.object_id = @TableId
ORDER BY tables.Name, trigger_events.Type, triggers.object_id;", new { TableId = Table.TableId })
                .Select(x => { return new TableTrigger(Database, Table, x.TriggerId, x.Name, x.Type, x.TypeName); })
                .ToList();
            }
        }

        public List<TableColumn> LoadColumns(Database Database, Table Table)
        {
            using (var con = new SqlConnection(connectionString))
            {
                return con.Query<dynamic>($@"use [{Database.Name}];
SELECT
    columns.column_id AS ColumnId,
    columns.name AS Name,
    types.name AS TypeName,
    columns.max_length AS Length,
    columns.precision AS Precision,
    columns.scale AS Scale,
    columns.is_nullable AS IsNullable,
    default_constraints.definition AS DefaultValue,
    primary_keys.name AS PrimaryKeyName,
    primary_keys.key_ordinal AS PrimaryKeyOrdinal,
    exd.value AS Comment
FROM sys.tables
LEFT OUTER JOIN sys.columns ON columns.object_id = tables.object_id
LEFT OUTER JOIN sys.types ON types.user_type_id = columns.user_type_id
LEFT OUTER JOIN sys.default_constraints ON default_constraints.parent_object_id = columns.object_id AND default_constraints.parent_column_id = columns.column_id
LEFT OUTER JOIN (
    SELECT
        key_constraints.Name,
        key_constraints.object_id,
        key_constraints.schema_id,
        key_constraints.parent_object_id,
        indexes.index_id,
        index_columns.column_id,
        index_columns.key_ordinal
    FROM sys.key_constraints
    INNER JOIN sys.indexes
        ON indexes.object_id = key_constraints.parent_object_id
        AND indexes.index_id = key_constraints.unique_index_id
        AND key_constraints.type = 'PK'
    INNER JOIN sys.index_columns
        ON index_columns.object_id = indexes.object_id
        AND index_columns.index_id = indexes.index_id
) AS primary_keys ON primary_keys.parent_object_id = columns.object_id AND primary_keys.column_id = columns.column_id
LEFT OUTER JOIN sys.extended_properties AS exd
    ON exd.major_id = columns.object_id AND exd.minor_id = columns.column_id AND exd.class = 1
ORDER BY tables.Name, columns.column_id")
                .Select(x => {
                    return new TableColumn(
                        Database,
                        Table,
                        x.ColumnId,
                        x.Name,
                        x.TypeName,
                        x.Length,
                        x.Precision,
                        x.Scale,
                        x.IsNullable,
                        x.DefaultValue,
                        x.PrimaryKeyName,
                        x.PrimaryKeyOrdinal,
                        x.Comment?.ToString()
                    );
                })
                .ToList();
            }
        }

        public List<TableIndex> LoadIndexes(Database Database, Table Table)
        {
            using (var con = new SqlConnection(connectionString))
            {
                return con.Query<dynamic>($@"use [{Database.Name}];
SELECT
    indexes.index_id AS IndexId,
    indexes.name AS Name,
    CASE
        WHEN indexes.is_primary_key = 1 THEN 1
        ELSE (
            CASE
                WHEN indexes.is_unique_constraint = 1 THEN 2
                ELSE (CASE WHEN indexes.type = 6 THEN 4 ELSE 3 END)
            END
        )
    END AS IndexType,
    Cast(CASE WHEN indexes.type_desc = 'CLUSTERED' THEN 1 ELSE 0 END AS bit) AS IsClustered,
    indexes.key_ordinal AS ColumnOrdinal,
    columns.column_id AS ColumnId,
    columns.name AS ColumnName,
    exd.value AS Comment
FROM sys.tables
LEFT OUTER JOIN sys.columns ON columns.object_id = tables.object_id
INNER JOIN (
    SELECT
        indexes.Name,
        indexes.object_id,
        indexes.index_id,
        indexes.type,
        indexes.type_desc,
        indexes.is_primary_key,
        indexes.is_unique_constraint,
        index_columns.column_id,
        index_columns.key_ordinal
    FROM sys.indexes AS indexes
    INNER JOIN sys.index_columns
        ON index_columns.object_id = indexes.object_id
        AND index_columns.index_id = indexes.index_id
) AS indexes
    ON indexes.object_id = columns.object_id AND indexes.column_id = columns.column_id
LEFT OUTER JOIN sys.extended_properties AS exd
    ON exd.major_id = indexes.object_id AND exd.minor_id = indexes.index_id AND exd.class = 7
WHERE tables.object_id = @TableId
ORDER BY tables.Name, IndexType, indexes.index_id, indexes.key_ordinal, columns.column_id;", new { TableId = Table.TableId })
                .Select(x => {
                    return new TableIndex(
                        Database,
                        Table,
                        x.IndexId,
                        x.Name,
                        x.IndexType,
                        x.IsClustered,
                        x.ColumnOrdinal,
                        x.ColumnId,
                        x.ColumnName,
                        x.Comment?.ToString()
                    );
                })
                .ToList();
            }
        }

        public List<View> LoadViews(Database Database)
        {
            using (var con = new SqlConnection(connectionString))
            {
                return con.Query<dynamic>($@"use [{Database.Name}];
SELECT
    views.object_id AS ViewId,
    views.name AS Name,
    sql_modules.definition AS Definition,
    exd.value AS Comment,
    views.create_date AS CreatedAt,
    views.modify_date AS UpdatedAt
FROM sys.views
INNER JOIN sys.objects ON views.object_id = objects.object_id
INNER JOIN sys.schemas ON objects.schema_id = schemas.schema_id
INNER JOIN sys.sql_modules ON objects.object_id = sql_modules.object_id
LEFT OUTER JOIN sys.extended_properties AS exd ON exd.major_id = views.object_id AND exd.minor_id = 0 AND exd.class = 1
ORDER BY Name;")
                .Select(x => { return new View(Database, x.ViewId, x.Name, x.Definition, x.Comment?.ToString(), x.CreatedAt, x.UpdatedAt); })
                .ToList();
            }
        }

        public List<ViewTrigger> LoadTriggers(Database Database, View View)
        {

            using (var con = new SqlConnection(connectionString))
            {
                return con.Query<dynamic>($@"use [{Database.Name}];
SELECT
    triggers.object_id AS TriggerId,
    triggers.name AS Name,
    trigger_events.Type AS Type,
    trigger_events.type_desc AS TypeName
FROM sys.views
INNER JOIN sys.triggers ON triggers.parent_id = views.object_id
INNER JOIN sys.trigger_events ON trigger_events.object_id = triggers.object_id
WHERE views.object_id = @ViewId
ORDER BY views.Name, trigger_events.Type, triggers.object_id;", new { ViewId = View.ViewId })
                .Select(x => { return new ViewTrigger(Database, View, x.TriggerId, x.Name, x.Type, x.TypeName); })
                .ToList();
            }
        }

        public List<ViewColumn> LoadColumns(Database Database, View View)
        {
            using (var con = new SqlConnection(connectionString))
            {
                return con.Query<dynamic>($@"use [{Database.Name}];
SELECT
    columns.column_id AS ColumnId,
    columns.name AS Name,
    types.name AS TypeName,
    columns.max_length AS Length,
    columns.precision AS Precision,
    columns.scale AS Scale,
    columns.is_nullable AS IsNullable,
    default_constraints.definition AS DefaultValue,
    primary_keys.name AS PrimaryKeyName,
    primary_keys.key_ordinal AS PrimaryKeyOrdinal,
    exd.value AS Comment
FROM sys.views
LEFT OUTER JOIN sys.columns ON columns.object_id = views.object_id
LEFT OUTER JOIN sys.types ON types.user_type_id = columns.user_type_id
LEFT OUTER JOIN sys.default_constraints ON default_constraints.parent_object_id = columns.object_id AND default_constraints.parent_column_id = columns.column_id
LEFT OUTER JOIN (
    SELECT
        key_constraints.Name,
        key_constraints.object_id,
        key_constraints.schema_id,
        key_constraints.parent_object_id,
        indexes.index_id,
        index_columns.column_id,
        index_columns.key_ordinal
    FROM sys.key_constraints
    INNER JOIN sys.indexes
        ON indexes.object_id = key_constraints.parent_object_id
        AND indexes.index_id = key_constraints.unique_index_id
        AND key_constraints.type = 'PK'
    INNER JOIN sys.index_columns
        ON index_columns.object_id = indexes.object_id
        AND index_columns.index_id = indexes.index_id
) AS primary_keys ON primary_keys.parent_object_id = columns.object_id AND primary_keys.column_id = columns.column_id
LEFT OUTER JOIN sys.extended_properties AS exd
    ON exd.major_id = columns.object_id AND exd.minor_id = columns.column_id AND exd.class = 1
ORDER BY views.Name, columns.column_id")
                .Select(x => {
                    return new ViewColumn(
                        Database,
                        View,
                        x.ColumnId,
                        x.Name,
                        x.TypeName,
                        x.Length,
                        x.Precision,
                        x.Scale,
                        x.IsNullable,
                        x.DefaultValue,
                        x.PrimaryKeyName,
                        x.PrimaryKeyOrdinal,
                        x.Comment?.ToString()
                    );
                })
                .ToList();
            }
        }

        public List<ViewIndex> LoadIndexes(Database Database, View View)
        {
            using (var con = new SqlConnection(connectionString))
            {
                return con.Query<dynamic>($@"use [{Database.Name}];
SELECT
    indexes.index_id AS IndexId,
    indexes.name AS Name,
    CASE
        WHEN indexes.is_primary_key = 1 THEN 1
        ELSE (
            CASE
                WHEN indexes.is_unique_constraint = 1 THEN 2
                ELSE (CASE WHEN indexes.type = 6 THEN 4 ELSE 3 END)
            END
        )
    END AS IndexType,
    Cast(CASE WHEN indexes.type_desc = 'CLUSTERED' THEN 1 ELSE 0 END AS bit) AS IsClustered,
    indexes.key_ordinal AS ColumnOrdinal,
    columns.column_id AS ColumnId,
    columns.name AS ColumnName,
    exd.value AS Comment
FROM sys.views
LEFT OUTER JOIN sys.columns ON columns.object_id = views.object_id
INNER JOIN (
    SELECT
        indexes.Name,
        indexes.object_id,
        indexes.index_id,
        indexes.type,
        indexes.type_desc,
        indexes.is_primary_key,
        indexes.is_unique_constraint,
        index_columns.column_id,
        index_columns.key_ordinal
    FROM sys.indexes AS indexes
    INNER JOIN sys.index_columns
        ON index_columns.object_id = indexes.object_id
        AND index_columns.index_id = indexes.index_id
) AS indexes
    ON indexes.object_id = columns.object_id AND indexes.column_id = columns.column_id
LEFT OUTER JOIN sys.extended_properties AS exd
    ON exd.major_id = indexes.object_id AND exd.minor_id = indexes.index_id AND exd.class = 7
WHERE views.object_id = @ViewId
ORDER BY views.Name, IndexType, indexes.index_id, indexes.key_ordinal, columns.column_id;", new { ViewId = View.ViewId })
                .Select(x => {
                    return new ViewIndex(
                        Database,
                        View,
                        x.IndexId,
                        x.Name,
                        x.IndexType,
                        x.IsClustered,
                        x.ColumnOrdinal,
                        x.ColumnId,
                        x.ColumnName,
                        x.Comment?.ToString()
                    );
                })
                .ToList();
            }
        }

    }
}
