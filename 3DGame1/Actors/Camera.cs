using OpenTK;
using OpenTK.Mathematics;
using SDL2;
using System.Runtime.InteropServices;

class Camera : Actor
{
    Actor mTargetActor;
    public Camera(Game game) : base(game)
    {
        mTargetActor = null;
    }

    public override void UpdateActor(float deltaTime)
    {
        base.UpdateActor(deltaTime);

        // カメラ位置よりビュー座標変換を設定する
        Vector3 position = GetPosition();
        Vector3 target = GetPosition() + 100.0f * GetForward(); // 100.0f前方がターゲット
        if (mTargetActor != null) target = mTargetActor.GetPosition() - GetPosition();  // ターゲットが設定されている場合
        Vector3 up = Calc.VEC3_UNIT_Y;
        Matrix4 viewMatrix = Calc.CreateLookAt(position, target, up);
        GetGame().GetRenderer().SetViewMatrix(viewMatrix);

    }
    
    public override void ProcessInput(nint state)
    {
        base.ProcessInput(state);

        float moveSpeed = 10.0f;
        Vector3 pos = GetPosition();
        int numKeys = (int)SDL.SDL_Scancode.SDL_NUM_SCANCODES;
        byte[] stateArray = new byte[numKeys];
        Marshal.Copy(state, stateArray, 0, numKeys);
        if (stateArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_A] != 0)
        {
            pos.X += moveSpeed;
        }
        else if (stateArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_D] != 0)
        {
            pos.X -= moveSpeed;
        }
        else if (stateArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_W] != 0)
        {
            pos.Y += moveSpeed;
        }
        else if (stateArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_S] != 0)
        {
            pos.Y -= moveSpeed;
        }
        else if (stateArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_UP] != 0)
        {
            pos.Z += moveSpeed;
        }
        else if (stateArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_DOWN] != 0)
        {
            pos.Z -= moveSpeed;
        }
        SetPosition(pos);
    }
}
