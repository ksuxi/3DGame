using SDL2;
using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

class Renderer
{
    private Game mGame;
    private Camera mCamera;
    private IntPtr mWindow; // SDL�E�B���h�E
    private IntPtr mContext; // SDL�R���e�L�X�g
    private Matrix4 mViewMatrix; // �r���[�ϊ��s��
    private Matrix4 mProjectionMatrix;  // �ˉe�ϊ��s��
    // ���C�e�B���O�p�����[�^
    private Vector3 mAmbientLight;  // ����
    private Vector3 mDirLightDirection;    // ���s���� ����
    private Vector3 mDirLightDiffuseColor; // ���s���� �g�U���ːF kd
    private Vector3 mDirLightSpecColor;    // ���s���� ���ʔ��ːF ks
    // 2DSprite�p�N���X
    private Shader m2DSpriteShader; // �V�F�[�_
    private VertexArray m2DSpriteVertexArray;   // ���_�N���X
    private Matrix4 m2DViewProjection;  // 2D�pView�ϊ��s��

    private List<SpriteComponent> mSpriteComps = new List<SpriteComponent>(); // �A�N�^�̃X�v���C�g���X�g
    private List<MeshComponent> mMeshComps = new List<MeshComponent>(); //  �A�N�^�̃��b�V�����X�g
    private Dictionary<string, Texture> mCashedTextures = new Dictionary<string, Texture>();    // �L���b�V���ς݃e�N�X�`�����X�g
    private Dictionary<string, Mesh> mCashedMeshes = new Dictionary<string, Mesh>();    // �L���b�V���ς݃��b�V�����X�g
    private Dictionary<Shader.MyShaderType, Shader> mCashedShaders = new Dictionary<Shader.MyShaderType, Shader>();   // �L���b�V���ς݃V�F�[�_���X�g
    

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

        // OpenTK�̏�����
        
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
        // �������Ɏ��s������false��Ԃ�
        bool success = SDL.SDL_Init(SDL.SDL_INIT_AUDIO | SDL.SDL_INIT_VIDEO) == 0;
        if (!success) return false;
        //OpenGL�ݒ�
        //�R�AOpenGL�v���t�@�C��
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, 
            SDL.SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);
        // �o�[�W����3.3���w��
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 3);
        // RGBA�e�`�����l��8�r�b�g�̃J���[�o�b�t�@
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_RED_SIZE, 8);
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_GREEN_SIZE, 8);
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_BLUE_SIZE, 8);
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_ALPHA_SIZE, 8);
        // Z�o�b�t�@�̃r�b�g��
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DEPTH_SIZE, 24);
        // �_�u���o�b�t�@
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DOUBLEBUFFER, 1);
        // �n�[�h�E�F�A�A�N�Z�����[�V����
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_ACCELERATED_VISUAL, 1);

        mWindow = SDL.SDL_CreateWindow("OpenGLTest",
               100, 100, (int)Game.ScreenWidth, (int)Game.ScreenHeight, SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);
                if (mWindow == 0) return false;
        // SDL�R���e�L�X�g�̍쐬
        mContext = SDL.SDL_GL_CreateContext(mWindow);
        if (mContext == 0) return false;

        return true;
    }

    public bool LoadData()
    {
        // �r���[�ˉe���W��ݒ�
        mViewMatrix = Calc.CreateLookAt(Calc.VEC3_ZERO, Calc.VEC3_UNIT_Z, Calc.VEC3_UNIT_Y);   // �J�����Ȃ��̏����l
        mProjectionMatrix = Calc.CreatePerspectiveFOV(Calc.ToRadians(50.0f),
            Game.ScreenWidth, Game.ScreenHeight, 25.0f, 10000.0f);

        // �J�����쐬
        mCamera = new Camera(mGame);
        mCamera.SetPosition(new Vector3(0.0f, 0.0f, -700.0f));

        // ���C�e�B���O�p�����[�^�ݒ�
        mAmbientLight = new Vector3(0.35f, 0.35f, 0.35f);
        mDirLightDirection = new Vector3(0.3f, 0.3f, 0.8f);
        mDirLightDiffuseColor = new Vector3(0.8f, 0.9f, 1.0f);
        mDirLightSpecColor = new Vector3(0.8f, 0.8f, 0.8f);

        // 2DSprite�p�V�F�[�_�쐬
        m2DSpriteShader = new Shader(Shader.MyShaderType.SPRITE);
        if (!m2DSpriteShader.Load(mGame))
        {
            return false;
        }

        // 2DSprite�p���_�N���X�쐬 (�O�p�|���S����2)
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

        // 2DSprite�p�r���[�s��擾
        m2DViewProjection = new Matrix4(
            2.0f / Game.ScreenWidth, 0.0f, 0.0f, 0.0f,
            0.0f, 2.0f / Game.ScreenHeight, 0.0f, 0.0f,
            0.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);

        return true;
    }

    public void Draw()
    {
        //  �w�i�F���N���A
        GL.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // z�o�b�t�@�L���A���u�����h����
        GL.Enable(EnableCap.DepthTest);
        GL.Disable(EnableCap.Blend);

        //  ���b�V���`��
        foreach (var meshComp in mMeshComps)
        {
            meshComp.Draw();
        }

        // z�o�b�t�@�����A���u�����h�L��
        GL.Disable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        // �X�v���C�g�`��
        m2DSpriteShader.SetActive();
        m2DSpriteShader.SetViewProjectionUniform(m2DViewProjection);
        m2DSpriteVertexArray.SetActive();
        foreach (var sprite in mSpriteComps)
        {
            sprite.Draw(m2DSpriteShader);
        }

        // �o�b�t�@���X�g�ƃX���b�v
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
        // �L���b�V���ς݂Ȃ�ԋp
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
        //  �L���b�V���ς݂Ȃ�ԋp
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

