using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Doc.CodeSamples.Tests
{
    #region singleton-type-example

    public struct Singlet : IComponentData
    {
        public int Value;
    }
    #endregion
    //Types used in examples below
    public struct Melee : IComponentData {}
    public struct Ranger : IComponentData {}
    public struct Player : IComponentData {}
    public struct Position : IComponentData { public float3 Value; }
    public struct RotationQuaternion : IComponentData { public quaternion Value; }
    public struct Displacement : IComponentData { public float3 Value; }

    public partial class EntityQueryExamples : SystemBase
    {
        void queryFromList()
        {
            #region query-from-list

            EntityQuery query = GetEntityQuery(typeof(Rotation),
                ComponentType.ReadOnly<RotationSpeed>());
            #endregion
        }

        void queryFromDescription()
        {
            #region query-from-description

            EntityQueryDesc description = new EntityQueryDesc
            {
                None = new ComponentType[]
                {
                    typeof(Static)
                },
                All = new ComponentType[]
                {
                    typeof(Rotation),
                    ComponentType.ReadOnly<RotationSpeed>()
                }
            };
            EntityQuery query = GetEntityQuery(description);

            #endregion
        }

        protected override void OnCreate()
        {
            #region query-description

            EntityQueryDesc description = new EntityQueryDesc
            {
                Any = new ComponentType[] { typeof(Melee), typeof(Ranger) },
                None = new ComponentType[] { typeof(Player) },
                All = new ComponentType[] { typeof(Position), typeof(Rotation) }
            };

            #endregion
            var query = GetEntityQuery(description);
            Entity entity = Entity.Null;
            #region entity-query-mask
            
            //查询某个Entity是否跟Query匹配
            var mask = EntityManager.GetEntityQueryMask(query);
            bool doesMatch = mask.Matches(entity);

            #endregion
        }

        protected override void OnUpdate()
        {
            var queryForSingleton = EntityManager.CreateEntityQuery(typeof(Singlet));
            var entityManager = EntityManager;
            #region create-singleton

            Entity singletonEntity = entityManager.CreateEntity(typeof(Singlet));
            entityManager.SetComponentData(singletonEntity, new Singlet { Value = 1 });

            #endregion


            #region set-singleton

            queryForSingleton.SetSingleton<Singlet>(new Singlet {Value = 1});
            #endregion
        }

        void ManualExamples1()
        {
            {
            #region define-query
            
            //表示该system不写入RotationSpeed。尽可能始终指定只读，
            //因为对数据的读取访问限制较少，这可以帮助job scheduler程序更有效地执行作job
            EntityQuery query
                = GetEntityQuery(typeof(RotationQuaternion),
                                 ComponentType.ReadOnly<RotationSpeed>());
            #endregion
            }
            {
                #region query-desc
                //使用EntityQueryDesc对象创建EntityQuery
                var queryDescription = new EntityQueryDesc
                {
                    None = new ComponentType[] { typeof(Static) },
                    All = new ComponentType[]{ typeof(RotationQuaternion),
                                           ComponentType.ReadOnly<RotationSpeed>() }
                };
                EntityQuery query = GetEntityQuery(queryDescription);
                #endregion
            }
            {
                #region combine-query

                var desc1 = new EntityQueryDesc
                {
                    All = new ComponentType[] { typeof(RotationQuaternion) }
                };

                var desc2 = new EntityQueryDesc
                {
                    All = new ComponentType[] { typeof(RotationSpeed) }
                };

                EntityQuery query
                    = GetEntityQuery(new EntityQueryDesc[] { desc1, desc2 });

                #endregion
            }
            {
                EntityManager entityManager = World.EntityManager;
                #region create-query

                EntityQuery query =
                    entityManager.CreateEntityQuery(typeof(RotationQuaternion),
                                        ComponentType.ReadOnly<RotationSpeed>());
                #endregion
            }


        }
    }
    #region query-writegroup

    public struct C1 : IComponentData { }

    [WriteGroup(typeof(C1))]
    public struct C2 : IComponentData { }

    [WriteGroup(typeof(C1))]
    public struct C3 : IComponentData { }

    public class ECSSystem : SystemBase
    {
        private EntityQuery query;
        protected override void OnCreate()
        {
            var queryDescription = new EntityQueryDesc
            {
                All = new ComponentType[] { ComponentType.ReadWrite<C1>(),
                                            ComponentType.ReadOnly<C3>() },
                Options = EntityQueryOptions.FilterWriteGroup
            };
            query = GetEntityQuery(queryDescription);

            //添加Entity测试
//            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
//            entityManager.CreateEntity(ComponentType.ReadWrite<C1>(), ComponentType.ReadWrite<C3>());
//            entityManager.CreateEntity(ComponentType.ReadWrite<C3>());
            
        }
        
//        struct EntityQueryJob1111 : IJobForEachWithEntity<C3>
//        {
//            public int frameCount;
//
//
//            public void Execute(Entity entity, int index, ref C3 c0)
//            {
//                Debug.Log($"EntityQueryJob1111===={frameCount}");
//            }
//        }

        protected override void OnUpdate()
        {
//            var job = new EntityQueryJob1111();
//            job.frameCount = UnityEngine.Time.frameCount;
//            job.Schedule(query, this.Dependency);
        }
    }
    #endregion

    #region get-query

    public partial class RotationSpeedSys : SystemBase
    {
        private EntityQuery query;

        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            //WithStoreEntityQueryInField 此方法存储为此实体生成的EntityQuery
            Entities
                .WithStoreEntityQueryInField(ref query)
                .ForEach(
                (ref RotationQuaternion rotation, in RotationSpeed speed) => {
                    rotation.Value
                        = math.mul(
                            math.normalize(rotation.Value),
                                quaternion.AxisAngle(math.up(),
                                    speed.RadiansPerSecond * deltaTime)
                         );
                })
                .Schedule();
        }
    }
    #endregion

    #region get-query-ijobchunk

    public class RotationSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            query = GetEntityQuery(typeof(RotationQuaternion),
                   ComponentType.ReadOnly<RotationSpeed>());
        }

        protected override void OnUpdate()
        {
            throw new NotImplementedException();
        }
    }
    #endregion
    #region shared-component-filter

    struct SharedGrouping : ISharedComponentData
    {
        public int Group;
    }

    class ImpulseSystem : SystemBase
    {
        EntityQuery query;

        protected override void OnCreate()
        {
            query = GetEntityQuery(typeof(Position),
                typeof(Displacement),
                typeof(SharedGrouping));
        }

        protected override void OnUpdate()
        {
            // Only iterate over entities that have the SharedGrouping data set to 1
            // 根据shareComponent的值来筛选
            query.SetSharedComponentFilter(new SharedGrouping { Group = 1 });
            
            //query.ToComponentDataArray  ==> 通过query直接拿到筛序到chunk数据
            var positions = query.ToComponentDataArray<Position>(Allocator.Temp);
            var displacements = query.ToComponentDataArray<Displacement>(Allocator.Temp);
            //这些NativeContainer 为什么不用Dispose 是因为？？ Allocator.Temp
            for (int i = 0; i < positions.Length; i++)
                positions[i] =
                    new Position
                    {
                        Value = positions[i].Value + displacements[i].Value
                    };
        }
    }

    #endregion
    class UpdateSystem : SystemBase
    {
        #region change-filter

        EntityQuery query;

        protected override void OnCreate()
        {
            query = GetEntityQuery(typeof(LocalToWorld),
                    ComponentType.ReadOnly<Translation>());
            query.SetChangedVersionFilter(typeof(Translation));

        }
        #endregion

        protected override void OnUpdate()
        {
            //通过query筛选出所有满足的entity
            //这个会阻塞主线程
            //This version of the function blocks until the Job used to fill the array is complete
            query.ToEntityArray(Allocator.Temp);
            
            //通过query筛选出所有满足的entity对应的Compoent数据
            query.ToComponentDataArray<Translation>(Allocator.Temp);
        }
    }
}
