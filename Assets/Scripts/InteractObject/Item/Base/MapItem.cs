using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : MonoBehaviour,IPickable
{
    public string itemName;
    public void Deal()
    {
        Debug.Log($"ºÒµΩ¡À{itemName}£°");
    }
}
