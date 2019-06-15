using System.Collections;
using UnityEngine;

public class CollidePlane : MonoBehaviour
{
    
    private void OnCollisionEnter(Collision other) {
        
        if(other.transform.tag == "BallistaP1" || other.transform.tag == "BallistaP2"){
            FindObjectOfType<SpawnBallista>().resetSpawn();
            // PlayerController[] goList = FindObjectsOfType<PlayerController>();
            // for(var i = 0; i < goList.Length; i++){
            //     Destroy(goList[i].gameObject);
            // }
        }

        Destroy(other.gameObject);
        Debug.Log(other.gameObject.name + " is destroyed");
    }
}
