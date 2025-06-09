using OpenTK;
using OpenTK.Graphics.OpenGL4;
using SDL2;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

class Texture : IDisposable
{
    private int mTextureID;
    private IntPtr mTexture;

    public void Dispose()
    {

    }
    public bool Load(string filePath)
    {
        mTexture = SDL_image.IMG_Load(filePath);
        if (mTexture == 0)
        {
            SDL.SDL_Log("Failed open texture.");
            return false;
        }

        // RGBフォーマットの指定
        int rgbFormat = (int)PixelInternalFormat.Rgb;
        SDL.SDL_Surface surface = Marshal.PtrToStructure<SDL.SDL_Surface>(mTexture);
        SDL.SDL_PixelFormat pixelFormat = Marshal.PtrToStructure<SDL.SDL_PixelFormat>(surface.format);
        if (pixelFormat.BitsPerPixel >= 4) rgbFormat = (int)PixelInternalFormat.Rgba;

        // テクスチャオブジェクトの作成
        GL.GenTextures(1, out mTextureID);
        GL.BindTexture(TextureTarget.Texture2D, mTextureID);
        SDL.SDL_Surface ans = Marshal.PtrToStructure<SDL.SDL_Surface>(mTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)rgbFormat, ans.w, ans.h, 0,
            (PixelFormat)rgbFormat, PixelType.UnsignedByte, ans.pixels);

        // バイリニアフィルタの有効化
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Linear);
        return true;
    }

    public int GetWidth()
    {
        SDL.SDL_Surface surface = Marshal.PtrToStructure<SDL.SDL_Surface>(mTexture);
        if (surface.Equals(null))
        {
            return 0;
        }
        else
        {
            return surface.w;
        }
    }

    public int GetHeight()
    {
        SDL.SDL_Surface surface = Marshal.PtrToStructure<SDL.SDL_Surface>(mTexture);
        if (surface.Equals(null))
        {
            return 0;
        }
        else
        {
            return surface.h;
        }
    }

    public void SetActive()
    {
        GL.BindTexture(TextureTarget.Texture2D, mTextureID);
    }
}