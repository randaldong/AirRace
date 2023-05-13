using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class SwitchView : MonoBehaviour
{
	[SerializeField] private GameObject aircraft;
	[SerializeField] private Transform cockpitView;
	[SerializeField] private Transform tailView;
	[SerializeField] private Transform noseView;

	// Start is called before the first frame update
	void Start()
    {
        Switch2Nose(); // cockpit view by default
	}

    public void Switch2Cockpit()
    {
		aircraft.transform.position = cockpitView.position;
	}

	public void Switch2Tail()
    {
		aircraft.transform.position = tailView.position;
	}

	public void Switch2Nose()
    {
		aircraft.transform.position = noseView.position;
	}
}
