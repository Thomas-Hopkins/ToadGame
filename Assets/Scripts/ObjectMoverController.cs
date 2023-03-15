/* Author: Thomas Hopkins
 * Date: 12/10/2021
 * FOR CMPSCI 3410 UMSL Prof. Henry Kang
 * 
 * This class is meant to be attached to objects which need to move to a location.
 * This handles ping-pong and looping movement as well as rotation.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMoverController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotateSpeed = 3f;
    public Vector3 moveTo = new Vector3(0, 0, 0);
    public Vector3 moveFrom = new Vector3(0, 0, 0);
    public bool rotate = false;
    public bool pingPong = true;
    public bool slow = false;
    public float slowDistance = 1f;
    public float slowStep = 0.1f;

    private float slowAmount = 1;

    // Update is called once per frame
    void Update()
    {
        if (GameController.Paused) return;
        // Get distance between current position and destination
        float distance = Vector3.Distance(transform.position, moveTo);

        // Slow down speed as we near destination
        if (slow && distance < slowDistance)
        {
            slowAmount = distance + slowStep;
        }

        transform.position = Vector3.MoveTowards(transform.position, moveTo, Time.deltaTime * moveSpeed * slowAmount);

        // If this is a "ping-pong" object swap our move destinations when the distance to it is almost 0
        if (pingPong && Mathf.Approximately(0, distance))
        {
            Vector3 tmp = moveTo;
            moveTo = moveFrom;
            moveFrom = tmp;
            slowAmount = 1;
        }
        // reset position to start if reached destination
        else if (Mathf.Approximately(0, distance))
        {
            transform.position = moveFrom;
            slowAmount = 1;
        }

        // Rotate the object if wanted
        if (rotate)
        {
            transform.Rotate(new Vector3(0, Time.deltaTime * rotateSpeed, 0));
        }
    }
}
