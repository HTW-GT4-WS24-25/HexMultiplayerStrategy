using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Unit
{
    public class UnitGroupHealthBar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Slider healthSlider;
        [SerializeField] Image healthBarSeparationLinePrefab;
        [SerializeField] private Transform healthBarSeparationLineHolder;
        [SerializeField] private Image sliderBackground;
        [SerializeField] private Image sliderFill;
    
        [Header("Settings")]
        [SerializeField] private float minXHealthBarPosition;
        [SerializeField] private float maxXHealthBarPosition;

        private List<Image> healthBarSeparationLines = new();
        private int _unitCount;
        private Vector3 _defaultScale;

        [Button]
        public void Show()
        {
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);
            transform.DOScale(_defaultScale, 0.3f).SetEase(Ease.InSine);
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

        [Button]
        public void SetHealth(float currentHealth, int unitCount)
        {
            if (unitCount != _unitCount)
            {
                _unitCount = unitCount;
                PositionHealthBarSeparationLines();
            }
        
            var healthPercentage = currentHealth / _unitCount;
            healthSlider.value = healthPercentage;
        }

        private void PositionHealthBarSeparationLines()
        {
            var separationLinesNeeded = _unitCount - 1;
            if (separationLinesNeeded > healthBarSeparationLines.Count)
            {
                var separationLinesToInstantiate = separationLinesNeeded - healthBarSeparationLines.Count;
                for (var i = 0; i < separationLinesToInstantiate; i++)
                {
                    healthBarSeparationLines.Add(Instantiate(healthBarSeparationLinePrefab, healthBarSeparationLineHolder));
                }
            }
        
            var fullHealthBarRange = maxXHealthBarPosition - minXHealthBarPosition;
            var separationLength = fullHealthBarRange / _unitCount;

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
