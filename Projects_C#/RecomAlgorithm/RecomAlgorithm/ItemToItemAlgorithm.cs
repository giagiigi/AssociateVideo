using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace RecomAlgorithm
{
    class ItemToItemAlgorithm
    {
        public static int recordNumLimit;
        public static double adjThreshold;
        public static int itemNum;
        public static int userNum;

        public static List<String> videoList = new List<String>();
        public static List<String> userList = new List<String>();

        public static SortedList<String, List<String>> itemRecomResult = new SortedList<String,List<String>>();

        public Boolean ifHaveUser(String userId)
        {           //用户列表是否包含该用户
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
        {       //视频列表是否包含该视频
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
        {             //查找用户的下标
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
        {          //查找视频的下标
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

        public double calCos(int[] vector1, int[] vector2)
        {
            double res = 0;

            int innerProduct = 0;
            for (int i = 0; i < vector1.Length; i++)
            {
                innerProduct += vector1[i] * vector2[i];
            }

            double len1 = 0.0;
            double len2 = 0.0;
            for (int i = 0; i < vector1.Length; i++)
            {
                len1 += vector1[i] * vector1[i];
            }
            for (int i = 0; i < vector2.Length; i++)
            {
                len2 += vector2[i] * vector2[i];
            }
            len1 = Math.Sqrt(len1);
            len2 = Math.Sqrt(len2);

            res = (double)(innerProduct) / (len1 * len2);

            return res;
        }

    }
}
