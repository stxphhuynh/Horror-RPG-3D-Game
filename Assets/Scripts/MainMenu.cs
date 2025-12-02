
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public string gameSceneName = "GameScene";
    
    public void PlayGame()
    {
        // name of gameplay scene
        SceneManager.LoadScene(1);
        
    }
    public void QuitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
        
    }
    public void WeaponMenu(){

    SceneManager.LoadScene("WeaponMenu");
    }
}
