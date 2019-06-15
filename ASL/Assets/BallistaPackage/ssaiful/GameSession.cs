using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class GameSession : MonoBehaviour
{
    public ARCoreSession ARss;
    OtherController otc;
    public float score;

    private void Start() {
        otc = FindObjectOfType<OtherController>();
        score = 10.0f;

        // FindObjectOfType<ARCoreSession>().SessionConfig.PlaneFindingMode = 
        //     DetectedPlaneFindingMode.Disabled;

        // FindObjectOfType<ARCoreSession>().SessionConfig.EnablePlaneFinding = false;
    }

    public void changeScore(float s){
        score = score - s;
    }

    public float getScore(){
        return score;
    }


    // public void setRot(){
    //     otc.getWorldAnchor().SetParent(this.transform);
    //     transform.LookAt(Camera.main.transform, transform.up);
    // }

}
