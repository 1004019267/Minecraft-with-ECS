/**
 *Copyright(C) 2019 by #COMPANY#
 *All rights reserved.
 *FileName:     #SCRIPTFULLNAME#
 *Author:       #AUTHOR#
 *Version:      #VERSION#
 *UnityVersion:#UNITYVERSION#
 *Date:         #DATE#
 *Description:   
 *History:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ToolbarScript : MonoBehaviour
{
    public int blockNum;
    public Transform select;
    // Start is called before the first frame update
    void Start()
    {
        select = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (PickaxeController.blockID==blockNum)
        {
            select.GetComponent<RawImage>().enabled = true;
        }
        else
        {
            select.GetComponent<RawImage>().enabled = false;
        }
    }
}
