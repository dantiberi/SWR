﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using Newtonsoft.Json;

namespace SWR_server
{
    public class DB
    {
        public static string dbname = "ProductDatabase.sqlite";
        public Boolean debugMode = false;//If true, will clear and create new db each run. 
        public static SQLiteConnection conn;
        public DB()
        {       
            if (!doesFileExist(dbname) || debugMode)
            {
                createDBFile(dbname);
                conn = connect(dbname);
                createTables(conn);
            }
            else
            {
                conn = connect(dbname);
            }

            printProductTable(conn);
            print(getJsonOfProduct(conn, 1));
            print(getLastInsertedProductId().ToString());
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
            if (conn.State == 0)//If closed then open
                openDbConnection();
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
            closeDB(conn);
        }

        public void addProduct(SQLiteConnection conn, string url, string name, double price, string imgUrl)
        {
            if (conn.State == 0)//If closed then open
                openDbConnection();
            SQLiteCommand sqlite_cmd;
            string Createsql = "INSERT INTO product VALUES(null, '" + name +"', '" + url +"', " + price + ", '" + imgUrl + "')";
            //print(Createsql);
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            closeDB(conn);
        }

        public void insertTestProduct(SQLiteConnection conn)
        {
            if (conn.State == 0)//If closed then open
                openDbConnection();
            SQLiteCommand sqlite_cmd;
            string Createsql = "INSERT INTO product VALUES(null, 'Nintendo 64', 'google.com', 97.35, null)";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            closeDB(conn);
        }
        public void printProductTable(SQLiteConnection conn)
        {
            if (conn.State == 0)//If closed then open
                openDbConnection();
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
            closeDB(conn);
        }

        public void insertData(SQLiteConnection conn)
        {
            if (conn.State == 0)//If closed then open
                openDbConnection();
            SQLiteCommand sqlite_cmd;
            string Createsql = "INSERT INTO ...";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            closeDB(conn);
        }

        public void readData(SQLiteConnection conn)
        {
            if (conn.State == 0)//If closed then open
                openDbConnection();
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
            closeDB(conn);
        }

        public string getJsonOfProduct(SQLiteConnection conn, int id)
        {
            if (conn.State == 0)//If closed then open
                openDbConnection();

            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM product WHERE p_id=" + id;
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            sqlite_datareader.Read();

            ProductModel p = new ProductModel();

            for(int i = 0; i < sqlite_datareader.FieldCount; i++)
            {
                //print(sqlite_datareader.GetName(i));
                switch (sqlite_datareader.GetName(i))
                {                   
                    case "name":
                        p.name = sqlite_datareader.GetValue(i).ToString();
                        break;
                    case "url":
                        p.url = sqlite_datareader.GetValue(i).ToString();
                        break;
                    case "price":
                        p.price = Double.Parse(sqlite_datareader.GetValue(i).ToString());
                        break;
                    case "img_url":
                        p.imgUrl = sqlite_datareader.GetValue(i).ToString();
                        break;
                    case "p_id":
                        p.id = int.Parse(sqlite_datareader.GetValue(i).ToString());
                        break;
                    default:
                        print("DB.getJsonOfProduct: UNHANDLED FIELD");
                        break;
                }
            }
            closeDB(conn);
            return JsonConvert.SerializeObject(p);  
        }

        

        public int getLastInsertedProductId()
        {
            if (conn.State == 0)//If closed then open
                openDbConnection();
            SQLiteCommand sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT last_insert_rowid() as p_id FROM product";
            int result = int.Parse(sqlite_cmd.ExecuteScalar().ToString());

            if(result == 0)//Could happen if this gets ran when the data was inserted before the conn was closed last.
            {
                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = "SELECT COUNT(*) FROM product";
                int count = int.Parse(sqlite_cmd.ExecuteScalar().ToString());

                if (count != 0)//If not empty, then return last id;
                {
                    sqlite_cmd = conn.CreateCommand();
                    sqlite_cmd.CommandText = "SELECT max(p_id) FROM product";
                    int res = int.Parse(sqlite_cmd.ExecuteScalar().ToString());
                    closeDB(conn);
                    return res;
                }
                else
                {
                    closeDB(conn);
                    return result;
                }
                    
            }
            closeDB(conn);
            return result;
        }

        public static void closeDB(SQLiteConnection conn)
        {
            conn.Close();
        }

        private static void print(string s)
        {
            System.Diagnostics.Debug.WriteLine("DB OUTPUT: " + s);
        }
    }
}
