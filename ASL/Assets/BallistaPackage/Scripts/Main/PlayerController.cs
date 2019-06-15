using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    protected Joystick joystick;
    protected MainATC matc;
    protected OtherController otc;

    public LineRenderer ldr;
    public ProjectileRenderer lrender;

    public GameObject plankSphere;
    static float plankAngle;
    float oriPlankAngle;
    float[] m_MyFloats = new float[4];

    SpawnBallista sbt;
    Vector3 oldTPos, newTPos,oldTRot,newTRot;

    protected bool jump;
    bool isAiming = false;

    Rigidbody rb;
    BoxCollider bc;

    private float THRESHOLD = 0.05f;
    Quaternion rot;
    Vector3 pos;
    
    // Start is called before the first frame update
    void Start()
    {
        sbt = FindObjectOfType<SpawnBallista>();
        otc = FindObjectOfType<OtherController>();
        matc = FindObjectOfType<MainATC>();

        joystick = FindObjectOfType<Joystick>();
        rb = this.gameObject.GetComponent<Rigidbody>();

        enabled = false;
        ldr.enabled = false;//try using getComponent<Renderer>().enabeld = false

        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        oldTPos = rb.transform.localPosition;
        oldTRot = rb.transform.localEulerAngles;

        var bc = GetComponent<BoxCollider>();
        bc.isTrigger = true;

        InvokeRepeating("PlankAngle",3.0f,0.1f);
        this.gameObject.GetComponent<ASL.ASLObject>()._LocallySetFloatCallback(PlankSyncFunc);

    }

    // Update is called once per frame
    void Update()
    {   
        // If we start aiming, show the projectile line
        if(matc.getStartCamSet() && !isAiming){
            ldr.enabled = true;
            isAiming = true;
        }

        // if(matc.getStartCamSet()){
            
        // }

        //If we're done aiming, shoot the bullet
        if(!matc.getStartCamSet() && isAiming){
            lrender.shootBullet();
            isAiming = false;
            ldr.enabled = false;;
        }

    }

    void checkUpdates(){
        
        if(joystick.onTouch){
            // rb.MovePosition(rb.position + (transform.forward *
            //     joystick.Vertical * 0.02f));
            this.gameObject.GetComponent<ASL.ASLObject>().SendAndSetClaim(()=>{
                pos = joystick.Vertical*0.05f * transform.forward;
                //rot = Quaternion.Euler(0.0f,joystick.Horizontal*0.01f,0.0f);
                rot = Quaternion.AngleAxis(joystick.Horizontal * 5.0f, Vector3.up);

                this.gameObject.GetComponent<ASL.ASLObject>()
                    .SendAndSetLocalPosition(transform.localPosition + pos);
                this.gameObject.GetComponent<ASL.ASLObject>()
                    .SendAndSetLocalRotation(rot);
            });
        }
        
    }

    void PlankAngle(){
        if(matc.getStartCamSet()){
            oriPlankAngle = matc.getAngleFloat();
            //plankSphere.transform.localRotation = Quaternion.Euler(-plankAngle,0.0f,0.0f);
            m_MyFloats[0] = oriPlankAngle;
            this.gameObject.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
            {
                Debug.Log("Plank Angle sent");
                this.gameObject.GetComponent<ASL.ASLObject>().SendFloat4(m_MyFloats);
            });
            //sets the plank angle here
            plankSphere.transform.rotation = Quaternion.Euler(-plankAngle,0.0f,0.0f);
        }else{
            plankSphere.transform.rotation = Quaternion.identity;
        }
    }

    public void checkOwnership(){
        if((otc.isPlayerHoster() && this.gameObject.CompareTag("BallistaP1")) || 
            (otc.isPlayerResolver() && this.gameObject.CompareTag("BallistaP2"))){
            enabled = true;
            InvokeRepeating("checkUpdates", 2.0f, 0.1f);
            Debug.Log("Player Controller enabled");
        }else{
            enabled = false;
            Debug.Log("Player Controller disabled");
        }
    }

    public static void PlankSyncFunc(float[] _myFloats)
    {
        plankAngle = _myFloats[0];
    }

    

}
