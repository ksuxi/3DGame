using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

class MeshComponent : Component
{
    private Mesh mMesh;
    private Shader mShader;
    public MeshComponent(Actor actor) : base(actor)
    {
        mMesh = null;
        mShader = null;
        mActor.GetGame().GetRenderer().AddMeshComp(this);
    }

    public void Draw()
    {
        if (mMesh == null) return;
        if (mShader == null) return;

        // シェーダをアクティブにする
        mShader.SetActive();
        //  ビュー射影行列、ライティングパラメータを設定
        var renderer = mActor.GetGame().GetRenderer();
        mShader.SetViewProjectionUniform(renderer.GetProjectionMatrix() * renderer.GetViewMatrix());
        mShader.SetLightingUniform(renderer);
        //  ワールド座標を設定
        Matrix4 world = mActor.GetWorldTransform();
        mShader.SetWorldTransformUniform(world);

        // ★★★ 以下を追加 ★★★
        // 逆転置行列を計算してシェーダーに渡す
        Matrix4 invTranspose = world;
        invTranspose.Invert();
        invTranspose.Transpose();
        mShader.SetMatrixUniform("uInverseTransposeWorld", invTranspose);

        // テクスチャをアクティブにする
        var texture = mMesh.GetTexture();
        if (texture != null) texture.SetActive();

        //  頂点座標をアクティブにする
        var vertexArray = mMesh.GetVertexArray();
        vertexArray.SetActive();

        //  シェーダを描画する
        GL.DrawElements(
            PrimitiveType.Triangles,
            vertexArray.GetNumIndices(),
            DrawElementsType.UnsignedInt,
            0);

    }

    public void SetMesh(Mesh mesh)
    {
        mMesh = mesh;
    }

    public void SetShader(Shader shader)
    {
        mShader = shader;
    }
}