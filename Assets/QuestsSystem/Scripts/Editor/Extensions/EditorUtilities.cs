// Quests system
// Created by SoloQ - https://soloq-dev.wixsite.com/main

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QuestsSystem.CustomEditor
{
    public class EditorUtilities
    {
        /// <summary>
        /// Finding a file in a project
        /// </summary>
        /// <param name="fileName">File name to find</param>
        /// <param name="format">File format ".format"</param>
        /// <param name="outFilePath">Out string to get result path</param>
        /// <returns>If file exist: True</returns>
        public static bool TryGetFileInProject(string fileName, string format, out string outFilePath)
        {
            outFilePath = string.Empty;
            string searchPattern = $"*{format}*";

            string[] files = Directory.GetFiles(Application.dataPath, searchPattern, SearchOption.AllDirectories); //Get all project files

            //Find file by name and format in array
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Name == $"{fileName}{format}")
                {
                    outFilePath = file;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Getting a file in a project
        /// </summary>
        /// <param name="fileName">File name to find</param>
        /// <param name="format">File format ".format"</param>
        /// <returns>If file exist: File path</returns>
        public static string GetFileInProject(string fileName, string format)
        {
            if (TryGetFileInProject(fileName, format, out string file))
            {
                return file;
            }
            return null;
        }

        /// <summary>
        /// Finding a script file (.cs) in a project
        /// </summary>
        /// <param name="fileName">Script name to find</param>
        /// <param name="outFilePath">Out string to get result path</param>
        /// <returns>If file exist: True</returns>
        public static bool TryGetScriptFileInProject(string fileName, out string outFilePath)
        {
            return TryGetFileInProject(fileName, ".cs", out outFilePath);
        }

        /// <summary>
        /// Find unity asset in a project
        /// </summary>
        /// <typeparam name="T">Type of asset</typeparam>
        /// <param name="assetName">Asset name to find</param>
        /// <param name="format">Asset format for search pattern</param>
        /// <returns></returns>
        public static string FindAssetPath<T>(string assetName, string format)
        {
            string[] assetsGUID = AssetDatabase.FindAssets("t:" + typeof(T)); //Find all assets by using format as pattern

            foreach (string asset in assetsGUID)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(asset); //Convert GUID to string path
                string[] splitedPath = assetPath.Split('/');

                //Get name of asset and check
                if (splitedPath.Last() == (assetName + format))
                {
                    return assetPath;
                }
            }

            return null;
        }

        /// <summary>
        /// Getting all quests configs in project
        /// </summary>
        /// <returns>List of all quests configs</returns>
        public static List<QuestConfig> GetAllQuestsConfigs()
        {
            List<QuestConfig> result = new List<QuestConfig>();

            string[] configsGUID = AssetDatabase.FindAssets("t:" + typeof(QuestConfig).Name); //Find all assets with QuestConfig type

            for (int i = 0; i < configsGUID.Length; i++)
            {
                string configAssetPath = AssetDatabase.GUIDToAssetPath(configsGUID[i]); //Convert GUID to string path
                QuestConfig config = AssetDatabase.LoadAssetAtPath<QuestConfig>(configAssetPath);
                result.Add(config);
            }

            return result;
        }
    }
}