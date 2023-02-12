using UnityEngine;

public class FieldRenderer : MonoBehaviour
{
    private Field _field;
    private GameObject[,] _cells;

    [SerializeField] private SwipeDetector _swipeDetector;
    [SerializeField] private GameObject _cellPrefab;

    [SerializeField] private int _width;
    [SerializeField] private int _height;

    private void Start()
    {
        _field = new Field(_width, _height);
        _cells = new GameObject[_width, _height];
        _swipeDetector.Swiped += OnSwiped;
        GenerateCells();
        UpdateCells();
    }

    private void OnSwiped(SwipeDirection direction)
    {
        _field.Slide(direction);
        UpdateCells();
    }

    private void GenerateCells()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                _cells[i, j] = Instantiate(_cellPrefab, transform.position + 
                    new Vector3(i - _width / 2f + 0.5f, j - _height / 2f + 0.5f, 0),
                    Quaternion.identity);
                _cells[i, j].SetActive(false);
            }
        }
    }

    private void UpdateCells()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                int value = _field[i, j];
                if (value > 0)
                {
                    _cells[i, j].SetActive(true);
                    _cells[i, j].GetComponent<Cell>().SetValue(value.ToString());
                }
                else
                {
                    _cells[i, j].SetActive(false);
                }
            }
        }
    }
}
