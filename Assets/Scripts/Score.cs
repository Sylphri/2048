using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreLabel;
    private int _score;

    public void Increase(int value)
    {
        _score += value;
        _scoreLabel.text = $"Score: {_score}";
    }
}
