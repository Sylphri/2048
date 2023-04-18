using UnityEngine;
using TMPro;

public class Stats : MonoBehaviour
{
    [SerializeField] private TMP_Text _timer;
    [SerializeField] private TMP_Text _movesCountLabel;
    [SerializeField] private TMP_Text _scoreLabel;
    [SerializeField] private TMP_Text _bestLabel;
    [SerializeField] private string _modeTitle;
    [SerializeField] private GameObject _loseMenu;
    [SerializeField] private GameObject _winMenu;

    private int _moves;  
    private int _score;
    private int _best;

    public bool gameOver { private set; get; }

    private void Start()
    {
        _best = PlayerPrefs.GetInt("Best" + _modeTitle, 0); 
        
        string bestStr = _best.ToString();
        if (bestStr.Length < 5)
            _bestLabel.text = bestStr;
        else if (bestStr.Length < 7)
            _bestLabel.text = bestStr.Remove(bestStr.Length - 3) + "K";
        else if (bestStr.Length < 10)
            _bestLabel.text = bestStr.Remove(bestStr.Length - 6) + "M";
        else 
            _bestLabel.text = bestStr.Remove(1) + "B";
    }

    private void Update()
    {
        if (gameOver) return;
        
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
        
        string scoreStr = _score.ToString();
        if (scoreStr.Length < 5)
            _scoreLabel.text = scoreStr;
        else if (scoreStr.Length < 7)
            _scoreLabel.text = scoreStr.Remove(scoreStr.Length - 3) + "K";
        else if (scoreStr.Length < 10)
            _scoreLabel.text = scoreStr.Remove(scoreStr.Length - 6) + "M";
        else 
            _scoreLabel.text = scoreStr.Remove(1) + "B";
        
        if (_score > _best)
            PlayerPrefs.SetInt("Best" + _modeTitle, _score);
    }

    public void GameOver()
    {
        gameOver = true;
        int time = (int)Time.timeSinceLevelLoad;
        _loseMenu.SetActive(true);
        LoseMenu menu = _loseMenu.GetComponent<LoseMenu>();
        menu.SetStats(_scoreLabel.text, _moves, time);
    }

    public void Win()
    {
        int time = (int)Time.timeSinceLevelLoad;
        _winMenu.SetActive(true);
        WinMenu menu = _winMenu.GetComponent<WinMenu>();
        menu.SetStats(_moves, time);
    }
}
