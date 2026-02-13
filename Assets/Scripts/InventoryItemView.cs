using UnityEngine;
using TMPro;

public class InventoryItemView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;

    public void Setup(string itemName)
    {
        _nameText.text = itemName;
    }
}
