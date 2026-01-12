using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace VoxelEngine.Graphics.Textures
{
    public class Texture
    {
        public readonly int Handle;
        public Texture(int handle)
        {
            Handle = handle;
        }

        public static int LoadFromFile(string TexturePath)
        {
            int textureID = GL.GenTexture();

            StbImage.stbi_set_flip_vertically_on_load(1);

            using (Stream stream = File.OpenRead(TexturePath))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

                GL.BindTexture(TextureTarget.Texture2D, textureID);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                    image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            }

            //Set Filtering
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            //Set Wrapping
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            
            return textureID;
        }

        public static int LoadFromMemory(byte[] data)
        {
            int textureID = GL.GenTexture();

            StbImage.stbi_set_flip_vertically_on_load(0);

            ImageResult image = ImageResult.FromMemory(data, ColorComponents.RedGreenBlueAlpha);

            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            //Set Filtering
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            //Set Wrapping
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            return textureID;
        }

        public static void Use(Texture texture)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture.Handle);
        }
    }
}
