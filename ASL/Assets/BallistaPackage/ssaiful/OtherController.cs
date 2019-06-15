//This will be the first controller to be execute where player can host
//or resolve the game, and set the player tag

//PlayerParent object should be parented to the worldAnchorTransform in all
//instances of the clients.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.CrossPlatform;

public class OtherController : MonoBehaviour
{
    private bool trackingComplete = false; //make sure we do hosting and resolving only once
    public enum GameState { Lobby, Hosting, Resolving, Playing }
    GameState gameState = GameState.Lobby;

    //public GameObject playground;

    private string cloudID;
    private bool isPlaying = false;
    public static Transform worldAnchorTransform;

    private static GameObject flagAnchorGO;
    public static string anchorStringID;

    private bool isHoster = false;
    private bool isResolver = false;

    // Update is called once per frame
    void Update()
    {
        if(gameState == GameState.Lobby && !trackingComplete){
            return;
        }
        if(gameState == GameState.Playing && trackingComplete){
            if(!isPlaying){
                //Debug.Log("Flag Global pos: "+ flagAnchorGO.transform.position);
                //Debug.Log("Flag Local pos: "+ flagAnchorGO.transform.localPosition);
            }
            isPlaying = true;
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

            GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>{
                GetComponent<ASL.ASLObject>().SendAndSetAnchorPoint(result.Anchor.CloudId);
            });
            
            transform.parent = worldAnchorTransform;
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            hostingResults(result.Anchor.transform, result.Anchor.CloudId);
            syncAnchorFlag();
            createFlagAnchor();
            // var pl = Instantiate(playground,worldAnchorTransform.position,Quaternion.identity);
            // pl.AddComponent<MeshRenderer>();
            
            trackingComplete = true;
            isHoster = true;
            gameState = GameState.Playing;
        });

    }

    private void hostingResults(Transform anchor, string AnchorId){
        worldAnchorTransform = anchor;
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

            transform.parent = worldAnchorTransform;
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            resolveAnchorResults(result.Anchor.transform);
            syncAnchorFlag();
            // var pl = Instantiate(playground,worldAnchorTransform.position,Quaternion.identity);
            // pl.AddComponent<MeshRenderer>();
            
            trackingComplete = true;
            isResolver = true;
            gameState = GameState.Playing;
            
        });
    }

    private void createFlagAnchor()
    {
        ASL.ASLHelper.InstanitateASLObject("AnchorFlagASL", Vector3.zero,
            Quaternion.identity, GetComponent<ASL.ASLObject>().m_Id, 
            GetType().Namespace + GetType().Name, "SetupAnchorParent");
        
        Debug.Log("Name: " + GetType().Namespace + "." + GetType().Name);
        Debug.Log("Parent m_id: " + GetComponent<ASL.ASLObject>().m_Id);
        if(anchorStringID == string.Empty || anchorStringID == null){
            Debug.Log("child m_id: is empty or null");
        }else{
            // created object id is ""
            Debug.Log("child m_id is: " + anchorStringID);
        }
        
    }

    private void resolveAnchorResults(Transform anchor){
        worldAnchorTransform = anchor;
    }

    public static void SetupAnchorParent(GameObject _myGameObject){
        _myGameObject.transform.parent = worldAnchorTransform;
        Debug.Log("Inside instantiate function");
        //is tied to the host PlayerParent object. PlayerParent object transform is 
        //parented to worldAnchorTransform and reset
        _myGameObject.GetComponent<ASL.ASLObject>().SendAndSetClaim(()=>{
            _myGameObject.GetComponent<ASL.ASLObject>().SendAndSetLocalPosition(
                Vector3.zero);
            _myGameObject.GetComponent<ASL.ASLObject>().SendAndSetLocalRotation(
                Quaternion.identity);
            anchorStringID = _myGameObject.GetComponent<ASL.ASLObject>().m_Id;
        });
    }

    void syncAnchorFlag(){
        var go = gameObject.GetComponentInChildren<Transform>();
        if(go == null){
            Debug.Log("Cant find AnchorFlag");
        }else{
            Debug.Log("child name: " + go.name);
            //is the PlayerParent object. PlayerParent object is parented
            // worldAnchorTransform and reset. Because flagAnchor is child of PlayerParent,
            // we can manipulate its transform here too
            go.transform.parent = worldAnchorTransform;
            go.GetComponent<ASL.ASLObject>().SendAndSetClaim(()=>{
                go.GetComponent<ASL.ASLObject>().SendAndSetLocalPosition(
                    Vector3.zero);
                go.GetComponent<ASL.ASLObject>().SendAndSetLocalRotation(
                    Quaternion.identity);
            });
        }
    }

    public bool getPlayStatus(){
        return isPlaying;
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

    public bool isPlayerHoster(){
        return isHoster;
    }

    public bool isPlayerResolver(){
        return isResolver;
    }

    public Transform getWorldAnchor(){
        return worldAnchorTransform;
    }

}
