using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

namespace Utils
{
    public static class UtilsTools
    {
        /// <summary>
        /// Returns a random valid position on the NavMesh within the bounds of the specified NavMeshSurface.
        /// The method repeatedly samples random points inside the surface's volume until it finds one on the NavMesh,
        /// using the given maxDistance for sampling. Useful for spawning or moving agents to random navigable locations.
        /// </summary>
        /// <param name="navMeshSurface">The NavMeshSurface to sample within.</param>
        /// <param name="maxDistance">The maximum distance to search for a valid NavMesh position from each random point.</param>
        /// <returns>A Vector3 position on the NavMesh within the surface bounds.</returns>
        public static Vector3 GetRandomPointInNavMeshVolume(NavMeshSurface navMeshSurface, float maxDistance)
        {
            // Get the bounds of the NavMesh surface
            Bounds navMeshBounds = navMeshSurface.navMeshData.sourceBounds;

            Vector3 randomPoint;
            NavMeshHit hit;

            // Try to generate a point inside the volume bounds
            do
            {
                randomPoint = new Vector3(
                    Random.Range(navMeshBounds.min.x, navMeshBounds.max.x), // Random x
                    navMeshBounds.center.y,                                // Fixed Y level
                    Random.Range(navMeshBounds.min.z, navMeshBounds.max.z)  // Random z
                );

                // Check if the random point is on the NavMesh within maxDistance
            } while (!NavMesh.SamplePosition(randomPoint, out hit, maxDistance, NavMesh.AllAreas));

            return hit.position; // Return the valid point inside the NavMesh volume
        }
    }
}
