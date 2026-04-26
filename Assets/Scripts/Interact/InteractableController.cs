using UnityEngine;

public interface IInteracter
{
    public Transform HoldPoint { get; }
    public Transform PlayerTransform { get; }
    public void StartInteracting();
    public void StopInteracting();
}

public abstract class InteractableController : MonoBehaviour
{
    [SerializeField] private TooltipData tooltipData;

    private string tooltip;

    public string Tooltip => tooltip;

    void OnEnable()
    {
        tooltipData.description.StringChanged += UpdateTooltip;
    }

    void OnDisable()
    {
        tooltipData.description.StringChanged -= UpdateTooltip;
    }

    public virtual bool CanInteract()
    {
        /*
        if (!this.gameObject.TryGetComponent(out IdentityReference identityReference))
        {
            return false;
        }
        PlayerObjectiveRuntime currentPlayerObjectiveRuntime = ObjectiveManager.Instance.CurrentPlayerObjectiveRuntime;
        return currentPlayerObjectiveRuntime is InteractionObjectiveRuntime currentPlayerInteractionObjectiveRuntime && 
            currentPlayerInteractionObjectiveRuntime.ShouldInteract(identityReference.Identity);
        */

        return true;
    }

    public abstract void Interact(IInteracter interacter);

    private void UpdateTooltip(string value)
    {
        tooltip = value;
    }
}