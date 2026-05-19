using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("Panneaux")]
    public GameObject pausePanel;

    private bool _enPause = false;
    public bool EnPause => _enPause;

    void Awake()
    {
        Instance = this;
    }

    public void TogglePause()
    {
        _enPause = !_enPause;

        if (_enPause)
            Ouvrir();
        else
            Fermer();
    }

    public void Ouvrir()
    {
        _enPause = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // gèle le temps
    }

    public void Fermer()
    {
        _enPause = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // reprend le temps
    }

    public void AllerMainMenu()
    {
        Time.timeScale = 1f; // important — remettre avant de changer de scène
        GameManager.Instance.LoadMainMenu();
    }
}