using Networking.Client;
using Networking.Host;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class TitleMenu : MonoBehaviour
    {
        [SerializeField] private TMP_InputField joinCodeInputField;
        [SerializeField] private Button joinButton;

        private void Start()
        {
            joinButton.interactable = false;
        }

        public async void StartHost()
        {
            await HostSingleton.Instance.GameManager.StartHostAsync();
        }

        public async void JoinGame()
        {
            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeInputField.text);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void OnJoinCodeUpdated(string joinCode)
        {
            joinButton.interactable = !string.IsNullOrEmpty(joinCode);
        }
    }
}
