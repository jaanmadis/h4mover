using UnityEngine;
using UnityEngine.EventSystems;

public static class CursorUtils
{
    public static EventSystem GlobalEventSystem => eventSystem;

    private static EventSystem eventSystem = null;

    public static void SetGlobalEventSystem(EventSystem globalEventSystem)
    {
        eventSystem = globalEventSystem;
    }

    public static void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (GlobalEventSystem != null)
        {
            GlobalEventSystem.enabled = false;
        }
    }

    public static void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        if (GlobalEventSystem != null)
        {
            GlobalEventSystem.enabled = true;
        }
    }
}
