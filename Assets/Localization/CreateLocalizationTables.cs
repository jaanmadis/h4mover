using UnityEditor;
using UnityEngine.Localization.Tables;
using UnityEditor.Localization;

public static class CreateLocalizationTables
{
    [MenuItem("Tools/Create Tooltips Table")]
    public static void CreateTable()
    {
        var collection = LocalizationEditorSettings.CreateStringTableCollection("Tooltips", "Assets/Localization");
    }
}