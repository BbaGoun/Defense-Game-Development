using UnityEngine;
using UnityEngine.SceneManagement;

namespace DaeGeon
{
    public class GameStartButton : MonoBehaviour
{
    public void MainStartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
}
