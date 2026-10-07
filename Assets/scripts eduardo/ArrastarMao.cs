using UnityEngine;

public class ArrastarMao : MonoBehaviour
{
    private Camera cam;
    private bool chegouNaPia = false;

    public SistemaHigiene sistemaHigiene;

    private void Start()
    {
        cam = Camera.main;
    }

    private void OnMouseDrag()
    {
        if (chegouNaPia)
            return;

        Vector3 posicao = cam.ScreenToWorldPoint(Input.mousePosition);
        posicao.z = transform.position.z;

        transform.position = posicao;
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.CompareTag("AreaMaos"))
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

            // Avisa o sistema que uma mão chegou
            sistemaHigiene.MaoChegou();
        }
    }
}