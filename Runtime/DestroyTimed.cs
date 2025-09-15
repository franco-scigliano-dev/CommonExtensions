using System.Collections;
using UnityEngine;

namespace com.fscigliano.CommonExtensions
{
    /// <summary>
    /// Creation Date:   08/03/2020 21:06:42
    /// Product Name:    Common extensions
    /// Developers:      Franco Scigliano
    /// Description:     Destroy a gameObject by time since it is instantiated
    /// </summary>
    public class DestroyTimed : MonoBehaviour
    {
        [SerializeField] protected float _time;
        private Coroutine destroyCoroutine;
        [SerializeField] protected bool _unscaledTime = false;
        #region Methods

        private void Awake()
        {
            destroyCoroutine = StartCoroutine(doDestroy());
        }

        private IEnumerator doDestroy()
        {
            if (_unscaledTime)
                yield return new WaitForSecondsRealtime(_time);
            else
                yield return new WaitForSeconds(_time);
            destroyCoroutine = null;
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (destroyCoroutine != null)
            {
                StopCoroutine(destroyCoroutine);
            }
        }

        #endregion
    }
}