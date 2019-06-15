using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mps
{
    public partial class Mp2_TheWorld : MonoBehaviour
    {

        private Vector3 m_RandomSpawnLocation;

        private GameObject mSelected = null;

        private Color kSelectedColor = new Color(0.8f, 0.8f, 0.1f, 0.5f);
        private Color kSelectedColorForOpponent = new Color(0.3f, 0.7f, 1.0f, .5f);
        private Color mOrgObjColor = Color.white; // remember obj's original color

        // Use this for initialization
        void Start()
        {
            // OK, this is a little ugly ...
            mSelected = GameObject.Find("GrandParent");
            ShowSelectedAxisFrame(false, mSelected);
            mSelected = GameObject.Find("Parent");
            ShowSelectedAxisFrame(false, mSelected);
            mSelected = GameObject.Find("Child");
            ShowSelectedAxisFrame(false, mSelected);
            mSelected = null;

        }

        public GameObject SelectObject(GameObject obj)
        {
            if ((obj != null) && (obj.name == "CreationPlane"))
            {
                obj = null;
            }

            SetObjectSelection(obj);
            return mSelected;
        }

        private void SetObjectSelection(GameObject g)
        {
            if (mSelected != null && mSelected.GetComponent<ASL.ASLObject>() != null)
            {
                //Setting the original color like this is obviously not ideal. But ASL does not keep track of an object's color
                //to help return the correct color - it only allows an object's color to be set - leaving the responsibility of the
                //implementing a way to get the original color back for all users to the programmer (like what we did here)
                if (mSelected.GetComponent<Renderer>().material.name == "GrandParentMaterial (Instance)")
                {
                    mOrgObjColor = new Color(0.155678f, 0.07843137f, 1, 1);
                    mSelected.GetComponent<ASL.ASLObject>()?.SendAndSetClaim(() =>
                    {
                        mSelected?.GetComponent<ASL.ASLObject>()?.SendAndSetObjectColor(mOrgObjColor, mOrgObjColor);
                    }, 1000, false);
                }
                else if (mSelected.GetComponent<Renderer>().material.name == "ParentMaterial (Instance)")
                {
                    mOrgObjColor = new Color(0.2506061f, 0.6132076f, 0.1648718f, 1);
                    mSelected.GetComponent<ASL.ASLObject>()?.SendAndSetClaim(() =>
                    {
                        mSelected.GetComponent<ASL.ASLObject>().SendAndSetObjectColor(mOrgObjColor, mOrgObjColor);
                    });
                }
                else if (mSelected.GetComponent<Renderer>().material.name == "ChildMaterial (Instance)")
                {
                    mOrgObjColor = new Color(1, 0.1650943f, 0.1650943f, 1);
                    mSelected.GetComponent<ASL.ASLObject>()?.SendAndSetObjectColor(mOrgObjColor, mOrgObjColor);
                }
                else
                {
                    mOrgObjColor = Color.white;
                    mSelected.GetComponent<ASL.ASLObject>()?.SendAndSetClaim(() =>
                    {
                        mSelected.GetComponent<ASL.ASLObject>().SendAndSetObjectColor(mOrgObjColor, mOrgObjColor);
                    });
                }

                ShowSelectedAxisFrame(false, mSelected);
            }

            mSelected = g;
            if (mSelected != null && mSelected.GetComponent<ASL.ASLObject>() != null)
            {
                mOrgObjColor = g.GetComponent<Renderer>().material.color; // save a local copy
                ShowSelectedAxisFrame(true, mSelected);

                //Method #1: All code you want to execute after claim is confirmed must go inside the { } braces
                mSelected.GetComponent<ASL.ASLObject>()?.SendAndSetClaim(() =>
                {
                    ActionToTake();
                    mSelected.GetComponent<ASL.ASLObject>()._LocallySetReleaseFunction(OnRelease);
                }, 10000);

                //Method #2: All code you want to execute after claim is confirmed must go in "ActionToTake"
                //mSelected.GetComponent<ASL.ASLObject>()?.SendAndSetClaim(ActionToTake);

                //Reminder: Any code after claim that is not 'inside' of the claim is not guarnteed to be recognized by other players
                //For example, if you call a transform update right after claim, it won't be seen by old (but still current in their eyes)
                //owner of the object
            }
        }

        private void ActionToTake()
        {
            mSelected.GetComponent<ASL.ASLObject>()?.SendAndSetObjectColor(kSelectedColor, kSelectedColorForOpponent);
        }

        private void OnRelease(GameObject _myGameObject)
        {
            if (_myGameObject != null && _myGameObject.GetComponent<ASL.ASLObject>() != null)
            {
                if (_myGameObject.GetComponent<Renderer>().material.name == "GrandParentMaterial (Instance)")
                {
                    mOrgObjColor = new Color(0.155678f, 0.07843137f, 1, 1);
                }
                else if (_myGameObject.GetComponent<Renderer>().material.name == "ParentMaterial (Instance)")
                {
                    mOrgObjColor = new Color(0.2506061f, 0.6132076f, 0.1648718f, 1);
                }
                else if (_myGameObject.GetComponent<Renderer>().material.name == "ChildMaterial (Instance)")
                {
                    mOrgObjColor = new Color(1, 0.1650943f, 0.1650943f, 1);
                }
                else
                {
                    mOrgObjColor = Color.white;
                }
                _myGameObject.GetComponent<ASL.ASLObject>()?.SendAndSetObjectColor(mOrgObjColor, mOrgObjColor);
                ShowSelectedAxisFrame(false, _myGameObject);
            }
        }

        private void ShowSelectedAxisFrame(bool on, GameObject go)
        {
            bool found = false;
            if (go != null)
            {
                int i = 0;
                while ((!found) && (i < go.transform.childCount))
                {
                    Transform g = go.transform.GetChild(i);
                    if (g.gameObject.name == "AxisFrame")
                    {
                        for (int gi = 0; gi < g.childCount; gi++)
                        {
                            Transform ax = g.GetChild(gi);
                            ax.GetComponent<Renderer>().enabled = on;
                        }
                        found = true;
                    }
                    i = i + 1;
                }
            }
        }

    }
}