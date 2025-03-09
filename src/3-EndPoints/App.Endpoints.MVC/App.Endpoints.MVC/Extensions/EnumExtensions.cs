﻿using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace App.Endpoints.MVC.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                ?.Name ?? enumValue.ToString();
        }
    }
}
