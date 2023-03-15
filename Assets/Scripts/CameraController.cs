/* Author: Thomas Hopkins
 * Date: 12/10/2021
 * FOR CMPSCI 3410 UMSL Prof. Henry Kang
 * 
 * This class controlls the main player camera movement. 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject playerParent;
    public float maxZoom = 20f;
    public float minZoom = 1f;
    public float rotateSpeed = 400f;
    public float zoomSpeed = 0.5f;

    private float zoomVelocity = 0f;
    private Quaternion targetRotation;
    private bool isRotating = false;

    // Start is called before the first frame update
    void Start()
    {
        AttachToPlayer();
    }

    void RotateCamera()
    {
        // Rotate clockwise or counter clockwise based on input
        if (!isRotating && Input.GetKeyDown("["))
        {
            isRotating = true;
            targetRotation = Quaternion.Euler(new Vector3(0, 90, 0) + transform.parent.rotation.eulerAngles);
        }
        else if (!isRotating && Input.GetKeyDown("]"))
        {
            isRotating = true;
            targetRotation = Quaternion.Euler(new Vector3(0, -90, 0) + transform.parent.rotation.eulerAngles);
        }

        // Calculate and apply amount of rotation per frame
        transform.parent.rotation = Quaternion.RotateTowards(transform.parent.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        // If we've approximately the full amount update the state
        if (Mathf.Approximately(transform.parent.rotation.eulerAngles.y, targetRotation.eulerAngles.y))
        {
            isRotating = false;
        }
    }

    void ZoomCamera()
    {
        // Get current input of zoom
        float zoomLevel = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

        // If we have some sort of non-zero input add it to current zoom velocity
        if (Mathf.Abs(zoomLevel) > 0)
        {
            zoomVelocity += zoomLevel;
        }
        else
        {
            // If no input recieved start interpolating velocity down to zero
            if (Mathf.Abs(zoomVelocity) < 0.1) zoomVelocity = 0;
            else zoomVelocity += zoomVelocity > 0 ? -Time.deltaTime : Time.deltaTime;
        }

        // Calculate new camera position
        Vector3 zoomPosition = transform.position + (transform.forward * zoomVelocity);

        // Ensure new position not outside of bounds, if it is do not update position
        if (Vector3.Distance(zoomPosition, playerParent.transform.position) > minZoom) return;
        if (Vector3.Distance(zoomPosition, playerParent.transform.position) < maxZoom) return;
        
        // Update position
        transform.position = zoomPosition;
    }

    // Update is called once per frame
    void Update()
    {
        RotateCamera();
        ZoomCamera();
    }

    public Vector3 GetCameraRotation()
    {
        return transform.parent.rotation.eulerAngles;
    }

    public void DetachFromPlayer()
    {
        transform.parent.parent = null;
    }

    public void AttachToPlayer()
    {
        transform.parent.parent = playerParent.transform;
        transform.parent.localPosition = new Vector3(0, 0, 0);
        transform.LookAt(playerParent.transform);
    }
}
