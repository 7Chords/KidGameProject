using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.UI;
using UnityEngine;
using UnityEngine.UI;

public class Enemystate : MonoBehaviour
{
    // todo.����ֵslider
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
        // ��վ�UI
        foreach (var ui in currentBuffUIs)
        {
            Destroy(ui);
        }
        currentBuffUIs.Clear();

        var buffList = enemyController.GetBuffList();

        foreach (var buff in buffList)
        {
            GameObject buffUI = Instantiate(buffUIPrefab, buffUIParent);
            
            // todo.����������iconͼƬ������Ŀǰbuff��û��ͼƬֵ
            currentBuffUIs.Add(buffUI);
        }
    }
}
