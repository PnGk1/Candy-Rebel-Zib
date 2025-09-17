using UnityEngine;

namespace Baseplate.HealthSystem
{
    [System.Serializable]
    public struct HeartClass
    {
        public GameObject Heart;
        public float MaxHitPoints;
        public float CurrentHitPoints;

        public HeartClass(GameObject heart, float hitPoints,float currentHitPoints)
        {
            Heart = heart;
            MaxHitPoints = hitPoints;
            CurrentHitPoints = currentHitPoints;
        }
    }
}
