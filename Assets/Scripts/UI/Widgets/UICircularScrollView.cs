//*****************************-》 基类 循环列表 《-****************************
//author kim
//初始化:
//      Init(callBackFunc)
//刷新整个列表（首次调用和数量变化时调用）:
//      ShowList(int = 数量)
//刷新单个项:
//      UpdateCell(int = 索引)
//刷新列表数据(无数量变化时调用):
//      UpdateList()
//回调:
//Func(GameObject = Cell, int = Index)  //刷新列表

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UIUtils
{
    public static void SetActive(GameObject obj, bool isActive)
    {
        if (obj != null)
        {
            obj.SetActive(isActive);
        }
    }
}

public enum e_Direction
{
    Horizontal,
    Vertical
}


public class UICircularScrollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public e_Direction m_Direction = e_Direction.Horizontal;

    /// <summary>
    /// 是否需要指示箭头
    /// </summary>
    public bool m_IsShowArrow = true;

    /// <summary>
    /// 每行或每列显示数量
    /// </summary>
    public int m_Row = 1;

    /// <summary>
    /// 间距
    /// </summary>
    public float m_Spacing = 0f; //间距

    /// <summary>
    /// 需要动态创建的Item模板
    /// </summary>
    public GameObject m_CellGameObject; //指定的cell

    protected Action<GameObject, int> m_FuncCallBackFunc;
    protected Action<GameObject, int> m_FuncOnClickCallBack;
    protected Action<int, bool, GameObject> m_FuncOnButtonClickCallBack;
    protected Action<bool, string> m_FunCallDragEnd;
    protected Action<GameObject> m_FuncRecycleCallback;
    protected Action m_FunDownArrowHide;

    protected RectTransform rectTrans;

    protected float m_PlaneWidth;
    protected float m_PlaneHeight;

    protected float m_ContentWidth;
    protected float m_ContentHeight;

    protected float m_CellObjectWidth;
    protected float m_CellObjectHeight;

    protected GameObject m_Content;
    protected RectTransform m_ContentRectTrans;

    private bool m_IsInitedObj = false;

    /// <summary>
    /// 开始箭头
    /// </summary>
    public GameObject m_PointingFirstArrow;

    /// <summary>
    /// 结束箭头
    /// </summary>
    public GameObject m_PointingEndArrow;

    //记录 物体的坐标 和 物体 
    public struct CellInfo
    {
        public Vector3 pos;
        public GameObject obj;
    };

    public CellInfo[] m_CellInfos;

    public bool m_IsInited = false;

    protected ScrollRect m_ScrollRect;

    protected int m_MaxCount = -1; //列表数量

    protected int m_MinIndex = -1;
    protected int m_MaxIndex = -1;

    protected bool isEnd = false;

    protected bool m_IsClearList = false; //是否清空列表

    public virtual void Init(int count, Action<GameObject, int> callBack, Action<GameObject> onRecycle = null)
    {
        Init(null, count, callBack, null, onRecycle);

        ShowList(count);
    }

    public virtual void Init(Action<bool, string> dragBack, int count, Action<GameObject, int> callBack, Action<GameObject> onRecycle = null)
    {
        Init(dragBack, count, callBack, null, onRecycle);

        ShowList(count);
    }

    public virtual void Init(int count, Action<GameObject, int> callBack, Action<GameObject, int> onClickCallBack,
        Action<int, bool, GameObject> onButtonClickCallBack, Action<GameObject> onRecycle = null)
    {
        if (onButtonClickCallBack != null)
        {
            m_FuncOnButtonClickCallBack = onButtonClickCallBack;
        }

        Init(null, count, callBack, onClickCallBack, onRecycle);
        ShowList(count);
    }
    

    public virtual void Init(Action<bool, string> dragBack, int count, Action<GameObject, int> callBack,
        Action<GameObject, int> onClickCallBack, Action<GameObject> onRecycle = null)
    {
        DisposeAll();

        m_FuncCallBackFunc = callBack;

        if (onClickCallBack != null)
        {
            m_FuncOnClickCallBack = onClickCallBack;
        }

        if (dragBack != null)
        {
            m_FunCallDragEnd = dragBack;
        }

        m_FuncRecycleCallback = onRecycle;

        if (m_IsInitedObj)
            return;


        m_Content = this.GetComponent<ScrollRect>().content.gameObject;

        if (m_CellGameObject == null)
        {
            m_CellGameObject = m_Content.transform.GetChild(0).gameObject;
        }
        m_CellGameObject.gameObject.SetActive(false);

        /* Cell 处理 */
        //m_CellGameObject.transform.SetParent(m_Content.transform.parent, false);
        //SetPoolsObj(m_CellGameObject);

        foreach (Transform child in m_Content.transform)
        {
            if (child.gameObject == m_CellGameObject)
                continue;

            SetPoolsObj(child.gameObject);
        }

        RectTransform cellRectTrans = m_CellGameObject.GetComponent<RectTransform>();
        cellRectTrans.pivot = new Vector2(0f, 1f);
        CheckAnchor(cellRectTrans);
        cellRectTrans.anchoredPosition = Vector2.zero;
        //记录 Cell 信息
        m_CellObjectHeight = cellRectTrans.rect.height;
        m_CellObjectWidth = cellRectTrans.rect.width;

        //记录 Plane 信息
        rectTrans = GetComponent<RectTransform>();
        Rect planeRect = rectTrans.rect;
        m_PlaneHeight = planeRect.height;
        m_PlaneWidth = planeRect.width;

        //记录 Content 信息
        m_ContentRectTrans = m_Content.GetComponent<RectTransform>();
        Rect contentRect = m_ContentRectTrans.rect;
        m_ContentHeight = contentRect.height;
        m_ContentWidth = contentRect.width;

        m_ContentRectTrans.pivot = new Vector2(0f, 1f);
        //m_ContentRectTrans.sizeDelta = new Vector2 (planeRect.width, planeRect.height);
        //m_ContentRectTrans.anchoredPosition = Vector2.zero;
        CheckAnchor(m_ContentRectTrans);
        RefreshList(m_Content, count);
        m_ScrollRect = this.GetComponent<ScrollRect>();

        m_ScrollRect.onValueChanged.RemoveAllListeners();
        //添加滑动事件
        m_ScrollRect.onValueChanged.AddListener(delegate (Vector2 value) { ScrollRectListener(value); });

        if (m_PointingFirstArrow != null || m_PointingEndArrow != null)
        {
            m_ScrollRect.onValueChanged.AddListener(delegate (Vector2 value) { OnDragListener(value); });
            OnDragListener(Vector2.zero);
        }

        m_IsInitedObj = true;
    }

    public void RegisterOnArrowHide(Action func)
    {
        m_FunDownArrowHide = func;
    }
    /// <summary>
    /// 刷新列表显示
    /// </summary>
    /// <param name="go"></param>
    /// <param name="num"></param>
    public void RefreshList(GameObject go, int num)
    {
        if (go != null)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                if (i > num - 1)
                {
                    go.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }

    //检查 Anchor 是否正确
    private void CheckAnchor(RectTransform rectTrans)
    {
        if (m_Direction == e_Direction.Vertical)
        {
            if (!((rectTrans.anchorMin == new Vector2(0, 1) && rectTrans.anchorMax == new Vector2(0, 1)) ||
                  (rectTrans.anchorMin == new Vector2(0, 1) && rectTrans.anchorMax == new Vector2(1, 1))))
            {
                rectTrans.anchorMin = new Vector2(0, 1);
                rectTrans.anchorMax = new Vector2(1, 1);
            }
        }
        else
        {
            if (!((rectTrans.anchorMin == new Vector2(0, 1) && rectTrans.anchorMax == new Vector2(0, 1)) ||
                  (rectTrans.anchorMin == new Vector2(0, 0) && rectTrans.anchorMax == new Vector2(0, 1))))
            {
                rectTrans.anchorMin = new Vector2(0, 0);
                rectTrans.anchorMax = new Vector2(0, 1);
            }
        }
    }

    //实时刷新列表时用
    public virtual void UpdateList()
    {
        if (m_CellInfos == null) return;
        for (int i = 0, length = m_CellInfos.Length; i < length; i++)
        {
            CellInfo cellInfo = m_CellInfos[i];
            if (cellInfo.obj != null)
            {
                float rangePos = m_Direction == e_Direction.Vertical ? cellInfo.pos.y : cellInfo.pos.x;
                if (!IsOutRange(rangePos))
                {
                    Func(m_FuncCallBackFunc, cellInfo.obj, true);
                }
            }
        }
    }

    /// <summary>
    /// 仅更新最大数量
    /// </summary>
    /// <param name="num"></param>
    public void UpdateMaxCount(int num)
    {
        float oldPos = 0;
        if (m_Direction == e_Direction.Vertical)
        {
            oldPos = m_ContentRectTrans.anchoredPosition.y;
        }
        else
        {
            oldPos = m_ContentRectTrans.anchoredPosition.x;
        }

        ShowList(num);

        if (m_Direction == e_Direction.Vertical)
        {
            oldPos = oldPos > m_ContentHeight ? m_ContentHeight : oldPos;
            m_ContentRectTrans.anchoredPosition = new Vector2(m_ContentRectTrans.anchoredPosition.x, oldPos);
        }
        else
        {
            oldPos = oldPos > m_ContentWidth ? m_ContentWidth : oldPos;
            m_ContentRectTrans.anchoredPosition = new Vector2(oldPos, m_ContentRectTrans.anchoredPosition.y);
        }
    }

    //刷新某一项
    public void UpdateCell(int index)
    {
        CellInfo cellInfo = m_CellInfos[index - 1];
        if (cellInfo.obj != null)
        {
            float rangePos = m_Direction == e_Direction.Vertical ? cellInfo.pos.y : cellInfo.pos.x;
            if (!IsOutRange(rangePos))
            {
                Func(m_FuncCallBackFunc, cellInfo.obj);
            }
        }
    }

    public virtual void ShowList(string numStr, bool isReset = true)
    {
    }

    public virtual void ShowList(int num, bool isReset = true)
    {
        m_MinIndex = -1;
        m_MaxIndex = -1;

        //-> 计算 Content 尺寸
        if (m_Direction == e_Direction.Vertical)
        {
            float contentSize = (m_Spacing + m_CellObjectHeight) * Mathf.CeilToInt((float)num / m_Row);
            m_ContentHeight = contentSize;
            m_ContentWidth = m_ContentRectTrans.sizeDelta.x;
            contentSize = contentSize < rectTrans.rect.height ? rectTrans.rect.height + 1 : contentSize;
            m_ContentRectTrans.sizeDelta = new Vector2(m_ContentWidth, contentSize);
            if (num != m_MaxCount && isReset)
            {
                m_ContentRectTrans.anchoredPosition = new Vector2(m_ContentRectTrans.anchoredPosition.x, 0);
            }
        }
        else
        {
            float contentSize = (m_Spacing + m_CellObjectWidth) * Mathf.CeilToInt((float)num / m_Row);
            m_ContentWidth = contentSize;
            m_ContentHeight = m_ContentRectTrans.sizeDelta.y;
            contentSize = contentSize < rectTrans.rect.width ? rectTrans.rect.width + 1 : contentSize;
            m_ContentRectTrans.sizeDelta = new Vector2(contentSize, m_ContentHeight);
            if (num != m_MaxCount && isReset)
            {
                m_ContentRectTrans.anchoredPosition = new Vector2(0, m_ContentRectTrans.anchoredPosition.y);
            }
        }

        //-> 计算 开始索引
        int lastEndIndex = 0;

        //-> 过多的物体 扔到对象池 ( 首次调 ShowList函数时 则无效 )
        if (m_IsInited)
        {
            lastEndIndex = num - m_MaxCount > 0 ? m_MaxCount : num;
            lastEndIndex = m_IsClearList ? 0 : lastEndIndex;

            int count = m_IsClearList ? m_CellInfos.Length : m_MaxCount;
            for (int i = lastEndIndex; i < count; i++)
            {
                if (m_CellInfos[i].obj != null)
                {
                    SetPoolsObj(m_CellInfos[i].obj);
                    m_CellInfos[i].obj = null;
                }
            }
        }

        //-> 以下四行代码 在for循环所用
        CellInfo[] tempCellInfos = m_CellInfos;
        m_CellInfos = new CellInfo[num];

        //-> 1: 计算 每个Cell坐标并存储 2: 显示范围内的 Cell
        for (int i = 0; i < num; i++)
        {
            // * -> 存储 已有的数据 ( 首次调 ShowList函数时 则无效 )
            if (m_MaxCount != -1 && i < lastEndIndex)
            {
                CellInfo tempCellInfo = tempCellInfos[i];
                //-> 计算是否超出范围
                float rPos = m_Direction == e_Direction.Vertical ? tempCellInfo.pos.y : tempCellInfo.pos.x;
                if (!IsOutRange(rPos))
                {
                    //-> 记录显示范围中的 首位index 和 末尾index
                    m_MinIndex = m_MinIndex == -1 ? i : m_MinIndex; //首位index
                    m_MaxIndex = i; // 末尾index

                    if (tempCellInfo.obj == null)
                    {
                        tempCellInfo.obj = GetPoolsObj();
                    }

                    tempCellInfo.obj.transform.GetComponent<RectTransform>().anchoredPosition = tempCellInfo.pos;
                    tempCellInfo.obj.name = i.ToString();
                    tempCellInfo.obj.SetActive(true);

                    Func(m_FuncCallBackFunc, tempCellInfo.obj);
                }
                else
                {
                    SetPoolsObj(tempCellInfo.obj);
                    tempCellInfo.obj = null;
                }

                m_CellInfos[i] = tempCellInfo;
                continue;
            }

            CellInfo cellInfo = new CellInfo();

            float pos = 0; //坐标( isVertical ? 记录Y : 记录X )
            float rowPos = 0; //计算每排里面的cell 坐标

            // * -> 计算每个Cell坐标
            if (m_Direction == e_Direction.Vertical)
            {
                pos = m_CellObjectHeight * Mathf.FloorToInt(i / m_Row) + m_Spacing * Mathf.FloorToInt(i / m_Row);
                rowPos = m_CellObjectWidth * (i % m_Row) + m_Spacing * (i % m_Row);
                cellInfo.pos = new Vector3(rowPos, -pos, 0);
            }
            else
            {
                pos = m_CellObjectWidth * Mathf.FloorToInt(i / m_Row) + m_Spacing * Mathf.FloorToInt(i / m_Row);
                rowPos = m_CellObjectHeight * (i % m_Row) + m_Spacing * (i % m_Row);
                cellInfo.pos = new Vector3(pos, -rowPos, 0);
            }

            //-> 计算是否超出范围
            float cellPos = m_Direction == e_Direction.Vertical ? cellInfo.pos.y : cellInfo.pos.x;
            if (IsOutRange(cellPos))
            {
                cellInfo.obj = null;
                m_CellInfos[i] = cellInfo;
                continue;
            }

            //-> 记录显示范围中的 首位index 和 末尾index
            m_MinIndex = m_MinIndex == -1 ? i : m_MinIndex; //首位index
            m_MaxIndex = i; // 末尾index

            //-> 取或创建 Cell
            GameObject cell = GetPoolsObj();
            cell.transform.GetComponent<RectTransform>().anchoredPosition = cellInfo.pos;
            cell.gameObject.name = i.ToString();

            //-> 存数据
            cellInfo.obj = cell;
            m_CellInfos[i] = cellInfo;

            //-> 回调  函数
            Func(m_FuncCallBackFunc, cell);
        }

        m_MaxCount = num;
        m_IsInited = true;

        OnDragListener(Vector2.zero);
    }

    // 更新滚动区域的大小
    public void UpdateSize()
    {
        Rect rect = GetComponent<RectTransform>().rect;
        m_PlaneHeight = rect.height;
        m_PlaneWidth = rect.width;
    }

    //滑动事件
    protected virtual void ScrollRectListener(Vector2 value)
    {
        UpdateCheck();
    }

    private void UpdateCheck()
    {
        if (m_CellInfos == null)
            return;

        //检查超出范围
        for (int i = 0, length = m_CellInfos.Length; i < length; i++)
        {
            CellInfo cellInfo = m_CellInfos[i];
            GameObject obj = cellInfo.obj;
            Vector3 pos = cellInfo.pos;

            float rangePos = m_Direction == e_Direction.Vertical ? pos.y : pos.x;
            //判断是否超出显示范围
            if (IsOutRange(rangePos))
            {
                //把超出范围的cell 扔进 poolsObj里
                if (obj != null)
                {
                    SetPoolsObj(obj);
                    m_CellInfos[i].obj = null;
                }
            }
            else
            {
                if (obj == null)
                {
                    //优先从 poolsObj中 取出 （poolsObj为空则返回 实例化的cell）
                    GameObject cell = GetPoolsObj();
                    cell.transform.localPosition = pos;
                    cell.gameObject.name = i.ToString();
                    m_CellInfos[i].obj = cell;

                    Func(m_FuncCallBackFunc, cell);
                }
            }
        }
    }

    //判断是否超出显示范围
    protected bool IsOutRange(float pos)
    {
        Vector3 listP = m_ContentRectTrans.anchoredPosition;
        if (m_Direction == e_Direction.Vertical)
        {
            if (pos + listP.y > m_CellObjectHeight || pos + listP.y < -rectTrans.rect.height)
            {
                return true;
            }
        }
        else
        {
            //Debug.Log("pos" + pos + "listP.x" + listP.x + "pos + listP.x" + (pos + listP.x) + "m_CellObjectWidth" + m_CellObjectWidth + "rectTrans.rect.width" + rectTrans.rect.width);
            if (pos + listP.x < -m_CellObjectWidth - 1 || pos + listP.x > rectTrans.rect.width + 1)
            {
                return true;
            }
        }

        return false;
    }

    //对象池 机制  (存入， 取出) cell
    protected Stack<GameObject> poolsObj = new Stack<GameObject>();

    //外部取出 cell
    public GameObject GetObjByIndex(int index)
    {
        GameObject cell = null;
        if (m_CellInfos.Length > index)
        {
            cell = m_CellInfos[index].obj;
        }
        return cell;
    }

    //取出 cell
    protected virtual GameObject GetPoolsObj()
    {
        GameObject cell = null;
        if (poolsObj.Count > 0)
        {
            cell = poolsObj.Pop();
        }

        if (cell == null)
        {
            cell = Instantiate(m_CellGameObject, m_Content.transform) as GameObject;
            // 添加点击事件监听
            AddCellClickEvent(cell);
        }

        cell.transform.SetParent(m_Content.transform);
        cell.transform.localScale = Vector3.one;
        UIUtils.SetActive(cell, true);

        return cell;
    }

    //存入 cell
    protected virtual void SetPoolsObj(GameObject cell)
    {
        if (cell != null)
        {
            poolsObj.Push(cell);
            m_FuncRecycleCallback?.Invoke(cell);
            UIUtils.SetActive(cell, false);
        }
    }

    // 绑定点击事件到Cell
    private void AddCellClickEvent(GameObject cell)
    {
        EventTrigger trigger = cell.GetComponent<EventTrigger>() ?? cell.AddComponent<EventTrigger>();
        EventTrigger.Entry entryClick = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        entryClick.callback.AddListener((data) => 
        {
            if (m_FuncOnClickCallBack != null)
            {
                int index = int.Parse(cell.name) + 1; // 转换为1-based索引
                m_FuncOnClickCallBack(cell, index);
            }
        });
        trigger.triggers.Add(entryClick);
    }
    
    //回调
    protected void Func(Action<GameObject, int> func, GameObject selectObject, bool isUpdate = false)
    {
        int num = int.Parse(selectObject.name) + 1;
        if (func != null)
        {
            func(selectObject, num);
        }
    }

    public void DisposeAll()
    {
        if (m_FuncCallBackFunc != null)
        {
            m_FuncCallBackFunc = null;
        }

        if (m_FuncOnClickCallBack != null)
        {
            m_FuncOnClickCallBack = null;
        }
    }

    protected void OnDestroy()
    {
        DisposeAll();
    }

    public virtual void OnClickCell(GameObject cell)
    {
    }

    //-> ExpandCircularScrollView 函数
    public virtual void OnClickExpand(int index)
    {
    }

    /// <summary>
    /// 跳转到指定item索引
    /// </summary>
    /// <param name="index">索引</param>
    public void SetToIndex(int index)
    {
        if (m_Direction == e_Direction.Vertical)
        {
            float posy = m_CellObjectHeight * index + index * m_Spacing;
            if (m_ContentHeight - rectTrans.rect.height < posy)
            {
                posy = m_ContentHeight - rectTrans.rect.height;
            }
            if (posy <= rectTrans.rect.height)
            {
                posy = 0;
            }
            m_ContentRectTrans.anchoredPosition = new Vector2(0, posy);
        }
        else
        {
            float posx = m_CellObjectWidth * index + index * m_Spacing;
            if (m_ContentWidth - rectTrans.rect.width < posx)
            {
                posx = m_ContentWidth - rectTrans.rect.width;
            }
            m_ContentRectTrans.anchoredPosition = new Vector2(-posx, 0);
        }
        UpdateCheck();
    }

    /// <summary>
    /// 跳转到指定item索引,非反复时使用
    /// </summary>
    /// <param name="index">索引</param>
    public void SetToIdx(int index)
    {
        if (m_Direction == e_Direction.Vertical)
        {
            float posy = m_CellObjectHeight * index + index * m_Spacing;
            if (m_ContentHeight - rectTrans.rect.height < posy)
            {
                posy = m_ContentHeight - rectTrans.rect.height;
            }
            m_ContentRectTrans.anchoredPosition = new Vector2(0, posy);
        }
        else
        {
            float posx = m_CellObjectWidth * index + index * m_Spacing;
            if (m_ContentWidth - rectTrans.rect.width < posx)
            {
                posx = m_ContentWidth - rectTrans.rect.width;
            }
            m_ContentRectTrans.anchoredPosition = new Vector2(posx, 0);
        }
        UpdateCheck();
    }

    //-> FlipCircularScrollView 函数
    public virtual void SetToPageIndex(int index)
    {
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (m_ContentRectTrans != null)
        {
            if (m_ContentRectTrans.anchoredPosition.y < -50)
            {
                if (m_FunCallDragEnd != null)
                {
                    m_FunCallDragEnd(true, "up");
                }
            }
            else if (m_ContentHeight - rectTrans.rect.height < m_ContentRectTrans.anchoredPosition.y)
            {
                if (m_FunCallDragEnd != null)
                {
                    m_FunCallDragEnd(true, "down");
                }
            }
        }
    }

    protected void OnDragListener(Vector2 value)
    {
        float normalizedPos = m_Direction == e_Direction.Vertical
            ? m_ScrollRect.verticalNormalizedPosition
            : m_ScrollRect.horizontalNormalizedPosition;

        if (m_Direction == e_Direction.Vertical)
        {
            if (m_ContentHeight - rectTrans.rect.height < 10)
            {
                UIUtils.SetActive(m_PointingFirstArrow, false);
                UIUtils.SetActive(m_PointingEndArrow, false);
                return;
            }
        }
        else
        {
            if (m_ContentWidth - rectTrans.rect.width < 10)
            {
                UIUtils.SetActive(m_PointingFirstArrow, false);
                UIUtils.SetActive(m_PointingEndArrow, false);
                return;
            }
        }

        var normalized = 1f / m_MaxCount;

        if (normalizedPos >= 1 - normalized)
        {
            UIUtils.SetActive(m_PointingFirstArrow, false);
            UIUtils.SetActive(m_PointingEndArrow, true);
        }
        else if (normalizedPos <= normalized)
        {
            if (m_PointingEndArrow != null && m_PointingEndArrow.activeSelf)
            {
                m_FunDownArrowHide?.Invoke();
            }
            UIUtils.SetActive(m_PointingFirstArrow, true);
            UIUtils.SetActive(m_PointingEndArrow, false);
        }
        else
        {
            UIUtils.SetActive(m_PointingFirstArrow, true);
            UIUtils.SetActive(m_PointingEndArrow, true);
        }
    }

    public void Clear()
    {
        m_FuncCallBackFunc = null;
        m_FuncOnClickCallBack = null;
        m_FuncOnButtonClickCallBack = null;
        m_FunCallDragEnd = null;
    }
}