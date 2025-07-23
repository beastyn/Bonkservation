using System;
using System.Collections;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.UIElements;
using UnityEditor;
#endif


namespace BrainDesigner.Scripts.Utils
{
    using Runtime;

    public class Utils
    {
        internal static string VariableNameToReadable(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
                return string.Empty;

            //Add spaces before upper case
            var spacedName = AddSpacesBeforeUppercase(variableName);

            return char.ToUpper(spacedName[0]) + spacedName.Substring(1);
        }

        internal static string AddSpacesBeforeUppercase(string input)
        {
            return string.Concat(input.Select((c, i) => i > 0 && char.IsUpper(c) ? " " + c : c.ToString()));
        }

#if UNITY_EDITOR
        internal static void ShowHelpBoxInEditor(VisualElement parentContainer, string textToSHow)
        {
            var helpBox = new HelpBox(textToSHow, HelpBoxMessageType.Warning);
            parentContainer.Add(helpBox);
        }
#endif

        public static string GetRelativeAssetPath(string absolutePath)
        {
            // Ensure the absolute path uses forward slashes
            absolutePath = absolutePath.Replace("\\", "/");

            // Get the absolute path to the "Assets" folder
            string assetsPath = Application.dataPath.Replace("\\", "/");




            // Check if the absolute path contains the "Assets" path
            int index = absolutePath.IndexOf(assetsPath);


            // Extract the relative path starting from "Assets"
            if (index >= 0)
                return "Assets" + absolutePath.Remove(index, assetsPath.Length);
            else
            {
                Debug.LogError("The provided path is not within the project's Assets folder.");
                return null;
            }
        }

        /*  internal static string VerifyItemName<T>(string prefix, string newDesignation, IEnumerable<T> collection,
            Func<T, string> getDesignation, string oldName = "")
          {
              var enumerable = collection as T[] ?? collection.ToArray();
              newDesignation = newDesignation.Replace(" ", "").Equals("")
                  ? $"{prefix}{enumerable.Length}"
                  : newDesignation;

              int indexIncrease = 0;
              while (enumerable.Any(item => getDesignation(item).Equals(newDesignation)) &&
                     newDesignation != oldName && indexIncrease < 10000)
              {
                  indexIncrease++;
                  newDesignation = $"{prefix}{enumerable.Length + indexIncrease}";
              }

              return newDesignation;
          }*/

        public static object DeepCopy(object original)
        {
            switch (original)
            {
                case null:
                    return null;
                case Node baseNode:
                    return baseNode.Clone();
                case IList list:
                    {
                        Type listType = original.GetType();

                        IList newList = (IList)Activator.CreateInstance(listType);

                        if (list.Count > 0 && (list[0] is not Behaviour && list[0] is not Task))
                        {
                            foreach (var item in list)
                            {
                                object copiedItem = DeepCopy(item);
                                newList.Add(copiedItem);
                            }
                        }

                        return newList;
                    }
                case ICloneable cloneable:
                    return cloneable.Clone();
                default:
                    return original.GetType().IsValueType ? original : null;
            }
        }
    }
}
