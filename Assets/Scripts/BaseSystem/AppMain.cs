using Chsopoly.Libs;
using UnityEngine;

namespace Chsopoly.BaseSystem
{
    public class AppMain : SingletonMonoBehaviour<AppMain>
    {
        const int FrameRate = 30;

        void Start ()
        {
            Application.targetFrameRate = FrameRate;
            Screen.sleepTimeout = UnityEngine.SleepTimeout.SystemSetting;
        }
    }
}