using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plankAngle : MonoBehaviour
{
    MainATC mtc;
    GameSession gs;
    SpawnBallista sb;
    OtherController otc;
    string bt;

    //bool isOwn = false;

    // Start is called before the first frame update
    void Start()
    {
        sb = FindObjectOfType<SpawnBallista>();
        mtc = FindObjectOfType<MainATC>();
        gs = FindObjectOfType<GameSession>();
        otc = FindObjectOfType<OtherController>();
        bt = sb.getBallistaTag();

        if((otc.isPlayerHoster() && bt == "BallistaP1") ||
        otc.isPlayerResolver() && bt == "BallistaP2"){
            //isOwn = true;
            InvokeRepeating("setPlank", 2.0f, 0.1f);
            Debug.Log("Plank setting owned");
        }

        
    }

    void setPlank(){ 
        if(mtc.getStartCamSet()){
            //angle constraints is from 20 to 70 degrees
            var angle = mtc.getAngleFloat();
            this.gameObject.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
            {
                Debug.Log("Setting plank angle");
                this.gameObject.GetComponent<ASL.ASLObject>().SendAndSetLocalRotation(Quaternion.Euler(-angle,0.0f,0.0f));
            });
            //transform.localRotation = Quaternion.Euler(-angle,0.0f,0.0f);
        }
        else{
            if(transform.localRotation != Quaternion.identity){
                transform.localRotation = Quaternion.identity;
            }
        }
    }
}
