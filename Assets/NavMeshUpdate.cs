using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshUpdate : MonoBehaviour
{
    [SerializeField]
    NavMeshSurface surfaceToBake;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.surfaceToBake.BuildNavMesh();
    }

}
