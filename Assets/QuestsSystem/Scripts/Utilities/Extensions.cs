// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using System.Text.RegularExpressions;

namespace QuestsSystem
{
    public static class Extensions
    {
        /// <summary>
        /// Converts incoming text to Pascal format
        /// Removes all spaces and symbols
        /// Hello+_world => HelloWorld
        /// </summary>
        /// <param name="value">String to convert</param>
        /// <returns></returns>
        public static string ToPascal(this string value)
        {
            string result = string.Empty;
            string cleanFromSymbols = Regex.Replace(value, "[^A-Za-z0-9 ]", ""); //Removing all symbols

            string[] questNameSplit = cleanFromSymbols.Split(' '); //Removing spaces

            //Change all first characters ToUpper
            foreach (string nameString in questNameSplit)
            {
                char[] nameChars = nameString.ToCharArray();
                nameChars[0] = char.ToUpper(nameChars[0]);
                result += new string(nameChars);
            }

            return result;
        }
    }
}
