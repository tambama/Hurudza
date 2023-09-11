using Hurudza.Common.Utils.Models;
using System.ComponentModel;
using System.Reflection;

namespace Hurudza.Common.Utils.Extensions;

public static class EnumExtensions
{
    /// <summary>
    ///     A generic extension method that aids in converting 
    ///     and retrieving an Enum from a String`.
    /// </summary>
    public static T ToEnum<T>(this string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enumType"></param>
    /// <returns></returns>
    public static List<EnumItem> ToList(this Type enumType)
    {
        List<EnumItem> enumList = new List<EnumItem>();

        foreach (var value in Enum.GetValues(enumType))
        {
            int id = (int)value;
            string name = GetEnumDescription((Enum)value);

            if (!string.IsNullOrEmpty(name))
            {
                enumList.Add(new EnumItem { Id = id, Name = name});
            }
        }

        return enumList;
    }

    /// <summary>
    ///     A generic extension method that aids in reflecting 
    ///     and retrieving any attribute that is applied to an `Enum`.
    /// </summary>
    public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
        where TAttribute : Attribute
    {
        return enumValue.GetType()
            .GetMember(enumValue.ToString())
            .First()
            .GetCustomAttribute<TAttribute>();
    }

    /// <summary>
    ///     A generic extension method that aids in reflecting 
    ///     and retrieving the description on an `Enum`.
    /// </summary>
    public static string GetDescription<T>(this T enumValue)
        where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
            return null;

        var description = enumValue.ToString();
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        if (fieldInfo != null)
        {
            var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attrs != null && attrs.Length > 0)
            {
                description = ((DescriptionAttribute)attrs[0]).Description;
            }
        }

        return description;
    }

    static string GetEnumDescription(Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        if (fieldInfo == null)
            return value.ToString();

        var descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

        if (descriptionAttributes != null && descriptionAttributes.Length > 0)
            return descriptionAttributes[0].Description;

        return value.ToString();
    }

}
