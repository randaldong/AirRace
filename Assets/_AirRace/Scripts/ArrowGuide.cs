using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowGuide : MonoBehaviour
{
	public WaypointsContainer waypointsContainer;
	public GameObject guideArrow; 
	[Range(0f, 1f)]
	public float arrowSensitivity = 0.05f;
	[Range(0f, 90f)]
	[Tooltip("The minimum angle in degrees between the forward direction and the direction to the arrow.")]
	public float minAngle = 5f;

	private bool shouldMove = false;
	private Vector3 attachDirection; // a nomalized vector pointing from camera to the arrow
	private GameObject nextWaypoint;

	private void Start()
	{
		attachDirection = guideArrow.transform.position - transform.position;
	}

	private void Update()
	{

		shouldMove = CheckShouldMove();
		if (shouldMove)
		{
			Vector3 targetPosition = GetTargetPosition();
			MoveArrowGradually(targetPosition);
		}
		RotateArrow();
	}

	private Vector3 GetTargetPosition()
	{
		return transform.position + attachDirection;
	}

	private void MoveArrowGradually(Vector3 targetPosition)
	{
		guideArrow.transform.position += (targetPosition - guideArrow.transform.position) * arrowSensitivity;
	}

	private void RotateArrow()
	{
		nextWaypoint = waypointsContainer.GetNextWaypoint();
		guideArrow.transform.forward = nextWaypoint.transform.position - guideArrow.transform.position;
	}

	private bool CheckShouldMove()
	{
		float angle = Vector3.Angle(attachDirection, transform.forward); // The angle in degrees between the two vectors
		return angle < minAngle;
	}
}
