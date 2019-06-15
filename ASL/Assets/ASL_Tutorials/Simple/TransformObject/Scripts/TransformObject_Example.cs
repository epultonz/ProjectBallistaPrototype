using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleDemos
{
    /// <summary>
    /// Example of how to perform the various Transforms (position, rotation, scale) on an object.
    /// As well as some examples of what to do for additive style movement
    /// </summary>
    public class TransformObject_Example : MonoBehaviour
    {
        /// <summary>Provides an easy way to access the object we want to perform transformations on. </summary>
        public GameObject m_ObjectToManipulate;
        /// <summary>Bool toggling if we are sending a claim, and thus able to send transforms or not</summary>
        public bool m_SendClaim = false;
        /// <summary>Bool triggering if we are doing additive transforms, such as building off of our previous location
        /// instead of just moving to a new position</summary>        
        public bool m_SendAdditiveTransform = false;
        /// <summary>Bool triggering if we should send a single transform or not. Will be set back to false after setting to true
        /// to prevent variables from being sent again</summary>
        public bool m_SendTransform = false;
        /// <summary>Bool triggering if we should reset the variables and location of the cube</summary>
        public bool m_Reset = false;
        /// <summary>
        /// The position we want to move to
        /// </summary>
        public Vector3 m_MoveToPosition;
        /// <summary>
        /// The amount we want to move every Update cycle
        /// </summary>
        public Vector3 m_AdditiveMovementAmount;
        /// <summary>
        /// The amount we want to rotate by. Since this is a quaternion, the result of the rotation may not be intuitive
        /// </summary>
        public Quaternion m_RotateAmount;
        /// <summary>
        /// The amount we want to scale our object by
        /// </summary>
        public Vector3 m_ScaleToAmount;
        /// <summary>
        /// The amount we want to increase our scale by every Update cycle
        /// </summary>
        public Vector3 m_AdditiveScaleAmount;

        /// <summary>
        /// Variable that holds what our position is after we update based on our additive movement, but may not actually represent
        /// our object's localPosition because localPosition gets updated after a packet is received, while this gets 
        /// updated right before that packet gets sent. Without this variable, localPosition would not be updated properly
        /// </summary>
        private Vector3 m_AdditivePosition;
        /// <summary>
        /// Variable that holds what our scale is after we update based on our additive scale amount, but may not actually represent
        /// our object's localScale because localScale gets updated after a packet is received, while this gets updated right
        /// before that packet gets sent. Without this variable, localScale would not be updated properly.
        /// </summary>
        private Vector3 m_AdditiveScale;
        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            //Set scale values to 1 because you can't have a a scale of 0
            m_ScaleToAmount = new Vector3(1, 1, 1);
            m_AdditiveScaleAmount = new Vector3(0.001f, 0.001f, 0.001f);
            m_RotateAmount = Quaternion.identity;
        }

        /*For more examples go to:
        * https://uwb-arsandbox.github.io/ASL/ASLDocumentation/Help/html/d900897e-ff57-8629-2242-59572d40152f.htm Position
        * https://uwb-arsandbox.github.io/ASL/ASLDocumentation/Help/html/aa3f986b-f78a-70ed-e40a-4d7e59f937b4.htm Rotation via *=Quaternion Method
        * https://uwb-arsandbox.github.io/ASL/ASLDocumentation/Help/html/4118c1f0-1630-da54-635e-557215c7a72a.htm Rotation via =Quaternion Method
        * https://uwb-arsandbox.github.io/ASL/ASLDocumentation/Help/html/e57d4b70-fd4a-2612-62f2-fdbb5afb6aae.htm Scale
        */

        /// <summary>
        /// Logic of our demo
        /// </summary>
        void Update()
        {
            if (m_SendClaim)
            {
                if (m_SendAdditiveTransform)
                {
                    //Set position by updating variable whose value only changes when we send a new position
                    //If we used m_ObjectToManipulate.transform.localPosition + m_additivePosition it may not sent the
                    //position accurately because this function can get called again before the packet returns from 
                    //the server, which updates the localPosition. Meaning you will end up sending the same position
                    //more than once. Doing it this way allows for steady, consistent, accurate movement
                    m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
                    {
                        m_AdditivePosition += m_AdditiveMovementAmount; //Add movement amount to our new position
                        m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetLocalPosition(m_AdditivePosition);
                    });

                    //There is no need to have an extra variable for rotation because on the backend rotation is set by
                    //doing *= which is how quaternions should be set. If in your demo it doesn't seem to be rotating properly
                    //it's because quaternions don't work the way you think they do. Try adjusting the w value. 
                    m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
                    {
                        m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetLocalRotation(m_RotateAmount);
                    });

                    //Set the scale this way for the same reason as the position
                    m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
                    {
                        m_AdditiveScale += m_AdditiveScaleAmount;
                        m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetLocalScale(m_AdditiveScale);
                    });
                }
                if (m_SendTransform)
                {
                    //Reset additive positions so if user switches back to additive transform, the transition will make sense (no wild teleports based on outdated values)
                    m_AdditivePosition = m_MoveToPosition; 
                    m_AdditiveScale = m_ScaleToAmount;

                    //Just set position
                    m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
                    {
                        m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetLocalPosition(m_MoveToPosition);
                    });

                    //Just set Rotation
                    m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
                    {
                        m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetLocalQuaternion(m_RotateAmount);
                    });

                    //Just set Scale
                    m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
                    {
                        m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetLocalScale(m_ScaleToAmount);
                    });

                    //Set back to false so these values are only sent once
                    m_SendTransform = false;
                }
            }
            //Reset all values to what they are on scene load and Move object back to original position and orientation
            if (m_Reset)
            {
                //Reset variables
                m_MoveToPosition = new Vector3(0, 0, 0);
                m_AdditivePosition = m_MoveToPosition;
                m_ScaleToAmount = new Vector3(1, 1, 1);
                m_AdditiveScale = new Vector3(0.001f, 0.001f, 0.001f);
                m_RotateAmount = Quaternion.identity;
                m_Reset = false;
                m_SendClaim = false;
                m_SendTransform = false;
                m_SendAdditiveTransform = false;

                //Reset Position
                m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
                {
                    m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetLocalPosition(m_MoveToPosition);
                });

                //Reset Rotation
                m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
                {
                    m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetLocalQuaternion(m_RotateAmount);
                });

                //Reset Scale
                m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
                {
                    m_ObjectToManipulate.GetComponent<ASL.ASLObject>().SendAndSetLocalScale(m_ScaleToAmount);
                });
            }


        }
    }
}
