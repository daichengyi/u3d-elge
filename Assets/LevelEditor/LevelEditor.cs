using System.Collections.Generic;
using TMPro;
using UnityEngine;

/** 关卡编辑器*/
// [ExecuteInEditMode]
public class LevelEditor : MonoBehaviour
{
    [SerializeField] private GameObject content;

    //格子
    [SerializeField] private GameObject grid;
    // 关卡节点
    [SerializeField] private GameObject levelNode;
    //动物
    [SerializeField] private GameObject sheep;

    // 网格行数
    [SerializeField] private int rows = 5;
    // 网格列数
    [SerializeField] private int columns = 5;

    [SerializeField] private TMP_InputField tmpInput;

    // 存储所有格子的引用
    private GameObject[,] gridObjects;

    private Dictionary<string, GameObject> sheepDictionary = new Dictionary<string, GameObject>();

    void Start()
    {
        UIManager.Instance.HideLoading();
        GenerateGrid();
    }

    // 生成网格
    private void GenerateGrid()
    {
        // 清除已有的格子
        ClearGrid();

        // 创建新的格子数组
        gridObjects = new GameObject[rows, columns];

        // 计算格子大小，使其填满content
        float gridWidth = 1;
        float gridHeight = 1;

        // 计算起始位置（左下角）
        Vector3 startPosition = grid.transform.position;
        Quaternion startEuler = grid.transform.rotation;

        int index = 0;
        // 生成格子
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // 计算格子位置
                Vector3 position = new Vector3(
                    startPosition.x + col * gridWidth,
                    startPosition.y,
                    startPosition.z + row * gridHeight);

                // 创建格子
                GameObject gridObj = Instantiate(grid, position, startEuler);
                gridObj.transform.SetParent(content.transform);
                gridObj.SetActive(true);

                gridObj.name = $"{index}";

                // 设置格子大小
                // gridObj.transform.localScale = new Vector3(gridWidth * 0.9f, 1, gridHeight * 0.9f);

                // 获取Grid组件并设置引用
                // Grid gridComponent = gridObj.GetComponent<Grid>();
                // if (gridComponent != null)
                // {
                //     gridComponent.SetLevelEditor(this);
                //     gridComponent.SetGridPosition(new Vector2Int(col, row));
                // }

                // 存储格子引用
                gridObjects[row, col] = gridObj;
                index++;
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 获取被点击的物体
            GameObject clickedObject = getMouseClickObj();
            if (clickedObject)
            {
                CreateSheepAtGrid(clickedObject);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            //移除
            // 获取被点击的物体
            GameObject clickedObject = getMouseClickObj();
            if (clickedObject)
            {
                GameObject sheepObj = sheepDictionary[clickedObject.transform.name];
                if (sheepObj)
                {
                    sheepDictionary.Remove(clickedObject.transform.name);
                    Destroy(sheepObj);
                }
            }
        }
    }

    private GameObject getMouseClickObj()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.name == "Plane") return null;
            return hit.collider.gameObject;
        }
        return null;
    }

    // 清除已有的格子
    private void ClearGrid()
    {
        if (gridObjects != null)
        {
            foreach (GameObject gridObj in gridObjects)
            {
                if (gridObj != null && gridObj.activeInHierarchy)
                {
                    if (gridObj.name.IndexOf("grid") > -1)
                    {
                        DestroyImmediate(gridObj);
                    }
                }
            }
        }
    }

    // 提供给Grid调用的方法，用于生成sheep
    public void CreateSheepAtGrid(GameObject gridObj)
    {
        // 获取对应格子的位置
        if (gridObj != null)
        {
            GameObject sheepObj;
            if (sheepDictionary.ContainsKey(gridObj.transform.name))
            {
                //已有，旋转方向、
                sheepObj = sheepDictionary[gridObj.transform.name];
                sheepObj.transform.eulerAngles = new Vector3(0, (sheepObj.transform.eulerAngles.y + 90) % 360, 0);
                return;
            }

            // 创建sheep，位置与格子相同，但y轴稍高
            Vector3 sheepPosition = gridObj.transform.position;
            sheepPosition.y = 0.1f; // 稍微抬高，避免与格子重叠

            // 实例化sheep
            sheepObj = Instantiate(sheep, sheepPosition, Quaternion.identity);

            // 将sheep设为level的子物体
            sheepObj.transform.SetParent(levelNode.transform);
            sheepObj.SetActive(true);

            sheepObj.transform.name = gridObj.transform.name;

            sheepDictionary.Add(gridObj.transform.name, sheepObj);
        }

    }

    // 设置网格大小
    public void SetGridSize(int newRows, int newColumns)
    {
        rows = Mathf.Max(1, newRows);
        columns = Mathf.Max(1, newColumns);

        // 重新生成网格
        GenerateGrid();
    }



    //======================================操作UI=========================================
    public void OnClickSave()
    {
        //保存
        int level = int.Parse(tmpInput.text);

        // 创建关卡数据
        LevelData levelData = new LevelData();
        levelData.name = "level" + level;
        levelData.position = levelNode.transform.position;
        levelData.eulerAngles = levelNode.transform.eulerAngles;
        levelData.localScale = levelNode.transform.localScale;

        // 获取所有sheep
        foreach (var sheep in sheepDictionary.Values)
        {
            SheepData sheepData = new SheepData();
            sheepData.index = sheep.transform.name;
            sheepData.position = sheep.transform.position;
            sheepData.eulerAngles = sheep.transform.eulerAngles;
            sheepData.localScale = sheep.transform.localScale;

            levelData.sheeps.Add(sheepData);
        }

        // 序列化为JSON
        string json = JsonUtility.ToJson(levelData, true); // true表示格式化JSON

        // 确保目录存在
        string directoryPath = "Assets/LevelEditor/Json";
        if (!System.IO.Directory.Exists(directoryPath))
        {
            System.IO.Directory.CreateDirectory(directoryPath);
        }

        // 保存到文件
        string filePath = System.IO.Path.Combine(directoryPath, levelData.name + ".json");
        System.IO.File.WriteAllText(filePath, json);

        Debug.Log("关卡数据已保存到: " + filePath);

        // 如果在编辑器中，刷新资源视图
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public void OnClickLoad()
    {
        // 确保目录存在
        string directoryPath = "Assets/LevelEditor/Json";
        if (!System.IO.Directory.Exists(directoryPath))
        {
            Debug.LogWarning("目录不存在: " + directoryPath);
            return;
        }

        // 获取目录中的所有JSON文件
        string[] jsonFiles = System.IO.Directory.GetFiles(directoryPath, "*.json");
        if (jsonFiles.Length == 0)
        {
            Debug.LogWarning("没有找到JSON文件");
            return;
        }

        // 如果输入框有值，尝试加载对应关卡
        if (!string.IsNullOrEmpty(tmpInput.text))
        {
            string levelName = "level" + tmpInput.text + ".json";
            string filePath = System.IO.Path.Combine(directoryPath, levelName);

            if (System.IO.File.Exists(filePath))
            {
                LoadLevelFromFile(filePath);
                return;
            }
            else
            {
                Debug.LogWarning("文件不存在: " + filePath);
            }
        }
    }

    private void LoadLevelFromFile(string filePath)
    {
        try
        {
            // 读取JSON文件
            string json = System.IO.File.ReadAllText(filePath);

            // 反序列化为LevelData
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);

            // 清除现有的sheep
            ClearAllSheep();

            // 设置level节点的transform
            levelNode.transform.position = levelData.position;
            levelNode.transform.eulerAngles = levelData.eulerAngles;
            levelNode.transform.localScale = levelData.localScale;

            // 创建所有sheep
            foreach (SheepData sheepData in levelData.sheeps)
            {
                // 实例化sheep
                GameObject sheepObj = Instantiate(sheep, sheepData.position, Quaternion.Euler(sheepData.eulerAngles));
                sheepObj.transform.localScale = sheepData.localScale;

                // 将sheep设为level的子物体
                sheepObj.transform.SetParent(levelNode.transform);
                sheepObj.SetActive(true);

                // 添加到字典中
                sheepDictionary[sheepData.index] = sheepObj;
            }

            Debug.Log("成功加载关卡: " + System.IO.Path.GetFileNameWithoutExtension(filePath));
        }
        catch (System.Exception e)
        {
            Debug.LogError("加载关卡失败: " + e.Message);
        }
    }

    private void ClearAllSheep()
    {
        // 清除所有现有的sheep
        foreach (GameObject sheepObj in sheepDictionary.Values)
        {
            if (sheepObj != null)
            {
                Destroy(sheepObj);
            }
        }

        // 清空字典
        sheepDictionary.Clear();

        // 也可以直接销毁level下的所有子物体
        foreach (Transform child in levelNode.transform)
        {
            Destroy(child.gameObject);
        }
    }
}


[System.Serializable]
public class LevelData
{
    public string name;
    public Vector3 position;
    public Vector3 eulerAngles;
    public Vector3 localScale;
    public List<SheepData> sheeps = new List<SheepData>();
}

[System.Serializable]
public class SheepData
{
    public string index;
    public Vector3 position;
    public Vector3 eulerAngles;
    public Vector3 localScale;
}