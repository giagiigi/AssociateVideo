package itemtoitem;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class ItemToItemAlgorithm {

	public static int recordNumLimit;
	public static double adjThreshold;
	public static int itemNum;
	public static int userNum;
	
	public static List<String> videoList = new ArrayList<String>();
	public static List<String> userList = new ArrayList<String>();
	
	public static Map<String,List<String>> itemRecomResult = new HashMap<String,List<String>>();
	
	public boolean ifHaveUser(String userId){           //用户列表是否包含该用户
		boolean have = false;
		for(String temp : userList){
			if(temp.equals(userId)){
				have = true;
				break;
			}
		}
		return have;
	}
	
	public boolean ifHaveVideo(String videoId){       //视频列表是否包含该视频
		boolean have = false;
		for(String temp : videoList){
			if(temp.equals(videoId)){
				have = true;
				break;
			}
		}
		return have;
	}
	
	public int findUserIndex(String userId){             //查找用户的下标
		int res = 0;
		for(int i=0;i<userList.size();i++){
			if(userList.get(i).equals(userId)){
				res = i;
				break;
			}
		}
		return res;
	}
	
	public int findVideoIndex(String videoId){          //查找视频的下标
		int res = 0;
		for(int i=0;i<videoList.size();i++){
			if(videoList.get(i).equals(videoId)){
				res = i;
				break;
			}
		}
		return res;
	}
	
	public double calCos(int[] vector1,int[] vector2){
		double res = 0;
		
		int innerProduct = 0;
		for(int i=0;i<vector1.length;i++){
			innerProduct += vector1[i]*vector2[i];
		}
		
		double len1 = 0.0;
		double len2 = 0.0;
		for(int i=0;i<vector1.length;i++){
			len1 += vector1[i]*vector1[i];
		}
		for(int i=0;i<vector2.length;i++){
			len2 += vector2[i]*vector2[i];
		}
		len1 = Math.sqrt(len1);
		len2 = Math.sqrt(len2);
		
		res = (double)(innerProduct)/(len1*len2);
				
		return res;
	}
	
}
