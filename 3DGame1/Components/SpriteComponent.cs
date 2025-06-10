using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SDL2;

class SpriteComponent : Component
{
	private Texture mTexture;   // テクスチャ
	public SpriteComponent(Actor actor) : base(actor)
	{
		mTexture = null;
		mActor.GetGame().GetRenderer().AddSpriteComp(this);
	}

	public void Draw(Shader shader)
	{
		if (mTexture == null) return;

        // テクスチャサイズを考慮したワールド変換座標を設定
        Matrix4 scaleMatrix = Matrix4.CreateScale((float)mTexture.GetWidth(),
            (float)mTexture.GetHeight(), 1.0f);
        Matrix4 world = mActor.GetWorldTransform() * scaleMatrix;
		shader.SetWorldTransformUniform(world);
		// テクスチャをアクティブにする
		mTexture.SetActive();

        // 設定されたシェーダを描画
        GL.DrawElements(
			PrimitiveType.Triangles,	// 描画するポリゴンの種類
			6,	// インデックスバッファの数
			DrawElementsType.UnsignedInt,	// インデックスの型
			0);
    }

	public void SetTexture(Texture texture)
	{
		mTexture = texture;		
	}
}