using System.Collections;
using System.Collections.Generic;
using KidGame.UI;
using UnityEngine;

public class MakeWindowController : WindowController
{
    private UICircularScrollView firstSelection;
    private UICircularScrollView secondSelection;
    
    protected override void OnPropertiesSet()
    {
        //todo 从单例获取配置so，有了就不获取了
        //根据so初始化列表和其中按钮即可
        //scrollView.Init(_materialSlotInfos.Count + _trapSlotInfos.Count,OnCellUpdate,null);
    }
    
    
    
}
