using UnityEngine;

public class ArrastarMao : MonoBehaviour
{
    private Camera cam;
    private bool chegouNaPia = false;
    private bool podeArrastarNovamente = false;

    public SistemaHigiene sistemaHigiene;

    private void Start()
    {
        cam = Camera.main;
    }

    private void OnMouseDrag()
    {
        // Se chegou na pia, só pode sair quando a secagem for liberada
        if (chegouNaPia && !podeArrastarNovamente)
        {
            return;
        }

        Vector3 posicao = cam.ScreenToWorldPoint(Input.mousePosition);
        posicao.z = transform.position.z;

        transform.position = posicao;
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        // Chegou na área da pia
        if (outro.CompareTag("AreaMaos") && !podeArrastarNovamente)
        {
            chegouNaPia = true;

            Vector3 posicao = outro.transform.position;

            if (gameObject.name == "MaoEsquerda")
            {
                posicao.x -= 0.4f;
            }
            else if (gameObject.name == "MaoDireita")
            {
                posicao.x += 0.4f;
            }

            posicao.z = transform.position.z;

            transform.position = posicao;

            sistemaHigiene.MaoChegou();
        }
    }

    public void LiberarParaSecagem()
    {
        podeArrastarNovamente = true;
        chegouNaPia = false;
    }
}