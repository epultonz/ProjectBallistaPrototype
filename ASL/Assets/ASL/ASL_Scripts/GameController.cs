using GameSparks.RT;
using UnityEngine;

namespace ASL
{
    /// <summary>
    /// GameController: Part of the GameController class that handles everything specific to GameController or used by just the GameController class
    /// </summary>
    public partial class GameController : MonoBehaviour
    {
        /// <summary> The GameController singleton. This is an unique variable - only 1 exists at a time. </summary>
        private static GameController m_Instance = null;

        /// <summary>
        /// The DataCode that allows us to strip data from packets. If you update this list, make sure you update the same list on the GameSparksPortal page.
        /// </summary>
        public enum DataCode
        {
            /// <summary>DataCode does not recognize 0 as value, this is here to ensure our actual Data Codes start at 1</summary>
            BadValue,
            /// <summary>The DataCode that represents our object's id. This is used in almost every packet.</summary>
            Id,
            /// <summary>The DataCode that represents our local position</summary>
            LocalPosition,
            /// <summary>The DataCode that represents our local rotation</summary>
            LocalRotation,
            /// <summary>The DataCode that represents our local scale</summary>
            LocalScale,
            /// <summary>The DataCode that represents the primitive type of an object</summary>
            PrimitiveType,
            /// <summary>The DataCode that represents the id of a parent object</summary>
            ParentId,
            /// <summary>The DataCode that represents the color we will set the object we own to</summary>
            MyColor,
            /// <summary>The DataCode that represents the color we will set the object we own to for everyone else</summary>
            OpponentColor,
            /// <summary>The DataCode that represents the player id</summary>
            Player,
            /// <summary>The DataCode that represents the old player's id</summary>
            OldPlayer,
            /// <summary>The DataCode that represents a player's peer id</summary>
            PlayerPeerId,
            /// <summary>The DataCode that represents a float value</summary>
            MyFloats,
            /// <summary>The DataCode that represents the name of a prefab</summary>
            PrefabName,
            /// <summary>The DataCode that represents the name of the user defined claim recovery function name</summary>
            ClaimRecoveryFunctionName,
            /// <summary>The DataCode that represents the name of the user defined claim recovery class name</summary>
            ClaimRecoveryClassName,
            /// <summary>The DataCode that represents the name of the user defined post instantiated game object class name</summary>
            InstantiatedGameObjectClassName,
            /// <summary>The DataCode that represents the name of the user defined post instantiated game object function name</summary>
            InstantiatedGameObjectFunctionName,
            /// <summary>The DataCode that represents the anchor point of an object</summary>
            AnchorPoint,
            /// <summary>The DataCode that represents the name of a scene</summary>
            SceneName,
            /// <summary>The DataCode that represents the name of the user defined <see cref="ASLObject.FloatCallback"/> class name</summary>
            SendFloatClassName,
            /// <summary>The DataCode that represents the name of the user defined <see cref="ASLObject.FloatCallback"/> function name</summary>
            SendFloatFunctionName
        }

        /// <summary>
        /// A Getter for the GameController singleton - a variable representing this class
        /// </summary>
        /// <returns>The GameController singleton - a constructor for this class that is unique</returns>
        public static GameController Instance()
        {
            if (m_Instance != null)
            {
                return m_Instance;
            }
            else
            {
                return m_Instance ?? null;
            }
        }

        /// <summary>
        /// Set assign the singleton for this class
        /// </summary>
        private void Awake()
        {
            m_Instance = this;
        }

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        private void Start()
        {
            SyncronizeID();
        }

        /// <summary>
        /// Find all ASL Objects in the scene and have the relay server create a unique ID for them as well as add this object to our ASLObject dictionary
        /// This function is triggered when this script is first loaded
        /// </summary>
        private void SyncronizeID()
        {
            ASLObject[] m_StartingASLObjects = FindObjectsOfType(typeof(ASLObject)) as ASLObject[];
            int startingObjectCount = 0;
            foreach (ASLObject _ASLObject in m_StartingASLObjects)
            {
                if (_ASLObject.m_Id == string.Empty || _ASLObject.m_Id == null) //If object does not have an ID
                {
                    using (RTData data = RTData.Get())
                    {
                        //Have the RT server create an ID and send it back to everyone
                        GameSparksManager.Instance().GetRTSession().SendData((int)GameSparksManager.OpCode.ServerSetId, GameSparksRT.DeliveryIntent.RELIABLE, data);

                        //Add the starting objects to our dictionary without a key. Key will be added later to ensure all clients have the same key
                        ASLHelper.m_ASLObjects.Add(startingObjectCount.ToString(), _ASLObject);
                    }
                    startingObjectCount++;
                }
            }
        }

        /// <summary>
        /// Sets the parent of a newly created
        /// </summary>
        /// <param name="_myObject"></param>
        /// <param name="_parentID"></param>
        private void SetParent(GameObject _myObject, string _parentID)
        {
            bool matchFound = false;
            //Search for the parent in ASLObjects            
            if (ASLHelper.m_ASLObjects.TryGetValue(_parentID ?? string.Empty, out ASLObject myParent))
            {
                _myObject.transform.SetParent(myParent.transform);
                matchFound = true;
            }

            //Search for parent in regular GameObjects - this is slow compared to our first attempt. But it should only be ran if the user
            //attempts to set the parent object by passing in the parent's name and not their id.
            if (!matchFound)
            {
                GameObject[] allGameObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];
                foreach (GameObject _gameObject in allGameObjects)
                {
                    if (_gameObject.name == _parentID)
                    {
                        _myObject.transform.SetParent(_gameObject.transform);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Upon game start, any ASL Objects in the scene do not have synchronized IDs. This function changes their ID to be synced with other clients
        /// </summary>
        /// <param name="_id">The new id for _object</param>
        /// <param name="_object">The ASL object that will be getting a new id</param>
        /// <returns></returns>
        private bool InitializeStartObject(string _id, ASLObject _object)
        {
            if (ASLHelper.m_ASLObjects.ContainsKey(_id))
            {
                _object._LocallySetID(_id);
                _object._LocallyRemoveReleaseCallback();
                return true;
            }
            else
            {
                Debug.LogError("Attempted to set the id of an object to a value that does not exist in our dictionary. If this was intended, then" +
                    " add this object to the m_ASLObjects dictionary first.");
                return false;
            }
        }

    }
}
