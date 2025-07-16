using UnityEngine;

public class HomeScene : MonoBehaviour
{
    void Start()
    {
        // UIManager.Instance.HideLoading();
    }
    void Update()
    {

    }
    public void onBtnGame()
    {
        UIManager.Instance.ShowLoading();
        ResourceManager.LoadScene("Game");
    }

    public void onBtnLevelEditor()
    {
        UIManager.Instance.ShowLoading();
        ResourceManager.LoadScene("LevelEditor");
    }

    public void onBtnSet()
    {
        _ = UIManager.Instance.OpenView(VIEW_NAME.Set);
    }
}
