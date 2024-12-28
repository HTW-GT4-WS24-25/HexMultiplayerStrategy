using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Unit.Group.Display
{
    public class HealthBar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Image healthBarSeparationLinePrefab;
        [SerializeField] private Transform healthBarSeparationLineHolder;
        [SerializeField] private Image sliderBackground;
        [SerializeField] private Image sliderFill;
    
        [Header("Settings")]
        [SerializeField] private float minXHealthBarPosition;
        [SerializeField] private float maxXHealthBarPosition;

        private readonly List<Image> healthBarSeparationLines = new();
        private int _maxUnitCount;
        private Vector3 _defaultScale;
        private const int FULL_HEALTH = 1;

        [Button]
        public void Show(int maxUnitCount)
        {
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);
            transform.DOScale(_defaultScale, 0.3f).SetEase(Ease.InSine);
            SetMaxHealth(maxUnitCount);
        }

        [Button]
        public void Hide()
        {
            transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InSine).OnComplete(() => gameObject.SetActive(false));
        }

        [Button]
        public void Initialize(Color color)
        {
            _defaultScale = transform.localScale;
        
            sliderFill.color = color;
            sliderBackground.color = color;
        }

        public void SetMaxHealth(int maxHealth)
        {
            _maxUnitCount = maxHealth;
            healthSlider.value = FULL_HEALTH;
            
            PositionHealthBarSeparationLines();
        }

        public void IncreaseMaxUnitCount(int unitsAdded){
            _maxUnitCount += unitsAdded;
            
            PositionHealthBarSeparationLines();
        }
        
        [Button]
        public void SetHealth(float currentHealth)
        {
            var healthPercentage = currentHealth / _maxUnitCount;
            healthSlider.value = healthPercentage;
            
            PositionHealthBarSeparationLines();
        }

        private void PositionHealthBarSeparationLines()
        {
            var separationLinesNeeded = _maxUnitCount - 1;
            if (separationLinesNeeded > healthBarSeparationLines.Count)
            {
                var separationLinesToInstantiate = separationLinesNeeded - healthBarSeparationLines.Count;
                for (var i = 0; i < separationLinesToInstantiate; i++)
                {
                    healthBarSeparationLines.Add(Instantiate(healthBarSeparationLinePrefab, healthBarSeparationLineHolder));
                }
            }
        
            var fullHealthBarRange = maxXHealthBarPosition - minXHealthBarPosition;
            var separationLength = fullHealthBarRange / _maxUnitCount;

            var separationLinesIndex = 0;
            foreach (var healthBarSeparationLine in healthBarSeparationLines)
            {
                if (separationLinesIndex >= separationLinesNeeded)
                {
                    healthBarSeparationLine.gameObject.SetActive(false);
                    continue;
                }
            
                healthBarSeparationLine.transform.localPosition = new Vector3(minXHealthBarPosition + (separationLinesIndex + 1) * separationLength, 0, 0);
                healthBarSeparationLine.gameObject.SetActive(true);
                separationLinesIndex++;
            }
        }
    }
}
