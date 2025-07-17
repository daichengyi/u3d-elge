using UnityEngine;

public class MovableController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    /** 射线检测距离 */
    [SerializeField] float raycastDistance = 1.5f;
    /** 碰撞层 */
    [SerializeField] LayerMask collisionLayerMask;
    /** 是否显示调试射线 */
    [SerializeField] bool showDebugRays = true;

    private bool isMoving = false;

    /** 
     * 0: 普通羊
     * 1: 爆炸羊
     * 2: 时间羊
     */
    private int type = 0;

    // 自定义事件定义
    public delegate void CollisionDetectedEvent(RaycastHit hit);
    public event CollisionDetectedEvent OnCollisionDetected;

    // Start is called before the first frame update
    public void init(int tp)
    {
        type = tp;
        // 初始化事件
        OnCollisionDetected += HandleCollision;
    }

    private void OnEnable()
    {
        // 注册到GameController
        GameController.Instance.RegisterMovable(this);
    }

    private void OnDisable()
    {
        // 从GameController注销
        GameController.Instance.UnregisterMovable(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (showDebugRays)
        {
            Debug.DrawRay(transform.position, transform.forward * raycastDistance, Color.red);
        }
        // 处理移动和前向碰撞检测
        if (isMoving)
        {
            // 前向射线检测
            Ray forwardRay = new Ray(transform.position, transform.forward);
            RaycastHit forwardHit;

            if (Physics.Raycast(forwardRay, out forwardHit, raycastDistance, collisionLayerMask))
            {
                // 检测到碰撞，触发碰撞事件
                if (OnCollisionDetected != null)
                {
                    OnCollisionDetected(forwardHit);
                    return;
                }
            }

            // 检查是否移出屏幕
            if (!IsVisibleOnScreen())
            {
                isMoving = false;
                Debug.Log("Object moved out of screen");
                Destroy(gameObject);
                return;
            }

            // 根据物体自身旋转角度，朝向本地z轴方向移动
            transform.Translate(0, 0, moveSpeed * Time.deltaTime, Space.Self);
        }
    }

    // 处理碰撞事件
    private void HandleCollision(RaycastHit hit)
    {
        isMoving = false;
        Debug.Log("Collision detected with: " + hit.transform.name);
        //播放对应动画
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