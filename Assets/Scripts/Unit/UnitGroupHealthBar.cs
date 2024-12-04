using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

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

    [Button]
    public void SetColor(Color color)
    {
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
