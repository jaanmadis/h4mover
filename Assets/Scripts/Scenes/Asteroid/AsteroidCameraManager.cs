using UnityEngine;

public enum AsteroidViewMode
{
    Player,
    Replay,
    Target,
}

public class AsteroidCameraManager : MonoBehaviour, ICameraManager
{
    [SerializeField] private Camera capsuleCamera;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Canvas playerCanvas;
    [SerializeField] private Canvas replayCanvas;
    [SerializeField] private Canvas targetCanvas;

    private AsteroidViewMode asteroidViewMode = AsteroidViewMode.Player;
    private Camera activeCamera = null;
    private Canvas activeCanvas = null;

    public static AsteroidCameraManager Instance { get; private set; }

    public AsteroidViewMode AsteroidViewMode => asteroidViewMode;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        capsuleCamera.gameObject.SetActive(false);
        replayCanvas.gameObject.SetActive(false);
        targetCanvas.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        playerCanvas.gameObject.SetActive(true);
        activeCamera = playerCamera;
        activeCanvas = playerCanvas;
    }

    public Camera GetCamera()
    {
        return activeCamera;
    }

    public void SetAsteroidViewMode(AsteroidViewMode newAsteroidViewMode)
    {
        activeCamera.gameObject.SetActive(false);
        activeCanvas.gameObject.SetActive(false);
        activeCamera.TryGetComponent(out AudioListener prevAudioListener);
        prevAudioListener.enabled = false;

        asteroidViewMode = newAsteroidViewMode;
        switch (asteroidViewMode)
        {
            case AsteroidViewMode.Player:
                activeCamera = playerCamera;
                activeCanvas = playerCanvas;
                break;
            case AsteroidViewMode.Replay:
                activeCamera = capsuleCamera;
                activeCanvas = replayCanvas;
                break;
            case AsteroidViewMode.Target:
                activeCamera = capsuleCamera;
                activeCanvas = targetCanvas;
                break;
        }

        activeCamera.gameObject.SetActive(true);
        activeCanvas.gameObject.SetActive(true);
        activeCamera.TryGetComponent(out AudioListener nextAudioListener);
        nextAudioListener.enabled = true;
    }
}
