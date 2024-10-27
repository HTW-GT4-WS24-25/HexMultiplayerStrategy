using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CancelButton : MonoBehaviour
    {
        [SerializeField] private Button cancelButton;

        private void OnEnable()
        {
            cancelButton.onClick.AddListener(() => GameEvents.UNIT.OnUnitGroupDeselected.Invoke());
        }

        private void OnDisable()
        {
            cancelButton.onClick.RemoveAllListeners();
        }
    }
}
