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
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

//IJobProcessComponentData可以放到JobSystem处理
public class DestroySystem : ComponentSystem
{
    //类似于Select语句
    struct BlockGroup
    {
        [ReadOnly] public readonly int Length;
        [ReadOnly] public EntityArray entity;
        //就是检索语句 符合的条件都在这里面
        [ReadOnly] public ComponentDataArray<Position> postions;
        [ReadOnly] public ComponentDataArray<BlockTag> tags;

    }

    struct DestoryBlockGroup
    {
        [ReadOnly] public readonly int Length;
        [ReadOnly] public EntityArray entity;
        [ReadOnly] public ComponentDataArray<Position> postions;
        [ReadOnly] public ComponentDataArray<DestoryTag> tags;
    }
    //某些方块上的花
    struct SurfacePlantGroup
    {
        [ReadOnly] public readonly int Length;
        [ReadOnly] public EntityArray entity;
        [ReadOnly] public ComponentDataArray<Position> postions;
        [ReadOnly] public ComponentDataArray<SurfacePlantTag> tags;
    }
    //群组 Inject是注入数据
    [Inject] BlockGroup targetBlocks;
    [Inject] DestoryBlockGroup sourceBlocks;
    [Inject] SurfacePlantGroup surfacePlants;

    protected override void OnUpdate()
    {
        for (int i = 0; i < sourceBlocks.Length; i++)
        {
            for (int j = 0; j < targetBlocks.Length; j++)
            {
                Vector3 offect = targetBlocks.postions[j].Value - sourceBlocks.postions[i].Value;
                //平方
                float sqrLen = offect.sqrMagnitude;
                //就是删除方块组中有和总方块组一样的 
                if (sqrLen==0)
                {
                    //同时寻找砖块上是否有草 有了也删除 就是草的位置y-1如果等于现在位置就删除
                    for (int k = 0; k < surfacePlants.Length; k++)
                    {
                        float3 pos = new float3(surfacePlants.postions[k].Value.x, surfacePlants.postions[k].Value.y + Vector3.down.y, surfacePlants.postions[k].Value.z);
                        offect = targetBlocks.postions[j].Value - pos;
                        sqrLen = offect.sqrMagnitude;

                        if (sqrLen == 0)
                        {
                            PostUpdateCommands.DestroyEntity(surfacePlants.entity[k]);
                        }
                    }

                    //删除 删除方块组和总方块组的该entity
                    PostUpdateCommands.DestroyEntity(sourceBlocks.entity[i]);
                    PostUpdateCommands.DestroyEntity(targetBlocks.entity[j]);
                }
            }
        }
    }
}
