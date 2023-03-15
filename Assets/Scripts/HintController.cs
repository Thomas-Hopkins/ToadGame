/* Author: Thomas Hopkins
 * Date: 12/11/2021
 * FOR CMPSCI 3410 UMSL Prof. Henry Kang
 * 
 * This class handles storing some data for hint objects.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintController : MonoBehaviour
{
    public string hintTitle;
    public string hintText;


    public string Title
    {
        get => hintTitle;
    }

    public string Text
    {
        get => hintText;
    }
}
