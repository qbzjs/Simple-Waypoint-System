using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

#if UNITY_EDITOR
[ConverterVersion("joe", 1)]
public class RotationSpeedFromBuildSettings_IJobChunk : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //获取BuildConfiguration配置
        var rotationSpeedSetting = conversionSystem.GetBuildConfigurationComponent<RotationSpeedSetting>();

        // Change rotation speed
        var data = new RotationSpeed_IJobEntityBatch { RadiansPerSecond = math.radians(rotationSpeedSetting.RotationSpeed) };
        dstManager.AddComponentData(entity, data);

        // Offset the translation of the generated object
        var translation = dstManager.GetComponentData<Translation>(entity);
        translation.Value.y += rotationSpeedSetting.Offset;
        dstManager.SetComponentData(entity, translation);
    }
}
#endif
