using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library;
using Library.Transformers;

namespace GUI {
    public sealed partial class MainForm : Form {
        private const string baseDir = "../../../obj";
        
        // REVIEW: CФЕРА
        private static readonly string Obj = $"{baseDir}/scr/ball.obj";
        private readonly TextureMap _diffuseMap = null;
        private readonly TextureMap _normalMap = null;
        private readonly TextureMap _specularMap = null;
        
        // REVIEW: САМОЛЕТ
        /*private static readonly string Obj = $"{baseDir}/piper/piper_pa18.obj";
        private readonly TextureMap _diffuseMap = new TextureMap($"{baseDir}/piper/texture/piper_diffuse.jpg");
        private readonly TextureMap _normalMap = new TextureMap($"{baseDir}/piper/texture/normal.jpg");
        private readonly TextureMap _specularMap = new TextureMap($"{baseDir}/piper/texture/piper_refl.jpg");*/
        
        private static readonly string _cubemapBaseDir = $"{baseDir}/cube1";
        private readonly string[] _cubemapPaths = {
            $"{_cubemapBaseDir}/posx.jpg",
            $"{_cubemapBaseDir}/negx.jpg",
            $"{_cubemapBaseDir}/posy.jpg",
            $"{_cubemapBaseDir}/negy.jpg",
            $"{_cubemapBaseDir}/posz.jpg",
            $"{_cubemapBaseDir}/negz.jpg",
        };

        private readonly Cubemap _cubeMap;
        
        private readonly IReadOnlyList<Vector2> _textureCoordinates = ObjParser.ParseTextureCoordinates(Obj);

        private readonly CoreTransformer _coreTransformer = new CoreTransformer();
        
        private readonly IReadOnlyList<Vector3> _vertices;
        private IReadOnlyList<Vector3> _vertexNormals = ObjParser.ParseNormals(Obj);
        private Vector3[] _worldVertices;
        private readonly IReadOnlyList<(FaceIndices, FaceIndices, FaceIndices)> _triangles;
        
        private Bitmap _bitmap;
        private Graphics _graphics;
        
        private readonly Timer _refreshTimer;
        
        private float[,] _zBuffer;
        private int _width, _height;
        
        private static readonly Vector3 _light = -new Vector3(0, 1, 0);
        
        // REVIEW: РЕДАКТИРУЙТЕ КОЭФФИЦИЕНТЫ 
        private static readonly float _ambientCoeff = 0.1f;
        private static readonly float _specularPower = 15f;
        private static readonly float _diffuseCoeff = 0.8f;
        private static readonly float _reflectionCoeff = 1.0f;
        
        private HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private const int fps = 150;
        
        public MainForm() {
            InitializeComponent();
            
            DoubleBuffered = true;
            WindowState = FormWindowState.Maximized;
            
            _vertices = ObjParser.ParseVertices(Obj);
            _worldVertices = new Vector3[_vertices.Count];
            _triangles = ObjParser.ParseFacesWithUV(Obj);
            _cubeMap = CubemapLoader.LoadCubemap(_cubemapPaths);
            
            Paint += OnPaint;
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;
            Resize += WindowResize;
            
            _refreshTimer = new Timer();
            _refreshTimer.Interval = 1000 / fps;
            _refreshTimer.Tick += ApplyTransformations;
            _refreshTimer.Start();
            
            FormClosing += OnFormClosing;
        }
        
        private void ApplyTransformations(object o, EventArgs ea)
        {
            // REVIEW: РЕДАКТИРУЙТЕ ШАГИ В ЗАВИСИМОСТИ ОТ РАЗМЕРОВ ОБЪЕКТОВ
            const float moveStep   = 1f;      // шаг панорамирования (Pan)
            const float dollyStep  = 1f;      // шаг приближения/удаления (Dolly)
            const float rotateStep = 0.1f;    // в радианах

            foreach (var key in pressedKeys)
            {
                switch (key)
                {
                    // Вращение камеры вокруг Target
                    case Keys.Left:
                        _coreTransformer.ObserverTransformer.RotateYaw(rotateStep);
                        break;
                    case Keys.Right:
                        _coreTransformer.ObserverTransformer.RotateYaw(-rotateStep);
                        break;
                    case Keys.Up:
                        _coreTransformer.ObserverTransformer.RotatePitch(-rotateStep);
                        break;
                    case Keys.Down:
                        _coreTransformer.ObserverTransformer.RotatePitch(rotateStep);
                        break;

                    // Панорамирование (сдвиг в плоскости экрана)
                    case Keys.A:
                        _coreTransformer.ObserverTransformer.Pan(new Vector3(-moveStep, 0, 0));
                        break;
                    case Keys.D:
                        _coreTransformer.ObserverTransformer.Pan(new Vector3( moveStep, 0, 0));
                        break;
                    case Keys.W:
                        _coreTransformer.ObserverTransformer.Pan(new Vector3(0,  moveStep, 0));
                        break;
                    case Keys.S:
                        _coreTransformer.ObserverTransformer.Pan(new Vector3(0, -moveStep, 0));
                        break;

                    // Dolly — приближение/отдаление
                    case Keys.Oemplus:
                        _coreTransformer.ObserverTransformer.Dolly(dollyStep);
                        break;
                    case Keys.OemMinus:
                        _coreTransformer.ObserverTransformer.Dolly(-dollyStep);
                        break;
                    
                    // Restore camera to default position
                    case Keys.R:
                        _coreTransformer.ObserverTransformer.RestorePosition();
                        break;

                    case Keys.Escape:
                        Close();
                        return;
                }
            }

            _coreTransformer.RecalculateResultTransformer();
            Invalidate();
        }
        
        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            _refreshTimer.Stop();
        }
        
        private void WindowResize(object sender, EventArgs e) {
            if (ClientSize.Width == 0 || ClientSize.Height == 0)
                return;
            
            _width = ClientSize.Width;
            _height = ClientSize.Height;
            _bitmap = new Bitmap(_width, _height);
            _graphics = Graphics.FromImage(_bitmap);
            
            _coreTransformer.WindowTransformer.width = ClientSize.Width;
            _coreTransformer.WindowTransformer.height = ClientSize.Height;
            _coreTransformer.RecalculateResultTransformer();

            _zBuffer = new float[_width, _height];
        }
        
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            pressedKeys.Add(e.KeyCode);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            pressedKeys.Remove(e.KeyCode);
        }
        
        private void OnPaint(object sender, PaintEventArgs e)
        {
            var (transformedVertices, oneOverWs) = _coreTransformer.Transform(_vertices);
            _worldVertices = _vertices.Select(v => _coreTransformer.TransformToWorld(v)).ToArray();
            var filteredTriangles = FilterTriangles(transformedVertices);
            //_vertexNormals = ComputeVertexNormals();
            
            _graphics.Clear(Color.Black); 
            var data = _bitmap.LockBits(new Rectangle(0, 0, _width, _height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                int* ptr = (int*)data.Scan0;
                int stride = data.Stride / 4;
                
                for (var i = 0; i < _zBuffer.GetLength(0); i++)
                    for (var j = 0; j < _zBuffer.GetLength(1); j++)
                        _zBuffer[i, j] = 1f;
                
                Parallel.ForEach(filteredTriangles, triangle =>
                {
                    var (i1, i2, i3) = triangle;
                    RenderTriangle(transformedVertices, oneOverWs, i1, i2, i3, ptr, stride);
                });
            }
            _bitmap.UnlockBits(data);
            e.Graphics.DrawImage(_bitmap, 0, 0);
        }
        
        private Vector3[] ComputeVertexNormals()
        {
            int vCount = _vertices.Count;
            var normals = new Vector3[vCount];
            var counts  = new int[vCount];

            foreach (var (i1, i2, i3) in _triangles)
            {
                var A = _coreTransformer.TransformToWorld(_vertices[i1.VertexIndex]);
                var B = _coreTransformer.TransformToWorld(_vertices[i2.VertexIndex]);
                var C = _coreTransformer.TransformToWorld(_vertices[i3.VertexIndex]);
                Vector3 faceNormal = Vector3Utils.NormalizeSafe(Vector3.Cross(B - A, C - A));

                normals[i1.VertexIndex] += faceNormal;
                normals[i2.VertexIndex] += faceNormal;
                normals[i3.VertexIndex] += faceNormal;

                counts[i1.VertexIndex]++;
                counts[i2.VertexIndex]++;
                counts[i3.VertexIndex]++;
            }

            for (int i = 0; i < vCount; i++)
            {
                if (counts[i] > 0)
                {
                    normals[i] = Vector3Utils.NormalizeSafe(normals[i] / counts[i]);
                }
                else
                {
                    normals[i] = Vector3.UnitY;
                }
            }

            return normals;
        }
        
        private List<(FaceIndices, FaceIndices, FaceIndices)> FilterTriangles(IReadOnlyList<Vector3> transformedVertices)
        {
            return _triangles
                .AsParallel()
                .Where(triangle =>
                {
                    var (i1, i2, i3) = triangle;
                    var p1V2 = new Vector2(transformedVertices[i1.VertexIndex].X, transformedVertices[i1.VertexIndex].Y);
                    var p2V2 = new Vector2(transformedVertices[i2.VertexIndex].X, transformedVertices[i2.VertexIndex].Y);
                    var p3V2 = new Vector2(transformedVertices[i3.VertexIndex].X, transformedVertices[i3.VertexIndex].Y);

                    var v1 = p2V2 - p1V2;
                    var v2 = p3V2 - p1V2;

                    var k = v1.X * v2.Y - v1.Y * v2.X;
                    return k < 0;
                })
                .ToList();
        }
        
        private unsafe void RenderTriangle(
            IReadOnlyList<Vector3> transformedVertices,
            IReadOnlyList<float> oneOverWs,
            FaceIndices i1, FaceIndices i2, FaceIndices i3,
            int* ptr, int stride)
        {
            var p1 = transformedVertices[i1.VertexIndex];
            var p2 = transformedVertices[i2.VertexIndex];
            var p3 = transformedVertices[i3.VertexIndex];

            var n1 = _vertexNormals[i1.NormalIndex];
            var n2 = _vertexNormals[i2.NormalIndex];
            var n3 = _vertexNormals[i3.NormalIndex];

            var wp1 = _worldVertices[i1.VertexIndex];
            var wp2 = _worldVertices[i2.VertexIndex];
            var wp3 = _worldVertices[i3.VertexIndex];

            var ow1 = oneOverWs[i1.VertexIndex];
            var ow2 = oneOverWs[i2.VertexIndex];
            var ow3 = oneOverWs[i3.VertexIndex];
            
            var uv1 = _textureCoordinates[i1.UvIndex];
            var uv2 = _textureCoordinates[i2.UvIndex];
            var uv3 = _textureCoordinates[i3.UvIndex];
    
            Alg.ScanlineTriangle(
                (_width, _height),
                new Alg.VertexData(p1, n1, wp1, uv1, ow1),
                new Alg.VertexData(p2, n2, wp2, uv2, ow2),
                new Alg.VertexData(p3, n3, wp3, uv3, ow3),
                (x, y, interpolatedNormal, worldP, uv) =>
                {
                    var z = Alg.GetZ(x, y, p1, p2, p3);
                    if (_zBuffer[x, y] > z)
                    {
                        bool shouldRender;
                        lock (_zBuffer)
                        {
                            if (_zBuffer[x, y] > z)
                            {
                                _zBuffer[x, y] = z;
                                shouldRender = true;
                            }
                            else
                            {
                                shouldRender = false;
                            }
                        }

                        if (shouldRender)
                        {
                            var texColor = _diffuseMap?.Color(uv.X, uv.Y) ?? Color.White;
                            var normal = _normalMap?.Normal(uv.X, uv.Y) ?? Vector3.Zero;
                            var specular = _specularMap?.Specular(uv.X, uv.Y) ?? 0;
                    
                            var color = ComputePhongColorWithTextures(interpolatedNormal, normal, worldP, texColor.ToVector3(), specular);
                            ptr[y * stride + x] = ColorFromVector(color).ToArgb();
                        }
                    }
                }
            );
        }

        private Vector3 ComputePhongColorWithTextures(Vector3 vertexNormal, Vector3 normalMapNormal, Vector3 wp, Vector3 texColor, float specular)
        {
            Vector3 viewDir = Vector3Utils.NormalizeSafe(_coreTransformer.ObserverTransformer.Eye - wp);
            Vector3 L = Vector3Utils.NormalizeSafe(-_light);

            Vector3 N = Vector3Utils.NormalizeSafe(vertexNormal + normalMapNormal);

            Vector3 ambient = texColor * _ambientCoeff;

            float ndotl = Math.Max(Vector3Utils.SafeDot(N, L), 0);
            Vector3 diffuse = _diffuseCoeff * texColor * ndotl;

            Vector3 RLight = Vector3.Reflect(-L, N);
            float rdotv = Math.Max(Vector3Utils.SafeDot(RLight, viewDir), 0);
            Vector3 specularV3 = texColor * (float)Math.Pow(rdotv, _specularPower) * (1 - specular);
            
            Vector3 RView = Vector3.Reflect(-viewDir, N);
            Color reflC = CubemapSampler.Sample(_cubeMap, RView);
            Vector3 reflection = reflC.ToVector3() * _reflectionCoeff;
            
            Vector3 result = ambient + diffuse + specularV3 + reflection;
            return Vector3.Clamp(result, Vector3.Zero, new Vector3(255, 255, 255));
        }

        /*private Vector3 ComputePhongColor(Vector3 normal,Vector3 wp)
        {
            Vector3 viewDir = Vector3Utils.NormalizeSafe(_coreTransformer.ObserverTransformer.Eye - wp);
            Vector3 L = Vector3Utils.NormalizeSafe(-_light);
            Vector3 N = normal;

            float ndotl = Math.Max(Vector3Utils.SafeDot(N, L), 0);
            Vector3 diffuse = _diffuseCoeff * _color * ndotl;

            Vector3 R = Vector3.Reflect(-L, N);
            float rdotv = Math.Max(Vector3Utils.SafeDot(R, viewDir), 0);
            Vector3 specular = _color * (float)Math.Pow(rdotv, _specularPower);
            
            return Vector3.Clamp(_ambient + diffuse + specular, Vector3.Zero, new Vector3(255, 255, 255));
        }*/

        private static Color ColorFromVector(Vector3 v)
        {
            return Color.FromArgb(
                255,
                ClampToByte(v.X),
                ClampToByte(v.Y),
                ClampToByte(v.Z)
            );
        }
        
        private static int ClampToByte(float value)
        {
            if (value < 0f)
                return 0;
            if (value > 255f)
                return 255;
            return (int)value;
        }

        private float GetIntense(int i1, int i2, int i3) {
            var v1 = _coreTransformer.TransformToWorld(_vertices[i1]);
            var v2 = _coreTransformer.TransformToWorld(_vertices[i2]);
            var v3 = _coreTransformer.TransformToWorld(_vertices[i3]);
            var n = Vector3Utils.NormalizeSafe(Vector3.Cross(v2 - v1, v3 - v1));
            return Math.Max(0.0f, Vector3Utils.SafeDot(n, -Vector3Utils.NormalizeSafe(_light)));
        }
    }
}