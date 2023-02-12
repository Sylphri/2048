using UnityEngine;

public class Field
{
    private readonly int[,] _field;

    public int Width { get; private set; }
    public int Height { get; private set; }

    public Field(int width, int height)
    {
        Width = width;
        Height = height;
        _field = new int[Width, Height];
        _field[Random.Range(0, Width), Random.Range(0, Height)] = 2;
    }

    public int this[int i, int j]
    {
        get { return _field[i, j]; }
        set { _field[i, j] = value; }
    }

    public void Slide(SwipeDirection direction)
    {
        switch(direction)
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

        int x = Random.Range(0, Width);
        int y = Random.Range(0, Height);
        while (_field[x, y] != 0)
        {
            x = Random.Range(0, Width);
            y = Random.Range(0, Height);
        }
        _field[x, y] = 2;
    }

    private void SlideLeft()
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 1; j < Width; j++)
            {
                if (_field[j, i] == 0 && _field[j - 1, i] != 0 && _field[j - 1, i] != _field[j, i])
                    continue;

                int pos = j - 1;
                while (pos > 0 && _field[pos, i] == 0)
                    pos--;

                if (_field[pos, i] == _field[j, i])
                {
                    _field[pos, i] *= 2;
                    _field[j, i] = 0;
                }
                else
                {
                    if (_field[pos, i] != 0)
                        _field[pos + 1, i] = _field[j, i];
                    else
                        _field[pos, i] = _field[j, i];
                    _field[j, i] = 0;
                }
            }
        }
    }

    private void SlideRight()
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = Width - 2; j >= 0; j--)
            {
                if (_field[j, i] == 0 && _field[j + 1, i] != 0 && _field[j + 1, i] != _field[j, i])
                    continue;

                int pos = j + 1;
                while (pos < Width - 1 && _field[pos, i] == 0)
                    pos++;

                if (_field[pos, i] == _field[j, i])
                {
                    _field[pos, i] *= 2;
                    _field[j, i] = 0;
                }
                else
                {
                    if (_field[pos, i] != 0)
                        _field[pos - 1, i] = _field[j, i];
                    else
                        _field[pos, i] = _field[j, i];
                    _field[j, i] = 0;
                }
            }
        }
    }

    private void SlideUp()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = Height - 2; j >= 0; j--)
            {
                if (_field[i, j] == 0 && _field[i, j + 1] != 0 && _field[i, j + 1] != _field[i, j])
                    continue;

                int pos = j + 1;
                while (pos < Height - 1 && _field[i, pos] == 0)
                    pos++;

                if (_field[i, pos] == _field[i, j])
                {
                    _field[i, pos] *= 2;
                    _field[i, j] = 0;
                }
                else
                {
                    if (_field[i, pos] != 0)
                        _field[i, pos - 1] = _field[i, j];
                    else
                        _field[i, pos] = _field[i, j];
                    _field[i, j] = 0;
                }
            }
        }
    }

    private void SlideDown()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 1; j < Height; j++)
            {
                if (_field[i, j] == 0 && _field[i, j - 1] != 0 && _field[i, j - 1] != _field[i, j])
                    continue;

                int pos = j - 1;
                while (pos > 0 && _field[i, pos] == 0)
                    pos--;

                if (_field[i, pos] == _field[i, j])
                {
                    _field[i, pos] *= 2;
                    _field[i, j] = 0;
                }
                else
                {
                    if (_field[i, pos] != 0)
                        _field[i, pos + 1] = _field[i, j];
                    else
                        _field[i, pos] = _field[i, j];
                    _field[i, j] = 0;
                }
            }
        }
    }
}
