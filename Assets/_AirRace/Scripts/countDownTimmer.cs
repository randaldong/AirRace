using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class countDownTimmer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public float time = 3.0f;
    [SerializeField] public bool activited = false;
    [SerializeField] public Text timeText;
    private bool gamestart = false;
    
    timmer timmerscript;
    handDist handScript;
    // Start is called before the first frame update
    void Start()
    {
        timmerscript = GameObject.FindGameObjectWithTag("Timmer").GetComponent<timmer>();
        handScript = GameObject.FindGameObjectWithTag("handRecg").GetComponent<handDist>();
    }

    // Update is called once per frame
    void Update()
    {
        if (activited)
        {

            time -= Time.deltaTime;
            DisplayTime(time);
            handScript.disableMove();
            if (time < 0.0f) {
                restore();
                //if haven't start game, start timmer
                if (!gamestart)
                {
                    timmerscript.StartTimmer();
                    gamestart = true;
                }
                handScript.enableMove();
            }
        }
    }

    void DisplayTime(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float second = Mathf.CeilToInt(time % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, second);
    }

    public void startCountDown()
    {
        activited = true;
	}

    void restore()
    {
        activited = false;
        time = 3.0f;
        timeText.text = "";
    }

}
