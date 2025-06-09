using OpenTK;
using OpenTK.Mathematics;

class Actor : IDisposable
{
    public enum State
    {
        EActive,
        EDead
    }

    private Game mGame;

    private State mState;   // 状態
    private Vector3 mPosition;  // 位置
    private Vector3 mScale;  // 大きさ
    private Quaternion mRotation;   // 回転
    private Matrix4 mWorldTransform;    // ワールド変換座標
    private bool mRecalculateWorldTransform;    // 再計算フラグ

    private List<Component> mComponents = new List<Component>();    // 保有するコンポーネント
    public Actor(Game game)
    {
        mState = State.EActive;
        mPosition = Calc.VEC3_ZERO;
        mScale = Calc.VEC3_UNIT;
        mRotation = Quaternion.Identity;
        mGame = game;
        mRecalculateWorldTransform = true;

        mGame.AddActor(this);
    }

    public void Dispose()
    {

    }

    public void Update(float deltaTime)
    {
        if (mState == State.EActive)
        {
            CalculateWorldTransform();
            UpdateComponents(deltaTime);
            UpdateActor(deltaTime);
        }
    }

    // コンポーネント更新処理
    public virtual void UpdateComponents(float deltaTime)
    {
        foreach (var component in mComponents)
        {
            component.Update(deltaTime);
        }
    }

    public virtual void UpdateActor(float deltaTime)
    {

    }

    public virtual void ProcessInput(IntPtr state)
    {

    }

    // ワールド変換座標計算処理
    public void CalculateWorldTransform()
    {
        /*
        if (mRecalculateWorldTransform)
        {
            // 拡大縮小ー＞回転ー＞平行移動
            // を逆の順番で乗算する
            
            mRecalculateWorldTransform = false;
            
            mWorldTransform = Matrix4.CreateTranslation(mPosition.X, mPosition.Y, mPosition.Z).Transposed();
            mWorldTransform *= Matrix4.CreateFromQuaternion(mRotation);
            mWorldTransform *= Matrix4.CreateScale(mScale.X, mScale.Y, mScale.Z);
            
        }
        */


        if (mRecalculateWorldTransform)
        {
            mRecalculateWorldTransform = false;

            // ①拡大・縮小、②回転、③平行移動の順で変換を合成する
            Matrix4 scaleMat = Matrix4.CreateScale(mScale);
            Matrix4 rotMat = Matrix4.CreateFromQuaternion(mRotation);
            Matrix4 transMat = Matrix4.CreateTranslation(mPosition);

            // 正しい順番: 平行移動 * 回転 * 拡大縮小
            mWorldTransform = scaleMat * rotMat * transMat;

        }
    }

    public Vector3 GetForward()
    {
        return Vector3.Transform(Calc.VEC3_UNIT_Z, mRotation);
    }

    public void SetRotationX(float radian)
    {
        Quaternion q = new Quaternion(Vector3.UnitX, radian);
        mRotation = mRotation * q;
        mRecalculateWorldTransform = true;
    }

    public void SetRotationY(float radian)
    {
        Quaternion q = new Quaternion(Vector3.UnitY, radian);
        mRotation = mRotation * q;
        mRecalculateWorldTransform = true;
    }

    public void SetRotationZ(float radian)
    {
        Quaternion q = new Quaternion(Vector3.UnitZ, radian);
        mRotation = mRotation * q;
        mRecalculateWorldTransform = true;
    }

    public State GetState() { return mState; }

    public Vector3 GetPosition() { return mPosition; }
    public void SetPosition(Vector3 pos) { mPosition = pos; mRecalculateWorldTransform = true; }
    public void SetScale(Vector3 scale) { mScale = scale; mRecalculateWorldTransform = true; }

    public void SetRotation(Quaternion rotation) { mRotation = rotation; mRecalculateWorldTransform = true; }

    public Matrix4 GetWorldTransform() { return mWorldTransform; }

    public Game GetGame() { return mGame; }



    public void AddComponent(Component component) { mComponents.Add(component); }
}