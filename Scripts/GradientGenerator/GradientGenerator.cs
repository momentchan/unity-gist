using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientGenerator : MonoBehaviour
{
    [SerializeField] private Shader shader;
    private Material mat;
    [SerializeField] private RenderTexture source;
    [SerializeField] private RenderTexture output;

    void Start()
    {
        mat = new Material(shader);
    }

    void Update()
    {
        Graphics.Blit(source, output, mat);
    }
}
