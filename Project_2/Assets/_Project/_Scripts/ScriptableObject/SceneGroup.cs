using System;
using System.Collections.Generic;
using System.Linq;

namespace _Project._Scripts.SceneManagement
{
    [Serializable]
    public class SceneGroup
    {
        public string _GroupName = "New Scene Group";
        public List<SceneData> _Scenes;

        public string FindSceneNameByType(SceneType sceneType)
        {
            return _Scenes.FirstOrDefault(scene => scene._SceneType == sceneType)?._Reference.SceneName;
        }
    }

    [Serializable]
    public class SceneData
    {
        public SceneField _Reference;
        public string _Name => _Reference.SceneName;
        public SceneType _SceneType;
    }

    public enum SceneType { ActiveScene, MainMenu, UserInterface, HUD, Cinematic, Environment, Tooling }
}
