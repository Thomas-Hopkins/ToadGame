/* Author: Thomas Hopkins
 * Date: 12/10/2021
 * FOR CMPSCI 3410 UMSL Prof. Henry Kang
 * 
 * This class handles interactions with the main menu for level selection
 * and quitting the game.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject levelMenu;
    public GameObject mainCamera;
    public GameObject levelsObject;
    public GameObject playButton;
    public Text levelNameText;
    public Image levelImage;
    public Image lockImage;

    public AudioSource buttonSound;

    public bool DEBUG;

    private bool cameraTransition = false;
    private float cameraSpeed = 200f;
    private Quaternion targetCameraRotation;
    private Vector3 targetCameraPosition;

    public static List<GameLevel> levelStack;
    // Store a static list of which levels are unlocked indexes correspond with levelStack
    public static List<bool> unlockedLevels; 
    public static int selectedLevel = 0;
    public GameLevel level1;
    public GameLevel level2;


    // Start is called before the first frame update
    void Start()
    {
        SwitchToMainMenu();

        // Add levels to level selector
        levelStack = new List<GameLevel>
        {
            level1,
            level2
        };
        // Setup unlocked levels if we haven't instantiated this yet
        if (unlockedLevels == null)
        {
            unlockedLevels = new List<bool>();
            for (int i = 0; i < levelStack.Count; i++)
            {
                unlockedLevels.Add(false);
            }
            unlockedLevels[0] = true;
        }

        // Build visual level stack
        float yOffset = 0f;
        for (int i = 0; i < levelStack.Count; i++)
        {
            GameObject levelCube = levelStack[i].gameObject;

            //Call SetColor using the shader property name "_Color" and setting the color to red
            levelCube.transform.SetParent(levelsObject.transform, false);
            levelCube.transform.localPosition = new Vector3(0, yOffset--, 0);
        }
        UpdateSelectedLevel("init");
    }

    // Update is called once per frame
    void Update()
    {
        if (DEBUG && Input.GetKeyDown(KeyCode.Backspace)) unlockedLevels[selectedLevel] = true; 
        if (cameraTransition)
        {
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, targetCameraPosition, cameraSpeed * Time.deltaTime);
            mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, targetCameraRotation, cameraSpeed * Time.deltaTime);

            // Equality operator on vectors checks for approximately equal
            if (mainCamera.transform.position == targetCameraPosition && mainCamera.transform.rotation == targetCameraRotation)
            {
                cameraTransition = false;
                targetCameraPosition = mainCamera.transform.position;
                targetCameraRotation = mainCamera.transform.rotation;
            } 
        }
    }

    private void SwitchToMainMenu()
    {
        // TODO: Handle transitions
        mainMenu.SetActive(true);
        levelMenu.SetActive(false);

        targetCameraPosition = new Vector3(0, 50, -10);
        targetCameraRotation = Quaternion.Euler(0, 0, 0);
        cameraTransition = true;
    }

    private void SwitchToLevelMenu()
    {
        // TODO: Handle transitions
        mainMenu.SetActive(false);
        levelMenu.SetActive(true);

        targetCameraPosition = new Vector3(0, 1, -10);
        targetCameraRotation = Quaternion.Euler(20, 0, 0);
        cameraTransition = true;
    }

    private void UpdateSelectedLevel(string direction)
    {
        if (direction.Equals("down"))
        {
            // Return out if we've reached end of stack
            if (selectedLevel >= levelStack.Count - 1) return;
            // Move previously selected back
            levelStack[selectedLevel].gameObject.transform.Translate(new Vector3(0, 0, 2));
            selectedLevel++;
            cameraTransition = true;
            targetCameraPosition.y -= 1.2f;

        }
        else if (direction.Equals("up"))
        {
            // return out if we've reached end of stack
            if (selectedLevel <= 0) return;
            // Move previously selected back
            levelStack[selectedLevel].gameObject.transform.Translate(new Vector3(0, 0, 2));
            selectedLevel--;
            cameraTransition = true;
            targetCameraPosition.y += 1.2f;
        }
        else if (direction.Equals("init"))
        {
            selectedLevel = 0;
        }

        levelNameText.text = levelStack[selectedLevel].Name;
        levelStack[selectedLevel].gameObject.transform.Translate(new Vector3(0, 0, -2));
        //TODO: Update image
        levelImage.sprite = levelStack[selectedLevel].image;
        if (!unlockedLevels[selectedLevel])
        {
            Button btn = playButton.GetComponent<Button>();
            btn.interactable = false;
            lockImage.gameObject.SetActive(true);
        }
        else
        {
            Button btn = playButton.GetComponent<Button>();
            btn.interactable = true;
            lockImage.gameObject.SetActive(false);
        }
    }

    public void OnMainMenuPlayPress()
    {
        // Switch to level menu on main menu play button press
        buttonSound.Play();
        SwitchToLevelMenu();
    }

    public void OnMainMenuQuitPress()
    {
        // Close app on main menu quit button press
        buttonSound.Play();
        Application.Quit();
    }

    public void OnMainMenuOptionsPress()
    {
        // TODO: Eventual options menu call here
    }

    public void OnLevelMenuBackPress()
    {
        buttonSound.Play();
        SwitchToMainMenu();
    }

    public void OnLevelMenuPlayPress()
    {
        if (!unlockedLevels[selectedLevel]) return;
        PlayerController.lives = 3;
        levelStack[selectedLevel].LoadLevel();
    }

    public void OnLevelMenuUpPress()
    {
        buttonSound.Play();
        UpdateSelectedLevel("up");
    }

    public void OnLevelMenuDownPress()
    {
        buttonSound.Play();
        UpdateSelectedLevel("down");
    }
}
