using UnityEngine;
using TMPro;

public class LoseMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text _loseStats;

    public void SetStats(string score, int moves, int time)
    {
        int seconds = time % 60;
        int minutes = time / 60;
        _loseStats.text = $"You earned {score} points with {moves} moves in {minutes}:{seconds:D2}.";
    }
}
