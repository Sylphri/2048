using UnityEngine;
using TMPro;

public class Stats : MonoBehaviour
{
    [SerializeField] private TMP_Text _timer;
    [SerializeField] private TMP_Text _movesCountLabel;
    [SerializeField] private TMP_Text _scoreLabel;
    [SerializeField] private TMP_Text _bestLabel;

    private int _moves;  
    private int _score;
    private int _best;

    private void Start()
    {
        _best = PlayerPrefs.GetInt("Best", 0); 
        _bestLabel.text = _best.ToString();
    }

    private void Update()
    {
        int seconds = (int)(Time.timeSinceLevelLoad) % 60;
        int minutes = (int)(Time.timeSinceLevelLoad) / 60;
        _timer.text = $"{minutes}:{seconds:D2}";
    }

    public void AddMove()
    {
        _moves++;
        _movesCountLabel.text = $"{_moves} moves";
    }

    public void IncreaseScore(int value)
    {
        _score += value;
        _scoreLabel.text = _score.ToString(); 
        if (_score > _best)
            PlayerPrefs.SetInt("Best", _score);
    }
}
