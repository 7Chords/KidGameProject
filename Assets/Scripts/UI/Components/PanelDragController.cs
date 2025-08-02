using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PanelDragController : MonoBehaviour, 
    IPointerDownHandler, 
    IBeginDragHandler,  // 新增：确保事件链完整
    IDragHandler, 
    IEndDragHandler,    // 新增：确保事件链完整
    IPointerUpHandler
{
    [Tooltip("拖动区域（留空则整个面板可拖动）")]
    public RectTransform dragArea;
    
    [Tooltip("是否限制在父容器内拖动")]
    public bool limitToParent = true;
    
    private RectTransform panelRect;
    
    private Vector2 mouseOffset;
    private bool isDragging;

    private void Awake()
    {
        panelRect = GetComponent<RectTransform>();
        
        
        // 默认拖动区域为自身
        if (dragArea == null)
        {
            dragArea = panelRect;
            Debug.LogWarning("未指定dragArea，默认使用面板自身作为拖动区域");
        }
        
        
        
       
    }

   

    // 鼠标按下：计算偏移量
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"[{Time.time:F2}] OnPointerDown");
        
        // 检查是否在拖动区域内
        if (!RectTransformUtility.RectangleContainsScreenPoint(
            dragArea,
            eventData.position,
            eventData.pressEventCamera))
        {
            Debug.Log("点击不在拖动区域内");
            return;
        }

        // 计算鼠标与面板原点的偏移量
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panelRect,
            eventData.position,
            eventData.pressEventCamera,
            out mouseOffset
        );

        isDragging = true;
    }

    // 开始拖动：必须实现，确保事件链不中断
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"[{Time.time:F2}] OnBeginDrag");
        // 强制设置当前对象为事件接收者
        eventData.selectedObject = gameObject;
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    // 拖动中：核心移动逻辑
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
        {
            Debug.LogWarning("OnDrag触发但isDragging=false");
            return;
        }

        Debug.Log($"[{Time.time:F2}] OnDrag");

        // 计算新位置（基于父容器坐标系）
        Vector2 newLocalPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panelRect.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out newLocalPos))
        {
            Vector2 targetPos = newLocalPos - mouseOffset;
            
            // 限制在父容器内
            if (limitToParent && panelRect.parent != null)
            {
                Rect parentRect = (panelRect.parent as RectTransform).rect;
                targetPos.x = Mathf.Clamp(
                    targetPos.x, 
                    parentRect.xMin, 
                    parentRect.xMax
                );
                targetPos.y = Mathf.Clamp(
                    targetPos.y,
                    parentRect.yMin,
                    parentRect.yMax);
            }

            panelRect.localPosition = targetPos;
        }
    }

    // 结束拖动：必须实现，防止OnPointerUp提前触发
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"[{Time.time:F2}] OnEndDrag");
        isDragging = false;
    }

    // 鼠标释放：清理状态
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log($"[{Time.time:F2}] OnPointerUp");
        isDragging = false;
    }

    // 防止禁用时状态残留
    private void OnDisable()
    {
        isDragging = false;
    }

    // Gizmos绘制拖动区域（Scene视图可视化）
    private void OnDrawGizmosSelected()
    {
        if (dragArea == null) return;
        
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Vector3[] corners = new Vector3[4];
        dragArea.GetWorldCorners(corners);
        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
        }
    }
}