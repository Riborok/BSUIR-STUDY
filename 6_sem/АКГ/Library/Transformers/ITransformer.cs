using System.Collections.Generic;
using System.Numerics;

namespace Library.Transformers;

public interface ITransformer {
    Matrix4x4 GetMatrix();
}