using System.Collections.Generic;
using UnityEngine;

public class Block : Component
{
    List<Arc> arc_list;

    public static GameObject CreateBlock(string ssvg_file)
    {
        GameObject result = new GameObject();
        Block block = result.AddComponent<Block>();

        block.arc_list = new List<Arc>();

        return result;
    }
}