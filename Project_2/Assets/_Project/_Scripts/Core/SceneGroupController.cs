using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace _Project._Scripts.SceneManagement
{
    public class SceneGroupController
    {
        public event Action<string> OnSceneLoaded = delegate { };
        public event Action<string> OnSceneUnLoaded = delegate { };
        public event Action OnSceneGroupLoaded = delegate { };

        SceneGroup _ActiveSceneGroup;

        public async Task LoadScenes(SceneGroup group, IProgress<float> progress, bool reloadDupScenes = false)
        {
            _ActiveSceneGroup = group;
            var loadedScenes = new List<string>();

            await UnloadScenes();

            int sceneCount = SceneManager.sceneCount;

            for(int i = 0; i < sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }

            var totalScenesToLoad = _ActiveSceneGroup._Scenes.Count;
            var operationGroup = new AsyncOperationGroup(totalScenesToLoad);

            for(int i = 0; i < totalScenesToLoad; i++)
            {
                var sceneData = group._Scenes[i];
                if(reloadDupScenes == false && loadedScenes.Contains(sceneData._Name)) continue;

                var operation = SceneManager.LoadSceneAsync(sceneData._Reference, LoadSceneMode.Additive);

                operationGroup.Operations.Add(operation);

                OnSceneLoaded.Invoke(sceneData._Name);
            }

            while(!operationGroup.IsDone)
            {
                progress?.Report(operationGroup.Progress);
                await Task.Delay(100);
            }

            Scene activeScene = SceneManager.GetSceneByName(_ActiveSceneGroup.FindSceneNameByType(SceneType.ActiveScene));

            if(activeScene.IsValid())
            {
                SceneManager.SetActiveScene(activeScene);
            }

            OnSceneGroupLoaded.Invoke();
        }

        public async Task UnloadScenes()
        {
            var scenes = new List<string>();
            var activeScene = SceneManager.GetActiveScene().name;

            int sceneCount = SceneManager.sceneCount;

            for(int i = sceneCount - 1; i > 0; i--)
            {
                var sceneAt = SceneManager.GetSceneAt(i);
                if(!sceneAt.isLoaded) continue;

                var sceneName = sceneAt.name;
                if(sceneName.Equals(activeScene) || sceneName == "Bootstrapper") continue;

                scenes.Add(sceneName);
            }

            var operationGroup = new AsyncOperationGroup(scenes.Count);
            
            foreach(var scene in scenes)
            {
                var operation = SceneManager.UnloadSceneAsync(scene);
                if(operation == null) continue;

                operationGroup.Operations.Add(operation);

                OnSceneUnLoaded.Invoke(scene);
            }

            while(!operationGroup.IsDone)
            {
                await Task.Delay(100);
            }

            await Resources.UnloadUnusedAssets();
        }
    }

    public readonly struct AsyncOperationGroup
    {
        public readonly List<AsyncOperation> Operations;

        public float Progress => Operations.Count == 0 ? 0 : Operations.Average(o => o.progress);
        public bool IsDone => Operations.All(o => o.isDone);

        public AsyncOperationGroup(int initialCapacity)
        {
            Operations = new List<AsyncOperation>(initialCapacity);
        }
    }
}
