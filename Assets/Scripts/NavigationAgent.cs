using UnityEngine;
using UnityEngine.AI;


public enum Destination
{
    ClassroomA,
    Cafeteria,
    DormA,
    ClassroomB,
    Gym,
    DormB
}
public enum State //学生状态
{
    Healthy,//易感
    InfectedWithSymptoms,//确诊
    InfectedNonSymptoms,//进入潜伏期
    InfectedContagious,//出现症状
}
public class NavigationAgent : MonoBehaviour
{
    public bool PatientZero; //是否是最初的感染者
    public State state;//本人状态

    //位置标志
    private GameObject gymTarget;
    private GameObject hospitalTarget;
    private GameObject classTarget;
    private GameObject cafeteriaTarget;
    private GameObject[] students;

    private ParamController paramController;
    private IAmClassroom[] classList;//教室队列
    private IAmCafeteria[] cafeteriaList;//食堂队列

    public Destination destination;//目的地

    //参数
    private float spreadRate;//传染率
    private float incubationSpreadRate;//潜伏期传染率
    private float gymFrequency;
    private float activityRate;

    //设置防疫措施
    private bool keepDistance = false;//保持社交距离
    private bool wearMask = false;//戴口罩
    private bool redHospital = false;//确诊者医疗隔离
    private bool orangeHospital = false;//出现症状者医疗隔离
    private bool yellowHospital = false;//传染源医疗隔离

    private NavMeshAgent studentAgent;
    private Vector3 startPoint;//记住起始点便于每天回宿舍

    private int cover = 999;//潜伏期
    private int infecDay = 999;//被感染的日期

    private int standardTime;//在去往下个目的地之间的时间
    private int count;

    private int day = 0;//第几天

    private bool goOutToday;//今天是否出宿舍  
    private bool goBack;//回宿舍而不是去体育馆

    Color orange = new Color(255f / 255f, 112f / 255f, 0f / 255f);//橙色

    void Start()
    {
        //找到参数控制器对参数进行控制
        paramController = FindObjectOfType<ParamController>();

        //从参数控制器获取参数（各种比率以及目标位置）
        spreadRate = paramController.SpreadRate;
        incubationSpreadRate = paramController.IncubationSpreadRate;
        gymFrequency = paramController.GymFrequency;
        activityRate = paramController.ActivityRate;
        /*
        keepDistance = paramController.KeepDistance;
        wearMask = paramController.WearMask;
        redHospital = paramController.RedHospital;
        orangeHospital = paramController.OrangeHospital;
        yellowHospital = paramController.YellowHospital;
        */
        hospitalTarget = paramController.HospitalTarget;
        gymTarget = paramController.GymTarget;
        classList = paramController.ClassList;
        cafeteriaList = paramController.CafeteriaList;
        standardTime = paramController.StandardTime * 60;
        studentAgent = GetComponent<NavMeshAgent>();
        startPoint = this.gameObject.transform.position;//记住出发点便于返回

        count = standardTime;//计数器记录标准时间，准备下一秒就去下一个地方
        studentAgent.destination = startPoint;//先将学生动向归零即起始点
        if (PatientZero)
        {
            infecDay = 1;//设定当前日期为感染日期
            cover = Random.Range(3, 14);//Got incubation period.随机潜伏期日期
            state = State.InfectedNonSymptoms;//变为无症状感染者
        }
    }

    void Update()
    {
        students = GameObject.FindGameObjectsWithTag("stu");//找到除了自己以外的其他学生的集合
        if (keepDistance)
        {
            LimitDistance();
        }
        ProbabilisticInfection();
    }


    void FixedUpdate()
    {
        if (cover + infecDay - 1 <= day)//从出现症状到确诊有一天时间
            state = State.InfectedContagious;

        if ((cover + infecDay) <= day)//被感染日期加上潜伏期的日子被确诊
            state = State.InfectedWithSymptoms;


        if (state == State.InfectedWithSymptoms)//确诊变红
            this.gameObject.GetComponent<Renderer>().material.color = Color.red;
        else if (state == State.InfectedNonSymptoms && day > infecDay + 1)//有传染性的无症状感染者变黄
            this.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
        else if (state == State.InfectedContagious)//有症状感染者变橙
            this.gameObject.GetComponent<Renderer>().material.color = orange;

        count++;

        if (count > standardTime)//当计数器超过标准时间需要移动到下一个目的地
        {
            //从UI更新参数
            /*
            spreadRate = paramController.SpreadRate;
            incubationSpreadRate = paramController.IncubationSpreadRate;
            gymFrequency = paramController.GymFrequency;
            activityRate = paramController.ActivityRate;
            keepDistance = paramController.KeepDistance;
            */

            count = 0;//移动到新位置之后计数器置零
            if (destination == Destination.ClassroomA)
            {
                day++;
                int i = Random.Range(0, 100);
                if (i <= activityRate)//出门几率为1~100，用随机数判断今天是否出门
                {
                    goOutToday = true;
                }
                else
                {
                    goOutToday = false;
                }
                Debug.Log("In day " + day.ToString());//打印日期
                paramController.text = day.ToString();
                studentAgent.enabled = true;
                destination = Destination.Cafeteria;//下一个目的地设在食堂
                int index = Random.Range(0, classList.Length); //随机选择教室
                classTarget = classList[index].gameObject; //去往随机选择的教室

                if (goOutToday)
                {
                    studentAgent.destination = classTarget.transform.GetChild(0).position;//如果今天出门就去选择的教室
                }
                else
                {
                    studentAgent.destination = startPoint;//如果不出门就呆在宿舍
                }
            }
            else if (destination == Destination.Cafeteria)
            {
                studentAgent.enabled = true;
                destination = Destination.DormA;//设定去食堂之后回宿舍
                int index = Random.Range(0, cafeteriaList.Length);
                cafeteriaTarget = cafeteriaList[index].gameObject;//随机选择一个食堂
                if (goOutToday)
                {
                    studentAgent.destination = cafeteriaTarget.transform.GetChild(0).position;
                }
                //如果不出门就不用变位置了
            }
            else if (destination == Destination.DormA)
            {
                studentAgent.enabled = true;
                destination = Destination.ClassroomB;//设定回宿舍之后去下一个教室（上下午的课）
                if (goOutToday)
                {
                    studentAgent.destination = startPoint;
                }

            }
            else if (destination == Destination.ClassroomB)
            {
                studentAgent.enabled = true;
                int i = Random.Range(0, 100);
                if (i < 20) //设定下午课程下课后目的地，有一定几率去体育馆或者回宿舍
                {
                    destination = Destination.Gym;
                }
                else
                {
                    destination = Destination.DormB;
                }
                int index = Random.Range(0, classList.Length);
                classTarget = classList[index].gameObject;
                if (goOutToday)
                    studentAgent.destination = classTarget.transform.GetChild(0).position;
            }
            else if (destination == Destination.Gym)
            {
                studentAgent.enabled = true;
                goBack = true; //去体育馆之后必回宿舍
                destination = Destination.DormB;
                if (goOutToday)
                {
                    studentAgent.destination = gymTarget.transform.position;
                }

                Debug.Log("gym");
            }
            else if (destination == Destination.DormB)
            {

                studentAgent.enabled = true;
                if (goBack)
                {
                    destination = Destination.ClassroomA;//设定下一天第一个目的地是教室
                    if (goOutToday)
                    {
                        studentAgent.destination = startPoint;
                    }

                    goBack = false;
                }
                else
                {
                    destination = Destination.DormB;
                    if (goOutToday)
                    {
                        studentAgent.destination = startPoint;
                    }
                    goBack = true;//回宿舍的学生继续判断为回宿舍
                }
            }


        }

        if (redHospital && this.gameObject.GetComponent<Renderer>().material.color == Color.red)
        {
            studentAgent.destination = hospitalTarget.transform.position;
        }
        if (orangeHospital && this.gameObject.GetComponent<Renderer>().material.color == orange)
        {
            studentAgent.destination = hospitalTarget.transform.position;
        }
        if (yellowHospital && this.gameObject.GetComponent<Renderer>().material.color == Color.yellow)
        {
            studentAgent.destination = hospitalTarget.transform.position;
        }
    }

    double InfectionFunction(double x)
    {
        return x < 1 ? (0.063f * (System.Math.Exp(-1 * System.Math.Log(2) * x) - 0.5)) : 0;
    }

    void ProbabilisticInfection()
    {
        double dis;
        foreach (var ball in students)
        {
            if (ball != this.gameObject)
            {
                dis = Vector3.Distance(this.transform.position, ball.transform.position);
                double i = Random.Range(1, 100000);
                if (wearMask)
                    i *= 10;
                if (dis <= 1 && i <= InfectionFunction(dis) * 100000 && (ball.GetComponent<Renderer>().material.color == orange || ball.GetComponent<Renderer>().material.color == Color.red || ball.GetComponent<Renderer>().material.color == Color.yellow))
                {
                    this.SendMessage("GotInfected");
                    Debug.Log("Distance:" + dis.ToString() + " Inf:" + InfectionFunction(dis).ToString() + " i:" + i.ToString());
                }
            }
        }
    }

    void GotInfected()
    {
        if (state == State.Healthy)
        {
            infecDay = day;//设定当前日期为感染日期
            cover = Random.Range(2, 14);//随机潜伏期日期
            state = State.InfectedNonSymptoms;//进入潜伏期
        }
    }

    void LimitDistance()//限制学生之间距离
    {
        foreach (var ball in students)
        {
            if (ball != this.gameObject)
            {
                if (Vector3.Distance(this.transform.position, ball.transform.position) < 0.6f)
                {
                    Vector3 pos = (this.transform.position - ball.transform.position).normalized;//获得两个物体之间的单位向量
                    pos *= 0.6f;//单位向量乘以距离系数
                    this.transform.position = pos + ball.transform.position;//加上距离向量
                }
            }
        }
    }
}
