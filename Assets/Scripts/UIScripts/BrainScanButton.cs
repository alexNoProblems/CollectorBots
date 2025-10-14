using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BrainScanButton : MonoBehaviour
{
    [SerializeField] private BrainScanner _scanner;

    private void Awake()
    {
        Button button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        _scanner.Scan();
    }
}
