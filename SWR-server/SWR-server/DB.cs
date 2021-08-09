using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using Newtonsoft.Json;

//https://stackoverflow.com/questions/21453697/angularjs-access-parent-scope-from-child-controller

namespace SWR_server
{
    public class DB
    {
        /// <summary>
        /// Name to be used for the database file.
        /// </summary>
        public static string dbname = "ProductDatabase.sqlite";

        /// <summary>
        /// If true, will delete and regenerate a new database file.
        /// If false, will load existing database file.
        /// </summary>
        public Boolean debugMode = false;//If true, will clear and create new db each run. 

        /// <summary>
        /// Static connection to the SQLite database.
        /// </summary>
        public static SQLiteConnection conn;

        /// <summary>
        /// Database object constructor.
        /// </summary>
        public DB()
        {       
            if (!DoesFileExist(dbname) || debugMode)
            {
                CreateDBFile(dbname);
                conn = Connect(dbname);
                CreateTables(conn);
            }
            else
            {
                conn = Connect(dbname);
            }

            //printProductTable(conn);
            //print(getJsonOfProduct(conn, 1));
            //print(getLastInsertedProductId(conn).ToString());
        }
        //static void Main(string[] args)
        //{
        //    SQLiteConnection sqlite_conn;
        //    sqlite_conn = CreateConnection();
        //    CreateTable(sqlite_conn);
        //    InsertData(sqlite_conn);
        //    ReadData(sqlite_conn);
        //}

        /// <summary>
        /// Open the static connection to the SQLite database.
        /// </summary>
        public static void OpenDbConnection()
        {
            conn.Open();
        }

        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        /// <param name="s">Name of file to check.</param>
        /// <returns>Returns true if the given filename exists.</returns>
        private Boolean DoesFileExist(string s)
        {
            return File.Exists(s);
        }

        /// <summary>
        /// Creates a new SQLite database file.
        /// </summary>
        /// <param name="fileName">Name of file.</param>
        private void CreateDBFile(string fileName)
        {
            //print("DB FILE " + fileName + " CREATED");
            SQLiteConnection.CreateFile(fileName);
        }

        /// <summary>
        /// Establish initial connection to SQLite database.
        /// </summary>
        /// <param name="fileName">Name of SQLite database file to connect to.</param>
        /// <returns>A SQLiteConnection object.</returns>
        private SQLiteConnection Connect(string fileName) //Was createConnection
        {

            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source = " + fileName + "; Version=3;");
            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex) { Print(ex.ToString()); }
            return sqlite_conn;
        }

        /// <summary>
        /// Create tables for a new SQLite database file.
        /// </summary>
        /// <param name="conn">Connection to a SQLite database.</param>
        private void CreateTables(SQLiteConnection conn)
        {
            if (conn.State == 0)//If closed then open
                OpenDbConnection();
            SQLiteCommand sqlite_cmd;

            //product Table
            string Createsql = @"CREATE TABLE product(
            p_id INTEGER PRIMARY KEY,
            name VARCHAR(30),
            url VARCHAR(1000) NOT NULL,
            price DOUBLE,
            img_url VARCHAR(1000),
            is_on_sale INT
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
            //closeDB(conn);
        }

        /// <summary>
        /// Adds a new product to the database with the given parameters.
        /// </summary>
        /// <param name="conn">SQLiteConnection to database file.</param>
        /// <param name="url">URL link to product.</param>
        /// <param name="name">Name of the product.</param>
        /// <param name="price">Price of the product.</param>
        /// <param name="imgUrl">URL link to the product image.</param>
        /// <param name="isOnSale">1 or 0 boolean to state if product is on sale.</param>
        /// <returns>Success status.</returns>
        public bool AddProduct(SQLiteConnection conn, string url, string name, double price, string imgUrl, int isOnSale)
        {
            if (conn.State == 0)//If closed then open
                OpenDbConnection();

            (bool, string) res = ExecuteNonQuery("INSERT INTO product VALUES(null, '" + name + "', '" + url + "', " + price + ", '" + imgUrl + "', " + isOnSale + ")", conn);
            return res.Item1;
        }

        /// <summary>
        /// Executed a query to the SQLite database.
        /// </summary>
        /// <param name="query">SQLite command to be executed</param>
        /// <returns>A SQLiteDataReader which contains the result of the query.</returns>
        public SQLiteDataReader ExecuteQuery(string query, SQLiteConnection conn)
        {
            try
            {
                if (conn.State == 0)//If closed then open
                    OpenDbConnection();
                SQLiteDataReader sqlite_datareader;
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = query;

                return sqlite_cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                Print("Exception Thrown On Query:\n" + e);
                return null;
            }
        }

        /// <summary>
        /// Executed a non-query to the SQLite database.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>(bool, string) where bool is: true = success, false = faild. String will represent the error. If there's no error, the string will be "OK"</returns>
        public (bool, string) ExecuteNonQuery(string command, SQLiteConnection conn)
        {
            try
            {
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = command;
                sqlite_cmd.ExecuteNonQuery();
            }catch(Exception e)
            {
                Print("Exception Thrown On Non-Query:\n" + e);
                return (false, e.ToString());
            }
            return (true, "OK");
        }

        /// <summary>
        /// Executed a scalar query to the SQLite database.
        /// </summary>
        /// <param name="query">SQLite command to be executed</param>
        /// <returns>A object which contains the result of the scalar query.</returns>
        public object ExecuteScalar(string query, SQLiteConnection conn)
        {
            try
            {
                if (conn.State == 0)//If closed then open
                    OpenDbConnection();
                SQLiteDataReader sqlite_datareader;
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = query;

                return sqlite_cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                Print("Exception Thrown On Scalar Query:\n" + e);
                return null;
            }
        }

        /// <summary>
        /// Inserts a test product into the database.
        /// </summary>
        /// <param name="conn">Connection to a SQLite database.</param>
        public void InsertTestProduct(SQLiteConnection conn)
        {
            if (conn.State == 0)//If closed then open
                OpenDbConnection();
            SQLiteCommand sqlite_cmd;
            string Createsql = "INSERT INTO product VALUES(null, 'Nintendo 64', 'google.com', 97.35, 'https://cdn1.bigcommerce.com/server4000/a642e/products/18161/images/29541/nintendo_64_charcoal_black_console_pre-owned_5___33487.1521828735.1280.1280.jpg?c=2', 1)";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            //closeDB(conn);
        }

        /// <summary>
        /// Prints out the product table to the VS debug console.
        /// </summary>
        /// <param name="conn">Connection to a SQLite database.</param>
        public void PrintProductTable(SQLiteConnection conn)
        {
            SQLiteDataReader sqlite_datareader = ExecuteQuery("SELECT * FROM product", conn);
            while (sqlite_datareader.Read())
            {
                string row = "| ";
                for (int i = 0; i < sqlite_datareader.GetValues().Count; i++)
                {
                    row += sqlite_datareader.GetValue(i).ToString() + " | ";
                }
                Print(row);
            }
            //closeDB(conn);
        }

        //public void insertData(SQLiteConnection conn)
        //{
        //    if (conn.State == 0)//If closed then open
        //        openDbConnection();
        //    SQLiteCommand sqlite_cmd;
        //    string Createsql = "INSERT INTO ...";
        //    sqlite_cmd = conn.CreateCommand();
        //    sqlite_cmd.CommandText = Createsql;
        //    sqlite_cmd.ExecuteNonQuery();
        //    //closeDB(conn);
        //}

        //public void readData(SQLiteConnection conn)
        //{
        //    if (conn.State == 0)//If closed then open
        //        openDbConnection();
        //    SQLiteDataReader sqlite_datareader;
        //    SQLiteCommand sqlite_cmd;
        //    sqlite_cmd = conn.CreateCommand();
        //    sqlite_cmd.CommandText = "SELECT * FROM SampleTable";

        //    sqlite_datareader = sqlite_cmd.ExecuteReader();
        //    while (sqlite_datareader.Read())
        //    {
        //        string myreader = sqlite_datareader.GetString(0);
        //        print(myreader);
        //    }
        //    //closeDB(conn);
        //}

        /// <summary>
        /// Returns a string Json object representing a object in the database.
        /// </summary>
        /// <param name="conn">Connection to a SQLite database.</param>
        /// <param name="id">p_id of product to be retrieved.</param>
        /// <returns>A string Json object representing a object in the database.</returns>
        public string GetJsonOfProduct(SQLiteConnection conn, int id)
        {
            SQLiteDataReader sqlite_datareader = ExecuteQuery("SELECT * FROM product WHERE p_id=" + id, conn);
            sqlite_datareader.Read();
            ProductModel p = new ProductModel();

            try
            {
                for (int i = 0; i < sqlite_datareader.FieldCount; i++)
                {
                    //Print(sqlite_datareader.GetName(i));
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
                        case "is_on_sale":
                            p.isOnSale = int.Parse(sqlite_datareader.GetValue(i).ToString());
                            break;
                        default:
                            Print("DB.getJsonOfProduct: UNHANDLED FIELD");
                            break;
                    }
                }
            }catch(System.InvalidOperationException e)
            {
                //Print("Product with id: " + id + " does not exist.");
                Print("Error while converting SQLite result to Product object.");
            }
            //closeDB(conn); //Leaved commented out or else will break getAllProductsInJson()
            return JsonConvert.SerializeObject(p);  
        }

        /// <summary>
        /// Gets the p_id of the last inserted product. If the connection has been closed since the last
        /// product was inserted, will return the last p_id.
        /// </summary>
        /// <param name="conn">Connection to a SQLite database.</param>
        /// <returns>p_id of the last inserted product.</returns>
        public int GetLastInsertedProductId(SQLiteConnection conn)
        {
            int result = int.Parse(ExecuteScalar("SELECT last_insert_rowid() as p_id FROM product", conn).ToString());

            if(result == 0)//Could happen if this gets ran when the data was inserted before the conn was closed last.
            {
                int count = int.Parse(ExecuteScalar("SELECT COUNT(*) FROM product", conn).ToString());

                if (count != 0)//If not empty, then return last id;
                {
                    int res = int.Parse(ExecuteScalar("SELECT max(p_id) FROM product", conn).ToString());
                    //closeDB(conn);
                    return res;
                }
                else
                {
                    //closeDB(conn);
                    return result;
                }
                    
            }
            //closeDB(conn);
            return result;
        }

        /// <summary>
        /// Returns a string that represents a JSON object array of all products stored in the database.
        /// </summary>
        /// <param name="conn">Connection to a SQLite database.</param>
        /// <returns>A string that represents a JSON object array of all products stored in the database.</returns>
        public string GetAllProductsInJson(SQLiteConnection conn)
        {
            if (conn.State == 0)//If closed then open
                OpenDbConnection();

            string ret = "{";

            SQLiteDataReader sqlite_datareader = ExecuteQuery("SELECT p_id FROM product", conn);
            while (sqlite_datareader.Read())
            {
                int i = int.Parse(sqlite_datareader.GetValue(0).ToString());
                //print(i + "");
                ret += "\"product_" + i + "\":" + GetJsonOfProduct(conn, i) + ",";
            }
            ret = ret.Remove(ret.Length - 1);
            ret += "}";          
            //closeDB(conn);
            return ret;
        }

        /// <summary>
        /// Removes product with given id from database.
        /// </summary>
        /// <param name="conn">Connection to a SQLite database.</param>
        /// <param name="id">p_id of product to be removed.</param>
        /// <returns>Status of command execution.</returns>
        public bool RemoveProduct(SQLiteConnection conn, int id)
        {
            if (conn.State == 0)//If closed then open
                OpenDbConnection();

            try
            {
                (bool, string) res = ExecuteNonQuery("DELETE FROM product WHERE p_id=" + id, conn);
                (bool, string) res2 = ExecuteNonQuery("DELETE FROM price_history WHERE p_id=" + id, conn);
                return res.Item1 && res2.Item1;
            }
            catch(System.InvalidOperationException e)
            {
                Print("Product with id: " + id + " does not exist.");
                return false;
            }
}
        /// <summary>
        /// Closes the connection to the SQLite database.
        /// </summary>
        /// <param name="conn">Connection to a SQLite database.</param>
        public static void CloseDB(SQLiteConnection conn)
        {
            conn.Close();
        }

        /// <summary>
        /// Used to easily print out debug info to VS debug console.
        /// </summary>
        /// <param name="s">String to be printed.</param>
        private static void Print(string s)
        {
            System.Diagnostics.Debug.WriteLine("DB OUTPUT: " + s);
        }
    }
}
