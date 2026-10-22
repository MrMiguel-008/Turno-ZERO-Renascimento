using UnityEngine;

public class GerenciadorProgressoSalvo : MonoBehaviour
{
    void Start()
    {
        // Assim que a cena do jogo carrega, recupera a missão ativa salva
        if (PlayerPrefs.HasKey("QuestAtivaID"))
        {
            string questSalva = PlayerPrefs.GetString("QuestAtivaID");

            // Procura o QuestManager na cena e devolve a missão que estava ativa
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.activeQuestID = questSalva;
            }
        }
    }

    void OnDisable()
    {
        // Assim que saíres da cena (por exemplo, ao apertar ESC para ir ao menu), 
        // este método guarda automaticamente qual era a missão ativa no momento
        if (QuestManager.Instance != null && !string.IsNullOrEmpty(QuestManager.Instance.activeQuestID))
        {
            PlayerPrefs.SetString("QuestAtivaID", QuestManager.Instance.activeQuestID);
            PlayerPrefs.Save();
        }
    }
}