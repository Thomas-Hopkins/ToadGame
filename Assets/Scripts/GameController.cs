/* Author: Thomas Hopkins
 * Date: 12/10/2021
 * FOR CMPSCI 3410 UMSL Prof. Henry Kang
 * 
 * This class handles interactions all main game logic such as:
 * GUI updates
 * Game state (won, lost, paused)
 * Game start
 * Level management
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public PlayerController Player;

    public Text coinText;
    public Text startText;
    public Text startTimeText;
    public Text livesText;
    public Text hintText;
    public Text hintTitle;
    public Text statusText;
    public GameObject hintPanel;
    public GameObject startPanel;
    public GameObject toadPanelStack;
    public GameObject pausePanel;
    public Text timerText;
    public Slider timerSlider;
    public float levelTime;
    public bool DEBUG;

    private bool gameOver = false;
    private bool gameWon = false;
    private Vector3 spawnPoint;
    private static bool isPaused = false;

    private float timer;
    private List<Image> toadUIStack;

    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
        startPanel.transform.parent.gameObject.SetActive(true);
        Player.CanMove = false;
        UpdateCoins(Player.coins);
        UpdateLives(PlayerController.lives);

        spawnPoint = Player.transform.position;
        timerSlider.maxValue = levelTime;
        timerSlider.value = 0;

        toadUIStack = new List<Image>();
        Image[] toads = toadPanelStack.GetComponentsInChildren<Image>();
        foreach (Image toad in toads)
        {
            toadUIStack.Add(toad);
            toad.color = Color.grey;
        }

        statusText.gameObject.SetActive(false);

        startText.text = "Find and rescue all baby toads before time runs out! Collect coins for a higher score!";
        startTimeText.text = ((int)levelTime).ToString() + " SECONDS!";
        Invoke("EndStartSequence", 4f);

    }

    // Update is called once per frame
    void Update()
    {
        if (DEBUG && Input.GetKeyDown(KeyCode.Backspace)) SetGameWon(); 
        // Pause game if we hit esc
        if (Input.GetKeyDown("escape")) TogglePause();
        if (!Player.CanMove) return;
        // Decrease timer
        timer -= Time.deltaTime;

        // If timer has reached 0 we have died
        if (timer <= 0)
        {
            CancelInvoke("HideStatusText");
            Player.Death();
            if (gameOver) return;
            statusText.text = "OUT OF TIME.";
            statusText.gameObject.SetActive(true);
            Invoke("HideStatusText", 2f);
            return;
        }

        timerSlider.value = timerSlider.maxValue - timer;
        timerText.text = "TIME LEFT: " + ((int)timer).ToString();

    }

    private void EndStartSequence()
    {
        if (isPaused)
        {
            Invoke("EndStartSequence", 2f);
            return;
        }
        Player.CanMove = true;
        startPanel.SetActive(false);
        timer = levelTime;
    }

    private void HideStatusText()
    {
        statusText.gameObject.SetActive(false);
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private void LoadNextLevel()
    {
        MenuController.levelStack[MenuController.selectedLevel].LoadLevel();
    }

    public void ShowDeathStatus()
    {
        CancelInvoke("HideStatusText");
        statusText.text = "YOU DIED.";
        statusText.color = Color.red;
        statusText.gameObject.SetActive(true);
        Invoke("HideStatusText", 3f);
    }

    public void ShowHint(string title, string body, float time = 5f)
    {
        CancelInvoke("HideHint");
        hintPanel.SetActive(true);
        hintText.text = body;
        hintTitle.text = title;
        Invoke("HideHint", time);
    }

    public void HideHint()
    {
        hintPanel.SetActive(false);
    }

    public void RespawnPlayer()
    {
        Player.transform.position = spawnPoint;
        timer = levelTime;
    }

    public void UpdateCoins(int coins)
    {
        coinText.text = coins + "/15" + " coins";
    }

    public void UpdateLives(int lives)
    {
        livesText.text = lives.ToString() + " lives";
    }

    public void UpdateToads(int toadind)
    {
        // Get the toad from the ui and set it as found
        CancelInvoke("HideStatusText");
        Image toad = toadUIStack[toadind].GetComponent<Image>();
        toad.color = Color.white;
        // Do not show toad saved text if this is the last toad 
        if (gameWon || gameOver) return;

        // Show toad saved text
        statusText.text = "Toad Saved!";
        statusText.color = Color.green;
        statusText.gameObject.SetActive(true);
        Invoke("HideStatusText", 2f);
    }

    public void AddToTimer(float time)
    {
        timer += time;
    }

    public bool GameOver
    {
        get => gameOver;
    }

    public bool GameWon
    {
        get => gameWon;
    }

    public static bool Paused
    {
        get => isPaused;
    }

    public void TogglePause()
    {
        // If we're paused unpause.
        if (isPaused)
        {
            pausePanel.SetActive(false);
            Player.CanMove = true;
            isPaused = false;
        }
        // Only pause if the game is not over and we are in a move state
        else if (!gameOver && !gameWon && Player.CanMove)
        {
            pausePanel.SetActive(true);
            Player.CanMove = false;
            isPaused = true;
        }
    }

    public void SetGameOver()
    {
        // Notify of loss and return to main menu
        gameOver = true;
        statusText.text = "GAME OVER.";
        statusText.color = Color.red;
        statusText.gameObject.SetActive(true);
        Invoke("ReturnToMainMenu", 3f);
    }

    public void SetGameWon()
    {
        // Notify of win and increment level
        gameWon = true;
        statusText.text = "YOU WIN!";
        statusText.color = Color.green;
        statusText.gameObject.SetActive(true);
        MenuController.selectedLevel++;

        // If we are at the last level or level stack doesn't exist return to main menu
        if (MenuController.levelStack == null || MenuController.selectedLevel >= MenuController.levelStack.Count)
        {
            Invoke("ReturnToMainMenu", 3f);
        }
        else
        {
            // If player got all coins grant an extra live for the next level
            if (Player.coins >= 15) PlayerController.lives++;
            MenuController.unlockedLevels[MenuController.selectedLevel] = true;
            Invoke("LoadNextLevel", 3f);
        }
    }

    public void OnQuitButtonPress()
    {
        // Kill player on quit button which returns them to main menu
        TogglePause();
        PlayerController.lives = 1;
        Player.Death();
    }

    public void OnContinueButtonPress()
    {
        Player.jumpSound.Play();
        TogglePause();
    }
}
