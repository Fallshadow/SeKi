using System;
using UnityEditor;
using System.Windows.Forms;
using UnityEngine;

namespace ASeKi.Demand
{
    // 找到System.Windows.Forms.dll：在unity的安装目录中找到它，如E:\Program Files (x86)\Unity\Editor\Data\Mono\lib\mono\2.0
    // 任意打开一项目，新建Plugins文件夹，将找到的System.Windows.Forms.dll复制进去
    public static class FreeSelectFolder
    {
        public static void SelectSaveImgFolder(Action<string> pressOk)
        {
            SaveFileDialog saveLog = new SaveFileDialog();
            // 默认路径
            saveLog.InitialDirectory = "c:\\";
            // 设置文件类型
            saveLog.Filter = "Image Files(*.JPG;)|*.JPG;|Image Files(*.PNG)|*.PNG |All files (*.*)|*.*";
            // 设置默认文件名
            saveLog.FileName = "MyFile";
            // 弹窗
            DialogResult result = saveLog.ShowDialog();
            // 确认保存
            if (result == DialogResult.OK)
            {
                string path = saveLog.FileName;
                pressOk?.Invoke(path);
            }
        }
        
        public static string SelectSaveImgFolderForPng()
        {
             string filePath = EditorUtility.SaveFilePanel("Save Png", "", "MyPng", "png");
             return filePath;
        }
    }
}
