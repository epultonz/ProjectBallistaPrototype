using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class SpawnBallista : MonoBehaviour
{
    public GameObject ballistaObject;

    static GameObject tryGO;

    bool spawned = false;
    static OtherController otc;
    MainATC matc;

    static bool isHoster = false;
    static string ballistaTag;

    private void Start() {
        otc = FindObjectOfType<OtherController>();
        matc = FindObjectOfType<MainATC>();
        if(otc == null){
            Debug.Log("Cant find otherController");
        }
        ballistaTag = string.Empty;
    }

    public void spawnBallistaObject(){
        if(!spawned && otc.getPlayStatus()){
            if(otc.isPlayerHoster()){
                ballistaTag = "BallistaP1";
                isHoster = true;
            }else if (otc.isPlayerResolver()){
                ballistaTag = "BallistaP2";
            }else{
                Debug.Log("Player is neither hoster or resolver");
            }

            Pose anchorPose;
            bool didHit = FindPointOnPlane(out anchorPose);
            if(didHit){
                //it went inside here
                //parent everything to the worldanchorTransform
                // when instantiating the transform is global.
                ASL.ASLHelper.InstanitateASLObject("BallistaObject", Vector3.zero, 
                    Quaternion.identity, GetComponent<ASL.ASLObject>().m_Id,
                     GetType().Namespace + GetType().Name, "CreatedBallistaObject");
                
                Debug.Log("Ballista Parent m_id is: " + GetComponent<ASL.ASLObject>().m_Id);
                spawned = true;
                matc.setPlayerTag();
                
            }
        }
    }

    public static void CreatedBallistaObject(GameObject _myGameObject)
    {
        if(_myGameObject.GetComponent<ASL.ASLObject>() != null){
            //_myGameObject.transform.parent = otc.getWorldAnchor();
            _myGameObject.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>   //this doesnt
            {
                if(isHoster){
                    _myGameObject.GetComponent<ASL.ASLObject>().SendAndSetObjectColor(Color.red, Color.red);
                }else{
                    _myGameObject.GetComponent<ASL.ASLObject>().SendAndSetObjectColor(Color.green, Color.green);
                }
                _myGameObject.GetComponent<ASL.ASLObject>().SendAndSetLocalPosition((Vector3.up * 0.15f));
                _myGameObject.GetComponent<ASL.ASLObject>().SendAndSetLocalRotation(Quaternion.identity);

                _myGameObject.tag = ballistaTag;
                if(_myGameObject != null){
                    Debug.Log("Ballista object created: " + 
                        _myGameObject.GetComponent<ASL.ASLObject>().m_Id + " <--");
                    Debug.Log("Ballista tag is " + ballistaTag);
                    _myGameObject.GetComponent<PlayerController>().checkOwnership();
                }else if(_myGameObject.GetComponent<ASL.ASLObject>().m_Id == string.Empty){
                    Debug.Log("Ballista ASL ID is empty");
                }

                var go = _myGameObject.transform.GetComponentInParent<OtherController>().gameObject;
                Debug.Log("Parent name is " + go.name);

            });
        } else {
            Debug.Log("Ballista doesnt have ASL component");
        }

    }

    private bool FindPointOnPlane(out Pose pose) {
        bool didHit = false;
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        Vector2 screenPos = new Vector2(Screen.width/2, Screen.height/2) ;
        didHit = Frame.Raycast(screenPos.x, screenPos.y, raycastFilter, out hit);
        pose = hit.Pose;
        return didHit;
    }

    public void resetSpawn(){
        spawned = false;
    }

    public string getBallistaTag(){
        return ballistaTag;
    }
}
