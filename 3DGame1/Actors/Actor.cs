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
    private Calc.Quaternion mRotation;   // 回転
    private Matrix4 mWorldTransform;    // ワールド変換座標
    private bool mRecalculateWorldTransform;    // 再計算フラグ

    private List<Component> mComponents = new List<Component>();    // 保有するコンポーネント
    public Actor(Game game)
    {
        mState = State.EActive;
        mPosition = Calc.VEC3_ZERO;
        mScale = Calc.VEC3_UNIT;
        mRotation = new Calc.Quaternion();
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
        
        if (mRecalculateWorldTransform)
        {
            // 拡大縮小ー＞回転ー＞平行移動
            // を逆の順番で乗算する
            
            mRecalculateWorldTransform = false;
            
            mWorldTransform = Calc.CreateTranslation(mPosition.X, mPosition.Y, mPosition.Z);
            mWorldTransform *= Calc.Quaternion.CreateQuaternion(mRotation);
            mWorldTransform *= Calc. CreateScale(mScale.X, mScale.Y, mScale.Z);
            
        }
        


        
    }

    public Vector3 GetForward()
    {
        return Calc.Quaternion.RotateVec(Calc.VEC3_UNIT_Z, mRotation);
    }

    public void SetRotationX(float radian)
    {
        Calc.Quaternion q = new Calc.Quaternion(Calc.VEC3_UNIT_X, radian);
        mRotation = Calc.Quaternion.Concatenate(mRotation, q);
        mRecalculateWorldTransform = true;
    }

    public void SetRotationY(float radian)
    {
        Calc.Quaternion q = new Calc.Quaternion(Calc.VEC3_UNIT_Y, radian);
        mRotation = Calc.Quaternion.Concatenate(mRotation, q);
        mRecalculateWorldTransform = true;
    }

    public void SetRotationZ(float radian)
    {
        Calc.Quaternion q = new Calc.Quaternion(Calc.VEC3_UNIT_Z, radian);
        mRotation = Calc.Quaternion.Concatenate(mRotation, q);
        mRecalculateWorldTransform = true;
    }

    public State GetState() { return mState; }

    public Vector3 GetPosition() { return mPosition; }
    public void SetPosition(Vector3 pos) { mPosition = pos; mRecalculateWorldTransform = true; }
    public void SetScale(Vector3 scale) { mScale = scale; mRecalculateWorldTransform = true; }

    public void SetRotation(Calc.Quaternion rotation) { mRotation = rotation; mRecalculateWorldTransform = true; }

    public Matrix4 GetWorldTransform() { return mWorldTransform; }

    public Game GetGame() { return mGame; }



    public void AddComponent(Component component) { mComponents.Add(component); }
}