using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrayScaleController : MonoBehaviour
{
    [SerializeField]
    private ComputeShader _shader = null;

    private RenderTexture _texture = null;

    private int _grayScaleKernel = -1;

    private void Start()
    {
        _grayScaleKernel = _shader.FindKernel("GrayScale");
    }

    private void OnDestroy()
    {
        if (_texture != null)
        {
            _texture.Release();
            _texture = null;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_texture == null)
        {
            CreateTexture();
        }
        else if (_texture.width != Screen.width || _texture.height != Screen.height)
        {
            _texture.Release();
            CreateTexture();
        }

        Graphics.Blit(source, _texture);
        _shader.SetTexture(_grayScaleKernel, "Dest", _texture);
        _shader.Dispatch(_grayScaleKernel, (Screen.width + 7) / 8, (Screen.height + 7) / 8, 1);
        Graphics.Blit(_texture, destination);
    }

    private void CreateTexture()
    {
        _texture = new RenderTexture(Screen.width, Screen.height, 24);
        _texture.enableRandomWrite = true; // IMPORTANT!
        _texture.Create();
    }
}
