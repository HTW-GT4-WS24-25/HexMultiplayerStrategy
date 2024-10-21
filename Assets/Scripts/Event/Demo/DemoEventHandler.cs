using DG.Tweening;
using UnityEngine;

namespace Event.Demo
{
    public class DemoEventHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform demoCube;

        [Header("Settings")] 
        [SerializeField] private float demoSquareWiggleStrength;
        [SerializeField] private float demoSquareWiggleTime;

        private Tween _wiggleTween;
    
        public void DebugLogDemoTime(float demoTime)
        {
            Debug.Log($"Event Triggered at time: {demoTime}");
        }

        public void MakeSquareWiggle()
        {
            _wiggleTween?.Kill();
            demoCube.localScale = Vector3.one;
        
            _wiggleTween = demoCube.DOPunchScale(Vector3.one * demoSquareWiggleStrength, demoSquareWiggleTime);    
        }
    }
}
