using UnityEngine;

namespace Mps
{
    public partial class Mp2_TheWorld : MonoBehaviour
    {
        private Vector3 kDeltaVector = new Vector3(0.5f, .5f, .5f);

        public void CreatePrimitive(PrimitiveType type)
        {
            if (mSelected != null) //If we have a parent
            {
                ASL.ASLHelper.InstanitateASLObject(type, kDeltaVector, Quaternion.identity, mSelected.GetComponent<ASL.ASLObject>().m_Id, "");
                int siblingCount = mSelected.transform.childCount;
                if (siblingCount > 0)
                {
                    Transform sibling = mSelected.transform.GetChild(0);
                }
            }
            else //No parent
            {
                float x = Random.Range(-4f, 4f);
                float y = Random.Range(0f, 2.5f);
                float z = Random.Range(-4f, 4f);

                //Set parent to be our GameController
                ASL.ASLHelper.InstanitateASLObject(type, new Vector3(x, y, z), Quaternion.identity, "GameController", GetType().Namespace + "." + GetType().Name, "CreatedGameObject");

            }
        }

        public static void CreatedGameObject(GameObject _myGameObject)
        {
            Debug.Log("Caller-Object ID: " + _myGameObject.GetComponent<ASL.ASLObject>().m_Id);
        }
    }
}