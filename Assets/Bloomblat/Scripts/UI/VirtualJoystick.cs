using UnityEngine;
using UnityEngine.EventSystems;

namespace Bloomblat.UI
{
    /// <summary>
    /// Virtual joystick implementation for mobile controls.
    /// Provides normalized input values similar to Input.GetAxis.
    /// </summary>
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("Joystick Settings")]
        [SerializeField] private float handleRange = 1f;
        [SerializeField] private float deadZone = 0.1f;
        [SerializeField] private bool snapToCenter = true;
        [SerializeField] private float returnSpeed = 10f;
        
        private RectTransform backgroundRect;
        private RectTransform handleRect;
        private Vector2 inputVector;
        private bool isDragging;
        private Vector2 centerPosition;
        private float backgroundRadius;
        
        public Vector2 GetInputVector() => inputVector;
        public float GetHorizontal() => inputVector.x;
        public float GetVertical() => inputVector.y;
        public bool IsDragging => isDragging;
        
        public void Initialize(RectTransform background, RectTransform handle)
        {
            backgroundRect = background;
            handleRect = handle;
            
            centerPosition = backgroundRect.anchoredPosition;
            backgroundRadius = backgroundRect.sizeDelta.x / 2f;
            
            // Ensure handle starts at center
            handleRect.anchoredPosition = centerPosition;
        }
        
        private void Update()
        {
            if (!isDragging && snapToCenter)
            {
                // Smoothly return handle to center when not dragging
                handleRect.anchoredPosition = Vector2.Lerp(
                    handleRect.anchoredPosition,
                    centerPosition,
                    returnSpeed * Time.deltaTime
                );
                
                // Update input vector based on handle position
                UpdateInputVector();
            }
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
            OnDrag(eventData);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
            
            if (snapToCenter)
            {
                // Input will be updated in Update() as handle returns to center
            }
            else
            {
                // Immediately reset input
                inputVector = Vector2.zero;
                handleRect.anchoredPosition = centerPosition;
            }
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging) return;
            
            // Convert screen position to local position
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                backgroundRect, 
                eventData.position, 
                eventData.pressEventCamera, 
                out localPoint))
            {
                // Calculate offset from center
                Vector2 offset = localPoint - centerPosition;
                
                // Clamp to background radius
                float distance = offset.magnitude;
                if (distance > backgroundRadius * handleRange)
                {
                    offset = offset.normalized * backgroundRadius * handleRange;
                }
                
                // Update handle position
                handleRect.anchoredPosition = centerPosition + offset;
                
                // Update input vector
                UpdateInputVector();
            }
        }
        
        private void UpdateInputVector()
        {
            // Calculate input vector based on handle position relative to center
            Vector2 offset = handleRect.anchoredPosition - centerPosition;
            float maxDistance = backgroundRadius * handleRange;
            
            // Normalize to -1 to 1 range
            inputVector = offset / maxDistance;
            
            // Apply dead zone
            if (inputVector.magnitude < deadZone)
            {
                inputVector = Vector2.zero;
            }
            else
            {
                // Remap from deadZone to 1
                float magnitude = (inputVector.magnitude - deadZone) / (1f - deadZone);
                inputVector = inputVector.normalized * magnitude;
            }
            
            // Clamp to unit circle
            inputVector = Vector2.ClampMagnitude(inputVector, 1f);
        }
        
        /// <summary>
        /// Sets the joystick position programmatically
        /// </summary>
        public void SetInputVector(Vector2 input)
        {
            inputVector = Vector2.ClampMagnitude(input, 1f);
            
            // Update handle position based on input
            Vector2 offset = inputVector * backgroundRadius * handleRange;
            handleRect.anchoredPosition = centerPosition + offset;
        }
        
        /// <summary>
        /// Resets the joystick to center position
        /// </summary>
        public void ResetJoystick()
        {
            inputVector = Vector2.zero;
            handleRect.anchoredPosition = centerPosition;
            isDragging = false;
        }
        
        /// <summary>
        /// Gets the angle of the joystick input in degrees (0-360)
        /// </summary>
        public float GetAngle()
        {
            if (inputVector.magnitude < deadZone)
                return 0f;
            
            float angle = Mathf.Atan2(inputVector.y, inputVector.x) * Mathf.Rad2Deg;
            if (angle < 0)
                angle += 360f;
            
            return angle;
        }
        
        /// <summary>
        /// Gets the magnitude of the joystick input (0-1)
        /// </summary>
        public float GetMagnitude()
        {
            return inputVector.magnitude;
        }
        
        /// <summary>
        /// Checks if the joystick is being pressed in a specific direction
        /// </summary>
        public bool IsPressed(Vector2 direction, float threshold = 0.5f)
        {
            return Vector2.Dot(inputVector.normalized, direction.normalized) > threshold && 
                   inputVector.magnitude > deadZone;
        }
        
        /// <summary>
        /// Gets directional input for 4-way movement (up, down, left, right)
        /// </summary>
        public Vector2 GetDirectionalInput()
        {
            if (inputVector.magnitude < deadZone)
                return Vector2.zero;
            
            Vector2 directional = Vector2.zero;
            
            if (Mathf.Abs(inputVector.x) > Mathf.Abs(inputVector.y))
            {
                // Horizontal movement is stronger
                directional.x = inputVector.x > 0 ? 1f : -1f;
            }
            else
            {
                // Vertical movement is stronger
                directional.y = inputVector.y > 0 ? 1f : -1f;
            }
            
            return directional;
        }
        
        /// <summary>
        /// Gets 8-way directional input (includes diagonals)
        /// </summary>
        public Vector2 Get8WayInput()
        {
            if (inputVector.magnitude < deadZone)
                return Vector2.zero;
            
            float angle = GetAngle();
            
            // Convert angle to 8-way direction
            if (angle >= 337.5f || angle < 22.5f)
                return Vector2.right;
            else if (angle >= 22.5f && angle < 67.5f)
                return new Vector2(1, 1).normalized;
            else if (angle >= 67.5f && angle < 112.5f)
                return Vector2.up;
            else if (angle >= 112.5f && angle < 157.5f)
                return new Vector2(-1, 1).normalized;
            else if (angle >= 157.5f && angle < 202.5f)
                return Vector2.left;
            else if (angle >= 202.5f && angle < 247.5f)
                return new Vector2(-1, -1).normalized;
            else if (angle >= 247.5f && angle < 292.5f)
                return Vector2.down;
            else if (angle >= 292.5f && angle < 337.5f)
                return new Vector2(1, -1).normalized;
            
            return Vector2.zero;
        }
        
        private void OnDrawGizmosSelected()
        {
            if (backgroundRect == null) return;
            
            // Draw joystick range
            Gizmos.color = Color.yellow;
            Vector3 worldCenter = backgroundRect.TransformPoint(centerPosition);
            Gizmos.DrawWireSphere(worldCenter, backgroundRadius * handleRange);
            
            // Draw dead zone
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(worldCenter, backgroundRadius * deadZone);
        }
    }
}
