using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AllyUnit : MonoBehaviour
{
    private Outline outline;
    private Transform unitTransform;
    void OnMouseDown()
    {
        unitTransform = transform;
        Time.timeScale = 0.5f;
        CustomLogger.Log($"{gameObject.name} was clicked!");
    }
}