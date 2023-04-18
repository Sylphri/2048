using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PopUpSpawner : MonoBehaviour
{
    [SerializeField] private float _height;
    [SerializeField] private float _lifeTime;
    [SerializeField] private GameObject _popUpPrefab;
    [SerializeField] private float _timeBetweenSpawn;

    private float _timeFromSpawn = 0;
    private List<string> _queue = new List<string>();

    private void Update()
    {
        if (_timeFromSpawn < _timeBetweenSpawn)
        {
            _timeFromSpawn += Time.deltaTime;
            return;
        }
        else if (_queue.Count > 0)
        {
            Spawn(_queue[0]);
            _queue.RemoveAt(0);
        }
    }

    public void Spawn(string value)
    {
        if (_timeFromSpawn < _timeBetweenSpawn)
        {
            _queue.Add(value);
            return;
        }
        
        GameObject popUpObj = Instantiate(_popUpPrefab, transform.position, Quaternion.identity, transform);       
        PopUp popUp = popUpObj.GetComponent<PopUp>();
        popUp.Height = _height;
        popUp.LifeTime = _lifeTime;
        TMP_Text popUpText = popUpObj.GetComponent<TMP_Text>();

        if (value.Length < 5)
            popUpText.text = "+" + value;
        else if (value.Length < 7)
            popUpText.text = "+" + value.Remove(value.Length - 3) + "K";
        else if (value.Length < 10)
            popUpText.text = "+" + value.Remove(value.Length - 6) + "M";
        else 
            popUpText.text = "+" + value.Remove(1) + "B";

        _timeFromSpawn = 0;
    }
}
