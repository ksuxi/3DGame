class Saikoro : Actor
{
    private Game mGame;
    // ��]�e�X�g�p
    private float testRot = 1.0f;
    public Saikoro(Game game, Shader.MyShaderType type) : base(game)
    {
        //  ���b�V���A�V�F�[�_�̐ݒ�
        var meshComp = new MeshComponent(this);
        var mesh = game.GetRenderer().GetMesh(game.GetAssetsPath() + "saikoro.fbx");
        meshComp.SetMesh(mesh);
        var shader = game.GetRenderer().GetShader(type);
        meshComp.SetShader(shader);
    }

    public override void UpdateActor(float deltaTime)
    {
        base.UpdateActor(deltaTime);

        //  ��]�e�X�g
        SetRotationX(Calc.ToRadians(testRot));
        SetRotationY(Calc.ToRadians(testRot));
    }

    public override void ProcessInput(IntPtr state)
    {

    }
}