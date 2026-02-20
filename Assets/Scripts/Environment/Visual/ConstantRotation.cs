using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    [SerializeField] private Vector3 rotationVector;

    private void Update() => transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + rotationVector * Time.deltaTime);
}
