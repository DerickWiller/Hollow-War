using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("O objeto que a câmera deve seguir. Arraste o Player aqui.")]
    public Transform target;

    [Header("Settings")]
    [Tooltip("Quão suavemente a câmera segue o alvo. Valores menores são mais lentos e suaves.")]
    [Range(0.01f, 1.0f)]
    public float smoothSpeed = 0.125f;

    [Tooltip("O deslocamento da câmera em relação ao alvo (X, Y, Z).")]
    public Vector3 offset;

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        // --- A CORREÇÃO ESTÁ AQUI ---
        // Nós calculamos a posição suave para X e Y, mas ignoramos o Z calculado.
        // Em vez disso, forçamos o Z da câmera a ser sempre o Z original dela mesma,
        // garantindo que ela nunca se aproxime dos sprites.
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}