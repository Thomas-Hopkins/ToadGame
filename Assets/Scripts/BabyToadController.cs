/* Author: Thomas Hopkins
 * Date: 12/10/2021
 * FOR CMPSCI 3410 UMSL Prof. Henry Kang
 * 
 * This class handles storing some data for a baby toad prefab
 * and getting it's color and index.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BabyToadController : MonoBehaviour
{
    public int toadIndex;
    private List<string> colors;

    private void Start()
    {
        colors = new List<string>();
        colors.Add("Green");
        colors.Add("Orange");
        colors.Add("Purple");
        colors.Add("Blue");
        colors.Add("Red");

        switch (toadIndex)
        {
            case 0:
                SetToadColor(Color.green);
                break;
            case 1:
                SetToadColor(new Vector4(239f / 255f, 148f / 255f, 28f / 255f, 1f));
                break;
            case 2:
                SetToadColor(new Vector4(193f / 255f, 28f / 255f, 239f / 255f, 1f));
                break;
            case 3:
                SetToadColor(Color.cyan);
                break;
            case 4:
                SetToadColor(Color.red);
                break;
            default:
                break;
        }
    }

    private void SetToadColor(Color color)
    {
        Renderer[] geoms = GetComponentsInChildren<Renderer>();
        foreach (Renderer geom in geoms)
        {
            geom.material.SetColor("_Color", color);
            geom.material.SetFloat("_Glossiness", 0.12f);
        }
    }

    public string ToadColor
    {
        get => colors[toadIndex];
    }
}
