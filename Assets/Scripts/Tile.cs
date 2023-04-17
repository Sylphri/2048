using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private TMP_Text _tileText;
    [SerializeField] private SpriteRenderer _background;

    public void SetValue(string value)
    {
        if (value.Length < 4)
        {
            _tileText.fontSize = 50;
            _tileText.text = value;
        }
        else if (value.Length == 4)
        {
            _tileText.fontSize = 40;
            _tileText.text = value;
        }
        else if (value.Length == 5)
        {
            _tileText.fontSize = 50;
            _tileText.text = value.Remove(2) + "K";
        }
        else if (value.Length == 6)
        {
            _tileText.fontSize = 40;
            _tileText.text = value.Remove(3) + "K";
        }
        else if (value.Length < 9)
        {
            _tileText.fontSize = 50;
            _tileText.text = value.Remove(value.Length - 6) + "M";
        }
        else if (value.Length == 9)
        {
            _tileText.fontSize = 40;
            _tileText.text = value.Remove(3) + "M";
        }
        else 
        {
            _tileText.fontSize = 50;
            _tileText.text = value.Remove(1) + "B";
        }
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
