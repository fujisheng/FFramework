﻿namespace Framework.Service
{
    internal abstract class Service
    {
        internal virtual void OnUpdate() { }
        internal virtual void OnLateUpdate() { }
        internal virtual void OnFixedUpdate() { }
        internal virtual void OnRelease() { }
        internal virtual void OnTearDown() { }
        internal virtual void OnApplicationFocus(bool focus) { }
        internal virtual void OnApplicationPause(bool pause) { }
        internal virtual void OnApplicationQuit() { }
        internal virtual void OnLowMemory() { }
    }
}