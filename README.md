# Simulation-control-measures-of-COVID-19

### 仿真场景
仿真程序以电子科技大学清水河校区宿舍区、学习区（教室、图书馆）、食堂、体育馆以及用作隔离病人场所的校医院为背景对
新冠病毒在校园中的传播进行模拟，并且设计了五种可选防疫措施分别验证防疫效果：
	-保持社交距离
	-戴口罩
	-对确诊者进行医疗隔离
	-对出现症状者进行隔离
	-对核酸检测呈阳性者进行隔离
通过对这些防疫措施进行排列组合来验证防疫措施对疫情防控所起的作用大小，并最终给出防疫建议。

### 程序介绍
1.本程序假设校园中可能存在以下四种学生：
	-易感者
	-无症状被感染者
	-有症状被感染者
	-确诊者
        由于本程序研究新冠病毒的传播，因此不考虑新冠病毒患者的治愈以及死亡情况。

2.学生在校园内每天的活动轨迹如下：
	宿舍->教室->食堂->宿舍->教室->(体育馆->)宿舍
	其中每个学生每天目的地的教室和食堂是在三个教室和三个食堂内随机生成，并且每个学生每天有20%的几率在下课后去
	往体育馆进行锻炼。

3.感染病毒的过程为：
	易感者->无症状感染者->有症状感染者->确诊者
    	-易感者->无症状感染者
	    易感者在与传染源接触后根据距离有概率被感染新冠病毒。
    	-无症状感染者->有症状感染者
	    无症状感染者在潜伏期后会出现新冠病毒的症状。
    	-有症状感染者->确诊者
	    有症状感染者在出现感染的之后一天确诊感染新冠病毒。

### 程序假设
1.根据文献[1]对于新冠病毒潜伏期的研究，假设新冠病毒潜伏期为2~14天。

2.根据文献[2]对于一个22岁中国学生对其密切接触者的感染案例的研究，假设大学生在密切接触情况下感染率为50%；根据《泰晤
士报》发表的数据假设易感者距离感染源1m被感染的概率为2.6%；假设感染函数为下凸递减函数，并且易感者在距离感染源2m之外
不会被感染，用$y = a * e^-(b*x) + c$拟合感染率函数为$y = 0.063 * e^-(\frac{ln(2)}{2})x - 0.5$。

3.根据文献[2]对于22岁中国学生密切接触者感染案例分析，假设易感者在感染病毒之后的一天才会成为感染源以及被核酸检测呈阳
性。
 
4.根据文献[3]对于口罩对新冠病毒感染率影响的研究，假设在大学内戴口罩将会使感染率下降到原先的十分之一。

5.仿真程序假设人与人之间需要保持的社交距离为1.2m。

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
