using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.fscigliano.CommonExtensions
{
    /// <summary>
    /// Creation Date:   27/02/2020 22:12:07
    /// Product Name:    Common extensions
    /// Developers:      Franco Scigliano
    /// Description:     Basic building block of scriptableObject based systems, provide equal overrides that return
    ///                  equals when two objects of this type have the same name and type. Useful to deal with
    ///                  ScriptableObjects that are in asset bundles and need to appear as the same object
    ///                  to the application.
    /// </summary>
    [CreateAssetMenu(menuName = k_menuPath + nameof(IDAsset), fileName = nameof(IDAsset))]
    public class IDAsset : ScriptableObjectWithDescription, IEquatable<IDAsset>, IEqualityComparer<IDAsset>
    {
        private const string k_menuPath = "fscigliano/";

        private int _guid = -1;
        private string _runtimeName = string.Empty;

        /// <summary>
        /// Resets cached GUID
        /// </summary>
        public virtual void ResetData()
        {
            _guid = -1;
        }

        /// <summary>
        /// Deterministic GUID based on name + type full name
        /// Works the same in editor and runtime
        /// </summary>
        public int guid
        {
            get
            {
                if (_guid == -1)
                {
                    _guid = ComputeDeterministicHash(name + GetType().FullName);
                }
                return _guid;
            }
        }

        public new string name
        {
            get
            {
                if (Application.isEditor) return base.name;
                if (string.IsNullOrEmpty(_runtimeName)) _runtimeName = base.name;
                return _runtimeName;
            }
            set
            {
                base.name = value;
                if (!Application.isEditor) _runtimeName = value;
            }
        }

        #region Equality

        public bool Equals(IDAsset other)
        {
            return other != null && guid == other.guid;
        }

        public override bool Equals(object obj)
        {
            return obj is IDAsset other && Equals(other);
        }

        public override int GetHashCode()
        {
            return guid;
        }

        public static bool operator ==(IDAsset a, IDAsset b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.guid == b.guid;
        }

        public static bool operator !=(IDAsset a, IDAsset b)
        {
            return !(a == b);
        }

        #endregion

        #region IEqualityComparer<IDAsset>

        public bool Equals(IDAsset x, IDAsset y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            return x.guid == y.guid;
        }

        public int GetHashCode(IDAsset obj)
        {
            return obj != null ? obj.guid : 0;
        }

        #endregion

        #region Deterministic Hash

        /// <summary>
        /// FNV-1a 32-bit hash for deterministic GUID
        /// </summary>
        private static int ComputeDeterministicHash(string str)
        {
            unchecked
            {
                const int fnvPrime = 16777619;
                int hash = (int)2166136261;

                foreach (char c in str)
                {
                    hash ^= c;
                    hash *= fnvPrime;
                }

                return hash;
            }
        }

        #endregion
    }
}
