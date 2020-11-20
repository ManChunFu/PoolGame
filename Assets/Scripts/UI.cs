using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel = null;
    [SerializeField] private GameObject _gameOverPanel = null;
    [SerializeField] private Image[] _ballImages = default;
    [SerializeField] private Sprite[] _ballSprites = default;
    [SerializeField] private MainCamera _camera = null;


    private int _ballCounter = 0;
    private void OnEnable()
    {
        PoolBall.IntoHole += DisplayBall; 
    }

    private void OnDisable()
    {
        PoolBall.IntoHole -= DisplayBall;
    }
    private void Start()
    {
        if (_pausePanel == null)
        {
            _pausePanel = transform.GetChild(0).gameObject;
            Assert.IsNotNull(_pausePanel, "No reference to the game object Pause_Panel.");
        }

        if (_pausePanel.activeSelf)
            _pausePanel.SetActive(false);

        if (_gameOverPanel == null)
        {
            _gameOverPanel = transform.GetChild(1).gameObject;
            Assert.IsNotNull(_gameOverPanel, "No reference to the game object GameOver_Panel");
        }

        if (_gameOverPanel.activeSelf)
            _gameOverPanel.SetActive(false);

        if (_camera == null)
        {
            _camera = FindObjectOfType<MainCamera>();
            Assert.IsNotNull(_camera, "No reference to the MainCamera class.");
        }

        _camera.enabled = true;

        if (_ballImages.Length > 0)
        {
            foreach (var image in _ballImages)
            {
                if (image.IsActive())
                    image.enabled = false;
            }
        }

    }

    public void ActivatePause(bool activate)
    {
        _pausePanel.SetActive(activate);
        _camera.enabled = !activate;
        Time.timeScale = (activate) ? 0 : 1;
    }

    public void ActivateGameOver(bool activate)
    {
        _ballCounter = 0;
        _gameOverPanel.SetActive(activate);
        _camera.enabled = !activate;
        Time.timeScale = (activate) ? 0 : 1;

    }

    private void DisplayBall(int ID)
    {
        _ballCounter++;

        if (_ballCounter == 7)
        {
            ActivateGameOver(true);
            return;
        }

        foreach (var image in _ballImages)
        {
            if (!image.IsActive())
            {
                image.sprite = _ballSprites[ID - 1];
                image.enabled = true;
                break;
            }
        }
    }

}
