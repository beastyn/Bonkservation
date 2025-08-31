using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using Gameplay;

namespace Managers
{

    public class Mascot
    {   
        public Transform MascotObject;
        public BuffType BuffType;
        public Mascot(Transform mascotObject, Vector3 position, BuffType buffType)
        {
            this.MascotObject = mascotObject;
            this.MascotObject.position = position;
            this.BuffType = buffType;

            this.MascotObject.gameObject.SetActive(true);
        }
    }

    public class MascotSpawner : MonoBehaviour
    {
        [Header("Mascots (scene instances)")]
        [Tooltip("Provide on Scene transforms of the mascots you want to place. Leave them disabled in the scene.")]
        [SerializeField] List<Transform> mascotsObjects = new();

        [Header("Placement Area")]        
        [Tooltip("Maximum radius from center.")]
        [SerializeField] float maxDistance = 300f;
        [SerializeField] NavMeshSurface navMeshSurface;

        List<Mascot> mascots = new();

        void Start()
        {
            this.SpawnAll();

        }

        public void SpawnAll()
        {           
            foreach (var mascot in mascotsObjects)
            {
               var randomPos = Utils.UtilsTools.GetRandomPointInNavMeshVolume(this.navMeshSurface, maxDistance);
               mascots.Add(new Mascot(mascotObject: mascot, position: randomPos, buffType: BuffType.DoubleDamage));
            }
        }        
    }
}


