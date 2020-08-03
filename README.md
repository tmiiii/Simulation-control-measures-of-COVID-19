# Simulation-control-measures-of-COVID-19

### 仿真背景
仿真程序基于电子科技大学清水河校区的宿舍区、学习区（教室、图书馆）、食堂、体育馆以及用作隔离病人的校医院的真实地图制作模拟场景对新冠病毒在校园中的传播进行模拟，并且设计了五种可选防疫措施分别验证防疫效果：  
+ 保持社交距离  
+ 戴口罩  
+ 对确诊者进行隔离  
+ 对有症状感染者进行隔离  
+ 对无症状传染源进行隔离  

通过对这些防疫措施进行排列组合来验证防疫措施对疫情防控所起的作用大小，并最终给出防疫建议。

### 程序介绍
1.本程序假设校园中可能存在以下四种学生：  
+ 健康学生 
+ 无症状感染者  
+ 有症状感染者  
+ 确诊者  

由于本程序研究新冠病毒的传播，因此不考虑新冠病毒患者的治愈以及死亡情况。初始情况为校园中有一位无症状感染者，
其余学生为健康学生。

2.学生在校园内每天的活动轨迹如下：
    **宿舍->学习区->食堂->宿舍->学习区->(体育馆->)宿舍**  

其中宿舍为白色区域，学习区为绿色区域、食堂为蓝色区域，体育馆为黑色区域，医院为橙色区域，每个学生每天去往的学习区和食堂在三个学习区和三个食堂内随机生成，并且每个学生每天有20%的几率在下午下课后去往体育馆进行锻炼，在锻炼之后返回宿舍。  
![Image text](https://github.com/TMITMiTmitmi/Simulation-control-measures-of-COVID-19/blob/master/image/image6.png)

3.学生患病过程为：**健康学生->无症状感染者->有症状感染者->确诊者**    
+ 健康学生->无症状感染者  
    健康学生在与传染源接触后根据感染率函数有概率感染新冠病毒成为无症状感染者。  
+ 无症状感染者->有症状感染者  
    无症状感染者在潜伏期后会出现症状成为有症状感染者。  
+ 有症状感染者->确诊者  
    有症状感染者在出现症状的一天后确诊感染新冠病毒。  

### 程序假设
1.根据文献[1]对于新冠病毒潜伏期的研究，假设新冠病毒潜伏期为2~14天。

2.根据文献[2]对于一个22岁中国学生对其密切接触者的感染案例的研究，假设大学生在密切接触情况下感染率为50%；根据《泰晤士报》发表的数据假设健康学生距离感染源1m被感染的概率为2.6%；假设感染函数为下凸递减函数，并且健康学生在距离感染源2m之外不会被感染，用y = a * （e^-(b*x) + c）拟合感染率函数为$y = 0.502 *(e^(-2.9*x) - 0.003)$。

3.根据文献[2]对于22岁中国学生密切接触者感染案例分析，假设健康学生在感染病毒之后的一天才会成为传染源以及被核酸检测呈阳性。
 
4.根据文献[3]对于口罩对新冠病毒感染率影响的研究，假设在学校内戴口罩将会使感染率下降到原先的十分之一。

5.仿真程序假设人与人之间需要保持的社交距离为1.2m。

6.根据文献[4]对于新冠病毒在环境中的存活时间的研究，假设新冠病毒能在室内残留2天。但由于新冠病毒主要通过飞沫传播，并且在仿真过程中室内残留病毒对于结果影响不大，因此忽略室内残留病毒对感染的影响。  

7.由于不发病的无症状感染者占感染者比例较小，因此假设所有无症状感染者都会发病转变为有症状感染者。  

### 程序原理
1.程序通过不同颜色的小球模拟不同状态的学生，其中蓝色小球为健康学生，黄色小球为感染病毒一天后成为传染源的无症状感染者，橙色小球为出现症状的感染者，红色小球为确诊者，除蓝色小球外其他颜色小球均可以感染蓝色小球使其变为黄色小球。  
    
2.程序通过在每一帧计算蓝色小球与其他颜色小球的距离，将距离代入上文的感染率函数求得小球的感染率，用随机数判断小球是否被感染。  

3.在保持社交距离条件下，程序通过在每一帧计算该小球与其他小球的距离，如果距离大于用于模拟1.2m社交距离的0.6，就将该小球与其他小球的距离加0.6来保持距离。  

4.在戴口罩条件下，用于判断感染的感染率函数变为基础感染率函数大小的十分之一。  

5.在对确诊者进行隔离、对有症状感染者进行隔离、对无症状传染源进行隔离的条件下，分别在每天的开始将红色、橙色、黄色小球送进医院隔离。  

### 实验数据分析
*说明：五种措施分别为：*  
+ *1：保持社交距离*    
+ *2：戴口罩*  
+ *3：对确诊者进行隔离*  
+ *4：对有症状感染者进行隔离*  
+ *5：对无症状传染源进行隔离*       

*对于实验数据的分析分为两个部分：*     
+ *1.最终所有学生都被感染：通过实验记录十次仿真中所有学生都被感染的日期的平均值判断防治措施效果，日期越大效果越好，涉及无措施、措施1、2、3、4、12、13、14、23、24、123、124的情况。*    
+ *2.最终只有个别学生被感染：说明疫情最终被控制，通过实验记录十次仿真中被最初的感染者感染的人数的平均值以及最初的感染者感染别人的次数判断防治措施效果，感染人数越少、感染次数越少，说明防治措施效果越好，涉及措施5、15、25、125的情况。*  

**数据分析：**    

   1.由图1可知在无措施以及分别采取措施1、2、3、4的情况下，戴口罩的效果最好，保持社交距离其次，而隔离学生的措施并没有成效，原因是在短时间内很少有同学达到出现症状阶段和确诊阶段，因此对于措施3、4来说传染源并没有减少，感染率也没有下降。  
   
![Image text](https://github.com/TMITMiTmitmi/Simulation-control-measures-of-COVID-19/blob/master/image/image1.png)

   2.由图2可知，在保持社交距离，即采取措施1的情况下，采取措施2、23、24的防治效果逐步提高，并且戴口罩效果提升最为显著，而隔离措施没有很大成效，原因同1。
   
![Image text](https://github.com/TMITMiTmitmi/Simulation-control-measures-of-COVID-19/blob/master/image/image2.png)

   3.由图3可知，在戴口罩，即采取措施2的情况下，采取措施1、13、14的防治效果逐步提高，并且保持社交距离提升效果最为显著，而隔离措施没有很大成效，原因同1。
   
![Image text](https://github.com/TMITMiTmitmi/Simulation-control-measures-of-COVID-19/blob/master/image/image3.png)

   4.由图4和图5可知，只要学校采取将无症状传染源隔离的措施，疫情就会得到控制，并且由措施5、15、25的数据可以看出，在隔离无症状传染源学生的情况下，防疫措施效果由好到差排列为：戴口罩>保持社交距离>不采取措施。同时在仿真中采取措施25、125时最初的感染者均没有感染其他人，仿真效果没有差别，与实际大范围情况可能有所差别。
   
![Image text](https://github.com/TMITMiTmitmi/Simulation-control-measures-of-COVID-19/blob/master/image/image4.png)

![Image text](https://github.com/TMITMiTmitmi/Simulation-control-measures-of-COVID-19/blob/master/image/image5.png)

### 防疫建议
+ 最理想的情况是学校对全体学生进行核酸检测，将无症状传染源隔离，同时规定学生戴口罩以及保持社交距离，这样可以在学校出现感染者时将学校内疫情控制住。  
+ 若没有条件进行核酸检测，则规定学生每天测量体温，上报身体情况，将有症状的学生进行隔离，同时规定学生戴口罩以及保持社交距离，在仿真中采取以上措施大大延长了所有学生被感染的时间。仿真只是小范围模拟，存在局限性，但是可以推测在诸如学校的大规模场所采取以上措施也会有效果。
+ 同时，即使本程序由于仿真规模较小忽略了室内残留病毒的影响，可以预见在诸如学校的大规模场所这种影响也是不可忽略的，因此学校需要定期对校内环境进行消毒杀菌，并且学生需要采取勤洗手等措施避免接触病毒，在非人传人途径抑制病毒传播。

### 备注
1.仿真程序逻辑代码位置：Assets/Scripts/NavigationAgent.cs，需在代码中修改变量改变防疫措施  
2.仿真程序可执行文件位置：Simulation.rar/COVID-19.exe  
3.仿真数据位置：Data.xlsx  

### 参考文献
[1]Linton, N.M.; Kobayashi, T.; Yang, Y.; Hayashi, K.; Akhmetzhanov, A.R.; Jung, S.-M.; Yuan, B.; Kinoshita, R.; 
Nishiura, H. Incubation Period and Other Epidemiological Characteristics of 2019 Novel Coronavirus Infections 
with Right Truncation: A Statistical Analysis of Publicly Available Case Data. J. Clin. Med. 2020, 9, 538.

[2]Lei Huang, Xiuwen Zhang, Xinyue Zhang, Zhijian Wei, Lingli Zhang, Jingjing Xu, Peipei Liang, Yuanhong Xu, 
Chengyuan Zhang, Aman Xu, Rapid asymptomatic transmission of COVID-19 during the incubation period 
demonstrating strong infectivity in a cluster of youngsters aged 16-23 years outside Wuhan and characteristics 
of young patients with COVID-19: A prospective contact-tracing study, Journal of Infection, Volume 80, Issue 6, 
2020, Pages e1-e13, ISSN 0163-4453,

[3]Steffen E. Eikenberry, Marina Mancuso, Enahoro Iboi, Tin Phan, Keenan Eikenberry, Yang Kuang, Eric Kostelich,
 Abba B. Gumel, To mask or not to mask: Modeling the potential for face mask use by the general public to curtail
 the COVID-19 pandemic, Infectious Disease Modelling, Volume 5, 2020, Pages 293-308, ISSN 2468-0427,  
 
[4]Federica Carraturo, Carmela Del Giudice, Michela Morelli, Valeria Cerullo, Giovanni Libralato, Emilia Galdiero, 
Marco Guida, Persistence of SARS-CoV-2 in the environment and COVID-19 transmission risk from environmental matrices
and surfaces, Environmental Pollution, Volume 265, Part B, 2020, 115010, ISSN 0269-7491,  

### 参考程序
https://github.com/justinw-fun/COVID-19SimulationInCollege

### 仿真环境
Unity 2020.2.0a18.2359
