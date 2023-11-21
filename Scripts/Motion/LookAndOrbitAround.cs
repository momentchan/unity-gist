using UnityEngine;

public class LookAndOrbitAround : MonoBehaviour
{
    [SerializeField] protected Transform target;
    public float radius = 40f;
    public float speed = 1f;
    public Vector3 veclocity = new Vector3(1, 0.5f, 1);

    protected float time;
    void Update()
    {
        time += Time.deltaTime * speed;

        transform.position = target.transform.position + new Vector3(radius * Mathf.Cos(time * veclocity.x),
                                                                     radius * Mathf.Sin(time * veclocity.y),
                                                                     radius * Mathf.Sin(time * veclocity.z));

        if (target != null)
            transform.LookAt(target);
    }
}