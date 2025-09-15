using com.fscigliano.PropertyDrawersCollection;
using UnityEngine;

namespace com.fscigliano.CommonExtensions
{
    /// <summary>
    /// Creation Date:   
    /// Product Name:    Common extensions
    /// Developers:      Franco Scigliano
    /// Description:
    /// </summary>
    public abstract class ScriptableObjectWithDescription : ScriptableObject
    {
        protected virtual string CustomDescription
        {
            get { return null; }
        }
        [SerializeField, Description("CustomDescription")] protected string _description;
    }
}