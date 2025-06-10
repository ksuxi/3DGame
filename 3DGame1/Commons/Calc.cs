using OpenTK;
using OpenTK.Mathematics;

class Calc
{

    // ビュー行列
    // eye：自身の位置
    // target：注視対象の位置
    // up：上方向ベクトル
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
            i.X, i.Y, i.Z, t.X,
             j.X, j.Y, j.Z, t.Y,
             k.X, k.Y, k.Z, t.Z,
             0.0f, 0.0f, 0.0f, 1.0f
        );
        return temp;
    }

    //  平行移動行列
    public static Matrix4 CreateTranslation(float x, float y, float z)
    {
        Matrix4 temp = new Matrix4(
        1.0f, 0.0f, 0.0f, x,
        0.0f, 1.0f, 0.0f, y,
        0.0f, 0.0f, 1.0f, z,
        0.0f, 0.0f, 0.0f, 1.0f
        );

        return temp;
    }

    // クオータニオン
    public class Quaternion
    {
        public float x, y, z;
        public float w;

        public Quaternion()
        {
            x = 0.0f;
            y = 0.0f;
            z = 0.0f;
            w = 1.0f;
        }

        public Quaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Quaternion(Vector3 axis, float angle)
        {
            float scalar = (float)Math.Sin(angle / 2.0f);
            x = axis.X * scalar;
            y = axis.Y * scalar;
            z = axis.Z * scalar;
            w = (float)Math.Cos(angle / 2.0f);
        }

        public static Matrix4 CreateQuaternion(Quaternion q)
        {
            Matrix4 temp = new Matrix4(
                1.0f - 2.0f * q.y * q.y - 2.0f * q.z * q.z,
                2.0f * q.x * q.y - 2.0f * q.w * q.z,
                2.0f * q.x * q.z + 2.0f * q.w * q.y,
                0.0f,
                2.0f * q.x * q.y + 2.0f * q.w * q.z,
                1.0f - 2.0f * q.x * q.x - 2.0f * q.z * q.z,
                2.0f * q.y * q.z - 2.0f * q.w * q.x,
                0.0f,
                2.0f * q.x * q.z - 2.0f * q.w * q.y,
                2.0f * q.y * q.z + 2.0f * q.w * q.x,
                1.0f - 2.0f * q.x * q.x - 2.0f * q.y * q.y,
                0.0f,
                0.0f,
                0.0f,
                0.0f,
                1.0f
                );

            return temp;
        }

        public static Vector3 RotateVec(Vector3 v, Quaternion q)
        {
            Vector3 qv = new Vector3(q.x, q.y, q.z);
            Vector3 retVec = v;
            retVec += 2.0f * Vector3.Cross(qv, Vector3.Cross(qv, v) + q.w * v);
            return retVec;
        }

        public static Quaternion Concatenate(Quaternion q, Quaternion p)
        {
            Vector3 qv = new Vector3(q.x, q.y, q.z);
            Vector3 pv = new Vector3(p.x, p.y, p.z);

            //  グラスマン積でベクトルとスカラを計算
            Vector3 outVec = p.w * qv + q.w * pv + Vector3.Cross(pv, qv);
            float outw = p.w * q.w - Vector3.Dot(pv, qv);

            return new Quaternion(outVec.X, outVec.Y, outVec.Z, outw);
        }
    }

    public static Matrix4 CreateScale(float x, float y, float z)
    {
        Matrix4 temp = new Matrix4(
            x, 0.0f, 0.0f, 0.0f,
            0.0f, y, 0.0f, 0.0f,
            0.0f, 0.0f, z, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);
        return temp;
    }

    public static float[] Matrix4ToFloat(Matrix4 mat)
    {
        return new float[]
    {
        mat.M11, mat.M21, mat.M31, mat.M41,
        mat.M12, mat.M22, mat.M32, mat.M42,
        mat.M13, mat.M23, mat.M33, mat.M43,
        mat.M14, mat.M24, mat.M34, mat.M44
    };
    }

    public static Matrix4 CreatePerspectiveFOV(float fov, float width, float height,
                                        float near, float far)
    {
        float yScale = 1.0f / (float)Math.Tan(fov / 2.0f);
        float xScale = yScale * height / width;

        Matrix4 temp = new Matrix4(
             xScale, 0.0f, 0.0f, 0.0f,
             0.0f, yScale, 0.0f, 0.0f,
             0.0f, 0.0f, far / (far - near), -near * far / (far - near),
             0.0f, 0.0f, 1.0f, 0.0f
            );
        return temp;
    }

    public static float ToRadians(float degrees)
    {
        return degrees * (float)Math.PI / 180.0f;
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