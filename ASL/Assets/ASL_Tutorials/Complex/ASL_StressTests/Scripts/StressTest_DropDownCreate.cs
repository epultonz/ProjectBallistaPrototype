﻿using System; // for assert
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // for GUI elements: Button, Toggle

namespace StressTesting
{
    public partial class StressTest_DropDownCreate : MonoBehaviour
    {

        // reference to all UI elements in the Canvas
        public Dropdown mCreateMenu = null;
        public StressTest_TheWorld TheWorld = null;

        // Use this for initialization
        void Start()
        {
            Debug.Assert(mCreateMenu != null);
            Debug.Assert(TheWorld != null);
            mCreateMenu.onValueChanged.AddListener(UserSelection);
        }

        PrimitiveType[] kLoadType = {
        PrimitiveType.Cube,     // this is used as menu label, not used
        PrimitiveType.Cube,
        PrimitiveType.Sphere,
        PrimitiveType.Cylinder };

        void UserSelection(int index)
        {
            if (index == 0)
                return;

            mCreateMenu.value = 0; // always show the menu function: Object to create

            // inform the world of user's action
            TheWorld.CreatePrimitive(kLoadType[index]);
        }
    }
}