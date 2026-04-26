using TMPro;
using UnityEngine;

public class PlayerInteractController : MonoBehaviour, IInteracter
{
    [SerializeField] private TextMeshProUGUI tooltip;
    [SerializeField] private Transform holdPoint;

    private const float PICKUP_RADIUS = 0.03f;
    private const float PICKUP_RANGE = 2f;

    private bool interacting = false;
    private InteractableController currentInteractable;

    public Transform HoldPoint => holdPoint;
    public Transform PlayerTransform => transform;
    
    void Update()
    {
        if (!interacting)
        {
            HandleHighlightingInteractable();
        }
    }

    public void HandleInteraction()
    {
        if (currentInteractable == null)
        {
            return;
        }
        currentInteractable.Interact(this);
    }

    public void StartInteracting()
    {
        interacting = true;
    }

    public void StopInteracting()
    {
        interacting = false;
    }

    private InteractableController FindBestInteractable()
    {
        InteractableController result = null;

        Vector3 origin = AsteroidCameraManager.Instance.GetCamera().transform.position;
        Vector3 forward = AsteroidCameraManager.Instance.GetCamera().transform.forward;

        RaycastHit[] hits = new RaycastHit[32];
        int hitCount = Physics.SphereCastNonAlloc(
            origin,
            PICKUP_RADIUS,
            forward,
            hits,
            PICKUP_RANGE
        );

        float bestScore = float.MaxValue;
        for (int i = 0; i < hitCount; i++)
        {
            if (hits[i].distance > PICKUP_RANGE)
            {
                continue;
            }

            if (!hits[i].collider.TryGetComponent(out InteractableController interactable))
            {
                continue;
            }

            if (!interactable.CanInteract())
            {
                continue;
            }

            float score = hits[i].distance;
            if (score < bestScore)
            {
                bestScore = score;
                result = interactable;
            }
        }
        return result;
    }

    private void HandleHighlightingInteractable()
    {
        InteractableController bestInteractable = FindBestInteractable();
        if (currentInteractable != bestInteractable) 
        {
            if (currentInteractable != null)
            {
                tooltip.text = "";
            }
            currentInteractable = bestInteractable;
            if (currentInteractable != null)
            {
                tooltip.text = currentInteractable.Tooltip;
            }
        }
    }
}
