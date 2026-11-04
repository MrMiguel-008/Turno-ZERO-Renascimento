using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EtapaEnxague : MonoBehaviour
{
    public Slider barraEnxague;
    public SistemaHigiene sistemaHigiene;
    public float intervalo = 0.08f;
    [Tooltip("Quanto a barra sobe a cada toque em ESPAÇO. A barra vai de 0 a 100.")]
    public float ganhoPorToque = 4f;

    private float contador = 0f;

    private void Start()
    {
        if (barraEnxague == null)
        {
            Debug.LogError("[EtapaEnxague] O campo 'barraEnxague' não foi preenchido no Inspector.", this);
            return;
        }

        barraEnxague.value = 0f;
        barraEnxague.gameObject.SetActive(false);
    }

    public void PrepararBarra()
    {
        barraEnxague.value = 0f;
        barraEnxague.gameObject.SetActive(true);
    }

    public void IniciarEnxague()
    {
        if (sistemaHigiene.EtapaAtual == EtapaHigiene.IniciarEnxague)
        {
            sistemaHigiene.EnxagueIniciado();
        }
    }

    private void Update()
    {
        if (contador > 0f)
        {
            contador -= Time.deltaTime;
        }

        if (sistemaHigiene.EtapaAtual != EtapaHigiene.Enxaguar || contador > 0f)
        {
            return;
        }

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            contador = intervalo;
            barraEnxague.value += ganhoPorToque;
            SoundManager.Tocar(Som.Bolhas);

            if (barraEnxague.value >= 100f)
            {
                barraEnxague.value = 100f;
                barraEnxague.gameObject.SetActive(false);
                SoundManager.Tocar(Som.Acerto);
                sistemaHigiene.EnxagueConcluido();
            }
        }
    }
}
