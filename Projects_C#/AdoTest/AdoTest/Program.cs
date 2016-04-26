using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace AdoTest
{
    class Program
    {
        //public sealed class ListCompare : IEqualityComparer<List<int>> {

        //    public bool Equals(List<int> li, List<int> l2)
        //    {
        //        if (li.GetType() != l2.GetType())
        //            return false;
        //        if (li.Count != l2.Count)
        //            return false;
        //        for (int i = 0; i < li.Count; i++)
        //        {
        //            if (li[i] != l2[i])
        //                return false;
        //        }
        //        return true;
        //    }
        //    public int GetHashCode(List<int> list)
        //    {
        //        String sum = "";
        //        foreach (int i in list)
        //        {
        //            sum = sum + i;
        //        }
        //        return sum.GetHashCode();
        //    }
        //}
        public static List<String> user = new List<string>();
        public static List<String> video = new List<string>();

        static void Main(string[] args)
        {
            int thresholdDays = 7;
            DateTime initialDate = new DateTime(2015, 12, 1);
            DateTime thresholdDate = new DateTime(2016, 3, 1);
            for (int i = 0; i < 100; i += thresholdDays)
            {
                DateTime startDate = initialDate.AddDays(i);
                DateTime endDate = startDate.AddDays(thresholdDays - 1);

                if(startDate>= thresholdDate)
                {
                    break;
                }

                string sqlQueryStr = string.Format("select * from XXX where startDateKey between {0} and {1}",
                    startDate.Year * 10000 + startDate.Month * 100 + startDate.Day,
                    endDate.Year * 10000 + endDate.Month * 100 + endDate.Day);
            }

            try
            {
                using (SqlConnection conn = new SqlConnection("Server=cia-sh-04; Database=OnlineLearning;" +
                               "Integrated Security=True")) //Defines SQL connection using the connection string.
                {
                    conn.Open(); //Opens the connection, will throw exception if it fails.

                    SqlCommand command = new SqlCommand(); //Creates SQL command object. We will 
                                                           //use SQL command to execute SQL query.
                    command.Connection = conn; //Assigns the SQL connection to the command object.
                    command.CommandType = System.Data.CommandType.Text; //SQL command can 
                                                                        //be different types, we use regular query text type here.
                    command.CommandText = "select top 5 * from VideoDailyView_Raw where startDateKey between 20160101 and 20160110"; //A sample of SQL query which is to query out a Ch9 video according to the asset ID (Tracking ID).

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read()) //if reader.Read() returns true, that means 
                                              //we still have data record returned.
                        {
                            //reader.GetString(0) means getting the column 0 data of 
                            //the returned data record as string type.
                            //Please note that it requires the type matches what returned
                            //from database. If column x is not string, but int, you will get exception when invoking reader.GetString(x). Instead you shall use reader.GetInt32(x).         
                            Console.WriteLine("userId:" + reader.GetString(1));
                            Console.WriteLine("videoId:" + reader.GetString(2));
                            //   Console.WriteLine(string.Format("VideoId is:\t{0}", reader.GetString(1)));
                            String userId = reader.GetString(1);
                            String videoId = reader.GetString(2);
                            user.Add(userId);
                            video.Add(videoId);
                        }
                        reader.Close();
                    }
                    conn.Close();
                }
                    using (SqlConnection tempconn = new SqlConnection("Server=cia-sh-04; Database=OnlineLearningResult;" +
                               "Integrated Security=True")) //Defines SQL connection using the connection string.
                    {
                        tempconn.Open(); //Opens the connection, will throw exception if it fails.
                        for (int i = 0; i < user.Count; i++)
                        {
                            Console.WriteLine(user[i]);
                            Console.WriteLine(video[i]);

                            SqlCommand tempcom = new SqlCommand();
                            tempcom.Connection = tempconn;
                            tempcom.CommandType = System.Data.CommandType.Text;
                            tempcom.CommandText = "insert into AssociateResult values('"+ user[i]+"','"+ video[i]+"', '', '', '', '', '')";
                            SqlDataReader reader = tempcom.ExecuteReader();
                            reader.Close();

                        }
                        tempconn.Close();
                    }
      

                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine("Press Enter to exit the program...");
            Console.ReadLine();

            //Dictionary<List<int>, int> dt = new Dictionary<List<int>, int>(new ListCompare());
            //List<int> li = new List<int>();
            //List<int> li2 = new List<int>();
            //li.Add(1);
            //li.Add(2);
            //String s = "";
            //int i = 100;
            //int j = 299;
            //s = s + i + j;
            //Console.WriteLine(s);
            //li2.Add(1);
            //li2.Add(2);
            //dt.Add(li,2);
            //Console.WriteLine(dt[li]);
            //try
            //{
            //    Console.WriteLine(dt[li2]);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}
            //if (dt.ContainsKey(li))
            //{
            //    Console.WriteLine("have li");
            //}
            //Console.ReadLine();

        }
    }
}

