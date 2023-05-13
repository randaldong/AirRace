using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public WaypointsContainer waypointsContainer;
    public GameObject player;
    private countDownTimmer countDownTimer;
    public timmer globalTimer;
    public handDist handControl;
    [SerializeField] GameObject crashSound;
    [SerializeField] GameObject checkpointSound;
    [SerializeField] GameObject finalSound;
    [SerializeField] GameObject targetSound;
    [SerializeField] GameObject arrow;
    AudioSource crash;
    AudioSource checkpoint;
    AudioSource final;
    AudioSource target;

    private float timeWaited = 0f;
    private bool isCollideTargetWaypoint;
	private bool isCollideWrongWaypoint;
	private Vector3 lastWaypointPos;
    private Vector3 nextWaypointPos;
    private bool isMoveDisabled;

    private void Start()
    {
        countDownTimer = GameObject.FindGameObjectWithTag("countDown").GetComponent<countDownTimmer>();
        crash = crashSound.GetComponent<AudioSource>();
        checkpoint = checkpointSound.GetComponent<AudioSource>();
        final = finalSound.GetComponent<AudioSource>();
        target = targetSound.GetComponent<AudioSource>();
    }
    void OnTriggerEnter(Collider other)
	{
        if (other.gameObject.tag == "RoadSign")
        {
            other.gameObject.SetActive(false);
        }
        else
        {
            CheckReachTargetWaypoint(other.transform.parent);
			if (isCollideTargetWaypoint)
			{
				lastWaypointPos = other.transform.parent.position;
				waypointsContainer.DeletePassedWaypoint();
				other.transform.parent.gameObject.SetActive(false);

                if (waypointsContainer.waypoints.Count != 0)
                {
                    nextWaypointPos = waypointsContainer.NextPointPos();
                    targetSound.transform.position = nextWaypointPos;
                    checkpoint.Play();
                }
                    ;
                if (waypointsContainer.waypoints.Count == 0)
				{
                    final.Play();
                    arrow.SetActive(false);
					globalTimer.FreezeTimmer();
                    target.Stop();
				}
			}
			else if (!isCollideWrongWaypoint)
			{
				countDownTimer.startCountDown();
				TeleportToLastWaypoint();
				StartFreeze();
			}
		}
        isCollideTargetWaypoint = false;
        isCollideWrongWaypoint = false;
    }

	void TeleportToLastWaypoint()
    {
        player.transform.position = lastWaypointPos;
        player.transform.forward = waypointsContainer.GetNextWaypoint().transform.position - lastWaypointPos;
        crash.Play();
	}

    void StartFreeze()
    {
        timeWaited += Time.deltaTime;

        if (timeWaited < countDownTimer.time && isMoveDisabled == false)
        {
            handControl.disableMove();
            isMoveDisabled = true;
		}
        else if (timeWaited > countDownTimer.time)
        {
			handControl.enableMove();
			isMoveDisabled = false;
            timeWaited = 0f;
		}
	}

	void CheckReachTargetWaypoint(Transform collideTransform)
    {
        if (collideTransform.gameObject.tag == "Waypoint")
        {
            float dist2TargetWaypoint = Vector3.Distance(waypointsContainer.GetNextWaypoint().transform.position, collideTransform.position);
            isCollideTargetWaypoint = dist2TargetWaypoint <= 0.01f;
            isCollideWrongWaypoint = !isCollideTargetWaypoint;
            //checkpoint.Play();
		}
    }
}
