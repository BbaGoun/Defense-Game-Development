using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartButton : MonoBehaviour
{
    public void MainStartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
