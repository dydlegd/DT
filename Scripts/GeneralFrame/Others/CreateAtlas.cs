using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class CreateAtlas {

    class SpriteEntry
    {
        public Texture2D tex;    // Sprite texture -- original texture or a temporary texture
        public Rect rect;        // Sprite's outer rectangle within the generated texture atlas
        public int minX = 0;    // Padding, if any (set if the sprite is trimmed)
        public int maxX = 0;
        public int minY = 0;
        public int maxY = 0;
        public bool temporaryTexture = false;    // Whether the texture is temporary and should be deleted
    }

    static Shader s_shader = Shader.Find("Unlit/Transparent Colored");
    static Material s_mat = new Material(s_shader);
    static int s_maximumAtlasSize = 2048;
    static TextureFormat s_TextureFormat = TextureFormat.RGBA32;

    static void Init(UIAtlas uIAtlas)
    {
        if (uIAtlas.spriteMaterial == null)
        {
            // 准备材质球    
            uIAtlas.spriteMaterial = s_mat;
        }
    }

    ///
    /// 运行时创建NGUI图集，NGUI2.6.3
    ///
    /// 使用该UIAtlas在运行时创建NGUI图集
    /// 用雨创建图集的多张小图
    public static void CreatAtlasFromTex(UIAtlas uIAtlas, List<Texture> textures)
    {
        if (null == textures || textures.Count <= 0)
        {
            Debug.LogWarning("textures is null or count <= 0 !!");
            return;
        }
        if (null == uIAtlas)
        {
            Debug.LogWarning("uIAtlas is null");
            return;
        }
        else
        {
            Init(uIAtlas);

            // 设定贴图，将小图映射为SpriteEntry
            List<SpriteEntry> sprites = CreateSprites(textures);
            // 将多个小图PackTexture为一张大图，给图集用
            uIAtlas.spriteMaterial.mainTexture = UpdateTexture(uIAtlas, sprites);
            ReplaceSprites(uIAtlas, sprites);
        }
    }

    #region copy UIAtlasMaker编辑器脚本

    static List<SpriteEntry> CreateSprites(List<Texture> textures)
    {
        List<SpriteEntry> list = new List<SpriteEntry>();

        foreach (Texture tex in textures)
        {
            Texture2D oldTex = tex as Texture2D;
            if (oldTex == null) continue;
            // If we want to trim transparent pixels, there is more work to be done
            Color32[] pixels = oldTex.GetPixels32();

            int xmin = oldTex.width;
            int xmax = 0;
            int ymin = oldTex.height;
            int ymax = 0;
            int oldWidth = oldTex.width;
            int oldHeight = oldTex.height;

            // Find solid pixels
            for (int y = 0, yw = oldHeight; y < yw; ++y)
            {
                for (int x = 0, xw = oldWidth; x < xw; ++x)
                {
                    Color32 c = pixels[y * xw + x];

                    if (c.a != 0)
                    {
                        if (y < ymin) ymin = y;
                        if (y > ymax) ymax = y;
                        if (x < xmin) xmin = x;
                        if (x > xmax) xmax = x;
                    }
                }
            }

            int newWidth = (xmax - xmin) + 1;
            int newHeight = (ymax - ymin) + 1;

            // If the sprite is empty, don't do anything with it
            if (newWidth > 0 && newHeight > 0)
            {
                SpriteEntry sprite = new SpriteEntry();
                sprite.rect = new Rect(0f, 0f, oldTex.width, oldTex.height);

                // If the dimensions match, then nothing was actually trimmed
                if (newWidth == oldWidth && newHeight == oldHeight)
                {
                    sprite.tex = oldTex;
                    sprite.temporaryTexture = false;
                }
                else
                {
                    // Copy the non-trimmed texture data into a temporary buffer
                    Color32[] newPixels = new Color32[newWidth * newHeight];

                    for (int y = 0; y < newHeight; ++y)
                    {
                        for (int x = 0; x < newWidth; ++x)
                        {
                            int newIndex = y * newWidth + x;
                            int oldIndex = (ymin + y) * oldWidth + (xmin + x);
                            newPixels[newIndex] = pixels[oldIndex];
                        }
                    }

                    // Create a new texture
                    sprite.temporaryTexture = true;
                    sprite.tex = new Texture2D(newWidth, newHeight);
                    sprite.tex.name = oldTex.name;
                    sprite.tex.SetPixels32(newPixels);
                    sprite.tex.Apply();

                    // Remember the padding offset
                    sprite.minX = xmin;
                    sprite.maxX = oldWidth - newWidth - xmin;
                    sprite.minY = ymin;
                    sprite.maxY = oldHeight - newHeight - ymin;
                }
                list.Add(sprite);
            }
        }
        return list;
    }

    static Texture2D UpdateTexture(UIAtlas atlas, List<SpriteEntry> sprites)
    {

        Texture2D tex = new Texture2D(1, 1, s_TextureFormat, false);
        PackTextures(tex, sprites);
        atlas.spriteMaterial.mainTexture = tex;
        return tex;
    }

    static void PackTextures(Texture2D tex, List<SpriteEntry> sprites)
    {
        Texture2D[] textures = new Texture2D[sprites.Count];
        for (int i = 0; i < sprites.Count; ++i) textures[i] = sprites[i].tex;

        Rect[] rects = tex.PackTextures(textures, 1, s_maximumAtlasSize);

        for (int i = 0; i < sprites.Count; ++i)
        {
            sprites[i].rect = NGUIMath.ConvertToPixels(rects[i], tex.width, tex.height, true);
        }
    }

    static void ReplaceSprites(UIAtlas atlas, List<SpriteEntry> sprites)
    {
        // Get the list of sprites we'll be updating
        List<UISpriteData> spriteList = atlas.spriteList;
        List<UISpriteData> kept = new List<UISpriteData>();

        // The atlas must be in pixels
        //atlas.coordinates = UIAtlas.Coordinates.Pixels;

        // Run through all the textures we added and add them as sprites to the atlas
        for (int i = 0; i < sprites.Count; ++i)
        {
            SpriteEntry se = sprites[i];
            UISpriteData sprite = AddSprite(spriteList, se);
            kept.Add(sprite);
        }

        // Remove unused sprites
        for (int i = spriteList.Count; i > 0; )
        {
            UISpriteData sp = spriteList[--i];
            if (!kept.Contains(sp)) spriteList.RemoveAt(i);
        }
        atlas.MarkSpriteListAsChanged();
    }

    static UISpriteData AddSprite(List<UISpriteData> sprites, SpriteEntry se)
    {
        UISpriteData sprite = null;

        // See if this sprite already exists
        foreach (UISpriteData sp in sprites)
        {
            if (sp.name == se.tex.name)
            {
                sprite = sp;
                break;
            }
        }

        if (sprite != null)
        {
            //float x0 = sprite.inner.xMin - sprite.outer.xMin;
            //float y0 = sprite.inner.yMin - sprite.outer.yMin;
            //float x1 = sprite.outer.xMax - sprite.inner.xMax;
            //float y1 = sprite.outer.yMax - sprite.inner.yMax;

            //sprite.outer = se.rect;
            //sprite.inner = se.rect;

            //sprite.inner.xMin = Mathf.Max(sprite.inner.xMin + x0, sprite.outer.xMin);
            //sprite.inner.yMin = Mathf.Max(sprite.inner.yMin + y0, sprite.outer.yMin);
            //sprite.inner.xMax = Mathf.Min(sprite.inner.xMax - x1, sprite.outer.xMax);
            //sprite.inner.yMax = Mathf.Min(sprite.inner.yMax - y1, sprite.outer.yMax);

            sprite.SetRect((int)se.rect.x, (int)se.rect.y, (int)se.rect.width, (int)se.rect.height);
        }
        else
        {
            sprite = new UISpriteData();
            sprite.name = se.tex.name;
            sprite.SetRect((int)se.rect.x, (int)se.rect.y, (int)se.rect.width, (int)se.rect.height);
            sprites.Add(sprite);
        }

        //float width = Mathf.Max(1f, sprite.outer.width);
        //float height = Mathf.Max(1f, sprite.outer.height);

        //// Sprite's padding values are relative to width and height
        //sprite.paddingLeft = se.minX / width;
        //sprite.paddingRight = se.maxX / width;
        //sprite.paddingTop = se.maxY / height;
        //sprite.paddingBottom = se.minY / height;
        return sprite;
    }

    #endregion copy UIAtlasMaker编辑器脚本
}
