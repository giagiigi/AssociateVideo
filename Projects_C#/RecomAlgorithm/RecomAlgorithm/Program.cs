using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace RecomAlgorithm
{
    class Program
    {
        static void Main(string[] args)
        {

            ItemToItemAlgorithm itia = new ItemToItemAlgorithm();
            ItemToItemAlgorithm.recordNumLimit = 100000;
            ItemToItemAlgorithm.adjThreshold = 0.8660;     //30度

            Console.WriteLine("开始的时间：" + DateTime.Now.ToString());

            //输入原始数据
            try
            {              //查询用户和视频数量
                StreamReader readerForList = new StreamReader(new FileStream("C:/Users/t-xubai/Desktop/DataAnalysis/"
                    + "DailySessionLog_Microsoft-DPE_2015-03-18.csv", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Read));      //连接原始数据集
         //     String Annotations = readerForList.ReadLine();                    //数据集的注释
                String Attributes = readerForList.ReadLine();                    //数据集的属性

                Console.WriteLine("******原始数据的属性为：");
                Console.WriteLine(Attributes);
                Console.WriteLine("recordNumLimit的值为：" + ItemToItemAlgorithm.recordNumLimit);
                Console.WriteLine("adjThreshold的值为：" + ItemToItemAlgorithm.adjThreshold);

                String record = null;
                int numLimit = 0;                             //控制分析记录的数量
                while ((record = readerForList.ReadLine()) != null)
                {
                    if (numLimit >= ItemToItemAlgorithm.recordNumLimit)
                        break;
                    String[] ele = record.Split(',');
                    String userId = ele[0];
                    String videoId = ele[1];
                    //	System.out.println("用户ID: "+userId+" "+"视频Id: "+videoId);

                    Boolean haveVideo = itia.ifHaveVideo(videoId);
                    if (haveVideo == false)
                    {
                        ItemToItemAlgorithm.videoList.Add(videoId);
                    }
                    Boolean haveUser = itia.ifHaveUser(userId);
                    if (haveUser == false)
                    {
                        ItemToItemAlgorithm.userList.Add(userId);
                    }
                    numLimit++;
                }

                Console.WriteLine("videoList的大小为：" + ItemToItemAlgorithm.videoList.Count);
                Console.WriteLine("userList的大小为：" + ItemToItemAlgorithm.userList.Count);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            ItemToItemAlgorithm.itemNum = ItemToItemAlgorithm.videoList.Count;
            ItemToItemAlgorithm.userNum = ItemToItemAlgorithm.userList.Count;
            int[][] recordMatrix = new int[ItemToItemAlgorithm.itemNum][];
            for (int i = 0; i < recordMatrix.Length; i++)
            {
                recordMatrix[i] = new int[ItemToItemAlgorithm.userNum];
            }

            try
            {                  //建立初始数据矩阵
                StreamReader newReaderForMatrix = new StreamReader(new FileStream("C:/Users/t-xubai/Desktop/DataAnalysis/"
                     + "DailySessionLog_Microsoft-DPE_2015-03-18.csv", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Read));      //连接原始数据集
           //   newReaderForMatrix.ReadLine();                    //数据集的注释
                newReaderForMatrix.ReadLine();                    //数据集的属性

                String tempRecord = null;
                int tempNumLimit = 0;                             //控制分析记录的数量
                while ((tempRecord = newReaderForMatrix.ReadLine()) != null)
                {
                    if (tempNumLimit >= ItemToItemAlgorithm.recordNumLimit)
                        break;
                    String[] ele = tempRecord.Split(',');
                    String userId = ele[0];
                    String videoId = ele[1];
                    //	System.out.println("用户ID: "+userId+" "+"视频Id: "+videoId);

                    int videoIndex = itia.findVideoIndex(videoId);
                    int userIndex = itia.findUserIndex(userId);
                    recordMatrix[videoIndex][userIndex] = 1;

                    tempNumLimit++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            /*		int[][] recordMatrix = { {0,0,1,0,0,0,1,1,1,0},{1,0,0,0,1,1,0,0,1,0},{1,1,0,0,1,1,0,1,1,0},
                            {0,0,0,0,0,0,0,1,1,1},{0,0,0,0,0,0,1,1,1,0}};   //原始数据矩阵
                    */
            /*		System.out.println("******原始数据矩阵为：");
                    for(int i=0;i<recordMatrix.length;i++){
                        for(int j=0;j<recordMatrix[0].length;j++){
                            System.out.print(recordMatrix[i][j]+" ");
                        }
                        System.out.println();
                    }
                    */
            for (int k = 0; k < ItemToItemAlgorithm.itemNum; k++)
            {
                List<String> adjVideoList = new List<String>();
                ItemToItemAlgorithm.itemRecomResult.Add(ItemToItemAlgorithm.videoList[k], adjVideoList);
            }

            //分析数据
            for (int i = 0; i < ItemToItemAlgorithm.itemNum; i++)
            {
                for (int j = i + 1; j < ItemToItemAlgorithm.itemNum; j++)
                {
                    double cos = 0.0;
                    cos = itia.calCos(recordMatrix[i], recordMatrix[j]);
                    if (cos >= ItemToItemAlgorithm.adjThreshold)
                    {
                        ItemToItemAlgorithm.itemRecomResult[ItemToItemAlgorithm.videoList[i]].
                                     Add(ItemToItemAlgorithm.videoList[j]);
                        ItemToItemAlgorithm.itemRecomResult[ItemToItemAlgorithm.videoList[j]].
                                     Add(ItemToItemAlgorithm.videoList[i]);
                    }
                }
            }

            //输出分析结果
            try
            {
                StreamWriter writer = new StreamWriter(new FileStream("C:/Users/t-xubai/Desktop/DataAnalysis/"
                   + "RecomResult.txt", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write), Encoding.Default);      //连接原始数据集
                writer.Write("分析数据的结果为：\r\n");
                writer.Write("recordNumLimit的值为：" + ItemToItemAlgorithm.recordNumLimit + "\r\n");
                writer.Write("adjThreshold的值为：" + ItemToItemAlgorithm.adjThreshold + "\r\n");
                writer.Write("videoList的大小为：" + ItemToItemAlgorithm.videoList.Count + "\r\n");
                writer.Write("userList的大小为：" + ItemToItemAlgorithm.userList.Count + "\r\n");

                int adjNumber = 0;
                Console.WriteLine("******分析后的相邻视频为：");
                for (int i = 0; i < ItemToItemAlgorithm.itemNum; i++)
                {
                    if (ItemToItemAlgorithm.itemRecomResult[ItemToItemAlgorithm.videoList[i]].Count == 0)
                        continue;
                    //		System.out.println(ItemToItemAlgorithm.videoList.get(i)+": "+
                    //                           ItemToItemAlgorithm.itemRecomResult.get(ItemToItemAlgorithm.videoList.get(i)));
                    adjNumber++;
                }
                Console.WriteLine("adjNumber的值为:" + adjNumber);

                for (int j = 0; j < ItemToItemAlgorithm.itemNum; j++)
                {
                    if (ItemToItemAlgorithm.itemRecomResult[ItemToItemAlgorithm.videoList[j]].Count == 0)
                        continue;
                    writer.Write(ItemToItemAlgorithm.videoList[j] + ": [");
                    foreach (String temp in ItemToItemAlgorithm.itemRecomResult[ItemToItemAlgorithm.videoList[j]])
                    {
                        writer.Write(temp+" ");
                    }
                    writer.WriteLine("]\r\n");
                }
                writer.Flush();     //将缓冲区中的数据强制写出
                writer.Close();     //关闭流 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("结束的时间：" + DateTime.Now.ToString());
            Console.ReadLine();
        }

    }
}
