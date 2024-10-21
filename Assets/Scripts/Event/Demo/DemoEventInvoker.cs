using UnityEngine;

namespace Event.Demo
{
    public class DemoEventInvoker : MonoBehaviour
    {
        [SerializeField] private FloatEvent demoEvent;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                demoEvent.Invoke(Time.timeSinceLevelLoad);
            }
        }
    }
}
