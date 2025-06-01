using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;               //TimeSpan


public static class TimeMgr
{
    public static float oriT;          //基准时间
    public static float curT;          //当前时间    

    public static void SetOriTime()
    {
        float tempT = Time.realtimeSinceStartup;
        oriT = Player.Instance.gameTime - tempT;
        SetCurTime();
    }

    public static void SetCurTime()
    {
        //当前时间不应该小于0
        curT = Mathf.Max(TimeMgr.oriT + Time.realtimeSinceStartup, 0);
        Player.Instance.gameTime = curT;      
    }


    //将秒数转化为00:00:00
    public static string GetFormatTime(int seconds)
    {
        //把秒数转为时分秒
        TimeSpan ts = new TimeSpan(0, 0, seconds);
        return $"{ts.Hours.ToString("00")}:{ts.Minutes.ToString("00")}:{ts.Seconds.ToString("00")}";
    }

    //将8位日期转为YYYY/MM/DD
    public static void SetDate(ref string date)
    {
        date = date.Insert(4, "/");
        date = date.Insert(7, "/");
    }

    //将8位时间转为HH:MM:SS
    public static void SetTime(ref string time)
    {
        time = time.Insert(2, ":");
        time = time.Insert(5, ":");
    }


}
