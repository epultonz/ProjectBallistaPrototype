using System; // for assert
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // for GUI elements: Button, Toggle
using UnityEngine.EventSystems;

namespace StressTesting
{
    public partial class StressTest_MainController : MonoBehaviour
    {

        // reference to all UI elements in the Canvas
        public Camera MainCamera = null;
        public Mps.Mp2_XfromControl mXform = null;
        public StressTest_DropDownCreate mCreateMenu = null;
        public StressTest_TheWorld mModel = null;
        public Mps.Mp2_ButtonManager mButton = null;


        // Use this for initialization
        void Start()
        {
            Debug.Assert(MainCamera != null);
            //        Debug.Assert(mXform != null);
            Debug.Assert(mModel != null);
            Debug.Assert(mCreateMenu != null);
            //        Debug.Assert(mButton != null);

        }

        // Update is called once per frame
        void Update()
        {
            LMBSelect();
        }

        private void SelectObject(GameObject g)
        {
            GameObject a = mModel.SelectObject(g);
            //       mXform.SetSelectedObject(a);
            //       mButton.SetDeleteInteractable(a);
        }

        void LMBSelect()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Debug.Log("Mouse is down");

                // Copied from: https://forum.unity.com/threads/preventing-ugui-mouse-click-from-passing-through-gui-controls.272114/
                if (!EventSystem.current.IsPointerOverGameObject()) // check for GUI
                {

                    RaycastHit hitInfo = new RaycastHit();

                    bool hit = Physics.Raycast(MainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, 1);
                    // 1 is the mask for default layer

                    if (hit)
                        SelectObject(hitInfo.transform.gameObject);
                    else
                        SelectObject(null);
                }
            }
        }
    }
}