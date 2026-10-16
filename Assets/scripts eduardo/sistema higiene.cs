
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SistemaHigiene : MonoBehaviour
{
    public Slider barraHigiene;
    public GameObject agua;
    public EtapaEnxague etapaEnxague;
    public LegendaHigiene legendaHigiene;

    public float intervalo = 0.08f;

    private int maosNaPia = 0;
    private float contador = 0f;
    private bool aguaLigada = false;
    private bool lavagemConcluida = false;

    private void Start()
    {
        barraHigiene.gameObject.SetActive(true);
        etapaEnxague.barraEnxague.gameObject.SetActive(false);

        legendaHigiene.MudarLegenda(
            "Coloque as duas mãos na pia."
        );
    }

    private void Update()
    {
        if (contador > 0)
        {
            contador -= Time.deltaTime;
        }

        if (maosNaPia >= 2 && aguaLigada && !lavagemConcluida)
        {
            if (Keyboard.current != null &&
                Keyboard.current.spaceKey.wasPressedThisFrame &&
                contador <= 0)
            {
                barraHigiene.value += 1f;
                contador = intervalo;

                if (barraHigiene.value >= 100)
                {
                    barraHigiene.value = 100;
                    lavagemConcluida = true;

                    barraHigiene.gameObject.SetActive(false);

                    aguaLigada = false;
                    agua.SetActive(false);

                    legendaHigiene.MudarLegenda(
                        "Pise no pedal para iniciar o enxágue."
                    );

                    Debug.Log("Lavagem concluída!");
                }
            }
        }
    }

   
    public bool MaosEstaoNaPia()
    {
        return maosNaPia >= 2;
    }

    public void DefinirAgua(bool estado)
    {
        aguaLigada = estado;

        if (!lavagemConcluida)
        {
            if (estado)
            {
                legendaHigiene.MudarLegenda(
                    "Pressione ESPAÇO repetidamente para ensaboar e lavar as mãos."
                );
            }
            else
            {
                legendaHigiene.MudarLegenda(
                    "Pise no pedal para abrir a torneira."
                );
            }
        }

        Debug.Log("Água ligada: " + aguaLigada);
    }

    public bool LavagemEstaConcluida()
    {
        return lavagemConcluida;
    }


    public void MaoChegou()
    {
        maosNaPia++;

        if (maosNaPia > 2)
        {
            maosNaPia = 2;
        }

        Debug.Log("Mãos na pia: " + maosNaPia);

        if (maosNaPia >= 2 && !lavagemConcluida)
        {
            legendaHigiene.MudarLegenda(
                "Pise no pedal para abrir a torneira."
            );
        }
    }

    public void LiberarMaosParaSecagem()
    {
        ArrastarMao[] maos =
            FindObjectsByType<ArrastarMao>(FindObjectsSortMode.None);

        foreach (ArrastarMao mao in maos)
        {
            mao.LiberarParaSecagem();
        }

        Debug.Log("Mãos liberadas para secagem!");
    }


}
