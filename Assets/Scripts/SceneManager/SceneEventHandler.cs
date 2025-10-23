using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneEventHandler : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Map1")
        {
            List<string> lines = new List<string>
            {
                "Finally, I manage to get inside this dungeon",
                "The World Tree Heart that got stolen from us is somewhere inside this place",
                "The World Tree missing its heart will make everything fallen into chaos",
                "I need to hurry, the fate of the whole world depends on me"
            };
            PlayerDialogue.Instance.ShowDialogue(lines);
        }
    }
}
