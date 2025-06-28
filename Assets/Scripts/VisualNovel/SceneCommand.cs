using Fungus;
using UnityEngine;

[CommandInfo("Custom", "Scene Event", "Scene Change")]
[AddComponentMenu("")]
public class SceneCommand : Command
{
    public override void OnEnter()
    {
        GameManager.Instance.BackHome();
        Continue();
    }
}
