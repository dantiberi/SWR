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
        public Boolean debugMode = true;//If true, will clear and create new db each run. 
        public static SQLiteConnection conn;
        public DB()
        {       
            if (!doesFileExist(dbname) || debugMode)
            {
                createDBFile(dbname);
                conn = connect(dbname);
            }
            else
            {
                conn = connect(dbname);
            }
            createTables(conn);

            //printProductTable(conn);
        }
        //static void Main(string[] args)
        //{
        //    SQLiteConnection sqlite_conn;
        //    sqlite_conn = CreateConnection();
        //    CreateTable(sqlite_conn);
        //    InsertData(sqlite_conn);
        //    ReadData(sqlite_conn);
        //}

        public static void openDbConnection()
        {
            conn.Open();
        }

        private Boolean doesFileExist(string s)
        {
            return File.Exists(s);
        }

        private void createDBFile(string fileName)
        {
            //print("DB FILE " + fileName + " CREATED");
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

        private void createTables(SQLiteConnection conn)
        {
            SQLiteCommand sqlite_cmd;

            //product Table
            string Createsql = @"CREATE TABLE product(
            p_id INTEGER PRIMARY KEY,
            name VARCHAR(30),
            url VARCHAR(1000) NOT NULL,
            price DOUBLE,
            img_url VARCHAR(1000)
            );
            ";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();

            //price_history Table
            Createsql = @"CREATE TABLE price_history(
            id INTEGER PRIMARY KEY,
            p_id INTEGER,
            price DOUBLE,
            date DATE
            );
            ";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();

            //Triggers

            //When new product added, add initial entry to price_history table.
            Createsql = @"CREATE TRIGGER link_price_history 
                AFTER INSERT ON product
            BEGIN
                INSERT INTO price_history (p_id, price, date) 
                VALUES
                (NEW.p_id, NEW.price, DATE('now'));
            END;
            ";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }

        public void addProduct(SQLiteConnection conn, string url, string name, double price, string imgUrl)
        {
            SQLiteCommand sqlite_cmd;
            string Createsql = "INSERT INTO product VALUES(null, '" + name +"', '" + url +"', " + price + ", '" + imgUrl + "')";
            //print(Createsql);
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }

        public void insertTestProduct(SQLiteConnection conn)
        {
            SQLiteCommand sqlite_cmd;
            string Createsql = "INSERT INTO product VALUES(null, 'Nintendo 64', 'google.com', 97.35, null)";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }
        public void printProductTable(SQLiteConnection conn)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM product";

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                string row = "| ";
                for (int i = 0; i < sqlite_datareader.GetValues().Count; i++)
                {
                    row += sqlite_datareader.GetValue(i).ToString() + " | ";
                }
                print(row);
            }
            conn.Close();
        }

        public void insertData(SQLiteConnection conn)
        {
            SQLiteCommand sqlite_cmd;
            string Createsql = "INSERT INTO ...";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }

        public void readData(SQLiteConnection conn)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM SampleTable";

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                string myreader = sqlite_datareader.GetString(0);
                print(myreader);
            }
            conn.Close();
        }

        private static void print(string s)
        {
            System.Diagnostics.Debug.WriteLine("DB OUTPUT: " + s);
        }
    }
}
