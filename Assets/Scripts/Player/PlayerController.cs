using UnityEngine;

//��ɫ���Խű�
public class PlayerController : MonoBehaviour
{
    [Header("�ƶ�����")]
    [Tooltip("��ɫ�ƶ��ٶ�")]
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
        // ��������
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