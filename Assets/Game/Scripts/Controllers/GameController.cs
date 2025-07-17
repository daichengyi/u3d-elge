using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> movableModels;
    private static GameController _instance;
    public static GameController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameController>();
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
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private async void Init()
    {
        movableList = new List<MovableController>();

        TextAsset json = await ResourceManager.AsyncLoadRes<TextAsset>($"Res/Json/level{UserModel.Ins.level}.json");

        // 反序列化为LevelData
        LevelData levelData = JsonUtility.FromJson<LevelData>(json.text);

        GameObject levelNode = transform.Find("level").gameObject;
        // 设置level节点的transform
        levelNode.transform.position = levelData.position;
        levelNode.transform.eulerAngles = levelData.eulerAngles;
        levelNode.transform.localScale = levelData.localScale;

        // movable
        foreach (MovableData mData in levelData.movables)
        {
            // movable
            GameObject movableObj = Instantiate(movableModels[mData.type], mData.position, Quaternion.Euler(mData.eulerAngles));

            movableObj.transform.name = mData.index;
            movableObj.transform.localScale = mData.localScale;

            // 将sheep设为level的子物体
            movableObj.transform.SetParent(levelNode.transform);
            movableObj.SetActive(true);

            MovableController movableController = movableObj.GetComponent<MovableController>();
            movableController.init(mData.type);
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
