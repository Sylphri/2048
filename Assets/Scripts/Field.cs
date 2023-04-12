using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

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

    private void RandomSpawn()
    {
        int pos = UnityEngine.Random.Range(0, _width * _height);
        for (int i = 0; i < _width * _height; i++)
        {
            int clamp = pos % (_width * _height);
            if (_field[clamp % _width, clamp / _height] == 0)
            {
                _field[clamp % _width, clamp / _height] =
                    UnityEngine.Random.Range(0f, 1f) > 0.9f ? 4 : 2;
                return;
            }
            pos++;
        }
    }

    // TODO: implement properly
    private bool CanMove()
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

    private Vector3 GetCellPosition(int x, int y)
    {
        return new Vector3(x - _width / 2f + 0.5f, y - _height / 2f + 0.5f, 0);
    }

    private Vector3 GetCellPosition(int i, int j, int pos, SwipeDirection dir)
    {
        switch (dir)
        {
            case SwipeDirection.Left:
            case SwipeDirection.Right:
                return GetCellPosition(pos, j);       
            case SwipeDirection.Up:
            case SwipeDirection.Down:
                return GetCellPosition(i, pos);
            default: throw new Exception("unreachable");
        }
    }

    private Vector3 GetPreviousCellPos(int i, int j, int pos, SwipeDirection dir)
    {
        switch (dir)
        {
            case SwipeDirection.Left:
                return GetCellPosition(pos + 1, j);
            case SwipeDirection.Right:
                return GetCellPosition(pos - 1, j);
            case SwipeDirection.Up:
                return GetCellPosition(i, pos - 1);
            case SwipeDirection.Down:
                return GetCellPosition(i, pos + 1); 
            default: throw new Exception("unreachable");
        }
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

        Slide(direction);

        if (_fieldChanged) RandomSpawn();
        if (!CanMove()) Reload();
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(0);
    }

    private struct SlideData
    {
        public IEnumerable iRange;
        public IEnumerable jRange;
        public int offset;
        public bool swap;

        public SlideData(IEnumerable iRange, IEnumerable jRange, int offset, bool swap)
        {
            this.iRange = iRange;
            this.jRange = jRange;
            this.offset = offset;
            this.swap = swap;
        }
    }

    private SlideData GetSlideData(SwipeDirection dir)
    {
        switch (dir)
        {
            case SwipeDirection.Left:
                return new SlideData(Enumerable.Range(0, _height), Enumerable.Range(1, _width - 1), -1, true);
            case SwipeDirection.Right:
                return new SlideData(Enumerable.Range(0, _height), Enumerable.Range(0, _width - 1).Reverse(), _width, true);
            case SwipeDirection.Up:
                return new SlideData(Enumerable.Range(0, _width), Enumerable.Range(0, _height - 1).Reverse(), _height, false);
            case SwipeDirection.Down:
                return new SlideData(Enumerable.Range(0, _width), Enumerable.Range(1, _height - 1), -1,false);
            default: throw new Exception("unreachable");
        }
    }

    private int GetPos(int i, int j, SwipeDirection dir)
    {
        SlideData data = GetSlideData(dir);
        int pos = 0;
        switch (dir)
        {
            case SwipeDirection.Left:
                pos = i - 1;
                while (_field[pos, j] == 0 && pos > data.offset + 1)
                    pos--;
                break;
            case SwipeDirection.Right:
                pos = i + 1;
                while (_field[pos, j] == 0 && pos < data.offset - 1)
                    pos++;
                break;
            case SwipeDirection.Up:
                pos = j + 1;
                while (_field[i, pos] == 0 && pos < data.offset - 1)
                    pos++;
                break;
            case SwipeDirection.Down:
                pos = j - 1;
                while (_field[i, pos] == 0 && pos > data.offset + 1)
                    pos--;
                break;
            default: throw new Exception("unreachable");
        }
        return pos;
    }

    private int GetFieldAtPos(int i, int j, int pos, SwipeDirection dir)
    {
        switch (dir)
        {
            case SwipeDirection.Right:
            case SwipeDirection.Left:
                return _field[pos, j];       
            case SwipeDirection.Up:
            case SwipeDirection.Down:
                return _field[i, pos];
            default: throw new Exception("unreachable");
        }
    }
    
    private void SetFieldAtPos(int i, int j, int pos, SwipeDirection dir, int value)
    {
        switch (dir)
        {
            case SwipeDirection.Right:
            case SwipeDirection.Left:
                _field[pos, j] = value;       
                break;
            case SwipeDirection.Up:
            case SwipeDirection.Down:
                _field[i, pos] = value;
                break;
            default: throw new Exception("unreachable");
        }
    }

    private bool NotPrevious(int i, int j, int pos, SwipeDirection dir)
    {
        switch (dir)
        {
            case SwipeDirection.Left:
                return pos != i - 1;
            case SwipeDirection.Right:
                return pos != i + 1;
            case SwipeDirection.Up:
                return pos != j + 1;
            case SwipeDirection.Down:
                return pos != j - 1;
            default: throw new Exception("unreachable");
        }
    }

    private void SetPrevious(int i, int j, int pos, SwipeDirection dir, int value)
    {
        switch (dir)
        {
            case SwipeDirection.Left:
                _field[pos + 1, j] = value;
                break;
            case SwipeDirection.Right:
                _field[pos - 1, j] = value;
                break;
            case SwipeDirection.Up:
                _field[i, pos - 1] = value;
                break;
            case SwipeDirection.Down:
                _field[i, pos + 1] = value; 
                break;
            default: throw new Exception("unreachable");
        }
    }

    private void Slide(SwipeDirection dir)
    {
        _fieldChanged = false;
        var sequence = DOTween.Sequence();
        sequence.OnComplete(() =>
        {
            RenderCells();
            _canSwipe = true;
        });

        SlideData data = GetSlideData(dir);
        foreach (int x in data.iRange)
        {
            int offset = data.offset;
            foreach (int y in data.jRange)
            {
                (int i, int j) = (x, y);
                if (data.swap) (i, j) = (j, i);
                if (_field[i, j] == 0) continue;

                int pos = GetPos(i, j, dir);
                if (GetFieldAtPos(i, j, pos, dir) == 0)
                {
                    SetFieldAtPos(i, j, pos, dir, _field[i, j]);
                    _field[i, j] = 0;
                    _fieldChanged = true;
                    sequence.Insert(0, _cells[i, j].transform.DOMove(GetCellPosition(i, j, pos, dir), _secondsToSwipe));
                    continue;
                }

                if (GetFieldAtPos(i, j, pos, dir) != _field[i, j] && NotPrevious(i, j, pos, dir))
                {
                    SetPrevious(i, j, pos, dir, _field[i, j]);
                    _field[i, j] = 0;
                    _fieldChanged = true;
                    sequence.Insert(0, _cells[i, j].transform.DOMove(GetPreviousCellPos(i, j, pos, dir), _secondsToSwipe));
                    continue;
                }

                if (GetFieldAtPos(i, j, pos, dir) == _field[i, j])
                {
                    SetFieldAtPos(i, j, pos, dir,GetFieldAtPos(i, j, pos, dir) * 2);
                    _field[i, j] = 0;
                    offset = pos;
                    _fieldChanged = true;
                    sequence.Insert(0, _cells[i, j].transform.DOMove(GetCellPosition(i, j, pos, dir), _secondsToSwipe));
                    continue;
                }
            }
        }
    }
}
