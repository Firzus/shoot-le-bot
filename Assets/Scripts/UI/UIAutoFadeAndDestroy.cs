using UnityEngine;
using UnityEngine.UI;

namespace Project
{
    public class UIAutoFadeAndDestroy : MonoBehaviour
    {
        public float Duration = 0.35f;
        public float ScaleFrom = 1.0f;
        public float ScaleTo = 1.0f;

        private Graphic _graphic;
        private float _elapsed;

        private void Awake()
        {
            _graphic = GetComponent<Graphic>();
            transform.localScale = Vector3.one * ScaleFrom;
        }

        private void Update()
        {
            if (_graphic == null) { Destroy(gameObject); return; }

            _elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(_elapsed / Duration);

            // scale
            float scale = Mathf.Lerp(ScaleFrom, ScaleTo, t);
            transform.localScale = Vector3.one * scale;

            // fade out
            var color = _graphic.color;
            color.a = 1f - t;
            _graphic.color = color;

            if (_elapsed >= Duration)
            {
                Destroy(gameObject);
            }
        }
    }
}


