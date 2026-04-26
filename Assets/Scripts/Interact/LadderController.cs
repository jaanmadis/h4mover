public class LadderController : InteractableController
{
    public override void Interact(IInteracter interacter)
    {
        interacter.StartInteracting();
        interacter.StopInteracting();
    }
}
