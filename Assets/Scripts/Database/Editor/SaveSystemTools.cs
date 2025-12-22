using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public static class SaveSystemTools
{
    [MenuItem("Tools/Player/Add 1000 Money")]
    public static void Add1000Money()
    {
        JollyPanda.LastFlag.Database.SaveSystem.AddMoney(1000);
        Debug.Log("Added 1000 money to player.");
    }

    [MenuItem("Tools/Player/Reset Save Data")]
    public static void ResetSaveData()
    {
        JollyPanda.LastFlag.Database.SaveSystem.ResetData();
        Debug.Log("Save data has been reset.");
    }
}
#endif