using UnityEngine;

public static class PlatformHelper
{
    public static bool ShouldUseMobileGUI()
    {
        bool bUseMobileGUI = false;
        /*
#if UNITY_EDITOR
        bUseMobileGUI = false;
        */
#if UNITY_ANDROID
        bUseMobileGUI = true;

#elif UNITY_IOS
        bUseMobileGUI = true;

#endif

        return bUseMobileGUI;
    }
}

