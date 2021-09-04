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
    [HideInInspector]
    public bool animalIsGrounded;
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

    private byte PlayerDragonStartedItsMovement; //byte is used to reduce the memory 0 is false 1 is true
    //private TurnSwitchController switchController;

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
        PlayerDragonStartedItsMovement = 0;
        Trajectory.TurnOnOffLineOfTrajectory(false);
        GetComponent<MeshRenderer>().material.color = Color.green;
        transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.green;
        //switchController = FindObjectOfType<TurnSwitchController>(); 
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
        PlayerDragonStartedItsMovement = 1;
    }

    private void setTheDragonToLegsAfterGrounding()
    {
        if (animalTransform.rotation.y > 0) animalTransform.rotation = Quaternion.Euler(0, 180, 90);
        else animalTransform.rotation = Quaternion.Euler(0, 0, 90);
    }

    // Update is called once per frame
    void Update()
    {
        if (TurnSwitchController.isPlayerTurn /*&& !PlayerDragonStartedItsMovement*/)
        {
            //Vector3 jumpPower = (aimPoint - animalTransform.position) * jumpPowerFloat;

            Vector3 jumpPower = Vector3.ClampMagnitude((aimPoint - animalTransform.position) * jumpPowerFloat,100);
            if (Input.touchCount == 1 && !playerDragonIsDied && playerDragonRB.velocity == Vector3.zero)
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
            Trajectory.ShowTrajectoryOfPlayerDragon(animalTransform.position, jumpPower);
        }

        playerDragonsLifeBarObject.transform.position = mainCam.WorldToScreenPoint(transform.position);
        //Debug.Log(CPUDragonRigidbody.velocity.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        if (dirTouchPhaseBegun)
        {
            //if (!aimingLinePlayer.enabled) aimingLinePlayer.enabled = true;
            burstDirection = (endPointOfLine - startPointOfLine) * 0.1f;

            jumpPowerFloat = burstDirection.magnitude * MAGNITUDE_MULTIPLIER;

            aimPoint = animalTransform.position + new Vector3(animalTransform.position.x + burstDirection.x, animalTransform.position.y + burstDirection.y, 0);

            //aimingLinePlayer.SetPosition(1, aimPoint);
            //aimingLinePlayer.SetPosition(0, animalTransform.position);

            if (animalIsGrounded) flip();

        }

        if (!animalIsGrounded)
            animalAngleTwoardFly();
        else if (playerDragonRB.velocity == Vector3.zero && PlayerDragonStartedItsMovement == 1 && TurnSwitchController.isPlayerTurn)
        {
            TurnSwitchController.isPlayerTurn = false;
            PlayerDragonStartedItsMovement = 0;
        }

        if (playerDragonRB.velocity == Vector3.zero && (animalTransform.rotation.eulerAngles.y != 0 || animalTransform.rotation.eulerAngles.y != 180)) setTheDragonToLegsAfterGrounding();
    }
}
