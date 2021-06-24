using System;
using System.Collections.Generic;
using UnityEngine;

namespace blu
{
    public abstract class Module : MonoBehaviour
    {
        protected List<Type> _dependancies = new List<Type>();
        public List<Type> Dependancies { get => _dependancies; }
        public Type type;

        public virtual void Initialize()
        {
            return;
        }

        protected virtual void SetDependancies()
        {
            return;
        }

        protected Module()
        {
            type = GetType();
            SetDependancies();
        }
    }
}