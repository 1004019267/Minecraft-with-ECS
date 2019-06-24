using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Collections;

public class EntitiesTest : MonoBehaviour
{
    //宣告内存字段放在一起 而不是杂乱无序的 提高速率
    public static EntityArchetype blockArchetype;

    public EntityManager manager;
    public Mesh blockMesh;
    public Material blockMaterial;

    public GameObject go;

    //宣告在加载场景之前运行 可以理解为预存
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        //宣告管理器
        EntityManager manager = World.Active.GetOrCreateManager<EntityManager>();
        //宣告内存区块(区域)
        blockArchetype = manager.CreateArchetype(
            typeof(Position)
            );
    }

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    void Start()
    {
        //Unity自己产生预制体 但是要添加GameObjectEntity Entities才能标记
        //或者 Instantiate   
        GameObject.CreatePrimitive(PrimitiveType.Cube)
            .AddComponent<GameObjectEntity>()
            .transform.position = new Vector3(-2, 0, 0);

        //PureECS 下面是不用Unity自带组件创建预制体 就是说脱离Unity也可以用的代码
        manager = World.Active.GetOrCreateManager<EntityManager>();

        //在内存块中设置Pos和创建一个tag(并没有用)
        Entity entities = manager.CreateEntity(blockArchetype);
        manager.SetComponentData(entities, new Position { Value = new int3(2, 0, 0) });
        //这个是自定义的我们没有..
        //manager.AddComponentData(entities, new BlockTag());

        //添加材质
        manager.AddSharedComponentData(entities, new MeshInstanceRenderer
        {
            mesh = blockMesh,
            material = blockMaterial,
        });

        //Hybrid ECS 引用unity自带GameObject创建预制体
        if (go)
        {
            //产生一个新的阵列
            using (NativeArray<Entity> entityArray = new NativeArray<Entity>(1, Allocator.Temp))
            {
                manager.Instantiate(go, entityArray);
                manager.SetComponentData(entities, new Position { Value = new float3(4, 0f, 0f) });
            }
        }

    }
}
