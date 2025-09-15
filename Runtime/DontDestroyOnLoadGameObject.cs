using UnityEngine;

namespace com.fscigliano.CommonExtensions
{
    /// <summary>
    /// Creation Date:   
    /// Product Name:    Common extensions
    /// Developers:      Franco Scigliano
    /// Description:     Adds DontDestroyOnLoad to a gameObject
    /// </summary>
    public class DontDestroyOnLoadGameObject : MonoBehaviour
    {
        #region Methods
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        #endregion
    }
}