using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boom : MonoBehaviour
{
    public GameObject go;
    SpawnBallista sb;
    GameSession gs;
    OtherController otc;

    Rigidbody rb;
    string bt;
    string oppBT;

    bool firstT = false;

    private void Start() {
        sb = FindObjectOfType<SpawnBallista>();
        gs = FindObjectOfType<GameSession>();
        otc = FindObjectOfType<OtherController>();
        bt = sb.getBallistaTag();
        rb = GetComponent<Rigidbody>();
        if(bt == "BallistaP1"){
            oppBT = "BallistaP2";
        }else{
            oppBT = "BallistaP1";
        }
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("plane") && !firstT){
            Instantiate(go, other.GetContact(0).point, Quaternion.AngleAxis(90f,Vector3.right));
            firstT = true;
        }
        if(other.gameObject.CompareTag(oppBT) ){
            //&& !other.gameObject.CompareTag("Untagged")
            Destroy(this.gameObject);
            Debug.Log("Hit!");
            if( (otc.isPlayerHoster() && oppBT == "BallistaP2") || 
            (otc.isPlayerResolver() && oppBT == "BallistaP1") ){
                gs.changeScore(1.0f);
            }
        }
        
    }

    public void launch(float yVal, float zVal){
        
        rb.velocity = transform.TransformDirection(new Vector3(0f, yVal, zVal));
        rb.useGravity = true;
    }
}
