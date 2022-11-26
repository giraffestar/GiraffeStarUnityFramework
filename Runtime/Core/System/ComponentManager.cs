using System.Collections.Generic;
using UnityEngine;

namespace GiraffeStar
{
    public class ComponentManager
    {
        readonly HashSet<Component> components;

        public ComponentManager()
        {
            components = new HashSet<Component>();
        }

        public void Register(Component comp)
        {
            if (comp == null)
            {
                throw new FrameworkException($"Tried to register non-Component");
            }

            if (components.Contains(comp))
            {
                throw new FrameworkException($"Tried to registered already registered Component : {comp}");
            }

            components.Add(comp);
        }

        public bool Unregister(Component comp)
        {
            if (!components.Contains(comp))
            {
                return false;
            }

            components.Remove(comp);

            return true;
        }

        public void CleanUpDestroyed()
        {
            components.RemoveWhere(x => x.gameObject == null);
        }

        public void CleanUp()
        {
            components.Clear();
        }
    }
}