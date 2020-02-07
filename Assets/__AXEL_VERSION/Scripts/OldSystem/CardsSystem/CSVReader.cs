using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace CSVReader
{
    public class CsvFiles
    {
        private static string currentWord;

        /// <summary>
        /// Get entry from TextAsset
        /// </summary>
        /// <param name="csv"></param>
        /// <param name="letterX"></param>
        /// <param name="lineY"></param>
        /// <returns></returns>
        public static string GetEntry(TextAsset csv, int letterX, int lineY)
        {
            string[] words = csv.text.Split(';');
            string[] lines = csv.text.Split('\n');
            string currentWord = "";

            return currentWord = lines[lineY].Split(';')[letterX];
        }

        /// <summary>
        /// Get an entry from path
        /// </summary>
        /// <param name="csvInternalPath"></param>
        /// <param name="letterX"></param>
        /// <param name="lineY"></param>
        /// <returns></returns>
        public static string GetEntryFromPath(string csvInternalPath, int letterX, int lineY)
        {
            string[] newCsv = File.ReadAllLines(Application.dataPath + csvInternalPath);

            currentWord = newCsv[lineY].Split(';')[letterX];

            return currentWord;
        }

        /// <summary>
        /// Get number of lines according to TextAsset
        /// </summary>
        /// <param name="csv"></param>
        /// <returns></returns>
        public static int GetNumberOfLines(TextAsset csv)
        {
            string[] lines = csv.text.Split('\n');
            int result = lines.Length;
            return result;
        }

        /// <summary>
        /// Get number of lines in the .csv according to path
        /// </summary>
        /// <param name="csvPath"></param>
        /// <returns></returns>
        public static int GetNumberOfLinesByPath(string csvPath)
        {
            string[] lines = File.ReadAllLines(Application.dataPath + csvPath);
            int result = lines.Length;
            return result;
        }

        /// <summary>
        /// Get Texture from TextAsset, in 'Resources' folder
        /// </summary>
        /// <param name="csv"></param>
        /// <param name="letterX"></param>
        /// <param name="lineY"></param>
        /// <returns></returns>
        public static Texture2D GetTexture(TextAsset csv, int letterX, int lineY)
        {
            string imgPath = Application.dataPath + GetEntry(csv, letterX, lineY);
            Debug.Log(imgPath);
            Texture2D tmp = Resources.Load<Texture2D>(imgPath);
            return tmp;
        }

        /// <summary>
        /// Get a sprite from path, in 'Resources' folder
        /// </summary>
        /// <param name="csvPath"></param>
        /// <param name="letterX"></param>
        /// <param name="lineY"></param>
        /// <returns></returns>
        public static Sprite GetSpriteFromPath(string csvPath, int letterX, int lineY)
        {
            string imgPath = "Images/" +  GetEntryFromPath(csvPath, letterX, lineY);
            Sprite tmp = Resources.Load<Sprite>(imgPath);
            return tmp;
        }
        public static Texture2D GetTexture2DFromPath(string csvPath, int letterX, int lineY)
        {
            string imgPath = "Images/" + GetEntryFromPath(csvPath, letterX, lineY);
            Texture2D tmp = Resources.Load<Texture2D>(imgPath);
            return tmp;
        }
    }
}