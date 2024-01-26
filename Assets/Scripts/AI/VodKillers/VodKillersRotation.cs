namespace AI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class VodKillersRotation : MonoBehaviour
    {
        [SerializeField] VodKillersSO vodKillers;

        /*// Start is called before the first frame update
        void Awake()
        {
            this.vodKillers.SetVodKiller(this.vodKillers.VodKillers[0]);
            StartCoroutine(this.SwitchKiller());
        }*/

        public Transform RequestKiller()
        {
            if ( vodKillers != null )
            {
                int rand = Random.Range(0, 3);
                this.vodKillers.SetVodKiller(this.vodKillers.VodKillers[rand]);

                return this.vodKillers.CurrentKiller.Killer;
            }
            return null;
        }

        IEnumerator SwitchKiller()
        {
            while (true)
            {
                int rand = Random.Range(0, 3);
                this.vodKillers.SetVodKiller(this.vodKillers.VodKillers[rand]);

                yield return new WaitForSeconds(Random.Range(5, 20));
            }
        }
    }
}
