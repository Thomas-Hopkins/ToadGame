/* Author: Thomas Hopkins
 * Date: 12/10/2021
 * FOR CMPSCI 3410 UMSL Prof. Henry Kang
 * 
 * This class serves as a description for a game level and holds various
 * data for game levels.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLevel : MonoBehaviour
{
    public string levelName = "DEFAULT";
    public string zoneName = "DEFAULT";
    public string sceneName = "SampleScene";
    public Sprite image;

    public string Name
    {
        get => zoneName + " : " + levelName;
    }

    public Sprite Image
    {
        get => image;
        set => image = value;
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(sceneName);
    }

}
