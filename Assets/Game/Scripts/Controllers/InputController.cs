using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** 射线输入控制器*/
public class InputController : MonoBehaviour
{
    [SerializeField] LayerMask clickableLayerMask;
    [SerializeField] float maxRayDistance = 100f;
    [SerializeField] bool showDebugRays = true;

    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
        {
            Debug.LogError("InputController必须挂载在带有Camera组件的物体上");
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        // 检测鼠标点击
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (showDebugRays)
            {
                Debug.DrawRay(ray.origin, ray.direction * maxRayDistance, Color.blue, 1f);
            }

            if (Physics.Raycast(ray, out hit, maxRayDistance, clickableLayerMask))
            {
                // 射线击中物体，通过GameController查找对应的MovableController
                MovableController movable = GameController.Instance.GetMovableByTransform(hit.transform);
                if (movable != null)
                {
                    // 调用MovableController的公共方法
                    movable.StartMoving();
                    Debug.Log("InputController: 点击了可移动物体 " + hit.transform.name);
                }
                else
                {
                    Debug.Log("InputController: 点击了非可移动物体 " + hit.transform.name);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (showDebugRays && Application.isPlaying && mainCamera != null)
        {
            Gizmos.color = Color.cyan;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Gizmos.DrawRay(ray.origin, ray.direction * maxRayDistance);
        }
    }
}
