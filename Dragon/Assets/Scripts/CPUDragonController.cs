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
    [HideInInspector]
    public bool animalIsGrounded;
    //is used to correctly flip animal direction
    private bool facingRight;
    private Transform CPUDragonTransform; //to cash the transform 
    private Transform playerDragonTransform; //to cash the transform 
    [HideInInspector]
    public int CPUDragonsLife;

    private Slider CPUDragonsLifeBar;
    private RawImage CPUDragonsLifeBarObject;

    private WinLoseController winLoseControllerObject;
    private MenuManager menuManagerObject;

    private bool enemyTurnIsStarted = false; //bool is used to trigger the start and end the attack phases of enemys animal
    [HideInInspector]
    public bool CPUDragonIsDied = false; //bool is used to trigger the start and end the attack phases of enemys animal

    private byte CPUDragonStartedItsMovement; //byte is used to reduce the memory 0 is false 1 is true

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
        CPUDragonStartedItsMovement = 0;

        GetComponent<MeshRenderer>().material.color = Color.yellow;
        transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.yellow;
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
        CPUDragonStartedItsMovement = 1;
    }
    //method to determine enemys jump direction vectors
    public void enemyMovementsController()
    {
        pointAOfCPU = CPUDragonTransform.position;
        pointBOfCPU = new Vector3(playerDragonTransform.position.x, playerDragonTransform.position.y + Random.Range(5, 20), playerDragonTransform.position.z);
        enemyTurnIsStarted = true;
    }

    private void setTheDragonToLegsAfterGrounding()
    {
        if (CPUDragonTransform.rotation.y > 0) CPUDragonTransform.rotation = Quaternion.Euler(0, 180, 90);
        else CPUDragonTransform.rotation = Quaternion.Euler(0, 0, 90);
    }

    public float AngleBalistic(float distance, float speedBullet)
    {
        //Находим велечину гравитации
        float gravity = Physics.gravity.magnitude;

        float discr = Mathf.Pow(speedBullet, 4) - 4 * (-gravity * gravity / 4) * (-distance * distance);
        //Время полёта
        float t = ((-speedBullet * speedBullet) - Mathf.Sqrt(discr)) / (-gravity * gravity / 2);
        t = Mathf.Sqrt(t);
        float th = gravity * t * t / 8;
        //Угол пушки
        float angle = 180 * (Mathf.Atan(4 * th / distance) / Mathf.PI);

        //Возрощаем угол
        return (angle);
    }

    private Vector3 calculateBestThrowSpeed(Vector3 origin, Vector3 target, float timeToTarget)
    {
        // calculate vectors
        Vector3 toTarget = target - origin;
        Vector3 toTargetXZ = toTarget;
        toTargetXZ.y = 0;

        // calculate xz and y
        float y = toTarget.y;
        float xz = toTargetXZ.magnitude;

        // calculate starting speeds for xz and y. Physics forumulase deltaX = v0 * t + 1/2 * a * t * t
        // where a is "-gravity" but only on the y plane, and a is 0 in xz plane.
        // so xz = v0xz * t => v0xz = xz / t
        // and y = v0y * t - 1/2 * gravity * t * t => v0y * t = y + 1/2 * gravity * t * t => v0y = y / t + 1/2 * gravity * t
        float t = timeToTarget;
        float v0y = y / t + 0.5f * Physics.gravity.magnitude * t;
        float v0xz = xz / t;

        // create result vector for calculated starting speeds
        Vector3 result = toTargetXZ.normalized;   // get direction of xz but with magnitude 1
        result *= v0xz;                    // set magnitude of xz to v0xz (starting speed in xz plane)
        result.y = v0y;                    // set y to v0y (starting speed of y plane)

        return result;
    }

    // Update is called once per frame
    void Update()
    {
        if (!CPUDragonIsDied && !TurnSwitchController.isPlayerTurn)
        {
            if (CPUDragonRB.velocity == Vector3.zero && !playerDragonObject.playerDragonIsDied && CPUDragonStartedItsMovement == 0)
            {
                enemyMovementsController();
            }
            if (animalIsGrounded) enemyFlip();
        }
        CPUDragonsLifeBarObject.transform.position = mainCam.WorldToScreenPoint(transform.position);
    }

    private void FixedUpdate()
    {
        if (!CPUDragonIsDied )
        {
            if (enemyTurnIsStarted)
            {
                burstDirection = calculateBestThrowSpeed(pointAOfCPU, pointBOfCPU, 2);
                //burstDirection = Vector3.ClampMagnitude(pointBOfCPU - pointAOfCPU, 20)* AngleBalistic((pointBOfCPU - pointAOfCPU).sqrMagnitude, Vector3.ClampMagnitude(pointBOfCPU - pointAOfCPU, 20).magnitude) * Random.Range(1.1f, 3.4f);
                //burstDirection = Vector3.ClampMagnitude(pointBOfCPU - pointAOfCPU, 20)*Random.Range(1.1f, 3.4f);
                CPUDragonRB.AddForce(burstDirection , ForceMode.Impulse);
                enemyTurnIsStarted = false;
                StartCoroutine(cancelGroundedState(0.1f));
            }

            if (!animalIsGrounded)
            {
                animalAngleTwoardFly();
            }
            else if (CPUDragonRB.velocity == Vector3.zero && CPUDragonStartedItsMovement == 1 && !TurnSwitchController.isPlayerTurn)
            {
                TurnSwitchController.isPlayerTurn = true;
                CPUDragonStartedItsMovement = 0;
            }

            if (CPUDragonRB.velocity == Vector3.zero && (CPUDragonTransform.rotation.eulerAngles.y != 0 || CPUDragonTransform.rotation.eulerAngles.y != 180)) setTheDragonToLegsAfterGrounding();
        }
    }
}
