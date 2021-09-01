using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDragonController : MonoBehaviour
{
    public TrajectoryRenderer Trajectory;

    private Rigidbody playerDragonRB;
    //private LineRenderer aimingLinePlayer;
    private bool dirTouchPhaseBegun;

    private Camera mainCam;

    private GameObject CPUDragon;
    private Rigidbody CPUDragonRigidbody;
    private CPUDragonController CPUDragonObject;

    private Vector2 startPointOfLine;
    private Vector2 endPointOfLine;
    private Vector2 burstDirection; //vector to hold the players animal jump direction

    private Vector3 aimPoint; //point is used to show the direction of players jump and power of jump

    private string floorTag = "Floor";
    private string enemyHornTag = "EnemyHorn";
    private float jumpPowerFloat;
    public const float MAGNITUDE_MULTIPLIER = 0.05f;
    private float animalAngle; 
    private Quaternion animalQuater;
    private bool animalIsGrounded;
    //is used to correctly flip animal direction
    private bool facingRight;
    private Transform animalTransform; //to cash the transform 
    [HideInInspector]
    public int playerDragonsLife;

    private Slider playerDragonsLifeBar;
    private RawImage playerDragonsLifeBarObject;

    [HideInInspector]
    public bool playerDragonIsDied;

    private WinLoseController winLoseControllerObject;
    private MenuManager menuManagerObject;

    private void OnEnable()
    {
        mainCam = Camera.main;
        menuManagerObject = FindObjectOfType<MenuManager>();
        animalTransform = transform;
        playerDragonRB = GetComponent<Rigidbody>();
        //aimingLinePlayer = GetComponent<LineRenderer>();
        dirTouchPhaseBegun = false;
        //aimingLinePlayer.enabled = false;
        aimPoint = new Vector3();
        jumpPowerFloat = 0;
        animalIsGrounded = false;
        facingRight = true;
        playerDragonsLife = 60;
        transform.position = new Vector3(-15, 10, 0);

        playerDragonsLifeBarObject = menuManagerObject.playerDragonsLifeBarObject;
        playerDragonsLifeBarObject.gameObject.SetActive(true);

        playerDragonsLifeBar = playerDragonsLifeBarObject.GetComponentInChildren<Slider>();
        playerDragonsLifeBar.maxValue = playerDragonsLife;
        playerDragonsLifeBar.value = playerDragonsLife;

        playerDragonIsDied = false;
        winLoseControllerObject = FindObjectOfType<WinLoseController>();
        StartCoroutine(GetPropertiesOfRival(0.1f));

        Trajectory.TurnOnOffLineOfTrajectory(false);
    }

    IEnumerator GetPropertiesOfRival(float timer)
    {
        yield return new WaitForSeconds(timer);
        CPUDragon = GameObject.FindGameObjectWithTag("Enemy");
        CPUDragonRigidbody = CPUDragon.GetComponent<Rigidbody>();
        CPUDragonObject = CPUDragon.GetComponent<CPUDragonController>();
    }

    void animalAngleTwoardFly()
    {
        if (facingRight)
        {
            animalAngle = Mathf.Atan2(playerDragonRB.velocity.y, playerDragonRB.velocity.x) * Mathf.Rad2Deg;
            animalQuater = Quaternion.AngleAxis(animalAngle, new Vector3(0, 0, 90));
            playerDragonRB.transform.rotation = Quaternion.Slerp(playerDragonRB.transform.rotation, Quaternion.Euler(0, 0, animalAngle + 90), 1f);
        }
        else
        {
            animalAngle = Mathf.Atan2(playerDragonRB.velocity.y, playerDragonRB.velocity.x) * Mathf.Rad2Deg;
            animalQuater = Quaternion.AngleAxis(animalAngle, new Vector3(0, 0, 90));
            playerDragonRB.transform.rotation = Quaternion.Slerp(playerDragonRB.transform.rotation, Quaternion.Euler(0, 180, -animalAngle-90), 1f);
        }
    }

    //method to turn players animal forward and back depending where aminig line is now (behind or in front of animal) 
    private void flip()
    {
        if (animalTransform.position.x<aimPoint.x /*aimingLinePlayer.GetPosition(0).x< aimingLinePlayer.GetPosition(1).x*/ && !facingRight)
        {
            facingRight = true;
            animalTransform.rotation = Quaternion.Euler(0,0,90);
        }
        else if (animalTransform.position.x > aimPoint.x/*aimingLinePlayer.GetPosition(0).x > aimingLinePlayer.GetPosition(1).x*/ && facingRight)
        {
            facingRight = false;
            animalTransform.rotation = Quaternion.Euler(0, 180, 90);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(floorTag)) animalIsGrounded = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(enemyHornTag) && !CPUDragonObject.CPUDragonIsDied && !playerDragonIsDied)
        {
            if (CPUDragonRigidbody.velocity.magnitude > 20 && CPUDragonRigidbody.velocity.magnitude <= 40)
            {
                playerDragonsLife -= 5;
                playerDragonsLifeBar.value = playerDragonsLife;
            }
            else if (CPUDragonRigidbody.velocity.magnitude > 40)
            {
                playerDragonsLife -= 9;
                playerDragonsLifeBar.value = playerDragonsLife;
            }
            if (playerDragonsLife <= 0)
            {
                playerDragonsLifeBarObject.gameObject.SetActive(false);
                playerDragonIsDied = true;
                winLoseControllerObject.DetermineTheWinner(false);
            }
        }
    }

    IEnumerator cancelGroundedState(float timer) {
        yield return new WaitForSeconds(timer);
        animalIsGrounded = false;
    }

   

    // Update is called once per frame
    void Update()
    {
        if (TurnSwitchController.isPlayerTurn)
        {
            Vector3 jumpPower = (aimPoint - animalTransform.position) * jumpPowerFloat;
            if (Input.touchCount == 1 && !playerDragonIsDied)
            {
                Touch _touch = Input.GetTouch(0);
                if (_touch.phase == TouchPhase.Began)
                {
                    dirTouchPhaseBegun = true;
                    startPointOfLine = _touch.position;
                    Trajectory.TurnOnOffLineOfTrajectory(true);
                }
                if (_touch.phase == TouchPhase.Ended)
                {
                    dirTouchPhaseBegun = false;
                    //aimingLinePlayer.enabled = false;
                    playerDragonRB.AddForce(jumpPower, ForceMode.Impulse);
                    Trajectory.TurnOnOffLineOfTrajectory(false);
                    StartCoroutine(cancelGroundedState(0.1f));
                }
                endPointOfLine = _touch.position;
            }
            playerDragonsLifeBarObject.transform.position = mainCam.WorldToScreenPoint(transform.position);

            Trajectory.ShowTrajectoryOfPlayerDragon(animalTransform.position, jumpPower);
        }

        //Debug.Log(CPUDragonRigidbody.velocity.sqrMagnitude);
    }



    private void FixedUpdate()
    {
        if (dirTouchPhaseBegun)
        {
            //if (!aimingLinePlayer.enabled) aimingLinePlayer.enabled = true;
            burstDirection = (endPointOfLine - startPointOfLine)*0.1f;

            jumpPowerFloat = burstDirection.magnitude * MAGNITUDE_MULTIPLIER;

            aimPoint = animalTransform.position + new Vector3(animalTransform.position.x+burstDirection.x, animalTransform.position.y + burstDirection.y,0);

            //aimingLinePlayer.SetPosition(1, aimPoint);
            //aimingLinePlayer.SetPosition(0, animalTransform.position);

            if (animalIsGrounded) flip();

        }
        if (TurnSwitchController.isPlayerTurn) {
            if (!animalIsGrounded)
                animalAngleTwoardFly();
            else if (playerDragonRB.velocity == Vector3.zero)
            {
                if (animalTransform.rotation.y > 0) animalTransform.rotation = Quaternion.Euler(0, 180, 90);
                else animalTransform.rotation = Quaternion.Euler(0, 0, 90);
                TurnSwitchController.isPlayerTurn = false;
            }
        }
    }
}
