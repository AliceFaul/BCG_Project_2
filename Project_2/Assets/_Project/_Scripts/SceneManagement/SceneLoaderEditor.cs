using UnityEngine;
using UnityEditor;

namespace _Project._Scripts.SceneManagement.Editor
{
    [CustomEditor(typeof(SceneLoader))]
    public class SceneLoaderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI ()
        {
            DrawDefaultInspector();

            SceneLoader sceneLoader = (SceneLoader) target;

            if(EditorApplication.isPlaying && GUILayout.Button("Load Earth Biome"))
            {
                LoadSceneGroup(sceneLoader, 0);
            }

            if (EditorApplication.isPlaying && GUILayout.Button("Load Frost Biome"))
            {
                LoadSceneGroup(sceneLoader, 1);
            }

            if (EditorApplication.isPlaying && GUILayout.Button("Load Lava Biome"))
            {
                LoadSceneGroup(sceneLoader, 2);
            }
        }

        static async void LoadSceneGroup(SceneLoader sceneLoader, int index) => await sceneLoader.LoadingSceneGroup(index);
    }
}
