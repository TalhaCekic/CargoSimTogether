using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DOTween k�t�phanesi i�in gerekli

public class Interact : MonoBehaviour
{
    public LayerMask interactableLayer; // Etkile�im yap�lacak layer
    public Transform targetTransform; // Hedef transform (�rne�in bir nokta)
    public float moveDuration = 2f; // Hareket s�resi

    private Rigidbody selectedRigidbody; // Se�ilen objenin Rigidbody'si

    private void Update()
    {
        if (Input.GetMouseButton(0)) // E tu�una bas�ld���nda
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Fare pozisyonundan bir ray olu�tur
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactableLayer)) // Raycast ile layer kontrol�
            {
                if (hit.rigidbody != null) // �arp�lan objede Rigidbody var m� kontrol et
                {
                    selectedRigidbody = hit.rigidbody; // �arp�lan objenin Rigidbody'sini se�
                    MoveObject(selectedRigidbody); // Objeyi hareket ettir
                }
            }
        }
        if (Input.GetMouseButton(1)) // E tu�una bas�ld���nda
        {
          
        }
    }

    private void MoveObject(Rigidbody rb)
    {
        // Rigidbody'yi hedef pozisyona do�ru hareket ettir
        rb.DOMove(targetTransform.position, moveDuration)
            .SetEase(Ease.InOutSine) // Daha smooth bir hareket i�in Ease ayar�
            .OnComplete(() => Debug.Log("Hareket tamamland�!")); // Hareket tamamland���nda bir i�lem yap
    }
}
