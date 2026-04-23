using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerCanvasController : MonoBehaviour
{
    [SerializeField] private GameObject buttonPanel;
    [SerializeField] private CanvasGroup buttonPanelCanvasGroup;
    [SerializeField] private CanvasGroup additionalTextsCanvasGroup;
    [SerializeField] private GameObject objectivesPanel;

    [Header("Player")]
    [SerializeField] private Light playerHelmetLight;
    [SerializeField] private PlayerMovementController playerMovementController;

    [Header("Buttons Panel")]
    [SerializeField] private TextMeshProUGUI movementText;
    [SerializeField] private TextMeshProUGUI ascendText;
    [SerializeField] private TextMeshProUGUI descendText;
    [SerializeField] private TextMeshProUGUI sprintText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI lightText;
    [SerializeField] private TextMeshProUGUI objectiveText;

    [Header("Objectives Panel")]
    [SerializeField] private TextMeshProUGUI objectiveNameText;
    [SerializeField] private TextMeshProUGUI objectiveDescriptionText;

    [Header("Navigation Panel")]
    [SerializeField] private TextMeshProUGUI altitudeValueText;
    [SerializeField] private TextMeshProUGUI horizontalSpeedText;
    [SerializeField] private TextMeshProUGUI verticalSpeedText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI angleText;

    private const float FADE_DELAY = 3f;
    private const float FADE_DURATION = 3f;

    private const string MOVEMENT_TEXT =       "Movement      [W/A/S/D]";
    private const string ASCEND_TEXT =         "Ascend        [SPACE]";
    private const string DESCEND_TEXT =        "Descend       [CTRL]";
    private const string SPRINT_TEXT =         "Flight Boost  [SHIFT]";
    private const string INTERACT_TEXT =       "Interact      [E]";
    private const string LIGHT_TEXT =          "Helmet Light  [F]";
    private const string OBJECTIVE_TEXT =      "Objective     [Tab]";
    private const string NEW_OBJECTIVE_TEXT =  "Objective     [Tab] *";

    private const string ALTITUDE_VALUE_FORMAT =   " ALT: {0,8:000.0} [m]";
    private const string HORIZONTAL_SPEED_FORMAT = "HSPD: {0,8:000.0} [m/s]";
    private const string VERTICAL_SPEED_FORMAT =   "VSPD: {0,8:+000.0;-000.0; 000.0} [m/s]";
    private const string DISTANCE_FORMAT =         " DST: {0,8:000.0} [m]";
    private const string ANGLE_FORMAT =            " ANG: {0,8:000.0} [\u00B0]";

    private bool objectivesAvailable = false;
    private string seenObjectiveId = null;
    private Coroutine fadeRoutine = null;

    public bool ObjectivesAvailable => objectivesAvailable;
    public string SeenObjectiveId => seenObjectiveId;

    void Awake()
    {
        movementText.text = MOVEMENT_TEXT;
        ascendText.text = ASCEND_TEXT;
        descendText.text = DESCEND_TEXT;
        sprintText.text = SPRINT_TEXT;
        interactText.text = INTERACT_TEXT;
        lightText.text = LIGHT_TEXT;
        objectivesPanel.SetActive(false);
    }

    void OnEnable()
    {
/*
        GlobalEventBus.Instance.Subscribe<PlayerObjectiveCompleteEvent>(OnPlayerObjectiveComplete);
        GlobalEventBus.Instance.Subscribe<PlayerObjectiveLoadEvent>(OnPlayerObjectiveLoad);
*/
        // Events may have been missed if this object were disabled.
        // Update to refresh stale state.
        UpdateObjectiveTexts();
    }

    void OnDisable()
    {
/*
        GlobalEventBus.Instance.UnSubscribe<PlayerObjectiveCompleteEvent>(OnPlayerObjectiveComplete);
        GlobalEventBus.Instance.UnSubscribe<PlayerObjectiveLoadEvent>(OnPlayerObjectiveLoad);
*/
    }

    void Update()
    {
        //lightText.color = playerHelmetLight.enabled ? Color.yellow : Color.white;
    }

    void FixedUpdate()
    {
        altitudeValueText.text = string.Format(ALTITUDE_VALUE_FORMAT, playerMovementController.Altitude);
        //angleText.text = string.Format(ANGLE_FORMAT, playerMovementController.AngleToCapsule);
        //distanceText.text = string.Format(DISTANCE_FORMAT, playerMovementController.DistanceToCapsule);
        horizontalSpeedText.text = string.Format(HORIZONTAL_SPEED_FORMAT, playerMovementController.HorizontalSpeed);
        verticalSpeedText.text = string.Format(VERTICAL_SPEED_FORMAT, playerMovementController.VerticalSpeed);
    }

    public void HandleShowObjectives(bool show)
    {
        if (show)
        {
            if (objectivesAvailable)
            {
                buttonPanel.SetActive(false);
                objectivesPanel.SetActive(true);
                SetObjectiveIndicatorText(true);
                //seenObjectiveId = ObjectiveManager.Instance.CurrentPlayerObjectiveRuntime?.PlayerObjective.Id;
            }
        }
        else
        {
            buttonPanel.SetActive(true);
            objectivesPanel.SetActive(false);
        }
    }
    /*
    public void OnPlayerObjectiveComplete(PlayerObjectiveCompleteEvent playerObjectiveLoadEvent)
    {
        objectiveNameText.text = "";
        objectiveDescriptionText.text = "";
        SetObjectiveIndicatorText(true);
    }

    public void OnPlayerObjectiveLoad(PlayerObjectiveLoadEvent playerObjectiveLoadEvent)
    {
        UpdateObjectiveTexts();
    }
    */
    public void SetObjectivesAvailable(bool newObjectivesAvailable)
    {
        objectivesAvailable = newObjectivesAvailable;
        buttonPanel.SetActive(objectivesAvailable);
        additionalTextsCanvasGroup.alpha = 0f;
        if (!objectivesAvailable && fadeRoutine == null)
        {
            fadeRoutine = StartCoroutine(Fade());
        }
    }

    public void SetSeenObjectiveId(string newSeenObjectiveId)
    {
        seenObjectiveId = newSeenObjectiveId;
    }

    private IEnumerator Fade()
    {
/*
        yield return GlobalCanvasManager.Instance.ShowFadeCanvasRoutine(
            Constants.SCENE_TRANSITION_FADE_DELAY, 
            Constants.SCENE_TRANSITION_FADE_DURATION, 
            0, 
            false
        );

        buttonPanel.SetActive(true);
        buttonPanelCanvasGroup.alpha = 0f;
        additionalTextsCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(FADE_DELAY);

        objectivesAvailable = true;

        float t = 0f;
        while (t < FADE_DURATION)
        {
            t += Time.deltaTime;
            buttonPanelCanvasGroup.alpha = t / FADE_DURATION;
            yield return null;
        }
        buttonPanelCanvasGroup.alpha = 1f;
*/
        yield return new WaitForSeconds(FADE_DELAY * 2);
/*
        t = 0f;
        while (t < FADE_DURATION)
        {
            t += Time.deltaTime;
            additionalTextsCanvasGroup.alpha = 1 - t / FADE_DURATION;
            yield return null;
        }
        additionalTextsCanvasGroup.alpha = 0f;
*/
    }

    private void UpdateObjectiveTexts()
    {
        /*
        // This is called by OnEnable which may be called before ObjectiveManager awake sets up ObjectiveManager.Instance
        if (ObjectiveManager.Instance == null)
        {
            return;
        }
        objectiveNameText.text = ObjectiveManager.Instance.CurrentPlayerObjectiveRuntime?.PlayerObjective.ShortName ?? "";
        objectiveDescriptionText.text = ObjectiveManager.Instance.CurrentPlayerObjectiveRuntime?.PlayerObjective.Description ?? "";
        SetObjectiveIndicatorText(seenObjectiveId == ObjectiveManager.Instance.CurrentPlayerObjectiveRuntime?.PlayerObjective.Id);
        */
    }

    private void SetObjectiveIndicatorText(bool seen)
    {
        objectiveText.text = seen ? OBJECTIVE_TEXT : NEW_OBJECTIVE_TEXT;
        objectiveText.color = seen ? Color.white : Color.yellow;
    }
}
