using OpenTK;
using OpenTK.Mathematics;

class Calc
{
    public static float ToRadians(float degrees)
    {
        return degrees * (float)Math.PI / 180.0f;
    }

    public static Matrix4 LookAt_ForwardZ(Vector3 eye, Vector3 target, Vector3 up)
    {
        // Z+方向（前方ベクトル）
        var z = Vector3.Normalize(target - eye); // ← 変更箇所（OpenTKの eye - target とは逆）

        // 右方向ベクトル X
        var x = Vector3.Normalize(Vector3.Cross(up, z)); // 右手系：up × forward
        var y = Vector3.Normalize(Vector3.Cross(z, x));  // 上方向ベクトル Y

        // 行列構築（列優先風に書く）
        Matrix4 result = new Matrix4();

        result.Row0 = new Vector4(x.X, y.X, z.X, 0);
        result.Row1 = new Vector4(x.Y, y.Y, z.Y, 0);
        result.Row2 = new Vector4(x.Z, y.Z, z.Z, 0);
        result.Row3 = new Vector4(
            -Vector3.Dot(x, eye),
            -Vector3.Dot(y, eye),
            -Vector3.Dot(z, eye),
            1);

        return result;
    }

    public static Matrix4 CreatePerspectiveFOV(float fov, float width, float height,
                                        float near, float far)
    {
        float yScale = 1.0f / (float)Math.Tan(fov / 2.0f);
        float xScale = yScale * height / width;

        Matrix4 temp = new Matrix4(
             xScale, 0.0f,   0.0f, 0.0f,
             0.0f,   yScale, 0.0f, 0.0f ,
             0.0f,   0.0f,   far / (far - near), -near * far / (far - near),
             0.0f,   0.0f,   1.0f, 0.0f 
            );
        return Matrix4.Transpose(temp);
    }

    public static Matrix4 CreateLookAt(Vector3 eye, Vector3 target, Vector3 up)
    {
        Vector3 k = Vector3.Normalize(target - eye);
        Vector3 i = Vector3.Normalize(Vector3.Cross(up, k));
        Vector3 j = Vector3.Normalize(Vector3.Cross(k, i));
        Vector3 t;
        t.X = -Vector3.Dot(i, eye);
        t.Y = -Vector3.Dot(j, eye);
        t.Z = -Vector3.Dot(k, eye);

        Matrix4 temp = new Matrix4(
            i.X,  i.Y,  i.Z,  t.X,
             j.X,  j.Y,  j.Z,  t.Y,
             k.X,  k.Y,  k.Z,  t.Z,
             0.0f, 0.0f, 0.0f, 1.0f 
        );
        return temp;
    }

    // Vec2
    public static Vector2 VEC2_ZERO = new Vector2(0.0f, 0.0f);
    // Vec3
    public static Vector3 VEC3_ZERO = new Vector3(0.0f, 0.0f, 0.0f);
    public static Vector3 VEC3_UNIT = new Vector3(1.0f, 1.0f, 1.0f);
    public static Vector3 VEC3_UNIT_X = new Vector3(1.0f, 0.0f, 0.0f);
    public static Vector3 VEC3_UNIT_Y = new Vector3(0.0f, 1.0f, 0.0f);
    public static Vector3 VEC3_UNIT_Z = new Vector3(0.0f, 0.0f, 1.0f);
}