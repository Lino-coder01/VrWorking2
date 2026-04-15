using UnityEngine;
using TMPro; // IMPORTANT si ton dropdown est en TextMeshPro
using UnityEngine.XR.Interaction.Toolkit;

public class TurnModeSelector : MonoBehaviour
{
    public TMP_Dropdown dropdown; // ton dropdown
    public Behaviour snapTurn;

    void Start()
    {
        dropdown.onValueChanged.AddListener(OnValueChanged);
        OnValueChanged(dropdown.value);
    }

    void OnValueChanged(int index)
    {
        // 0 = Only Head → désactive
        // 1 = Snap Turn → active
        snapTurn.enabled = (index == 1);
    }
}