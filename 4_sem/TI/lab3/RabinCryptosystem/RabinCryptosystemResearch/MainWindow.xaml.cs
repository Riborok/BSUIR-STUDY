using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using RabinCryptosystemResearchHelper;

namespace RabinCryptosystemResearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private UnambiguityTest? _unambiguityTest;
        private Ranges _ranges;
        private volatile bool _isInitialized = false;
        
        public MainWindow()
        {
            InitializeComponent();
            Task.Run(() =>
            {
                _unambiguityTest = new UnambiguityTest();
                _isInitialized = true;
            });
        }
        
        private void MainViewport_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var requestRange = RequestRange();
            if (requestRange.HasValue)
            {
                WaitInitialization();
                _ranges = requestRange.Value;
                MainViewport.Children.Clear();
                AddAxisLinesAndLabels();
                var results = _unambiguityTest!.Run(_ranges.pRange, _ranges.qRange, _ranges.bRange);
                foreach (var result in results)
                    AddPoint(result);
            }
        }

        private void WaitInitialization()
        {
            while (!_isInitialized)
                Thread.Sleep(42);
        }
        
        private void AddAxisLinesAndLabels()
        {
            var primes = _unambiguityTest!.Primes;
            var (pRange, qRange, bRange) = _ranges;
            
            // Axis p
            MainViewport.Children.Add(new ArrowVisual3D { Point1 = new Point3D(0, 0, 0), Point2 = new Point3D(pRange.Count, 0, 0), Diameter = 0.5 });
            for (int i = pRange.Start; i < Math.Min(primes.Count, pRange.End); i++)
              MainViewport.Children.Add(new TextVisual3D { Position = new Point3D(i - pRange.Start, 0, -1), Text = primes[i].ToString(CultureInfo.InvariantCulture) });

            // Axis q
            MainViewport.Children.Add(new ArrowVisual3D { Point1 = new Point3D(0, 0, 0), Point2 = new Point3D(0, qRange.Count, 0), Diameter = 0.5 });
            for (var i = qRange.Start; i < Math.Min(primes.Count, qRange.End); i++)
              MainViewport.Children.Add(new TextVisual3D { Position = new Point3D(0, i - qRange.Start, -1), Text = primes[i].ToString(CultureInfo.InvariantCulture) });

            // Axis b
            MainViewport.Children.Add(new ArrowVisual3D { Point1 = new Point3D(0, 0, 0), Point2 = new Point3D(0, 0, bRange.Count), Diameter = 0.5 });
            for (var i = 1; i <= bRange.End; i++)
              MainViewport.Children.Add(new TextVisual3D { Position = new Point3D(0, -1, i - bRange.Start), Text = i.ToString(CultureInfo.InvariantCulture) });
            
            MainViewport.Children.Add(new TextVisual3D { Position = new Point3D(pRange.Count, 0, 0), Text = "p" });
            MainViewport.Children.Add(new TextVisual3D { Position = new Point3D(0, qRange.Count, 0), Text = "q" });
            MainViewport.Children.Add(new TextVisual3D { Position = new Point3D(0, 0, bRange.Count), Text = "b" });
        }
        
        private void AddPoint(TestResult testResult)
        {
            var point3D = new Point3D(
                testResult.pIndex - _ranges.pRange.Start, 
                testResult.qIndex - _ranges.qRange.Start, 
                testResult.b - _ranges.bRange.Start
            );

            var sphere = new SphereVisual3D { Center = point3D, Radius = 0.25 };
            MainViewport.Children.Add(sphere);
        }
        
        private Ranges? RequestRange()
        {
            var dialog = new RangeInputDialog
            {
                Owner = this
            };
            if (dialog.ShowDialog() == true)
            {
                return new Ranges(
                    new Range(dialog.pStart, dialog.pEnd),
                    new Range(dialog.qStart, dialog.qEnd),
                    new Range(dialog.bStart, dialog.bEnd)
                );
            }
            return null;
        }
    }
}
