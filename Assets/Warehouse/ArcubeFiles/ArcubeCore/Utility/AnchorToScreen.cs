using UnityEngine;

namespace Arcube
{
    /// <summary>
    /// Anchors a 3D object to a screen edge/corner with percentage-based offset.
    /// </summary>
    public class AnchorToScreen : MonoBehaviour
    {
        public enum ScreenAnchor
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            Center,
            CenterLeft,
            CenterRight
        }

        [Header("Anchor Settings")]
        public ScreenAnchor anchor = ScreenAnchor.TopLeft;
        public Camera mainCamera;
        public float distanceFromCamera = 2f;

        [Header("Offset (Percentage of Screen)")]
        [Tooltip("Horizontal offset in percentage (0.1 = 10% of screen width)")]
        public float xOffsetPercent = 0f;
        [Tooltip("Vertical offset in percentage (0.1 = 10% of screen height)")]
        public float yOffsetPercent = 0f;

        private void Start()
        {
            if (!mainCamera) mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            // Base anchor position in screen space
            Vector2 anchorPos = GetScreenPointFromAnchor(anchor);

            // Calculate percentage-based offset
            float offsetX = xOffsetPercent * Screen.width;
            float offsetY = yOffsetPercent * Screen.height;

            // Adjust based on anchor
            switch (anchor)
            {
                case ScreenAnchor.TopLeft:
                    anchorPos.x += offsetX;
                    anchorPos.y -= offsetY;
                    break;
                case ScreenAnchor.TopRight:
                    anchorPos.x -= offsetX;
                    anchorPos.y -= offsetY;
                    break;
                case ScreenAnchor.BottomLeft:
                    anchorPos.x += offsetX;
                    anchorPos.y += offsetY;
                    break;
                case ScreenAnchor.BottomRight:
                    anchorPos.x -= offsetX;
                    anchorPos.y += offsetY;
                    break;
                case ScreenAnchor.Center:
                    anchorPos.x += offsetX;
                    anchorPos.y += offsetY;
                    break;
                case ScreenAnchor.CenterLeft:
                    anchorPos.x += offsetX;
                    anchorPos.y += offsetY;
                    break;
                case ScreenAnchor.CenterRight:
                    anchorPos.x -= offsetX;
                    anchorPos.y += offsetY;
                    break;
            }

            // Convert to world position
            var screenPosWithDepth = new Vector3(anchorPos.x, anchorPos.y, distanceFromCamera);
            var worldPos = mainCamera.ScreenToWorldPoint(screenPosWithDepth);
            transform.position = worldPos;
        }

        private Vector2 GetScreenPointFromAnchor(ScreenAnchor anchorPoint)
        {
            float w = Screen.width;
            float h = Screen.height;
            return anchorPoint switch
            {
                ScreenAnchor.TopLeft => new Vector2(0, h),
                ScreenAnchor.TopRight => new Vector2(w, h),
                ScreenAnchor.BottomLeft => new Vector2(0, 0),
                ScreenAnchor.BottomRight => new Vector2(w, 0),
                ScreenAnchor.Center => new Vector2(w / 2f, h / 2f),
                ScreenAnchor.CenterLeft => new Vector2(0, h / 2f),
                ScreenAnchor.CenterRight => new Vector2(w, h / 2f),
                _ => new Vector2(w, h)
            };
        }
    }
}
