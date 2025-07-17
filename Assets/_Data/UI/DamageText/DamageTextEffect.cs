using UnityEngine;
using TMPro;

namespace _Data.UI.DamageText
{
    public class DamageTextEffect : MonoBehaviour
    {
        public TextMeshProUGUI textMesh;
        public float moveUpDistance = 1.0f;
        public float duration = 1.0f;
        public float startScale = 0.5f;
        public float endScale = 1.2f;
        public Color startColor = Color.white;
        public Color endColor = new Color(1, 1, 1, 0);

        private Vector3 startPos;
        private Vector3 endPos;
        private float timer;
        private bool isPlaying = false;
        private Color[] colorOptions = new Color[] { Color.white, Color.red, Color.green, Color.blue };

        public void Play(string text, Vector3 worldPosition)
        {
            if (textMesh == null)
                textMesh = GetComponent<TextMeshProUGUI>();
            textMesh.text = text;
            // Random màu: trắng, đỏ, xanh lá, xanh dương
            Color randomColor = colorOptions[Random.Range(0, colorOptions.Length)];
            textMesh.color = randomColor;
            startColor = randomColor; // Để fade out đúng màu
            startPos = worldPosition;
            endPos = worldPosition + Vector3.up * moveUpDistance;
            timer = 0f;
            isPlaying = true;
            transform.position = startPos;
            transform.localScale = Vector3.one * startScale;
        }

        void Update()
        {
            if (!isPlaying) return;
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            // Move up
            transform.position = Vector3.Lerp(startPos, endPos, t);
            // Scale up
            float scale = Mathf.Lerp(startScale, endScale, t);
            transform.localScale = Vector3.one * scale;
            // Fade out
            if (textMesh != null)
                textMesh.color = Color.Lerp(startColor, endColor, t);
            if (timer >= duration)
            {
                isPlaying = false;
                Destroy(gameObject); // Hoặc trả về pool nếu dùng object pool
            }
        }
    }
} 