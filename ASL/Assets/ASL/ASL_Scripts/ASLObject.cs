﻿using GameSparks.RT;
using UnityEngine;

namespace ASL
{
    /// /// <summary>
    /// ASLObject: ASLObject Partial Class containing all of the functions and variables relating to server actions - actions that affect all players instead of just a single player.
    /// use this class to communicate object information to other players.
    /// </summary>
    public partial class ASLObject : MonoBehaviour
    {
        /// <summary>Flag indicating whether or not this player currently owns this object or not</summary>
        public bool m_Mine { get; private set; }

        /// <summary>The anchor point for AR applications for this object</summary>
        public string m_AnchorPoint { get; private set; }

        /// <summary>The unique id of this object. This id is synced with all other players</summary>
        public string m_Id { get; private set; }

        /// <summary>Callback to be executed once a player successfully claims this object</summary>
        public ClaimCallback m_ClaimCallback { get; private set; }

        /// <summary>Callback to be executed when a user passes a float value</summary>
        public FloatCallback m_FloatCallback { get; private set; }

        /// <summary>Callback to be executed when a claim is rejected</summary>
        public ClaimCancelledRecoveryCallback m_ClaimCancelledRecoveryCallback { get; private set; }

        /// <summary>Callback to be executed when an object is released. After the callback is executed, it will be set to null to prevent multiple callback's from stacking
        /// on this object after each release. To ensure you aren't attempting to execute this callback after its been set to null, make sure you always reset the callback
        /// when you obtain it if is important to have a release function of some sort</summary>
        public ReleaseFunction m_ReleaseFunction { get; private set; }

        /// <summary>Callback to be executed after an ASL Object is instantiated</summary>
        public ASLGameObjectCreatedCallback m_ASLGameObjectCreatedCallback { get; private set; }

        /// <summary>Flag indicating whether or not there are any outstanding (not recognized by other users) claims on this object</summary>
        public bool m_OutStandingClaims { get; private set; }

        /// <summary>Delegate for the Claim callback function </summary>
        public delegate void ClaimCallback();

        /// <summary> Delegate for the Float callback function </summary>
        /// <param name="_i">The float(s) to be passed into the user defined function</param>
        public delegate void FloatCallback(float[] _i);

        /// <summary>Delegate for the release callback function</summary>
        /// <param name="_go">The GameObject to be passed into the user defined function. </param>
        public delegate void ReleaseFunction(GameObject _go);

        /// <summary>Delegate for the claim canceled (rejected) callback function</summary>
        /// <param name="_id">The id of the object that got its claim rejected</param>
        /// <param name="cancelledCount">How many claims were rejected/canceled. Can be null</param>
        public delegate void ClaimCancelledRecoveryCallback(string _id, int cancelledCount);

        /// <summary>Delegate for the GameObject creation function which is executed when this object is instantiated</summary>
        /// <param name="_go"></param>
        public delegate void ASLGameObjectCreatedCallback(GameObject _go);     
        
        /// <summary>The number of outstanding claims for this object. </summary>
        public int m_OutstandingClaimCallbackCount { get; set; }


        /// <summary>
        /// Claims an object for the user until someone steals it or the passed in claim time is reached. Changing the time you hold onto an object and 
        /// deciding to reset or not the current amount of time you have held it is generally not recommended, but does have occasional applications
        /// </summary>
        /// <param name="callback">The callback to be called when the claim is approved by the server</param>
        /// <param name="claimTimeOut">The amount of time in milliseconds the user will own this object. If set to less than 0,
        /// then the user will own the object until it gets stolen.</param>
        /// /// <param name="resetClaimTimeout">Flag indicating whether or not a claim should reset the claim timer for this object</param>
        /// <example><code>
        /// void SomeFunction()
        /// {
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetClaim(() =>
        ///     {
        ///         //Whatever code we want to execute after we have successfully claimed this object, such as:
        ///         gameobject.GetComponent&lt;ASL.ASLObject&gt;().DeleteObject(); //Delete our object
        ///         //or
        ///         gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetLocalPosition(new Vector3(5, 0, -2)); //Move our object in its local space to 5, 0, -2
        ///     });
        /// }
        /// </code>
        /// <code>
        /// void SomeFunction()
        /// {
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetClaim(ActionToTake);
        /// }
        /// private void ActionToTake()
        /// {
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;().DeleteObject(); //Delete our object
        /// }
        /// void SomeOtherFunction()
        /// {
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetClaim(ADifferentActionToTake, 2000); //Hold onto this object before releasing it to the server for 2 seconds
        /// }
        /// private void ADifferentActionToTake()
        /// {
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetLocalPosition(new Vector3(5, 0, -2)); //Move our object in its local space to 5, 0, -2
        /// }
        /// </code>
        /// <code>
        /// void SomeFunction()
        /// {
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetClaim(() =>
        ///     {
        ///         //Whatever code we want to execute after we have successfully claimed this object, such as:
        ///         gameobject.GetComponent&lt;ASL.ASLObject&gt;().DeleteObject(); //Delete our object
        ///         //or
        ///         gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetLocalPosition(new Vector3(5, 0, -2)); //Move our object in its local space to 5, 0, -2
        ///     }, 0); //Hold onto this object until someone steals it
        /// }
        /// </code>
        /// <code>
        /// void SomeFunction()
        /// {
        ///     //Claim an object for 1 second, but don't reset whatever our current time length of time we have already held on to it for.
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetClaim(SomeActionToTake, 1000, false); 
        /// }
        /// private void SomeActionToTake()
        /// {
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;().DeleteObject(); //Delete our object
        /// }
        /// </code>
        /// <code>
        /// void SomeFunction()
        /// {
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetClaim(() =>
        ///     {
        ///         //Whatever code we want to execute after we have successfully claimed this object, such as:
        ///         gameobject.GetComponent&lt;ASL.ASLObject&gt;().DeleteObject(); //Delete our object
        ///         //or
        ///         gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetLocalPosition(new Vector3(5, 0, -2)); //Move our object in its local space to 5, 0, -2
        ///     }, 0, false); //Hold on to this object until someone steals it, but don't reset whatever our current length of time we have already held on to it for
        /// }
        /// </code></example>
        public void SendAndSetClaim(ClaimCallback callback, int claimTimeOut = 1000, bool resetClaimTimeout = true)
        {
            if (!m_Mine) //If we already own the object, don't send anything and instead call our callback right away
            {
                //Debug.Log("Outstanding Claims: " + m_OutStandingClaims);
                if (!m_OutStandingClaims)
                {
                    m_OutStandingClaims = true;
                    using (RTData data = RTData.Get())
                    {
                        data.SetString((int)GameController.DataCode.Id, m_Id);
                        GameSparksManager.Instance().GetRTSession().SendData((int)GameSparksManager.OpCode.Claim, GameSparksRT.DeliveryIntent.RELIABLE, data);                      
                    }
                }
                m_ClaimCallback += callback;
                m_OutstandingClaimCallbackCount++;
            }
            else
            {
                callback();
            }
            if (resetClaimTimeout)
            {
                m_ClaimReleaseTimer = 0; //Reset release timer
                m_ClaimTime = claimTimeOut; //Reset claim length
            }
        }

        /// <summary>
        /// Send and sets this objects color for the user who called this function and for all other players
        /// </summary>
        /// <param name="_myColor">The color to be set for the user who called this function</param>
        /// <param name="_opponentsColor">The color to be set for everyone who did not call this function (everyone else)</param>
        /// <example><code>
        /// void SomeFunction()
        /// {
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetClaim(() =>
        ///     {
        ///         gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetObjectColor(new Color(0, 0, 0, 1),  new Color(1, 1, 1, 1));
        ///     });
        /// }
        /// </code></example>
        public void SendAndSetObjectColor(Color _myColor, Color _opponentsColor)
        {
            if (m_Mine)
            {
                transform.GetComponent<Renderer>().material.color = _myColor;
                using (RTData data = RTData.Get())
                {
                    data.SetString((int)GameController.DataCode.Id, m_Id);
                    data.SetVector4((int)GameController.DataCode.MyColor, _myColor);
                    data.SetVector4((int)GameController.DataCode.OpponentColor, _opponentsColor);
                    GameSparksManager.Instance().GetRTSession().SendData((int)GameSparksManager.OpCode.SetObjectColor, GameSparksRT.DeliveryIntent.RELIABLE, data);
                }
                
            }
        }

        /// <summary>
        /// Deletes this object for all users
        /// </summary>
        /// <example><code>
        /// void SomeFunction()
        /// {
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetClaim(() =>
        ///     {
        ///         gameobject.GetComponent&lt;ASL.ASLObject&gt;().DeleteObject();
        ///     });
        /// }
        /// </code></example>
        public void DeleteObject()
        {
            if (gameObject && m_Mine)
            {
                using (RTData data = RTData.Get())
                {
                    data.SetString((int)GameController.DataCode.Id, m_Id);
                    GameSparksManager.Instance().GetRTSession().SendData((int)GameSparksManager.OpCode.DeleteObject, GameSparksRT.DeliveryIntent.RELIABLE, data);
                }
            }
        }

        /// <summary>
        /// Sends and sets the local transform for this object for all users
        /// </summary>
        /// <param name="_localPosition">The new local position for this object</param>
        /// <example><code>
        /// void SomeFunction()
        /// {
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetClaim(() =>
        ///     {
        ///         gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetLocalPosition(new Vector3(1, 2, 5));
        ///     });
        /// }
        /// </code></example>
        public void SendAndSetLocalPosition(Vector3? _localPosition)
        {
            if (m_Mine) //Can only send a transform if we own the object
            {
                if (_localPosition.HasValue)
                {
                    using (RTData data = RTData.Get())
                    {
                        data.SetString((int)GameController.DataCode.Id, m_Id);
                        data.SetVector3((int)GameController.DataCode.LocalPosition, new Vector3(_localPosition.Value.x, _localPosition.Value.y, _localPosition.Value.z));
                        GameSparksManager.Instance().GetRTSession().SendData((int)GameSparksManager.OpCode.LocalTransformUpdate, GameSparksRT.DeliveryIntent.RELIABLE, data);
                    }
                }
                else //Send my position as is, not a new position as there was no new position passed in.
                {
                    using (RTData data = RTData.Get())
                    {
                        data.SetString((int)GameController.DataCode.Id, m_Id);
                        data.SetVector3((int)GameController.DataCode.LocalPosition, new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z));
                        GameSparksManager.Instance().GetRTSession().SendData((int)GameSparksManager.OpCode.LocalTransformUpdate, GameSparksRT.DeliveryIntent.RELIABLE, data);
                    }
                }
            }
        }

        /// <summary>
        /// Sends and sets the local rotation for this object for all users
        /// </summary>
        /// <param name="_localRotation">The new local rotation for this object. Is set using Quaternion *=</param>
        /// <example><code>
        /// void SomeFunction()
        /// {
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetClaim(() =>
        ///     {
        ///         gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetLocalRotation(new Quaternion(0, 80, 60, 1));
        ///     });
        /// }
        /// </code></example>
        public void SendAndSetLocalRotation(Quaternion? _localRotation)
        {
            if (_localRotation.HasValue)
            {
                using (RTData data = RTData.Get())
                {
                    data.SetString((int)GameController.DataCode.Id, m_Id);
                    data.SetVector4((int)GameController.DataCode.LocalRotation, new Vector4(_localRotation.Value.x, _localRotation.Value.y, _localRotation.Value.z, _localRotation.Value.w));
                    GameSparksManager.Instance().GetRTSession().SendData((int)GameSparksManager.OpCode.LocalRotationUpdate, GameSparksRT.DeliveryIntent.RELIABLE, data);
                }
            }
            else //Send my position as is, not a new position as there was no new position passed in.
            {
                using (RTData data = RTData.Get())
                {
                    data.SetString((int)GameController.DataCode.Id, m_Id);
                    data.SetVector4((int)GameController.DataCode.LocalRotation, new Vector4(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z, transform.localRotation.w));
                    GameSparksManager.Instance().GetRTSession().SendData((int)GameSparksManager.OpCode.LocalRotationUpdate, GameSparksRT.DeliveryIntent.RELIABLE, data);
                }
            }
        }

        /// <summary>
        /// Sends and sets the local rotation for this object for all users. This option should only be used if you know how a quaternion works. Otherwise, use SendAndSetLocalRotation
        /// which, when received, multiplies your current rotation quaternion by your new one, moving you to the rotation you (usually) desire. This option ignores whatever your previous
        /// rotation was and instead just sets it. This is nice for going to a specific location, but is not good for moving seamlessly to a new position, aka, for rotating over time - for
        /// that, use SendAndSetRotation. 
        /// </summary>
        /// <param name="_localRotation">The new local rotation for this object. Is set using Quaternion =</param>
        /// <example><code>
        /// void SomeFunction()
        /// {
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetClaim(() =>
        ///     {
        ///         gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetLocalRotation(new Quaternion(0, 80, 60, 1));
        ///     });
        /// }
        /// </code></example>
        public void SendAndSetLocalQuaternion(Quaternion? _localRotation)
        {
            if (_localRotation.HasValue)
            {
                using (RTData data = RTData.Get())
                {
                    data.SetString((int)GameController.DataCode.Id, m_Id);
                    data.SetVector4((int)GameController.DataCode.LocalRotation, new Vector4(_localRotation.Value.x, _localRotation.Value.y, _localRotation.Value.z, _localRotation.Value.w));
                    GameSparksManager.Instance().GetRTSession().SendData((int)GameSparksManager.OpCode.LocalQuaternionUpdate, GameSparksRT.DeliveryIntent.RELIABLE, data);
                }
            }
            else //Send my position as is, not a new position as there was no new position passed in.
            {
                using (RTData data = RTData.Get())
                {
                    data.SetString((int)GameController.DataCode.Id, m_Id);
                    data.SetVector4((int)GameController.DataCode.LocalRotation, new Vector4(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z, transform.localRotation.w));
                    GameSparksManager.Instance().GetRTSession().SendData((int)GameSparksManager.OpCode.LocalQuaternionUpdate, GameSparksRT.DeliveryIntent.RELIABLE, data);
                }
            }
        }

        /// <summary>
        /// Send and set the local scale for this object for all users
        /// </summary>
        /// <param name="_localScale">The new local scale for this object</param>
        /// <example><code>
        /// void SomeFunction()
        /// {
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetClaim(() =>
        ///     {
        ///         gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetLocalScale(new Vector3(.5, 2, .5));
        ///     });
        /// }
        /// </code></example>
        public void SendAndSetLocalScale(Vector3? _localScale)
        {
            if (_localScale.HasValue)
            {
                using (RTData data = RTData.Get())
                {
                    data.SetString((int)GameController.DataCode.Id, m_Id);
                    data.SetVector3((int)GameController.DataCode.LocalScale, new Vector3(_localScale.Value.x, _localScale.Value.y, _localScale.Value.z));
                    GameSparksManager.Instance().GetRTSession().SendData((int)GameSparksManager.OpCode.LocalScaleUpdate, GameSparksRT.DeliveryIntent.RELIABLE, data);
                }
            }
            else //Send my position as is, not a new position as there was no new position passed in.
            {
                using (RTData data = RTData.Get())
                {
                    data.SetString((int)GameController.DataCode.Id, m_Id);
                    data.SetVector3((int)GameController.DataCode.LocalScale, new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z));
                    GameSparksManager.Instance().GetRTSession().SendData((int)GameSparksManager.OpCode.LocalScaleUpdate, GameSparksRT.DeliveryIntent.RELIABLE, data);
                }
            }
        }

        /// <summary>
        /// Send and set the AR anchor point for this object for all users. 
        /// </summary>
        /// <param name="_anchorPoint">The anchor point for this object to reference</param>
        /// <example><code>
        /// void SomeFunction()
        /// {
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetClaim(() =>
        ///     {
        ///         gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetAnchorPoint("Your Anchor Point Here");
        ///     });
        /// }
        /// </code></example>
        public void SendAndSetAnchorPoint(string _anchorPoint)
        {
            using (RTData data = RTData.Get())
            {
                data.SetString((int)GameController.DataCode.Id, m_Id);
                data.SetString((int)GameController.DataCode.AnchorPoint, _anchorPoint);
                GameSparksManager.Instance().GetRTSession().SendData((int)GameSparksManager.OpCode.AnchorPointUpdate, GameSparksRT.DeliveryIntent.RELIABLE, data);
            }           
        }

        /// <summary>
        /// Send and set  up to 4 float value(s). The float value(s) will then be processed by a user defined function.
        /// A switch and case statement system can be used to create a function that can handle more than just 4 floats
        /// if that is needed
        /// GameSparks does not provide the capability to send a float array, but they do allow a Vector4. Therefore,
        /// this function actually converts the user's float array into a vector4, meaning if the array contains more
        /// than four floats, those floats will be lost. There is a way to serialize data and send an array that way
        /// https://docs.gamesparks.com/tutorials/database-access-and-cloud-storage/c-sharp-object-serialization-for-gamesparks.html
        /// But until it is determined that implementing such a method is necessary, it will not be implemented in ASL
        /// </summary>
        /// <param name="f">The float value to be passed to all users</param>
        /// <example>
        /// <code>
        /// void SomeFunction()
        /// {
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetClaim(() =>
        ///     {
        ///         float[] myValue = new float[1];
        ///         myValue[0] = 3.5;
        ///         gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendFloat4(myValue); //In this example, playerHealth would be updated to 3.5 for all users
        ///     });
        /// }
        /// </code><code>
        /// The same gameobject would then also have these qualities: 
        /// 
        /// void Start()
        /// {
        ///     //Or if this object was created dynamically, then to have this function assigned on creation instead of in start like this one is
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;()._LocallySetFloatCallback(UserDefinedFunction) 
        /// }
        /// public void UserDefinedFunction(float[] f)
        /// {
        ///     //Update some value for all users based on f. 
        ///     //Example:
        ///     playerHealth = f[0]; //Where playerHealth is shown to kept track/shown to all users
        /// }
        ///</code>
        /// It is possible to use this function to send more than 4 variables if the user sets up the function to execute upon receiving SendFloat4 to include a switch/case statement
        /// with the final value in the float array being what determines how the other three values are handled. See below for an example
        /// <code>
        /// //For example, use this function to update player stats using the same SendFloat4 UserDefinedFunction that can also update velocity and score
        /// void SomeFunction()
        /// {
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetClaim(() =>
        ///     {
        ///         float[] myValue = new float[1];
        ///         myValue[0] = 3.5;
        ///         myValue[1] = 0;
        ///         myValue[2] = 1.2
        ///         myValue[3] = 2
        ///         //In this example, playerHealth would be updated to 3.5 for all users, playerArmor to 0, playerSpeedBuff to 1.2, and the switch case to properly assign these values, 2
        ///         gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendFloat4(myValue); 
        ///     });
        /// }
        /// //For example, use this function to update velocity using the same SendFloat4 UserDefinedFunction that can also update player stats and score
        /// void SomeOtherFunction()
        /// {
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendAndSetClaim(() =>
        ///     {
        ///         float[] myValue = new float[1];
        ///         myValue[0] = 17.8;
        ///         myValue[1] = 180.2;
        ///         myValue[3] = 1
        ///         //In this example, velocity would be set to 17.8 and direction to 180.2
        ///         gameobject.GetComponent&lt;ASL.ASLObject&gt;().SendFloat4(myValue); 
        ///     });
        /// }
        /// 
        /// void Start()
        /// {
        ///     //Or if this object was created dynamically, then to have this function assigned on creation instead of in start like this one is
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;()._LocallySetFloatCallback(UserDefinedFunction) 
        /// }
        /// public void UserDefinedFunction(float[] f)
        /// {
        ///     //Update some value for all users based on f. 
        ///     //Example:
        ///     switch (f[3])
        ///     {
        ///         case 0:
        ///             //Update score based on f[0] for example
        ///         break;
        ///         case 1:
        ///             //Update player velocity and direction with f[0] and f[1] for example
        ///             playerVelocity = f[0]; //Velocity gets set to 17.8
        ///             playerDirection = f[1]; //Player Direction gets set to 180.2
        ///         break;
        ///         case 2:
        ///             playerHealth = f[0]; //Health gets assigned to 3.5
        ///             playerArmor = f[1]; //Armor gets assigned to 0
        ///             playerSpeedBuff = f[2]; SpeedBuff gets assigned to 1.2
        ///         break;
        ///     }
        /// }
        /// </code>
        ///</example>
        public void SendFloat4(float[] f)
        {
            if (m_Mine) //Can only send a transform if we own the object
            {
                if (f.Length > 4)
                {
                    Debug.LogWarning("SendFloat4 float array cannot have a length greater than 4 due to GameSpark restrictions. See documentation for details.");
                }
                using (RTData data = RTData.Get())
                {
                    Vector4 floats = Vector4.zero; 
                    //Manually convert float[] to Vector4
                    if (f.Length >= 1)
                    {
                        floats.x = f[0];
                    }
                    if (f. Length >= 2)
                    {
                        floats.y = f[1];
                    }
                    if (f.Length >= 3)
                    {
                        floats.z = f[2];
                    }
                    if (f.Length >= 4)
                    {
                        floats.w = f[3];
                    }

                    data.SetString((int)GameController.DataCode.Id, m_Id);
                    data.SetVector4((int)GameController.DataCode.MyFloats, floats);
                    GameSparksManager.Instance().GetRTSession().SendData((int)GameSparksManager.OpCode.SetFloat, GameSparksRT.DeliveryIntent.RELIABLE, data);
                }
            }
        }



    }
}
