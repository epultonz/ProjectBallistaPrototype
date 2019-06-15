using System; // for assert
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // for GUI elements: Button, Toggle
using UnityEngine.EventSystems;

namespace Mps
{
    public partial class Mp2_MainController : MonoBehaviour
    {

        // Mouse click selection 
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