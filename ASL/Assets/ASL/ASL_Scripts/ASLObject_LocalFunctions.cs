﻿using GameSparks.RT;
using UnityEngine;

namespace ASL
{
    /// <summary>
    /// ASLObject_LocalFunctions: ASLObject Partial Class containing all of the functions and variables relating to local actions - actions that affect a single player instead of all players
    /// </summary>
    public partial class ASLObject : MonoBehaviour
    {
        /// <summary>How long a claim has been active on this object. This gets reset, unless otherwise specified, every time a player claims an object</summary>
        private float m_ClaimReleaseTimer = 0;
        /// <summary>How long a claim can be held for this object. This gets set every time a player claims an object</summary>
        private float m_ClaimTime = 0;

        /// <summary>Function that is executed upon object initialization</summary>
        private void Awake()
        {
            m_Id = string.Empty;
            m_Mine = false;
            m_OutStandingClaims = false;
            m_OutstandingClaimCallbackCount = 0;
        }

        /// <summary>
        /// Currently counts down this object's claim time and releases the object back to the relay server after the specified amount of time has passed.
        /// Update is called once per frame
        /// </summary>
        private void Update()
        {
            // If we own it and we have no callbacks to perform
            //Then we don't need to own it anymore and thus can start to cancel it
            if (m_Mine && m_OutstandingClaimCallbackCount == 0 && m_ClaimTime > 0 && m_ClaimTime != 0) //if 0, then hold onto until stolen
            {
                m_ClaimReleaseTimer += Time.deltaTime * 1000; //Translate to milliseconds
                if (m_ClaimReleaseTimer > m_ClaimTime) //If time to release our claim
                {
                    m_ReleaseFunction?.Invoke(gameObject); //If user wants to do something before object is released - let them do so
                    _LocallyRemoveReleaseCallback();
                    using (RTData data = RTData.Get())
                    {
                        data.SetString((int)GameController.DataCode.Id, m_Id);
                        GameSparksManager.Instance().GetRTSession().SendData((int)GameSparksManager.OpCode.ReleaseClaimToServer, GameSparksRT.DeliveryIntent.RELIABLE, data);
                    }
                    m_Mine = false; //Release                    
                }
            }
        }

        /// <summary>
        /// Removes any outstanding claim callbacks for this object as well as sets the outstanding claim flag to false
        /// </summary>
        public void _LocallyRemoveClaimCallbacks()
        {
            m_OutStandingClaims = false;
            m_ClaimCallback = null;
        }

        /// <summary>
        /// This function should NOT be called by the user as it will only update the local player, it will not update all players. Claim the object as our own or give up the object. 
        /// This function is used when an incoming packet from the relay server needs to update the claim of this objects. 
        /// </summary>
        /// <param name="_claim">Based on what was sent, claim this is our own or relinquish it.</param>
        public void _LocallySetClaim(bool _claim)
        {
            m_Mine = _claim;
        }

        /// <summary>
        /// This function should NOT be called by the user as it will only update the local player, it will not update all players. Set the unique ID of this object. 
        /// This function is used when an incoming packet from the relay server needs to update the id of this object, this happens when the object is created. 
        /// </summary>
        /// <param name="_id">The new id</param>
        public void _LocallySetID(string _id)
        {
            m_Id = _id;
        }

        /// <summary>
        /// This function should NOT be called by the user as it will only update the local player, it will not update all players. Sets the anchor point for this ASL Object. 
        /// This function is used when an incoming packet from the relay server needs to update the anchor point of this objects. 
        /// </summary>
        /// <param name="_anchorPoint">The new anchor point for this object</param>
        public void _LocallySetAnchorPoint(string _anchorPoint)
        {
            m_AnchorPoint = _anchorPoint;
        }

        /// <summary>
        /// This function will only update the local player, not all players. Sets the claim cancelled recovery callback function for this object. 
        /// </summary>
        /// <param name="_rejectedClaimRecoveryFunction">The cancelled claim user provided function</param>
        public void _LocallySetClaimCancelledRecoveryCallback(ClaimCancelledRecoveryCallback _rejectedClaimRecoveryFunction)
        {
            m_ClaimCancelledRecoveryCallback = _rejectedClaimRecoveryFunction;
        }

        /// <summary>
        /// This function will only update the local player, not all players. Sets callback function that will be called upon object creation for this object. 
        /// </summary>
        /// <param name="_yourUponCreationFunction">The function to be executed upon this object's creation, provided by the user</param>
        public void _LocallySetGameObjectCreatedCallback(ASLGameObjectCreatedCallback _yourUponCreationFunction)
        {
            m_ASLGameObjectCreatedCallback = _yourUponCreationFunction;
        }

        /// <summary>
        /// This function will only update the local player, not all players. Sets the float callback function for this object. 
        /// </summary>
        /// <param name="_yourFloatFunction">The function that will be used to perform an action whenever a user sends a float</param>
        /// <example><code>
        /// void Start()
        /// {
        ///     gameobject.GetComponent&lt;ASL.ASLObject&gt;()._LocallySetFloatCallback(UserDefinedFunction)
        /// }
        /// public void UserDefinedFunction(float[] f)
        /// {
        ///     //Update some value for all users based on f. 
        ///     //Example:
        ///     playerHealth = f[0]; //Where playerHealth is shown to kept track/shown to all users
        /// }
        ///</code></example>
        public void _LocallySetFloatCallback(FloatCallback _yourFloatFunction)
        {
            m_FloatCallback = _yourFloatFunction;
        }

        /// <summary>
        /// This function will only update the local player, not all players. Set the release function to be executed upon this object's release (when claim switches to false) 
        /// </summary>
        /// <param name="_releaseFunction">The user provided release function</param>
        public void _LocallySetReleaseFunction(ReleaseFunction _releaseFunction)
        {
            m_ReleaseFunction = _releaseFunction;
        }

        /// <summary>
        /// This function will only update the local player, not all players. Removes any release functions that may be attached to this object
        /// </summary>
        public void _LocallyRemoveReleaseCallback()
        {
            m_ReleaseFunction = null;
        }
    }
}
