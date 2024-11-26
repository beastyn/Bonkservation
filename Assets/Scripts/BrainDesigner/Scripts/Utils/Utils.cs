using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using Codice.CM.Common;
using System.Runtime.CompilerServices;
using System.Reflection;
using UnityEditor.UIElements;

namespace BrainDesigner.Scripts.Utils
{
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

        internal static void ShowHelpBoxInEditor(VisualElement parentContainer, string textToSHow)
        {
            var helpBox = new HelpBox(textToSHow, HelpBoxMessageType.Warning);
            parentContainer.Add(helpBox);
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
    }
}
