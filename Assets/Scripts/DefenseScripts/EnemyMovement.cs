using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 1.0f;
    private Rigidbody2D _rigid2d;
    private Animator _animator;
    private void Awake()
    {
        _rigid2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _rigid2d.velocity = Vector3.down * (speed * Time.timeScale);
        _animator.Play("1_Run");
    }
}
