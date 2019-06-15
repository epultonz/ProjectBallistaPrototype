using GameSparks.RT;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ASL
{
    /// <summary>
    /// GameController_PacketHandler: Part of the GameController class that processes all of the packets received by the relay server, passed to these specific functions from the GameSparksManager class
    /// </summary>
    public partial class GameController : MonoBehaviour
    {
        /// <summary>
        /// Looks for and assigns any ASLObjects that do not have a unique ID yet. This ID is given through the relay server. 
        /// This function is triggered by a packet received from the relay server.
        /// </summary>
        /// <param name="_packet">The packet containing the unique ID of an ASL Object</param>
        public void SetObjectID(RTPacket _packet)
        {
            //Cycle through all items in the dictionary looking for the items with invalid keys 
            //For objects that start in the scene, their keys are originally set to be invalid so we set them properly through the RT Server
            //Ensuring all clients have the same key for each object
            foreach(KeyValuePair<string, ASLObject> _object in ASLHelper.m_ASLObjects)
            {
                //Since GUIDs aren't numbers, if we find a number, then we know it's a fake key value and it should be updated to match all other clients
                if (int.TryParse(_object.Key, out int result))
                {
                    ASLHelper.m_ASLObjects.Add(_packet.Data.GetString((int)DataCode.Id), _object.Value);
                    ASLHelper.m_ASLObjects.Remove(_object.Key);
                    InitializeStartObject(_packet.Data.GetString((int)DataCode.Id), _object.Value);
                    break;
                }
            }
        }

        /// <summary>
        /// Destroys an ASL Object based upon its ID. This function is triggered by a packet received from the relay server.
        /// </summary>
        /// <param name="_packet">The packet from the relay server containing the ID of what ASL Object to delete</param>
        public void DeleteObject(RTPacket _packet)
        {
            if (ASLHelper.m_ASLObjects.TryGetValue(_packet.Data.GetString((int)DataCode.Id) ?? string.Empty, out ASLObject myObject))
            {
                ASLHelper.m_ASLObjects.Remove(_packet.Data.GetString((int)DataCode.Id));
                Destroy(myObject.gameObject);       
            }
        }

        /// <summary>
        /// Updates the local transform of an ASL Object based upon its ID. This function is triggered by a packet received from the relay server.
        /// </summary>
        /// <param name="_packet">The packet from the relay server containing the ID of what ASL Object to modified
        /// and the Vector3 of the object's new position</param>
        public void SetLocalTransform(RTPacket _packet)
        {
            if (ASLHelper.m_ASLObjects.TryGetValue(_packet.Data.GetString((int)DataCode.Id) ?? string.Empty, out ASLObject myObject))
            {
                myObject.transform.localPosition = (Vector3)_packet.Data.GetVector3((int)DataCode.LocalPosition);
            }
        }

        /// <summary>
        /// Updates the local rotation of an ASL Object based upon its ID. This function is triggered by a packet received from the relay server.
        /// </summary>
        /// <param name="_packet">The packet from the relay server containing the ID of what ASL Object to modified
        /// and the Vector4 of the object's new rotation</param>
        public void SetLocalRotation(RTPacket _packet)
        {
            if (ASLHelper.m_ASLObjects.TryGetValue(_packet.Data.GetString((int)DataCode.Id) ?? string.Empty, out ASLObject myObject))
            {
                myObject.transform.localRotation *= new Quaternion
                    (_packet.Data.GetVector4((int)DataCode.LocalRotation).Value.x,
                    _packet.Data.GetVector4((int)DataCode.LocalRotation).Value.y,
                    _packet.Data.GetVector4((int)DataCode.LocalRotation).Value.z,
                    _packet.Data.GetVector4((int)DataCode.LocalRotation).Value.w);
            }
        }

        /// <summary>
        /// Updates the local rotation of an ASL Object based upon its ID. This function is triggered by a packet received from the relay server. Unlike SetLocationRotation
        /// this function will not set the rotation by *=, meaning it does not transition nicely from the previous rotation, but instead teleports to the new rotation given
        /// You will most likely want to use SetLocalRotation.
        /// </summary>
        /// <param name="_packet">The packet from the relay server containing the ID of what ASL Object to modified
        /// and the Vector4 of the object's new rotation</param>
        public void SetLocalQuaternion(RTPacket _packet)
        {
            if (ASLHelper.m_ASLObjects.TryGetValue(_packet.Data.GetString((int)DataCode.Id) ?? string.Empty, out ASLObject myObject))
            {
                myObject.transform.localRotation = new Quaternion
                    (_packet.Data.GetVector4((int)DataCode.LocalRotation).Value.x,
                    _packet.Data.GetVector4((int)DataCode.LocalRotation).Value.y,
                    _packet.Data.GetVector4((int)DataCode.LocalRotation).Value.z,
                    _packet.Data.GetVector4((int)DataCode.LocalRotation).Value.w);
            }
        }

        /// <summary>
        /// Updates the local scale of an ASL Object based upon its ID. This function is triggered by a packet received from the relay server.
        /// </summary>
        /// <param name="_packet">The packet from the relay server containing the ID of what ASL Object to modified
        /// and the Vector3 of the object's new scale</param>
        public void SetLocalScale(RTPacket _packet)
        {
            if (ASLHelper.m_ASLObjects.TryGetValue(_packet.Data.GetString((int)DataCode.Id) ?? string.Empty, out ASLObject myObject))
            {
                myObject.transform.localScale = ((Vector3)_packet.Data.GetVector3((int)DataCode.LocalScale));
            }
        }

        /// <summary>
        /// Updates the Anchor Point of an ASL Object based upon its ID. The anchor point is used for AR applications.
        /// This function is triggered by a packet received from the relay server.
        /// </summary>
        /// <param name="_packet">The packet from the relay server containing the ID of what ASL Object to modified
        /// and the object's new Anchor Point information</param>
        public void SetAnchorPoint(RTPacket _packet)
        {
            if (ASLHelper.m_ASLObjects.TryGetValue(_packet.Data.GetString((int)DataCode.Id) ?? string.Empty, out ASLObject myObject))
            {               
                myObject._LocallySetAnchorPoint(_packet.Data.GetString((int)DataCode.AnchorPoint));
            }
        }

        /// <summary>
        /// Finds and claims a specified object and updates everybody's permission for that object. This function is triggered by a packet received from the relay server.
        /// </summary>
        /// <param name="_packet">The packet containing the id of the object to claim</param>
        public void SetObjectClaim(RTPacket _packet)
        {
            if (ASLHelper.m_ASLObjects.TryGetValue(_packet.Data.GetString((int)DataCode.Id) ?? string.Empty, out ASLObject myObject))
            {
                if (GameSparksManager.m_PlayerIds.TryGetValue(_packet.Data.GetString((int)DataCode.Player), out bool me))
                {
                    if (me) //If this is the player who sent the claim
                    {                        
                        myObject._LocallySetClaim(true);
                        myObject.m_ClaimCallback?.Invoke();
                        myObject.m_OutstandingClaimCallbackCount = 0;
                        myObject._LocallyRemoveClaimCallbacks();
                    }
                    else //This is not the player who sent the claim - remove any claims this player may have (shouldn't be any)
                    {
                        myObject._LocallySetClaim(false);
                        myObject._LocallyRemoveClaimCallbacks();
                    }
                }
            }
        }

        /// <summary>
        /// Releases an object so another user can claim it. This function will also call this object's release function if it exists.
        /// This function is triggered by a packet received from the relay server.
        /// </summary>
        /// <param name="_packet">The packet containing the id of the object that another player wants to claim</param>
        public void ReleaseClaimedObject(RTPacket _packet)
        {
            if (ASLHelper.m_ASLObjects.TryGetValue(_packet.Data.GetString((int)DataCode.Id) ?? string.Empty, out ASLObject myObject))
            {
                if (GameSparksManager.m_PlayerIds.TryGetValue(_packet.Data.GetString((int)DataCode.OldPlayer), out bool currentOwner))
                {
                    if (currentOwner) //If this client is the current owner
                    {
                        //Send a packet to new owner informing them that the previous owner (this client) no longer owns the object
                        //So they can do whatever they want with it now.
                        using (RTData data = RTData.Get())
                        {
                            data.SetString((int)DataCode.Id, myObject.m_Id);
                            data.SetString((int)DataCode.Player, _packet.Data.GetString((int)DataCode.Player));
                            GameSparksManager.Instance().GetRTSession().SendData((int)GameSparksManager.OpCode.ClaimFromPlayer, 
                                GameSparksRT.DeliveryIntent.RELIABLE, data, (int)_packet.Data.GetInt((int)DataCode.PlayerPeerId));
                        }
                        myObject.m_ReleaseFunction?.Invoke(gameObject); //If user wants to do something before object is released - let them do so
                        myObject._LocallyRemoveReleaseCallback();

                        myObject._LocallySetClaim(false);
                        myObject._LocallyRemoveClaimCallbacks();
                    }                    
                }
            }
        }

        /// <summary>
        /// Get the claim to an object that was previously owned by another player. This function is triggered by a packet received from the relay server.
        /// </summary>
        /// <param name="_packet">The packet sent by the previous owner of this object, 
        /// it contains the id of the object to be claimed by the receiver of this packet.</param>
        public void ObjectClaimReceived(RTPacket _packet)
        {
            if (ASLHelper.m_ASLObjects.TryGetValue(_packet.Data.GetString((int)DataCode.Id) ?? string.Empty, out ASLObject myObject))
            {
                myObject._LocallySetClaim(true);
                //Call the function the user passed into original claim as they now have "complete control" over the object
                myObject.m_ClaimCallback?.Invoke();
                myObject.m_OutstandingClaimCallbackCount = 0;
                myObject._LocallyRemoveClaimCallbacks();
            }
        }

        /// <summary>
        /// Sets the object specified by the id contained in _packet to the color specified in _packet. This function is triggered by a packet received from the relay server.
        /// </summary>
        /// <param name="_packet">The packet containing the id of the object to change the color of, the color for the owner of the object,
        /// and the color for those who don't own the object</param>
        public void SetObjectColor(RTPacket _packet)
        {
            if (ASLHelper.m_ASLObjects.TryGetValue(_packet.Data.GetString((int)DataCode.Id) ?? string.Empty, out ASLObject myObject))
            {
                if (GameSparksManager.m_PlayerIds.TryGetValue(_packet.Data.GetString((int)DataCode.Player), out bool sender))
                {
                    if (sender) //If this was the one who sent the color
                    {
                        myObject.GetComponent<Renderer>().material.color = (Color)_packet.Data.GetVector4((int)DataCode.MyColor);
                    }
                    else //Everyone else
                    {
                        myObject.GetComponent<Renderer>().material.color = (Color)_packet.Data.GetVector4((int)DataCode.OpponentColor);
                    }
                }
            }
        }

        /// <summary>
        /// This function spawns a primitive object with ASL attached as a component. This function is triggered by a packet received from the relay server.
        /// </summary>
        /// <param name="_packet">The packet containing the id of the object to create, what type of primitive to create, where to create it, and depending on what the user inputted, may
        /// contain its parent's id, a callback function's class name and function name that is called upon creation, and a callback function's class name and function name
        /// that will get called whenever a claim for that object is rejected.</param>
        public void SpawnPrimitive(RTPacket _packet)
        {
            GameObject newASLObject = GameObject.CreatePrimitive((PrimitiveType)(int)_packet.Data.GetInt((int)DataCode.PrimitiveType));
            //Do we need to set the parent?
            if (_packet.Data.GetString((int)DataCode.ParentId) != string.Empty || _packet.Data.GetString((int)DataCode.ParentId) != null)
            {
                SetParent(newASLObject, _packet.Data.GetString((int)DataCode.ParentId));
            }

            newASLObject.transform.localPosition = (Vector3)_packet.Data.GetVector3((int)DataCode.LocalPosition);
            newASLObject.transform.localRotation = new Quaternion(_packet.Data.GetVector4((int)DataCode.LocalRotation).Value.x, _packet.Data.GetVector4((int)DataCode.LocalRotation).Value.y,
                    _packet.Data.GetVector4((int)DataCode.LocalRotation).Value.z, _packet.Data.GetVector4((int)DataCode.LocalRotation).Value.w);

            //Set ID
            newASLObject.AddComponent<ASLObject>()._LocallySetID(_packet.Data.GetString((int)DataCode.Id));


            //If we have the means to set up the recovery callback function - then do it
            if (_packet.Data.GetString((int)DataCode.ClaimRecoveryClassName) != string.Empty && _packet.Data.GetString((int)DataCode.ClaimRecoveryClassName) != null &&
                _packet.Data.GetString((int)DataCode.ClaimRecoveryFunctionName) != string.Empty && _packet.Data.GetString((int)DataCode.ClaimRecoveryFunctionName) != null)
            {
                Type callerClass = Type.GetType(_packet.Data.GetString((int)DataCode.ClaimRecoveryClassName));
                newASLObject.GetComponent<ASLObject>()._LocallySetClaimCancelledRecoveryCallback((ASLObject.ClaimCancelledRecoveryCallback)Delegate.CreateDelegate(typeof(ASLObject.ClaimCancelledRecoveryCallback),
                    callerClass, _packet.Data.GetString((int)DataCode.ClaimRecoveryFunctionName)));
            }

            //If we have the means to set up the SendFloat callback function - then do it
            if (_packet.Data.GetString((int)DataCode.SendFloatClassName) != string.Empty && _packet.Data.GetString((int)DataCode.SendFloatClassName) != null &&
                _packet.Data.GetString((int)DataCode.SendFloatFunctionName) != string.Empty && _packet.Data.GetString((int)DataCode.SendFloatFunctionName) != null)
            {
                Type callerClass = Type.GetType(_packet.Data.GetString((int)DataCode.SendFloatClassName));
                newASLObject.GetComponent<ASLObject>()._LocallySetFloatCallback(
                    (ASLObject.FloatCallback)Delegate.CreateDelegate(typeof(ASLObject.FloatCallback),
                    callerClass, _packet.Data.GetString((int)DataCode.SendFloatFunctionName)));
            }

            //Add object to our dictionary
            ASLHelper.m_ASLObjects.Add(_packet.Data.GetString((int)DataCode.Id), newASLObject.GetComponent<ASLObject>());

            //If this client is the creator of this object, then call the ASLGameObjectCreatedCallback if it exists for this object
            if (GameSparksManager.m_PlayerIds.TryGetValue(_packet.Data.GetString((int)DataCode.Player), out bool isCreator))
            {
                if (isCreator && _packet.Data.GetString((int)DataCode.InstantiatedGameObjectClassName) != string.Empty 
                    && _packet.Data.GetString((int)DataCode.InstantiatedGameObjectClassName) != null &&
                _packet.Data.GetString((int)DataCode.InstantiatedGameObjectFunctionName) != string.Empty && _packet.Data.GetString((int)DataCode.InstantiatedGameObjectFunctionName) != null)
                {
                    //Find Callback function
                    Type callerClass = Type.GetType(_packet.Data.GetString((int)DataCode.InstantiatedGameObjectClassName));
                    newASLObject.GetComponent<ASLObject>()._LocallySetGameObjectCreatedCallback(
                        (ASLObject.ASLGameObjectCreatedCallback)Delegate.CreateDelegate(typeof(ASLObject.ASLGameObjectCreatedCallback),callerClass, 
                        _packet.Data.GetString((int)DataCode.InstantiatedGameObjectFunctionName)));
                    //Call function
                    newASLObject.GetComponent<ASLObject>().m_ASLGameObjectCreatedCallback.Invoke(newASLObject);
                }
            }
        }

        /// <summary>
        /// This function spawns a prefab object with ASL attached as a component. This function is triggered by a packet received from the relay server.
        /// </summary>
        /// <param name="_packet">The packet containing the id of the object to create, what prefab to create it with, where to create it, and depending on what the user inputted, may
        /// contain its parent's id, a callback function's class name and function name that is called upon creation, and a callback function's class name and function name
        /// that will get called whenever a claim for that object is rejected.</param>
        public void SpawnPrefab(RTPacket _packet)
        {
            GameObject newASLObject = Instantiate(Resources.Load(@"MyPrefabs\" + _packet.Data.GetString((int)DataCode.PrefabName))) as GameObject;
            //Do we need to set the parent?
            if (_packet.Data.GetString((int)DataCode.ParentId) != string.Empty || _packet.Data.GetString((int)DataCode.ParentId) != null)
            {
                SetParent(newASLObject, _packet.Data.GetString((int)DataCode.ParentId));
            }

            newASLObject.transform.localPosition = (Vector3)_packet.Data.GetVector3((int)DataCode.LocalPosition);
            newASLObject.transform.localRotation = new Quaternion(_packet.Data.GetVector4((int)DataCode.LocalRotation).Value.x, _packet.Data.GetVector4((int)DataCode.LocalRotation).Value.y,
                    _packet.Data.GetVector4((int)DataCode.LocalRotation).Value.z, _packet.Data.GetVector4((int)DataCode.LocalRotation).Value.w);

            //Set ID
            newASLObject.AddComponent<ASLObject>()._LocallySetID(_packet.Data.GetString((int)DataCode.Id));


            //If we have the means to set up the recovery callback function - then do it
            if (_packet.Data.GetString((int)DataCode.ClaimRecoveryClassName) != string.Empty && _packet.Data.GetString((int)DataCode.ClaimRecoveryClassName) != null &&
                _packet.Data.GetString((int)DataCode.ClaimRecoveryFunctionName) != string.Empty && _packet.Data.GetString((int)DataCode.ClaimRecoveryFunctionName) != null)
            {
                Type callerClass = Type.GetType(_packet.Data.GetString((int)DataCode.ClaimRecoveryClassName));
                newASLObject.GetComponent<ASLObject>()._LocallySetClaimCancelledRecoveryCallback(
                    (ASLObject.ClaimCancelledRecoveryCallback)Delegate.CreateDelegate(typeof(ASLObject.ClaimCancelledRecoveryCallback),
                    callerClass, _packet.Data.GetString((int)DataCode.ClaimRecoveryFunctionName)));
            }

            //If we have the means to set up the SendFloat callback function - then do it
            if (_packet.Data.GetString((int)DataCode.SendFloatClassName) != string.Empty && _packet.Data.GetString((int)DataCode.SendFloatClassName) != null &&
                _packet.Data.GetString((int)DataCode.SendFloatFunctionName) != string.Empty && _packet.Data.GetString((int)DataCode.SendFloatFunctionName) != null)
            {
                Debug.Log("Class Info: " + _packet.Data.GetString((int)DataCode.SendFloatClassName));
                Debug.Log("Function Info: " + _packet.Data.GetString((int)DataCode.SendFloatFunctionName));
                Type callerClass = Type.GetType(_packet.Data.GetString((int)DataCode.SendFloatClassName));
                Debug.Log("callerClass: " + callerClass);
                newASLObject.GetComponent<ASLObject>()._LocallySetFloatCallback(
                    (ASLObject.FloatCallback)Delegate.CreateDelegate(typeof(ASLObject.FloatCallback),
                    callerClass, _packet.Data.GetString((int)DataCode.SendFloatFunctionName)));
            }

            //Add object to our dictionary
            ASLHelper.m_ASLObjects.Add(_packet.Data.GetString((int)DataCode.Id), newASLObject.GetComponent<ASLObject>());

            //If this client is the creator of this object, then call the ASLGameObjectCreatedCallback if it exists for this object
            if (GameSparksManager.m_PlayerIds.TryGetValue(_packet.Data.GetString((int)DataCode.Player), out bool isCreator))
            {
                if (isCreator && _packet.Data.GetString((int)DataCode.InstantiatedGameObjectClassName) != string.Empty
                    && _packet.Data.GetString((int)DataCode.InstantiatedGameObjectClassName) != null &&
                _packet.Data.GetString((int)DataCode.InstantiatedGameObjectFunctionName) != string.Empty && _packet.Data.GetString((int)DataCode.InstantiatedGameObjectFunctionName) != null)
                {
                    //Find Callback function
                    Type callerClass = Type.GetType(_packet.Data.GetString((int)DataCode.InstantiatedGameObjectClassName));
                    newASLObject.GetComponent<ASLObject>()._LocallySetGameObjectCreatedCallback(
                        (ASLObject.ASLGameObjectCreatedCallback)Delegate.CreateDelegate(typeof(ASLObject.ASLGameObjectCreatedCallback), callerClass,
                        _packet.Data.GetString((int)DataCode.InstantiatedGameObjectFunctionName)));
                    //Call function
                    newASLObject.GetComponent<ASLObject>().m_ASLGameObjectCreatedCallback.Invoke(newASLObject);
                }
            }
        }

        /// <summary>
        /// Reject a player's claim request on an ASL Object. This function is triggered by a packet received from the relay server.
        /// </summary>
        /// <param name="_packet">The packet containing the id of the object that a player wanted to claim</param>
        public void RejectClaim(RTPacket _packet)
        {
            if (ASLHelper.m_ASLObjects.TryGetValue(_packet.Data.GetString((int)DataCode.Id) ?? string.Empty, out ASLObject myObject))
            {
                Debug.LogWarning("Claim Rejected");
                //Remove all callbacks created as our claim was rejected
                if (myObject.m_ClaimCallback?.GetInvocationList().Length == null)
                {
                    myObject.m_ClaimCancelledRecoveryCallback?.Invoke(myObject.m_Id, 0);
                }
                else
                {
                    myObject.m_ClaimCancelledRecoveryCallback?.Invoke(myObject.m_Id, myObject.m_ClaimCallback.GetInvocationList().Length);
                }
                myObject._LocallyRemoveClaimCallbacks();
            }
        }

        /// <summary>
        /// Loads a new scene for all players. This function is triggered by a packet received from the relay server.
        /// </summary>
        /// <param name="_packet">The packet containing the name of the new scene to be loaded</param>
        public void LoadScene(RTPacket _packet)
        {
            SceneManager.LoadScene(_packet.Data.GetString((int)DataCode.SceneName));
        }

        /// <summary>
        /// Pass in the float value(s) from the relay server to a function of the user's choice (delegate function). The function that uses these float(s) is determined 
        /// by the user by setting the ASL Object of choosing's m_FloatCallback function to their own personal function. This function is triggered by a packet received from the relay server.
        /// Remember, though the user can pass in a float array, the max size of this array is 4 because we send it via a Vector4 due to GameSpark constraints
        /// </summary>
        /// <param name="_packet">The packet containing the id of the ASL Object and the float value to be passed into the user defined m_FloatCallback function</param>
        public void SetFloat(RTPacket _packet)
        {
            if (ASLHelper.m_ASLObjects.TryGetValue(_packet.Data.GetString((int)DataCode.Id) ?? string.Empty, out ASLObject myObject))
            {
                //Convert passed in Vector4 to a float array for our function
                Vector4 myFloatsAsVector4 = (Vector4)_packet.Data.GetVector4((int)DataCode.MyFloats);
                float[] myFloats = new float[4];
                myFloats[0] = myFloatsAsVector4.x;
                myFloats[1] = myFloatsAsVector4.y;
                myFloats[2] = myFloatsAsVector4.z;
                myFloats[3] = myFloatsAsVector4.w;
                myObject.m_FloatCallback?.Invoke(myFloats);
            }
        }

    }
}