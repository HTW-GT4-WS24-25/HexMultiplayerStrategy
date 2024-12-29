using Core.GameEvents;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Input
{
    public class PauseHandler : NetworkBehaviour
    {
        [SerializeField] private UnityEvent onServerPause;
        [SerializeField] private UnityEvent onServerResume;
        [SerializeField] private UnityEvent onClientPause;
        [SerializeField] private UnityEvent onClientResume;
        
        private bool _isPaused;
        
        public override void OnNetworkSpawn()
        {
            if (IsServer)
                ClientEvents.Input.PauseTogglePressed += HandlePauseTogglePressed;
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
                ClientEvents.Input.PauseTogglePressed -= HandlePauseTogglePressed;
        }

        #region Server

        private void HandlePauseTogglePressed()
        {
            _isPaused = !_isPaused;

            if (_isPaused)
            {
                Time.timeScale = 0f;
                onServerPause?.Invoke();
                PauseGameClientRpc();
            }
            else
            {
                Time.timeScale = 1f;
                onServerResume?.Invoke();
                ResumeGameClientRpc();
            }
        }

        #endregion

        #region Client

        [ClientRpc]
        private void PauseGameClientRpc()
        {
            if (IsServer)
                return;
            
            Time.timeScale = 0;
            onClientPause?.Invoke();
        }

        [ClientRpc]
        private void ResumeGameClientRpc()
        {
            if (IsServer)
                return;
            
            Time.timeScale = 1;
            onClientResume?.Invoke();
        }

        #endregion
    }
}
