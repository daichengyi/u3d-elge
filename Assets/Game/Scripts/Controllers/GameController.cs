using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController _instance;
    public static GameController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameController>();
                if (_instance == null)
                {
                    Debug.LogError("GameController实例不存在");
                }
            }
            return _instance;
        }
    }

    private List<MovableController> movableList;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        movableList = new List<MovableController>();
        var levelNode = transform.GetChild(1);
        foreach (Transform child in levelNode)
        {
            var movable = child.GetComponent<MovableController>();
            if (movable != null)
            {
                movableList.Add(movable);
            }
        }
    }

    // 添加MovableController到列表
    public void RegisterMovable(MovableController movable)
    {
        if (!movableList.Contains(movable))
        {
            movableList.Add(movable);
        }
    }

    // 从列表中移除MovableController
    public void UnregisterMovable(MovableController movable)
    {
        if (movableList.Contains(movable))
        {
            movableList.Remove(movable);
        }
    }

    // 获取指定Transform对应的MovableController
    public MovableController GetMovableByTransform(Transform targetTransform)
    {
        foreach (var movable in movableList)
        {
            if (movable.transform == targetTransform)
            {
                return movable;
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
