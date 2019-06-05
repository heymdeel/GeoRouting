using GeoRouting.AppLayer.Model.Entities;
using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;
using LinqToDB.DataProvider.PostgreSQL;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoRouting.AppLayer.Model
{
    public class ConnectionStringSettings : IConnectionStringSettings
    {
        public string ConnectionString { get; set; }
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public bool IsGlobal => false;
    }

    public class MySettings : ILinqToDBSettings
    {
        public IEnumerable<IDataProviderSettings> DataProviders
        {
            get { yield break; }
        }

        public string DefaultConfiguration => "Npgsql";
        public string DefaultDataProvider => "Npgsql";

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                yield return
                    new ConnectionStringSettings
                    {
                        Name = "PostgresConfig",
                        ProviderName = "Npgsql",
                        ConnectionString = "User ID=postgres; Password=12345; Server=localhost; Port=5432; Database=find_me; Pooling=true;"
                    };
            }
        }
    }

    public partial class DbContext : DataConnection
    {
        public ITable<User> Users { get => GetTable<User>(); }
        public ITable<WaysAttributes> WaysAttributes { get => GetTable<WaysAttributes>(); }
        public ITable<Way> Ways { get => GetTable<Way>(); }
        public ITable<WayBackUp> WaysBackup { get => GetTable<WayBackUp>(); }
        public ITable<Category> Categories { get => GetTable<Category>(); }
        public ITable<Entities.Attribute> Attributes { get => GetTable<Entities.Attribute>(); }
        public ITable<PointAttribute> PointAttributes { get => GetTable<PointAttribute>(); }
        public ITable<WaysVertice> Vertices { get => GetTable<WaysVertice>(); }

        static DbContext()
        {
            DefaultSettings = new MySettings();
            NpgsqlConnection.GlobalTypeMapper.UseNetTopologySuite();

            // change this to the normal logging
            TurnTraceSwitchOn();
            WriteTraceLine = (s1, s2) =>
            {
                Console.WriteLine(s1, s2);
            };
        }
    }
}
