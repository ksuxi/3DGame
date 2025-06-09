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

    private State mState;   // ���
    private Vector3 mPosition;  // �ʒu
    private Vector3 mScale;  // �傫��
    private Quaternion mRotation;   // ��]
    private Matrix4 mWorldTransform;    // ���[���h�ϊ����W
    private bool mRecalculateWorldTransform;    // �Čv�Z�t���O

    private List<Component> mComponents = new List<Component>();    // �ۗL����R���|�[�l���g
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

    // �R���|�[�l���g�X�V����
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

    // ���[���h�ϊ����W�v�Z����
    public void CalculateWorldTransform()
    {
        /*
        if (mRecalculateWorldTransform)
        {
            // �g��k���[����]�[�����s�ړ�
            // ���t�̏��Ԃŏ�Z����
            
            mRecalculateWorldTransform = false;
            
            mWorldTransform = Matrix4.CreateTranslation(mPosition.X, mPosition.Y, mPosition.Z).Transposed();
            mWorldTransform *= Matrix4.CreateFromQuaternion(mRotation);
            mWorldTransform *= Matrix4.CreateScale(mScale.X, mScale.Y, mScale.Z);
            
        }
        */


        if (mRecalculateWorldTransform)
        {
            mRecalculateWorldTransform = false;

            // �@�g��E�k���A�A��]�A�B���s�ړ��̏��ŕϊ�����������
            Matrix4 scaleMat = Matrix4.CreateScale(mScale);
            Matrix4 rotMat = Matrix4.CreateFromQuaternion(mRotation);
            Matrix4 transMat = Matrix4.CreateTranslation(mPosition);

            // ����������: ���s�ړ� * ��] * �g��k��
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