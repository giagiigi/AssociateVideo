package apriori;

import java.util.ArrayList;     
import java.util.Arrays;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Comparator;

public class AprioriAlgorithm {

	public static int userNum;           //用户数量
	public static int itemNum;           //视频数量
	public static int recordNumLimit;            //将要分析的记录数量
	public static double minSupThreshold;        //最小支持度
	
	public static double scoreOfOneDay = 3.0;          //时间间隔的权重
	public static double scoreOfThreeDay = 2.0;
	public static double scoreOfOneWeek = 1.0;
	public static double scoreOfOneMonth = 0.5;
	public static double scoreOfThreeMonth = 0.1;
	
    public static Map<Integer,List<Integer>> recordMap = new HashMap<Integer,List<Integer>>();   //记录观看的人
    public static Map<Integer,List<String>> recordStartTimeMap = new HashMap<Integer,List<String>>();  //记录观看的开始时间
	
	public static List<String> userList = new ArrayList<String>();      //用户列表
	public static List<String> videoList = new ArrayList<String>();     //视频列表
	
	public static List<Integer> freOne = new ArrayList<Integer>();      //第一层频繁的视频下标
	public static List<String> freOneId = new ArrayList<String>();      //第一层频繁的视频id
	
	public static List<List<Integer>> freTwo = new ArrayList<List<Integer>>();   //第二层频繁的视频下标
	public static List<List<String>> freTwoId = new ArrayList<List<String>>();   //第二层频繁的视频id
	
	public static List<List<Integer>> freThree = new ArrayList<List<Integer>>();   //第三层频繁的视频下标
	public static List<List<String>> freThreeId = new ArrayList<List<String>>();   //第三层频繁的视频id
	
	public static List<List<Integer>> freFour = new ArrayList<List<Integer>>();    //第四层频繁的视频下标
	public static List<List<String>> freFourId = new ArrayList<List<String>>();   //第三层频繁的视频id
	
	public static Map<Integer,List<Integer>> videoRankTwo = new HashMap<Integer,List<Integer>>();    //二层排名
	public static Map<Integer,Integer> frequTimeOne = new HashMap<Integer,Integer>();           //一层的频率
	public static Map<List<Integer>,Double> frequTimeTwo = new HashMap<List<Integer>,Double>();     //二层的频率
	
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
	
	public void findOneFre(){         //寻找一层频繁集合
		
		System.out.println("******第一层频繁集合为:");
		int[] sumOne = new int[itemNum];                     //寻找第一次频繁的视频id
		double[] ratioOne = new double[itemNum];
		
		for(int i=0;i<itemNum;i++){
//			for(int j=0;j<userNum;j++){
//				sumOne[i] = sumOne[i] + recordMatrix[j][i];
//			}
			for(int j=0;j<userNum;j++){
				if(recordMap.get(j).contains(i)){
					sumOne[i]++;
				}
			}
			ratioOne[i] = (double)(sumOne[i])/userNum;
		//	System.out.println(ratioOne[i]);
		}
			
		for(int i=0;i<ratioOne.length;i++){
			if(ratioOne[i]>=minSupThreshold){
				freOne.add(i);                              //第一层，将视频id加入列表中
				frequTimeOne.put(i, sumOne[i]);
			    System.out.print("("+i+"), ");
			}
		}
		System.out.println();
		for(int temp:freOne){
			String tempId = videoList.get(temp);
			freOneId.add(tempId);
			System.out.print("("+tempId+"), ");
		}
		
		if(freOne.size()==0){
			System.out.println("无一层频繁集合");
		}
		else{
			System.out.println("一共有"+freOne.size()+"个集合！");
		}
	}
	
	public void findTwoFre(){       //寻找二层频繁集合
		
		System.out.println("******第二层频繁集合为:");
		for(int i=0;i<freOne.size();i++){
			for(int j=i+1;j<freOne.size();j++){
				
				int[] eleTwo = new int[2];
				eleTwo[0] = freOne.get(i);
				eleTwo[1] = freOne.get(j);
				
				List<Integer> temp = new ArrayList<Integer>();
				int togetherNum = 0; 
				double score = 0;
				
				for(int k=0;k<userNum;k++){
//					if(recordMatrix[k][eleTwo[0]]==1&&recordMatrix[k][eleTwo[1]]==1){
//						togetherNum++;
//					}	
	                if(recordMap.get(k).contains(eleTwo[0])&&recordMap.get(k).contains(eleTwo[1])){
	                	togetherNum++;
	                	
	                	String eleTime0 = recordStartTimeMap.get(k).get(recordMap.get(k).indexOf(eleTwo[0]));     //计算时间差值，赋予不同的权值(分数)
	                	String eleTime1 = recordStartTimeMap.get(k).get(recordMap.get(k).indexOf(eleTwo[1]));
	                
	                	int finaltime0 = Integer.parseInt(eleTime0);
	                	int finaltime1 = Integer.parseInt(eleTime1);
	                	int difftime = Math.abs(finaltime0-finaltime1);
	                	if(difftime<=86400)
	                		score = score + scoreOfOneDay;
	                	else if(difftime<=259200)
	                		score = score + scoreOfThreeDay;
	                	else if(difftime<=604800)
	                		score = score + scoreOfOneWeek;
	                	else if(difftime<=2629743)
	                		score = score + scoreOfOneMonth;
	                	else if(difftime<=7889229)
	                		score = score + scoreOfThreeMonth;
	                }
				}
				
				double ratioTwo = (double)(togetherNum)/userNum;
				if(ratioTwo>=minSupThreshold){
					temp.add(eleTwo[0]);
					temp.add(eleTwo[1]);
					freTwo.add(temp);                      //第二层，将视频id加入列表中
				//	frequTimeTwo.put(temp, togetherNum);
					frequTimeTwo.put(temp, score);
					System.out.print("("+temp.get(0)+","+temp.get(1)+") "+score+", ");
				}
			}
		}
		System.out.println();
		for(List<Integer> temp:freTwo){
			List<String> tempIdList = new ArrayList<String>();
			for(int othtemp:temp){
				String tempId = videoList.get(othtemp);
				tempIdList.add(tempId);
			}
			freTwoId.add(tempIdList);
			System.out.print("("+tempIdList.get(0)+","+tempIdList.get(1)+"), ");
		}
		
		for(List<Integer> temp:freTwo){              //生成videoRankTwo
			int ele0 = temp.get(0);
			int ele1 = temp.get(1);
			if(videoRankTwo.keySet().contains(ele0)){
				videoRankTwo.get(ele0).add(ele1);
			}
			else{
				List<Integer> templist = new ArrayList<Integer>();
				videoRankTwo.put(ele0, templist);
				videoRankTwo.get(ele0).add(ele1);
			}
			if(videoRankTwo.keySet().contains(ele1)){
				videoRankTwo.get(ele1).add(ele0);
			}
			else{
				List<Integer> templist = new ArrayList<Integer>();
				videoRankTwo.put(ele1, templist);
				videoRankTwo.get(ele1).add(ele0);
			}
		}
		System.out.println();
		System.out.println("排序前videoRankTwo："+videoRankTwo);
		for(Map.Entry<Integer, List<Integer>> entry: videoRankTwo.entrySet()){     //根据支持度排序videoRankTwo
			Collections.sort(entry.getValue(),new Comparator<Integer>(){
				public int compare(Integer o1, Integer o2) {
					double num1 = 0;
					double num2 = 0; 
					List<Integer> l11 = new ArrayList<Integer>();
					List<Integer> l12 = new ArrayList<Integer>();
					l11.add(entry.getKey());
					l11.add(o1);
					l12.add(o1);
					l12.add(entry.getKey());
					if(frequTimeTwo.containsKey(l11)){
						num1 = frequTimeTwo.get(l11);
					}
					else if(frequTimeTwo.containsKey(l12)){
						num1 = frequTimeTwo.get(l12);
					}
					List<Integer> l21 = new ArrayList<Integer>();
					List<Integer> l22 = new ArrayList<Integer>();
					l21.add(entry.getKey());
					l21.add(o2);
					l22.add(o2);
					l22.add(entry.getKey());
					if(frequTimeTwo.containsKey(l21)){
						num2 = frequTimeTwo.get(l21);
					}
					else if(frequTimeTwo.containsKey(l22)){
						num2 = frequTimeTwo.get(l22);
					}
					if(num2>num1)
						return 1;
					else
						return -1;
				}
			});
		}
		System.out.println("排序后videoRankTwo："+videoRankTwo);
		if(freTwo.size()==0){
			System.out.println("无二层频繁集合");
		}
		else{
			System.out.println("一共有"+freTwo.size()+"个集合！");
		}
	}
	
	public void findThreeFre(){       //寻找三层频繁集合
		
		System.out.println("******第三层频繁集合为:");
		for(int i=0;i<freOne.size();i++){
			for(int j=0;j<freTwo.size();j++){
			
				if(freOne.get(i).equals(freTwo.get(j).get(0))||freOne.get(i).equals(freTwo.get(j).get(1)))
					continue;
				
				int[] eleThree = new int[3];
				eleThree[0] = freOne.get(i);
				eleThree[1] = freTwo.get(j).get(0);
				eleThree[2] = freTwo.get(j).get(1);
				
				List<Integer> temp = new ArrayList<Integer>();
				int togetherNum = 0; 
				for(int k=0;k<userNum;k++){
//					if(recordMatrix[k][eleThree[0]]==1&&recordMatrix[k][eleThree[1]]==1&&
//							recordMatrix[k][eleThree[2]]==1){
//						togetherNum++;
//					}	
				    if(recordMap.get(k).contains(eleThree[0])&&recordMap.get(k).contains(eleThree[1])&&recordMap.get(k).contains(eleThree[2])){
		                	togetherNum++;
		            }
				}
				
				double ratioThree = (double)(togetherNum)/userNum;
				if(ratioThree>=minSupThreshold){
					temp.add(eleThree[0]);
					temp.add(eleThree[1]);
					temp.add(eleThree[2]);
					freThree.add(temp);                      //第三层，将视频id加入列表中
			//		System.out.println(eleThree[0]+" "+eleThree[1]+" "+eleThree[2]);
				}
			}
		}
		//消除重复集合
		int[][] tempThreeMatrix = new int[freThree.size()][3];
		for(int i=0;i<freThree.size();i++){
			tempThreeMatrix[i][0] = freThree.get(i).get(0);
			tempThreeMatrix[i][1] = freThree.get(i).get(1);
			tempThreeMatrix[i][2] = freThree.get(i).get(2);
			Arrays.sort(tempThreeMatrix[i]);
		}
		int numOfDelete = 0;
		for(int i=1;i<tempThreeMatrix.length;i++){
			for(int j=0; j<i;j++){
				if(tempThreeMatrix[i][0]==tempThreeMatrix[j][0]&&tempThreeMatrix[i][1]==tempThreeMatrix[j][1]&&
					  tempThreeMatrix[i][2]==tempThreeMatrix[j][2]){
					freThree.remove(i-numOfDelete);           //消除重复集合
					numOfDelete++;
					break;
				}
			}
		}
	//	System.out.println("消除重复的三层频繁集合，结果为:");
		for(int i=0;i<freThree.size();i++){
			System.out.print("("+freThree.get(i).get(0)+","+freThree.get(i).get(1)+","+freThree.get(i).get(2)+"), ");
		}
		System.out.println();
		for(List<Integer> temp:freThree){
			List<String> tempIdList = new ArrayList<String>();
			for(int othtemp:temp){
				String tempId = videoList.get(othtemp);
				tempIdList.add(tempId);
			}
			freThreeId.add(tempIdList);
			System.out.print("("+tempIdList.get(0)+","+tempIdList.get(1)+","+tempIdList.get(2)+"), ");
		}
		
		if(freThree.size()==0){
			System.out.println("无三层频繁集合");
		}
		else{
			System.out.println("一共有"+freThree.size()+"个集合！");
		}
	}
	
	public void findFourFre(){
		
		System.out.println("******第四层频繁集合为:");
		for(int i=0;i<freOne.size();i++){
			for(int j=0;j<freThree.size();j++){
				
				if(freOne.get(i).equals(freThree.get(j).get(0))||freOne.get(i).equals(freThree.get(j).get(1))||
						freOne.get(i).equals(freThree.get(j).get(2)))            //避免重复集合
					continue;

				int[] eleFour = new int[4];
				eleFour[0] = freOne.get(i);
				eleFour[1] = freThree.get(j).get(0);
				eleFour[2] = freThree.get(j).get(1);
				eleFour[3] = freThree.get(j).get(2);
				
				List<Integer> temp = new ArrayList<Integer>();
				int togetherNum = 0; 
				for(int k=0;k<userNum;k++){
//					if(recordMatrix[k][eleFour[0]]==1&&recordMatrix[k][eleFour[1]]==1&&
//							recordMatrix[k][eleFour[2]]==1&&recordMatrix[k][eleFour[3]]==1){
//						togetherNum++;
//					}
					 if(recordMap.get(k).contains(eleFour[0])&&recordMap.get(k).contains(eleFour[1])&&recordMap.get(k).contains(eleFour[2])
							               &&recordMap.get(k).contains(eleFour[3])){
		                	togetherNum++;
		            }
				}
				
				double ratioFour = (double)(togetherNum)/userNum;
				if(ratioFour>=minSupThreshold){
					temp.add(eleFour[0]);
					temp.add(eleFour[1]);
					temp.add(eleFour[2]);
					temp.add(eleFour[3]);
					freFour.add(temp);                      //第四层，将视频id加入列表中
			//		System.out.println(eleFour[0]+" "+eleFour[1]+" "+eleFour[2]+" "+eleFour[3]);
				}
			} 
		}
		//消除重复集合
		int[][] tempFourMatrix = new int[freFour.size()][4];
		for(int i=0;i<freFour.size();i++){
			tempFourMatrix[i][0] = freFour.get(i).get(0);
			tempFourMatrix[i][1] = freFour.get(i).get(1);
			tempFourMatrix[i][2] = freFour.get(i).get(2);
			tempFourMatrix[i][3] = freFour.get(i).get(3);
			Arrays.sort(tempFourMatrix[i]);
		}
		int numOfDelete = 0;
		for(int i=1;i<tempFourMatrix.length;i++){
			for(int j=0; j<i;j++){
				if(tempFourMatrix[i][0]==tempFourMatrix[j][0]&&tempFourMatrix[i][1]==tempFourMatrix[j][1]&&
						tempFourMatrix[i][2]==tempFourMatrix[j][2]&&tempFourMatrix[i][3]==tempFourMatrix[j][3]){
					freFour.remove(i-numOfDelete);           //消除重复集合
					numOfDelete++;
					break;
				}
			}
		}
	//	System.out.println("消除重复的四层频繁集合，结果为:");
		for(int i=0;i<freFour.size();i++){
			System.out.print("("+freFour.get(i).get(0)+","+freFour.get(i).get(1)+","+
		                               freFour.get(i).get(2)+","+freFour.get(i).get(3)+"), ");
		}
		System.out.println();
		for(List<Integer> temp:freFour){
			List<String> tempIdList = new ArrayList<String>();
			for(int othtemp:temp){
				String tempId = videoList.get(othtemp);
				tempIdList.add(tempId);
			}
			freFourId.add(tempIdList);
			System.out.print("("+tempIdList.get(0)+","+tempIdList.get(1)+","+tempIdList.get(2)+","+tempIdList.get(3)+"), ");
		}
		
		if(freFour.size()==0){
			System.out.println("无四层频繁集合");
		}
		else{
			System.out.println("一共有"+freFour.size()+"个集合！");
		}
	}
	
}
