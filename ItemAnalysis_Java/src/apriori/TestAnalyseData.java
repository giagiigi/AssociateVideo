package apriori;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.FileReader;
import java.io.FileWriter;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

public class TestAnalyseData {

	public static void main(String[] args) {

		AprioriAlgorithm aa = new AprioriAlgorithm();
		AprioriAlgorithm.recordNumLimit = 100000;
		AprioriAlgorithm.minSupThreshold = 0.003;
		
		Date start = new Date();
		System.out.println("开始时间："+start);
		
		//输入原始数据
		try{              //查询用户和视频数量
			BufferedReader readerForList = new BufferedReader(new FileReader("/Users/apple/Desktop/Work/dataset/"
					+ "DailySessionLog_Microsoft-DPE_2015-03-18.csv"));      //连接原始数据集
			String Annotations = readerForList.readLine();                    //数据集的注释
			String Attributes = readerForList.readLine();                    //数据集的属性
			
			System.out.println("******原始数据的属性为：");
			System.out.println(Attributes);
			System.out.println("recordNumLimit的值为："+AprioriAlgorithm.recordNumLimit);
			System.out.println("minSupThreshold的值为："+AprioriAlgorithm.minSupThreshold);
			
			String record = null;
			int numLimit = 0;                             //控制分析记录的数量
			while((record=readerForList.readLine())!=null){
				
				String ele[] = record.split(",");
				
				String pencentage = ele[22];                     //播放时间比例小于3/17
				if(Integer.parseInt(pencentage)<17.65)
					continue;
				String playtime = ele[10];
				if(Integer.parseInt(playtime)<60000)             //播放时长小于60s
					continue;
				
				if(numLimit>=AprioriAlgorithm.recordNumLimit)
					break;
				
				String userId = ele[0];
				String videoId = ele[1];
				
				String startTime = ele[8];
			//	System.out.println("用户ID: "+userId+" "+"视频Id: "+videoId);
				
				boolean haveUser = aa.ifHaveUser(userId);
				if(haveUser==false){
					AprioriAlgorithm.userList.add(userId);
					List<Integer> tempList = new ArrayList<Integer>();
					AprioriAlgorithm.recordMap.put(aa.findUserIndex(userId), tempList);
					
					List<String> timetempList = new ArrayList<String>();
					AprioriAlgorithm.recordStartTimeMap.put(aa.findUserIndex(userId), timetempList);
				}
				boolean haveVideo = aa.ifHaveVideo(videoId);
				if(haveVideo==false){
					AprioriAlgorithm.videoList.add(videoId);
				}
				
				int userIndex = aa.findUserIndex(userId);
				int videoIndex = aa.findVideoIndex(videoId);
				AprioriAlgorithm.recordMap.get(userIndex).add(videoIndex);
				AprioriAlgorithm.recordStartTimeMap.get(userIndex).add(startTime);
				
				numLimit++;
			}
			
			System.out.println("userList的大小为："+AprioriAlgorithm.userList.size());
			System.out.println("videoList的大小为："+AprioriAlgorithm.videoList.size());			
		}
		catch(Exception e){
			e.printStackTrace(); 
		}
		
		AprioriAlgorithm.userNum = AprioriAlgorithm.userList.size();
		AprioriAlgorithm.itemNum = AprioriAlgorithm.videoList.size();
//		int[][] recordMatrix = new int[AprioriAlgorithm.userNum][AprioriAlgorithm.itemNum];
		
//		try{                  //建立初始数据矩阵(稀疏矩阵)，包括行索引链表、列索引链表、值链表(值均为1)
//			BufferedReader newReaderForMatrix = new BufferedReader(new FileReader("/Users/apple/Desktop/"
//					+ "Work/results/DailySessionLog_Microsoft-DPE_2015-03-18.csv"));      //连接原始数据集
//			newReaderForMatrix.readLine();                    //数据集的注释
//			newReaderForMatrix.readLine();                    //数据集的属性
//			
//			String tempRecord = null;
//			int tempNumLimit = 0;                             //控制分析记录的数量
//			while((tempRecord=newReaderForMatrix.readLine())!=null){
//				
//				String ele[] = tempRecord.split(",");
//				
//				String pencentage = ele[22];
//				if(Integer.parseInt(pencentage)<17.65)
//					continue;
//				String playtime = ele[10];
//				if(Integer.parseInt(playtime)<60000)
//					continue;
//
//				if(tempNumLimit>=AprioriAlgorithm.recordNumLimit)
//					break;
//				
//				String userId = ele[0];
//				String videoId = ele[1];
//			//	System.out.println("用户ID: "+userId+" "+"视频Id: "+videoId);
//				int userIndex = aa.findUserIndex(userId);
//				int videoIndex = aa.findVideoIndex(videoId);
//				AprioriAlgorithm.recordMap.get(userIndex).add(videoIndex);
//				
//				tempNumLimit++;
//			}
//		}
//		catch(Exception e){
//			e.printStackTrace();
//		}
		
//		int[][] recordMatrix = { { 0,1,1,0,0}, { 0,0,1,0,0}, { 1,0,0,0,0}, { 0,0,0,0,0},
//				{ 0,1,1,0,0}, { 0,1,1,0,0}, { 1,0,0,0,1}, { 1,0,1,1,1}, { 1,1,1,1,1}, { 0,0,0,1,0}};   //原始数据矩阵
//		System.out.println("******原始数据矩阵为：");
//		for(int i=0;i<recordMatrix.length;i++){
//			for(int j=0;j<recordMatrix[0].length;j++){
//				System.out.print(recordMatrix[i][j]+" ");
//			}
//			System.out.println();
//		}
		
		//分析数据
		aa.findOneFre();       //搜索第一层频繁集合
		aa.findTwoFre();       //第二层
		aa.findThreeFre();     //第三层
		aa.findFourFre();      //第四层
		
		//输出分析结果
		try{
		/*	File file = new File("/Users/apple/Desktop/Work/AprioriResult.txt");
			if(!file.exists()){
				file.createNewFile();
			}*/                //没有该文件时创建，否则覆盖
			BufferedWriter writer = new BufferedWriter(new FileWriter("/Users/apple/Desktop/Work/tempresult/AprioriResult031002.txt",false));
			writer.write("分析数据的结果为：\r\n");
			writer.write("recordNumLimit的值为："+AprioriAlgorithm.recordNumLimit+"\r\n");
			writer.write("minSupThreshold的值为："+AprioriAlgorithm.minSupThreshold+"\r\n");
			writer.write("userList的大小为："+AprioriAlgorithm.userList.size()+"\r\n");
			writer.write("videoList的大小为："+AprioriAlgorithm.videoList.size()+"\r\n");
		
			writer.write("******第一层频繁集合为:\r\n");
			
			for(int temp:AprioriAlgorithm.freOne){
				writer.write("("+temp+"), ");
			}
			writer.newLine();
			for(String temp:AprioriAlgorithm.freOneId){
				writer.write("("+temp+"), ");
			}
			if(AprioriAlgorithm.freOne.size()==0){
				writer.write("无一层频繁集合\r\n");
			}
			else{
				writer.write("一共有"+AprioriAlgorithm.freOne.size()+"个集合！\r\n");
			}
			
			writer.write("******第二层频繁集合为:\r\n");
			for(List<Integer> temp:AprioriAlgorithm.freTwo){
				writer.write("("+temp.get(0)+","+temp.get(1)+"), ");
			}
			writer.newLine();
			for(List<String> temp:AprioriAlgorithm.freTwoId){
				writer.write("("+temp.get(0)+","+temp.get(1)+"), \r\n");
			}
			if(AprioriAlgorithm.freTwo.size()==0){
				writer.write("无二层频繁集合\r\n");
			}
			else{
				writer.write("一共有"+AprioriAlgorithm.freTwo.size()+"个集合！\r\n");
			}
			
			writer.write("******第三层频繁集合为:\r\n");
			for(List<Integer> temp:AprioriAlgorithm.freThree){
				writer.write("("+temp.get(0)+","+temp.get(1)+","+temp.get(2)+"), ");
			}
			writer.newLine();
			for(List<String> temp:AprioriAlgorithm.freThreeId){
				writer.write("("+temp.get(0)+","+temp.get(1)+","+temp.get(2)+"), \r\n");
			}
			if(AprioriAlgorithm.freThree.size()==0){
				writer.write("无三层频繁集合\r\n");
			}
			else{
				writer.write("一共有"+AprioriAlgorithm.freThree.size()+"个集合！\r\n");
			}
			
			writer.write("******第四层频繁集合为:\r\n");
			for(List<Integer> temp:AprioriAlgorithm.freFour){
				writer.write("("+temp.get(0)+","+temp.get(1)+","+temp.get(2)+","+temp.get(3)+"), ");
			}
			writer.newLine();
			for(List<String> temp:AprioriAlgorithm.freFourId){
				writer.write("("+temp.get(0)+","+temp.get(1)+","+temp.get(2)+","+temp.get(3)+"), \r\n");
			}
			if(AprioriAlgorithm.freFour.size()==0){
				writer.write("无四层频繁集合\r\n");
			}
			else{
				writer.write("一共有"+AprioriAlgorithm.freFour.size()+"个集合！\r\n");
			}
			
			writer.flush();     //将缓冲区中的数据强制写出
			writer.close();     //关闭流
		}
		catch(Exception e){
			e.printStackTrace();
		}
		
		Date end = new Date();
		System.out.println("结束时间："+end);
	}

}
