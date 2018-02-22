using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows;
using Microsoft.Kinect;

namespace RecordBody
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ja-JP");
        }

        #endregion

        private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            // Color
            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null) Camera.Source = frame.ToBitmap();
            }

            // Body
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame == null) return;
                RemovePlayer(); //remove inactive player 
                Canvas.Children.Clear();

                _bodies = new Body[frame.BodyFrameSource.BodyCount];

                frame.GetAndRefreshBodyData(_bodies);

                foreach (var body in _bodies)
                {
                    if (body == null) continue;
                    if (!body.IsTracked) continue;
                    //get activeBody id for edit it 
                    var activeBody = GetBodyId(body.TrackingId);

                    //draw skeleton 
                    Canvas.DrawSkeleton(body, _sensor.CoordinateMapper, TrackBody[activeBody]);

                    //update all body information 
                    TrackBody[activeBody].EditBody((int) DateTime.UtcNow.Subtract(DateStart).TotalSeconds);
                    if (_recordActive) AddValue(body, activeBody);
                }
                if (_recordActive) _frameId++;
            }
        }

        #region Members

        private KinectSensor _sensor;
        private MultiSourceFrameReader _reader;
        private IList<Body> _bodies;

        /// <summary>
        ///     True if the record is active
        /// </summary>
        private bool _recordActive;

        //body
        /// <summary>
        ///     Tab of 6 body will be track by the Sensor
        /// </summary>
        private static readonly TrackBody[] TrackBody = new TrackBody[6];

        /// <summary>
        ///     date when the soft launch for compare date.
        /// </summary>
        private static readonly DateTime DateStart = DateTime.Now;

        //file ressources 
        /// <summary>
        ///     Tab of Stringbuilder for save body information
        /// </summary>
        private readonly StringBuilder[] _csvPosition = new StringBuilder[6];

        private readonly StringBuilder[] _csvRotation = new StringBuilder[6];

        /// <summary>
        ///     id of frame for each line of body
        /// </summary>
        private int _frameId;

        #endregion

        #region Event handlers

        /// <summary>
        ///     Initialize value ont load of the windows
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _sensor = KinectSensor.GetDefault();

            if (_sensor == null) return;
            _sensor.Open();

            _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Body);
            _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
        }

        /// <summary>
        ///     Free the sensor when the user close the windows
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            _reader?.Dispose();
            _sensor?.Close();
        }

        /// <summary>
        ///     Action when the user click on Start record button
        ///     reset value and display
        ///     initialize StringBuilder
        /// </summary>
        private void StartRecord_Click(object sender, RoutedEventArgs e)
        {
            _recordActive = true;
            StartRecord.Visibility = Visibility.Hidden;
            StopRecord.Visibility = Visibility.Visible;
            for (var i = 0; i < _csvPosition.Length; i++)
            {
                _csvPosition[i] = new StringBuilder();
                _csvRotation[i] = new StringBuilder();
            }
        }

        /// <summary>
        ///     Action when the user click on Stop record button
        ///     Write value and change display
        /// </summary>
        private void StopRecord_Click(object sender, RoutedEventArgs e)
        {
            _recordActive = false;
            WriteValue();
            StartRecord.Visibility = Visibility.Visible;
            StopRecord.Visibility = Visibility.Hidden;
        }

        #endregion
    }
}