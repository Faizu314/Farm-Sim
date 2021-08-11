using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] [Range(1f, 30f)] private float lerpSpeed;

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, player.position + Vector3.up, lerpSpeed * Time.deltaTime);
    }
}
