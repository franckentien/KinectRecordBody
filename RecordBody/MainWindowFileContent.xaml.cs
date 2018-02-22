using System;
using System.Globalization;
using System.IO;
using Microsoft.Kinect;

namespace RecordBody
{
    public partial class MainWindow
    {
        #region AddValue 

        /// <summary>
        ///     Add the value to the Stringbuilder
        /// </summary>
        /// <param name="body">all body information</param>
        /// <param name="idBody">id of body in the tabs </param>
        private void AddValue(Body body, int idBody)
        {
            AddValuePositions(body, idBody);
            AddValueRotation(body, idBody);
        }

        /// <summary>
        ///     Add value for the Joint Positions
        /// </summary>
        /// <param name="body">all body information</param>
        /// <param name="idBody">id of body in the tabs </param>
        private void AddValuePositions(Body body, int idBody)
        {
            var line = _frameId.ToString();

            foreach (var joint in body.Joints.Values)
            {
                line += "," + joint.JointType;
                //if it's goal 

                line += "," + joint.TrackingState;

                // write value of positions
                line += "," + joint.Position.X;
                line += "," + joint.Position.Y;
                line += "," + joint.Position.Z;
            }

            _csvPosition[idBody].AppendLine(line);
        }

        /// <summary>
        ///     Add value for the Joint Rotations
        /// </summary>
        /// <param name="body">all body information</param>
        /// <param name="idBody">id of body in the tabs </param>
        private void AddValueRotation(Body body, int idBody)
        {
            var line = _frameId.ToString();

            foreach (var joint in body.JointOrientations.Values)
            {
                line += "," + joint.JointType;

                //the orientation was a quaternions 
                line += "," + -joint.Orientation.X;
                line += "," + -joint.Orientation.Y;
                line += "," + joint.Orientation.Z;
                line += "," + joint.Orientation.W;
            }
            _csvRotation[idBody].AppendLine(line);
        }

        #endregion

        #region Write value 

        /// <summary>
        ///     write value in the file
        /// </summary>
        private void WriteValue()
        {
            _frameId = 0;
            WriteValuePosition();
            WriteValueRotation();
        }

        /// <summary>
        ///     Write the files for the Joint Position
        /// </summary>
        private void WriteValuePosition()
        {
            //create a folder 
            var file = new DirectoryInfo("Body Position");
            file.Create();

            //get actual time for name of the file 
            var time = DateTime.Now.ToString("hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

            for (var i = 0; i < _csvPosition.Length; i++)
            {
                if (_csvPosition[i] == null) continue;
                if (_csvPosition[i].ToString() != "")
                {
                    File.WriteAllText("Body Position/" + time + "-" + "Body" + (i + 1) + ".csv",
                        _csvPosition[i].ToString());
                }
                _csvPosition[i] = null;
            }
        }

        /// <summary>
        ///     Write the files for the Joints Rotation
        /// </summary>
        private void WriteValueRotation()
        {
            //create a folder 
            var file = new DirectoryInfo("Body Rotation");
            file.Create();

            //get actual time for name of the file 
            var time = DateTime.Now.ToString("hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

            for (var i = 0; i < _csvRotation.Length; i++)
            {
                if (_csvRotation[i] == null) continue;
                if (_csvRotation[i].ToString() != "")
                {
                    File.WriteAllText("Body Rotation/" + time + "-" + "Body" + (i + 1) + ".csv",
                        _csvRotation[i].ToString());
                }
                _csvRotation[i] = null;
            }
        }

        #endregion
    }
}