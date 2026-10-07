using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SistemaHigiene : MonoBehaviour
{
    public Slider barraHigiene;
    public GameObject agua;
    public EtapaEnxague etapaEnxague;

    public float intervalo = 0.08f;

    private int maosNaPia = 0;
    private float contador = 0f;
    private bool aguaLigada = false;
    private bool lavagemConcluida = false;

    private void Start()
    {
        // A barra de higiene começa aparecendo
        barraHigiene.gameObject.SetActive(true);

        // A barra de enxágue começa escondida
        etapaEnxague.barraEnxague.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (contador > 0)
        {
            contador -= Time.deltaTime;
        }

        if (maosNaPia >= 2 && aguaLigada && !lavagemConcluida)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame && contador <= 0)
            {
                barraHigiene.value += 1f;
                contador = intervalo;

                if (barraHigiene.value >= 100)
                {
                    barraHigiene.value = 100;
                    lavagemConcluida = true;

                    // Esconde a barra de higiene
                    barraHigiene.gameObject.SetActive(false);

                    // Desliga a água
                    aguaLigada = false;
                    agua.SetActive(false);

                    Debug.Log("Lavagem concluída!");
                }
            }
        }
    }

    public void MaoChegou()
    {
        maosNaPia++;

        if (maosNaPia > 2)
        {
            maosNaPia = 2;
        }

        Debug.Log("Mãos na pia: " + maosNaPia);
    }

    public bool MaosEstaoNaPia()
    {
        return maosNaPia >= 2;
    }

    public void DefinirAgua(bool estado)
    {
        aguaLigada = estado;

        Debug.Log("Água ligada: " + aguaLigada);
    }

    public bool LavagemEstaConcluida()
    {
        return lavagemConcluida;
    }
}