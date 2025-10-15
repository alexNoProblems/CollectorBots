using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BrainScanButton : MonoBehaviour
{
    [SerializeField] private BrainScanner _scanner;

    private Button _button;

    private void Awake()
    {
       _button = GetComponent<Button>();

        if (_button != null)
            _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDestroy()
    {
        if (_button != null)
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }
    }

    private void OnButtonClicked()
    {
        _scanner.Scan();
    }
}
