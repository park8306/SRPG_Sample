using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SnapMover : MonoBehaviour
{
    private void Start()
    {
        if (Application.isPlaying)
        {
            Destroy(this);
        }
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position.ToVector3Snap();
    }
}
