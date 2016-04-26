using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Apriori
{
    class AprioriAlgorithm
    {
        public static int userNum;           //用户数量
        public static int itemNum;           //视频数量
        public static int recordNumLimit;            //将要分析的记录数量
        public static double minSupThreshold;        //最小支持度

        public static double scoreOfOneDay = 3.0;      //时间间隔的权重
        public static double scoreOfThreeDay = 2.0;
        public static double scoreOfOneWeek = 1.0;
        public static double scoreOfOneMonth = 0.5;
        public static double scoreOfThreeMonth = 0.1;

        public static Dictionary<int, List<int>> recordMap = new Dictionary<int, List<int>>();  //用户观看记录，存储结构
        public static Dictionary<int, List<int>> recordStartTimeMap = new Dictionary<int, List<int>>();  //对应视频开始观看的时间

        public static List<String> userList = new List<String>();      //用户列表
        public static List<String> videoList = new List<String>();     //视频列表

        public static List<int> freOne = new List<int>();      //第一层频繁的视频下标
        public static List<String> freOneId = new List<String>();      //第一层频繁的视频id

        public static List<List<int>> freTwo = new List<List<int>>();   //第二层频繁的视频下标
        public static List<List<String>> freTwoId = new List<List<String>>();   //第二层频繁的视频id

        public static List<List<int>> freThree = new List<List<int>>();   //第三层频繁的视频下标
        public static List<List<String>> freThreeId = new List<List<String>>();   //第三层频繁的视频id

        public static List<List<int>> freFour = new List<List<int>>();    //第四层频繁的视频下标
        public static List<List<String>> freFourId = new List<List<String>>();   //第三层频繁的视频id

        public static Dictionary<int, int> frequTimeOne = new Dictionary<int, int>();           //一层的频率
        public static Dictionary<int, List<int>> videoRankTwo = new Dictionary<int, List<int>>();    //二层排名,videoid映射assocandidate
        public static Dictionary<List<int>, double> frequTimeTwo = new Dictionary<List<int>, double>(new ListCompare());  //videoid,assocandidate映射score
        public static Dictionary<List<int>, List<int>> videoAssoUserid = new Dictionary<List<int>, List<int>>(new ListCompare());    //videoid,assocandidate映射userid
        public static Dictionary<List<int>, List<int>> videoAssoDiffTime = new Dictionary<List<int>, List<int>>(new ListCompare());  //videoid,assocandidate映射difftime

        public sealed class ListCompare : IEqualityComparer<List<int>>   //重写List的相等
        {
            public bool Equals(List<int> li, List<int> l2)
            {
                if (li.GetType() != l2.GetType())
                    return false;
                if (li.Count != l2.Count)
                    return false;
                for (int i = 0; i < li.Count; i++)
                {
                    if (li[i] != l2[i])
                        return false;
                }
                return true;
            }
            public int GetHashCode(List<int> list)
            {
                String sum = "";
                foreach (int i in list)
                {
                    sum = sum + i;
                }
                return sum.GetHashCode();
            }
        }

        public Boolean ifHaveUser(String userId)
        {                 //用户列表是否包含该用户
            Boolean have = false;
            foreach (String temp in userList)
            {
                if (temp.Equals(userId))
                {
                    have = true;
                    break;
                }
            }
            return have;
        }

        public Boolean ifHaveVideo(String videoId)
        {                 //视频列表是否包含该视频
            Boolean have = false;
            foreach (String temp in videoList)
            {
                if (temp.Equals(videoId))
                {
                    have = true;
                    break;
                }
            }
            return have;
        }

        public int findUserIndex(String userId)
        {                 //查找用户的下标
            int res = 0;
            for (int i = 0; i < userList.Count; i++)
            {
                if (userList[i].Equals(userId))
                {
                    res = i;
                    break;
                }
            }
            return res;
        }

        public int findVideoIndex(String videoId)
        {                //查找视频的下标
            int res = 0;
            for (int i = 0; i < videoList.Count; i++)
            {
                if (videoList[i].Equals(videoId))
                {
                    res = i;
                    break;
                }
            }
            return res;
        }

        public void findOneFre()
        {                //寻找一层频繁集合
            Console.WriteLine("******第一层频繁集合为:");
            int[] sumOne = new int[itemNum];                     //寻找第一次频繁的视频id
            double[] ratioOne = new double[itemNum];

            for (int i = 0; i < itemNum; i++)
            {
                //for (int j = 0; j < userNum; j++)
                //{
                //    sumOne[i] = sumOne[i] + recordMatrix[j][i];
                //}
                for(int j=0;j< userNum; j++)
                {
                    if (recordMap[j].Contains(i))
                    {
                        sumOne[i]++;
                    }
                }
                ratioOne[i] = (double)(sumOne[i]) / userNum;
                //	System.out.println(ratioOne[i]); 
                if (ratioOne[i] >= minSupThreshold)
                {
                    freOne.Add(i);                              //第一层，将视频id加入列表中
                    frequTimeOne.Add(i, sumOne[i]);
                    Console.Write("(" + i + "), ");
                }
            }
            Console.WriteLine();
            foreach (int temp in freOne)
            {
                String tempId = videoList[temp];
                freOneId.Add(tempId);
                //Console.Write("(" + tempId + "), ");
            }

            if (freOne.Count == 0)
            {
                Console.WriteLine("无一层频繁集合");
            }
            else {
                Console.WriteLine("一共有" + freOne.Count + "个集合！");
            }
        }

        public void findTwoFre()
        {       //寻找二层频繁集合
            Console.WriteLine("******第二层频繁集合为:");
            for (int i = 0; i < freOne.Count; i++)
            {
                for (int j = i + 1; j < freOne.Count; j++)
                {
                    int[] eleTwo = new int[2];
                    eleTwo[0] = freOne[i];
                    eleTwo[1] = freOne[j];

                    int togetherNum = 0;
                    double score = 0;
                    List<int> useridlist = new List<int>();     //同时观看这两个关联视频的用户
                    List<int> difftimelist = new List<int>();   //同时观看这两个关联视频时间差，两个链表对应

                    for (int k = 0; k < userNum; k++)
                    {
                        //if (recordMatrix[k][eleTwo[0]] == 1 && recordMatrix[k][eleTwo[1]] == 1)
                        //{
                        //    togetherNum++;
                        //}
                        if (recordMap[k].Contains(eleTwo[0]) && recordMap[k].Contains(eleTwo[1]))
                        {
                            togetherNum++;

                            int finaltime0 = recordStartTimeMap[k][recordMap[k].IndexOf(eleTwo[0])];
                            int finaltime1 = recordStartTimeMap[k][recordMap[k].IndexOf(eleTwo[1])];
                            int difftime = Math.Abs(finaltime0-finaltime1);
                            if (difftime <= 86400)              //根据关联的视频时间差进行打分，出现次数越多，分数增加次数也越多
                                score = score + scoreOfOneDay;
                            else if (difftime <= 259200)
                                score = score + scoreOfThreeDay;
                            else if (difftime <= 604800)
                                score = score + scoreOfOneWeek;
                            else if (difftime <= 2629743)
                                score = score + scoreOfOneMonth;
                            else if (difftime <= 7889229)
                                score = score + scoreOfThreeMonth;

                            useridlist.Add(k);
                            difftimelist.Add(difftime);
                        }
                    }

                    double ratioTwo = (double)(togetherNum) / userNum;
                    if (ratioTwo >= minSupThreshold)
                    {
                        List<int> temp1 = new List<int>();
                        temp1.Add(eleTwo[0]);
                        temp1.Add(eleTwo[1]);

                        List<int> temp2 = new List<int>();
                        temp2.Add(eleTwo[1]);
                        temp2.Add(eleTwo[0]);

                        freTwo.Add(temp1);                      //第二层，将视频id加入列表中
                        //frequTimeTwo.Add(temp,togetherNum);
                        frequTimeTwo.Add(temp1, score);
                        frequTimeTwo.Add(temp2, score);
                        //Console.Write("(" + temp1[0] + "," + temp1[1] + ") "+ score + ", ");

                        videoAssoUserid.Add(temp1,useridlist);
                        videoAssoUserid.Add(temp2, useridlist);
                        videoAssoDiffTime.Add(temp1,difftimelist);
                        videoAssoDiffTime.Add(temp2, difftimelist);
                    }
                }
            }
            Console.WriteLine();
            foreach (List<int> temp in freTwo)                 //将freTwo生成freTwoId
            {
                List<String> tempIdList = new List<String>();
                foreach (int othtemp in temp)
                {
                    String tempId = videoList[othtemp];
                    tempIdList.Add(tempId);
                }
                freTwoId.Add(tempIdList);
                //Console.Write("(" + tempIdList[0] + "," + tempIdList[1] + "), ");
            }

            Console.WriteLine();
            foreach (List<int> temp in freTwo)
            {                                                      //生成videoRankTwo
                int ele0 = temp[0];
                int ele1 = temp[1];
                if (videoRankTwo.ContainsKey(ele0))
                {
                    videoRankTwo[ele0].Add(ele1);
                }
                else {
                    List<int> templist = new List<int>();
                    videoRankTwo.Add(ele0, templist);
                    videoRankTwo[ele0].Add(ele1);
                }
                if (videoRankTwo.ContainsKey(ele1))
                {
                    videoRankTwo[ele1].Add(ele0);
                }
                else {
                    List<int> templist = new List<int>();
                    videoRankTwo.Add(ele1, templist);
                    videoRankTwo[ele1].Add(ele0);
                }
            }
            //Console.WriteLine("before sorting videoRankTwo：");
            //foreach (KeyValuePair<int,List<int>> entry in videoRankTwo)
            //{
            //    Console.Write(entry.Key+"=[");
            //    foreach (int temp in entry.Value)
            //    {
            //        Console.Write(temp+", ");
            //    }
            //    Console.Write("]  ");
            //}
            Console.WriteLine();
            foreach (KeyValuePair<int, List<int>> entry in videoRankTwo)
            {                                                    //根据score排序videoRankTwo的值
                entry.Value.Sort((int o1, int o2) =>
                {            //lamda表达式
                    double num1 = 0;
                    double num2 = 0;
                    List<int> l1 = new List<int>();
                    l1.Add(entry.Key);
                    l1.Add(o1);
                    num1 = frequTimeTwo[l1];

                    List<int> l2 = new List<int>();
                    l2.Add(entry.Key);
                    l2.Add(o2);
                    num2 = frequTimeTwo[l2];
       
                    if (num2 > num1)
                        return 1;
                    else
                        return -1;
                });
            }

            Console.WriteLine("after sorting videoRankTwo：");
            //foreach (KeyValuePair<int, List<int>> entry in videoRankTwo)
            //{
            //    Console.Write(entry.Key + "=[");
            //    foreach (int temp in entry.Value)
            //    {
            //        Console.Write(temp + ", ");
            //    }
            //    Console.Write("]  ");
            //}
            Console.WriteLine();

            if (freTwo.Count == 0)
            {
                Console.WriteLine("无二层频繁集合");
            }
            else {
                Console.WriteLine("一共有" + freTwo.Count + "个集合！");
            }
        }

        public void findThreeFre()
        {       //寻找三层频繁集合
            Console.WriteLine("******第三层频繁集合为:");
            for (int i = 0; i < freOne.Count; i++)
            {
                for (int j = 0; j < freTwo.Count; j++)
                {

                    if (freOne[i] == freTwo[j][0] || freOne[i] == freTwo[j][1])  //避免重复集合
                        continue;

                    int[] eleThree = new int[3];
                    eleThree[0] = freOne[i];
                    eleThree[1] = freTwo[j][0];
                    eleThree[2] = freTwo[j][1];

                    List<int> temp = new List<int>();
                    int togetherNum = 0;
                    for (int k = 0; k < userNum; k++)
                    {
                        //if (recordMatrix[k][eleThree[0]] == 1 && recordMatrix[k][eleThree[1]] == 1 &&
                        //        recordMatrix[k][eleThree[2]] == 1)
                        //{
                        //    togetherNum++;
                        //}
                        if (recordMap[k].Contains(eleThree[0]) && recordMap[k].Contains(eleThree[1]) && recordMap[k].Contains(eleThree[2]))
                        {
                            togetherNum++;
                        }
                    }

                    double ratioThree = (double)(togetherNum) / userNum;
                    if (ratioThree >= minSupThreshold)
                    {
                        temp.Add(eleThree[0]);
                        temp.Add(eleThree[1]);
                        temp.Add(eleThree[2]);
                        freThree.Add(temp);                      //第三层，将视频id加入列表中
                                                                 //		System.out.println(eleThree[0]+" "+eleThree[1]+" "+eleThree[2]);
                    }
                }
            }
            //消除重复集合
            int[][] tempThreeMatrix = new int[freThree.Count][];
            for(int i = 0; i < tempThreeMatrix.Length; i++)
            {
                tempThreeMatrix[i] = new int[3];
            }
            for (int i = 0; i < freThree.Count; i++)
            {
                tempThreeMatrix[i][0] = freThree[i][0];
                tempThreeMatrix[i][1] = freThree[i][1];
                tempThreeMatrix[i][2] = freThree[i][2];
                Array.Sort(tempThreeMatrix[i]);
            }
            int numOfDelete = 0;
            for (int i = 1; i < tempThreeMatrix.Length; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (tempThreeMatrix[i][0] == tempThreeMatrix[j][0] && tempThreeMatrix[i][1] == tempThreeMatrix[j][1] &&
                          tempThreeMatrix[i][2] == tempThreeMatrix[j][2])
                    {
                        freThree.RemoveAt(i - numOfDelete);           //消除重复集合
                        numOfDelete++;
                        break;
                    }
                }
            }
            //	System.out.println("消除重复的三层频繁集合，结果为:");
            for (int i = 0; i < freThree.Count; i++)
            {
                Console.Write("(" + freThree[i][0] + "," + freThree[i][1] + "," + freThree[i][2] + "), ");
            }
            Console.WriteLine();
            foreach (List<int> temp in freThree)
            {
                List<String> tempIdList = new List<String>();
                foreach (int othtemp in temp)
                {
                    String tempId = videoList[othtemp];
                    tempIdList.Add(tempId);
                }
                freThreeId.Add(tempIdList);
                Console.Write("(" + tempIdList[0] + "," + tempIdList[1] + "," + tempIdList[2] + "), ");
            }

            if (freThree.Count == 0)
            {
                Console.WriteLine("无三层频繁集合");
            }
            else {
                Console.WriteLine("一共有" + freThree.Count + "个集合！");
            }
        }

        public void findFourFre()
        {          //寻找四层频繁集合
            Console.WriteLine("******第四层频繁集合为:");
            for (int i = 0; i < freOne.Count; i++)
            {
                for (int j = 0; j < freThree.Count; j++)
                {
                    if (freOne[i] == freThree[j][0] || freOne[i] == freThree[j][1] ||
                            freOne[i] == freThree[j][2])            //避免重复集合
                        continue;

                    int[] eleFour = new int[4];
                    eleFour[0] = freOne[i];
                    eleFour[1] = freThree[j][0];
                    eleFour[2] = freThree[j][1];
                    eleFour[3] = freThree[j][2];

                    List<int> temp = new List<int>();
                    int togetherNum = 0;
                    for (int k = 0; k < userNum; k++)
                    {
                        //if (recordMatrix[k][eleFour[0]] == 1 && recordMatrix[k][eleFour[1]] == 1 &&
                        //        recordMatrix[k][eleFour[2]] == 1 && recordMatrix[k][eleFour[3]] == 1)
                        //{
                        //    togetherNum++;
                        //}
                        if (recordMap[k].Contains(eleFour[0]) && recordMap[k].Contains(eleFour[1]) && recordMap[k].Contains(eleFour[2])
                                                    && recordMap[k].Contains(eleFour[3]))
                        {
                            togetherNum++;
                        }
                    }

                    double ratioFour = (double)(togetherNum) / userNum;
                    if (ratioFour >= minSupThreshold)
                    {
                        temp.Add(eleFour[0]);
                        temp.Add(eleFour[1]);
                        temp.Add(eleFour[2]);
                        temp.Add(eleFour[3]);
                        freFour.Add(temp);        //第四层，将视频id加入列表中
                    }
                }
            }
            //消除重复集合
            int[][] tempFourMatrix = new int[freFour.Count][];
            for (int i = 0; i < tempFourMatrix.Length; i++)
            {
                tempFourMatrix[i] = new int[4];
            }
            for (int i = 0; i < freFour.Count; i++)
            {
                tempFourMatrix[i][0] = freFour[i][0];
                tempFourMatrix[i][1] = freFour[i][1];
                tempFourMatrix[i][2] = freFour[i][2];
                tempFourMatrix[i][3] = freFour[i][3];
                Array.Sort(tempFourMatrix[i]);
            }
            int numOfDelete = 0;
            for (int i = 1; i < tempFourMatrix.Length; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (tempFourMatrix[i][0] == tempFourMatrix[j][0] && tempFourMatrix[i][1] == tempFourMatrix[j][1] &&
                            tempFourMatrix[i][2] == tempFourMatrix[j][2] && tempFourMatrix[i][3] == tempFourMatrix[j][3])
                    {
                        freFour.RemoveAt(i - numOfDelete);           //消除重复集合
                        numOfDelete++;
                        break;
                    }
                }
            }
            //	System.out.println("消除重复的四层频繁集合，结果为:");
            for (int i = 0; i < freFour.Count; i++)
            {
                Console.Write("(" + freFour[i][0] + "," + freFour[i][1] + "," +
                                           freFour[i][2] + "," + freFour[i][3] + "), ");
            }
            Console.WriteLine();
            foreach (List<int> temp in freFour)
            {
                List<String> tempIdList = new List<String>();
                foreach (int othtemp in temp)
                {
                    String tempId = videoList[othtemp];
                    tempIdList.Add(tempId);
                }
                freFourId.Add(tempIdList);
                Console.Write("(" + tempIdList[0] + "," + tempIdList[1] + "," + tempIdList[2] + "," + tempIdList[3] + "), ");
            }

            if (freFour.Count == 0)
            {
                Console.WriteLine("无四层频繁集合");
            }
            else {
                Console.WriteLine("一共有" + freFour.Count + "个集合！");
            }
        }
    }
}
