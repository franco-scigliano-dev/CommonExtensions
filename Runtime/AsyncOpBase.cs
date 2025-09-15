using System;
using UnityEngine;

namespace com.fscigliano.CommonExtensions
{
    /// <summary>
    /// Creation Date:   01/02/2020 22:51:22
    /// Product Name:    Common extensions
    /// Developers:      Franco Scigliano
    /// Description:     
    /// </summary>
    public class AsyncOpBase<T>: CustomYieldInstruction where T:AsyncOpBase<T>
    {
        public Action<string> OnComplete;
        protected bool _isDone = false;

        public bool IsDone
        {
            get { return _isDone; }
            set { _isDone = value;
                OnIsDoneChanged();
            }
        }

        protected virtual void OnIsDoneChanged()
        {
            
        }

        public T SetOnComplete(Action<string> callback)
        {
            OnComplete = callback;
            return this as T;
        }

        public override bool keepWaiting
        {
            get { return !IsDone; }
        }
    }
}