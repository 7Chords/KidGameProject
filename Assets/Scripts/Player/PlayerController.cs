using UnityEngine;

//角色测试脚本
public class PlayerController : MonoBehaviour
{
    [Header("移动参数")]
    [Tooltip("角色移动速度")]
    public float MoveSpeed = 5.0f;


    private Rigidbody rb;
    private Vector3 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleInput();
        Rotate();
    }
    private void HandleInput()
    {
        if (!GlobalValue.EnablePlayerInput) return;
        // 处理输入
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;
    }
    private void Rotate()
    {
        if (movement.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = targetRotation;
        }
    }
    void FixedUpdate()
    {
        if (rb != null && movement != Vector3.zero)
        {
            rb.MovePosition(rb.position + movement * MoveSpeed * GlobalValue.GameDeltaTime);
        }
    }
}