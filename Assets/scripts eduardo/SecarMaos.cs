

using UnityEngine;
using UnityEngine.UI;

public class SecarMaos : MonoBehaviour
{
    public Slider barraSecagem;
    public AreaSecagemMaos areaSecagem;
    public LegendaHigiene legendaHigiene;

    public float velocidadeSecagem = 10f;

    private bool arrastando = false;
    private bool papelNaArea = false;
    private bool secagemConcluida = false;
    private bool papelDescartado = false;
    private bool legendaSecagemMostrada = false;

    private void Start()
    {
        barraSecagem.value = 0;
        barraSecagem.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (papelDescartado)
            return;

        arrastando = true;
    }

    private void OnMouseUp()
    {
        arrastando = false;
    }

    private void Update()
    {
        if (papelDescartado || !arrastando || secagemConcluida)
            return;

        if (!areaSecagem.DuasMaosNaArea() || !papelNaArea)
            return;

        if (!legendaSecagemMostrada)
        {
            legendaHigiene.MudarLegenda(
                "Esfregue o papel-toalha nas mãos para secá-las."
            );

            legendaSecagemMostrada = true;
        }

        if (!barraSecagem.gameObject.activeSelf)
            barraSecagem.gameObject.SetActive(true);

        barraSecagem.value += velocidadeSecagem * Time.deltaTime;

        if (barraSecagem.value >= 100)
        {
            barraSecagem.value = 100;
            secagemConcluida = true;
            arrastando = false;

            barraSecagem.gameObject.SetActive(false);

            legendaHigiene.MudarLegenda(
                "Leve o papel usado até a lixeira."
            );

            Debug.Log("Mãos secas! Agora descarte o papel no lixo.");
        }
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.CompareTag("AreaSecagem"))
        {
            papelNaArea = true;
        }

        if (outro.CompareTag("AreaLixo") && secagemConcluida)
        {
            papelDescartado = true;
            gameObject.SetActive(false);

            legendaHigiene.MudarLegenda(
                "Higienização concluída!"
            );

            Debug.Log("Papel descartado no lixo!");
        }
    }

    private void OnTriggerExit2D(Collider2D outro)
    {
        if (outro.CompareTag("AreaSecagem"))
        {
            papelNaArea = false;
        }
    }

    public void MostrarLegendaSecagem()
    {
        if (!secagemConcluida && !papelDescartado)
        {
            legendaHigiene.MudarLegenda(
                "Arraste o papel-toalha sobre as mãos para secá-las."
            );
        }
    }

}
