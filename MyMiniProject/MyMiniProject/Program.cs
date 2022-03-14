using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MyMiniProject
{
    internal class Program
    {
        class User
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }
        class Containers
        {
            public string ContainerType { get; set; }
            public int ContainerCapacity { get; set; }
        }

        class Status_Tracking
        {
            public string FromPort { get; set; }
            public string ToPort { get; set; }
            public string Phase { get; set; }
            public DateTime date { get; set; }
        }

        class Ports
        {
            public string PortId { get; set; }
            public string PortName { get; set; }
        }

        public class ConnectMySqlDb
        {
            IDictionary<int, User> userData = new Dictionary<int, User>();

            private MySqlConnection connection;
            private string server;
            private string database;
            private string username;
            private string password;

            public ConnectMySqlDb()
            {
                initialize();
            }

            private void initialize()
            {
                this.server = "localhost";
                this.database = "emodal";
                this.username = "root";
                this.password = "root";
                string connectionString;
                connectionString = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={password};";
                this.connection = new MySqlConnection(connectionString);
            }

            //Opening Database Connection
            private bool OpenConnection()
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (MySqlException ex)
                {
                    switch (ex.Number)
                    {
                        case 0:
                            Console.WriteLine("Cannot connect to server. Contact Admin");
                            break;
                        case 1045:
                            Console.WriteLine("Invalid Credentials");
                            break;
                    }
                    return false;
                }
            }

            //Closing Database Connection
            private bool CloseConnection()
            {
                try
                {
                    this.connection.Close();
                    return true;
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }

            //Insert User Data in User Table
            public void InsertUser(string fname, string lname, string username, string password)
            {
                string query = $"INSERT INTO user (fname, lname, username, password) VALUES ('{fname}', '{lname}', '{username}', '{password}')";

                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, this.connection);
                    cmd.ExecuteNonQuery();
                    this.CloseConnection();
                }
            }

            //Login
            public string getUserWithUsernamePass(string username, string password)
            {
                string query = $"SELECT id FROM user WHERE username='{username}' AND password='{password}';";
                string userid = "";

                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, this.connection);
                    MySqlDataReader datareader = cmd.ExecuteReader();

                    foreach (System.Data.Common.DbDataRecord data in datareader)
                    {
                        userid = Convert.ToString(data.GetValue(0));
                        Console.WriteLine("You have successfully logged in!");
                    }
                    datareader.Close();
                    this.CloseConnection();
                }
                return userid;
            }

            //Add Container
            public void insertContainer(string userid, string ContainerType, int Capacity)
            {
                int user=Convert.ToInt32(userid);
                string query = $"insert into container (userID, containerType, capacity) values ({user}, '{ContainerType}', {Capacity});";

                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, this.connection);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Container Added Successfully!");
                    this.CloseConnection();
                }
            }

            //Show Containers
            public void showContainers(string loginid)
            {
                int user = Convert.ToInt32(loginid);
                string query = $"SELECT id, containerType, capacity FROM container WHERE userID='{user}';";
                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, this.connection);
                    MySqlDataReader datareader = cmd.ExecuteReader();

                    Console.WriteLine("ID\tType\tCapacity");
                    foreach (System.Data.Common.DbDataRecord data in datareader)
                    {
                        int containerid = Convert.ToInt32(data.GetValue(0));
                        string containertype = Convert.ToString(data.GetValue(1));
                        int capacity = Convert.ToInt32(data.GetValue(2));
                        Console.WriteLine("{0}\t{1}\t{2}",containerid,containertype,capacity);
                    }
                    datareader.Close();
                    this.CloseConnection();
                    Console.WriteLine("Listed Containers showed aboove.");
                    Console.ReadKey();
                }
            }

            public bool ShowContainers(string id)
            {
                int user = Convert.ToInt32(id);
                string query = $"SELECT id, containerType, capacity FROM container WHERE userID='{user}';";
                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, this.connection);
                    MySqlDataReader datareader = cmd.ExecuteReader();

                    Console.WriteLine("ID\tType\tCapacity");
                    foreach (System.Data.Common.DbDataRecord data in datareader)
                    {
                        int containerid = Convert.ToInt32(data.GetValue(0));
                        string containertype = Convert.ToString(data.GetValue(1));
                        int capacity = Convert.ToInt32(data.GetValue(2));
                        Console.WriteLine("{0}\t{1}\t{2}", containerid, containertype, capacity);
                    }
                    datareader.Close();
                    this.CloseConnection();
                }
                return true;
            }
            //Insert Container Status in Status_Tracking Table
            public void Insertstatus(string containerID, string source, string destination, string phase,string date)
            {
                int conid = Convert.ToInt32(containerID);
                string query = $"INSERT INTO status (containerID,source,destination,phase,date) VALUES ({conid},'{source}','{destination}','{phase}','{date}')";
                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, this.connection);
                    cmd.ExecuteNonQuery();
                    Console.Clear();
                    Console.WriteLine("Container Status Inserted!");
                    this.CloseConnection();
                }
            }

            public void Showstatus(string loginid, int conid)
            {
                int user = Convert.ToInt32(loginid);
                string query = $"SELECT containerID, containerType, capacity, source, destination, phase, date FROM container c,status s WHERE s.containerID=c.id AND (s.containerID='{conid}' AND c.userID='{user}') ;";
                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, this.connection);
                    MySqlDataReader datareader = cmd.ExecuteReader();

                    Console.WriteLine("ID\tType\tCapacity\tSource\tDestination\tPhase\t\tDate");
                    foreach (System.Data.Common.DbDataRecord data in datareader)
                    {
                        int containerid = Convert.ToInt32(data.GetValue(0));
                        string containertype = Convert.ToString(data.GetValue(1));
                        int capacity = Convert.ToInt32(data.GetValue(2));
                        string source = Convert.ToString(data.GetValue(3));
                        string destination = Convert.ToString(data.GetValue(4));
                        string phase = Convert.ToString(data.GetValue(5));
                        string date = Convert.ToString(data.GetValue(6));
                        Console.WriteLine("{0}\t{1}\t{2}\t\t{3}\t{4}\t\t{5}\t{6}", containerid, containertype, capacity, source, destination, phase, date);
                    }
                    datareader.Close();
                    this.CloseConnection();
                    Console.ReadKey();
                }
            }
        }

        //Main Funcation
        static void Main(string[] args)
        {
            ConnectMySqlDb sqlObject = new ConnectMySqlDb();
            int containerid;
            Console.WriteLine("Hello Admins! Welcome to the C# Project of eModal.");
            while (true)
            {
                while (true)
                {
                    Console.WriteLine("1. LogIn\n2. SignUp\n3. Exit");
                    Console.WriteLine("Choose option ");
                    int res = Convert.ToInt32(Console.ReadLine());
                    if (res == 1)
                    {
                        Console.Clear();
                        string loginid = Login();
                        Console.WriteLine($"Hi {loginid}! Have a greate Day");
                        while (loginid != "")
                        {
                            Console.Write("*****************************\n1.Add Container\n2.Show Containers\n3.Update Container Status\n4.Show Container Status\n5.Logout\n\nProvide Index Value:");
                            int loginres = Convert.ToInt32(Console.ReadLine());
                            switch (loginres)
                            {
                                case 1:
                                    AddContainers(loginid);
                                    break;
                                case 2:
                                    Console.Clear();
                                    sqlObject.showContainers(loginid);
                                    break;
                                case 3:
                                    UpdateContainerStatus(loginid);
                                    break;
                                case 4:
                                    Console.WriteLine("Provide Container ID:");
                                    containerid = Convert.ToInt32(Console.ReadLine());
                                    sqlObject.Showstatus(loginid, containerid);
                                    break;
                                case 5:
                                    loginid = "";
                                    Console.Clear();
                                    break;
                                default:
                                    Console.WriteLine("Invalid input");
                                    break;
                                }
                        }
                    }
                    else if(res == 2)
                    {
                        Signup();
                    }
                    else if(res == 3)
                    {
                        Environment.Exit(0);
                    }
                    else {
                        Console.Clear();
                        Console.WriteLine("Provide Proper Choice");
                    }
                }
            }
        }
        //Admin Registration
        static void Signup()
        {
            Console.Clear();
            ConnectMySqlDb sqlObject = new ConnectMySqlDb();
            User ad = new User();
            int passwordtype = 0;
            Console.WriteLine("Provide First Name: ");
            ad.FirstName = Console.ReadLine();
            Console.WriteLine("Provide Last Name: ");
            ad.LastName = Console.ReadLine();
            Console.WriteLine("Provide Username: ");
            ad.Username = Console.ReadLine();
            Console.WriteLine("Want Your Own Password or System Generated:\n1.Own Password \n2.System Generated\nProvide Your Way: ");
            passwordtype = Convert.ToInt32(Console.ReadLine());
            if (passwordtype == 1)
            {
                Console.WriteLine("Provide Your Password: ");
                ad.Password = Console.ReadLine();
            }
            if (passwordtype == 2)
            {
                ad.Password = GetPassword();
                Console.WriteLine("Your System Generated Password is: {0}", ad.Password);
            }
            sqlObject.InsertUser(ad.FirstName, ad.LastName, ad.Username, ad.Password);
            Console.WriteLine($"{ad.Username} is Created.");
            Console.ReadLine();
            Console.Clear();
        }
        //Random Password Generation
        private static string GetPassword(int length = 5)
        {
            string charset = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();

            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
                chars[i] = charset[random.Next(0, charset.Length)];
            return new string(chars);
        }

        //Admin Login
        static string Login()
        {
            ConnectMySqlDb sqlObject = new ConnectMySqlDb();
            Console.Write("Username : ");
            string Username = Console.ReadLine();
            Console.Write("Password : ");
            string Password = Console.ReadLine();
            Console.Clear();
            string userid= sqlObject.getUserWithUsernamePass(Username,Password);
            return userid;
        }

        //Add Containers
        static void AddContainers(string loginid)
        {
            ConnectMySqlDb sqlObject = new ConnectMySqlDb();
            Containers NewContainer = new Containers();
            Console.Write("Container Types :\n1. Simple\n2. Liquis\n3. Gaseous\n4. Wooden\n");
            Console.Write("Select Type : ");
            int Type = Convert.ToInt32(Console.ReadLine());
            switch (Type)
            {
                case 1:
                    NewContainer.ContainerType = "Simple";
                    break;
                case 2:
                    NewContainer.ContainerType = "Liquid";
                    break;
                case 3:
                    NewContainer.ContainerType = "Gaseous";
                    break;
                case 4:
                    NewContainer.ContainerType = "Wooden";
                    break;
                default:
                    Console.WriteLine("Invalid Input");
                    break;
            }
            Console.Write("Enter Capacity : ");
            NewContainer.ContainerCapacity = Convert.ToInt32(Console.ReadLine());
            Console.Clear();
            sqlObject.insertContainer(loginid,NewContainer.ContainerType, NewContainer.ContainerCapacity);
        }

        //Container Status Tracking
        static void UpdateContainerStatus(string loginid)
        {
            Console.Clear();
            ConnectMySqlDb sqlObject = new ConnectMySqlDb();
            var portlist = new List<Ports>()
            {
                new Ports() { PortId="P101",PortName="US"},
                new Ports() { PortId="P102",PortName="Russia"},
                new Ports() { PortId="P103",PortName="Chaina"},
                new Ports() { PortId="P104",PortName="UK"},
            };
            Status_Tracking st = new Status_Tracking();
            Console.Clear();
            string containerid = null;
            int containerphaseid = 0;
            bool hasvalue = sqlObject.ShowContainers(loginid);
            Console.WriteLine("Provide Container ID: ");
            containerid=Console.ReadLine();
            if (containerid != null)
            {
                if (hasvalue)
                {
                    Console.WriteLine("\n1.Available\n2.Booked\n3.Onway\n4.Delivered\nProvide Container Phase Index Value:");
                    containerphaseid = Convert.ToInt32(Console.ReadLine());
                    switch (containerphaseid)
                    {
                        case 1:
                            st.Phase = "Available";
                            st.FromPort = "";
                            st.ToPort = "";
                            break;
                        case 2:
                            st.Phase = "Booked";
                            Console.WriteLine("--------------Ports--------------");
                            foreach (Ports p in portlist)
                                Console.WriteLine(p.PortName);
                            Console.WriteLine("Provide Container Source Location:");
                            st.FromPort = Console.ReadLine();
                            Console.WriteLine("Provide Container Destination Location:");
                            st.ToPort =Console.ReadLine();
                            break;
                        case 3:
                            st.Phase = "Onway";
                            st.FromPort = "";
                            st.ToPort = "";
                            break;
                        case 4:
                            st.Phase = "Delivered";
                            st.FromPort = "";
                            st.ToPort = "";
                            break;
                        default:
                            st.Phase = "";
                            st.FromPort = "";
                            st.ToPort = "";
                            Console.WriteLine("");
                            break;
                    }
                    st.date = DateTime.Now;
                    string currentdate = st.date.ToString();
                    sqlObject.Insertstatus(containerid, st.FromPort, st.ToPort, st.Phase, currentdate);
                }
                else
                {
                    Console.WriteLine("Container is Not Available in Database");
                }
            }
        }
    }
}
