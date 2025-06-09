using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SDL2;

class SpriteComponent : Component
{
	private Texture mTexture;   // �e�N�X�`��
	public SpriteComponent(Actor actor) : base(actor)
	{
		mTexture = null;
		mActor.GetGame().GetRenderer().AddSpriteComp(this);
	}

	public void Draw(Shader shader)
	{
		if (mTexture == null) return;

        // �e�N�X�`���T�C�Y���l���������[���h�ϊ����W��ݒ�
        Matrix4 scaleMatrix = Matrix4.CreateScale((float)mTexture.GetWidth(),
            (float)mTexture.GetHeight(), 1.0f);
        Matrix4 world = mActor.GetWorldTransform() * scaleMatrix;
		shader.SetWorldTransformUniform(world);
		// �e�N�X�`�����A�N�e�B�u�ɂ���
		mTexture.SetActive();

        // �ݒ肳�ꂽ�V�F�[�_��`��
        GL.DrawElements(
			PrimitiveType.Triangles,	// �`�悷��|���S���̎��
			6,	// �C���f�b�N�X�o�b�t�@�̐�
			DrawElementsType.UnsignedInt,	// �C���f�b�N�X�̌^
			0);
    }

	public void SetTexture(Texture texture)
	{
		mTexture = texture;		
	}
}