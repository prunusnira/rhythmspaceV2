using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using B83.Image.BMP;

namespace BMSCore
{
    public class Tools
    {
        public static Sprite createSpriteFromFile(string FilePath, float PixelsPerUnit = 0.1f)
        {

            // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference

            Texture2D SpriteTexture = LoadTexture(FilePath);
            Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);

            return NewSprite;
        }

        public static Texture2D LoadTexture(string FilePath)
        {

            // Load a PNG or JPG file from disk to a Texture2D
            // Returns null if load fails

            Texture2D Tex2D;
            byte[] FileData;

            if (File.Exists(FilePath))
            {
                if (FilePath.ToLower().EndsWith(".bmp"))
                {
                    BMPLoader loader = new BMPLoader();
                    BMPImage img = loader.LoadBMP(FilePath);
                    Tex2D = img.ToTexture2D();
                    return Tex2D;
                }
                else
                {
                    FileData = File.ReadAllBytes(FilePath);
                    Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
                    if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                        return Tex2D;                 // If data = readable -> return texture
                }
            }
            return null;                     // Return null if load failed
        }

        public static Sprite TextureToSprite(Texture2D tex, int width, int height, Vector2 pivot)
        {
            return Sprite.Create(tex, new Rect(0, 0, width, height), pivot, 1f);
        }
    }
}
