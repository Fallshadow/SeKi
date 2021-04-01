#if UNITY_EDITOR

using System;
using UnityEditor;

namespace ASeKi.action
{
    public class StandaloneFileBrowserEditor : IStandaloneFileBrowser
    {
        public string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
        {
            string path = "";

            if (extensions == null)
            {
                path = EditorUtility.OpenFilePanel(title, directory, "");
            }
            else
            {
                path = EditorUtility.OpenFilePanelWithFilters(title, directory,
                    GetFilterFromFileExtensionList(extensions));
            }

            return string.IsNullOrEmpty(path) ? new string[0] : new[] {path};
        }

        public void OpenFilePanelAsync(string title, string directory, ExtensionFilter[] extensions, bool multiselect,
            Action<string[]> cb)
        {
            cb.Invoke(OpenFilePanel(title, directory, extensions, multiselect));
        }

        public string[] OpenFolderPanel(string title, string directory, bool multiselect)
        {
            var path = EditorUtility.OpenFolderPanel(title, directory, "");
            return string.IsNullOrEmpty(path) ? new string[0] : new[] {path};
        }

        public void OpenFolderPanelAsync(string title, string directory, bool multiselect, Action<string[]> cb)
        {
            cb.Invoke(OpenFolderPanel(title, directory, multiselect));
        }

        public string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions)
        {
            var ext = extensions != null ? extensions[0].Extensions[0] : "";
            var name = string.IsNullOrEmpty(ext) ? defaultName : defaultName + "." + ext;
            return EditorUtility.SaveFilePanel(title, directory, name, ext);
        }

        public void SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions,
            Action<string> cb)
        {
            cb.Invoke(SaveFilePanel(title, directory, defaultName, extensions));
        }

        // EditorUtility.OpenFilePanelWithFilters extension filter format
        private static string[] GetFilterFromFileExtensionList(ExtensionFilter[] extensions)
        {
            string types = "";
            
            for (var index = 0; index < extensions.Length; index++)
            {
                var extension = extensions[index];

                if (index == extensions.Length - 1)
                {
                    types += $"{extension.Name}";
                }
                else
                {
                    types += $"{extension.Name},";
                }
            }

            string[] result = { "Filter Files:", types, "All files", "*" };

            return result;
        }
    }
}

#endif