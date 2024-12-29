using Core.PlayerData;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Core.UI
{
    public class SetupUI : MonoBehaviour
    {
        [SerializeField] TMP_InputField nameInputField;
        [SerializeField] Button continueButton;

        private void Start()
        {
            nameInputField.text = PlayerNameStorage.Name;
        
            UpdateButtonState();
        }

        public void UpdateButtonState()
        {
            continueButton.interactable = !string.IsNullOrEmpty(nameInputField.text);
        }

        public void OnContinueClicked()
        {
            PlayerNameStorage.Name = nameInputField.text;
            
            var nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}
