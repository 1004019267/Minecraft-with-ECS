using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using System;

public class GameSetting : MonoBehaviour
{
    Texture2D heightmap;

    public static EntityArchetype blockArchetype;

    [Header("Wrold = ChunkBase x ChunkBase")]
    public int chunckBase = 1;

    [Header("Mesh Info")]
    public Mesh blockMesh;
    public Mesh surfaceMesh;
    public Mesh tallGrassMesh;

    [Header("Nature Block Type")]
    public Material stoneMat;
    public Material woodMat;
    public Material leavesMat;
    public Material surfaceMat;
    public Material cobbleMat;
    public Material dirtMaterial;
    public Material tallGrassMat;
    public Material roseMat;
    public Material CloudMat;

    [Header("Other Block Type")]
    public Material glassMat;
    public Material brickMat;
    public Material plankMat;
    public Material tntMat;
    //找不到用粉色
    [Header("")]
    public Material pinkMat;

    public bool createCollider = true;

    public GameObject boxCollider;
    Mesh meshTemp;
    Material maTemp;

    EntityManager manager;
    Entity entities;

    int random;

    public ColliderPool colPool;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        //检查场景是否有 有得到没有创建
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

        //创建一个碰撞池
        colPool = new ColliderPool(boxCollider, transform);

        ChunckGenerator(chunckBase);
    }

    void ChunckGenerator(int amount)
    {
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

                    switch (highlevel)
                    {
                        //表层 根据一个单位 和 多个单位 分开方法创建
                        case 0:
                            random = UnityEngine.Random.Range(1, 201);
                            if (random <= 20)
                            {
                                //草
                                PlantGenerator(x, y, z, 1);
                            }
                            else if (random == 198)
                            {
                                //云
                                CloudGenerator(x, y, z);
                            }
                            else if (random == 199)
                            {
                                //树
                                TreeGenerator(x, y, z);
                            }
                            else if (random == 200)
                            {
                                //花
                                PlantGenerator(x, y, z, 2);
                            }
                            airChecker = true;
                            break;
                        case 1:
                            //绿色带土的方块
                            meshTemp = surfaceMesh;
                            maTemp = surfaceMat;
                            break;
                        case 2:
                        case 3:
                        case 4:
                            //土
                            meshTemp = blockMesh;
                            maTemp = dirtMaterial;
                            break;
                        case 5:
                        case 6:
                            //石头
                            meshTemp = blockMesh;
                            maTemp = stoneMat;
                            break;
                        case 7:
                        case 8:
                            //鹅卵石
                            meshTemp = blockMesh;
                            maTemp = cobbleMat;
                            break;
                        default:
                            airChecker = true;
                            break;
                    }

                    if (!airChecker)
                    {
                        CreatePrefab(x, y, z ,meshTemp, maTemp, new BlockTag { });
                    }
                }
            }
        }
    }

    void TreeGenerator(int x, int y, int z)
    {
        for (int i = y; i < y + 7; i++)
        {
            //躯干部分          
            if (i == y + 6)
            {
                //树顶
                maTemp = leavesMat;
            }
            else
            {
                maTemp = woodMat;
            }

            CreatePrefab(x, i, z, blockMesh, maTemp, new BlockTag { });

            //树叶 就是个正方形
            if (i >= y + 3 && i <= y + 6)
            {
                for (int j = x - 1; j <= x + 1; j++)
                {
                    for (int k = z - 1; k <= z + 1; k++)
                    {
                        //不能随机到躯干
                        if (j != x || k != z)
                        {
                            CreatePrefab(j, i, k, blockMesh, leavesMat, new BlockTag { });
                        }

                    }
                }
            }
        }
    }

    void PlantGenerator(int x, int y, int z, int plantType)
    {
        switch (plantType)
        {
            case 1:
                maTemp = tallGrassMat;
                break;
            default:
                maTemp = roseMat;
                break;

        }

        CreatePrefab(x, y, z, tallGrassMesh, maTemp, new SurfacePlantTag { },false,(entities) => { manager.AddComponentData(entities, new Rotation { Value = Quaternion.Euler(0, 45, 0) }); });
    }

    void CloudGenerator(int x, int y, int z)
    {
        random = UnityEngine.Random.Range(4, 7);

        //提升y的高度 产生一个方形的云
        for (int i = 0; i < random; i++)
        {
            for (int j = 0; j < random; j++)
            {
                CreatePrefab(x + i, y + 15, z + j, blockMesh, CloudMat,new BlockTag { },false);
            }
        }
    }

    delegate void CreateFunc(Entity entities);

    void CreatePrefab<T>(int x, int y, int z, Mesh mesh, Material ma,T componentData ,bool isCollider=true,CreateFunc func = null)where T :struct,IComponentData
    {
        if(isCollider)
        AddCollider(new Vector3(x, y, z));

        Entity entities = manager.CreateEntity(blockArchetype);
        manager.SetComponentData(entities, new Position { Value = new int3(x, y, z) });
        manager.AddComponentData(entities, componentData);

        //找不到是粉色方块
        if (!maTemp)
            maTemp = pinkMat;

        func?.Invoke(entities);
        manager.AddSharedComponentData(entities, new MeshInstanceRenderer
        {
            mesh = mesh,
            material = ma,
        });
    }

    //在相同位置生成一个1x1x1的正方形碰撞
    void AddCollider(Vector3 vec3)
    {
        if (createCollider)
        {
            colPool.AddCollider(vec3);
        }
    }


}