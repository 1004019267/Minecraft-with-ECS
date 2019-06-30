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
using UnityEngine.Audio;
using Unity.Entities;
using System;
using Unity.Transforms;
using Unity.Rendering;

public class PickaxeController : MonoBehaviour
{
    public LayerMask blockLayer;

    GameObject player;

    public static Transform blockToDestroy;

    Material blockToPlace;

    public static int blockID = 1;

    //音效
    public AudioClip grass_audio;
    public AudioClip stone_audio;
    public AudioClip dirt_audio;
    public AudioClip wood_audio;

    AudioSource AS;

    //挖掘喷发的特效
    public ParticleSystem digEffect;

    EntityManager manager;

    GameSetting gs;
    // Start is called before the first frame update
    void Start()
    {
        AS = transform.GetComponent<AudioSource>();
        gs = FindObjectOfType<GameSetting>();
        player = transform.parent.gameObject;
        Cursor.lockState = CursorLockMode.Locked;

        manager = World.Active.GetOrCreateManager<EntityManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //滑动过界就改变为另一边
        if (blockID > 7)
        {
            blockID = 1;
        }
        if (blockID < 1)
        {
            blockID = 7;
        }
        //滑动选择方块
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            blockID++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            blockID--;
        }

        //看是选择哪个砖块
        switch (blockID)
        {
            case 1:
                blockToPlace = gs.stoneMat;
                break;
            case 2:
                blockToPlace = gs.plankMat;
                break;
            case 3:
                blockToPlace = gs.glassMat;
                break;
            case 4:
                blockToPlace = gs.woodMat;
                break;
            case 5:
                blockToPlace = gs.cobbleMat;
                break;
            case 6:
                blockToPlace = gs.tntMat;
                break;
            case 7:
                blockToPlace = gs.brickMat;
                break;
        }
        //左键放方块 右键删除方块
        if (Input.GetMouseButtonDown(1))
        {
            PlaceBlock(blockToPlace);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            DestroyBlock();
        }
    }

    private void PlaceBlock(Material blockToPlace)
    {
        RaycastHit hit;
        //向角色正前方发射射线
        Physics.Raycast(transform.position, transform.forward, out hit,7 ,blockLayer);
        if (hit.transform != null)
        {
            //根据不同方块播放音效
            if (blockID == 1 || blockID == 3 || blockID == 5 || blockID == 7)
            {
                AS.PlayOneShot(stone_audio);
            }
            else if (blockID == 2 || blockID == 4)
            {
                AS.PlayOneShot(wood_audio);
            }
            //创建一个方块在射线前方法线的位置 加上碰撞
            Position pos = new Position { Value = hit.transform.position + hit.normal };
            Entity entities = manager.CreateEntity(GameSetting.blockArchetype);
            manager.SetComponentData(entities, pos);
            manager.AddComponentData(entities, new BlockTag { });
            manager.AddSharedComponentData(entities, new MeshInstanceRenderer
            {
                mesh = gs.blockMesh,
                material = blockToPlace
            });

            gs.colPool.AddCollider(pos.Value);

        }
    }


    private void DestroyBlock()
    {
        RaycastHit hit;
        //向角色正前方发射射线
        Physics.Raycast(transform.position, transform.forward, out hit,7, blockLayer);
        if (hit.transform!=null)
        {
            //在同样位置
            Entity entities = manager.CreateEntity(GameSetting.blockArchetype);
            manager.SetComponentData(entities, new Position { Value=hit.transform.position});
            manager.AddComponentData(entities, new DestoryTag { });

            if (digEffect&&!digEffect.isPlaying)
            {
                digEffect.transform.position = hit.transform.position;
                digEffect.Play();
            }
            //放音效删除
            AS.PlayOneShot(dirt_audio);
            Destroy(hit.transform.gameObject);
        }
    }

}
