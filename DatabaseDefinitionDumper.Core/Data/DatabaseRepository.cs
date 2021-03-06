﻿using DatabaseDefinitionDumper.Core.Domain;
using DatabaseDefinitionDumper.Core.Repository;
using System;
using System.Collections.Generic;

namespace DatabaseDefinitionDumper.Core.Data
{
    public class DatabaseRepository : IDatabaseRepository
    {
        private IDataSource dataSource;
        public DatabaseRepository(IDataSource dataSource)
        {
            this.dataSource = dataSource;
        }

        public IObservable<bool> TestConnection()
        {
            return dataSource.TestConnection();
        }

        public List<Database> LoadDatabases()
        {
            return dataSource.LoadDatabases();
        }

        public List<Table> LoadTables(Database Database)
        {
            return dataSource.LoadTables(Database);
        }

        public List<TableTrigger> LoadTriggers(Database Database, Table Table)
        {
            return dataSource.LoadTriggers(Database, Table);
        }

        public List<TableColumn> LoadColumns(Database Database, Table Table)
        {
            return dataSource.LoadColumns(Database, Table);
        }

        public List<TableIndex> LoadIndexes(Database Database, Table Table)
        {
            return dataSource.LoadIndexes(Database, Table);
        }

        public List<View> LoadViews(Database Database)
        {
            return dataSource.LoadViews(Database);
        }

        public List<ViewTrigger> LoadTriggers(Database Database, View View)
        {
            return dataSource.LoadTriggers(Database, View);
        }

        public List<ViewColumn> LoadColumns(Database Database, View View)
        {
            return dataSource.LoadColumns(Database, View);
        }

        public List<ViewIndex> LoadIndexes(Database Database, View View)
        {
            return dataSource.LoadIndexes(Database, View);
        }

        public List<dynamic> LoadProcedures(Database Database)
        {
            return dataSource.LoadProcedures(Database);
        }
    }
}
