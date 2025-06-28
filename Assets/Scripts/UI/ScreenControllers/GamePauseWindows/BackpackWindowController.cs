using System.Collections;
using System.Collections.Generic;
using KidGame.UI;
using UnityEngine;

public class BackpackWindowController : WindowController
{
    private UICircularScrollView scrollView;
    protected override void Awake()
    {
        base.Awake();
        
    }

    protected override void OnPropertiesSet() {
        scrollView = gameObject.GetComponentInChildren<UICircularScrollView>();
        scrollView.Init(100,null,null);
    }
}
