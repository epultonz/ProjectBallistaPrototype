using System; // for assert
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // for GUI elements: Button, Toggle
using UnityEngine.EventSystems;

namespace StressTesting
{
    public partial class StressTest_MainControllerNoXformControl : MonoBehaviour
    {

        // reference to all UI elements in the Canvas
        public Camera MainCamera = null;
        public StressTest_TheWorld mModel = null;
        public StressTest_ButtonManager mButton = null;
        public Text Object1Pos;
        public Text Object2Pos;
        public Text Object3Pos;
        public Text Object4Pos;
        public Text Object5Pos;

        public GameObject Grandparent = null;
        public GameObject Parent = null;
        public GameObject Child = null;
        public GameObject Capsule = null;
        public GameObject Cube = null;

        // Use this for initialization
        void Start()
        {
            Debug.Assert(MainCamera != null);
            Debug.Assert(mModel != null);
            Debug.Assert(mButton != null);

            Debug.Assert(Object1Pos != null);
            Debug.Assert(Object2Pos != null);
            Debug.Assert(Object3Pos != null);
            Debug.Assert(Object4Pos != null);
            Debug.Assert(Object5Pos != null);

            Object1Pos.text = "Object 1 Pos: " + Grandparent.transform.localPosition;
            Object2Pos.text = "Object 2 Pos: " + Parent.transform.localPosition;
            Object3Pos.text = "Object 3 Pos: " + Child.transform.localPosition;
            Object4Pos.text = "Object 4 Pos: " + Capsule.transform.localPosition;
            Object5Pos.text = "Object 5 Pos: " + Cube.transform.localPosition;

        }

        // Update is called once per frame
        void Update()
        {
            LMBSelect();

            //Update positions to user.
            Object1Pos.text = "Object 1 Pos: " + Grandparent.transform.localPosition;
            Object2Pos.text = "Object 2 Pos: " + Parent.transform.localPosition;
            Object3Pos.text = "Object 3 Pos: " + Child.transform.localPosition;
            Object4Pos.text = "Object 4 Pos: " + Capsule.transform.localPosition;
            Object5Pos.text = "Object 5 Pos: " + Cube.transform.localPosition;
        }

        private void SelectObject(GameObject g)
        {
            GameObject a = mModel.SelectObject(g);
            mButton.SetDeleteInteractable(a);
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