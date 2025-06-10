using SDL2;
using Assimp;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

class Mesh : IDisposable
{
    public Texture mTexture;
    public VertexArray mVertexArray;

    public Mesh()
    {
        mVertexArray = null;
    }

    public void Dispose()
    {

    }

    public bool Load(string filePath, Game game)
    {
        // fbxファイルの読み込み
        var importer = new AssimpContext();
        var scene = importer.ImportFile(filePath,
            PostProcessSteps.Triangulate);
        var mesh = scene.Meshes[0];

        float[] vertices = new float[mesh.VertexCount * 8];
        foreach (Face face in mesh.Faces)
        {
            foreach (var index in face.Indices)
            {
                var vertex = mesh.Vertices[index];
                var normals = mesh.Normals[index];
                var uv = mesh.TextureCoordinateChannels[0][index];
                vertices[index * 8 + 0] = vertex.X;
                vertices[index * 8 + 1] = vertex.Y;
                vertices[index * 8 + 2] = vertex.Z;
                vertices[index * 8 + 3] = normals.X;
                vertices[index * 8 + 4] = normals.Y;
                vertices[index * 8 + 5] = normals.Z;
                vertices[index * 8 + 6] = uv.X;
                vertices[index * 8 + 7] = -uv.Y;
            }
        }
        var indices = mesh.GetUnsignedIndices();
        
        mVertexArray = new VertexArray(vertices, (uint)vertices.Count(), indices, (uint)indices.Length);

        string fileName = "default_tex.png";

        if (scene.Materials[0].GetMaterialTexture(TextureType.Diffuse, 0, out TextureSlot diffuseTexture))
        {
            fileName = diffuseTexture.FilePath;
        }

        mTexture = game.GetRenderer().GetTexture(game.GetAssetsPath() + fileName);

        return true;
    }

    public static float[] ToFloatArray(List<Vector3D> vec)
    {
        float[] res = new float[vec.Count];
        foreach (var v in vec)
        {
            res.Append(v.X);
            res.Append(v.Y);
            res.Append(v.Z);
        }
        return res;
    }

    

    public Texture GetTexture()
    {
        return mTexture;
    }
    public VertexArray GetVertexArray() { return mVertexArray;}
}

