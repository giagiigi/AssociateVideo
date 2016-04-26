﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;

namespace Apriori
{
    class Program
    {
        static void Main(string[] args)
        {

            AprioriAlgorithm aa = new AprioriAlgorithm();
            AprioriAlgorithm.recordNumLimit = 100000000;
            AprioriAlgorithm.minSupThreshold = 0.0020;

            DateTime initialDate = new DateTime(2015, 7, 1);            //分析的起始时间
            DateTime terminalDate = new DateTime(2015, 9, 30);           //分析的终止时间

            String startprotime = DateTime.Now.ToString();
            Console.WriteLine("startprotime：" + startprotime);

            int initialTime = initialDate.Year * 10000 + initialDate.Month * 100 + initialDate.Day;
            int terminalTime = terminalDate.Year * 10000 + terminalDate.Month * 100 + terminalDate.Day;

            //输入原始数据
            try
            {
                //查询用户和视频数量
                SqlConnection conn = new SqlConnection("Server=cia-sh-04; Database=OnlineLearning;" +
                                     "Integrated Security=True");   //连接sql server
                conn.Open();

                Console.WriteLine("recordNumLimit：" + AprioriAlgorithm.recordNumLimit);
                Console.WriteLine("minSupThreshold：" + AprioriAlgorithm.minSupThreshold);

                DateTime startDate = new DateTime(1, 1, 1);
                DateTime endDate = new DateTime(1, 1, 1);
       
                int thresholdDays = 1;
                for (int i = 0; i < 1000; i += thresholdDays)
                {
                    if (endDate == terminalDate)
                        break;
                    if (endDate.AddDays(thresholdDays) > terminalDate)
                    {
                        startDate = initialDate.AddDays(i);
                        endDate = terminalDate;
                    }
                    else
                    {
                        startDate = initialDate.AddDays(i);
                        endDate = startDate.AddDays(thresholdDays - 1);
                    }

                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandType = System.Data.CommandType.Text;

                    command.CommandText = string.Format("select viewerId,asset,startDateKey,playing_time,percentage_complete " +
                                      "from VideoDailyView_Raw where startDateKey between {0} and {1}",
                                      startDate.Year * 10000 + startDate.Month * 100 + startDate.Day,
                                      endDate.Year * 10000 + endDate.Month * 100 + endDate.Day);
 
                    SqlDataReader reader = command.ExecuteReader();

                    int numLimit = 0;
                    while (reader.Read())
                    {
                        if (numLimit >= AprioriAlgorithm.recordNumLimit)
                            break;

                        if (Convert.ToInt32(reader.GetValue(4)) < 17.65)         //播放比例小于3/17
                            continue;
                        if (Convert.ToInt32(reader.GetValue(3)) < 60000)          //播放时长小于60s
                            continue;

                        String userId = reader.GetString(0);
                        String videoId = reader.GetString(1);

                        Boolean haveUser = aa.ifHaveUser(userId);
                        if (haveUser == false)
                        {
                            AprioriAlgorithm.userList.Add(userId);
                            List<int> tempList = new List<int>();
                            AprioriAlgorithm.recordMap.Add(aa.findUserIndex(userId), tempList);
                        }
                        Boolean haveVideo = aa.ifHaveVideo(videoId);
                        if (haveVideo == false)
                        {
                            AprioriAlgorithm.videoList.Add(videoId);
                        }

                        int userIndex = aa.findUserIndex(userId);
                        int videoIndex = aa.findVideoIndex(videoId);

                        AprioriAlgorithm.recordMap[userIndex].Add(videoIndex);

                        numLimit++;
                    }

                    reader.Close();

                }
                conn.Close();
                Console.WriteLine("userList的大小为：" + AprioriAlgorithm.userList.Count);
                Console.WriteLine("videoList的大小为：" + AprioriAlgorithm.videoList.Count);

                //StreamReader readerForList = new StreamReader(new FileStream("C:/Users/t-xubai/Desktop/DataAnalysis/"
                //      + "DailySessionLog_Microsoft-DPE_2015-03-18.csv", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Read));      //连接原始数据集

                //String Attributes = readerForList.ReadLine();                    //数据集的属性
                //String record = null;
                //int numLimit = 0;                             //控制分析记录的数量
                //while ((record = readerForList.ReadLine()) != null)
                //{
                //    if (numLimit >= AprioriAlgorithm.recordNumLimit)
                //        break;
                //    String[] ele = record.Split(',');
                //    //for (int i = 0; i < ele.Length; i++)
                //    //{
                //    //    ele[i] = ele[i].Substring(1, ele[i].Length - 2);   //去除""
                //    //}
                //    ele[0] = ele[0].Substring(1, ele[0].Length - 2);
                //    ele[1] = ele[1].Substring(1, ele[1].Length - 2);
                //    ele[10] = ele[10].Substring(1, ele[10].Length - 2);
                //    ele[22] = ele[22].Substring(1, ele[22].Length - 2);
                //    String pencentage = ele[22];
                //    if (int.Parse(pencentage) < 17.65)         //播放比例小于3/17
                //        continue;
                //    String playtime = ele[10];
                //    if (int.Parse(playtime) < 60000)          //播放时长小于60s
                //        continue;

                //    String userId = ele[0];
                //    String videoId = ele[1];

                //    Boolean haveUser = aa.ifHaveUser(userId);
                //    if (haveUser == false)
                //    {
                //        AprioriAlgorithm.userList.Add(userId);
                //        List<int> tempList = new List<int>();
                //        AprioriAlgorithm.recordMap.Add(aa.findUserIndex(userId), tempList);
                //    }
                //    Boolean haveVideo = aa.ifHaveVideo(videoId);
                //    if (haveVideo == false)
                //    {
                //        AprioriAlgorithm.videoList.Add(videoId);
                //    }
                //    int userIndex = aa.findUserIndex(userId);
                //    int videoIndex = aa.findVideoIndex(videoId);

                //    AprioriAlgorithm.recordMap[userIndex].Add(videoIndex);

                //        numLimit++;
                //}

                //Console.WriteLine("userList的大小为：" + AprioriAlgorithm.userList.Count);
                //Console.WriteLine("videoList的大小为：" + AprioriAlgorithm.videoList.Count);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            AprioriAlgorithm.userNum = AprioriAlgorithm.userList.Count;
            AprioriAlgorithm.itemNum = AprioriAlgorithm.videoList.Count;

            //int[][] recordMatrix = new int[AprioriAlgorithm.userNum][];
            //for (int i=0;i<recordMatrix.Length;i++)
            //{
            //    recordMatrix[i] = new int[AprioriAlgorithm.itemNum];
            //}

            //		int[][] recordMatrix = { { 0,1,1,0,0}, { 0,0,1,0,0}, { 1,0,0,0,0}, { 0,0,0,0,0},
            //				{ 0,1,1,0,0}, { 0,1,1,0,0}, { 1,0,0,0,1}, { 1,0,1,1,1}, { 1,1,1,1,1}, { 0,0,0,1,0}};   //原始数据矩阵
            /*  Console.WriteLine("******原始数据矩阵为：");
                  for(int i=0;i<recordMatrix.length;i++){
                      for(int j=0;j<recordMatrix[0].length;j++){
                          System.out.print(recordMatrix[i][j]+" ");
                      }
                      System.out.println();
                  }*/

            //分析数据
            aa.findOneFre();       //搜索第一层频繁集合
            aa.findTwoFre();       //第二层
            aa.findThreeFre();     //第三层
            aa.findFourFre();      //第四层

            //输出分析结果
            try
            {
                /*	File file = new File("/Users/apple/Desktop/Work/AprioriResult.txt");
                    if(!file.exists()){
                        file.createNewFile();
                    }*/                //没有该文件时创建，否则覆盖

                using (SqlConnection writeconn = new SqlConnection("Server=cia-sh-04; Database=OnlineLearningResult;" +
                              "Integrated Security=True")) //Defines SQL connection using the connection string.
                {
                    writeconn.Open(); //Opens the connection, will throw exception if it fails.
                    foreach (KeyValuePair<int, List<int>> entry in AprioriAlgorithm.videoRankTwo)
                    {
                        String videoId = AprioriAlgorithm.videoList[entry.Key];
                        String assoCandidate1 = "";
                        String assoCandidate2 = "";
                        String assoCandidate3 = "";
                        String assoCandidate4 = "";
                        String assoCandidate5 = "";
                        String assoCandidate6 = "";
                        if (entry.Value.Count > 0)
                            assoCandidate1 = AprioriAlgorithm.videoList[entry.Value[0]];
                        if (entry.Value.Count > 1)
                            assoCandidate2 = AprioriAlgorithm.videoList[entry.Value[1]];
                        if (entry.Value.Count > 2)
                            assoCandidate3 = AprioriAlgorithm.videoList[entry.Value[2]];
                        if (entry.Value.Count > 3)
                            assoCandidate4 = AprioriAlgorithm.videoList[entry.Value[3]];
                        if (entry.Value.Count > 4)
                            assoCandidate5 = AprioriAlgorithm.videoList[entry.Value[4]];
                        if (entry.Value.Count > 5)
                            assoCandidate6 = AprioriAlgorithm.videoList[entry.Value[5]];

                        SqlCommand writecom = new SqlCommand();
                        writecom.Connection = writeconn;
                        writecom.CommandType = System.Data.CommandType.Text;
                        writecom.CommandText = "insert into AssociateResult values('" + videoId + "','" + assoCandidate1 +
                                                 "','" + assoCandidate2 + "','" + assoCandidate3 + "','" + assoCandidate4 +
                                                 "','" + assoCandidate5 + "','" + assoCandidate6 + "'," + 
                                                    initialTime + "," + terminalTime + ")";
                        SqlDataReader reader = writecom.ExecuteReader();
                        reader.Close();
                    }

                    writeconn.Close();
                }

                StreamWriter writer = new StreamWriter(new FileStream("C:/Users/t-xubai/Desktop/DataAnalysis/"
                         + "AprioriDatabaseResultRank032207010930.txt", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write),Encoding.Default);      //连接原始数据集
                writer.Write("startprotime:"+ startprotime+"\r\n");
                writer.Write("分析数据的结果为：\r\n");
                writer.Write("recordNumLimit的值为：" + AprioriAlgorithm.recordNumLimit + "\r\n");
                writer.Write("minSupThreshold的值为：" + AprioriAlgorithm.minSupThreshold + "\r\n");
                writer.Write("userList的大小为：" + AprioriAlgorithm.userList.Count + "\r\n");
                writer.Write("videoList的大小为：" + AprioriAlgorithm.videoList.Count + "\r\n");
                
                writer.Write("******第一层频繁集合为:\r\n");

                foreach (int temp in AprioriAlgorithm.freOne)
                {
                    writer.Write("(" + temp + "), ");
                }
                writer.Write("\r\n");
                foreach (String temp in AprioriAlgorithm.freOneId)
                {
                    writer.Write("(" + temp + "), ");
                }
                if (AprioriAlgorithm.freOne.Count == 0)
                {
                    writer.Write("无一层频繁集合\r\n");
                }
                else {
                    writer.Write("一共有" + AprioriAlgorithm.freOne.Count + "个集合！\r\n");
                }

                writer.Write("******第二层频繁集合为:\r\n");
                foreach (List<int> temp in AprioriAlgorithm.freTwo)
                {
                    writer.Write("(" + temp[0] + "," + temp[1] + "), ");
                }
                writer.Write("\r\n");
                foreach (List<String> temp in AprioriAlgorithm.freTwoId)
                {
                    writer.Write("(" + temp[0] + "," + temp[1] + "), \r\n");
                }
                if (AprioriAlgorithm.freTwo.Count == 0)
                {
                    writer.Write("无二层频繁集合\r\n");
                }
                else {
                    writer.Write("一共有" + AprioriAlgorithm.freTwo.Count + "个集合！\r\n");
                }

                writer.Write("******第三层频繁集合为:\r\n");
                foreach (List<int> temp in AprioriAlgorithm.freThree)
                {
                    writer.Write("(" + temp[0] + "," + temp[1] + "," + temp[2] + "), ");
                }
                writer.Write("\r\n");
                foreach (List<String> temp in AprioriAlgorithm.freThreeId)
                {
                    writer.Write("(" + temp[0] + "," + temp[1] + "," + temp[2] + "), \r\n");
                }
                if (AprioriAlgorithm.freThree.Count == 0)
                {
                    writer.Write("无三层频繁集合\r\n");
                }
                else {
                    writer.Write("一共有" + AprioriAlgorithm.freThree.Count + "个集合！\r\n");
                }

                writer.Write("******第四层频繁集合为:\r\n");
                foreach (List<int> temp in AprioriAlgorithm.freFour)
                {
                    writer.Write("(" + temp[0] + "," + temp[1] + "," + temp[2] + "," + temp[3] + "), ");
                }
                writer.Write("\r\n");
                foreach (List<String> temp in AprioriAlgorithm.freFourId)
                {
                    writer.Write("(" + temp[0] + "," + temp[1] + "," + temp[2] + "," + temp[3] + "), \r\n");
                }
                if (AprioriAlgorithm.freFour.Count == 0)
                {
                    writer.Write("无四层频繁集合\r\n");
                }
                else {
                    writer.Write("一共有" + AprioriAlgorithm.freFour.Count + "个集合！\r\n");
                }
                String endprotime = DateTime.Now.ToString();
                Console.WriteLine("endprotime：" + endprotime);
                writer.Write("endprotime:" + endprotime+"\r\n");
                writer.Flush();     //将缓冲区中的数据强制写出
                writer.Close();     //关闭流
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            Console.ReadLine();
        }

    }
}
