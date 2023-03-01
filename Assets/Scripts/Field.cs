using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

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
        RandomSpawn();
        RenderCells();
    }

    void RandomSpawn()
    {
        int pos = Random.Range(0, _width * _height);
        for (int i = 0; i < _width * _height; i++)
        {
            int clamp = pos % (_width * _height);
            if (_field[clamp / _width, clamp % _height] == 0)
            {
                _field[clamp / _width, clamp % _height] =
                    Random.Range(0f, 1f) > 0.9f ? 4 : 2;
                return;
            }
            pos++;
        }
    }

    bool CanMove()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (i > 0 && (_field[i - 1, j] == _field[i, j] || _field[i - 1, j] == 0))
                    return true;
                if (i < _width - 1 && (_field[i + 1, j] == _field[i, j] || _field[i + 1, j] == 0))
                    return true;
                if (j > 0 && (_field[i, j - 1] == _field[i, j] || _field[i, j - 1] == 0))
                    return true;
                if (j < _height - 1 && (_field[i, j + 1] == _field[i, j] || _field[i, j + 1] == 0))
                    return true;
            }
        }
        return false;
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
                if (_cells[i, j] != null)
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
            case SwipeDirection.Down:
                SlideDown();
                break;
            case SwipeDirection.Up:
                SlideUp();
                break;
        }

        if (_fieldChanged) RandomSpawn();
        if (!CanMove())
            Reload();
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(0);
    }

    private void SlideLeft()
    {
        _fieldChanged = false;
        var sequence = DOTween.Sequence();
        sequence.OnComplete(() =>
        {
            RenderCells();
            _canSwipe = true;
        });
        for (int j = 0; j < _height; j++)
        {
            int offset = -1;
            for (int i = 1; i < _width; i++)
            {
                if (_field[i, j] == 0) continue;

                int pos = i - 1;
                while (_field[pos, j] == 0 && pos > offset + 1)
                    pos--;

                if (_field[pos, j] == 0)
                {
                    _field[pos, j] = _field[i, j];
                    _field[i, j] = 0;
                    _fieldChanged = true;
                    sequence.Insert(0, _cells[i, j].transform.DOMove(GetCellPosition(pos, j), _secondsToSwipe));
                    continue;
                }

                if (_field[pos, j] != _field[i, j] && pos != i - 1)
                {
                    _field[pos + 1, j] = _field[i, j];
                    _field[i, j] = 0;
                    _fieldChanged = true;
                    sequence.Insert(0, _cells[i, j].transform.DOMove(GetCellPosition(pos + 1, j), _secondsToSwipe));
                    continue;
                }

                if (_field[pos, j] == _field[i, j])
                {
                    _field[pos, j] *= 2;
                    _field[i, j] = 0;
                    offset = pos;
                    _fieldChanged = true;
                    sequence.Insert(0, _cells[i, j].transform.DOMove(GetCellPosition(pos, j), _secondsToSwipe));
                    continue;
                }
            }
        }
    }

    private void SlideRight()
    {
        _fieldChanged = false;
        var sequence = DOTween.Sequence();
        sequence.OnComplete(() =>
        {
            RenderCells();
            _canSwipe = true;
        });
        for (int j = 0; j < _height; j++)
        {
            int offset = _width;
            for (int i = _width - 2; i >= 0; i--)
            {
                if (_field[i, j] == 0) continue;

                int pos = i + 1;
                while (_field[pos, j] == 0 && pos < offset - 1)
                    pos++;

                if (_field[pos, j] == 0)
                {
                    _field[pos, j] = _field[i, j];
                    _field[i, j] = 0;
                    _fieldChanged = true;
                    sequence.Insert(0, _cells[i, j].transform.DOMove(GetCellPosition(pos, j), _secondsToSwipe));
                    continue;
                }

                if (_field[pos, j] != _field[i, j] && pos != i + 1)
                {
                    _field[pos - 1, j] = _field[i, j];
                    _field[i, j] = 0;
                    _fieldChanged = true;
                    sequence.Insert(0, _cells[i, j].transform.DOMove(GetCellPosition(pos - 1, j), _secondsToSwipe));
                    continue;
                }

                if (_field[pos, j] == _field[i, j])
                {
                    _field[pos, j] *= 2;
                    _field[i, j] = 0;
                    offset = pos;
                    _fieldChanged = true;
                    sequence.Insert(0, _cells[i, j].transform.DOMove(GetCellPosition(pos, j), _secondsToSwipe));
                    continue;
                }
            }
        }
    }

    private void SlideUp()
    {
        _fieldChanged = false;
        var sequence = DOTween.Sequence();
        sequence.OnComplete(() =>
        {
            RenderCells();
            _canSwipe = true;
        });
        for (int i = 0; i < _width; i++)
        {
            int offset = _height;
            for (int j = _height - 2; j >= 0; j--)
            {
                if (_field[i, j] == 0) continue;

                int pos = j + 1;
                while (_field[i, pos] == 0 && pos < offset - 1)
                    pos++;

                if (_field[i, pos] == 0)
                {
                    _field[i, pos] = _field[i, j];
                    _field[i, j] = 0;
                    _fieldChanged = true;
                    sequence.Insert(0, _cells[i, j].transform.DOMove(GetCellPosition(i, pos), _secondsToSwipe));
                    continue;
                }

                if (_field[i, pos] != _field[i, j] && pos != j + 1)
                {
                    _field[i, pos - 1] = _field[i, j];
                    _field[i, j] = 0;
                    _fieldChanged = true;
                    sequence.Insert(0, _cells[i, j].transform.DOMove(GetCellPosition(i, pos - 1), _secondsToSwipe));
                    continue;
                }

                if (_field[i, pos] == _field[i, j])
                {
                    _field[i, pos] *= 2;
                    _field[i, j] = 0;
                    offset = pos;
                    _fieldChanged = true;
                    sequence.Insert(0, _cells[i, j].transform.DOMove(GetCellPosition(i, pos), _secondsToSwipe));
                    continue;
                }
            }
        }
    }

    private void SlideDown()
    {
        _fieldChanged = false;
        var sequence = DOTween.Sequence();
        sequence.OnComplete(() =>
        {
            RenderCells();
            _canSwipe = true;
        });
        for (int i = 0; i < _width; i++)
        {
            int offset = -1;
            for (int j = 1; j < _height; j++)
            {
                if (_field[i, j] == 0) continue;

                int pos = j - 1;
                while (_field[i, pos] == 0 && pos > offset + 1)
                    pos--;

                if (_field[i, pos] == 0)
                {
                    _field[i, pos] = _field[i, j];
                    _field[i, j] = 0;
                    _fieldChanged = true;
                    sequence.Insert(0, _cells[i, j].transform.DOMove(GetCellPosition(i, pos), _secondsToSwipe));
                    continue;
                }

                if (_field[i, pos] != _field[i, j] && pos != j - 1)
                {
                    _field[i, pos + 1] = _field[i, j];
                    _field[i, j] = 0;
                    _fieldChanged = true;
                    sequence.Insert(0, _cells[i, j].transform.DOMove(GetCellPosition(i, pos + 1), _secondsToSwipe));
                    continue;
                }

                if (_field[i, pos] == _field[i, j])
                {
                    _field[i, pos] *= 2;
                    _field[i, j] = 0;
                    offset = pos;
                    _fieldChanged = true;
                    sequence.Insert(0, _cells[i, j].transform.DOMove(GetCellPosition(i, pos), _secondsToSwipe));
                    continue;
                }
            }
        }
    }
}
