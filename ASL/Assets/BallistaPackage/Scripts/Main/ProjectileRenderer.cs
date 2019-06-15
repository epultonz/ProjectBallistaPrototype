using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ProjectileRenderer : MonoBehaviour
{   
    private LineRenderer lr;
    private bool active;

    private float velocity;
    private float angle;
    private const int resolution = 10; //how many lines for smooth render trajectory

    float g;        //gravity
    float radianAngle;
    Vector3 targetPos;

    MainATC atc;
    //public Animator anim;
    static Transform t;
    static float vY;
    static float vZ;
    static float m_vY;
    static float m_vZ;

    static bool ready = false; 
    static int player;

    public GameObject bulletPrefab;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        g = Mathf.Abs(Physics.gravity.y);
        atc = FindObjectOfType<MainATC>();
        
        t = transform;
        if(atc != null){
            RenderArc();
        }else{
            Debug.Log("atc null in line renderer");
        }
        
        if(atc.getPlayerTag() == "BallistaP1"){
            player = 1;
        }else{
            player = 2;
        }

        //GetComponent<ASL.ASLObject>().SetFloatCallbackLocally(SendFloatBullet);
    }

    // Update is called once per frame
    void Update()
    {
        velocity = atc.getVelocityFloat();
        angle = atc.getAngleFloat();

        if(lr != null && Application.isPlaying){
            RenderArc();
        }

        if(ready){
            // shoot the projectile with applied velocity on y and z axis
            // transform direction is needed to convert from local space to worldspace
            Debug.Log("Shooting the bullet with vY: " + vY.ToString("F2") + 
                " and vZ: " + vZ.ToString("F2"));
            var pro = GameObject.FindGameObjectWithTag("bullet");
            if(pro != null){
                pro.GetComponent<boom>().launch(vY, vZ);
                pro.tag = "Untagged";
            }
            ready = false;
        }

    }

    public void shootBullet(){
        if(bulletPrefab == null){
            Debug.Log("No bullet prefab");
            return;
        }

        // Velocity formula source from:
        // https://vilbeyli.github.io/Projectile-Motion-Tutorial-for-Arrows-and-Missiles-in-Unity3D/#physics
        float R = Vector3.Distance(transform.position,targetPos); //distance
        float tanAlpha = Mathf.Tan(angle*Mathf.Deg2Rad);

        m_vZ = Mathf.Sqrt( g*R*R / (2.0f * (R*tanAlpha)) );
        m_vY = tanAlpha * vZ;
        Debug.Log("Local Vz and Vy are " + m_vZ.ToString("F2") + " " + m_vY.ToString("F2"));

        ASL.ASLHelper.InstanitateASLObject("Bullet",transform.position,transform.rotation,"",
            GetType().Namespace + GetType().Name, "CreatedBulletObject" , "", "", 
            GetType().Namespace + GetType().Name, "SendFloatBullet");
        
    }

    public static void CreatedBulletObject(GameObject bullet)
    {
        bullet.GetComponent<ASL.ASLObject>().SendAndSetClaim(()=>{
            float[] yzValues = new float[2];
            yzValues[0] = m_vY;
            yzValues[1] = m_vZ;
            bullet.GetComponent<ASL.ASLObject>().SendFloat4(yzValues);
            if(player == 1){
                bullet.GetComponent<ASL.ASLObject>().SendAndSetObjectColor(Color.red, Color.red);
            }else{
                bullet.GetComponent<ASL.ASLObject>().SendAndSetObjectColor(Color.green, Color.green);
            }
            
        });
        
    }

    public static void SendFloatBullet(float[] _floats)
    {
        vY = _floats[0];
        vZ = _floats[1];
        ready = true;
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
            
        return transform.TransformDirection(new Vector3(transform.localPosition.x,
            transform.position.y + y,
            transform.position.z + z));

    }

}
