using UnityEngine;
using TMPro;

public class PopUpSpawner : MonoBehaviour
{
    [SerializeField] private float _height;
    [SerializeField] private float _lifeTime;
    [SerializeField] private GameObject _popUpPrefab;

    public void Spawn(string value)
    {
        Debug.Log("spawned");
        GameObject popUpObj = Instantiate(_popUpPrefab, transform.position, Quaternion.identity, transform);       
        PopUp popUp = popUpObj.GetComponent<PopUp>();
        popUp.Height = _height;
        popUp.LifeTime = _lifeTime;
        TMP_Text popUpText = popUpObj.GetComponent<TMP_Text>();

        if (value.Length < 5)
            popUpText.text = value;
        else if (value.Length == 5)
            popUpText.text = value.Remove(value.Length - 3) + "K";
        else if (value.Length < 9)
            popUpText.text = value.Remove(value.Length - 6) + "M";
        else 
            popUpText.text = value.Remove(1) + "B";
    }
}
