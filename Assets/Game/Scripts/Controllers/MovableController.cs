using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 9)
        {
            Debug.Log("Collision detected");
        }
    }

    // 注意：此方法要求物体必须有3D Collider组件（如BoxCollider）才能正常工作
    // 同时确保物体的Layer没有被设置为忽略Raycast，且相机设置正确
    void OnMouseDown()
    {
        Debug.Log("3D object clicked");
    }
}
