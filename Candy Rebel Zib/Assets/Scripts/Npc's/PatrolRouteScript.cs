using System;
using UnityEngine;

namespace Baseplate.AI
{
    public class PatrolRouteScript : MonoBehaviour
    {
        [Header("Gizmos Settings")]
        [SerializeField] float GizmosSize = 1.0f;

        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                Gizmos.DrawWireSphere(GetPosition(i), GizmosSize);
                Gizmos.DrawLine(GetPosition(i), GetPosition(j));
            }
        }

        public int GetNextIndex(int i)
        {
            if (i + 1 >= transform.childCount)
            {
                return 0;
            }
            else
            {
                return i + 1;
            }
        }

        public Vector3 GetPosition(int i)
        {
            return transform.GetChild(i).transform.position;
        }

    }
}
