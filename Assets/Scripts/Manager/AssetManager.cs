using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;
using UnityEngine.UI;

public static class AssetManager
{
    static Dictionary<string, SpriteAtlas> m_SpriteAtlas = new Dictionary<string, SpriteAtlas>();

    // //从SpriteAtlas中读取
    // public static Sprite LoadSprite(string atlas_path, string sprite_name)
    // {
    //     atlas_path  = "Atlas/Dynamic/" + atlas_path;
    //     if (!m_SpriteAtlas.ContainsKey(atlas_path)) 
    //     {
    //         m_SpriteAtlas[atlas_path] = Resources.Load<SpriteAtlas>(atlas_path);  //图集名称
    //     }

    //     Sprite spr = m_SpriteAtlas[atlas_path].GetSprite(sprite_name);

    //     return spr;
    // }


    //通过Addressables从图集中 加载 子纹理
    public static void LoadSprite(Image image, string atlas_address, string sprite_name)
    {
        atlas_address   = "Atlas/Dynamic/" + atlas_address;

        if (m_SpriteAtlas.ContainsKey(atlas_address))  {
            image.sprite = m_SpriteAtlas[atlas_address].GetSprite(sprite_name);
            image.SetNativeSize();

            return;
        }

        Addressables.LoadAssetAsync<SpriteAtlas>(atlas_address).Completed += (AsyncOperationHandle<SpriteAtlas> handle)=>{
            if (handle.Status != AsyncOperationStatus.Succeeded) {
                Debug.LogError("Failed to load sprite : " + atlas_address);
                return;
            }

            SpriteAtlas atlas = handle.Result;
            m_SpriteAtlas[atlas_address] = atlas;

            image.sprite = atlas.GetSprite(sprite_name);
            image.SetNativeSize();
        };
    }

    public static void LoadSprite(SpriteRenderer image, string atlas_address, string sprite_name)
    {
        atlas_address   = "Atlas/Dynamic/" + atlas_address;

        if (m_SpriteAtlas.ContainsKey(atlas_address))  {
            image.sprite = m_SpriteAtlas[atlas_address].GetSprite(sprite_name);

            return;
        }

        Addressables.LoadAssetAsync<SpriteAtlas>(atlas_address).Completed += (AsyncOperationHandle<SpriteAtlas> handle)=>{
            if (handle.Status != AsyncOperationStatus.Succeeded) {
                Debug.LogError("Failed to load sprite : " + atlas_address);
                return;
            }

            SpriteAtlas atlas = handle.Result;
            m_SpriteAtlas[atlas_address] = atlas;

            image.sprite = atlas.GetSprite(sprite_name);
        };
    }
}
