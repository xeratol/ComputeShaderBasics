using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLifeController : MonoBehaviour
{
    [SerializeField]
    private ComputeShader _shader = null;
    private ComputeBuffer _lifeBuffer = null;
    private RenderTexture _texture = null;

    [SerializeField]
    private Material _material = null;

    [SerializeField]
    private Vector2Int _dimensions = new Vector2Int(16, 16);

    private int _stepKernel = -1;
    private int _drawKernel = -1;

    [SerializeField, Min(0.1f)]
    private float _frameDelay = 10;
    private float _frameTimer = 0;

    private void OnValidate()
    {
        _dimensions.x = Mathf.Max(_dimensions.x, 8);
        _dimensions.y = Mathf.Max(_dimensions.y, 8);
    }

    void Start()
    {
        _stepKernel = _shader.FindKernel("Step");
        _drawKernel = _shader.FindKernel("Draw");

        _lifeBuffer = new ComputeBuffer(_dimensions.x * _dimensions.y, sizeof(uint));
        var initData = new uint[_dimensions.x, _dimensions.y];
        initData[1, 0] = 1;
        initData[2, 1] = 1;
        initData[0, 2] = 1;
        initData[1, 2] = 1;
        initData[2, 2] = 1;
        _lifeBuffer.SetData(initData);

        _texture = new RenderTexture(_dimensions.x, _dimensions.y, 8);
        _texture.filterMode = FilterMode.Point;
        _texture.enableRandomWrite = true;
        _texture.Create();

        _material.mainTexture = _texture;

        _shader.SetInt("WIDTH", _dimensions.x);
        _shader.SetInt("HEIGHT", _dimensions.y);
    }

    private void OnDestroy()
    {
        if (_lifeBuffer != null)
        {
            _lifeBuffer.Release();
            _lifeBuffer = null;
        }

        if (_texture != null)
        {
            _texture.Release();
            _texture= null;
        }
    }

    void Update()
    {
        _frameTimer += Time.deltaTime;
        if (_frameTimer >= _frameDelay)
        {
            _frameTimer -= _frameDelay;

            _shader.SetBuffer(_stepKernel, "Life", _lifeBuffer);
            _shader.Dispatch(_stepKernel, (_dimensions.x + 7) / 8, (_dimensions.y + 7) / 8, 1);

            _shader.SetBuffer(_drawKernel, "Life", _lifeBuffer);
            _shader.SetTexture(_drawKernel, "Result", _texture);
            _shader.Dispatch(_drawKernel, (_dimensions.x + 7) / 8, (_dimensions.y + 7) / 8, 1);
        }
    }
}
