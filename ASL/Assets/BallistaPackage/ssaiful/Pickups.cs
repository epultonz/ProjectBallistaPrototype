using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    public GameObject target;
    float radius;
    // Start is called before the first frame update
    void Start()
    {
        radius = 2.5f;
        InvokeRepeating("SpawnPickups", 15.0f, 5.0f);
    }

    void SpawnPickups(){
        if(Random.value < .2f){
            Instantiate (target, Random.insideUnitSphere * radius 
            + transform.position,  Random.rotation);
        }
    }
}
