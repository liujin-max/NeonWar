using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

public class ExtractTexture : MonoBehaviour
{
    [MenuItem ("Assets/ExtractTexture")]  
    public static void DoExtractTexture()
    {
        string fontPath = @"Assets/Font/PangMenZhengDaoBiaoTiTiMianFeiBan SDF.asset";

        string texturePath = fontPath.Replace(".asset", ".png");
        TMP_FontAsset targetFontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontPath);
        Texture2D texture2D = new Texture2D(targetFontAsset.atlasTexture.width, targetFontAsset.atlasTexture.height, TextureFormat.Alpha8, false);
        Graphics.CopyTexture(targetFontAsset.atlasTexture, texture2D);
        byte[] dataBytes = texture2D.EncodeToPNG();
        FileStream fs = File.Open(texturePath, FileMode.OpenOrCreate);
        fs.Write(dataBytes, 0, dataBytes.Length);
        fs.Flush();
        fs.Close();
        AssetDatabase.Refresh();
        Texture2D atlas = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);

        var matPresets = TMP_EditorUtility.FindMaterialReferences(targetFontAsset);
        targetFontAsset.material.SetTexture(ShaderUtilities.ID_MainTex, atlas);

        foreach (var mat in matPresets)
        {
            mat.SetTexture(ShaderUtilities.ID_MainTex, atlas);
        }

        AssetDatabase.RemoveObjectFromAsset(targetFontAsset.atlasTexture);
        targetFontAsset.atlasTextures[0] = atlas;
        targetFontAsset.material.mainTexture = atlas;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
