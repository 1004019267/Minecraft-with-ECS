using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;

/// <summary>
/// 加载一堆方块
/// </summary>
public class SpawnNumBlocks : MonoBehaviour
{
    Texture2D heightmap;

    public static EntityArchetype blockArchetype;

    //10X10的地方留着 之外的不显示或删除 类似于遮挡剔除
    [Header("Wrold = ChunkBase x ChunkBase")]
    public int chunckBase = 1;

    [Header("Mesh Info")]
    public Mesh blockMesh;

    [Header("For Log")]
    public Material[] mats;

    Material maTemp;

    public EntityManager manager;
    public Entity entities;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        EntityManager manager = World.Active.GetOrCreateManager<EntityManager>();

        blockArchetype = manager.CreateArchetype(
            typeof(Position)
            );
    }

    void Start()
    {
        manager = World.Active.GetOrCreateManager<EntityManager>();
        PerlinNoiseGenerator perlin = new PerlinNoiseGenerator();
        heightmap = perlin.GenerateHeightMap();
        ChunkGenerator(chunckBase);
    }

    void ChunkGenerator(int amount)
    {
        //一个chunckBase相当于1500个方块
        int totalamount = (amount * amount) * 1500;

        int highlevel;
        bool airChecker;

        for (int y = 0; y < 15; y++)
        {
            for (int x = 0; x < 10 * amount; x++)
            {
                for (int z = 0; z < 10 * amount; z++)
                {
                    //返回像素颜色 数很小乘上100
                    highlevel = (int)(heightmap.GetPixel(x, z).r * 100) - y;
                    airChecker = false;

                    Vector3 posTemp = new Vector3(x, y, z);
              
                    if (highlevel>=0& highlevel< mats.Length-1)
                    {
                        maTemp = mats[highlevel];
                    }
                    else
                    {
                         //超过的视为空气
                            maTemp = mats[mats.Length - 1];
                            airChecker = true;
                    }
                  
                    if (!airChecker)
                    {
                        Entity entities = manager.CreateEntity(blockArchetype);
                        manager.SetComponentData(entities, new Position { Value = new int3(x, y, z) });
                        //manager.AddComponentData(entities, new BlockTag { });

                        manager.AddSharedComponentData(entities, new MeshInstanceRenderer
                        {
                            mesh = blockMesh,
                            material = maTemp,
                        });
                    }
                }
            }
        }
    }


}
