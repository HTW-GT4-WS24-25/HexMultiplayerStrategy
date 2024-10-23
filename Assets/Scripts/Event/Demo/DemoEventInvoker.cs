using UnityEngine;
using UnityEngine.Serialization;

namespace GameEvent.Demo
{
    public class DemoEventInvoker : MonoBehaviour
    {
        [FormerlySerializedAs("demoEvent")] [SerializeField] private FloatGameEvent demoGameEvent;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                demoGameEvent.Invoke(Time.timeSinceLevelLoad);
            }
        }
    }
}
