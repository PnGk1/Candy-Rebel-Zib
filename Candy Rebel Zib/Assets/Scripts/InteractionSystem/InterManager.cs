using Baseplate.InputManager;
using UnityEngine;

namespace Baseplate.interactionSystem
{
    public class InterManager : MonoBehaviour
    {

        //Values && Cache
        private IInteractable selectedInteractable;
        private Vector3 InteractPosition;
        private float ClosestDistancedInteractable = 0;

        //how many Interactables Can Be Detected
        private Collider[] hits = new Collider[10];

        //settings
        [Header("Interaction Detection Settings")]
        [Range(0.6f, 1.5f)]
        [SerializeField] float hitboxRadius;

        [Range(1f, 1.5f)]
        [SerializeField] float InteractMaxDistance;

        [Range(0.01f, 0.25f)]
        [SerializeField] float minDistanceImprovement = 0.05f;

        [Header("Layermask Settings")]
        [SerializeField] LayerMask interactablesMask;

        //Input System
        private PlayerInputManager playerInputManager;
        private PlayerControls playerControls;

        private void Awake()
        {
            //Getting The Components
            playerInputManager = GetComponent<PlayerInputManager>();
            playerControls = playerInputManager.playerControls;
        }
        private void Update()
        {
            FindInteractPosition();
            FindInteraction();
            Hover();
            if (playerControls.Player.Interact.WasPerformedThisFrame())
            {
                interact();
            }
        }
        private void FindInteractPosition()
        {
            InteractPosition = transform.position + transform.forward;
        }

        private void FindInteraction()
        {
            int hitCount = Physics.OverlapSphereNonAlloc(InteractPosition, hitboxRadius, hits, interactablesMask);

            selectedInteractable = null;
            ClosestDistancedInteractable = Mathf.Infinity;

            if (hitCount <= 0) return;

            for (int i = 0; i < hitCount; i++)
            {
                var col = hits[i];
                if (col == null || !col.gameObject.activeInHierarchy) continue;

                col.TryGetComponent<IInteractable>(out var interactable);

                if (interactable == null) continue;

                float distance = Vector3.Distance(InteractPosition, col.transform.position);
                if (distance <= InteractMaxDistance && distance < ClosestDistancedInteractable - minDistanceImprovement)
                {
                    ClosestDistancedInteractable = distance;
                    selectedInteractable = interactable;
                }
            }
        }

        private void interact()
        {
            if (selectedInteractable != null)
            {
                selectedInteractable.Interact();
            }
        }
        private void Hover()
        {
            if (selectedInteractable != null)
            {
                selectedInteractable.OnHover();
            }
        }
    }
}

