using UnityEngine;

/*
[RequireComponent(typeof(PlayerInteractController))]
[RequireComponent(typeof(PlayerJetpackController))]
*/

[RequireComponent(typeof(PlayerMovementController))]
public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private PlayerCameraController playerCameraController;

    /*
    [SerializeField] private PlayerCanvasController playerCanvasController;
    [SerializeField] private PlayerHelmetLightController playerHelmetLightController;

    private PlayerInteractController playerInteractController;
    private PlayerJetpackController playerJetpackController;
    */

    private PlayerMovementController playerMovementController;

    void Awake()
    {
        /*
                playerInteractController = GetComponent<PlayerInteractController>();
                playerJetpackController = GetComponent<PlayerJetpackController>();
        */
        playerMovementController = GetComponent<PlayerMovementController>();
    }

    void Update()
    {
        //if (AsteroidCameraManager.Instance.AsteroidViewMode != AsteroidViewMode.Player)
        //{
        //    return;
        //}

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //playerCanvasController.HandleShowObjectives(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            //playerCanvasController.HandleShowObjectives(false);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            //playerHelmetLightController.HandleToggleHelmetLight();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            //playerInteractController.HandleInteraction();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            playerMovementController.HandleAscend();
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            playerMovementController.HandleDescend();
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //playerJetpackController.HandleForward();
        }
        playerCameraController.HandleMouseLook();
        playerMovementController.HandleMovement();
    }
}
