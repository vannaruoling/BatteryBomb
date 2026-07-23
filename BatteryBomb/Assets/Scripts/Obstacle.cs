using UnityEngine;

public class Obstacle : MonoBehaviour
{

    public float minSize = 0.5f;
    public float maxSize = 2.0f;
    public float maxSpinSpeed = 10f;
    public float minSpeed = 100f;
    public float maxSpeed = 300f;

    public GameObject bounceEffectPrefab;
    Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Random circluar vector
        Vector2 randomDirection = Random.insideUnitCircle;

        rb = GetComponent<Rigidbody2D>();

        float randomSize = Random.Range(minSize, maxSize);
        float randomSpeed = Random.Range(minSpeed, maxSpeed) / randomSize; // 
        float randomTorque = Random.Range(-maxSpinSpeed, maxSpinSpeed);

        transform.localScale = new Vector3(randomSize, randomSize, 1);

        rb.AddForce(Vector2.right * randomSpeed);
        rb.AddForce(randomDirection * randomSpeed);
        rb.AddTorque(randomTorque);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 contactPoint = collision.GetContact(0).point;
        GameObject bounceEffect = Instantiate(bounceEffectPrefab, contactPoint, Quaternion.identity);

        // Destroy the effect after 1 second
        Destroy(bounceEffect, 1f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
