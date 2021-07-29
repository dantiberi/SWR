using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace SWR_server
{
    public class DB
    {
        public static string dbname = "ProductDatabase.sqlite";
        public DB()
        {
            if (!doesFileExist(dbname))
            {
                createDBFile(dbname);
            }
            else
            {
                connect(dbname);
            }
        }
        //static void Main(string[] args)
        //{
        //    SQLiteConnection sqlite_conn;
        //    sqlite_conn = CreateConnection();
        //    CreateTable(sqlite_conn);
        //    InsertData(sqlite_conn);
        //    ReadData(sqlite_conn);
        //}

        public Boolean doesFileExist(string s)
        {
            return File.Exists(s);
        }

        public void createDBFile(string fileName)
        {
            print("DB FILE " + fileName + " CREATED");
            SQLiteConnection.CreateFile(fileName);
        }

        private SQLiteConnection connect(string fileName) //Was createConnection
        {

            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source = " + fileName + "; Version=3;");
            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex) { print(ex.ToString()); }
            return sqlite_conn;
        }

        static void createTables(SQLiteConnection conn)
        {
            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE ...";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }

        static void insertData(SQLiteConnection conn)
        {
            SQLiteCommand sqlite_cmd;
            string Createsql = "INSERT INTO ...";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }

        static void readData(SQLiteConnection conn)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM SampleTable";

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                string myreader = sqlite_datareader.GetString(0);
                Console.WriteLine(myreader);
            }
            conn.Close();
        }

        public static void print(string s)
        {
            System.Diagnostics.Debug.WriteLine("OUTPUT: " + s);
        }
    }
}
