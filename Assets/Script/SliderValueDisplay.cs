using UnityEngine;
using TMPro;

public class SliderValueDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueText;

    public void UpdateText(float value)
    {
        if (valueText != null)
        {
            valueText.text = value.ToString("F2");
        }
    }
}