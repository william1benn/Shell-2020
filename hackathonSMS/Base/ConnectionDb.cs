using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hackathon.Base
{
    public class Connection 
    {
        public DataTable dTable {get; set;}

        public DataTableReader getData(string query)
        {
         try {
         string server = Environment.GetEnvironmentVariable("SERVER");
         string db = Environment.GetEnvironmentVariable("DB");
         string user = Environment.GetEnvironmentVariable("USER");
         string password = Environment.GetEnvironmentVariable("PASSWORD");

         string conn = $"server={server};database={db};uid={user};pwd={password};Pooling=True;";
         using (var mySqlClientConn = new MySqlConnection(conn))
         {
         mySqlClientConn.Open();

         using (var command = new MySqlCommand(query,mySqlClientConn))
         {
         var dbResult = command.ExecuteReader();
         var dataTable = new DataTable();

          dataTable.Load( dbResult );

          dTable = dataTable;

         return dataTable.CreateDataReader();
        }
        }
        }  catch ( Exception e ) 
        {
            Console.WriteLine("Error" + e);
            return null;
        }
    }
  }
} 
