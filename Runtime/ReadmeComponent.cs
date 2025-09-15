using UnityEngine;

namespace com.fscigliano.CommonExtensions
{
    /// <summary>
    /// Product Name:    Common extensions
    /// Developers:      Franco Scigliano
    /// Description:     Quick and fast component i did to be able to better visually document this project
    /// Changelog:       
    /// </summary>
    public class ReadmeComponent : MonoBehaviour
    {
        [TextArea(3, 25), SerializeField] private string _description;
    }

}
