using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace RecordBody
{
    public static class Extensions
    {
        #region Camera

        public static ImageSource ToBitmap(this ColorFrame frame)
        {
            var width = frame.FrameDescription.Width;
            var height = frame.FrameDescription.Height;
            var format = PixelFormats.Bgr32;

            var pixels = new byte[width*height*((format.BitsPerPixel + 7)/8)];

            if (frame.RawColorImageFormat == ColorImageFormat.Bgra)
            {
                frame.CopyRawFrameDataToArray(pixels);
            }
            else
            {
                frame.CopyConvertedFrameDataToArray(pixels, ColorImageFormat.Bgra);
            }

            var stride = width*format.BitsPerPixel/8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }

        # endregion

        #region Body

        private static Point Scale(this Joint joint, CoordinateMapper mapper)
        {
            var point = new Point();

            var colorPoint = mapper.MapCameraPointToColorSpace(joint.Position);
            point.X = float.IsInfinity(colorPoint.X) ? 0.0 : colorPoint.X;
            point.Y = float.IsInfinity(colorPoint.Y) ? 0.0 : colorPoint.Y;

            return point;
        }

        #endregion

        #region Drawing

        /// <summary>
        ///     Function re-used for draw skeleton
        /// </summary>
        public static void DrawSkeleton(this Canvas canvas, Body body, CoordinateMapper mapper, TrackBody trackBody)
        {
            if (body == null) return;

            foreach (var joint in body.Joints.Values)
            {
                canvas.DrawPointValue(joint, mapper);
            }

            #region draw line 

            canvas.DrawLine(body.Joints[JointType.Head], body.Joints[JointType.Neck], mapper, trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.Neck], body.Joints[JointType.SpineShoulder], mapper,
                trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderLeft], mapper,
                trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderRight], mapper,
                trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.SpineMid], mapper,
                trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.ShoulderLeft], body.Joints[JointType.ElbowLeft], mapper,
                trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.ShoulderRight], body.Joints[JointType.ElbowRight], mapper,
                trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.ElbowLeft], body.Joints[JointType.WristLeft], mapper,
                trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.ElbowRight], body.Joints[JointType.WristRight], mapper,
                trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.WristLeft], body.Joints[JointType.HandLeft], mapper,
                trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.WristRight], body.Joints[JointType.HandRight], mapper,
                trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.HandLeft], body.Joints[JointType.HandTipLeft], mapper,
                trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.HandRight], body.Joints[JointType.HandTipRight], mapper,
                trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.WristLeft], body.Joints[JointType.ThumbLeft], mapper,
                trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.WristRight], body.Joints[JointType.ThumbRight], mapper,
                trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.SpineMid], body.Joints[JointType.SpineBase], mapper,
                trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.SpineBase], body.Joints[JointType.HipLeft], mapper,
                trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.SpineBase], body.Joints[JointType.HipRight], mapper,
                trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.HipLeft], body.Joints[JointType.KneeLeft], mapper, trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.HipRight], body.Joints[JointType.KneeRight], mapper,
                trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.KneeLeft], body.Joints[JointType.AnkleLeft], mapper,
                trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.KneeRight], body.Joints[JointType.AnkleRight], mapper,
                trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.AnkleLeft], body.Joints[JointType.FootLeft], mapper,
                trackBody.ColorLine);
            canvas.DrawLine(body.Joints[JointType.AnkleRight], body.Joints[JointType.FootRight], mapper,
                trackBody.ColorLine);

            #endregion
        }

        /// <summary>
        ///     take all value and edit it for draw point
        /// </summary>
        private static void DrawPointValue(this Panel canvas, Joint joint, CoordinateMapper mapper)
        {
            if (joint.TrackingState == TrackingState.NotTracked) return;

            var point = joint.Scale(mapper);

            if (point.X == 0 && point.Y == 0) return;

            #region parametre

            var colorBrush = new SolidColorBrush(Colors.Green) {Opacity = 1};
            const int size = 30;

            #endregion

            DrawPointDrawing(canvas, size, colorBrush, point);
        }

        /// <summary>
        ///     draw point with value
        /// </summary>
        private static void DrawPointDrawing(this Panel canvas, int size, Brush colorBrush, Point point)
        {
            if (point.X == 0 && point.Y == 0) return;

            #region drawingPoint 

            // draw ellipse with parameter 
            var ellipse = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = colorBrush
            };

            //set ellipse positions 
            Canvas.SetLeft(ellipse, point.X - ellipse.Width/2);
            Canvas.SetTop(ellipse, point.Y - ellipse.Height/2);

            //display ellipse
            canvas.Children.Add(ellipse);

            #endregion
        }

        /// <summary>
        ///     re-used function for draw line
        /// </summary>
        private static void DrawLine(this Panel canvas, Joint first, Joint second, CoordinateMapper mapper,
            Brush colorline)
        {
            if (first.TrackingState == TrackingState.NotTracked || second.TrackingState == TrackingState.NotTracked)
                return;

            var firstPoint = first.Scale(mapper);
            Point secondPoint = second.Scale(mapper);

            if ((firstPoint.X == 0 && firstPoint.Y == 0) || (secondPoint.X == 0 && secondPoint.Y == 0)) return;

            var line = new Line
            {
                X1 = firstPoint.X,
                Y1 = firstPoint.Y,
                X2 = secondPoint.X,
                Y2 = secondPoint.Y,
                StrokeThickness = 10,
                Stroke = colorline
            };
            canvas.Children.Add(line);
        }

        #endregion
    }
}