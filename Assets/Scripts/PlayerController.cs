/* Author: Thomas Hopkins
 * Date: 12/10/2021
 * FOR CMPSCI 3410 UMSL Prof. Henry Kang
 * 
 * This class handles all user inputs, collisions, and state changes which
 * may effect the player object.
 * This class has multiple callbacks to the GameController to update the
 * game states based on player interactions.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speedMult = 10f; // Speed multiplier
    public float unitSize = 0.5f;
    public GameObject playerGeom;
    public CameraController playerCamera;
    public GameController gameController;
    public AudioSource deathSound;
    public AudioSource jumpSound;
    public AudioSource bigJumpSound;
    public AudioSource blockedSound;
    public AudioSource coinSound;
    public AudioSource savedSound;
    public AudioSource lifeSound;
    public AudioSource gameWinSound;
    public AudioSource gameLoseSound;


    public static int lives = 3; // Lives persist across levels, so we set this to static
    public int coins = 0;
    public int toadsSaved = 0;

    private bool isAirborn = true;
    private bool isDead = false;
    private bool canMove = true;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    bool IsValidMove(Vector3 movement)
    {
        // Get a list of potential colliders in the space we want to move to
        Collider[] colliders = Physics.OverlapSphere(transform.position + movement, unitSize / 3);

        // See if any of these colliders are a wall, if so this is not a valid move.
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Wall")) return false;
        }

        // Checks succeeded, valid move
        return true;
    }

    void MoveLeft()
    {
        if (isAirborn) return;

        // Calculate and apply which way we want to face
        playerGeom.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0) + playerCamera.GetCameraRotation());

        // Calculate our potential move vector based on current camera position
        Vector3 moveVector = playerCamera.transform.parent.right * -unitSize;
        if (!IsValidMove(moveVector))
        {
            blockedSound.Play();
            return;
        }

        // If this was a valid move, move here
        transform.Translate(moveVector);
        jumpSound.Play();
    }

    void MoveRight()
    {
        if (isAirborn) return;

        // Calculate and apply which way we want to face
        playerGeom.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0) + playerCamera.GetCameraRotation());

        // Calculate our potential move vector based on current camera position
        Vector3 moveVector = playerCamera.transform.parent.right * unitSize;
        if (!IsValidMove(moveVector))
        {
            blockedSound.Play();
            return;
        }

        // If this was a valid move, move here
        transform.Translate(moveVector);
        jumpSound.Play();
    }

    void MoveBackward()
    {
        if (isAirborn) return;

        // Calculate and apply which way we want to face
        playerGeom.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0) + playerCamera.GetCameraRotation());

        // Calculate our potential move vector based on current camera position
        Vector3 moveVector = playerCamera.transform.parent.forward * -unitSize;
        if (!IsValidMove(moveVector))
        {
            blockedSound.Play();
            return;
        }

        // If this was a valid move, move here
        transform.Translate(moveVector);
        jumpSound.Play();
    }

    void MoveForward()
    {
        if (isAirborn) return;

        // Calculate and apply which way we want to face
        playerGeom.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0) + playerCamera.GetCameraRotation());

        // Calculate our potential move vector based on current camera position
        Vector3 moveVector = playerCamera.transform.parent.forward * unitSize;
        if (!IsValidMove(moveVector))
        {
            blockedSound.Play();
            return;
        }

        // If this was a valid move, move here
        transform.Translate(moveVector);
        jumpSound.Play();
    }

    void MoveJumpForward()
    {
        if (isAirborn) return;

        // Calculate potential movement of forward one unit and up 2
        Vector3 moveVector = playerGeom.transform.forward * unitSize + new Vector3(0, unitSize, 0);
        if (!IsValidMove(moveVector))
        {
            blockedSound.Play();
            return;
        }

        // If valid move jump here
        isAirborn = true;
        transform.Translate(moveVector);
        bigJumpSound.Play();
    }

    void CenterOnNewParent()
    {
        // TODO: Fix bug that causes player to fall off a platform if colliding at right edge
        transform.localPosition = new Vector3(
            Mathf.Floor(transform.localPosition.x) + 0.5f,
            transform.localPosition.y,
            Mathf.Floor(transform.localPosition.z) + 0.5f
        );

    }

    // Update is called once per frame
    void Update()
    {
        // Return if we are in a cant move state or game is over
        if (!canMove) return;
        if (gameController.GameOver) return;

        if (Input.GetKeyDown("left"))
        {
            MoveLeft();
        }
        else if (Input.GetKeyDown("right")) 
        {
            MoveRight();
        }
        else if (Input.GetKeyDown("down"))
        {
            MoveBackward();
        }
        else if (Input.GetKeyDown("up"))
        {
            MoveForward();
        }
        else if (Input.GetKeyDown("space"))
        {
            MoveJumpForward();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // Switch parent if player collides with ground 
        if (other.gameObject.CompareTag("Ground") && isAirborn)
        {
            isAirborn = false;
            transform.parent = other.transform;
            CenterOnNewParent();
        }
        if (other.gameObject.CompareTag("Teleport"))
        {
            Vector3 position = other.gameObject.GetComponent<TeleporterController>().teleportTo;
            gameObject.transform.position = position;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Ground") && isAirborn)
        {
            isAirborn = false;
            transform.parent = other.transform;
            CenterOnNewParent();
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isAirborn = true;
            rb.AddForce(0, -200, 0);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.parent = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Death"))
        {
            Death();
        }
        else if (other.gameObject.CompareTag("Hint"))
        {
            HintController hint = other.GetComponent<HintController>();
            gameController.ShowHint(hint.Title, hint.Text);
        }
        else if (other.gameObject.CompareTag("Coin"))
        {
            coins++;
            coinSound.Play();
            gameController.UpdateCoins(coins);
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("ExtraLife"))
        {
            lives++;
            lifeSound.Play();
            // update lives UI
            // despawn extra life object
            other.gameObject.SetActive(false);
            gameController.UpdateLives(lives);
        }
        else if (other.gameObject.CompareTag("BabyToad"))
        {
            toadsSaved++;
            canMove = false;
            // Update toads saved UI
            // despawn toad object
            other.gameObject.SetActive(false);
            if (toadsSaved >= 5)
            {
                gameWinSound.Play();
                gameController.SetGameWon();
                BabyToadController toad = other.gameObject.GetComponent<BabyToadController>();
                gameController.UpdateToads(toad.toadIndex);
            }
            else
            {
                savedSound.Play();
                BabyToadController toad = other.gameObject.GetComponent<BabyToadController>();
                gameController.UpdateToads(toad.toadIndex);
                Invoke("Respawn", 2f);
            }
        }
    }

    private void Respawn()
    {
        playerGeom.transform.localScale = new Vector3(1, 1, 1);
        rb.velocity = Vector3.zero;
        gameController.RespawnPlayer();
        playerCamera.AttachToPlayer();
        canMove = true;
        isDead = false;
    }

    public void Death()
    {
        if (isDead) return;
        deathSound.Play();
        playerGeom.transform.localScale = new Vector3(1.2f, 0.2f, 1.2f);
        // Player death
        lives--;
        gameController.UpdateLives(lives);
        canMove = false;
        isDead = true;

        // If lives < 0 game over screen
        if (lives <= 0)
        {
            playerCamera.DetachFromPlayer();
            gameLoseSound.Play();
            gameController.SetGameOver();
        }
        else
        {
            playerCamera.DetachFromPlayer();
            gameController.ShowDeathStatus();
            Invoke("Respawn", 3f);
        }
    }

    public void setCanMove(bool val)
    {
        canMove = val;
        if (!rb) return;
        if (!canMove)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    public bool CanMove
    {
        get => canMove;
        set => setCanMove(value);
    }

}
