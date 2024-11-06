using System.ComponentModel;
using System.Reflection;

namespace Labb_BlazorApp.Extensions;

public static class EnumUtils
{
    public static string StringValueOf(this Enum value)
    {
        //This extension is credited to https://waynehartman.com/posts/c-enums-and-string-values.html

        FieldInfo fieldInfo = value.GetType().GetField(value.ToString())!;
        DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attributes.Length > 0)
            return attributes[0].Description;
        else
            return value.ToString();
    }
}