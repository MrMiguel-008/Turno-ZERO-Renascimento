
using UnityEngine;
using UnityEngine.UI;

public class SecarMaos : MonoBehaviour
{
    public Slider barraSecagem;
    public AreaSecagemMaos areaSecagem;

    public float velocidadeSecagem = 10f;

    private bool arrastando = false;
    private bool papelNaArea = false;
    private bool secagemConcluida = false;
    private bool papelDescartado = false;

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
        if (papelDescartado || !arrastando)
            return;

        // Depois da secagem, o papel pode ir para o lixo.
        if (secagemConcluida)
            return;

        if (!areaSecagem.DuasMaosNaArea() || !papelNaArea)
            return;

        if (!barraSecagem.gameObject.activeSelf)
            barraSecagem.gameObject.SetActive(true);

        barraSecagem.value += velocidadeSecagem * Time.deltaTime;

        if (barraSecagem.value >= 100)
        {
            barraSecagem.value = 100;
            secagemConcluida = true;
            arrastando = false;

            barraSecagem.gameObject.SetActive(false);

            Debug.Log("Mãos secas! Agora descarte o papel no lixo.");
        }
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.CompareTag("AreaSecagem"))
        {
            papelNaArea = true;
        }

        // O papel só pode ser descartado após a secagem.
        if (outro.CompareTag("AreaLixo") && secagemConcluida)
        {
            papelDescartado = true;

            gameObject.SetActive(false);

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
}
