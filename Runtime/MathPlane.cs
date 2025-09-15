using UnityEngine;

namespace com.fscigliano.CommonExtensions
{
    /// <summary>
    /// Creation Date:   11/02/2020 18:22:20
    /// Product Name:    Common extensions
    /// Developers:      Franco Scigliano
    /// Description:     
    /// </summary>
    public class MathPlane : MonoBehaviour
    {
        #region Inspector Fields

        [Header("Debug"), SerializeField] protected bool _drawDebug = true;
        [SerializeField] protected Vector2 _debugSize = new Vector2(1f, 1f);

        [SerializeField] protected Color _debugColor = new Color(0, 0, 255, 125);

        #endregion

        #region Properties, Consts and Statics

        private Plane _plane;
        private static Camera _mainCamera;

        #endregion

        #region Methods

        private void Awake()
        {
            _plane = new Plane(transform.up, transform.position);
        }

        void OnDrawGizmos()
        {
            if (_drawDebug)
            {
                Quaternion rotation = Quaternion.LookRotation(transform.TransformDirection(transform.up));
                Matrix4x4 trs = Matrix4x4.TRS(transform.TransformPoint(transform.position), rotation, Vector3.one);
                Gizmos.matrix = trs;
                Gizmos.color = _debugColor;
                Gizmos.DrawCube(Vector3.zero, new Vector3(_debugSize.x, _debugSize.y, 0.0001f));
                Gizmos.matrix = Matrix4x4.identity;
                Gizmos.color = Color.white;
            }
        }

        public bool IsMouseInPlane(Vector3 screenPos)
        {
            var sPos = screenPos;
            GetCamera();
            Ray r = _mainCamera.ScreenPointToRay(sPos);
            float enter;
            return (_plane.Raycast(r, out enter));
        }

        public Vector3 GetMousePosition(Vector3 screenPos)
        {
            var sPos = screenPos;
            GetCamera();
            Ray r = _mainCamera.ScreenPointToRay(sPos);
            float enter;
            if (_plane.Raycast(r, out enter))
            {
                Vector3 hitPoint = r.GetPoint(enter);
                return hitPoint;
            }

            return Vector3.zero;
        }

        private void GetCamera()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }
        }

        public void SetWorldPosition(Vector3 wPos)
        {
            transform.position = wPos;
            _plane.SetNormalAndPosition(transform.up, transform.position);
        }

        public void SetWorldRotation(Quaternion rot)
        {
            transform.rotation = rot;
            _plane.SetNormalAndPosition(transform.up, transform.position);
        }

        public void SetWorldPositionRotation(Vector3 wPos, Quaternion rot)
        {
            transform.position = wPos;
            transform.rotation = rot;
            _plane.SetNormalAndPosition(transform.up, transform.position);
        }

        #endregion
    }
}