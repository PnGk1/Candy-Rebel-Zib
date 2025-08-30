using UnityEngine;

namespace Baseplate.interactionSystem
{
    public class Button : MonoBehaviour, IInteractable
    {
        [SerializeField] string InteractionTetx;
        public void Interact()
        {
            Debug.Log(InteractionTetx);
        }
        public void OnHover()
        {
            Debug.Log(InteractionTetx);
        }
    }
}
