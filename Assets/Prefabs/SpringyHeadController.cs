using UnityEngine;

public class SpringyHeadController : MonoBehaviour
{
    public Rigidbody headRigidbody;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            headRigidbody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
    }
}