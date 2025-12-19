using Microsoft.Win32;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Lab_4_CSHARP
{
    public partial class MainWindow
    {
        private BitmapSource? _sourceImage;
        private BitmapSource? _fragmentImage;

        public MainWindow()
        {
            InitializeComponent();
        }

        // ============================================================
        // ЗАГРУЗКА ИСХОДНОГО ИЗОБРАЖЕНИЯ
        // ============================================================
        private void BtnLoadSrc_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Изображения|*.png;*.jpg;*.bmp";

            if (dlg.ShowDialog() == true)
            {
                _sourceImage = new BitmapImage(new Uri(dlg.FileName));
                ImgSource.Source = _sourceImage;
            }
        }

        // ============================================================
        // СЛУЧАЙНЫЙ ФРАГМЕНТ
        // ============================================================
        private void BtnRandomFragment_Click(object sender, RoutedEventArgs e)
        {
            if (_sourceImage == null) return;

            int W = _sourceImage.PixelWidth;
            int H = _sourceImage.PixelHeight;

            Random rnd = new Random();

            int fw = W / 4;     // размер фрагмента
            int fh = H / 4;

            int x = rnd.Next(0, W - fw);
            int y = rnd.Next(0, H - fh);

            _fragmentImage = new CroppedBitmap(_sourceImage, new Int32Rect(x, y, fw, fh));
            ImgFragment.Source = _fragmentImage;
        }

        // ============================================================
        // ВЗАИМНАЯ КОРРЕЛЯЦИЯ
        // ============================================================
        private void BtnCrossCorr_Click(object sender, RoutedEventArgs e)
        {
            if (_sourceImage == null || _fragmentImage == null) return;

            var corrMap = ComputeCrossCorrelation(_sourceImage, _fragmentImage,
                                                  out int bestX, out int bestY);

            ImgCorrelation.Source = corrMap;

            HighlightFragment(bestX, bestY, _fragmentImage.PixelWidth, _fragmentImage.PixelHeight);
        }

        // ============================================================
        // АВТОКОРРЕЛЯЦИЯ
        // ============================================================
        private void BtnAutoCorr_Click(object sender, RoutedEventArgs e)
        {
            if (_sourceImage == null) return;

            var corrMap = ComputeAutoCorrelation(_sourceImage);
            ImgCorrelation.Source = corrMap;
        }

        // ============================================================
        // ВЫДЕЛЕНИЕ НАЙДЕННОГО ПРЯМОУГОЛЬНИКА
        // ============================================================
        private void HighlightFragment(int x, int y, int w, int h)
        {
            if (_sourceImage == null) return;

            DrawingVisual dv = new DrawingVisual();

            using (DrawingContext dc = dv.RenderOpen())
            {
                // рисуем исходное изображение
                dc.DrawImage(_sourceImage, new Rect(0, 0, _sourceImage.PixelWidth, _sourceImage.PixelHeight));

                // красная рамка
                Pen pen = new Pen(Brushes.Red, 4);
                dc.DrawRectangle(null, pen, new Rect(x, y, w, h));
            }

            RenderTargetBitmap result = new RenderTargetBitmap(
                _sourceImage.PixelWidth,
                _sourceImage.PixelHeight,
                96, 96,
                PixelFormats.Pbgra32);

            result.Render(dv);
            ImgSource.Source = result;
        }

        // ============================================================
        // КОРРЕЛЯЦИЯ: ВЗАИМНАЯ
        // ============================================================
        private WriteableBitmap ComputeCrossCorrelation(
            BitmapSource img, BitmapSource tpl,
            out int bestX, out int bestY)
        {
            int W = img.PixelWidth;
            int H = img.PixelHeight;
            int w = tpl.PixelWidth;
            int h = tpl.PixelHeight;

            byte[] I = GetGray(img);
            byte[] T = GetGray(tpl);

            // Для шаблона считаем
            // среднее значение яркости
            // нормировочный множитель (сумма квадратов отклонений от среднего)
            double meanT = T.Select(v => (double)v).Average();
            double normT = Math.Sqrt(T.Sum(v => Math.Pow(v - meanT, 2)));

            double[,] C = new double[H - h, W - w];

            double bestVal = double.MinValue;
            int bx = 0, by = 0;

            for (int y = 0; y <= H - h - 1; y++)
            {
                for (int x = 0; x <= W - w - 1; x++)
                {
                    // Вычисление среднего значения окна изображения
                    double meanI = 0;
                    for (int j = 0; j < h; j++)
                        for (int i = 0; i < w; i++)
                            meanI += I[(y + j) * W + (x + i)];

                    meanI /= (w * h);

                    double normI = 0;
                    double cross = 0;

                    for (int j = 0; j < h; j++)
                    {
                        for (int i = 0; i < w; i++)
                        {
                            // отклонение пикселя изображения от среднего значения окна
                            double a = I[(y + j) * W + (x + i)] - meanI;
                            // отклонение пикселя шаблона от среднего значения шаблона 
                            double b = T[j * w + i] - meanT;

                            // Кросс-корреляция в числителе:
                            cross += a * b;
                            
                            // Нормировка окна изображения
                            normI += a * a;
                        }
                    }

                    // нормированной взаимной корреляции
                    double corr = cross / (Math.Sqrt(normI) * normT + 1e-9);
                    C[y, x] = corr;

                    if (corr > bestVal)
                    {
                        bestVal = corr;
                        bx = x;
                        by = y;
                    }
                }
            }

            bestX = bx;
            bestY = by;

            return MatrixToBitmap(C);
        }

        // ============================================================
        // КОРРЕЛЯЦИЯ: АВТОКОРРЕЛЯЦИЯ
        // ============================================================
        private WriteableBitmap ComputeAutoCorrelation(BitmapSource img)
        {
            int W = img.PixelWidth;
            int H = img.PixelHeight;

            byte[] I = GetGray(img);
            double[,] C = new double[H, W];

            for (int dy = -H / 2; dy < H / 2; dy++)
            {
                for (int dx = -W / 2; dx < W / 2; dx++)
                {
                    double sum = 0;

                    for (int y = 0; y < H; y++)
                    {
                        int yy = y + dy;
                        if (yy < 0 || yy >= H) continue;

                        for (int x = 0; x < W; x++)
                        {
                            int xx = x + dx;
                            if (xx < 0 || xx >= W) continue;

                            // Стандартная формула автокорреляции
                            sum += I[y * W + x] * I[yy * W + xx];
                        }
                    }

                    C[dy + H / 2, dx + W / 2] = sum;
                }
            }

            return MatrixToBitmap(C);
        }

        // ============================================================
        // ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ
        // ============================================================
        private byte[] GetGray(BitmapSource bmp)
        {
            int W = bmp.PixelWidth;
            int H = bmp.PixelHeight;

            byte[] raw = new byte[W * H * 4];
            bmp.CopyPixels(raw, W * 4, 0);

            byte[] gray = new byte[W * H];

            for (int i = 0; i < gray.Length; i++)
            {
                byte b = raw[i * 4 + 0];
                byte g = raw[i * 4 + 1];
                byte r = raw[i * 4 + 2];
                gray[i] = (byte)((r + g + b) / 3);
            }

            return gray;
        }

        private WriteableBitmap MatrixToBitmap(double[,] M)
        {
            int H = M.GetLength(0);
            int W = M.GetLength(1);

            double min = M.Cast<double>().Min();
            double max = M.Cast<double>().Max();

            var bmp = new WriteableBitmap(W, H, 96, 96, PixelFormats.Gray8, null);
            byte[] data = new byte[W * H];

            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                    data[y * W + x] = (byte)(255 * (M[y, x] - min) / (max - min + 1e-9));

            bmp.WritePixels(new Int32Rect(0, 0, W, H), data, W, 0);
            return bmp;
        }
    }
}
