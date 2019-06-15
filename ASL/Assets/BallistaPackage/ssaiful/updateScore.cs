using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class updateScore : MonoBehaviour
{
    GameSession gs;
    OtherController otc;

    public Text txt;
    // Start is called before the first frame update
    void Start()
    {
        gs = FindObjectOfType<GameSession>();
        otc = FindObjectOfType<OtherController>();
        InvokeRepeating("doUpdate", 5.0f, 0.2f);
    }

    void doUpdate(){
        if(otc.getPlayStatus()){
            if(this.enabled == false){
                this.enabled = true;
            }
            
            float score = gs.getScore();
            txt.text = score.ToString("F2");
        }
        if(gs.getScore() == 5.0f){
            txt.text = "YOU WON!";
        }
    }
}
