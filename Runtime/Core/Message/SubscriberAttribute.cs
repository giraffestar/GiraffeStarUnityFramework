using System;

namespace GiraffeStar
{
    /// <summary>
    /// 메소드에 붙이면 메세지를 받을 수 있다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SubscriberAttribute : Attribute
    {
        public readonly string[] Filters;
        public readonly int Priority;

        /// <summary>
        /// Filter (도메인)을 지정해 일부 Subscriber만 메세지를 받도록 지정할 수 있다.
        /// </summary>
        public SubscriberAttribute(int priority, params string[] filters)
        {
            if (filters == null || filters.Length == 0)
            {
                throw new FrameworkException("Must be under any Filter");
            }
            Filters = filters;
            Priority = priority;
        }
        
        public SubscriberAttribute(params string[] filters) : this(0, filters)
        {
            
        }

        public SubscriberAttribute(int priority) : this(priority, "Default")
        {

        }
        
        public SubscriberAttribute() : this(0, "Default")
        {

        }
    }
}