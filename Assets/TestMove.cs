using UnityEngine;

namespace DefaultNamespace
{
    public sealed class TestMove : MonoBehaviour 
    {
        public UnityEngine.AI.NavMeshAgent agent { get; private set; }
        public Transform target;                                  


        private void Start()
        {
            agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            
        }


        private void Update()
        {
            if (target != null)
                agent.SetDestination(target.position);
        }
    }
}
