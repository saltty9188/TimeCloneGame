using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordingDoor : Door
{

    #region Inspector fields
    [SerializeField] private Recorder playerRecorder = null;
    #endregion

    #region Private fields
    private bool inFight;
    private bool stayOpen;
    #endregion

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        inFight = false;
        stayOpen = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(!playerRecorder.IsRecording) 
        {
            inFight = false;
        }

        if(playerRecorder.IsRecording && !inFight)
        {
            activations = requiredActivations;
        }
        else if(!stayOpen)
        {
            activations = 0;
        }  

        TriggerIfValid();
    }

    public void KeepOpen()
    {
        stayOpen = true;
        activations = requiredActivations;
        TriggerIfValid();
    }

    public void SetInFight(bool inFight)
    {
        this.inFight = inFight;
    }
}
