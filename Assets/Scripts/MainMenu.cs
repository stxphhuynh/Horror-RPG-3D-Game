
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public string gameSceneName = "GameScene";
    
    public void Playgame()
    {
        // name of gameplay scene
        SceneManager.LoadScene(gameSceneName);
        
    }

   
    public void QuitGame()
    {
        Application.Quit();
        
    }
}
