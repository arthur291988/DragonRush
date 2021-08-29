using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDragonController : MonoBehaviour
{
    private Rigidbody playerDragonRB;
    private LineRenderer aimingLinePlayer;
    private bool dirTouchPhaseBegun;

    private Camera mainCam;

    private GameObject CPUDragon;
    private Rigidbody CPUDragonRigidbody;
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
    [SerializeField]
    private Slider playerDragonsLifeBar;
    [SerializeField]
    private RawImage playerDragonsLifeBarObject;

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
        if (aimingLinePlayer.GetPosition(0).x< aimingLinePlayer.GetPosition(1).x && !facingRight)
        {
            facingRight = true;
            animalTransform.rotation = Quaternion.Euler(0,0,90);
        }
        else if (aimingLinePlayer.GetPosition(0).x > aimingLinePlayer.GetPosition(1).x && facingRight)
        {
            facingRight = false;
            animalTransform.rotation = Quaternion.Euler(0, 180, 90);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(floorTag)) animalIsGrounded = true;
        //if (collision.gameObject.CompareTag(enemyHornTag))
        //{
        //    Debug.Log("HeeeeeeyPlayer");
        //    if (CPUDragonRigidbody.velocity.magnitude > 20 && CPUDragonRigidbody.velocity.magnitude <= 40)
        //    {
        //        playerDragonsLife -= 5;
        //        playerDragonsLifeBar.value = playerDragonsLife;
        //    }
        //    else if (CPUDragonRigidbody.velocity.magnitude > 40)
        //    {
        //        playerDragonsLife -= 9;
        //        playerDragonsLifeBar.value = playerDragonsLife;
        //    }
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(enemyHornTag))
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
            if (playerDragonsLife <= 0) playerDragonsLifeBarObject.gameObject.SetActive(false);
        }
    }

    IEnumerator cancelGroundedState(float timer) {
        yield return new WaitForSeconds(timer);
        animalIsGrounded = false;
    }

    private void OnEnable()
    {
        mainCam = Camera.main;
        animalTransform = transform;
        playerDragonRB = GetComponent<Rigidbody>();
        aimingLinePlayer = GetComponent<LineRenderer>();
        aimPoint = new Vector3();
        jumpPowerFloat = 0;
        animalIsGrounded = false;
        facingRight = true;
        playerDragonsLife = 60;
        playerDragonsLifeBar.maxValue = playerDragonsLife;
        playerDragonsLifeBar.value = playerDragonsLife;

        StartCoroutine(GetRigidbodyOfRival(0.1f));
    }
    IEnumerator GetRigidbodyOfRival(float timer)
    {
        yield return new WaitForSeconds(timer);
        CPUDragon = GameObject.FindGameObjectWithTag("Enemy");
        CPUDragonRigidbody = CPUDragon.GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1 )
        {
            Touch _touch = Input.GetTouch(0);
            if (_touch.phase == TouchPhase.Began)
            {
                dirTouchPhaseBegun = true;
                startPointOfLine = _touch.position;
            }
            if (_touch.phase == TouchPhase.Ended)
            {
                dirTouchPhaseBegun = false;
                aimingLinePlayer.enabled = false;

                playerDragonRB.AddForce((aimPoint- animalTransform.position) * jumpPowerFloat, ForceMode.Impulse);

                StartCoroutine(cancelGroundedState(0.1f));
            }
            endPointOfLine = _touch.position;
        }
        playerDragonsLifeBarObject.transform.position = mainCam.WorldToScreenPoint(transform.position);

        //Debug.Log(CPUDragonRigidbody.velocity.sqrMagnitude);
    }



    private void FixedUpdate()
    {
        if (dirTouchPhaseBegun)
        {
            aimingLinePlayer.enabled = true;
            burstDirection = (endPointOfLine - startPointOfLine)*0.1f;

            jumpPowerFloat = burstDirection.magnitude * MAGNITUDE_MULTIPLIER;

            aimPoint = animalTransform.position + new Vector3(animalTransform.position.x+burstDirection.x, animalTransform.position.y + burstDirection.y,0);

            aimingLinePlayer.SetPosition(1, aimPoint);
            aimingLinePlayer.SetPosition(0, animalTransform.position);

            if (animalIsGrounded) flip();

        }
        if (!animalIsGrounded)
            animalAngleTwoardFly();
        else if (playerDragonRB.velocity == Vector3.zero)
        {
            if(animalTransform.rotation.y>0) animalTransform.rotation = Quaternion.Euler(0, 180, 90);
            else animalTransform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }
}
