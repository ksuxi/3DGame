using SDL2;
using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

class Renderer
{
    private Game mGame;
    private Camera mCamera;
    private IntPtr mWindow; // SDLウィンドウ
    private IntPtr mContext; // SDLコンテキスト
    private Matrix4 mViewMatrix; // ビュー変換行列
    private Matrix4 mProjectionMatrix;  // 射影変換行列
    // ライティングパラメータ
    private Vector3 mAmbientLight;  // 環境項
    private Vector3 mDirLightDirection;    // 平行光源 向き
    private Vector3 mDirLightDiffuseColor; // 平行光源 拡散反射色 kd
    private Vector3 mDirLightSpecColor;    // 平行光源 鏡面反射色 ks
    // 2DSprite用クラス
    private Shader m2DSpriteShader; // シェーダ
    private VertexArray m2DSpriteVertexArray;   // 頂点クラス
    private Matrix4 m2DViewProjection;  // 2D用View変換行列

    private List<SpriteComponent> mSpriteComps = new List<SpriteComponent>(); // アクタのスプライトリスト
    private List<MeshComponent> mMeshComps = new List<MeshComponent>(); //  アクタのメッシュリスト
    private Dictionary<string, Texture> mCashedTextures = new Dictionary<string, Texture>();    // キャッシュ済みテクスチャリスト
    private Dictionary<string, Mesh> mCashedMeshes = new Dictionary<string, Mesh>();    // キャッシュ済みメッシュリスト
    private Dictionary<Shader.MyShaderType, Shader> mCashedShaders = new Dictionary<Shader.MyShaderType, Shader>();   // キャッシュ済みシェーダリスト
    

    public Renderer(Game game)
    {
        mGame = game;
        mWindow = IntPtr.Zero;
        mAmbientLight = Calc.VEC3_ZERO;
        mDirLightDirection = Calc.VEC3_ZERO;
        mDirLightDiffuseColor = Calc.VEC3_ZERO;
        mDirLightSpecColor = Calc.VEC3_ZERO;
        m2DSpriteShader = null;
        m2DSpriteVertexArray = null;
    }

    public bool Initialize()
    {
        if (!InitSDL())
        {
            SDL.SDL_Log(SDL. SDL_GetError());
            return false;
        }

        // OpenTKの初期化
        
        GL.LoadBindings(new SDLBindingsContext());

        return true;
    }

    public class SDLBindingsContext : IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            return SDL.SDL_GL_GetProcAddress(procName);
        }
    }

    private bool InitSDL()
    {
        // 初期化に失敗したらfalseを返す
        bool success = SDL.SDL_Init(SDL.SDL_INIT_AUDIO | SDL.SDL_INIT_VIDEO) == 0;
        if (!success) return false;
        //OpenGL設定
        //コアOpenGLプロファイル
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, 
            SDL.SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);
        // バージョン3.3を指定
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 3);
        // RGBA各チャンネル8ビットのカラーバッファ
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_RED_SIZE, 8);
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_GREEN_SIZE, 8);
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_BLUE_SIZE, 8);
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_ALPHA_SIZE, 8);
        // Zバッファのビット数
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DEPTH_SIZE, 24);
        // ダブルバッファ
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DOUBLEBUFFER, 1);
        // ハードウェアアクセラレーション
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_ACCELERATED_VISUAL, 1);

        mWindow = SDL.SDL_CreateWindow("OpenGLTest",
               100, 100, (int)Game.ScreenWidth, (int)Game.ScreenHeight, SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);
                if (mWindow == 0) return false;
        // SDLコンテキストの作成
        mContext = SDL.SDL_GL_CreateContext(mWindow);
        if (mContext == 0) return false;

        return true;
    }

    public bool LoadData()
    {
        // ビュー射影座標を設定
        mViewMatrix = Calc.CreateLookAt(Calc.VEC3_ZERO, Calc.VEC3_UNIT_Z, Calc.VEC3_UNIT_Y);   // カメラなしの初期値
        mProjectionMatrix = Calc.CreatePerspectiveFOV(Calc.ToRadians(50.0f),
            Game.ScreenWidth, Game.ScreenHeight, 25.0f, 10000.0f);

        // カメラ作成
        mCamera = new Camera(mGame);
        mCamera.SetPosition(new Vector3(0.0f, 0.0f, -700.0f));

        // ライティングパラメータ設定
        mAmbientLight = new Vector3(0.35f, 0.35f, 0.35f);
        mDirLightDirection = new Vector3(0.3f, 0.3f, 0.8f);
        mDirLightDiffuseColor = new Vector3(0.8f, 0.9f, 1.0f);
        mDirLightSpecColor = new Vector3(0.8f, 0.8f, 0.8f);

        // 2DSprite用シェーダ作成
        m2DSpriteShader = new Shader(Shader.MyShaderType.SPRITE);
        if (!m2DSpriteShader.Load(mGame))
        {
            return false;
        }

        // 2DSprite用頂点クラス作成 (三角ポリゴン＊2)
        float[] vertices =
        {
            -0.5f, 0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, // top left
             0.5f, 0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, // top right
             0.5f,-0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, // bottom right
            -0.5f,-0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f  // bottom left
        };

        uint[] indices =
        {
            0, 1, 2,
            2, 3, 0
        };

        m2DSpriteVertexArray = new VertexArray(vertices, 4, indices, 6);

        // 2DSprite用ビュー行列取得
        m2DViewProjection = new Matrix4(
            2.0f / Game.ScreenWidth, 0.0f, 0.0f, 0.0f,
            0.0f, 2.0f / Game.ScreenHeight, 0.0f, 0.0f,
            0.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);

        return true;
    }

    public void Draw()
    {
        //  背景色をクリア
        GL.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // zバッファ有効、αブレンド無効
        GL.Enable(EnableCap.DepthTest);
        GL.Disable(EnableCap.Blend);

        //  メッシュ描画
        foreach (var meshComp in mMeshComps)
        {
            meshComp.Draw();
        }

        // zバッファ無向、αブレンド有効
        GL.Disable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        // スプライト描画
        m2DSpriteShader.SetActive();
        m2DSpriteShader.SetViewProjectionUniform(m2DViewProjection);
        m2DSpriteVertexArray.SetActive();
        foreach (var sprite in mSpriteComps)
        {
            sprite.Draw(m2DSpriteShader);
        }

        // バッファリストとスワップ
        SDL.SDL_GL_SwapWindow(mWindow);
    }

    public Texture GetTexture(string filePath)
    {
        if (mCashedTextures.ContainsKey(filePath)) return mCashedTextures[filePath];

        Texture texture = new Texture();
        if (texture.Load(filePath))
        {
            mCashedTextures.Add(filePath, texture); 
        }
        else
        {
            texture.Dispose();
            texture = null;
        }

        return texture;
    }

    public Shader GetShader(Shader.MyShaderType type)
    {
        // キャッシュ済みなら返却
        if (mCashedShaders.ContainsKey(type)) return mCashedShaders[type];

        Shader shader = new Shader(type);
        if (shader.Load(mGame))
        {
            shader.SetViewProjectionUniform(mProjectionMatrix * mViewMatrix);
            mCashedShaders.Add(type, shader);
        }
        else
        {

        }
        return shader; 
    }

    public Mesh GetMesh(string filePath)
    {
        //  キャッシュ済みなら返却
        if (mCashedMeshes.ContainsKey(filePath)) return mCashedMeshes[filePath];

        Mesh mesh = new Mesh();
        if (mesh.Load(filePath, mGame))
        {
            mCashedMeshes.Add(filePath, mesh);
        }
        else
        {
            mesh.Dispose();
            mesh = null;
        }

        return mesh;
    }

    public void AddSpriteComp(SpriteComponent sprite) { mSpriteComps.Add(sprite); }
    public void AddMeshComp(MeshComponent mesh) {  mMeshComps.Add(mesh); }

    public Matrix4 GetProjectionMatrix() { return mProjectionMatrix; }

    public void SetViewMatrix(Matrix4 matrix) { mViewMatrix = matrix; }

    public Matrix4 GetViewMatrix() { return mViewMatrix; }

    public Vector3 GetAmbientLight() { return mAmbientLight; }

    public Vector3 GetDirLightDirection() { return mDirLightDirection; }

    public Vector3 GetDirLightDiffuseColor() { return mDirLightDiffuseColor; }

    public Vector3 GetDirLightSpecColor() { return mDirLightSpecColor; }

    public Camera GetCamera() { return mCamera; }

    
}

