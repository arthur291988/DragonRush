using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CPUDragonController : MonoBehaviour
{
    private Rigidbody CPUDragonRB;

    private GameObject PlayerDragon; 
    private Rigidbody PlayerDragonRigidbody;
    private PlayerDragonController playerDragonObject;

    //this coordinates are used to estimate the direction of jump of enemys animal
    private Vector3 pointAOfCPU;
    private Vector3 pointBOfCPU;
    private Vector3 burstDirection; //vector to hold the CPU animal jump direction

    private Camera mainCam;

    private Vector3 aimPoint; //point is used to show the direction of players jump and power of jump

    private string floorTag = "Floor";
    private string playerHornTag = "PlayerHorn";
    //private float jumpPowerFloat;
    public const float MAGNITUDE_MULTIPLIER = 0.05f;
    private float animalAngle;
    private Quaternion animalQuater;
    private bool animalIsGrounded;
    //is used to correctly flip animal direction
    private bool facingRight;
    private Transform CPUDragonTransform; //to cash the transform 
    private Transform playerDragonTransform; //to cash the transform 
    [HideInInspector]
    public int CPUDragonsLife;

    private bool CPUFinishedItsTurn;

    private Slider CPUDragonsLifeBar;
    private RawImage CPUDragonsLifeBarObject;

    private WinLoseController winLoseControllerObject;
    private MenuManager menuManagerObject;

    private bool enemyTurnIsStarted = false; //bool is used to trigger the start and end the attack phases of enemys animal
    [HideInInspector]
    public bool CPUDragonIsDied = false; //bool is used to trigger the start and end the attack phases of enemys animal

    private void OnEnable()
    {
        mainCam = Camera.main;
        menuManagerObject = FindObjectOfType<MenuManager>();
        animalIsGrounded = false;
        CPUDragonRB = GetComponent<Rigidbody>();
        CPUDragonTransform = gameObject.transform;
        CPUDragonsLife = 60;
        transform.position = new Vector3(15,10,0);

        CPUDragonsLifeBarObject = menuManagerObject.CPUDragonsLifeBarObject;
        CPUDragonsLifeBarObject.gameObject.SetActive(true);

        CPUDragonsLifeBar = CPUDragonsLifeBarObject.GetComponentInChildren<Slider>();
        CPUDragonsLifeBar.maxValue = CPUDragonsLife;
        CPUDragonsLifeBar.value = CPUDragonsLife;

        winLoseControllerObject = FindObjectOfType<WinLoseController>();
        StartCoroutine(GetPropertiesOfRival(0.1f));
        CPUDragonIsDied = false;
    }
    IEnumerator GetPropertiesOfRival(float timer)
    {
        yield return new WaitForSeconds(timer);
        PlayerDragon = GameObject.FindGameObjectWithTag("Player");
        playerDragonTransform = PlayerDragon.transform;
        PlayerDragonRigidbody = PlayerDragon.GetComponent<Rigidbody>();
        playerDragonObject = PlayerDragon.GetComponent<PlayerDragonController>();
    }
    private void enemyFlip()
    {
        if (playerDragonTransform.position.x > CPUDragonTransform.position.x && facingRight == false)
        {
            facingRight = true;
            CPUDragonTransform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (playerDragonTransform.position.x < CPUDragonTransform.position.x && facingRight == true)
        {
            facingRight = false;
            CPUDragonTransform.rotation = Quaternion.Euler(0, 180, 90);
        }
    }

    //the method to set the angle so it is directed toward flying direction
    void animalAngleTwoardFly()
    {
        if (facingRight)
        {
            animalAngle = Mathf.Atan2(CPUDragonRB.velocity.y, CPUDragonRB.velocity.x) * Mathf.Rad2Deg;
            animalQuater = Quaternion.AngleAxis(animalAngle, new Vector3(0, 0, 90));
            CPUDragonRB.transform.rotation = Quaternion.Slerp(CPUDragonRB.transform.rotation, Quaternion.Euler(0, 0, animalAngle + 90), 1f);
        }
        else
        {
            animalAngle = Mathf.Atan2(CPUDragonRB.velocity.y, CPUDragonRB.velocity.x) * Mathf.Rad2Deg;
            animalQuater = Quaternion.AngleAxis(animalAngle, new Vector3(0, 0, 90));
            CPUDragonRB.transform.rotation = Quaternion.Slerp(CPUDragonRB.transform.rotation, Quaternion.Euler(0, 180, -animalAngle - 90), 1f);
        }
    }

    //method to determine enemys jump direction vectors
    public void enemyMovementsController()
    {
        pointAOfCPU = CPUDragonTransform.position;
        pointBOfCPU = new Vector3(playerDragonTransform.position.x, playerDragonTransform.position.y + Random.Range(5, 20), playerDragonTransform.position.z);
        enemyTurnIsStarted = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(floorTag)) animalIsGrounded = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerHornTag) && !playerDragonObject.playerDragonIsDied &&!CPUDragonIsDied)
        {
            if (PlayerDragonRigidbody.velocity.magnitude > 20 && PlayerDragonRigidbody.velocity.magnitude <= 40)
            {
                CPUDragonsLife -= 5;
                CPUDragonsLifeBar.value = CPUDragonsLife;
            }
            else if (PlayerDragonRigidbody.velocity.magnitude > 40)
            {
                CPUDragonsLife -= 9;
                CPUDragonsLifeBar.value = CPUDragonsLife;
            }
            if (CPUDragonsLife <= 0)
            {
                CPUDragonsLifeBarObject.gameObject.SetActive(false);
                CPUDragonIsDied = true;
                winLoseControllerObject.DetermineTheWinner(true);
            }
        }
    }

    IEnumerator cancelGroundedState(float timer)
    {
        yield return new WaitForSeconds(timer);
        animalIsGrounded = false;
    }

    private void finishTheMovementOfCPU() {
        CPUFinishedItsTurn = true;
        if (CPUDragonTransform.rotation.y > 0) CPUDragonTransform.rotation = Quaternion.Euler(0, 180, 90);
        else CPUDragonTransform.rotation = Quaternion.Euler(0, 0, 90);
    }

    // Update is called once per frame
    void Update()
    {
        if (!CPUDragonIsDied && !TurnSwitchController.isPlayerTurn)
        {
            if (CPUFinishedItsTurn && !playerDragonObject.playerDragonIsDied)
            {
                enemyMovementsController();
                CPUFinishedItsTurn = false;
            }
            if (animalIsGrounded) enemyFlip();
            CPUDragonsLifeBarObject.transform.position = mainCam.WorldToScreenPoint(transform.position);
            //Debug.Log(PlayerDragonRigidbody.velocity.magnitude);
        }
    }

    private void FixedUpdate()
    {
        if (!CPUDragonIsDied && !TurnSwitchController.isPlayerTurn)
        {
            if (enemyTurnIsStarted)
            {
                burstDirection = Vector3.ClampMagnitude(pointBOfCPU - pointAOfCPU, 50);
                CPUDragonRB.AddForce(burstDirection * Random.Range(1.1f, 3.4f), ForceMode.Impulse);
                enemyTurnIsStarted = false;
                StartCoroutine(cancelGroundedState(0.1f));
            }

            if (!animalIsGrounded)
            {
                animalAngleTwoardFly();
            }
            else if (CPUDragonRB.velocity == Vector3.zero)
            {
                TurnSwitchController.isPlayerTurn = true;
                finishTheMovementOfCPU();
            }
        }

    }
}
