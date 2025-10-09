using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BrainCounterUI : MonoBehaviour
{
    [SerializeField] private BrainScanner _scanner;
    [SerializeField] private TMP_Text _countText;
    [SerializeField] private Image _icon;

    private void OnEnable()
    {
        if (_scanner != null)
        {
            _scanner.OnBrainDelivered += HandleDelivered;
            HandleDelivered(_scanner.DeliveredCount);
        }
    }

    private void OnDisable()
    {
        if (_scanner != null)
            _scanner.OnBrainDelivered -= HandleDelivered;
    }

    private void HandleDelivered(int count)
    {
        if (_countText != null)
            _countText.text = count.ToString();
    }
}
