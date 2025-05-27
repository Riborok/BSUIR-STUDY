using System;
using System.Collections.Generic;
using System.Numerics;

namespace Library.Transformers;

public class WorldTransformer: ITransformer {
    public Vector3 Translation { get; private set; } = Vector3.Zero;
    public Vector3 Scale { get; private set; } = Vector3.One;
    public Vector3 RotationAngle { get; private set; } = Vector3.Zero;
    
    public void Move(Vector3 delta) => Translation += delta;
    public void ScaleBy(float percent) {
        var scaleFactor = 1 + (percent / 100f);
        Scale *= new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }
    public void RotateBy(Vector3 deltaRotation) => RotationAngle += deltaRotation;
    
    public Matrix4x4 GetMatrix() {
        var matrices = new TransformationMatrices(this);
        return Matrix4x4.Transpose(
            matrices.TranslationMatrix
                                * matrices.ScaleMatrix 
                                * matrices.RotationMatrixX 
                                * matrices.RotationMatrixY
                                * matrices.RotationMatrixZ
        );
    }

    private class TransformationMatrices {
        private readonly WorldTransformer _transformer;
        public Matrix4x4 TranslationMatrix { get; }
        public Matrix4x4 ScaleMatrix { get; }
        public Matrix4x4 RotationMatrixX { get; }
        public Matrix4x4 RotationMatrixY { get; }
        public Matrix4x4 RotationMatrixZ { get; }
        
        public TransformationMatrices(WorldTransformer transformer)
        {
            _transformer = transformer;
            TranslationMatrix = CreateTranslationMatrix();
            ScaleMatrix = CreateScaleMatrix();
            RotationMatrixX = CreateRotationMatrixX();
            RotationMatrixY = CreateRotationMatrixY();
            RotationMatrixZ = CreateRotationMatrixZ();
        }
        
        private Matrix4x4 CreateTranslationMatrix() {
            var translation = _transformer.Translation;
            return new Matrix4x4 {
                M11 = 1,
                M14 = translation.X,
                M22 = 1,
                M24 = translation.Y,
                M33 = 1,
                M34 = translation.Z,
                M44 = 1
            };
        }

        private Matrix4x4 CreateScaleMatrix() {
            var scale = _transformer.Scale;
            return new Matrix4x4 {
                M11 = scale.X,
                M22 = scale.Y,
                M33 = scale.Z,
                M44 = 1
            };
        }

        private Matrix4x4 CreateRotationMatrixX()
        {
            var angle = _transformer.RotationAngle.X;
            return new Matrix4x4 {
                M11 = 1,
                M22 = (float)Math.Cos(angle),
                M23 = (float)-Math.Sin(angle),
                M32 = (float)Math.Sin(angle),
                M33 = (float)Math.Cos(angle),
                M44 = 1
            };
        }

        private Matrix4x4 CreateRotationMatrixY()
        {
            var angle = _transformer.RotationAngle.Y;
            return new Matrix4x4 {
                M11 = (float)Math.Cos(angle),
                M13 = (float)Math.Sin(angle),
                M22 = 1,
                M31 = (float)-Math.Sin(angle),
                M33 = (float)Math.Cos(angle),
                M44 = 1
            };
        }

        private Matrix4x4 CreateRotationMatrixZ() {
            var angle = _transformer.RotationAngle.Z;
            return new Matrix4x4 {
                M11 = (float)Math.Cos(angle),
                M12 = (float)-Math.Sin(angle),
                M21 = (float)Math.Sin(angle),
                M22 = (float)Math.Cos(angle),
                M33 = 1,
                M44 = 1
            };
        }
    }
}