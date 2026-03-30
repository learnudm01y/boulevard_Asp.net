using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.Helper
{
    public class MatchObjectHelper
    {
        public static dynamic GenerateMatchedObject(dynamic parent, dynamic child)
        {
            var childProperties = child.GetType().GetProperties();
            var parentProperties = parent.GetType().GetProperties();

            foreach (var parentProperty in parentProperties)
            {
                object parentPropertyValue = null;
                foreach (var childProperty in childProperties)
                {
                    if (parentProperty.Name == childProperty.Name)
                    {
                        if (parentProperty.PropertyType == childProperty.PropertyType)
                        {
                            parentPropertyValue = parentProperty.GetValue(parent);
                            childProperty.SetValue(child, parentPropertyValue);
                            break;
                        }
                    }
                }
            }
            return child;
        }
    }
}