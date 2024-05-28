using UnityEngine;

public class FollowImage : MonoBehaviour
{
    public Transform target; // Takip edilecek ana obje (Image)
    private Vector3 offset; // Başlangıçtaki ofset (konum farkı)

    void Start()
    {
        if (target != null)
        {
            // Başlangıçtaki ofseti hesapla
            offset = transform.position - target.position;
        }
    }

    void Update()
    {
        if (target != null)
        {
            // Ana objenin pozisyonuna göre ofseti koruyarak yeni pozisyonu hesapla
            transform.position = target.position + offset;
        }
    }
}

