using UnityEngine;

namespace Framework.Component.UI
{
    [ExecuteInEditMode]
    public class EffectCameraController : MonoBehaviour
    {
        Camera effectCamera;
        public Shader shader;
        Material _material;
        Material material
        {
            get
            {
                if (_material != null)
                {
                    return _material;
                }

                _material = new Material(shader)
                {
                    hideFlags = HideFlags.DontSave
                };
                if (material)
                {
                    return material;
                }

                return null;
            }
        }

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        private void Awake()
        {
            effectCamera = GetComponent<Camera>();
        }

        private void Start()
        {
            effectCamera.transform.localPosition = new Vector3(Screen.width / 2f, Screen.height / 2f, effectCamera.transform.localPosition.z);
            effectCamera.orthographicSize = Screen.height / 2;
        }
    }
}