using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ParamController : MonoBehaviour
{

    public Text DayUI; 
    public Toggle[] CheckBox;
    public GameObject GymTarget;
    public GameObject HospitalTarget;
    public AudioSource audioSource;

    //初始化参数值
    public float TimeScale = 1.0f;
    public float ActivityRate = 100.0f;
    public float SpreadRate = 5.0f;
    public float IncubationSpreadRate = 2.0f;
    public float GymFrequency = 50.0f;
    public bool KeepDistance = false;//默认不保持社交距离
    public bool WearMask = false;//戴口罩
    public bool RedHospital = false;//确诊者医疗隔离
    public bool OrangeHospital = false;//出现症状者医疗隔离
    public bool YellowHospital = false;//传染源医疗隔离
    public int StandardTime = 5; //到下一个目的地的时间间隔

    //隐藏教室、食堂列表以及天数
    [HideInInspector]
    public IAmClassroom[] ClassList;
    [HideInInspector]
    public IAmCafeteria[] CafeteriaList;
    [HideInInspector]
    public string text;

    public Slider[] Sliders;//滑块
    public Text[] InfectedNum;//人数
    private MouseController mouseController;
    List<GameObject> stuList = new List<GameObject>();
    List<Collider> StuCollider = new List<Collider>();

    private NavigationAgent[] navTargets;
    private bool isPlaying;//开始暂停键判断是不是应该播放

    void InitPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("SpreadRate"))
        {
            SpreadRate = PlayerPrefs.GetFloat("SpreadRate");
        }
        if (PlayerPrefs.HasKey("IncubationSpreadRate"))
        {
            IncubationSpreadRate = PlayerPrefs.GetFloat("IncubationSpreadRate");
        }
        if (PlayerPrefs.HasKey("GymFrequency"))
        {
            GymFrequency = PlayerPrefs.GetFloat("GymFrequency");
        }
        if (PlayerPrefs.HasKey("TimeScale"))
        {
            TimeScale = PlayerPrefs.GetFloat("TimeScale");
        }
        if (PlayerPrefs.HasKey("ActivityRate"))
        {
            ActivityRate = PlayerPrefs.GetFloat("ActivityRate");
        }
        if (PlayerPrefs.HasKey("KeepDistance"))
        {
            KeepDistance = PlayerPrefs.GetInt("KeepDistance") == 0 ? false : true;
        }
        if (PlayerPrefs.HasKey("WearMask"))
        {
            WearMask = PlayerPrefs.GetInt("WearMask") == 0 ? false : true;
        }
        if (PlayerPrefs.HasKey("RedHospital"))
        {
            RedHospital = PlayerPrefs.GetInt("RedHospital") == 0 ? false : true;
        }
        if (PlayerPrefs.HasKey("OrangeHospital"))
        {
            OrangeHospital = PlayerPrefs.GetInt("OrangeHospital") == 0 ? false : true;
        }
        if (PlayerPrefs.HasKey("YellowHospital"))
        {
            YellowHospital = PlayerPrefs.GetInt("YellowHospital") == 0 ? false : true;
        }
    }

    private void OnEnable()
    {
        //恢复上次保存的参数
        InitPlayerPrefs();
        ClassList = FindObjectsOfType<IAmClassroom>();
        CafeteriaList = FindObjectsOfType<IAmCafeteria>();
        navTargets = FindObjectsOfType<NavigationAgent>();
        mouseController = FindObjectOfType<MouseController>();
        foreach (var item in navTargets)
        {
            stuList.Add(item.gameObject);
            StuCollider.Add(item.gameObject.GetComponent<Collider>());
        }
        audioSource = GetComponent<AudioSource>();
    }
    /// <summary>
    /// Upate UI in every 0.5s
    /// </summary>
    /// <returns></returns>
    IEnumerator SometimesUpdateInfectedNum()//更新人数
    {
        int[] infectArray = new int[4] { 0, 0, 0, 0 };
        foreach (var item in navTargets)
        {
            infectArray[(int)item.state] = infectArray[(int)item.state] + 1;
        }

        for (int i = 0; i < InfectedNum.Length; i++)
        {
            InfectedNum[i].text = infectArray[i].ToString();
        }

        yield return new WaitForSeconds(0.5f);//每0.5s刷新一次

    }

    private void Start()
    {
        CheckBox[0].onValueChanged.AddListener((bool value) => OnCheckBoxValueChange(value, CheckBox[0]));
        CheckBox[1].onValueChanged.AddListener((bool value) => OnCheckBoxValueChange1(value, CheckBox[1]));
        CheckBox[2].onValueChanged.AddListener((bool value) => OnCheckBoxValueChange2(value, CheckBox[2]));
        CheckBox[3].onValueChanged.AddListener((bool value) => OnCheckBoxValueChange3(value, CheckBox[3]));
        CheckBox[4].onValueChanged.AddListener((bool value) => OnCheckBoxValueChange4(value, CheckBox[4]));

        foreach (Slider item in Sliders)
        {
            item.onValueChanged.AddListener((float value) => OnSliderValueChange(value, item));
        }

        FlushSlidersCheckBox();

        setActivate(isPlaying);

    }

    private void OnCheckBoxValueChange(bool value, Toggle checkBox)
    {
        KeepDistance = value;
    }

    private void OnCheckBoxValueChange1(bool value, Toggle checkBox)
    {
        WearMask = value;
    }

    private void OnCheckBoxValueChange2(bool value, Toggle checkBox)
    {
        RedHospital = value;
    }

    private void OnCheckBoxValueChange3(bool value, Toggle checkBox)
    {
        OrangeHospital = value;
    }

    private void OnCheckBoxValueChange4(bool value, Toggle checkBox)
    {
        YellowHospital = value;
    }

    private void OnSliderValueChange(float value, Slider EventSender)
    {
        Text text = EventSender.gameObject.GetComponentInChildren<Text>();
        text.text = Math.Round(value, 2).ToString() + "%";
        mouseController.enabled = false;
        switch (EventSender.name)
        {
            case "TimeScaleSlider":
                TimeScale = value;
                text.text = "x" + Math.Round(value, 2).ToString();
                break;
            case "InfSlider":
                SpreadRate = value;
                break;
            case "PreInfSlider":
                IncubationSpreadRate = value;
                break;
            case "GymSlider":
                GymFrequency = value;
                break;
            case "ActivateSlider":
                ActivityRate = value;
                break;

        }
    }

    void Update()
    {
        if (isPlaying)
        {
            Time.timeScale = TimeScale;
        }
        else
        {
            Time.timeScale = 0;
        }

        DayUI.text = "天数 " + text;
        if (mouseController.enabled == false && Input.GetMouseButtonUp(0))
        {
            mouseController.enabled = true;
        }
        StartCoroutine(SometimesUpdateInfectedNum());

    }

    void setActivate(bool condition)
    {
        foreach (var item in navTargets)
        {
            item.enabled = condition;
        }
        foreach (var item in StuCollider)
        {
            item.enabled = condition;
        }

    }

    void FlushSlidersCheckBox()
    {
        CheckBox[0].isOn = KeepDistance;
        CheckBox[1].isOn = WearMask;
        CheckBox[2].isOn = RedHospital;
        CheckBox[3].isOn = OrangeHospital;
        CheckBox[4].isOn = YellowHospital;
        foreach (Slider item in Sliders)
        {
            switch (item.name)
            {
                case "TimeScaleSlider":
                    item.value = TimeScale;

                    break;
                case "InfSlider":
                    item.value = SpreadRate;
                    break;
                case "PreInfSlider":
                    item.value = IncubationSpreadRate;
                    break;
                case "GymSlider":
                    item.value = GymFrequency;
                    break;
                case "ActivateSlider":
                    item.value = ActivityRate;
                    break;
            }

        }
    }

    /// <summary>
    /// When click Reset Button, reset all the params.
    /// </summary>
    public void ResetParams()
    {
        SpreadRate = 5.0f;
        IncubationSpreadRate = 2.0f;
        GymFrequency = 50.0f;
        ActivityRate = 100.0f;
        TimeScale = 1.0f;
        KeepDistance = false;
        WearMask = false;
        RedHospital = false;
        OrangeHospital = false;
        YellowHospital = false;
        FlushSlidersCheckBox();
    }
    /// <summary>
    /// When Click Start button, enable all the agents and disable all the sliders 
    /// and checkbox, except TimeScale.
    /// </summary>
    public void StartPlay()
    {
        isPlaying = true;
        setActivate(isPlaying);
        foreach (Slider item in Sliders)
        {
            if (item.name != "TimeScaleSlider")
            {
                item.interactable = false;
            }

        }
        foreach (Toggle item in CheckBox)
        {
            item.interactable = false;
        }
        audioSource.Play();
    }
    public void StopPlaying()
    {
        audioSource.Stop();
        isPlaying = false;
        setActivate(isPlaying);
        foreach (Slider item in Sliders)
        {
            item.interactable = true;
        }
        foreach (Toggle item in CheckBox)
        {
            item.interactable = true;
        }

        PlayerPrefs.SetFloat("SpreadRate", SpreadRate);
        PlayerPrefs.SetFloat("IncubationSpreadRate", IncubationSpreadRate);
        PlayerPrefs.SetFloat("GymFrequency", GymFrequency);
        PlayerPrefs.SetFloat("TimeScale", TimeScale);
        PlayerPrefs.SetFloat("ActivityRate", ActivityRate);
        PlayerPrefs.SetFloat("KeepDistance", KeepDistance == false ? 0 : 1);
        PlayerPrefs.SetFloat("WearMask", WearMask == false ? 0 : 1);
        PlayerPrefs.SetFloat("RedHospital", RedHospital == false ? 0 : 1);
        PlayerPrefs.SetFloat("OrangeHospital", OrangeHospital == false ? 0 : 1);
        PlayerPrefs.SetFloat("YellowHospital", YellowHospital == false ? 0 : 1);

        string SceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(SceneName);

    }
}
