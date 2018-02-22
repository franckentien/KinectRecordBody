using System.Windows.Media;

namespace RecordBody
{
    public class TrackBody
    {
        #region constructor 

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="id">id body kineck</param>
        /// <param name="player">id player in tab</param>
        public TrackBody(ulong id, int player)
        {
            Id = id;
            switch (player)
            {
                case 0:
                    ColorLine = new SolidColorBrush(Colors.Lime);
                    break;
                case 1:
                    ColorLine = new SolidColorBrush(Colors.DeepPink);
                    break;
                case 2:
                    ColorLine = new SolidColorBrush(Colors.Blue);
                    break;
                case 3:
                    ColorLine = new SolidColorBrush(Colors.Red);
                    break;
                case 4:
                    ColorLine = new SolidColorBrush(Colors.Yellow);
                    break;
                case 5:
                    ColorLine = new SolidColorBrush(Colors.White);
                    break;
            }
        }

        #endregion

        #region function 

        /// <summary>
        ///     Update all data information for body
        /// </summary>
        /// <param name="lastActivity">int for the actual timestamp</param>
        public void EditBody(int lastActivity)
        {
            LastActivity = lastActivity;
        }

        #endregion

        #region variable

        //sensor variable 
        /// <summary>
        ///     TrackingId of sensor
        /// </summary>
        internal ulong Id { get; private set; }

        //personal variable 
        /// <summary>
        ///     personal color line for drawing
        /// </summary>
        internal SolidColorBrush ColorLine { get; private set; }

        //body information 
        /// <summary>
        ///     last activity track of user
        /// </summary>
        internal int LastActivity { get; private set; }

        #endregion
    }
}