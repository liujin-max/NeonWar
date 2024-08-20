using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public static class AssetManager
{
    static Dictionary<string, SpriteAtlas> m_SpriteAtlas = new Dictionary<string, SpriteAtlas>();


    //从SpriteAtlas中读取
    public static Sprite LoadSprite(string atlas_path, string sprite_name)
    {
        atlas_path  = "Atlas/" + atlas_path;
        if (!m_SpriteAtlas.ContainsKey(atlas_path)) 
        {
            m_SpriteAtlas[atlas_path] = Resources.Load<SpriteAtlas>(atlas_path);  //图集名称
        }

        Sprite spr = m_SpriteAtlas[atlas_path].GetSprite(sprite_name);

        return spr;
    }
}
