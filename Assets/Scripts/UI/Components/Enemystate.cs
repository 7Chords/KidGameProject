using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.UI;
using UnityEngine;
using UnityEngine.UI;

public class Enemystate : MonoBehaviour
{
    // todo.精神值slider
    public ProgressBar sanSlider;
    
    public EnemyController enemyController;
    public Transform buffUIParent;
    public GameObject buffUIPrefab;

    private List<GameObject> currentBuffUIs = new List<GameObject>();

    private void Update()
    {
        UpdateBuffUI();
    }

    private void UpdateBuffUI()
    {
        // 清空旧UI
        foreach (var ui in currentBuffUIs)
        {
            Destroy(ui);
        }
        currentBuffUIs.Clear();

        var buffList = enemyController.GetBuffList();

        foreach (var buff in buffList)
        {
            GameObject buffUI = Instantiate(buffUIPrefab, buffUIParent);
            
            // todo.在这里设置icon图片，不过目前buff还没有图片值
            currentBuffUIs.Add(buffUI);
        }
    }
}
