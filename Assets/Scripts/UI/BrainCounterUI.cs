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
            _storage.BrainsCountChanged += OnChanged;
            OnChanged(_storage.Count);
        }
    }

    private void OnDisable()
    {
        if (_storage != null)
            _storage.BrainsCountChanged -= OnChanged;
    }

    private void OnChanged(int count)
    {
        _countText.text = count.ToString();
    }
}
