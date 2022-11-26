using System;

namespace GiraffeStarEditor
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CustomAssetAttribute : Attribute
    {
        public string[] Extensions;

        public CustomAssetAttribute(params string[] extensions)
        {
            Extensions = extensions;
        }
    }
}