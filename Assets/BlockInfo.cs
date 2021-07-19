using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    Walkable,
    Water,
}
public class BlockInfo : MonoBehaviour
{
    public BlockType blockType;
}
