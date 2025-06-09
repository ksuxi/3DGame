/*

using SDL2;
using Assimp;

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
        var importer = new AssimpContext();
       // シーンの作成、三角ポリゴン化
        var scene = importer.ImportFile(filePath, PostProcessSteps.Triangulate);
        // メッシュの作成
        var mesh = scene.Meshes;

        string fileName = "default_tex.png";
        
        if (scene.MeshCount > 0)
        {
            var material = scene.Materials[0];
            if (material.HasTextureDiffuse)
            {
                // 拡散テクスチャ（Diffuse）を取得
                var texture = material.TextureDiffuse;

                // ファイル名のみ取得
                fileName = System.IO.Path.GetFileName(texture.FilePath);
            }
        }
        mTexture = game.GetRenderer().GetTexture(game.GetAssetsPath() + fileName);

        var uvSetName = mesh[0].TextureCoordinateChannelCount;

        //  頂点座標の読み込み
        List<Vector3D> vertexList = new List<Vector3D>();
        foreach (var vertex in mesh[0].Vertices)
        {
            vertexList.Add(vertex);
        }

        mVertexArray = new VertexArray(ToFloatArray(mesh[0].Vertices), (uint)mesh[0].Vertices.Count,
            Array.ConvertAll(mesh[0].GetIndices(), x => (uint)x),  (uint)mesh[0].GetIndices().Length);

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

*/


using Assimp;
using SDL2;

class Mesh : IDisposable
{
    private Texture mTexture;
    private VertexArray mVertexArray;

    public Mesh()
    {
        mTexture = null;
        mVertexArray = null;
    }

    public bool Load(string filePath, Game game)
    {
        using (var importer = new AssimpContext())
        {
            // FBX/OBJ等を読み込み（Triangulate付き）
            var scene = importer.ImportFile(filePath, PostProcessSteps.Triangulate | PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.FlipUVs);

            if (scene == null || scene.MeshCount == 0)
            {
                SDL.SDL_Log("Failed to load mesh from file: " + filePath);
                return false;
            }

            var mesh = scene.Meshes[0];

            if (!mesh.HasNormals || mesh.TextureCoordinateChannelCount == 0)
            {
                SDL.SDL_Log("Mesh missing normals or UVs.");
                return false;
            }

            // テクスチャ読み込み（最初のマテリアルのみ対応）
            string textureFile = "default_tex.png";
            if (scene.MaterialCount > 0)
            {
                var material = scene.Materials[0];
                if (material.HasTextureDiffuse)
                {
                    textureFile = Path.GetFileName(material.TextureDiffuse.FilePath);
                }
            }

            mTexture = game.GetRenderer().GetTexture(Path.Combine(game.GetAssetsPath(), textureFile));

            // 頂点バッファ生成
            var vertices = BuildVertexArray(mesh);
            var indices = mesh.GetIndices().Select(i => (uint)i).ToArray();

            mVertexArray = new VertexArray(vertices, (uint)(vertices.Length / 8), indices, (uint)indices.Length);
        }

        return true;
    }

    private float[] BuildVertexArray(Assimp.Mesh mesh)
    {
        int vertexCount = mesh.VertexCount;
        float[] data = new float[vertexCount * 8];

        for (int i = 0; i < vertexCount; i++)
        {
            // 位置座標
            data[i * 8 + 0] = mesh.Vertices[i].X;
            data[i * 8 + 1] = mesh.Vertices[i].Y;
            data[i * 8 + 2] = mesh.Vertices[i].Z;

            // 法線
            data[i * 8 + 3] = mesh.Normals[i].X;
            data[i * 8 + 4] = mesh.Normals[i].Y;
            data[i * 8 + 5] = mesh.Normals[i].Z;

            // UV（Yを反転）
            var uv = mesh.TextureCoordinateChannels[0][i];
            data[i * 8 + 6] = uv.X;
            data[i * 8 + 7] = -uv.Y;
        }

        return data;
    }

    public Texture GetTexture()
    {
        return mTexture;
    }

    public VertexArray GetVertexArray()
    {
        return mVertexArray;
    }

    public void Dispose()
    {
        
    }
}
