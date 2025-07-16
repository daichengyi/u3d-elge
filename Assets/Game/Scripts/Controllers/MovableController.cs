using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class MovableController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float raycastDistance = 1f;
    [SerializeField] LayerMask collisionLayerMask;
    [SerializeField] bool showDebugRays = true;

    private bool isMoving = false;

    // 自定义事件定义
    public delegate void CollisionDetectedEvent(RaycastHit hit);
    public event CollisionDetectedEvent OnCollisionDetected;

    // Start is called before the first frame update
    void Start()
    {
        // 初始化事件
        OnCollisionDetected += HandleCollision;
    }

    private void OnEnable()
    {
        // 注册到GameController
        if (GameController.Instance != null)
        {
            GameController.Instance.RegisterMovable(this);
        }
    }

    private void OnDisable()
    {
        // 从GameController注销
        if (GameController.Instance != null)
        {
            GameController.Instance.UnregisterMovable(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 处理移动和前向碰撞检测
        if (isMoving)
        {
            // 根据物体自身旋转角度，朝向本地z轴方向移动
            transform.Translate(0, 0, moveSpeed * Time.deltaTime, Space.Self);

            // 前向射线检测
            Ray forwardRay = new Ray(transform.position, transform.forward);
            RaycastHit forwardHit;

            if (showDebugRays)
            {
                Debug.DrawRay(transform.position, transform.forward * raycastDistance, Color.red);
            }

            if (Physics.Raycast(forwardRay, out forwardHit, raycastDistance, collisionLayerMask))
            {
                // 检测到碰撞，触发碰撞事件
                if (OnCollisionDetected != null)
                {
                    OnCollisionDetected(forwardHit);
                }
            }

            // 检查是否移出屏幕
            if (!IsVisibleOnScreen())
            {
                isMoving = false;
                Debug.Log("Object moved out of screen");
                Destroy(gameObject);
            }
        }
    }

    // 处理碰撞事件
    private void HandleCollision(RaycastHit hit)
    {
        this.isMoving = false;
        Debug.Log("Collision detected with: " + hit.transform.name);
    }

    // 公共接口：开始移动
    public void StartMoving()
    {
        isMoving = true;
        Debug.Log("Object starting to move");
    }

    // 公共接口：停止移动
    public void StopMoving()
    {
        isMoving = false;
        Debug.Log("Object stopped moving");
    }

    // 检查物体是否在屏幕内可见
    private bool IsVisibleOnScreen()
    {
        if (Camera.main == null)
            return false;

        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);

        // 检查是否在视口范围内 (0,0)到(1,1)
        bool visible = viewportPosition.x > 0 && viewportPosition.x < 1 &&
                       viewportPosition.y > 0 && viewportPosition.y < 1 &&
                       viewportPosition.z > 0;

        return visible;
    }

    // 在编辑器中绘制调试射线
    private void OnDrawGizmos()
    {
        if (showDebugRays)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.forward * raycastDistance);
        }
    }
}