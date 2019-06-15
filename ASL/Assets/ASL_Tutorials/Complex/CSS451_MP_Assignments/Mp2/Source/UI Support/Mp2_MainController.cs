using System; // for assert
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // for GUI elements: Button, Toggle

namespace Mps
{

    public partial class Mp2_MainController : MonoBehaviour
    {

        // reference to all UI elements in the Canvas
        public Camera MainCamera = null;
        public Mp2_XfromControl mXform = null;
        public Mp2_DropDownCreate mCreateMenu = null;
        public Mp2_TheWorld mModel = null;
        public Mp2_ButtonManager mButton = null;


        // Use this for initialization
        void Start()
        {
            Debug.Assert(MainCamera != null);
            Debug.Assert(mXform != null);
            Debug.Assert(mModel != null);

        }

        // Update is called once per frame
        void Update()
        {
            LMBSelect();
        }

        private void SelectObject(GameObject g)
        {
            GameObject a = mModel.SelectObject(g);
            mXform.SetSelectedObject(a);
            mButton.SetDeleteInteractable(a);
        }
    }
}