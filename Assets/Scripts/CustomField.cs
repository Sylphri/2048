using UnityEngine;

[RequireComponent(typeof(Field))]
public class CustomField : MonoBehaviour
{
    [SerializeField] private GameObject _backgroundPrefab;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private Transform _slotsParent;
    
    void Awake()
    {
        int width = PlayerPrefs.GetInt("Width", 4);
        int height = PlayerPrefs.GetInt("Height", 4);
        bool bonuses = PlayerPrefs.GetInt("Bonuses", 0) == 1 ? true : false;

        Field field = GetComponent<Field>();

        GameObject background = Instantiate(_backgroundPrefab, transform.position, Quaternion.identity);
        background.transform.localScale = new Vector3(width + 0.1f, height + 0.1f, 1);
       
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 pos = new Vector3(i - width / 2f + 0.5f, j - height / 2f + 0.5f, -1f) + transform.position;
                Instantiate(_slotPrefab, pos, Quaternion.identity, _slotsParent);
            }
        }
        
        field.Width = width;
        field.Height = height;
        field.Bonuses = bonuses;
    }
}
