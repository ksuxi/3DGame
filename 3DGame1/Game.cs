using SDL2;
using OpenTK;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;


class Game
{
    public const float ScreenWidth = 1024.0f;
    public const float ScreenHeight = 768.0f;

    private List<Actor> mActors = new List<Actor>();    // アクタリスト
    private List<Actor> mPendingActors = new List<Actor>(); // 待機中のアクタリスト

    private Renderer mRenderer;

    private uint mTicksCount;   // ゲーム時間
    private bool mIsRunning;
    private bool mUpdatingActors;   // アクタ更新中か否か

    private string AssetsPath = "C:\\Users\\skp02\\source\\repos\\3DGame1\\3DGame1\\Assets\\";      // Assetsパス
    private string ShaderPath = "C:\\Users\\skp02\\source\\repos\\3DGame1\\3DGame1\\Shaders\\"; // シェーダーパス

    public Game()
    {
        mTicksCount = 0;
        mIsRunning = true;
        mUpdatingActors = false;
    }

    public bool Initialize()
    {
        mRenderer = new Renderer(this);
        if (!mRenderer.Initialize())
        {
            SDL.SDL_Log("Failed intialize renderer.");
            //mRenderer.Dispose();
            return false;
        }

        mTicksCount = SDL.SDL_GetTicks();

        if (!LoadData())
        {
            SDL.SDL_Log("failed load data.");
            return false;
        }

        return true;
    }

    public bool LoadData()
    {
        if (!mRenderer.LoadData()) return false;
        /*
        var saikoro = new Saikoro(this, Shader.MyShaderType.BASIC);
        saikoro.SetPosition(new Vector3(-120.0f, 100.0f, 0.0f));
        saikoro.SetScale(new Vector3(50.0f, 50.0f, 50.0f));
        */
        var saikoro2 = new Saikoro(this, Shader.MyShaderType.SPRITE);
        saikoro2.SetPosition(new Vector3(120.0f, 100.0f, 0.0f));
        saikoro2.SetScale(new Vector3(50.0f, 50.0f, 50.0f));
        /*
        var saikoro3 = new Saikoro(this, Shader.MyShaderType.LAMBERT);
        saikoro3.SetPosition(new Vector3(-120.0f, -100.0f, 0.0f));
        saikoro3.SetScale(new Vector3(50.0f, 50.0f, 50.0f));
        
        var saikoro4 = new Saikoro(this, Shader.MyShaderType.PHONG);
        saikoro4.SetPosition(new Vector3(120.0f, -100.0f, 0.0f));
        saikoro4.SetScale(new Vector3(50.0f, 50.0f, 50.0f));
        */
        var ui1 = new Actor(this);
        ui1.SetPosition(new Vector3(250.0f, 300.0f, 0.0f));
        ui1.SetScale(new Vector3(0.8f, 0.8f, 0.0f));
        var sc = new SpriteComponent(ui1);
        sc.SetTexture(mRenderer.GetTexture(AssetsPath + "msg_start.png"));

        return true;
    }

    public void RunLoop()
    {
        while (mIsRunning)
        {
            ProcessInput();
            Update();
            GenerateOutput();
        }
    }

    private void Update()
    {
        // 最低16msは待機
        while (!SDL.SDL_TICKS_PASSED(SDL.SDL_GetTicks(), mTicksCount + 16)) ;
        // フレームの経過時間を取得(最大50ms)
        float deltaTime = (SDL.SDL_GetTicks() - mTicksCount) / 1000.0f;
        if (deltaTime > 0.05f)
        {
            deltaTime = 0.05f;
        }
        mTicksCount = SDL.SDL_GetTicks();

        // アクタ更新処理
        mUpdatingActors = true;
        foreach (var actor in mActors)
        {
            actor.Update(deltaTime);
        }
        mUpdatingActors = false;

        // 待機中のアクタを追加
        foreach (var pending in mPendingActors)
        {
            mActors.Add(pending);
        }
        mPendingActors.Clear();

        // 死亡したアクタを破棄
        List<Actor> deadActors = new List<Actor>();
        foreach (var actor in mActors)
        {
            if (actor.GetState() == Actor.State.EDead)
            {
                deadActors.Add(actor);
            }
        }
        foreach (var actor in deadActors)
        {
            actor.Dispose();
        }
    }

    private void ProcessInput()
    {
        SDL.SDL_Event myevent;
        while (SDL.SDL_PollEvent(out myevent) != 0)
        {
            switch (myevent.type)
            {
                case SDL.SDL_EventType.SDL_QUIT:
                    mIsRunning = false;
                    break;
            }
        }

        // キー入力イベント
        IntPtr statePtr = SDL.SDL_GetKeyboardState(out int numKeys);
        byte[] stateArray = new byte[numKeys];
        Marshal.Copy(statePtr, stateArray, 0, numKeys);
        if (stateArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_ESCAPE] != 0)
        {
            mIsRunning = false;
        }

        // 各アクアのイベント
        foreach (var actor in mActors)
        {
            actor.ProcessInput(statePtr);
        }
    }

    private void GenerateOutput()
    {
        mRenderer.Draw();
    }

    public void Shutdown()
    {

    }

    public string GetShaderPath()
    {
        return ShaderPath;
    }

    public string GetAssetsPath()
    {
        return AssetsPath;
    }

    public void AddActor(Actor actor) { mActors.Add(actor); }

    public Renderer GetRenderer() { return mRenderer; }
}
