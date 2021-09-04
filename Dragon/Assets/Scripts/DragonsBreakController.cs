using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonsBreakController : MonoBehaviour
{
    public Rigidbody CPUDragonRB;
    public Rigidbody PlayerDragonRB;

    private Transform CPUDragonTransform;
    private Transform PlayerDragonTransform;

    private ParticleSystem preBurstEffect;
    public ParticleSystem burstEffect;

    private CPUDragonController CPUDragonObject;
    private PlayerDragonController PlayerDragonObject;

    private bool breakTimerIsStarted;

    private void breakTheDragons() {
        Vector3 baseDirectionOfFly = (CPUDragonTransform.position - PlayerDragonTransform.position)*30;
        Vector3 CPUDragonFlyDirection = new Vector3(baseDirectionOfFly.x, baseDirectionOfFly.y*Random.Range(2,7), baseDirectionOfFly.z);
        Vector3 PlayerDragonFlyDirection = new Vector3(baseDirectionOfFly.x*-1, baseDirectionOfFly.y * Random.Range(2, 7), baseDirectionOfFly.z);
        CPUDragonRB.AddForce(CPUDragonFlyDirection,ForceMode.Impulse);
        PlayerDragonRB.AddForce(PlayerDragonFlyDirection, ForceMode.Impulse);
    }

    private void stopBurstProcess() {
        StopCoroutine(CountTheBreakBurst());
        breakTimerIsStarted = false;
        preBurstEffect.Stop();
        burstEffect.Stop();
    }

    private void Start()
    {
        CPUDragonTransform = CPUDragonRB.transform;
        PlayerDragonTransform = PlayerDragonRB.transform;
        PlayerDragonObject = PlayerDragonRB.GetComponent<PlayerDragonController>();
        CPUDragonObject = CPUDragonRB.GetComponent<CPUDragonController>();
        preBurstEffect = GetComponent<ParticleSystem>();
    }

    private IEnumerator CountTheBreakBurst() {
        transform.position = PlayerDragonTransform.position + (CPUDragonTransform.position - PlayerDragonTransform.position) / 2;
        breakTimerIsStarted = true;
        preBurstEffect.Play();
        yield return new WaitForSeconds(3f);
        preBurstEffect.Stop();
        if (Vector3.Distance(CPUDragonTransform.position, PlayerDragonTransform.position) < 3)
        {
            burstEffect.transform.position = transform.position;
            burstEffect.Play();
            breakTheDragons();
        }
        yield return new WaitForSeconds(1f);
        breakTimerIsStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(CPUDragonTransform.position, PlayerDragonTransform.position) < 3 && !PlayerDragonObject.playerDragonIsDied && !CPUDragonObject.CPUDragonIsDied && !breakTimerIsStarted 
           /* && CPUDragonObject.animalIsGrounded && PlayerDragonObject.animalIsGrounded*/) {
            StartCoroutine(CountTheBreakBurst());
        }
        if (Vector3.Distance(CPUDragonTransform.position, PlayerDragonTransform.position) > 3 && breakTimerIsStarted) {
            stopBurstProcess();
        }
    }
}
