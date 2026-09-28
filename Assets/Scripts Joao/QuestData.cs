using UnityEngine;

public enum QuestType
{
    EnterArea,
    TalkToNPC,
    InteractWithItem,
    ChangeScene
}

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest System/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Identificacao")]
    public int questIndexID; // Número único da missão (ex: 0, 1, 2...)
    public string questTitle;
    [TextArea] public string questDescription;

    [Header("Configuracao do Objetivo")]
    public QuestType questType;

    [Tooltip("Nome exato da Area, do NPC ou da Cena para este objetivo")]
    public string targetIdentifier;
}