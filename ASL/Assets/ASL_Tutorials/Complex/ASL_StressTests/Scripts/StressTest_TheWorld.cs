using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StressTesting
{

    public partial class StressTest_TheWorld : MonoBehaviour
    {

        private Vector3 m_RandomSpawnLocation;

        private GameObject[] TestObjects = new GameObject[5];
        public GameObject Grandparent = null;
        public GameObject Parent = null;
        public GameObject Child = null;
        public GameObject Capsule = null;
        public GameObject Cube = null;

        private GameObject mSelected = null;

        private Color kSelectedColor = new Color(0.8f, 0.8f, 0.1f, 0.5f);
        private Color kSelectedColorForOpponent = new Color(0.3f, 0.7f, 1.0f, .5f);
        private Color mOrgObjColor = Color.white; // remember obj's original color

        static public bool StopTest;

        // Use this for initialization
        void Start()
        {
            StopTest = false;

            TestObjects[0] = Grandparent;
            TestObjects[1] = Parent;
            TestObjects[2] = Child;
            TestObjects[3] = Capsule;
            TestObjects[4] = Cube;

            TestObjects[0].GetComponent<ASL.ASLObject>()._LocallySetFloatCallback(StopClients);
            TestObjects[1].GetComponent<ASL.ASLObject>()._LocallySetFloatCallback(StopClients);
            TestObjects[2].GetComponent<ASL.ASLObject>()._LocallySetFloatCallback(StopClients);
            TestObjects[3].GetComponent<ASL.ASLObject>()._LocallySetFloatCallback(StopClients);
            TestObjects[4].GetComponent<ASL.ASLObject>()._LocallySetFloatCallback(StopClients);

        }
        private float timer = 0;
        private float randomTime = 0;
        private float moveTimer = 0;
        private float creationTimer = 0;
        private float deletionTimer = 0;
        private float randomMoveTime = 0;
        private float randomCreationTime = 0;
        private float randomDeletionTime = 0;
        private float randomX, randomY, randomZ;
        float x0, y0, z0, x1, y1, z1, x2, y2, z2, x3, y3, z3, x4, y4, z4;
        int objectNumber = 0;
        private void Update()
        {
            if (SceneManager.GetActiveScene().name == "StressTest_FightOverOneObject")
            {
                if (!StopTest)
                {
                    if (timer > randomTime)
                    {
                        switch (objectNumber)
                        {
                            case 1:
                                TestObjects[0].GetComponent<ASL.ASLObject>()?.SendAndSetClaim(() =>
                                {
                                    CheckBounds(ref x0, ref y0, ref z0);
                                    TestObjects[0].GetComponent<ASL.ASLObject>()?.SendAndSetLocalPosition(new Vector3(x0 += randomX, y0 += randomY, z0 += randomZ));
                                    randomX = Random.Range(-1.0f, 1.0f);
                                    randomY = Random.Range(-1.0f, 1.0f);
                                    randomZ = Random.Range(-1.0f, 1.0f);
                                });
                                break;
                            case 2:
                                TestObjects[1].GetComponent<ASL.ASLObject>()?.SendAndSetClaim(() =>
                                {
                                    CheckBounds(ref x1, ref y1, ref z1);
                                    TestObjects[1].GetComponent<ASL.ASLObject>()?.SendAndSetLocalPosition(new Vector3(x1 += randomX, y1 += randomY, z1 += randomZ));
                                    randomX = Random.Range(-1.0f, 1.0f);
                                    randomY = Random.Range(-1.0f, 1.0f);
                                    randomZ = Random.Range(-1.0f, 1.0f);
                                });
                                break;
                            case 3:
                                TestObjects[2].GetComponent<ASL.ASLObject>()?.SendAndSetClaim(() =>
                                {
                                    CheckBounds(ref x2, ref y2, ref z2);
                                    TestObjects[2].GetComponent<ASL.ASLObject>()?.SendAndSetLocalPosition(new Vector3(x2 += randomX, y2 += randomY, z2 += randomZ));
                                    randomX = Random.Range(-1.0f, 1.0f);
                                    randomY = Random.Range(-1.0f, 1.0f);
                                    randomZ = Random.Range(-1.0f, 1.0f);
                                });
                                break;
                            case 4:
                                TestObjects[3].GetComponent<ASL.ASLObject>()?.SendAndSetClaim(() =>
                                {
                                    CheckBounds(ref x3, ref y3, ref z3);
                                    TestObjects[3].GetComponent<ASL.ASLObject>()?.SendAndSetLocalPosition(new Vector3(x3 += randomX, y3 += randomY, z3 += randomZ));
                                    randomX = Random.Range(-1.0f, 1.0f);
                                    randomY = Random.Range(-1.0f, 1.0f);
                                    randomZ = Random.Range(-1.0f, 1.0f);
                                });
                                break;
                            case 5:
                                TestObjects[4].GetComponent<ASL.ASLObject>()?.SendAndSetClaim(() =>
                                {
                                    CheckBounds(ref x4, ref y4, ref z4);
                                    TestObjects[4].GetComponent<ASL.ASLObject>()?.SendAndSetLocalPosition(new Vector3(x4 += randomX, y4 += randomY, z4 += randomZ));
                                    randomX = Random.Range(-1.0f, 1.0f);
                                    randomY = Random.Range(-1.0f, 1.0f);
                                    randomZ = Random.Range(-1.0f, 1.0f);
                                });

                                break;
                        }
                        objectNumber = Random.Range(1, 6);
                        randomTime = Random.Range(0, 2000); //Random time between 0 and 1 second - represents how often we send claim/packet
                        timer = 0;
                    }
                }
                timer += Time.deltaTime * 1000; //Timer in miliseconds
            }
            else if (SceneManager.GetActiveScene().name == "StressTest_CreateDelete")
            {
                if (creationTimer > randomCreationTime) //Create an object in a random location
                {
                    randomX = Random.Range(-5.0f, 5.0f);
                    randomY = Random.Range(0.0f, 3.0f);
                    randomZ = Random.Range(-5.0f, 5.0f);
                    Vector3 randomVector = new Vector3(randomX, randomY, randomZ);

                    ASL.ASLHelper.InstanitateASLObject(PrimitiveType.Cube, randomVector,
                        Quaternion.identity, "", GetType().Namespace + "." + GetType().Name, "CreatedGameObject",
                        GetType().Namespace + "." + GetType().Name, "CancelledClaimRecovery");


                    randomCreationTime = Random.Range(1500, 2500);
                    creationTimer = 0;
                }
                if (moveTimer > randomMoveTime) //Randomly move a random object
                {
                    randomX = Random.Range(-5.0f, 5.0f);
                    randomY = Random.Range(0f, 3.0f);
                    randomZ = Random.Range(-5.0f, 5.0f);
                    Vector3 randomVector = new Vector3(randomX, randomY, randomZ);

                    var aslObjects = FindObjectsOfType<ASL.ASLObject>(); //Warning: Getting objects this way is slow
                    var t = ASL.ASLHelper.m_ASLObjects.Values.Count;
                    if (aslObjects.Length > 0)
                    {
                        objectNumber = Random.Range(0, aslObjects.Length - 1);

                        aslObjects[objectNumber].SendAndSetClaim(() =>
                        {
                            aslObjects[objectNumber].SendAndSetLocalPosition(randomVector);
                        });

                    }
                    randomMoveTime = Random.Range(1000, 2000); //Random time between 0 and 1 second - represents how often we send claim/packet
                    moveTimer = 0;
                }
                if (deletionTimer > randomDeletionTime) //Delete a random object
                {
                    var aslObjects = FindObjectsOfType<ASL.ASLObject>();
                    if (aslObjects.Length > 0)
                    {
                        objectNumber = Random.Range(0, aslObjects.Length - 1);
                        aslObjects[objectNumber].SendAndSetClaim(() =>
                        {
                            aslObjects[objectNumber].DeleteObject();
                        });
                    }
                    randomDeletionTime = Random.Range(1500, 2400); //Random time between 0 and 1 second - represents how often we send claim/packet
                    deletionTimer = 0;
                }

                moveTimer += Time.deltaTime * 1000; //Timer in miliseconds
                creationTimer += Time.deltaTime * 1000; //Timer in miliseconds
                deletionTimer += Time.deltaTime * 1000; //Timer in miliseconds
            }
        }
        private void CheckBounds(ref float x, ref float y, ref float z)
        {
            if (x > 5 || x < -5) { x = 0; }
            if (y > 3 || y < 0) { y = 0; }
            if (z > 5 || z < -5) { z = 0; }
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
            if (mSelected != null && mSelected.GetComponent<ASL.ASLObject>())
            {
                //Setting the original color like this is obviously not ideal. But ASL does not keep track of an object's color
                //to help return the correct color - it only allows an object's color to be set - leaving the responsibility of the
                //implementing a way to get the original color back for all users to the programmer (like what we did here)
                if (mSelected.GetComponent<Renderer>().material.name == "GrandParentMaterial (Instance)")
                {
                    mOrgObjColor = new Color(0.155678f, 0.07843137f, 1, 1);
                    mSelected.GetComponent<ASL.ASLObject>()?.SendAndSetClaim(() =>
                    {
                        mSelected.GetComponent<ASL.ASLObject>().SendAndSetObjectColor(mOrgObjColor, mOrgObjColor);
                    });
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
                    mSelected.GetComponent<ASL.ASLObject>()?.SendAndSetClaim(() =>
                    {
                        mSelected.GetComponent<ASL.ASLObject>().SendAndSetObjectColor(mOrgObjColor, mOrgObjColor);
                    });
                }
                else
                {
                    mOrgObjColor = Color.white;
                    mSelected.GetComponent<ASL.ASLObject>()?.SendAndSetClaim(() =>
                    {
                        mSelected.GetComponent<ASL.ASLObject>().SendAndSetObjectColor(mOrgObjColor, mOrgObjColor);
                    });
                }

                ShowSelectedAxisFrame(false);
            }

            mSelected = g;
            if (mSelected != null && mSelected.GetComponent<ASL.ASLObject>())
            {
                mOrgObjColor = g.GetComponent<Renderer>().material.color; // save a local copy
                ShowSelectedAxisFrame(true);

                randomX = mSelected.transform.localPosition.x;
                randomY = mSelected.transform.localPosition.y;
                randomZ = mSelected.transform.localPosition.z;

            }
        }

        private void ShowSelectedAxisFrame(bool on)
        {
            bool found = false;
            if (mSelected != null)
            {
                int i = 0;
                while ((!found) && (i < mSelected.transform.childCount))
                {
                    Transform g = mSelected.transform.GetChild(i);
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

        private Vector3 kDeltaVector = new Vector3(0.5f, .5f, .5f);

        public void CreatePrimitive(PrimitiveType type)
        {
            if (mSelected != null) //If we have a parent
            {
                ASL.ASLHelper.InstanitateASLObject(type, kDeltaVector, Quaternion.identity, mSelected.GetComponent<ASL.ASLObject>().m_Id, "", "", "", "");
                int siblingCount = mSelected.transform.childCount;
                if (siblingCount > 0)
                {
                    Transform sibling = mSelected.transform.GetChild(0);
                }
            }
            else //No parent
            {
                float x = Random.Range(-4f, 4f);
                float y = Random.Range(0f, 2.5f);
                float z = Random.Range(-4f, 4f);

                //Set parent to be our GameController
                ASL.ASLHelper.InstanitateASLObject(type, new Vector3(x, y, z), Quaternion.identity, "GameController", "", "", "", "");

            }
        }

        private void ActionToTake()
        {
            mSelected.GetComponent<ASL.ASLObject>()?.SendAndSetObjectColor(kSelectedColor, kSelectedColorForOpponent);
        }

        //Must be public and static to work
        public static void CancelledClaimRecovery(string _id, int _cancelledCallbacks)
        {
            Debug.LogWarning("We are going to cancel " + _cancelledCallbacks +
                " callbacks generated by a claim for object: " + _id + " rather than try to recover.");
        }


        //If you want to do anything with the object you just created - put the code here. It will execute once the server creates your object
        public static void CreatedGameObject(GameObject _myGameObject)
        {

        }

        public void StopClients(float[] f)
        {
            if (f[0] == 0)
            {
                StopTest = true;
                Debug.Log("Stop");
            }
            if (f[1] == 1)
            {
                Debug.Log("1");
            }
            if (f[2] == 2)
            {
                Debug.Log("2");
            }
            if (f[3] == 3)
            {
                Debug.Log("3");
            }
        }


    }
}