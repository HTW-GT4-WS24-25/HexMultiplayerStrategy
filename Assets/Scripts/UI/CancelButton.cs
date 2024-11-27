using GameEvents;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CancelButton : MonoBehaviour
    {
        [SerializeField] private Button cancelButton;

        private void OnEnable()
        {
            cancelButton.onClick.AddListener(() => ClientEvents.Unit.OnUnitGroupDeselected.Invoke());
        }

        private void OnDisable()
        {
            cancelButton.onClick.RemoveAllListeners();
        }
    }
}
