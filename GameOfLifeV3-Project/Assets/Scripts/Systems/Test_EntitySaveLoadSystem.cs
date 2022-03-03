using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Scenes;
using UnityEngine;

namespace TMG.GameOfLifeV3
{
    // ReSharper disable once InconsistentNaming
    [UpdateInGroup(typeof(PresentationSystemGroup), OrderLast = true)]
    public class Test_EntitySaveLoadSystem : SystemBase
    {
        private ReferencedUnityObjects _referencedUnityObjects;
        private StandardSaveData _stdSaveData;
        private HybridSaveData _hybridSaveData;
        //private object[] _standardObjects;
        private string _jsonData;
        private EntityQuery _allSaveEntities;
        private NativeArray<EntityRemapUtility.EntityRemapInfo> _entityRemapInfo;
        
        protected override void OnCreate()
        {
            _referencedUnityObjects = ScriptableObject.CreateInstance<ReferencedUnityObjects>();
            _stdSaveData = new StandardSaveData();
            _hybridSaveData = new HybridSaveData();
            //_standardObjects = new object[0];
            var entityQueryDesc = new EntityQueryDesc
            {
                None = new ComponentType[] {typeof(RequestSceneLoaded)}
            };
            _allSaveEntities = EntityManager.CreateEntityQuery(entityQueryDesc);
        }

        protected override void OnUpdate()
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Clear(1);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Clear(2);
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    Clear(3);
                }
            }

            else if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Save(1);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Save(2);
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    Save(3);
                }
            }

            else if (Input.GetKey(KeyCode.LeftAlt))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Load(1);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Load(2);
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    Load(3);
                }
            }
        }

        private void Save(int saveSlot)
        {
            Debug.Log($"Player saving to slot {saveSlot}");
            StandardSave(saveSlot);
            //HybridSave(saveSlot);
        }

        private void StandardSave(int saveSlot)
        {
            using var saveWorld = new World("SaveWorld", WorldFlags.Staging);
            using var entitiesToSave = _allSaveEntities.ToEntityArray(Allocator.Temp);
            _entityRemapInfo =
                new NativeArray<EntityRemapUtility.EntityRemapInfo>(EntityManager.EntityCapacity,
                    Allocator.Persistent);
            saveWorld.EntityManager.CopyEntitiesFrom(EntityManager, entitiesToSave, entitiesToSave);
            using (var writer = new StreamBinaryWriter($"{Application.dataPath}/TestSaveData{saveSlot}"))
            {
                SerializeUtility.SerializeWorld(saveWorld.EntityManager, writer, out var standardObjects, _entityRemapInfo);
                
                _stdSaveData.Array = standardObjects;
            }
            
            var jsonString = JsonUtility.ToJson(_stdSaveData);
            PlayerPrefs.SetString($"TGOL-TestSave{saveSlot}", jsonString);
        }
        
        /*private void HybridSave(int saveSlot)
        {
            using (var writer = new StreamBinaryWriter($"{Application.dataPath}/TestSaveData{saveSlot}"))
            {
                SerializeUtilityHybrid.Serialize(EntityManager, writer, out _referencedUnityObjects);
            }

            _hybridSaveData.Array = _referencedUnityObjects.Array;
            for (var i = 0; i < _hybridSaveData.Array.Length; i++)
            {
                Debug.Log($"Object at {i} is {_hybridSaveData.Array[i].ToString()}");
            }

            _jsonData = JsonUtility.ToJson(_standardObjects);
            Debug.Log(_jsonData);
        }*/

        private void Load(int saveSlot)
        {
            Debug.Log($"Player loading from slot {saveSlot}");

            StandardLoad(saveSlot);
        }

        private void StandardLoad(int saveSlot)
        {
            var jsonString = PlayerPrefs.GetString($"TGOL-TestSave{saveSlot}");
            var jsonData = JsonUtility.FromJson<StandardSaveData>(jsonString);
            
            EntityManager.DestroyEntity(_allSaveEntities);
            using var loadWorld = new World("LoadWorld", WorldFlags.Staging);
            var loadEman = loadWorld.EntityManager;
            var eeTrans = loadEman.BeginExclusiveEntityTransaction();
            using (var reader = new StreamBinaryReader($"{Application.dataPath}/TestSaveData{saveSlot}"))
            {
                SerializeUtility.DeserializeWorld(eeTrans, reader, jsonData.Array);
            }
            loadEman.EndExclusiveEntityTransaction();
            EntityManager.CopyEntitiesFrom(loadEman, loadEman.GetAllEntities());
            //EntityManager.MoveEntitiesFrom(loadEman, _entityRemapInfo);
            World.DefaultGameObjectInjectionWorld.GetExistingSystem<ChangeCellsSystem>().AfterReloadOps();
            World.DefaultGameObjectInjectionWorld.GetExistingSystem<SpawnGridSystem>().ResetBlobAsset();
        }
        
        private void HybridLoad(int saveSlot)
        {
            var loadWorld = new World("LoadWorld", WorldFlags.Staging);
            var loadEman = loadWorld.EntityManager;
            //var eetrans = new ExclusiveEntityTransaction();

            using (var reader = new StreamBinaryReader($"{Application.dataPath}/TestSaveData{saveSlot}"))
            {
                //SerializeUtility.DeserializeWorld(eetrans, reader, _objects);
                SerializeUtilityHybrid.Deserialize(loadEman, reader, _referencedUnityObjects);
            }

            EntityManager.MoveEntitiesFrom(loadEman);
            //EntityManager.MoveEntitiesFrom(eetrans.EntityManager);
            //loadWorld.EntityManager.EndExclusiveEntityTransaction();
        }

        private void Clear(int saveSlot)
        {
            Debug.Log($"Player clearing slot {saveSlot}");
        }

        protected override void OnDestroy()
        {
            _entityRemapInfo.Dispose();
        }
    }

    public class StandardSaveData
    {
        public object[] Array;
    }
    
    public class HybridSaveData
    {
        public Object[] Array;
    }
}