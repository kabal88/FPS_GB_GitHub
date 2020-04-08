using UnityEngine;
using UnityEngine.AI;

namespace Geekbrains
{
    public static class Patrol
    {
        public static Vector3 GenericRandomPoint(Transform agent)
        {
            //todo перемещение по точкам
            Vector3 result;

            var dis = Random.Range(5, 50);
            var randomPoint = Random.insideUnitSphere * dis;

            NavMesh.SamplePosition(agent.position + randomPoint,
                out var hit, dis, NavMesh.AllAreas);
            result = hit.position;

            return result;
        }

        public static Vector3 GenericStartingPoint(Transform agent, Affiliation side)
        {
            Vector3 result;
            Vector3 startingPoint;

            var dis = Random.Range(2, 8);
            var randomPoint = Random.insideUnitSphere * dis;


            switch (side)
            {
                case Affiliation.SideOne:
                    startingPoint = ServiceLocatorMonoBehaviour.GetService<Reference>().SpawningPointTeamOne.position;
                    break;
                case Affiliation.SideTwo:
                    startingPoint = ServiceLocatorMonoBehaviour.GetService<Reference>().SpawningPointTeamTwo.position;
                    break;
                case Affiliation.None:
                default:
                    startingPoint = Vector3.zero;
                    break;
            }
            NavMesh.SamplePosition(startingPoint + randomPoint,
                out var hit, dis, NavMesh.AllAreas);
            result = hit.position;

            return result;

        }
    }
}
