# Simulation-control-measures-of-COVID-19

### 仿真场景
仿真程序以电子科技大学清水河校区的宿舍区、学习区（教室、图书馆）、食堂、体育馆以及用作隔离病人的校医院为背景对新冠病毒在校园中的传播进行模拟，并且设计了五种可选防疫措施分别验证防疫效果：  
+ 保持社交距离  
+ 戴口罩  
+ 对确诊者进行医疗隔离  
+ 对出现症状者进行隔离  
+ 对核酸检测呈阳性者进行隔离  

通过对这些防疫措施进行排列组合来验证防疫措施对疫情防控所起的作用大小，并最终给出防疫建议。

### 程序介绍
1.本程序假设校园中可能存在以下四种学生：  
+ 易感者  
+ 无症状被感染者  
+ 有症状被感染者  
+ 确诊者  

由于本程序研究新冠病毒的传播，因此不考虑新冠病毒患者的治愈以及死亡情况。初始情况为校园中有一位无症状感染者，
其余学生为易感者。

2.学生在校园内每天的活动轨迹如下：
    **宿舍->教室->食堂->宿舍->教室->(体育馆->)宿舍**  

其中每个学生每天目的地的教室和食堂是在三个教室和三个食堂内随机生成，并且每个学生每天有20%的几率在下课后去
往体育馆进行锻炼。  
![Image text](https://github.com/TMITMiTmitmi/Simulation-control-measures-of-COVID-19/blob/master/image/image6.png)

3.感染病毒的过程为：**易感者->无症状感染者->有症状感染者->确诊者**    
+ 易感者->无症状感染者  
    易感者在与传染源接触后根据距离有概率被感染新冠病毒。  
+ 无症状感染者->有症状感染者  
    无症状感染者在潜伏期后会出现新冠病毒的症状。  
+ 有症状感染者->确诊者  
    有症状感染者在出现感染的之后一天确诊感染新冠病毒。  

### 程序假设
1.根据文献[1]对于新冠病毒潜伏期的研究，假设新冠病毒潜伏期为2~14天。

2.根据文献[2]对于一个22岁中国学生对其密切接触者的感染案例的研究，假设大学生在密切接触情况下感染率为50%；根据《泰晤
士报》发表的数据假设易感者距离感染源1m被感染的概率为2.6%；假设感染函数为下凸递减函数，并且易感者在距离感染源2m之外
不会被感染，用y = a * e^-(b*x) + c拟合感染率函数为y = 0.063 * e^-(ln(2)/2)x - 0.5。

3.根据文献[2]对于22岁中国学生密切接触者感染案例分析，假设易感者在感染病毒之后的一天才会成为感染源以及被核酸检测呈阳
性。
 
4.根据文献[3]对于口罩对新冠病毒感染率影响的研究，假设在大学内戴口罩将会使感染率下降到原先的十分之一。

5.仿真程序假设人与人之间需要保持的社交距离为1.2m。

6.根据文献[4]对于新冠病毒在环境中的存活时间的研究，假设新冠病毒能在室内残留2天。但由于新冠病毒主要通过飞沫传播，并且
在仿真过程中室内残留病毒对于结果影响不大，因此忽略室内残留病毒对感染的影响。

### 程序原理
1.程序通过小球模拟学生，蓝色的小球为易感者，黄色的小球为感染病毒一天后成为感染源的无症状感染者，橙色的小球为出现症状
的感染者，红色的小球为确诊者，除蓝色小球外其他颜色小球均可以感染蓝色小球使其变为黄色小球。
    
2.程序通过在每一帧计算蓝色小球与其他颜色小球的距离，将距离代入上文的感染率函数求得小球的感染率，用随机数判断小球是否
被感染。

3.在保持社交距离条件下，程序通过在每一帧计算小球与其他小球的距离，如果距离大于用于模拟1.2m社交距离的0.6，就将该小球与
其他小球的距离加0.6来保持距离。

4.在戴口罩条件下，用于判断感染的感染率函数变为基础感染率函数的十分之一。

5.在对确诊者进行医疗隔离、对出现症状者进行隔离、对核酸检测呈阳性者进行隔离的条件下，分别在每天的开始将红色、橙色、黄色
小球送进医院隔离。

### 实验数据分析
*说明：五种措施分别为：*  
+ *1：保持社交距离*    
+ *2：戴口罩*  
+ *3：对确诊者进行医疗隔离*  
+ *4：对出现症状者进行隔离*  
+ *5：对核酸检测呈阳性者进行隔离*       

*对于实验数据的分析分为两个部分：*     
+ *全校所有同学在最后都被感染，通过实验记录十次仿真全校所有学生都被感染的日期的平均值判断防治措施效果，日期越大效*
*果越好，涉及措施0、1、2、3、4、12、13、14、23、24、123、124。*    
+ *全校只有个别同学在最后被感染，说明疫情最终被控制，通过实验记录十次仿真全校被最初的感染者感染的人数的平均值以及*
*最初的感染者感染别人的次数判断防治措施效果，感染人数越少、感染次数越少，说明防治措施效果越好，涉及措施5、15、25、125。*  

**数据分析：**    

   1.由图1可知在只实行一个措施的时候，戴口罩的效果最好，保持社交距离其次，而对学生隔离的措施并没有成效，原因是在短时间
   内很少有同学到达出现症状阶段和确诊阶段，因此对于措施0、3、4来说感染源并没有减少，感染率也没有下降。  
   
![Image text](https://github.com/TMITMiTmitmi/Simulation-control-measures-of-COVID-19/blob/master/image/image1.png)

   2.由图2可知，在保持社交距离，即采取措施1的情况下，采取措施2、23、24的防治效果逐步提高，并且戴口罩效果提升最为显著，
   而隔离措施没有很大成效，原因同1。
   
![Image text](https://github.com/TMITMiTmitmi/Simulation-control-measures-of-COVID-19/blob/master/image/image2.png)

   3.由图3可知，在带口罩，即采取措施2的情况下，采取措施1、13、14的防治效果逐步提高，并且保持社交距离提升效果最为显著，
   而隔离措施没有很大成效，原因同1。
   
![Image text](https://github.com/TMITMiTmitmi/Simulation-control-measures-of-COVID-19/blob/master/image/image3.png)

   4.由图4和图5可知，只要学校采取将核酸检测阳性的同学隔离的措施，疫情就会得到控制，并且由措施5、15、25的数据可以看出，
   在隔离核酸检测阳性学生的情况下，防疫措施效果由好到坏排列为：戴口罩>保持社交距离>不采取措施。
   
![Image text](https://github.com/TMITMiTmitmi/Simulation-control-measures-of-COVID-19/blob/master/image/image4.png)

![Image text](https://github.com/TMITMiTmitmi/Simulation-control-measures-of-COVID-19/blob/master/image/image5.png)

### 防疫建议
最理想的情况是学校对学生进行核酸检测，将检测呈阳性的学生隔离，同时规定学生戴口罩以及保持社交距离，这样可以在学校出
现感染者时将学校内疫情控制住。  
若没有条件进行核酸检测，则规定学生每天测量体温，上报身体情况，将有症状的学生进行隔离，同时规定学生戴口罩以及保持社
交距离，在仿真中采取以上措施大大延长了全校同学被感染的时间，由于仿真只是小范围模拟存在局限性，因此采取以下措施也会有
效果。
同时，即使本程序由于仿真规模较小忽略了室内残留病毒的影响，可以预见的在诸如学校的大规模场所这份影响是不可忽略的，因此
学校需要定期对校内环境进行消毒杀菌，在非人传人途径抑制病毒传播。

### 备注
1.仿真程序逻辑代码位置：Assets/Scripts/NavigationAgent.cs，需在代码中修改变量改变防疫措施  
2.仿真程序可执行文件位置：Simulation.rar/COVID-19.exe  

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
