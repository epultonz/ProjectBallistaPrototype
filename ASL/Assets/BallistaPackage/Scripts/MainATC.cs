using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainATC : MonoBehaviour
{
    private bool startCamSet = false;

    protected OtherController otc;
    private string ballistaTag;

    private float camInitZPos, camInitYPos;
    private float camCurrZPos, camCurrYPos;
    private float camFinalZPos, camFinalYPos;

    private float velocity;
    private float angle;

    private const float zOFFSET = 1.0f;
    private const float yOFFSET = 10.0f;
    private const float zRESCALE = 10.0f;
    private const float yRESCALE = 100.0f;

    void Start() {
        otc = FindObjectOfType<OtherController>();
    }

    // Update is called once per frame
    void Update()
    {
        // See if we have any touch input at this current frame
        if(Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            //  If the spot we touch is the ballista placeholder, then do something
            if(touch.phase == TouchPhase.Began ){
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit) && !startCamSet 
                    && hit.transform.tag == ballistaTag){
                    // Initialize initial position
                    startCamSet = true;
                    // We scale the values
                    camInitZPos = Camera.main.transform.position.z*zRESCALE;
                    camInitYPos = Camera.main.transform.position.y*yRESCALE;
                }
            }
            // if user lift their finger, set the final position
            if(touch.phase == TouchPhase.Ended){
                camFinalZPos = Camera.main.transform.position.z;
                camFinalYPos = Camera.main.transform.position.y;
                startCamSet = false;
            }

            camCurrZPos = Camera.main.transform.position.z*zRESCALE;
            camCurrYPos = Camera.main.transform.position.y*yRESCALE;

            //holdStr.text = System.Math.Round(camCurrZPos, 2).ToString();

            // Clamp the values and add the offsets
            velocity = Mathf.Clamp((camInitZPos - camCurrZPos)+zOFFSET, 2.0f, 8.0f);
            angle = Mathf.Clamp((camInitYPos - camCurrYPos)+yOFFSET, 20.0f, 70.0f);
        }
    }

    public float getVelocityFloat(){
        return velocity;
    }

    public float getAngleFloat(){
        return angle;
    }

    public bool getStartCamSet(){
        return startCamSet;
    }

    public void setPlayerTag(){
        if(otc.isPlayerHoster()){
            ballistaTag = "BallistaP1";
        }else if (otc.isPlayerResolver()){
            ballistaTag = "BallistaP2";
        }
    }

    public string getPlayerTag(){
        return ballistaTag;
    }

}
