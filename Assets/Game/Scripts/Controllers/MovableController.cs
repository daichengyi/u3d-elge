using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovableController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float backDistance = -2f;
    [SerializeField] float backDuration = 0.5f;
    private bool isMoving = false;
    private bool isRewinding = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            // 根据物体自身旋转角度，朝向本地z轴方向移动
            transform.Translate(0, 0, moveSpeed * Time.deltaTime, Space.Self);

            // 检查是否移出屏幕
            if (!IsVisibleOnScreen())
            {
                isMoving = false;
                Debug.Log("Object moved out of screen");
                Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        this.isMoving = false;

        this.isRewinding = true;

        // 停止任何可能正在进行的DOTween动画
        DOTween.Kill(transform);

        // 使用DOTween实现Z轴回退
        transform.DOLocalMoveX(transform.localPosition.x + backDistance, backDuration)
            .SetEase(Ease.InOutElastic)
            .OnComplete(() =>
            {
                isRewinding = false;
                Debug.Log("Rewind completed");
            });
    }

    // 注意：此方法要求物体必须有3D Collider组件（如BoxCollider）才能正常工作
    // 同时确保物体的Layer没有被设置为忽略Raycast，且相机设置正确
    void OnMouseDown()
    {
        // 只有在不处于回退状态时才响应点击
        if (!isRewinding)
        {
            isMoving = true;
            Debug.Log("3D object clicked, starting to move");
        }
        else
        {
            Debug.Log("Cannot move while rewinding");
        }
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
}