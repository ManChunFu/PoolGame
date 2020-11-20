using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UI _ui = null;

    private void OnEnable()
    {
        CueBall.OnFallToHole += GameOver;
    }

    private void OnDisable()
    {
        CueBall.OnFallToHole -= GameOver;
    }
    private void Start()
    {
        Time.timeScale = 1;
        if (_ui == null)
        {
            _ui = FindObjectOfType<UI>();
            Assert.IsNotNull(_ui, "No reference to the game object Pause_Panel");
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _ui.ActivatePause(true);
        }
    }

    private void GameOver()
    {
        _ui.ActivateGameOver(true);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
        PoolBallsContainer.RemoveAll();
    }

    public void ResumeGame()
    {
        _ui.ActivatePause(false);
    }

    public void Restart()
    {
        _ui.ActivateGameOver(false);
        PoolBallsContainer.RemoveAll();
        SceneManager.LoadScene(1);

    }

}
