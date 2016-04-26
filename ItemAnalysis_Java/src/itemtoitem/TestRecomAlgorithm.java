package itemtoitem;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.FileReader;
import java.io.FileWriter;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

public class TestRecomAlgorithm {

	public static void main(String[] args) {
		
		ItemToItemAlgorithm itia = new ItemToItemAlgorithm();
		ItemToItemAlgorithm.recordNumLimit = 3000;
		ItemToItemAlgorithm.adjThreshold = 0.8660;     //30度
		
		Date start = new Date();
		System.out.println("开始的时间："+start);
		
		//输入原始数据
		try{              //查询用户和视频数量
			BufferedReader readerForList = new BufferedReader(new FileReader("/Users/apple/Desktop/"
					+ "Work/results/DailySessionLog_Microsoft-DPE_2015-03-18.csv"));      //连接原始数据集
			String Annotations = readerForList.readLine();                    //数据集的注释
			String Attributes = readerForList.readLine();                    //数据集的属性
			
			System.out.println("******原始数据的属性为：");
			System.out.println(Attributes);
			System.out.println("recordNumLimit的值为："+ItemToItemAlgorithm.recordNumLimit);
			System.out.println("adjThreshold的值为："+ItemToItemAlgorithm.adjThreshold);
			
			String record = null;
			int numLimit = 0;                             //控制分析记录的数量
			while((record=readerForList.readLine())!=null){
				if(numLimit>=ItemToItemAlgorithm.recordNumLimit)
					break;
				String ele[] = record.split(",");
				String userId = ele[0];
				String videoId = ele[1];
			//	System.out.println("用户ID: "+userId+" "+"视频Id: "+videoId);
				
				boolean haveVideo = itia.ifHaveVideo(videoId);
				if(haveVideo==false){
					ItemToItemAlgorithm.videoList.add(videoId);
				}
				boolean haveUser = itia.ifHaveUser(userId);
				if(haveUser==false){
					ItemToItemAlgorithm.userList.add(userId);
				}
				numLimit++;
			}
			
			System.out.println("videoList的大小为："+ItemToItemAlgorithm.videoList.size());	
			System.out.println("userList的大小为："+ItemToItemAlgorithm.userList.size());
		}
		catch(Exception e){
			e.printStackTrace(); 
		}
		
		ItemToItemAlgorithm.itemNum = ItemToItemAlgorithm.videoList.size();
		ItemToItemAlgorithm.userNum = ItemToItemAlgorithm.userList.size();
		int[][] recordMatrix = new int[ItemToItemAlgorithm.itemNum][ItemToItemAlgorithm.userNum];
		
		try{                  //建立初始数据矩阵
			BufferedReader newReaderForMatrix = new BufferedReader(new FileReader("/Users/apple/Desktop/"
					+ "Work/results/DailySessionLog_Microsoft-DPE_2015-03-18.csv"));      //连接原始数据集
			newReaderForMatrix.readLine();                    //数据集的注释
			newReaderForMatrix.readLine();                    //数据集的属性
			
			String tempRecord = null;
			int tempNumLimit = 0;                             //控制分析记录的数量
			while((tempRecord=newReaderForMatrix.readLine())!=null){
				if(tempNumLimit>=ItemToItemAlgorithm.recordNumLimit)
					break;
				String ele[] = tempRecord.split(",");
				String userId = ele[0];
				String videoId = ele[1];
			//	System.out.println("用户ID: "+userId+" "+"视频Id: "+videoId);

				int videoIndex = itia.findVideoIndex(videoId);
				int userIndex = itia.findUserIndex(userId);
				recordMatrix[videoIndex][userIndex] = 1;
				
				tempNumLimit++;
			}
		}
		catch(Exception e){
			e.printStackTrace();
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
		for(int k=0;k<ItemToItemAlgorithm.itemNum;k++){
			List<String> adjVideoList = new ArrayList<String>();
			ItemToItemAlgorithm.itemRecomResult.put(ItemToItemAlgorithm.videoList.get(k), adjVideoList);
		}
		
		//分析数据
	    for(int i=0;i<ItemToItemAlgorithm.itemNum;i++){
	    	for(int j=i+1;j<ItemToItemAlgorithm.itemNum;j++){
	    		double cos = 0.0;
	    		cos = itia.calCos(recordMatrix[i],recordMatrix[j]);
	    		if(cos>=ItemToItemAlgorithm.adjThreshold){
	    			ItemToItemAlgorithm.itemRecomResult.get(ItemToItemAlgorithm.videoList.get(i)).
	    			             add(ItemToItemAlgorithm.videoList.get(j));
	    			ItemToItemAlgorithm.itemRecomResult.get(ItemToItemAlgorithm.videoList.get(j)).
	    			             add(ItemToItemAlgorithm.videoList.get(i));
	    		}
	    	}
	    }
		
		//输出分析结果
		try{
			BufferedWriter writer = new BufferedWriter(new FileWriter("/Users/apple/Desktop/Work/results/RecomResult.txt",false));
			writer.write("分析数据的结果为：\r\n");
			writer.write("recordNumLimit的值为："+ItemToItemAlgorithm.recordNumLimit+"\r\n");
			writer.write("adjThreshold的值为："+ItemToItemAlgorithm.adjThreshold+"\r\n");
			writer.write("videoList的大小为："+ItemToItemAlgorithm.videoList.size()+"\r\n");
			writer.write("userList的大小为："+ItemToItemAlgorithm.userList.size()+"\r\n");
			
			int adjNumber = 0;
			System.out.println("******分析后的相邻视频为：");
			for(int i=0;i<ItemToItemAlgorithm.itemNum;i++){
				if(ItemToItemAlgorithm.itemRecomResult.get(ItemToItemAlgorithm.videoList.get(i)).size()==0)
					continue;
		//		System.out.println(ItemToItemAlgorithm.videoList.get(i)+": "+
			//                           ItemToItemAlgorithm.itemRecomResult.get(ItemToItemAlgorithm.videoList.get(i)));
				adjNumber++;
			}
			System.out.println("adjNumber的值为:"+adjNumber);
			
			for(int j=0;j<ItemToItemAlgorithm.itemNum;j++){
				if(ItemToItemAlgorithm.itemRecomResult.get(ItemToItemAlgorithm.videoList.get(j)).size()==0)
					continue;
				writer.write(ItemToItemAlgorithm.videoList.get(j)+": "+
			                           ItemToItemAlgorithm.itemRecomResult.get(ItemToItemAlgorithm.videoList.get(j))+"\r\n");
			}
			writer.flush();     //将缓冲区中的数据强制写出
			writer.close();     //关闭流
		}
		catch(Exception e){
			e.printStackTrace();
		}

		Date end = new Date();
		System.out.println("结束的时间："+end);
	}

}
