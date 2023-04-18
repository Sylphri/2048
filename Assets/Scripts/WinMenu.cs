using UnityEngine;
using TMPro;

public class WinMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text _loseStats;

    public void SetStats(int moves, int time)
    {
        int seconds = time % 60;
        int minutes = time / 60;
        _loseStats.text = $"You win with {moves} moves in {minutes}:{seconds:D2}.";
    }
}
