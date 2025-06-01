using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class InfoPanel : MonoBehaviour
{
    [Header("按钮组")]
    public Button[] Add;
    public Button[] Sub;
    [Header("数值文本")]
    public Text sceneName;
    public Text level;
    public Text gameTime;
    public Text isFullScreen;
    public Text difficulty;
    public Image colorImg;
    [Header("材质颜色预设")]
    [ColorUsage(true)]
    public Color[] colorPreset;
    public Material m;

    int difficultyID;
    int colorID=3;


    private void Awake()
    {
        Add[0].onClick.AddListener(() => ChangeScene(1));
        Sub[0].onClick.AddListener(() => ChangeScene(-1));
        Add[1].onClick.AddListener(() => Player.Instance.level++);
        Sub[1].onClick.AddListener(() => Player.Instance.level--);
        Add[2].onClick.AddListener(() => Player.Instance.isFullScreen = true);
        Sub[2].onClick.AddListener(() => Player.Instance.isFullScreen = false);
        Add[3].onClick.AddListener(() => { difficultyID++;
                                           Player.Instance.difficulty = (Player.Difficulty)difficultyID; });
        Sub[3].onClick.AddListener(() => { difficultyID--;
                                           Player.Instance.difficulty = (Player.Difficulty)difficultyID; });
        Add[4].onClick.AddListener(() => { colorID++; Player.Instance.color = colorPreset[colorID]; });
        Sub[4].onClick.AddListener(() => { colorID--; Player.Instance.color = colorPreset[colorID]; });
    
    }
   
    //这些不要写Awake里，因为那时玩家数据还没更新，会报错
    private void Start()
    {
        //获取当前场景并设置场景名
        Scene curScene = SceneManager.GetActiveScene();
        Player.Instance.scensName = curScene.name;
        sceneName.text = Player.Instance.scensName;
       
        //获取初始难度的序号
        difficultyID = (int)Player.Instance.difficulty;
    }

    private void LateUpdate()
    {
        //实时更新UI      
        level.text = Player.Instance.level.ToString();
        isFullScreen.text = Player.Instance.isFullScreen.ToString();
        difficulty.text = Player.Instance.difficulty.ToString();
        colorImg.color = Player.Instance.color;
        m.SetColor("_BaseColor", Player.Instance.color);              
        gameTime.text = TimeMgr.GetFormatTime((int)TimeMgr.curT);
    }

    //由+/-按钮调用，场景一定会改变
    void ChangeScene(int d)
    {
        int i = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(i + d);      
    }

}