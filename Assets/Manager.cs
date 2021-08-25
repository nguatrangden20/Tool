using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SFB;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public TMP_InputField formResource;
    public TMP_InputField targetResource;
    public TMP_InputField fristAsset;

    public GameObject filePrefabs;
    public RawImage outputImage;
    public TextMeshProUGUI outputText;


    public void ShowExplorer(string path)
    {        
        switch (path)
        {
            case "Form Resource":
                var textFormResource = StandaloneFileBrowser.OpenFolderPanel("", "", false);
                if(textFormResource.Length > 0)
                    formResource.text = textFormResource[0];
                break;

            case "Target Resource":
                var textTargetResource = StandaloneFileBrowser.OpenFolderPanel("", "", false);
                if(textTargetResource.Length > 0)
                    targetResource.text = textTargetResource[0];
                break;

            case "Frist Asset":
                var textFristAsset = StandaloneFileBrowser.OpenFolderPanel("", "", false);
                if(textFristAsset.Length > 0)
                    fristAsset.text = textFristAsset[0];
                break;

            default: break;
        }
    }

    FileInfo[] fileInfos;
    public void Refresh()
    {
        var info = new DirectoryInfo(formResource.text);
        fileInfos = info.GetFiles();        
        
        foreach (var file in fileInfos)
        {
            GameObject go = Instantiate(this.filePrefabs, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            go.transform.SetParent(GameObject.Find("Canvas/BackGround/Mid/Left/Scroll View/Viewport/Content").transform, false);
            go.transform.localScale = new Vector3(1,1,1);

            go.gameObject.transform.Find("File Name/Text").GetComponent<TextMeshProUGUI>().text = file.Name;
            go.gameObject.transform.Find("Path/Text").GetComponent<TextMeshProUGUI>().text = file.FullName;  
                     
        }
    }

    public void GetTexture(string path)
    {
        var bytes = File.ReadAllBytes(path);
        var texture = new Texture2D(1,1);
        texture.LoadImage(bytes, false);

        OutputImage(texture);
    }

    public void GetText(string path)
    {
        var text = File.ReadAllText(path);
        
        OutputText(text);
    }


        private void OutputText(string text)
    {
        outputImage.gameObject.SetActive(false);
        outputText.gameObject.SetActive(true);
        
        outputText.text = text;
    }

    private void OutputImage(Texture2D texture2D)
    {
        outputImage.gameObject.SetActive(true);
        outputText.gameObject.SetActive(false);

        outputImage.texture = texture2D;
    }

    public void EncryptFile()
    {
        foreach (var file in fileInfos)
        {
            var assetsBytes = File.ReadAllBytes(file.FullName);

            byte [] encryptBytes = null;

            switch (CheckTypeFile(file))
            {
                case typeFile.Image:
                    encryptBytes = Encrypt(assetsBytes);
                    break;

                case typeFile.Txt:
                    encryptBytes = Encrypt(assetsBytes, assetsBytes.Length);
                    break;
                
                default: break;
            }

            string encryptFilePath = targetResource.text + @"\" + file.Name;
            Debug.Log(encryptFilePath);

            if(encryptBytes != null) WriteByte(encryptBytes, encryptFilePath);
        }
    }

    private void WriteByte(byte[] bytes, string dest)
    {
        var desTemp = dest;
        var ext = Path.GetExtension(desTemp);
        if (!string.IsNullOrEmpty(ext))
        {
            desTemp = desTemp.Replace(ext, ".unity3d");
        }

        File.WriteAllBytes(desTemp, bytes);
    }

    private byte[] Encrypt(byte[] data, int encryptTotal = 1)
    {
        for (var i = 0; i < encryptTotal; i++) 
        {
            data[i] = (byte)~data[i];
        }
        return data;
    }
    
    private typeFile CheckTypeFile(FileInfo file)
    {
        if(file.Name.Contains(".txt")) return typeFile.Txt;
        else return typeFile.Image;
    }

    enum typeFile
    {
        Txt,
        Image
    }
}
