using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float gravity = -50f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] private float moveInput = 1;
    [SerializeField] public Transform leader;
    [SerializeField] public Transform ceilingCollider;


    const int MAX_FPS = 30;
    public float lagSeconds = 0.5f;


    private Vector3 move;
    Vector3[] _positionBuffer;
    float[] _timeBuffer;
    int _oldestIndex;
    int _newestIndex;
    private Animator _animator;
    private bool isGrounded;
    private bool isUnderCeiling;
    private bool isInitial;
    private float counter = 15;

    // Use this for initialization
    void Start()
    {
        int bufferLength = Mathf.CeilToInt(lagSeconds * MAX_FPS);
        _positionBuffer = new Vector3[bufferLength];
        _timeBuffer = new float[bufferLength];

        _positionBuffer[0] = _positionBuffer[1] = new Vector3(leader.position.x - 50f,leader.position.y, 0);
        _timeBuffer[0] = _timeBuffer[1] = Time.time;

        _oldestIndex = 0;
        _newestIndex = 1;

        _animator = GetComponent<Animator>();
        isInitial = true;
    }


    void LateUpdate()
    {
        if (isInitial)
        {
            counter -= Time.deltaTime;

            // Insert newest position into our cache.
            // If the cache is full, overwrite the latest sample.
            int newIndex = (_newestIndex + 1) % _positionBuffer.Length;
            if (newIndex != _oldestIndex)
                _newestIndex = newIndex;

            _positionBuffer[_newestIndex] = new Vector3(leader.position.x - counter, leader.position.y + 25 - counter, 0);
            _timeBuffer[_newestIndex] = Time.time;

            // Skip ahead in the buffer to the segment containing our target time.
            float targetTime = Time.time - lagSeconds;
            int nextIndex;
            while (_timeBuffer[nextIndex = (_oldestIndex + 1) % _timeBuffer.Length] < targetTime)
                _oldestIndex = nextIndex;

            // Interpolate between the two samples on either side of our target time.
            float span = _timeBuffer[nextIndex] - _timeBuffer[_oldestIndex];
            float progress = 0f;
            if (span > 0f)
            {
                progress = (targetTime - _timeBuffer[_oldestIndex]) / span;
            }

            if (counter < 14)
            {
                isInitial = false;
            }
        } else
        {
            // Insert newest position into our cache.
            // If the cache is full, overwrite the latest sample.
            int newIndex = (_newestIndex + 1) % _positionBuffer.Length;
            if (newIndex != _oldestIndex)
                _newestIndex = newIndex;

            _positionBuffer[_newestIndex] = new Vector3(leader.position.x, leader.position.y - 0.2f, leader.position.z);
            _timeBuffer[_newestIndex] = Time.time;

            // Skip ahead in the buffer to the segment containing our target time.
            float targetTime = Time.time - lagSeconds;
            int nextIndex;
            while (_timeBuffer[nextIndex = (_oldestIndex + 1) % _timeBuffer.Length] < targetTime)
                _oldestIndex = nextIndex;

            // Interpolate between the two samples on either side of our target time.
            float span = _timeBuffer[nextIndex] - _timeBuffer[_oldestIndex];
            float progress = 0f;
            if (span > 0f)
            {
                progress = (targetTime - _timeBuffer[_oldestIndex]) / span;
            }
            isUnderCeiling = Physics.CheckSphere(ceilingCollider.position, 0.1f, groundLayer, QueryTriggerInteraction.Ignore);
            isGrounded = Physics.CheckSphere(transform.position, 0.1f, groundLayer, QueryTriggerInteraction.Ignore);

            if (isGrounded && move.y < 0)
            {
                move.y = 0;
            }
            else
            {
                move.y += gravity * Time.deltaTime;
            }

            transform.position = Vector3.Lerp(_positionBuffer[_oldestIndex], _positionBuffer[nextIndex], progress);

            _animator.SetFloat("Speed", moveInput);

            _animator.SetBool("isGrounded", isGrounded);

            _animator.SetFloat("FallingSpeed", move.y);

            _animator.SetBool("isSliding", isUnderCeiling);
        }
        
    }

    void OnDrawGizmos()
    {
        if (_positionBuffer == null || _positionBuffer.Length == 0)
            return;

        Gizmos.color = Color.grey;

        Vector3 oldPosition = _positionBuffer[_oldestIndex];
        int next;
        for (int i = _oldestIndex; i != _newestIndex; i = next)
        {
            next = (i + 1) % _positionBuffer.Length;
            Vector3 newPosition = _positionBuffer[next];
            Gizmos.DrawLine(oldPosition, newPosition);
            oldPosition = newPosition;
        }
    }
}
