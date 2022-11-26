using System;
using System.Collections.Generic;

namespace GiraffeStar
{
    /// <summary>
    /// Wrapper to use list collections with JsonUtility library
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ListWrapper<T>
    {
        public List<T> Value;
    }
}