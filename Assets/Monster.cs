using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GroundManager.Instance.AddBlockInfo(transform.position, BlockType.Monster);   
    }
}
