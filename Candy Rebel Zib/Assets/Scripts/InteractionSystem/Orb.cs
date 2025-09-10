using UnityEngine;

namespace Baseplate.interactionSystem
{
    public class Orb : MonoBehaviour, IInteractable
    {
        [SerializeField] string InteractionTetx;
        public void Interact()
        {
            Debug.Log(InteractionTetx);
            Destroy(gameObject);
        }
        public void OnHover()
        {
            Debug.Log("Hovering");
        }
    }
}
