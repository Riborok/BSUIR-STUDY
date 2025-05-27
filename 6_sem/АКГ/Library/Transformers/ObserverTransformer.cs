using System.Numerics;

namespace Library.Transformers;

public class ObserverTransformer: ITransformer {
    // REVIEW: ЕСЛИ ОБЪЕКТ ДАЛЕКО/БЛИЗКО - РЕДАКТИРУЙТЕ Z
    // ЕСЛИ КАМЕРА ВНУТРИ ОБЪЕКТА - БУДЕТ ВЫЛЕТ/БОЛЬШИЕ ЛАГИ
    private static readonly Vector3 defaultEyePos = new Vector3 { X = 0, Y = 0, Z = 200 };
    private static readonly Vector3 defaultTargetPos = Vector3.Zero;
    
    public Vector3 Eye { get; set; } = defaultEyePos;
    public Vector3 Target { get; set; } = defaultTargetPos;

    private static readonly Vector3 Up = new (0, 1, 0);
    
    // Restoring position to default
    public void RestorePosition() {
        Eye = defaultEyePos;
        Target = defaultTargetPos;
    }

    // Перемещение («панорамирование») камеры и точки взгляда вдоль локальных осей
    public void Pan(Vector3 offset)
    {
        Eye += offset;
        Target += offset;
    }

    // Приближение/удаление (движение вперед/назад)
    public void Dolly(float amount)
    {
        // Двигаем вдоль вектора (Target - Eye)
        var forward = Vector3.Normalize(Target - Eye);
        Eye    += forward * amount;
    }

    // Вращение камеры вокруг Target по углу yaw (вокруг Up)
    public void RotateYaw(float angleRadians)
    {
        var dir = Eye - Target;
        var q   = Quaternion.CreateFromAxisAngle(Up, angleRadians);
        dir = Vector3.Transform(dir, q);
        Eye = Target + dir;
    }

    // Вращение камеры вокруг Target по углу pitch (вокруг локальной оси вправо)
    public void RotatePitch(float angleRadians)
    {
        var dir     = Eye - Target;
        var right   = Vector3.Normalize(Vector3.Cross(Up, dir));
        var q       = Quaternion.CreateFromAxisAngle(right, angleRadians);
        dir = Vector3.Transform(dir, q);
        Eye = Target + dir;
    }
    
    public Matrix4x4 GetMatrix() {
        var axisZ = Vector3.Normalize(Eye - Target);
        var axisX = Vector3.Normalize(Vector3.Cross(Up, axisZ));
        var axisY = Vector3.Cross(axisZ, axisX);

        var observerMatrix = new Matrix4x4 {
            M11 = axisX.X, M12 = axisX.Y, M13 = axisX.Z, M14 = -Vector3.Dot(axisX, Eye),
            M21 = axisY.X, M22 = axisY.Y, M23 = axisY.Z, M24 = -Vector3.Dot(axisY, Eye),
            M31 = axisZ.X, M32 = axisZ.Y, M33 = axisZ.Z, M34 = -Vector3.Dot(axisZ, Eye),
            M41 = 0f,      M42 = 0f,      M43 = 0f,      M44 = 1f
        };

        return Matrix4x4.Transpose(observerMatrix);
    }
}