/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using IO.Scripting;
using IO.FileSystem;

namespace IO.View.Concrete
{
    public partial class UpdateAssay : Form
    {
        /// <summary>
        /// The assay to update
        /// </summary>
        public IAssay Assay { get; set; }

        /// <summary>
        /// The file system to use
        /// </summary>
        public ISimpleFileSystem FileSystem { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public UpdateAssay()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.UpdateInProgress;
            pictureBoxAnimation.Left = 531;
            pictureBoxAnimation.Top = 132;
        }

        /// <summary>
        /// Reset the form
        /// </summary>
        /// <param name="visible">Whether the form is visible</param>
        protected override void ResetForm(bool visible)
        {
            // Call the base class
            base.ResetForm(visible);

            if (visible)
            {
                // Issue the next command
                IssueFormCommand(new CommandMessage(FormName, FormCommand.Next)
                {
                    Parameters = new Dictionary<string, object>() 
                    { 
                        { "Assay", Assay },
                        { "FileSystem", FileSystem },
                    },
                });
            }
        }
    }
}
