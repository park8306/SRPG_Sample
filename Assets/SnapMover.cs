using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SnapMover : MonoBehaviour
{
    // 스냅이동을 하고 싶은 오브젝트에게 스냅 이동을 시키자
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
