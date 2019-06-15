using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    GameSession gs;
    // Start is called before the first frame update
    void Start()
    {
        gs = FindObjectOfType<GameSession>();
        Destroy(this.gameObject, 6.0f);
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("BallistaP1") || 
        other.gameObject.CompareTag("BallistaP2")){
            gs.changeScore(2.0f);
        }
    }

}
