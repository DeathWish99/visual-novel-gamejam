using Fungus;
using UnityEngine;
using VisualNovel.GameJam.Manager;

[CommandInfo("Custom", "Audio Event", "Audio Player")]
[AddComponentMenu("")]
public class AudioPlayCommad : Command
{
    [SerializeField] protected AudioType audioType;
    [SerializeField] protected string audioID;
    [SerializeField] protected bool stopBGM;

    public override void OnEnter()
    {
        if (stopBGM)
        {
            AudioManager.Instance.PauseBGM();
        }
        else
        {
            if (audioType == AudioType.BGM)
            {
                AudioManager.Instance.PlayBGM(audioID);
            }
            else
            {
                AudioManager.Instance.PlaySFX(audioID);
            }
        }

        Continue();
    }

    public enum AudioType { BGM, SFX }
}
