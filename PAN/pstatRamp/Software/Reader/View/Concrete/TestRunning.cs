/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using IO.Model.Serializable;
using IO.Model.Volatile;

namespace IO.View.Concrete
{
    /// <summary>
    /// Test running form
    /// </summary>
    public partial class TestRunning : Form
    {
        /// <summary>
        /// Progress indicator dimensions
        /// </summary>
        private static readonly int PROGRESS_IMAGE_RADIUS = 55;
        private static readonly int PROGRESS_IMAGE_DIAMETER = PROGRESS_IMAGE_RADIUS + PROGRESS_IMAGE_RADIUS;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TestRunning()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.TestRunning;
        }

        /// <summary>
        /// Reparent any controls on the background image
        /// </summary>
        public override void Reparent()
        {
            // Make a list of controls within the backbround image
            var controls = new List<Control>();

            foreach (Control control in Controls)
            {
                // Ignore the background image itself and the animation
                if ((control != pictureBoxBackground) &&
                    (control != pictureBoxAnimation) &&
                    (control != pictureBoxProgress) &&
                    pictureBoxBackground.Bounds.Contains(control.Bounds))
                {
                    controls.Add(control);
                }
            }

            // Loop through the controls reparenting them
            foreach (var control in controls)
            {
                // Make images transparent
                if (control is PictureBox)
                {
                    Transparent((PictureBox)control, pictureBoxBackground);
                }
                else
                {
                    Reparent(control, pictureBoxBackground);
                }
            }
        }

        /// <summary>
        /// Update the date and time on the form
        /// </summary>
        protected override void UpdateStatus()
        {
            // Call the base method
            base.UpdateStatus();

            lock (ITests.Instance.CurrentTest)
            {
                // Create a bitmap for the background image
                var backgroundImage = new Bitmap(pictureBoxBackground.Width, pictureBoxBackground.Height);

                // Draw the background image
                pictureBoxBackground.DrawToBitmap(backgroundImage, pictureBoxBackground.ClientRectangle);

                pictureBoxProgress.Visible = true;

                // Extract the area of the bitmap that is coverd by the control and set this to the background
                backgroundImage = backgroundImage.Clone(new Rectangle(
                    pictureBoxProgress.Left - pictureBoxBackground.Left,
                    pictureBoxProgress.Top - pictureBoxBackground.Top,
                    pictureBoxProgress.Width, pictureBoxProgress.Height), backgroundImage.PixelFormat);

                // Create and use a graphics object based on this bitmap
                using (var graphics = Graphics.FromImage(backgroundImage))
                {
                    // Intialise some dimensions
                    int halfWidth = pictureBoxProgress.Width / 2;
                    int halfheight = pictureBoxProgress.Height / 2;

                    // Create the progress indicator rectangle
                    var progressRectangle = new Rectangle(halfWidth - PROGRESS_IMAGE_RADIUS,
                        halfheight - PROGRESS_IMAGE_RADIUS, PROGRESS_IMAGE_DIAMETER, PROGRESS_IMAGE_DIAMETER);

                    // Set the smoothing mode to anti-alias
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    // Fill a circle in the background colour
                    graphics.FillEllipse(Form.MAIN_COLOR_BRUSH, progressRectangle);

                    // Add a border around the indicator
                    progressRectangle.Inflate(-Form.BORDER_WIDTH, -Form.BORDER_WIDTH);

                    // Calculate the start angle for the pie from the pecent complete value
                    float startAngle = (3.6f * ITests.Instance.CurrentTest.PercentComplete);

                    // Fill the remaining time pie in the background colour
                    graphics.FillPie(Form.BACKGROUND_COLOR_BRUSH, progressRectangle, startAngle - 90.0f, 360.0f - startAngle);
                }

                // Set the background image and refresh the control
                pictureBoxProgress.BackgroundImage = backgroundImage;
                pictureBoxProgress.Refresh();

                // Check for the end of the test
                if (ISessions.Instance.CurrentSession.CurrentTest != null)
                {
                    // Issue the next command
                    IssueFormCommand(new CommandMessage(FormName, FormCommand.Next));
                }
            }
        }
    }
}
