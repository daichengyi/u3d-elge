using System.Collections.Generic;
using UnityEngine;

/** 关卡表*/
internal class ConfigLevel
{
    public int level;
    /** 显示比例*/
    public int lvProp;
    /** 上升提示*/
    public int upTips;
    /** 机制提示*/
    public int mechanism;
    /** 无操作提示*/
    public int tips;
}

/** 公共配置*/
internal class ConfigPublic
{
    /** 关卡循环*/
    public int levelLoop;
    /** 连击时间/s*/
    public int comboTime;
    /** 无操作提示时间/s*/
    public int noOps;
}

/** 特殊类型表*/
internal class ConfigMechanism
{
    public int id;
    /** 类型
    0: 普通羊
    1: 爆炸羊，触碰|任意操作/次数
    2: 时间羊，倒计时/秒
    */
    public int type;
    /** 值*/
    public int value;
}

/** 关卡数据*/
[System.Serializable]
public class LevelData
{
    public string name;
    public Vector3 position;
    public Vector3 eulerAngles;
    public Vector3 localScale;
    public List<MovableData> movables = new List<MovableData>();
}

[System.Serializable]
public class MovableData
{
    public string index;

    public int type;
    public Vector3 position;
    public Vector3 eulerAngles;
    public Vector3 localScale;
}