using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private TMP_Text _tileText;
    [SerializeField] private SpriteRenderer _background;

    public void SetValue(string value)
    {
        _tileText.text = value;
    }

    public void SetBackgroundColor(Color color)
    {
        _background.color = color; 
    }

    public void SetTextColor(Color color)
    {
        _tileText.color = color;
    }
}
