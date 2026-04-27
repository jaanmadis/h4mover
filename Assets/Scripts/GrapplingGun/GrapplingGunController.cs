using System.Collections;
using UnityEngine;

/*
 
1. load arrow into gun
2. attach tether to arrow
3. shoot the arrow with tether
4a. pull self towards arrow
4b. detach tether from arrow
* after shooting you can make gun disappear
* spooling back tether -- harmless
* spooling back arrow -- lethal
* how would arro get un-lodged anyway
 
 */

public class GrapplingGunController : MonoBehaviour
{
    [Header("Arrow")]
    [SerializeField] private GrapplingArrowController grapplingArrow;

    [Header("Cable")]
    [SerializeField] private LineRenderer grapplingGunCable;
    [SerializeField] private Transform cablePoint;

    [Header("Player Camera")]
    [SerializeField] private PlayerCameraController playerCameraController;
    [SerializeField] private Transform playerCameraTransform;

    [Header("Player Movement")]
    [SerializeField] private PlayerMovementController playerMovementController;

    private const float AUTO_RETRACT_DISTANCE_SQ = 25f;
    private const float AUTO_RETRACT_TIME = 5f;
    private const float MAX_RANGE = 200f;

    private Coroutine autoRetractRoutine;

    void FixedUpdate()
    {
        if ((grapplingArrow.transform.position - transform.position).sqrMagnitude < AUTO_RETRACT_DISTANCE_SQ)
        {
            HandleAlternateFire();
        }
    }

    void LateUpdate()
    {
        if (grapplingGunCable.enabled)
        {
            grapplingGunCable.SetPosition(0, cablePoint.position);
            grapplingGunCable.SetPosition(1, grapplingArrow.CablePoint.position);
        }
    }

    public void HandlePrimaryFire()
    {
        // qqq make a targeting rectangle iwth like gren/red indicator if we can fire or not
        if (grapplingArrow.CurrentState == GrapplingArrowController.State.Loaded && 
            Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit hit, MAX_RANGE) &&
            hit.distance * hit.distance > AUTO_RETRACT_DISTANCE_SQ)
        {
            grapplingArrow.FireArrow(playerCameraTransform.forward);
            grapplingGunCable.enabled = true;
            playerCameraController.LockView();
            playerMovementController.SetPlayerControl(false);
            autoRetractRoutine = StartCoroutine(AutoRetract());
        }
        else if (grapplingArrow.CurrentState == GrapplingArrowController.State.Attached)
        {
            CancelAutoRetract();
            playerMovementController.HandleGrapplePull(grapplingArrow.transform.position);
        }
    }

    public void HandleAlternateFire()
    {
        if (grapplingArrow.CurrentState == GrapplingArrowController.State.Attached)
        {
            CancelAutoRetract();
            RetractArrow();
            playerMovementController.HandleGrappleRetract();
        }
    }

    public void HandleArrowRetracted()
    {
        grapplingGunCable.enabled = false;
        playerCameraController.UnlockView();
        playerMovementController.SetPlayerControl(true);
    }

    private IEnumerator AutoRetract()
    {
        yield return new WaitForSeconds(AUTO_RETRACT_TIME);
        RetractArrow();
        autoRetractRoutine = null;
    }

    private void CancelAutoRetract()
    {
        if (autoRetractRoutine != null)
        {
            StopCoroutine(autoRetractRoutine);
            autoRetractRoutine = null;
        }
    }

    private void RetractArrow()
    {
        grapplingArrow.RetractArrow();
    }
}
