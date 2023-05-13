using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using PathCreation;
using UnityEngine.UIElements;

public class TrackGenerator : MonoBehaviour
{
	public GameObject waypoinsContainer;
	public TextAsset file;
	[Range(0.01f, 1f)]
	public float curvature = 0.01f;
	public bool closedLoop;
	public GameObject waypointPrefab;
	public GameObject roadSignPrefab;
	public int roadSignNumber;
	public GameObject player;
	private List<Vector3> waypointPositions;
	private List<Vector3> roadSignPositions;
	private List<Quaternion> roadSignRotations;

	private PathCreator pathCreator;

	void Start()
	{
		waypointPositions = new List<Vector3>();
		roadSignPositions = new List<Vector3>();
		roadSignRotations = new List<Quaternion>();
		pathCreator = GetComponent<PathCreator>();
		ParseFile();
		GenerateWaypoints();
		GeneratePath();
		GenerateRoadSigns();
		InitPlayer();
	}

	void ParseFile()
	{
		float ScaleFactor = 0.0254f;
		string content = file.ToString();
		string[] lines = content.Split('\n');
		for (int i = 0; i < lines.Length; i++)
		{
			string[] coords = lines[i].Split(' ');
			Vector3 pos = new Vector3(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2]));
			waypointPositions.Add(pos * ScaleFactor);
		}
	}

	void GenerateWaypoints()
	{
		Vector3 directionVector = waypointPositions[1] - waypointPositions[0];
		directionVector.y = 0;
		for (int i = 0; i < waypointPositions.Count; i++)
		{
			if (i > 0)
			{
				directionVector = waypointPositions[i] - waypointPositions[i - 1];
			}
			GameObject waypoint = Instantiate(waypointPrefab, waypointPositions[i], Quaternion.LookRotation(directionVector.normalized));
			waypoint.transform.parent = waypoinsContainer.transform;
			waypoinsContainer.GetComponent<WaypointsContainer>().AddWaypoint(waypoint);
			if (i <= 8)
			{
				waypoint.transform.Find("number").GetComponent<SimpleHelvetica>().Text = "0" + (i + 1).ToString();
				waypoint.transform.Find("number").GetComponent<SimpleHelvetica>().GenerateText();
				waypoint.transform.Find("number").GetComponent<SimpleHelvetica>().ApplyMeshRenderer();
			}
			else
			{
				// the number of waypoints will < 100
				waypoint.transform.Find("number").GetComponent<SimpleHelvetica>().Text = (i + 1).ToString();
				waypoint.transform.Find("number").GetComponent<SimpleHelvetica>().CharacterSpacing = -1;
				waypoint.transform.Find("number").GetComponent<SimpleHelvetica>().GenerateText();
				waypoint.transform.Find("number").GetComponent<SimpleHelvetica>().ApplyMeshRenderer();
			}
		}
	}
	void GeneratePath()
	{
		
		BezierPath bezierPath = new BezierPath(waypointPositions, closedLoop, PathSpace.xyz);
        bezierPath.AutoControlLength = curvature; // set the curve to straightline
        GetComponent<PathCreator>().bezierPath = bezierPath;
	}

	void GenerateRoadSigns()
	{
		float delta = 1.0f / (roadSignNumber + 1);
		for (int i = 0; i < roadSignNumber; i++)
		{
			roadSignPositions.Add(pathCreator.path.GetPointAtTime(i * delta));
			roadSignRotations.Add(pathCreator.path.GetRotation(i * delta));
			GameObject roadSign = Instantiate(roadSignPrefab, roadSignPositions[i], roadSignRotations[i]);
			roadSign.transform.parent = transform;
		}

	}

	void InitPlayer()
	{
		player.transform.position = waypointPositions[0];
		player.transform.forward = pathCreator.path.GetDirection(0);
	}
}
