
using UnityEngine;
using TMPro;

public class LegendaHigiene : MonoBehaviour
{
    public TMP_Text textoLegenda;

    public void MudarLegenda(string mensagem)
    {
        textoLegenda.text = mensagem;
    }
}
