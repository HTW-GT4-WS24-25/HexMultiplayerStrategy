using Core.GameEvents;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.InGame.UnitSelection
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
