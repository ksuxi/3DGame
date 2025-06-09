using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SDL2;
using System.Dynamic;

class Shader
{
    private MyShaderType mType; // シェーダタイプ
    // シェーダのIDを格納
    private int mVertexShader;
    private int mFlagShader;
    private int mShaderProgram;
    public enum MyShaderType
    {
        BASIC,
        SPRITE,
        LAMBERT,
        PHONG,
    };

    private const string UNIFORM_VIEW_PROJECTION_NAME = "uViewProjection";
    private const string UNIFORM_WORLD_TRANSFORM_NAME = "uWorldTransform";
    private const string UNIFORM_CAMERA_POS = "uCameraPos";
    private const string UNIFORM_AMBIENT_COLOR = "uAmbientColor";
    private const string UNIFORM_DIR_LIGHT_DIRECTION = "uDirLight.mDirection";
    private const string UNIFORM_DIR_LIGHT_DIFFUSE_COLOR = "uDirLight.mDiffuseColor";
    private const string UNIFORM_DIR_LIGHT_SPEC_COLOR = "uDirLight.mSpecColor";
    private const string UNIFORM_SPEC_POWER = "uSpecPower";

    private float mSpecPower;   // 鏡面反射指数
    public Shader(MyShaderType type, float specPower = 10.0f)
    {
        mType = type;
        mVertexShader = 0;
    }

    public bool Load(Game game)
    {
        if (!CompileShader(game.GetShaderPath() + GetVertFileName(),
            ShaderType.VertexShader, out mVertexShader)
            || !CompileShader(game.GetShaderPath() + GetFlagFileName(),
            ShaderType.FragmentShader, out mFlagShader))
        {
            return false;
        }

        // 頂点シェーダ、フラグメントシェーダをリンクして
        // シェーダプログラムを作成
        mShaderProgram = GL.CreateProgram();
        GL.AttachShader(mShaderProgram, mVertexShader);
        GL.AttachShader(mShaderProgram, mFlagShader);
        GL.LinkProgram(mShaderProgram);

        GL.GetProgram(mShaderProgram, GetProgramParameterName.LinkStatus, out int status);
        if (status == 0)
        {
            return false;
        }

        return true;
    }

    public void SetActive()
    {
        GL.UseProgram(mShaderProgram);
    }

    public void SetWorldTransformUniform(Matrix4 world)
    {
        SetMatrixUniform(UNIFORM_WORLD_TRANSFORM_NAME, world);
    }

    public void SetViewProjectionUniform(Matrix4 viewProjection)
    {
        SetMatrixUniform(UNIFORM_VIEW_PROJECTION_NAME, viewProjection);
    }

    // ライティングuniform設定
    public void SetLightingUniform(Renderer renderer)
    {
        switch (mType)
        {
            case MyShaderType.BASIC:
            case MyShaderType.SPRITE:
                //  設定しない
                break;
            case MyShaderType.LAMBERT:
                SetVectorUniform(UNIFORM_AMBIENT_COLOR, renderer.GetAmbientLight());
                SetVectorUniform(UNIFORM_DIR_LIGHT_DIRECTION, renderer.GetDirLightDirection());
                SetVectorUniform(UNIFORM_DIR_LIGHT_DIFFUSE_COLOR, renderer.GetDirLightDiffuseColor());
                break;
            case MyShaderType.PHONG:
                SetVectorUniform(UNIFORM_CAMERA_POS, renderer.GetCamera().GetPosition());
                SetVectorUniform(UNIFORM_AMBIENT_COLOR, renderer.GetAmbientLight());
                SetVectorUniform(UNIFORM_DIR_LIGHT_DIRECTION, renderer.GetDirLightDirection());
                SetVectorUniform(UNIFORM_DIR_LIGHT_DIFFUSE_COLOR, renderer.GetDirLightDiffuseColor());
                SetVectorUniform(UNIFORM_DIR_LIGHT_SPEC_COLOR, renderer.GetDirLightSpecColor());
                SetFloatUniform(UNIFORM_SPEC_POWER, mSpecPower);
                break;
        }
    }
    public bool CompileShader(string filePath, ShaderType shaderType, out int outShader)
    {
        try
        {
            // ファイルを開く
            string shaderFile = File.ReadAllText(filePath);
            // 全てのテキストを一つの文字列に読み込む

            // 指定されたタイプのシェーダを作成
            outShader = GL.CreateShader(shaderType);
            GL.ShaderSource(outShader, shaderFile);
            GL.CompileShader(outShader);

            // コンパイルが成功したかチェック
            GL.GetShader(outShader, ShaderParameter.CompileStatus, out int status);
            if (status != (int)All.True)
            {
                string infolog = GL.GetShaderInfoLog(outShader);
                throw new Exception( infolog);
            }

            GL.GetShaderSource(outShader, 10000, out int len, out string source);
            Console.WriteLine(source);

            return true;
        }
        catch (Exception ex)
        {
            outShader = 0;
            SDL.SDL_Log("Failed compile shader : " + ex.Message);
                    return false;
        }
    }

    private string GetVertFileName()
    {
        string fileName = null;
        switch (mType)
        {
            case MyShaderType.BASIC:
                fileName = "BasicVert.glsl";
                break;
            case MyShaderType.SPRITE:
                fileName = "SpriteVert.glsl";
                break;
            case MyShaderType.LAMBERT:
                fileName = "LambertVert.glsl";
                break;
            case MyShaderType.PHONG:
                fileName = "PhongVert.glsl";
                break;
        }
        return fileName;
    }

    private string GetFlagFileName()
    {
        string fileName = null;
        switch (mType)
        {
            case MyShaderType.BASIC:
                fileName = "BasicFrag.glsl";
                break;
            case MyShaderType.SPRITE:
                fileName = "SpriteFrag.glsl";
                break;
            case MyShaderType.LAMBERT:
                fileName = "LambertFrag.glsl";
                break;
            case MyShaderType.PHONG:
                fileName = "PhongFrag.glsl";
                break;
        }
        return fileName;
    }

    /*
    public void SetMatrixUniform(string name, Matrix4 matrix)
    {
        int location = GL.GetUniformLocation(mShaderProgram, name);
        
        float[] ans = new float[]
        {
            matrix.M11, matrix.M12, matrix.M13, matrix.M14,
            matrix.M21, matrix.M22, matrix.M23, matrix.M24,
            matrix.M31, matrix.M32, matrix.M33, matrix.M34,
            matrix.M41, matrix.M42, matrix.M43, matrix.M44,
        };
        
        float[] ans = new float[]
        {
            matrix.M11, matrix.M21, matrix.M31, matrix.M41,
            matrix.M12, matrix.M22, matrix.M32, matrix.M42,
            matrix.M13, matrix.M23, matrix.M33, matrix.M43,
            matrix.M14, matrix.M24, matrix.M34, matrix.M44,
        };
        GL.UniformMatrix4(location, 1, true, ans);
    }
    */

    // Shader.cs 内
    public void SetMatrixUniform(string name, Matrix4 matrix)
    {
        int location = GL.GetUniformLocation(mShaderProgram, name);

        // GL.UniformMatrix4に直接マトリクスを渡す (第2引数は転置するかどうか)
        GL.UniformMatrix4(location, false, ref matrix);
    }

    public void SetVectorUniform(string name, Vector3 vector)
    {
        int location = GL.GetUniformLocation(mShaderProgram, name);
        GL.Uniform3(location, vector);
    }

    public void SetFloatUniform(string name, float value) {
        int location = GL.GetUniformLocation(mShaderProgram, name);
        GL.Uniform1(location, value);
    }
}