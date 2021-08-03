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
        public static void openDbConnection()
        {
            conn.Open();
        }

        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        /// <param name="s">Name of file to check.</param>
        /// <returns>Returns true if the given filename exists.</returns>
        private Boolean doesFileExist(string s)
        {
            return File.Exists(s);
        }

        /// <summary>
        /// Creates a new SQLite database file.
        /// </summary>
        /// <param name="fileName">Name of file.</param>
        private void createDBFile(string fileName)
        {
            //print("DB FILE " + fileName + " CREATED");
            SQLiteConnection.CreateFile(fileName);
        }

        /// <summary>
        /// Establish initial connection to SQLite database.
        /// </summary>
        /// <param name="fileName">Name of SQLite database file to connect to.</param>
        /// <returns>A SQLiteConnection object.</returns>
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

        /// <summary>
        /// Create tables for a new SQLite database file.
        /// </summary>
        /// <param name="conn">Connection to a SQLite database.</param>
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
        public void addProduct(SQLiteConnection conn, string url, string name, double price, string imgUrl, int isOnSale)
        {
            if (conn.State == 0)//If closed then open
                openDbConnection();

            SQLiteCommand sqlite_cmd;
            string Createsql = "INSERT INTO product VALUES(null, '" + name +"', '" + url +"', " + price + ", '" + imgUrl + "', " + isOnSale + ")";
            //print(Createsql);
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            //closeDB(conn);
        }

        /// <summary>
        /// Inserts a test product into the database.
        /// </summary>
        /// <param name="conn">Connection to a SQLite database.</param>
        public void insertTestProduct(SQLiteConnection conn)
        {
            if (conn.State == 0)//If closed then open
                openDbConnection();
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

            try
            {
                for (int i = 0; i < sqlite_datareader.FieldCount; i++)
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
                        case "is_on_sale":
                            p.isOnSale = int.Parse(sqlite_datareader.GetValue(i).ToString());
                            break;
                        default:
                            print("DB.getJsonOfProduct: UNHANDLED FIELD");
                            break;
                    }
                }
            }catch(System.InvalidOperationException e)
            {
                print("Product with id: " + id + " does not exist.");
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
        public int getLastInsertedProductId(SQLiteConnection conn)
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
        public string getAllProductsInJson(SQLiteConnection conn)
        {
            if (conn.State == 0)//If closed then open
                openDbConnection();

            string ret = "{";

            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT p_id FROM product";
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            sqlite_datareader.Read();
            while (sqlite_datareader.Read())
            {
                int i = int.Parse(sqlite_datareader.GetValue(0).ToString());
                //print(i + "");
                ret += "\"product_" + i + "\":" + getJsonOfProduct(conn, i) + ",";
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
        public void removeProduct(SQLiteConnection conn, int id)
        {
            if (conn.State == 0)//If closed then open
                openDbConnection();

            try
            {
                SQLiteCommand sqlite_cmd;
                string Createsql = "DELETE FROM product WHERE p_id=" + id;
                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = Createsql;
                sqlite_cmd.ExecuteNonQuery();
            }catch(System.InvalidOperationException e)
            {
                print("Product with id: " + id + " does not exist.");
            }
}
        /// <summary>
        /// Closes the connection to the SQLite database.
        /// </summary>
        /// <param name="conn">Connection to a SQLite database.</param>
        public static void closeDB(SQLiteConnection conn)
        {
            conn.Close();
        }

        /// <summary>
        /// Used to easily print out debug info to VS debug console.
        /// </summary>
        /// <param name="s">String to be printed.</param>
        private static void print(string s)
        {
            System.Diagnostics.Debug.WriteLine("DB OUTPUT: " + s);
        }
    }
}
