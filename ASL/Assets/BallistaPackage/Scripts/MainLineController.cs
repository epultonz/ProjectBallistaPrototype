using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MainLineController : MonoBehaviour
{   
    private LineRenderer lr;

    //public float maxTension = 5.0f;
    //public float minTension = 1.0f;

    [Range(2.0f,8.0f)]public float velocity;
    [Range(20.0f,75.0f)]public float angle;
    private const int resolution = 10; //how many lines for smooth render trajectory

    float g;        //gravity
    float radianAngle;
    Vector3 targetPos;

    public GameObject bulletPrefab;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        g = Mathf.Abs(Physics.gravity.y);
        RenderArc();
    }

    // Update is called once per frame
    void Update()
    {
        if(lr != null && Application.isPlaying){
            RenderArc();
        }

    }

    public void shootBullet(){
        if(bulletPrefab == null){
            Debug.Log("No bullet prefab");
            return;
        }
        var bullet = Instantiate(bulletPrefab,transform.position,transform.rotation);
        
        // Velocity formula source from:
        // https://vilbeyli.github.io/Projectile-Motion-Tutorial-for-Arrows-and-Missiles-in-Unity3D/#physics
        float R = Vector3.Distance(transform.position,targetPos); //distance
        float tanAlpha = Mathf.Tan(angle*Mathf.Deg2Rad);

        float vZ = Mathf.Sqrt( g*R*R / (2.0f * (R*tanAlpha)) );
        float vY = tanAlpha * vZ;

        // shoot the projectile with applied velocity on y and z axis
        // transform direction is needed to convert from local space to worldspace
        bullet.GetComponent<Rigidbody>().velocity = 
           transform.TransformDirection(new Vector3(0f, vY, vZ));

    }

    // Function to visualize the trajectory
    // Credits to https://www.youtube.com/watch?v=iLlWirdxass
    void RenderArc(){
        lr.positionCount = resolution+1; // how many line we want to render. 
        lr.SetPositions(CalcArcArray());    // where we want to position the lines
    }

    // Create an array of vector3 positions for arc
    Vector3[] CalcArcArray(){
        Vector3[] arcArray = new Vector3[resolution+1];
    
        // convert the degree angle to radian since the formula uses radian
        radianAngle = Mathf.Deg2Rad * angle;

        // Formula source from: 
        // https://en.wikipedia.org/wiki/Projectile_motion#Maximum_distance_of_projectile
        // Calculate the max distance of a projectile given the velocity, angle, and gravity 
        float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / g;

        for (int i = 0; i <= resolution; i++)
        {
            // Calculates the position of each line
            // t can be considered at time
            float t = (float)i / (float)resolution;
            arcArray[i] = CalcArcPoint(t,maxDistance);
        }
        // Set the last element as target
        targetPos = arcArray[arcArray.Length - 1];
        return arcArray;
    }

    // Calculate height and distance of each vertex
    Vector3 CalcArcPoint(float t, float maxDistance){
        // the current line at z position
        float z = t * maxDistance;

        // Formula source from:
        // https://en.wikipedia.org/wiki/Projectile_motion#Displacement
        // To calculate the height of the line, given its z position
        float y = z * Mathf.Tan(radianAngle) - 
            ( (g * z * z) / (2*velocity*velocity*Mathf.Cos(radianAngle)*Mathf.Cos(radianAngle)));
        return transform.TransformDirection(new Vector3(transform.position.x,
            transform.position.y + y,
            transform.position.z + z));

    }

}
