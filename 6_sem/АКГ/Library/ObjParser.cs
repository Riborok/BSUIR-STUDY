using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace Library {
    public struct FaceIndices(int vertexIndex, int uvIndex, int normalIndex) {
        public int VertexIndex = vertexIndex;
        public int UvIndex = uvIndex;
        public int NormalIndex = normalIndex;
    }
    
    public static class ObjParser {
        public static IReadOnlyList<Vector3> ParseVertices(string filePath)
        {
            var culture = CultureInfo.InvariantCulture;
            var vertexList = new List<Vector3>();
            foreach (var line in File.ReadLines(filePath))
            {
                if (line.StartsWith("v "))
                {
                    var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 4)
                    {
                        if (float.TryParse(parts[1], NumberStyles.Float, culture, out var x) &&
                            float.TryParse(parts[2], NumberStyles.Float, culture, out var y) &&
                            float.TryParse(parts[3], NumberStyles.Float, culture, out var z))
                        {
                            vertexList.Add(new Vector3(x, y, z));
                        }
                    }
                }
            }
            return vertexList;
        }
        
        public static IReadOnlyList<(int, int, int)> ParseFaces(string filePath)
        {
            var faceList = new List<(int, int, int)>();
            foreach (var line in File.ReadLines(filePath))
            {
                if (line.StartsWith("f "))
                {
                    var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    
                    var indices = new List<int>();
                    for (var i = 1; i < parts.Length; i++)
                    {
                        var vertexParts = parts[i].Split('/');
                        if (int.TryParse(vertexParts[0], out var vertexIndex))
                        {
                            indices.Add(vertexIndex - 1);
                        }
                    }

                    switch (indices.Count) {
                        case 3:
                            faceList.Add((indices[0], indices[1], indices[2]));
                            break;
                        case 4:
                            faceList.Add((indices[0], indices[1], indices[2]));
                            faceList.Add((indices[0], indices[2], indices[3]));
                            break;
                        default:
                            throw new Exception("Unknown face index");
                    }
                }
            }
            return faceList;
        }
        
        public static IReadOnlyList<Vector2> ParseTextureCoordinates(string filePath)
        {
            var culture = CultureInfo.InvariantCulture;
            var uvList = new List<Vector2>();
            foreach (var line in File.ReadLines(filePath))
            {
                if (line.StartsWith("vt "))
                {
                    var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3)
                    {
                        if (float.TryParse(parts[1], NumberStyles.Float, culture, out var u) &&
                            float.TryParse(parts[2], NumberStyles.Float, culture, out var v))
                        {
                            uvList.Add(new Vector2(u, v));
                        }
                    }
                }
            }
            return uvList;
        }
        
        public static IReadOnlyList<(FaceIndices, FaceIndices, FaceIndices)> ParseFacesWithUV(string filePath)
        {
            var faceList = new List<(FaceIndices, FaceIndices, FaceIndices)>();
            foreach (var line in File.ReadLines(filePath))
            {
                if (line.StartsWith("f "))
                {
                    var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    var indices = new List<FaceIndices>();
            
                    for (var i = 1; i < parts.Length; i++)
                    {
                        var vertexParts = parts[i].Split('/');
                        int vertexIndex = int.Parse(vertexParts[0]) - 1;
                        int uvIndex = vertexParts.Length > 1 && !string.IsNullOrEmpty(vertexParts[1]) 
                            ? int.Parse(vertexParts[1]) - 1 
                            : 0;
                        int normalIndex = vertexParts.Length > 2 && !string.IsNullOrEmpty(vertexParts[2])
                            ? int.Parse(vertexParts[2]) - 1
                            : 0;
                
                        indices.Add(new FaceIndices(vertexIndex, uvIndex, normalIndex));
                    }

                    switch (indices.Count) {
                        case 3:
                            faceList.Add((indices[0], indices[1], indices[2]));
                            break;
                        case 4:
                            faceList.Add((indices[0], indices[1], indices[2]));
                            faceList.Add((indices[0], indices[2], indices[3]));
                            break;
                        default:
                            Console.WriteLine("Unknown face index: " + indices.Count);
                            break;
                    }
                }
            }
            return faceList;
        }
        
        public static IReadOnlyList<Vector3> ParseNormals(string filePath)
        {
            var culture = CultureInfo.InvariantCulture;
            var normalList = new List<Vector3>();
            foreach (var line in File.ReadLines(filePath))
            {
                if (line.StartsWith("vn "))
                {
                    var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 4)
                    {
                        if (float.TryParse(parts[1], NumberStyles.Float, culture, out var x) &&
                            float.TryParse(parts[2], NumberStyles.Float, culture, out var y) &&
                            float.TryParse(parts[3], NumberStyles.Float, culture, out var z))
                        {
                            normalList.Add(new Vector3(x, y, z));
                        }
                    }
                }
            }
            return normalList;
        }
    }
}