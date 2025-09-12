using Baseplate.InputManager;
using UnityEngine;
using UnityEngine.AI;

namespace Baseplate.AI
{
    public class AIcontroller : MonoBehaviour
    {
        private Vector3 currentPos;
        private PlayerInputManager targetedPlayer;
        private int WaypointIndex = 0;
        //cache
        private NavMeshAgent agent;
        //settings
        [Header("Detection Settings")]
        [SerializeField]
        private float SightRange = 5f;

        [SerializeField]
        private LayerMask PlayerMask;

        [Header("Speed Settings")]
        [SerializeField]
        private float walkSpeed = 3f;

        [SerializeField]
        private float fastWalkSpeed = 4.5f;

        [SerializeField]
        private float runSpeed = 6f;

        [Header("Memory Settings")]

        [SerializeField]
        private float TimeBetweenForgettingPlayer = 5f;

        private float TimeSinceLastPlayerInteraction = 0f;

        private Vector3 LastKnownPlayerPosition;

        [Header("Waypoint Settings")]
        [SerializeField]
        private float stoppingRadius = 1f;

        [SerializeField] PatrolRouteScript CurrentPatrolRoute;

        private State state;
        private enum State
        {
            roaming, Following, Suspicious
        }
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }
        private void Update()
        {
            currentPos = agent.transform.position;
            switch (state)
            {
                case State.roaming:
                    ChangeSpeed(walkSpeed);
                    Roaming();
                    break;

                case State.Following:
                    ChangeSpeed(runSpeed);
                    following();
                    break;

                case State.Suspicious:
                    ChangeSpeed(fastWalkSpeed);
                    suspicious();
                    break;
            }
            Detection();
        }

        private void Roaming()
        {
            if (agent == null || CurrentPatrolRoute == null) return;

            Vector3 nextLocation = CurrentPatrolRoute.GetPosition(WaypointIndex);

            if (!agent.hasPath)
            {
                    agent.SetDestination(nextLocation);
            }

            // wait until the agent's path is ready
            if (agent.pathPending) return;

            float distance = Vector3.Distance(currentPos, nextLocation);
            if (distance < stoppingRadius)
            {
                WaypointIndex = CurrentPatrolRoute.GetNextIndex(WaypointIndex);
                Vector3 newLocation = CurrentPatrolRoute.GetPosition(WaypointIndex);
                agent.SetDestination(newLocation);
                return;
            }
        }

        private void following()
        {

            ForgetTimer(State.Suspicious);
        }

        private void suspicious()
        {
            if (targetedPlayer != null)
            {
                LastKnownPlayerPosition = targetedPlayer.transform.position;
                targetedPlayer = null;
            }
            agent.SetDestination(LastKnownPlayerPosition);
            ForgetTimer(state = State.roaming);
        }

        private void ForgetTimer(State ForgetState)
        {
            if (TimeSinceLastPlayerInteraction < TimeBetweenForgettingPlayer)
            {
                TimeSinceLastPlayerInteraction += Time.deltaTime;
                return;
            }
            TimeSinceLastPlayerInteraction = 0;
            agent.SetDestination(currentPos);
            state = ForgetState;
        }
        private void Detection()
        {
            Collider[] playersInRange = Physics.OverlapSphere(currentPos, SightRange, PlayerMask);
            for (int i = 0; i < playersInRange.Length; i++)
            {
                PlayerInputManager player = playersInRange[i].GetComponentInParent<PlayerInputManager>();

                if (player != null)
                {
                    Vector3 Dir = player.transform.position - currentPos;
                    RaycastHit hit;

                    if (Physics.Raycast(currentPos, Dir.normalized, out hit, SightRange))
                    {
                        if (hit.transform.IsChildOf(player.transform))
                        {
                            targetedPlayer = player;
                            agent.SetDestination(targetedPlayer.transform.position);
                            state = State.Following;
                            break;
                        }
                    }
                }
            }
        }

        private void ChangeSpeed(float speed)
        {
            if (agent.speed == speed) return;
            agent.speed = speed;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(currentPos, SightRange);
        }
    }
}
