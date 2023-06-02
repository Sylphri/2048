using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class Field : MonoBehaviour
{
    [SerializeField] private SwipeDetector _swipeDetector;
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private Transform _tilesParent;
    [SerializeField] private Stats _stats;
    [SerializeField] private PopUpSpawner _spawner;
    [SerializeField] private AudioSource _slideSound;
    [SerializeField] private AudioSource _mergeSound;

    [Header("Field")]
    [SerializeField] private bool _updateMoves = true;
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private float _secondsToSwipe;
    [SerializeField] private float _secondsToPop;
    [SerializeField] private float _popScale;
    [SerializeField] private Color[] _tileColors;
    [SerializeField] private Color[] _textColors;

    public int Width { set { _width = value; } }
    public int Height { set { _height = value; } }
    public bool Bonuses { set { _enableBonuses = value; } }
    
    [Header("Bonuses")]
    [SerializeField] private bool _enableBonuses;
    [SerializeField] private GameObject _bonusMulPrefab;
    [SerializeField] private GameObject _bonusDivPrefab;
    [SerializeField] private GameObject _bonusDesPrefab;

    private const int BONUS_MUL = -1;
    private const int BONUS_DIV = -2;
    private const int BONUS_DES = -3;
    private const int BONUS_COUNT = -3;

    private int[,] _field;
    private Tile[,] _tiles;

    private bool _canSwipe = true;
    private bool _fieldChanged = false;
    private bool _merged = false;

    private void Start()
    {
        _field = new int[_width, _height];
        _tiles = new Tile[_width, _height];
        _swipeDetector.Swiped += OnSwiped;
        RandomSpawn();
        RenderTiles();
    }

    private void RandomSpawn()
    {
        int pos = UnityEngine.Random.Range(0, _width * _height);
        for (int i = 0; i < _width * _height; i++)
        {
            int clamp = pos % (_width * _height);
            if (_field[clamp % _width, clamp / _width] != 0)
            {
                pos++;
                continue;
            }
           
            if (_enableBonuses)
            {
                _field[clamp % _width, clamp / _width] =
                    UnityEngine.Random.Range(0f, 1f) > 0.85f ?
                        UnityEngine.Random.Range(0f, 0.15f) > 0.1f ? 
                            UnityEngine.Random.Range(BONUS_COUNT, 0) 
                        : 4
                    : 2;
            }
            else
            {
                _field[clamp % _width, clamp / _width] =
                    UnityEngine.Random.Range(0f, 1f) > 0.9f ? 4 : 2;
            }
            return;
        }
    }

    private bool CanMove()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_field[i, j] < 0)
                    return true;
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

    private Vector3 GetTilePos(int x, int y)
    {
        return new Vector3((x - _width / 2f + 0.5f) * transform.localScale.x,
                           (y - _height / 2f + 0.5f) * transform.localScale.y,
                          -1.1f) + transform.position;
    }

    private Vector3 GetTilePos(int i, int j, int pos, SwipeDirection dir)
    {
        switch (dir)
        {
            case SwipeDirection.Left:
            case SwipeDirection.Right:
                return GetTilePos(pos, j);       
            case SwipeDirection.Up:
            case SwipeDirection.Down:
                return GetTilePos(i, pos);
            default: throw new Exception("unreachable");
        }
    }
    
    private Vector3 GetPreviousTilePos(int i, int j, int pos, SwipeDirection dir)
    {
        switch (dir)
        {
            case SwipeDirection.Left:
                return GetTilePos(pos + 1, j);
            case SwipeDirection.Right:
                return GetTilePos(pos - 1, j);
            case SwipeDirection.Up:
                return GetTilePos(i, pos - 1);
            case SwipeDirection.Down:
                return GetTilePos(i, pos + 1); 
            default: throw new Exception("unreachable");
        }
    }

    private Vector2Int GetTileIdx(int i, int j, int pos, SwipeDirection dir)
    {
        switch (dir)
        {
            case SwipeDirection.Left:
            case SwipeDirection.Right:
                return new Vector2Int(pos, j);       
            case SwipeDirection.Up:
            case SwipeDirection.Down:
                return new Vector2Int(i, pos); 
            default: throw new Exception("unreachable");
        }
    }

    private void RaiseTileAtPos(int i, int j, int pos, SwipeDirection dir)
    {
        switch (dir)
        {
            case SwipeDirection.Left:
                while (_tiles[pos, j] == null)
                    pos++;
                _tiles[pos, j].transform.position += new Vector3(0f, 0f, -0.1f);
                break;           
            case SwipeDirection.Right:
                while (_tiles[pos, j] == null)
                    pos--;
                _tiles[pos, j].transform.position += new Vector3(0f, 0f, -0.1f);
                break;           
            case SwipeDirection.Up:
                while (_tiles[i, pos] == null)
                    pos--;
                _tiles[i, pos].transform.position += new Vector3(0f, 0f, -0.1f);
                break;           
            case SwipeDirection.Down:
                while (_tiles[i, pos] == null)
                    pos++;
                _tiles[i, pos].transform.position += new Vector3(0f, 0f, -0.1f);
                break;           
            default: throw new Exception("unreachable");
        }
    }

    private void RenderTiles()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_tiles[i, j] != null)
                {
                    Destroy(_tiles[i, j].gameObject);
                    _tiles[i, j] = null;
                }

                if (_field[i, j] == 0) continue;

                GameObject prefab = null;
                switch (_field[i, j])
                {
                    case BONUS_MUL: prefab = _bonusMulPrefab; break;
                    case BONUS_DIV: prefab = _bonusDivPrefab; break;
                    case BONUS_DES: prefab = _bonusDesPrefab; break;
                    default: prefab = _tilePrefab; break;
                }
                
                GameObject tile = Instantiate(prefab, GetTilePos(i, j), Quaternion.identity, _tilesParent);
                Vector3 tileScale = tile.transform.localScale;
                tile.transform.localScale = new Vector3(tileScale.x * transform.localScale.x, tileScale.y * transform.localScale.y, tileScale.z);
                _tiles[i, j] = tile.GetComponent<Tile>();
                if (_field[i, j] > 0)
                {
                    _tiles[i, j].SetValue(_field[i, j].ToString());

                    int index = Mathf.Max(((int)Mathf.Log(_field[i, j], 2)) - 1, 0) % _tileColors.Length;
                    _tiles[i, j].SetBackgroundColor(_tileColors[index]);
                    _tiles[i, j].SetTextColor(_textColors[index]);
                }
            }
        }
    }

    private IEnumerator PopTiles(List<Vector2Int> indeces)
    {
        if (indeces.Count != 0)
        {
            List<Transform> tiles = new List<Transform>();
            foreach (var idx in indeces)
                tiles.Add(_tiles[idx.x, idx.y].transform);

            float past = 0;
            Vector3 scale = tiles[0].transform.localScale;
            while (past < _secondsToPop)
            {
                float ratio = past / _secondsToPop * Mathf.PI;
                foreach (var tile in tiles)
                {
                    if (tile != null)
                        tile.localScale = new Vector3(Mathf.Sin(ratio) * _popScale + scale.x, Mathf.Sin(ratio) * _popScale + scale.y, scale.z);       
                }
                past += Time.deltaTime;
                yield return null;
            }
            
            foreach (var tile in tiles)
                if (tile != null)
                    tile.localScale = scale; 
        }
        yield return null;
    }

    private void OnSwiped(SwipeDirection direction)
    {
        if (!_canSwipe || _stats.gameOver) return;
        _canSwipe = false;

        Slide(direction);

        if (_fieldChanged) 
        {
            RandomSpawn();
            if (_updateMoves) _stats.AddMove();
        }
        
        if (!CanMove()) 
            _stats.GameOver();
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
                return new SlideData(Enumerable.Range(0, _width), Enumerable.Range(1, _height - 1), -1, false);
            default: throw new Exception("unreachable");
        }
    }

    private int GetPos(int i, int j, SwipeDirection dir, int prev, bool prevMerge)
    {
        SlideData data = GetSlideData(dir);
        int pos = 0;
        switch (dir)
        {
            case SwipeDirection.Left:
                pos = i - 1;
                while (_field[pos, j] == 0 && pos > data.offset + 1 && (!prevMerge || pos > prev + 1))
                    pos--;
                break;
            case SwipeDirection.Right:
                pos = i + 1;
                while (_field[pos, j] == 0 && pos < data.offset - 1 && (!prevMerge || pos < prev - 1))
                    pos++;
                break;
            case SwipeDirection.Up:
                pos = j + 1;
                while (_field[i, pos] == 0 && pos < data.offset - 1 && (!prevMerge || pos < prev - 1))
                    pos++;
                break;
            case SwipeDirection.Down:
                pos = j - 1;
                while (_field[i, pos] == 0 && pos > data.offset + 1 && (!prevMerge || pos > prev + 1))
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
        List<Vector2Int> mergedTiles = new List<Vector2Int>();
        var sequence = DOTween.Sequence();
        sequence.OnComplete(() =>
        {
            RenderTiles();
            StartCoroutine(PopTiles(mergedTiles));
            if (_merged) 
            {
                if (PlayerPrefs.GetInt("Sounds", 1) == 1)
                    _mergeSound.Play();
                _merged = false;
            };
            _canSwipe = true;
        });

        int totalScore = 0;
        SlideData data = GetSlideData(dir);
        foreach (int x in data.iRange)
        {
            int offset = data.offset;
            int pos = 0;
            bool prevMerge = false;
            foreach (int y in data.jRange)
            {
                (int i, int j) = (x, y);
                if (data.swap) (i, j) = (j, i);
                if (_field[i, j] == 0) continue;

                pos = GetPos(i, j, dir, pos, prevMerge);
                int atPos = GetFieldAtPos(i, j, pos, dir); 
                prevMerge = false;
                
                if (atPos == 0)
                {
                    SetFieldAtPos(i, j, pos, dir, _field[i, j]);
                    _field[i, j] = 0;
                    _fieldChanged = true;
                    sequence.Insert(0, _tiles[i, j].transform.DOMove(GetTilePos(i, j, pos, dir), _secondsToSwipe));
                    continue;
                }

                if (atPos != _field[i, j] && atPos > -1 && _field[i, j] > -1 && NotPrevious(i, j, pos, dir))
                {
                    SetPrevious(i, j, pos, dir, _field[i, j]);
                    _field[i, j] = 0;
                    _fieldChanged = true;
                    sequence.Insert(0, _tiles[i, j].transform.DOMove(GetPreviousTilePos(i, j, pos, dir), _secondsToSwipe));
                    continue;
                }

                // merge
                int newValue = -1;
                
                if (atPos < 0 && _field[i, j] < 0) continue;
                
                if (atPos == BONUS_MUL || _field[i, j] == BONUS_MUL)
                    newValue = atPos == BONUS_MUL ? _field[i, j] * 2 : atPos * 2;
                else if (atPos == BONUS_DIV || _field[i, j] == BONUS_DIV)
                    newValue = atPos == BONUS_DIV ? _field[i, j] / 2 : atPos / 2;
                else if (atPos == BONUS_DES || _field[i, j] == BONUS_DES)
                    newValue = 0;
                else if (atPos == _field[i, j])
                    newValue = atPos * 2;

                if (newValue > -1)
                {
                    RaiseTileAtPos(i, j, pos, dir);
                    sequence.Insert(0, _tiles[i, j].transform.DOMove(GetTilePos(i, j, pos, dir), _secondsToSwipe));
                    
                    _stats.IncreaseScore(newValue);
                    if (!_stats.gameWin && newValue == 2048)
                        _stats.Win();
                    
                    SetFieldAtPos(i, j, pos, dir, newValue);
                    _field[i, j] = 0;
                    
                    if (newValue != 0)
                        mergedTiles.Add(GetTileIdx(i, j, pos, dir));
                    
                    totalScore += newValue;
                    offset = pos;
                    prevMerge = true;
                    _merged = true;
                    _fieldChanged = true;
                }
            }
        }

        if (_fieldChanged && PlayerPrefs.GetInt("Sounds", 1) == 1)
            _slideSound.Play();
        
        if (totalScore > 0)
            _spawner.Spawn(totalScore.ToString());
    }
}
