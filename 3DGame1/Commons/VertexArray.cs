using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

class VertexArray
{
    private int mNumVertices;   // 頂点バッファの頂点数
    private int mNumIndices;    // インデックスバッファの頂点数
    private int mVertexBuffer;  // 頂点バッファのOpenGLID
    private int mIndexBuffer;    // インデックスバッファのOpenGLID
    private int mVertexArray;   // 頂点配列オブジェクトのOpenGLID
    
    public VertexArray(float[] vertices, 
        uint numVertices, uint[] indices, uint numIndices)
    {
        mNumVertices = (int)numVertices;
        mNumIndices = (int)numIndices;

        // 頂点配列オブジェクトの生成
        GL.GenVertexArrays(1, out mVertexArray);
        GL.BindVertexArray(mVertexArray);

        // 頂点バッファの作成
        GL.GenBuffers(1, out mVertexBuffer);
        GL.BindBuffer(BufferTarget.ArrayBuffer, mVertexBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer,             //  バッファの種類
                        (int)numVertices * sizeof(float),   // コピーするバイト数(位置(xyz), 法線(xyz), u, v)
                        vertices,                                         // コピー元
                        BufferUsageHint.StaticDraw);        //  データの利用方法

        //  インデックスバッファの作成
        GL.GenBuffers(1, out mIndexBuffer);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, mIndexBuffer);
        GL.BufferData(BufferTarget.ElementArrayBuffer,
                           (int)numIndices * sizeof(uint),
                           indices,
                           BufferUsageHint.StaticDraw);

        // 頂点レイアウトの指定
        // 頂点属性0:   位置(x,y,z)
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, 0);
        //  頂点属性1, 法線(x,y,z)
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, 
            (nint)(sizeof(float)*3));
        //  頂点属性2, u, v
        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, sizeof(float) * 8,
            (nint)(sizeof(float)*6));
    }

    public void SetActive()
    {
        GL.BindVertexArray(mVertexArray);
    }

    public int GetNumIndices() { return mNumIndices; }
}