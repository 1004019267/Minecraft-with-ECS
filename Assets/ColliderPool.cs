using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderPool 
{
    GameObject boxCollider;
    Transform parent;

    public ColliderPool(GameObject boxCollider, Transform parent)
    {
        this.boxCollider = boxCollider;
        this.parent = parent;
    }
    //在同样位置创建一个单位碰撞
    public void AddCollider(Vector3 vec3)
    {
        GameObject obj = GameObject.Instantiate(boxCollider);
        obj.transform.position = vec3;
        obj.transform.parent = parent;
        obj.layer = 9;
    }
}
