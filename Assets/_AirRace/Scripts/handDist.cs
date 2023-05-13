using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using System;

public class handDist : MonoBehaviour
{
    private float nextActionTime = 0.0f;
    public float period = 0.1f;
    private float dist;
    /*float maxDist;
    float minDist;*/
    private float l_pinchDist;
    private float palmsDist;
    [SerializeField] private float accleration = 1.0f;
    [SerializeField] private float angularSpeed = 1.0f;
    [SerializeField] private GameObject toMove;
    [SerializeField] private GameObject enginePlayer;

    private AudioSource engineAudioSource;


    // Start is called before the first frame update
    //initial lize hand
    XRHandSubsystem p_subsystem;
    XRHand leftHand;
    XRHand rightHand;
    MetaAimHand gesture;
    //subsystem initialize
    static readonly List<XRHandSubsystem> s_SubsystemsReuse = new List<XRHandSubsystem>();
    XRHandSubsystem m_Subsystem;

    //joint pose
    Pose l_indexPos;
    Pose l_thumbPos;
    Pose l_palmPos;
    Pose l_indexTipPos;
    Pose l_littleTipPos;

    Pose rotateOrigin;
    Pose rotateTarget;
    Pose r_palmPos;


    //flag about movement
    Boolean lMoveTriggered = false;
    Boolean lpalmForword = true;
    Boolean l_pinched;
    Boolean claped;
    Boolean R_pinched_set = false;
    Boolean Gamestart;

    int cameraIndex;

    timmer timmerscript;
    countDownTimmer countDownScript;
    SwitchView switchViewScript;


    void Start()
    {
        //initialize timmer
        timmerscript = GameObject.FindGameObjectWithTag("Timmer").GetComponent<timmer>();
        countDownScript = GameObject.FindGameObjectWithTag("countDown").GetComponent<countDownTimmer>();
        switchViewScript = GameObject.FindGameObjectWithTag("cameraManager").GetComponent<SwitchView>();
        Gamestart = false;
        l_pinched = false;
        claped = false;
        cameraIndex = 1;//set cemera as nose

        engineAudioSource = enginePlayer.GetComponent<AudioSource>();
        engineAudioSource.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        //get hand input
        SubsystemManager.GetSubsystems(s_SubsystemsReuse);
        if (s_SubsystemsReuse.Count == 0)
            return;
        m_Subsystem = s_SubsystemsReuse[0];
        leftHand = m_Subsystem.leftHand;
        rightHand = m_Subsystem.rightHand;
        //MetaAimFlags flags = MetaAimFlags.Computed; 
        //lefthand gesture
        trackLeftJoint(leftHand);

        dist = Vector3.Distance(l_indexPos.position, l_thumbPos.position);

        //detect left  pinch gesture, when pinch allow movement on release
        l_pinchDist = Vector3.Distance(l_indexTipPos.position, l_thumbPos.position);
        //start game
        if (!Gamestart)
        {
            if (l_pinchDist < 0.015f && l_pinchDist != 0)
            {
                l_pinched = true;
            }
            else
            {
                if (l_pinched == true)
                {
                    l_pinched = false;
                    lMoveTriggered = !lMoveTriggered;
                    //timmerscript.startTimmer();
                    countDownScript.startCountDown();
                    engineAudioSource.Play();
                    Gamestart = true;
                }
            }
        }
      
		//determing moving direction
		if (l_palmPos.rotation.eulerAngles.z < 270f && l_palmPos.rotation.eulerAngles.z > 90f)
		{
			lpalmForword = false;
		}
		else
		{
			lpalmForword = true;
		}
		//right hand 
		//call movement
		if (lMoveTriggered)
		{
			moveObject(dist, toMove);
		}

		//rotate the game object by certain degree.
		rotateAngle(rightHand);

		//clap gesture
		palmsDist = Vector3.Distance(l_palmPos.position, r_palmPos.position);
		if (palmsDist < 0.08f)
		{
			claped = true;
		}
		else
		{
			if (claped == true)
			{
				cameraIndex++;
				if (cameraIndex > 3)
				{
					cameraIndex = 1;
				}
				switchCamera(cameraIndex);
				claped = false;
			}
		}

		//update every time second
		if (Time.time > nextActionTime)
		{
			nextActionTime += period;
			/*Debug.Log(palmsDist);
			Debug.Log(claped);*/
		}
		
    }
    void trackLeftJoint(XRHand hand)
    {
        XRHandJoint index = hand.GetJoint(XRHandJointID.IndexIntermediate);
        XRHandJoint thumbTip = hand.GetJoint(XRHandJointID.ThumbTip);
        XRHandJoint palm = hand.GetJoint(XRHandJointID.Palm);
        XRHandJoint indexTip = hand.GetJoint(XRHandJointID.IndexTip);
        XRHandJoint little = hand.GetJoint(XRHandJointID.LittleTip);

        index.TryGetPose(out Pose indpose);
        thumbTip.TryGetPose(out Pose thpose);
        palm.TryGetPose(out Pose palmpose);
        indexTip.TryGetPose(out Pose indexTipPos);
        little.TryGetPose(out Pose littleTipPos);

        l_indexPos = indpose;
        l_thumbPos = thpose;
        l_palmPos = palmpose;
        l_indexTipPos = indexTipPos;
        l_littleTipPos = littleTipPos;
    }

    void moveObject(float dist, GameObject toMove)
    {
        //use 0.025-0.075 to map the speed. 0.075 speed is 0, 0.025 is 1
        if (dist < 0.025f)
            dist = .025f;
        if (dist > 0.075f)
            dist = .075f;
        float speedFactor = 0.075f - dist;

        //play audio when speed factor is not 0
        engineAudioSource.volume = (speedFactor / 0.05f);

        float speed = speedFactor * accleration;
        if (!lpalmForword)
            speed = -speed;
        //Debug.Log(speed);
        toMove.transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
    }

    Vector2 rotateAngle(XRHand r_hand)
    {

        XRHandJoint r_index = r_hand.GetJoint(XRHandJointID.IndexTip);
        XRHandJoint r_thumb = r_hand.GetJoint(XRHandJointID.ThumbTip);
        XRHandJoint r_palm = r_hand.GetJoint(XRHandJointID.Palm);
        r_index.TryGetPose(out Pose indexpos);
        r_thumb.TryGetPose(out Pose thumbPos);
        r_palm.TryGetPose(out Pose palmPos);
        r_palmPos = palmPos;

        float r_pinchDist = Vector3.Distance(indexpos.position, thumbPos.position);
        Vector2 angle;
        //Debug.Log(r_pinchDist);
        //if pinched
        if (r_pinchDist < 0.015f)
        {
            if (!R_pinched_set)
            {
                rotateOrigin = thumbPos;
                R_pinched_set = true;
                //Debug.Log(thumbPos.position);
            }
            rotateTarget = thumbPos;

            angle = new Vector2(rotateTarget.position.x - rotateOrigin.position.x, rotateTarget.position.y - rotateOrigin.position.y);
            rotateObj(angle);
        }

        else
        {
            R_pinched_set = false;
            angle = Vector2.zero;
        }
        return angle;
    }

    //rotate help method
    void rotateObj(Vector2 angle)
    {
        //x angle mapping
        Quaternion xrotationIncrement = Quaternion.AngleAxis(angle.x * angularSpeed, Vector3.up);
        //y angle mapping
        Quaternion yrotationIncrement = Quaternion.AngleAxis(angle.y * angularSpeed, Vector3.left);
        toMove.transform.rotation = toMove.transform.rotation * xrotationIncrement * yrotationIncrement;
        // restore z rotation
        Vector3 eulers = toMove.transform.eulerAngles;
        toMove.transform.rotation = Quaternion.Euler(eulers.x, eulers.y, 0f);

    }

    void switchCamera( int index)
    {
        //use cockpit camera
        if (index == 1) 
        {
            switchViewScript.Switch2Nose();
        }
        //use tail camera
        if (index == 2)
        {
            switchViewScript.Switch2Cockpit();
        }
        //use nose camera
        if(index == 3)
        {
            switchViewScript.Switch2Tail();
        }
    }
    // enablemovement
    public void enableMove()
    {
        lMoveTriggered = true;
    }
    public void disableMove()
    {
        lMoveTriggered = false;
    }

}
