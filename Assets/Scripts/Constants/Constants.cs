// Player input to "Both"
// Player camera AA to TAA
// Vsync to on and every frame

// Astral Cargo & Tropical Fruit Corporation Ltd.
// ACTFC Space Division

// Option to set SI > Imperial. Start game, explode on takeoff, unit incompatability.

// Same GameObject + Required -> RequireComponent + GetComponent
// Same GameObject + Optional -> GetComponent (no require)
// Different GameObject -> SerializeField

// what about making the player orbit the asteroid when approaching?

using UnityEngine;

public static class Constants
{
    public const float PLAYER_CROUCH_FACTOR = 0.5f;
    public const float PLAYER_HEIGHT = 2f;

    public const float GRAVITY_STRENGTH_ASTEROID = 2f;

    //public const float GRAVITY_STRENGTH_LANDING = 1f;

    public const float SCENE_TRANSITION_FADE_DURATION = 1f;
    public const float SCENE_TRANSITION_FADE_DELAY = 0.5f;

    public static readonly Vector3 ASTEROID_CENTER = Vector3.zero;

    //public static readonly Vector3 ASTEROID_SURFACE_LANDING = new(0, -89750, 0);

    //public const float PLAYER_DEFAULT_ACCELERATION = 20f;

    //public const string GAME_OBJECT_BEACON_ID = "game_object_beacon_id";
    //public const string GAME_OBJECT_CAPSULE_CAMERA_PANEL_ID = "game_object_capsule_camera_panel_id";
    //public const string GAME_OBJECT_CAPSULE_LANDED = "game_object_capsule_landed";
    //public const string GAME_OBJECT_CAPSULE_LANDING_ID = "game_object_capsule_landing_id";
    //public const string GAME_OBJECT_DRILL_ID = "game_object_drill_id";
    //public const string GAME_OBJECT_FLAGPOLE_ID = "game_object_flagpole_id";
    //public const string GAME_OBJECT_FLAGPOLE_BUTTON_ID = "game_object_flagpole_button_id";
    //public const string GAME_OBJECT_PLAYER_ID = "game_object_player_id";
    //public const string GAME_OBJECT_THRUSTER_ID_PREFIX= "game_object_thruster_id_";

    //public const string DEATH_CAPSULE_LANDING_MALFUNCTION_1 = "Minor error in automatic attitude control";
    //public const string DEATH_CAPSULE_LANDING_MALFUNCTION_2 = "Leading to suboptimal performance of starboard engine";
    //public const string DEATH_CAPSULE_LANDING_MALFUNCTION_3 = "Human asset non-recoverable";

    //public const string DEATH_CAPSULE_LANDING_CRASH_1 = "Landing procedure unsuccessful";
    //public const string DEATH_CAPSULE_LANDING_CRASH_2 = "Velocity outside recommended parameters";
    //public const string DEATH_CAPSULE_LANDING_CRASH_3 = "Human asset unavailable for further mission duties";

    //public const float PLAYER_OBJECTIVE_PLACE_EQUIPMENT_TOLERANCE = 3f;
 
    //public const float PLAYER_OBJECTIVE_PLACE_FLAGPOLE_DISTANCE = 15f;
    //public const string PLAYER_OBJECTIVE_PLACE_FLAGPOLE_ID = "player_objective_place_flagpole_id";
    //public const string PLAYER_OBJECTIVE_PLACE_FLAGPOLE_NAME = "Place Flagpole";
    //public const string PLAYER_OBJECTIVE_PLACE_FLAGPOLE_DESC = "Place the flagpole approximately 15 meters from the lander to mark the landing site.";

    //public const float PLAYER_OBJECTIVE_TARGET_CAMERA_AT_FLAGPOLE_TOLERANCE = 30f;
    //public const string PLAYER_OBJECTIVE_TARGET_CAMERA_AT_FLAGPOLE_ID = "player_objective_position_camera_at_flagpole_id";
    //public const string PLAYER_OBJECTIVE_TARGET_CAMERA_AT_FLAGPOLE_NAME = "Target Camera at Flagpole";
    //public const string PLAYER_OBJECTIVE_TARGET_CAMERA_AT_FLAGPOLE_DESC = "Target the lander camera at the flagpole to record the ACTFC flag raising ceremony for maximum shareholder satisfaction.";

    //public const float PLAYER_OBJECTIVE_PLACE_BEACON_DISTANCE = 35f;
    //public const string PLAYER_OBJECTIVE_PLACE_BEACON_ID = "player_objective_place_beacon_id";
    //public const string PLAYER_OBJECTIVE_PLACE_BEACON_NAME = "Deploy Long-Range Beacon";
    //public const string PLAYER_OBJECTIVE_PLACE_BEACON_DESC = "Deploy the long-range beacon along the rotational axis to ensure stable transmission.\n\nPosition the beacon approximately 35 meters from the lander.";

    //public const float PLAYER_OBJECTIVE_PLACE_DRILL_DISTANCE = 250f;
    //public const float PLAYER_OBJECTIVE_PLACE_DRILL_TOLERANCE = 10f;
    //public const string PLAYER_OBJECTIVE_PLACE_DRILL_ID = "player_objective_place_drill_id";
    //public const string PLAYER_OBJECTIVE_PLACE_DRILL_NAME = "Deploy Deep Core Probe";
    //public const string PLAYER_OBJECTIVE_PLACE_DRILL_DESC = "Deploy the deep core probe at the equatorial stress maximum to conduct internal resonance mapping.\n\nPosition the probe approximately 250 meters from the lander.";

    //public const string PLAYER_OBJECTIVE_RAISE_FLAG_ID = "player_objective_raise_flag_id";
    //public const string PLAYER_OBJECTIVE_RAISE_FLAG_NAME = "Raise the flag";
    //public const string PLAYER_OBJECTIVE_RAISE_FLAG_DESC = "Activate the flagpole to deploy the ACTFC flag.\n\nFace the camera and salute to commemorate this historic moment.\n\nThis installation will stand for billions of years as a lasting symbol of ACTFC leadership and innovation.";

    //public const string PLAYER_OBJECTIVE_RETURN_TO_CAPSULE_ID = "player_objective_return_to_capsule_id";
    //public const string PLAYER_OBJECTIVE_RETURN_TO_CAPSULE_NAME = "Return to Lander";
    //public const string PLAYER_OBJECTIVE_RETURN_TO_CAPSULE_DESC = "Mission control has detected an anomaly near the landing site. Return to the lander.\n\nMaintain a heading of 0.0 [°] using your suit's compass to return to the lander.\n\nFurther instructions pending.";

    //public const string PLAYER_OBJECTIVE_RETURN_TO_FLAG_ID = "player_objective_return_to_flag_id";
    //public const string PLAYER_OBJECTIVE_RETURN_TO_FLAG_NAME = "Inspect Flagpole";
    //public const string PLAYER_OBJECTIVE_RETURN_TO_FLAG_DESC = "Return to the flagpole and conduct a visual inspection.\n\nThe installation is expected to remain upright for its full multi-billion-year service life.\n\nIdentify any factors affecting ACTFC brand presentation.";

    //public const string PLAYER_OBJECTIVE_REPLAY_CAMERA_ID = "player_objective_replay_camera_id";
    //public const string PLAYER_OBJECTIVE_REPLAY_CAMERA_NAME = "Review recorded camera footage";
    //public const string PLAYER_OBJECTIVE_REPLAY_CAMERA_DESC = "Due to the transmission distance, mission control received only low-resolution, compressed footage.\n\nAccess the recorded lander camera data and replay the footage to analyze what occurred here.";

    //public const string PLAYER_OBJECTIVE_PLACE_FLAGPOLE_AGAIN_ID = "player_objective_place_flagpole_again_id";
    //public const string PLAYER_OBJECTIVE_PLACE_FLAGPOLE_AGAIN_NAME = "Place Flagpole, Again";
    //public const string PLAYER_OBJECTIVE_PLACE_FLAGPOLE_AGAIN_DESC = "Restore ACTFC flag to preserve mission optics, stakeholder confidence and brand visibility.\n\nThis installation is projected to stand for billions of years, regardless of local anomalies.";
}
