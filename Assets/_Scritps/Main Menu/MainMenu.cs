using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void EnterGame()
    {
        GameController.Instance.GamePlayScene();
    }

    public void ExitGame()
    {
        GameController.Instance.ExitGame();
    }
}
