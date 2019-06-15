using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using GoogleARCore.CrossPlatform;

public class Controller : MonoBehaviour
{
    public float thrust;
    private Rigidbody rb;
    private Renderer rend;
    private bool objectSet = false;

    private static GameObject spawnedCube;

    private bool trackingComplete = false; //make sure we do hosting and resolving only once
    public enum GameState { Lobby, Hosting, Resolving, Playing }
    GameState gameState = GameState.Lobby;

    private string cloudID;
    private Transform worldAnchor;

    private void Start() {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameState == GameState.Lobby && !trackingComplete){
            return;
        }
        if(gameState == GameState.Playing && trackingComplete){
            if(!objectSet){
                spawnedCube = GameObject.FindWithTag("SpawnedCube");
                
                if(spawnedCube != null){
                    rb = spawnedCube.GetComponent<Rigidbody>();
                    rend = spawnedCube.GetComponent<Renderer>();
                    objectSet = true;
                    Debug.Log("Spawned Cube located.");
                }else{
                    Debug.Log("Cant find cube");
                }
                   
            }
            return;
        }

        if(gameState == GameState.Hosting && !trackingComplete){
            doHosting();
        }

        if(gameState == GameState.Resolving && !trackingComplete){
            cloudID = GetComponent<ASL.ASLObject>().m_AnchorPoint;
            if(cloudID != string.Empty){
                Debug.Log("CloudID found");
                doResolving(cloudID);
            } else {
                Debug.Log("CloudID is null");
            }
        }
        
    }

    // Code from ARCore LightBoard Example
    bool FindPointOnPlane(out Pose pose) {
        bool didHit = false;
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        Vector2 screenPos = new Vector2(Screen.width/2, Screen.height/2) ;
        didHit = Frame.Raycast(screenPos.x, screenPos.y, raycastFilter, out hit);
        pose = hit.Pose;
        return didHit;
    }

    public void doHosting(){
        Pose anchorPose;
        bool didHit = FindPointOnPlane(out anchorPose);
        if (!didHit) {
            Debug.Log("Didn't hit a plane... Point your device at plane and try again");
            return;
        }
        Debug.Log("Point found, creating anchor");

        Anchor hostAnchor = Session.CreateAnchor(anchorPose);
        trackingComplete = true;
        XPSession.CreateCloudAnchor(hostAnchor).ThenAction(result => {
            if (result.Response != CloudServiceResponse.Success) {
                var errorString = string.Format("Failed to HOST cloud anchor: {0}.", result.Response);
                Debug.Log(errorString);
                trackingComplete = false;
                return;
            }
            Debug.Log("Response is: " + result.Response.ToString());
            Debug.Log("Hosting Complete with cloudID: " + result.Anchor.CloudId);
            Debug.Log("Anchored at position: "+ result.Anchor.transform.position.ToString());

            GetComponent<ASL.ASLObject>().SendAndSetAnchorPoint(result.Anchor.CloudId);
            hostingResults(result.Anchor.transform, result.Anchor.CloudId);
            
            trackingComplete = true;
            gameState = GameState.Playing;
        });

    }

    public void hostingResults(Transform anchor, string AnchorId){
        worldAnchor = anchor;
        cloudID = AnchorId;
    }

    private void doResolving(string cloudID)
    {   
        trackingComplete = true;
        XPSession.ResolveCloudAnchor(cloudID).ThenAction(result => {
            if (result.Response != CloudServiceResponse.Success) {
                var errorString = string.Format("Failed to RESOLVE cloud anchor: {0}.", result.Response);
                Debug.LogError(errorString);
                Debug.Log("Could not find your anchor..." + errorString);
                trackingComplete = false;
                return;
            }

            Debug.Log(string.Format("RESOLVED with cloudID {0}", result.Anchor.CloudId));
            Debug.LogFormat("Resolved at position {0}", result.Anchor.transform.position);
            
            resolveAnchorResults(result.Anchor.transform);
            //setObjectSpawn(result.Anchor.transform.position, result.Anchor.transform.rotation);
            trackingComplete = true;
            gameState = GameState.Playing;
        });
    }

    // private void setObjectSpawn(Vector3 position, Quaternion rotation)
    // {
    //     ASL.ASLObject.InstantiateNetworkPrefab("ExampleCube",position,rotation,"", 
    //         GetType().Namespace + GetType().Name,"CreatedGameObject","","");
    //     Debug.Log("Object Spawned");
    // }

    private void resolveAnchorResults(Transform anchor){
        worldAnchor = anchor;
        //gameObject.transform.position = anchor.position;
        //gameObject.transform.rotation = anchor.rotation;
    }

    //Repeat every two seconds
    public void PrintAnchorPosition(){
        if(worldAnchor != null){
            Debug.Log("Anchor Pos: " + worldAnchor.position);
        }
    }

    //If you want to do anything with the object you just created - put the code here.
    // It will execute once the server creates your object
    public static void CreatedGameObject(GameObject _myGameObject)
    {
        _myGameObject.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        spawnedCube = _myGameObject;
    }

    // // ExampleCube Manipulator methods
    public void moveObjectForward(){
        if(!objectSet){
            return;
        }
        rb.AddRelativeForce(Vector3.forward * thrust);
        spawnedCube.GetComponent<ASL.ASLObject>().SendAndSetClaim(()=>{
            spawnedCube.GetComponent<ASL.ASLObject>().SendAndSetLocalPosition(rb.transform.position);
            
        });
    }

    public void moveObjectBackward(){
        if(!objectSet){
            return;
        }
        rb.AddRelativeForce(Vector3.forward * -thrust);
        spawnedCube.GetComponent<ASL.ASLObject>().SendAndSetClaim(()=>{
            spawnedCube.GetComponent<ASL.ASLObject>().SendAndSetLocalPosition(rb.transform.position);
        });
    }

    public void changeColor(){
        if(!objectSet){
            return;
        }
        rend.material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        Color col = rend.material.color;
        spawnedCube.GetComponent<ASL.ASLObject>().SendAndSetClaim(()=>{
            spawnedCube.GetComponent<ASL.ASLObject>().SendAndSetObjectColor(col,col);
        });
    }
    
    // public UI methods
    public void hostClick(){
        if(trackingComplete){
            return;
        }
        Debug.Log("Host Clicked");
        gameState = GameState.Hosting;
    }

    public void resolveClick(){
        if(trackingComplete){
            return;
        }
        Debug.Log("Resolve Clicked");
        gameState = GameState.Resolving;
    }

}
