using UnityEngine;

public class ArrastarPapel : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void OnMouseDrag()
    {
        Vector3 posicao = cam.ScreenToWorldPoint(Input.mousePosition);

        posicao.z = transform.position.z;

        transform.position = posicao;
    }
}