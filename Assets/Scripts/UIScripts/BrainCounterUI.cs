using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BrainCounterUI : MonoBehaviour
{
    [SerializeField] private BrainStorage _storage;
    [SerializeField] private TMP_Text _countText;
    [SerializeField] private Image _icon;

    private void OnEnable()
    {
        if (_storage != null)
        {
            _storage.OnDeliveredChanged += HandleDelivered;
            HandleDelivered(_storage.DeliveredCount);
        }
    }

    private void OnDisable()
    {
        if (_storage != null)
            _storage.OnDeliveredChanged -= HandleDelivered;
    }

    private void HandleDelivered(int count)
    {
        if (_countText != null)
            _countText.text = count.ToString();
    }
}
