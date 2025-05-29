#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ResetPlayerPrefs
{
    [MenuItem("Window/PlayerPrefs �ʱ�ȭ")]
    private static void ResetPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs has been reset.");
    }
}
#endif
