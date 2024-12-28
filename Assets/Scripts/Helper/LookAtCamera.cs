using UnityEngine;

namespace Helper
{
    public class LookAtCamera : MonoBehaviour
    {
        private void LateUpdate()
        {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }
}
