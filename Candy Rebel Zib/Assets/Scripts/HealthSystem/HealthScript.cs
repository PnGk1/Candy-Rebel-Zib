using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Baseplate.HealthSystem
{
    public class HealthScript : MonoBehaviour, IDamageable
    {
        //UI Elements
        [SerializeField] GameObject HeartPrefab;
        private GameObject HealthPanel;

        //Cache
        [SerializeField] private List<HeartClass> CurrentHearts = new List<HeartClass>();

        //Parameters
        [SerializeField, Range(1, 10)] int MaxHearts;
        private float MaxHitPoints = 2;

        private void Awake()
        {
            HealthPanel = GameObject.Find("HealthPanel");

            //Setting Up
            EnsureHeartsExist();
            RefreshUI();
        }

        public void Damage(float hitpoints)
        {
            Debug.Log("Damaged");

            int damageLeft = Mathf.CeilToInt(hitpoints);

            for (int i = CurrentHearts.Count - 1; i >= 0 && damageLeft > 0; i--)
            {
                var heart = CurrentHearts[i];

                if (heart.CurrentHitPoints <= 0)
                    continue;

                float damageToApply = Mathf.Min(heart.CurrentHitPoints, damageLeft);
                heart.CurrentHitPoints -= damageToApply;
                damageLeft -= (int)damageToApply;

                CurrentHearts[i] = heart;

                RefreshUI();
            }
        }

        public void Heal(float hitpoints)
        {
            Debug.Log("Healed");

            int HealLeft = Mathf.CeilToInt(hitpoints);

            for (int i = 0; i < CurrentHearts.Count && HealLeft > 0; i++)
            {
                var heart = CurrentHearts[i];

                if (heart.CurrentHitPoints >= heart.MaxHitPoints)
                    continue;

                float missingHealth = heart.MaxHitPoints - heart.CurrentHitPoints;
                float healthToApply = Mathf.Min(missingHealth, HealLeft);

                heart.CurrentHitPoints += healthToApply;
                HealLeft -= (int)healthToApply;

                CurrentHearts[i] = heart;

                RefreshUI();
            }
        }

        public void RefreshUI()
        {
            Image Frontground;

            foreach (var heart in CurrentHearts)
            {
               float precent = (heart.CurrentHitPoints - 0) / (heart.MaxHitPoints - 0);
               Image[] Heartimages = heart.Heart.GetComponentsInChildren<Image>();

               foreach (Image heartimage in Heartimages)
               {
                   if (heartimage.gameObject.name == "Frontground")
                    {
                        Frontground = heartimage;
                        Frontground.fillAmount = precent;
                        break;
                    }
               }
            }
        }

        public void EnsureHeartsExist()
        {
            int length = CurrentHearts.Count;

            if (MaxHearts == length) return;

            int neededHearts = MaxHearts - length;

            if (neededHearts > 0)
            {
                for (int i = length; i < MaxHearts; i++)
                {
                    GameObject instantiatedHeart = Instantiate(HeartPrefab, HealthPanel.transform);
                    var heartClass = new HeartClass(instantiatedHeart, MaxHitPoints, MaxHitPoints);
                    CurrentHearts.Add(heartClass);
                }
            }
            else if (neededHearts < 0)
            {
                int removeCount = -neededHearts;
                for (int i = 0; i < removeCount; i++)
                {
                    int lastIndex = CurrentHearts.Count - 1;
                    Destroy(CurrentHearts[lastIndex].Heart);
                    CurrentHearts.RemoveAt(lastIndex);
                }
            }

            ReorderHearts();
        }

        private void ReorderHearts()
        {
            int targetIndex = 0;

            for (int i = 0; i < CurrentHearts.Count; i++)
            {
                if (CurrentHearts[i].Heart != null)
                {
                    if (i != targetIndex)
                    {
                        var selectedHeart = CurrentHearts[i];
                        CurrentHearts.RemoveAt(i);
                        CurrentHearts.Insert(targetIndex, selectedHeart);
                    }
                    targetIndex++;
                }
            }
        }

    }
}
