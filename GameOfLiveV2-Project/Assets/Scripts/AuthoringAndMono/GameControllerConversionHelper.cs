using Unity.Entities;
using UnityEngine;

namespace TMG.GameOfLiveV2
{
    public class GameControllerConversionHelper : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            GameOfLifeMonoController.Instance.GameControllerEntity = entity;
        }
    }
}