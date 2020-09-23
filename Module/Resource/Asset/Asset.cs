using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Framework.Module.Resource
{
    public class Asset : IAsset, IEnumerator
    {
        private List<Object> _requires;
        public Type AssetType { get; internal set; }
        public string Name { get; internal set; }
        public LoadState LoadState { get; internal set; }
        public bool IsUnused { get { return RefCount <= 0; } }
        public int RefCount { get; protected set; }
        public virtual bool IsDone { get { return true; } }
        public virtual float Progress { get { return 1; } }
        public virtual string Error { get; protected set; }
        public string Text { get; protected set; }
        public byte[] Bytes { get; protected set; }
        public Object asset { get; internal set; }
        public Action<IAsset> Completed { get; set; }

        public Asset()
        {
            asset = null;
            LoadState = LoadState.Init;
        }

        public void Retain()
        {
            RefCount++;
        }

        public void Release()
        {
            RefCount--;
        }

        bool CheckRequires
        {
            get { return _requires != null; }
        }

        public void Require(Object obj)
        {
            if (_requires == null)
            {
                _requires = new List<Object>();
            } 

            _requires.Add(obj);
            Retain();
        }

        public void Dequire(Object obj)
        {
            if (_requires == null)
            {
                return;
            }  

            if (_requires.Remove(obj))
            {
                Release();
            }  
        }

        void UpdateRequires()
        {
            for (var i = 0; i < _requires.Count; i++)
            {
                var item = _requires[i];
                if (item != null)
                {
                    continue;
                } 
                Release();
                _requires.RemoveAt(i);
                i--;
            }

            if (_requires.Count == 0)
            {
                _requires = null;
            } 
        }

        internal virtual void Load()
        {
            _Load();
        }

        [Conditional("UNITY_EDITOR")]
        void _Load()
        {
            if (Utility.loadDelegate != null)
            {
                asset = Utility.loadDelegate(Name, AssetType);
            }   
        }

        [Conditional("UNITY_EDITOR")]
        void _Unload()
        {
            if (asset == null)
            {
                return;
            }
            if (!(asset is GameObject))
            {
                Resources.UnloadAsset(asset);
            }

            asset = null;
        }

        internal virtual void Unload()
        {
            _Unload();
        }

        internal virtual bool Update()
        {
            if (CheckRequires)
            {
                UpdateRequires();
            }
                
            if (!IsDone)
            {
                return true;
            }
                
            if (Completed == null)
            {
                return false;
            }
                
            try
            {
                Completed.Invoke(this);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            Completed = null;
            return false;
        }

        public bool MoveNext()
        {
            return !IsDone;
        }

        public void Reset()
        {
        }

        public object Current
        {
            get { return null; }
        }
    }
}