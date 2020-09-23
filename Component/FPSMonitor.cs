using UnityEngine;

namespace Framework.Utility
{
    public class FPSMonitor : MonoBehaviour
    {
        public bool Visible;
        public float FPS;
        private float _currTime;
        private float _nextTime;
        private float _frameCount;

        private void Start()
        {
            _nextTime = Time.realtimeSinceStartup + 1;
        }

        private void OnGUI()
        {
            if (!Visible)
            {
                return;
            }

            if (Time.realtimeSinceStartup > _nextTime)
            {
                FPS = Time.frameCount - _frameCount;
                _frameCount = Time.frameCount;
                _nextTime = Time.realtimeSinceStartup + 1;
            }
            GUI.skin.label.fontSize = 24;
            GUILayout.Label(string.Format("<color=white>FPS : {0}</color> ", FPS));
        }
    }
}