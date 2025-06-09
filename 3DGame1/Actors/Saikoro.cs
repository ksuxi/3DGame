class Saikoro : Actor
{
    private Game mGame;
    // 回転テスト用
    private float testRot = 1.0f;
    public Saikoro(Game game, Shader.MyShaderType type) : base(game)
    {
        //  メッシュ、シェーダの設定
        var meshComp = new MeshComponent(this);
        var mesh = game.GetRenderer().GetMesh(game.GetAssetsPath() + "saikoro.fbx");
        meshComp.SetMesh(mesh);
        var shader = game.GetRenderer().GetShader(type);
        meshComp.SetShader(shader);
    }

    public override void UpdateActor(float deltaTime)
    {
        base.UpdateActor(deltaTime);

        //  回転テスト
        SetRotationX(Calc.ToRadians(testRot));
        SetRotationY(Calc.ToRadians(testRot));
    }

    public override void ProcessInput(IntPtr state)
    {

    }
}