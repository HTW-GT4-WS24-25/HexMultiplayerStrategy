using Core.GameEvents;
using Core.Input;
using Core.Unit.Group;
using Unity.Netcode;
using UnityEngine;

namespace Core.Unit
{
    public class MouseHoverUnitHighlighter : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private LayerMask unitLayer;

        private ulong _owningPlayerId;
        private UnitGroup _currentHoveredUnitGroup;
        private Collider _currentHoveredUnitCollider;
        private bool _isDay;

        private void Awake()
        {
            _owningPlayerId = NetworkManager.Singleton.LocalClientId;
        }

        private void OnEnable()
        {
            ClientEvents.DayNightCycle.OnSwitchedCycleState += HandleDayNightCycleChanged;
        }
        
        private void OnDisable()
        {
            ClientEvents.DayNightCycle.OnSwitchedCycleState += HandleDayNightCycleChanged;
        }

        private void HandleDayNightCycleChanged(DayNightCycle.DayNightCycle.CycleState cycleState)
        {
            _isDay = cycleState == DayNightCycle.DayNightCycle.CycleState.Day;
            if(!_isDay)
                DisableCurrentHover();
        }

        private void Update()
        {
            if (!_isDay)
                return;
            
            var ray = Camera.main.ScreenPointToRay(inputReader.MousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, unitLayer))
            {
                ProcessUnitHit(hit);
            }
            else
            {
                DisableCurrentHover();
            }
        }

        private void ProcessUnitHit(RaycastHit hit)
        {
            if(hit.collider == _currentHoveredUnitCollider)
                return;
            
            DisableCurrentHover(); 
            
            if(!hit.collider.transform.parent.TryGetComponent<UnitGroup>(out var unitGroup))
                return;
            
            Debug.Log("Unit hit!");
            
            if(unitGroup.PlayerId != _owningPlayerId)
                return;
            
            unitGroup.EnableHoverHighlight();
            _currentHoveredUnitGroup = unitGroup;
            _currentHoveredUnitCollider = hit.collider;
        }

        private void DisableCurrentHover()
        {
            _currentHoveredUnitGroup?.DisableHoverHighlight();
            _currentHoveredUnitGroup = null;
            _currentHoveredUnitCollider = null;
        }
    }
}
