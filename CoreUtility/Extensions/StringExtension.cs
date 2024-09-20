using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;

namespace CoreUtility.Extensions
{
    public static class StringExtension
    {
        public static T ParseEnum<T>(this string value){
            return (T) Enum.Parse(typeof(T), value, true);
        }

        public static string ToNonAlNum (this string text) {
            return Regex.Replace(text, "[^A-Za-z0-9]", "");
        }

        public static string CreateString(this ICollection<string> lines) {
            var str = string.Empty;
            foreach (var line in lines)
                str += line + "\n";

            return str;
        }
        
        #region JsonFormatting
        static string FormatLine(string data) =>  "    " + data + ",";
        static string FormatToJson<T>(string key, T newValue) => $"{key}: {newValue}";
        static string ChangeKeyName(string jsonValue, string key) => ChangeName(jsonValue, "Value", key);
        static string ChangeName(string text, string from, string to) => text.Replace($"\"{from}\"", $"\"{to}\"");
        static string CreateNewValue<T>(string line, T newValue) => FormatToJson(GetKeyFromLine(line), newValue) + ",";
        static string ConvertLineToJson(string line) => "{" + line.Remove(line.Length - 1) + "}";
        static string GetKeyFromLine(string line) => line.Split(':')[0];
        
        #endregion
    }
}