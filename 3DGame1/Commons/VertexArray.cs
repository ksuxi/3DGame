using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

class VertexArray
{
    private int mNumVertices;   // ���_�o�b�t�@�̒��_��
    private int mNumIndices;    // �C���f�b�N�X�o�b�t�@�̒��_��
    private int mVertexBuffer;  // ���_�o�b�t�@��OpenGLID
    private int mIndexBuffer;    // �C���f�b�N�X�o�b�t�@��OpenGLID
    private int mVertexArray;   // ���_�z��I�u�W�F�N�g��OpenGLID
    
    public VertexArray(float[] vertices, 
        uint numVertices, uint[] indices, uint numIndices)
    {
        mNumVertices = (int)numVertices;
        mNumIndices = (int)numIndices;

        // ���_�z��I�u�W�F�N�g�̐���
        GL.GenVertexArrays(1, out mVertexArray);
        GL.BindVertexArray(mVertexArray);

        // ���_�o�b�t�@�̍쐬
        GL.GenBuffers(1, out mVertexBuffer);
        GL.BindBuffer(BufferTarget.ArrayBuffer, mVertexBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer,             //  �o�b�t�@�̎��
                        (int)numVertices * sizeof(float),   // �R�s�[����o�C�g��(�ʒu(xyz), �@��(xyz), u, v)
                        vertices,                                         // �R�s�[��
                        BufferUsageHint.StaticDraw);        //  �f�[�^�̗��p���@

        //  �C���f�b�N�X�o�b�t�@�̍쐬
        GL.GenBuffers(1, out mIndexBuffer);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, mIndexBuffer);
        GL.BufferData(BufferTarget.ElementArrayBuffer,
                           (int)numIndices * sizeof(uint),
                           indices,
                           BufferUsageHint.StaticDraw);

        // ���_���C�A�E�g�̎w��
        // ���_����0:   �ʒu(x,y,z)
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, 0);
        //  ���_����1, �@��(x,y,z)
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, 
            (nint)(sizeof(float)*3));
        //  ���_����2, u, v
        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, sizeof(float) * 8,
            (nint)(sizeof(float)*6));
    }

    public void SetActive()
    {
        GL.BindVertexArray(mVertexArray);
    }

    public int GetNumIndices() { return mNumIndices; }
}