using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static CameraController instance;
    public static Camera MainCamera { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        MainCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

    }


}
