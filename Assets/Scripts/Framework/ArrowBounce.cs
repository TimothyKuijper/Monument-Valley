using UnityEngine;

public class ArrowBounce : MonoBehaviour
{
    void FixedUpdate()
    {
        gameObject.transform.Rotate(new Vector3(0,0.7f,0));
        var startPos = transform.position;
        float yOffset = Mathf.Sin(Time.time * 2) * 0.01f;
        transform.position = startPos + new Vector3(0, yOffset, 0);
    }
}
