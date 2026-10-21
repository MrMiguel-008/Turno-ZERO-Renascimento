
using UnityEngine;
using TMPro;

public class LegendaHigiene : MonoBehaviour
{
    public TMP_Text textoLegenda;

    private void Awake()
    {
        EsconderLegenda();
    }

    public void MudarLegenda(string mensagem)
    {
        if (textoLegenda == null)
            return;

        textoLegenda.text = mensagem;
        MostrarLegenda();
    }

    public void MostrarLegenda()
    {
        if (textoLegenda != null)
            textoLegenda.gameObject.SetActive(true);
    }

    public void EsconderLegenda()
    {
        if (textoLegenda != null)
            textoLegenda.gameObject.SetActive(false);
    }
}
