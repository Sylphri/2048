using TMPro;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private TMP_Text _cellText;

    public void SetValue(string value)
    {
        _cellText.text = value;
    }
}
