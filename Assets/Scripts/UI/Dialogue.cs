using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    public CharacterData characterData;
    public string[] dialogueLines;
    public float autoProgressDelay = 1.5f;
    public float typingSpeed = 0.05f;
}
