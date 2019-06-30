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
using System;
using Unity.Entities;
//都只是作为一个标签
//比如自带的Position 如果需要的属性没有可以自己在这边定义
[Serializable]
public struct BlockTag : IComponentData { }
public class BlockTagComponment : ComponentDataWrapper<BlockTag> { };

[Serializable]
public struct DestoryTag : IComponentData { }
public class DestoryTagComponment : ComponentDataWrapper<DestoryTag> { };

[Serializable]
public struct SurfacePlantTag : IComponentData { }
public class SurfacePlantTagComponment : ComponentDataWrapper<SurfacePlantTag> { };