using System.Collections;
using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField] private SwipeDetector _swipeDetector;
    [SerializeField] private GameObject _cellPrefab;

    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private float _secondsToSwipe;

    private int[,] _field;
    private Cell[,] _cells;

    private bool _canSwipe = true;
    private bool _fieldChanged = false;

    private void Start()
    {
        _field = new int[_width, _height];
        _cells = new Cell[_width, _height];
        _swipeDetector.Swiped += OnSwiped;
        _field[0, 0] = 2;
        RenderCells();
    }

    void RandomSpawn()
    {
        int x = Random.Range(0, _width);
        int y = Random.Range(0, _height);
        while (_field[x, y] != 0)
        {
            x = Random.Range(0, _width);
            y = Random.Range(0, _height);
        }
        _field[x, y] = 2;
    }

    Vector3 GetCellPosition(int x, int y)
    {
        return new Vector3(x - _width / 2f + 0.5f, y - _height / 2f + 0.5f, 0);
    }

    private void RenderCells()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if(_cells[i, j] != null)
                {
                    Destroy(_cells[i, j].gameObject);
                    _cells[i, j] = null;
                }

                if (_field[i, j] == 0) continue;
                
                GameObject cell = Instantiate(_cellPrefab, GetCellPosition(i, j), Quaternion.identity);
                _cells[i, j] = cell.GetComponent<Cell>();
                _cells[i, j].SetValue(_field[i, j].ToString());
            }
        }
    }

    IEnumerator Move(Transform cell, Vector3 position, float seconds)
    {
        float past = 0;
        while((cell.position - position).magnitude > 9e-2)
        {
            Vector3 offset = (position - cell.position) * Time.deltaTime / (seconds - past);
            cell.position += offset;
            past += Time.deltaTime;
            yield return null;
        }
        cell.position = position;
    }

    private void OnSwiped(SwipeDirection direction)
    {
        if (!_canSwipe) return;

        _canSwipe = false;

        switch (direction)
        {
            case SwipeDirection.Left:
                SlideLeft();
                break;
            case SwipeDirection.Right:
                SlideRight();
                break;
            case SwipeDirection.Up:
                SlideUp();
                break;
            case SwipeDirection.Down:
                SlideDown();
                break;
        }

        if(_fieldChanged) RandomSpawn();
        StartCoroutine(WaitBeforeRender(_secondsToSwipe));
    }

    IEnumerator WaitBeforeRender(float seconds)
    {
        yield return new WaitForSeconds(seconds + 0.01f);
        RenderCells();
        _canSwipe = true;
    }

    private void SlideLeft()
    {
        for (int j = 0; j < _height; j++)
        {
            for(int i = 1; i < _width; i++)
            {
                if (_field[i, j] == 0) continue;

                int pos = i - 1;
                while (_field[pos, j] == 0 && pos > 0) 
                    pos--;

                _fieldChanged = true;
                if(pos == 0 && _field[pos, j] == 0)
                {
                    _field[pos, j] = _field[i, j];
                    _field[i, j] = 0;
                    StartCoroutine(Move(_cells[i, j].transform, GetCellPosition(pos, j), _secondsToSwipe));
                    continue;
                }

                if (_field[pos, j] != _field[i , j] && pos != i - 1)
                {
                    _field[pos + 1, j] = _field[i, j];
                    _field[i, j] = 0;
                    StartCoroutine(Move(_cells[i, j].transform, GetCellPosition(pos + 1, j), _secondsToSwipe));
                    continue;
                }

                if(_field[pos, j] == _field[i, j])
                {
                    _field[pos, j] *= 2;
                    _field[i, j] = 0;
                    StartCoroutine(Move(_cells[i, j].transform, GetCellPosition(pos, j), _secondsToSwipe));
                    continue;
                }
                _fieldChanged = false;
            }
        }
    }

    private void SlideRight()
    {
        for (int j = 0; j < _height; j++)
        {
            for (int i = _width - 2; i >= 0; i--)
            {
                if (_field[i, j] == 0) continue;

                int pos = i + 1;
                while (_field[pos, j] == 0 && pos < _width - 1)
                    pos++;

                _fieldChanged = true;
                if (pos == _width - 1 && _field[pos, j] == 0)
                {
                    _field[pos, j] = _field[i, j];
                    _field[i, j] = 0;
                    StartCoroutine(Move(_cells[i, j].transform, GetCellPosition(pos, j), _secondsToSwipe));
                    continue;
                }

                if (_field[pos, j] != _field[i, j] && pos != i + 1)
                {
                    _field[pos - 1, j] = _field[i, j];
                    _field[i, j] = 0;
                    StartCoroutine(Move(_cells[i, j].transform, GetCellPosition(pos - 1, j), _secondsToSwipe));
                    continue;
                }

                if (_field[pos, j] == _field[i, j])
                {
                    _field[pos, j] *= 2;
                    _field[i, j] = 0;
                    StartCoroutine(Move(_cells[i, j].transform, GetCellPosition(pos, j), _secondsToSwipe));
                    continue;
                }
                _fieldChanged = false;
            }
        }
    }

    private void SlideUp()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = _height - 2; j >= 0; j--)
            {
                if (_field[i, j] == 0) continue;

                int pos = j + 1;
                while (_field[i, pos] == 0 && pos < _height - 1)
                    pos++;

                _fieldChanged = true;
                if (pos == _height - 1 && _field[i, pos] == 0)
                {
                    _field[i, pos] = _field[i, j];
                    _field[i, j] = 0;
                    StartCoroutine(Move(_cells[i, j].transform, GetCellPosition(i, pos), _secondsToSwipe));
                    continue;
                }

                if (_field[i, pos] != _field[i, j] && pos != j + 1)
                {
                    _field[i, pos - 1] = _field[i, j];
                    _field[i, j] = 0;
                    StartCoroutine(Move(_cells[i, j].transform, GetCellPosition(i, pos - 1), _secondsToSwipe));
                    continue;
                }

                if (_field[i, pos] == _field[i, j])
                {
                    _field[i, pos] *= 2;
                    _field[i, j] = 0;
                    StartCoroutine(Move(_cells[i, j].transform, GetCellPosition(i, pos), _secondsToSwipe));
                    continue;
                }
                _fieldChanged = false;
            }
        }
    }

    private void SlideDown()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 1; j < _height; j++)
            {
                if (_field[i, j] == 0) continue;

                int pos = j - 1;
                while (_field[i, pos] == 0 && pos > 0)
                    pos--;

                _fieldChanged = true;
                if (pos == 0 && _field[i, pos] == 0)
                {
                    _field[i, pos] = _field[i, j];
                    _field[i, j] = 0;
                    StartCoroutine(Move(_cells[i, j].transform, GetCellPosition(i, pos), _secondsToSwipe));
                    continue;
                }

                if (_field[i, pos] != _field[i, j] && pos != j - 1)
                {
                    _field[i, pos + 1] = _field[i, j];
                    _field[i, j] = 0;
                    StartCoroutine(Move(_cells[i, j].transform, GetCellPosition(i, pos + 1), _secondsToSwipe));
                    continue;
                }

                if (_field[i, pos] == _field[i, j])
                {
                    _field[i, pos] *= 2;
                    _field[i, j] = 0;
                    StartCoroutine(Move(_cells[i, j].transform, GetCellPosition(i, pos), _secondsToSwipe));
                    continue;
                }
                _fieldChanged = false;
            }
        }
    }
}