using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DOTween kütüphanesi için gerekli

public class Interact : MonoBehaviour
{
    public LayerMask interactableLayer; // Etkileþim yapýlacak layer
    public Transform targetTransform; // Hedef transform (örneðin bir nokta)
    public float moveDuration = 2f; // Hareket süresi

    private Rigidbody selectedRigidbody; // Seçilen objenin Rigidbody'si

    private void Update()
    {
        if (Input.GetMouseButton(0)) // E tuþuna basýldýðýnda
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Fare pozisyonundan bir ray oluþtur
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactableLayer)) // Raycast ile layer kontrolü
            {
                if (hit.rigidbody != null) // Çarpýlan objede Rigidbody var mý kontrol et
                {
                    selectedRigidbody = hit.rigidbody; // Çarpýlan objenin Rigidbody'sini seç
                    MoveObject(selectedRigidbody); // Objeyi hareket ettir
                }
            }
        }
        if (Input.GetMouseButton(1)) // E tuþuna basýldýðýnda
        {
          
        }
    }

    private void MoveObject(Rigidbody rb)
    {
        // Rigidbody'yi hedef pozisyona doðru hareket ettir
        rb.DOMove(targetTransform.position, moveDuration)
            .SetEase(Ease.InOutSine) // Daha smooth bir hareket için Ease ayarý
            .OnComplete(() => Debug.Log("Hareket tamamlandý!")); // Hareket tamamlandýðýnda bir iþlem yap
    }
}
