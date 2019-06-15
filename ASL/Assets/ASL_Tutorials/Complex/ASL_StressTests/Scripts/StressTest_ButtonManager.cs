using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StressTesting
{
    public class StressTest_ButtonManager : MonoBehaviour
    {
        public Button mDeleteButton = null;
        public Button mStopAll = null;

        private GameObject mSelected;
        // Start is called before the first frame update
        void Start()
        {
            Debug.Assert(mDeleteButton != null);
            Debug.Assert(mStopAll != null);
            mDeleteButton.interactable = false;
        }

        public void SetDeleteInteractable(GameObject g)
        {
            mSelected = g;
            if (g != null)
            {
                mDeleteButton.interactable = true;
            }
            else
            {
                mDeleteButton.interactable = false;
            }
        }

        public void DeleteObject()
        {
            if (mSelected != null)
            {
                mSelected.GetComponent<ASL.ASLObject>().DeleteObject();
            }
            mSelected = null;
            SetDeleteInteractable(mSelected);
        }

        public void StopAllClients()
        {
            var randomObject = FindObjectOfType<ASL.ASLObject>();

            randomObject.GetComponent<ASL.ASLObject>()?.SendAndSetClaim(() =>
            {
                float[] myValue = new float[5];
                myValue[0] = 0;
                myValue[1] = 1;
                myValue[2] = 2;
                myValue[3] = 3;
                myValue[4] = 4;
                randomObject.GetComponent<ASL.ASLObject>()?.SendFloat4(myValue);
            });

        }

    }
}