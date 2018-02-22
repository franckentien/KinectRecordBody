using System;
using System.Windows;

namespace RecordBody
{
    public partial class MainWindow
    {
        #region player

        /// <summary>
        ///     Get the id for trackbody tab of a track body object
        ///     if is unknow create a new object
        /// </summary>
        /// <param name="trackingId">trackingId give by kinect sensor </param>
        /// <returns>index id in the tab of body </returns>
        private static int GetBodyId(ulong trackingId)
        {
            var firstId = 5;
            for (var i = 0; i < TrackBody.Length; i++)
            {
                if (TrackBody[i] == null)
                {
                    // save the lower id null
                    if (firstId > i) firstId = i;
                }
                else
                {
                    if (TrackBody[i].Id == trackingId)
                    {
                        //if the trackid is know return the index id 
                        return i;
                    }
                }
            }
            //if i dont found id i create a new object and return this index in tab
            TrackBody[firstId] = new TrackBody(trackingId, firstId);
            return firstId;
        }

        /// <summary>
        ///     Remove player if is inactive for more 5 seconds
        /// </summary>
        private void RemovePlayer()
        {
            var keepActive = false;
            for (var i = 0; i < TrackBody.Length; i++)
            {
                if (TrackBody[i] == null) continue;
                if ((int) DateTime.UtcNow.Subtract(DateStart).TotalSeconds - TrackBody[i].LastActivity > 5)
                {
                    TrackBody[i] = null;
                }
                else
                {
                    keepActive = true;
                }
            }
            if (keepActive || !_recordActive) return;
            _recordActive = false;
            StopRecord.Visibility = Visibility.Hidden;
            StartRecord.Visibility = Visibility.Visible;
            WriteValue();
            _frameId = 0;
        }

        #endregion
    }
}