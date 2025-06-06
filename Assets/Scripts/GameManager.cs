using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{ 
    public static GameManager Instance;

    public int Health;
    public Text HealthText;
    private int score;
    public Text ScoreText;

    public GameObject OpenUI;
    public GameObject Win;
    public GameObject BGAnim;
    public GameObject BGNonAnim;


    public GameObject Player;
    public GameObject Cam1;
    public GameObject Cam2;

    public bool gameOver = false;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Invoke("cammove", 4.8f);
        BGAnim.gameObject.SetActive(true);
        BGNonAnim.gameObject.SetActive(false);
        Health = 3;
        score = 0;
        UpdateHealthScore();
    }

    public void cammove()
    {
        Cam1.SetActive(false);
        Cam2.SetActive(true);
        Player.SetActive(true);
    }
    private void Update()
    {
        if (!gameOver)
        {
            IncrementScore(1); 
        }
    }

    public void IncrementScore(int points)
    {
        if (!gameOver)
        {
            score += points;
            UpdateHealthScore();
        }
    }

    public void DecreaseHealth(int damage)
    {
        if (!gameOver)
        {
            Health -= damage;
            UpdateHealthScore();

            if (Health <= 0)
            {
                Health = 0;
              
                gameOver = true;
                LevelFailed();
            }
        }
    }

    private void UpdateHealthScore()
    {
        HealthText.text = "Health: " + Health.ToString();
        ScoreText.text = "Score: " + score.ToString();
    }

    public void LevelComplete()
    {
        BGAnim.gameObject.SetActive(false);
        BGNonAnim.gameObject.SetActive(true);   
        
        FindObjectOfType<GameAudioManager>().PlayGameWinAudio();
        gameOver = true;
        Invoke("Close",3f);
    }

    public void LevelFailed()
    {
        
        FindObjectOfType<GameAudioManager>().PlayGameOverAudio();
        gameOver = true;
        UpdateHealthScore();
        Invoke("open",3f);
    }

    private void open()
    {
        OpenUI.SetActive(true);
    }private void Close()
    {
        Win.SetActive(true);
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(0);
    }
}