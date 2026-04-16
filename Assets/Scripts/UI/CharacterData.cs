using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Character")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite portrait;
    public RuntimeAnimatorController animatorController;
    public AudioClip voiceSound;
    public float voicePitch = 1f;
}
