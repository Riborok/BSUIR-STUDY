using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows.Documents;

namespace Library.Transformers;

public class CoreTransformer {
    public WindowTransformer WindowTransformer { get; } = new WindowTransformer();
    public ProjectionTransformer ProjectionTransformer { get; } = new ProjectionTransformer();
    public ObserverTransformer ObserverTransformer { get; } = new ObserverTransformer();
    
    public WorldTransformer WorldTransformer { get; } = new WorldTransformer();

    private Matrix4x4 _modelMatrix;
    private Matrix4x4 _viewMatrix;
    private Matrix4x4 _projectionMatrix;
    private Matrix4x4 _viewportMatrix;
    private Matrix4x4 _resultTransformer;

    public void RecalculateResultTransformer() {
        _modelMatrix = WorldTransformer.GetMatrix();
        _viewMatrix = ObserverTransformer.GetMatrix();
        _projectionMatrix = ProjectionTransformer.GetMatrix();
        _viewportMatrix = WindowTransformer.GetMatrix();
        _resultTransformer = _modelMatrix * _viewMatrix * _projectionMatrix * _viewportMatrix;
    }
    
    public (IReadOnlyList<Vector3>, IReadOnlyList<float>) Transform(IReadOnlyList<Vector3> vertices) {
        var results = vertices.Select(Transform).ToList();
        return (
            results.Select(r => r.Item1).ToList(),
            results.Select(r => r.Item2).ToList()
        );
    }

    private (Vector3, float) Transform(Vector3 vertex) {
        var v4 = Vector4.Transform(vertex, _resultTransformer);
        return (new Vector3(v4.X / v4.W, v4.Y / v4.W, v4.Z / v4.W), 1/v4.W);
    }
    
    public Vector3 TransformToWorld(Vector3 vertex) {
        var v4 = Vector4.Transform(vertex, _modelMatrix);
        return new Vector3(v4.X, v4.Y, v4.Z);
    }
}