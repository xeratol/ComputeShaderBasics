using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorDemoController : MonoBehaviour
{
    [SerializeField]
    private ComputeShader _shader = null;

    [SerializeField]
    private Material _material = null;

    [SerializeField]
    private Color _color;

    private RenderTexture _texture = null;

    void Start()
    {
        _texture = new RenderTexture(32, 32, 24);
        _texture.filterMode = FilterMode.Point;
        _texture.enableRandomWrite = true; // IMPORTANT!
        _texture.Create();

        Debug.Assert(_texture != null, "Failed to create Render Texture", this);

        _material.mainTexture = _texture;

        var kernel = _shader.FindKernel("Colorize");
        _shader.SetTexture(kernel, "Result", _texture);
        _shader.SetFloats("Color", new[] { _color.r, _color.g, _color.b });
        _shader.Dispatch(kernel, 4, 4, 1);
    }

    private void OnDestroy()
    {
        _texture.Release();
        _texture = null;
    }
}
