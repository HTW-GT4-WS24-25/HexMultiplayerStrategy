using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class SetupUI : MonoBehaviour
    {
        [SerializeField] TMP_InputField nameInputField;
        [SerializeField] Button continueButton;
    
        private const string PlayerNameKey = "PlayerName";

        private void Start()
        {
            var playerName = PlayerPrefs.GetString(PlayerNameKey, "");
            nameInputField.text = playerName;
        
            UpdateButtonState();
        }

        public void UpdateButtonState()
        {
            continueButton.interactable = !string.IsNullOrEmpty(nameInputField.text);
        }

        public void OnContinueClicked()
        {
            var nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}
