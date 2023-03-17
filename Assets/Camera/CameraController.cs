/**
 * Shernan Javier
 * 
 * References:
 * https://www.maxester.com/blog/2020/02/24/how-do-you-make-the-camera-follow-the-player-in-unity-3d/
 * https://gamedevsolutions.com/smooth-camera-follow-in-unity3d-c/
 * 
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target = null;

    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x + 3, target.position.y - 2, target.position.z + 1) + offset, Time.deltaTime * 3);
    }
}
