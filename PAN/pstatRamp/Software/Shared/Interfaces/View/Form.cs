/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Globalization;
using System.Collections.Generic;

namespace IO.View
{
    public interface IForm
    {
        /// <summary>
        /// Form name accessor
        /// </summary>
        string FormName { get; }

        /// <summary>
        /// Close all the windows in the stack
        /// </summary>
        void Unwind();

        /// <summary>
        /// Show all the forms in the stack
        /// </summary>
        void ShowAll();

        /// <summary>
        /// Hide all the forms in the stack
        /// </summary>
        void HideAll();

        /// <summary>
        /// Unwind the form stack to the last menu position
        /// </summary>
        void Menu();

        /// <summary>
        /// Unwind the form stack to the home position
        /// </summary>
        void Home();

        /// <summary>
        /// Unwind the form stack one form
        /// </summary>
        void Back();

        /// <summary>
        /// Show a form from a menu
        /// </summary>
        /// <param name="form">The name of the form to show</param>
        /// <param name="goHomeFirst">Flag to go home before showing the form</param>
        /// <param name="parameters">The parameters for the form</param>
        void ShowFormFromMenu(string name, bool goHomeFirst = false, 
            Dictionary<string, object> parameters = null);

        /// <summary>
        /// Show a form from a form
        /// </summary>
        /// <param name="form">The name of the form to show</param>
        /// <param name="parameters">The parameters for the form</param>
        void ShowFormFromForm(string name, Dictionary<string, object> parameters = null);

        /// <summary>
        /// Show a message to the user
        /// </summary>
        /// <param name="resource">The resource name for the message</param>
        /// <param name="arg">Optional argument for the resource</param>
        void ShowMessage(string resource, object arg = null);

        /// <summary>
        /// Logout the current user
        /// </summary>
        void Logout();

        /// <summary>
        /// Disable the form, kick off the wait timer and issue the passed form command
        /// </summary>
        /// <param name="command">The command message</param>
        void IssueFormCommand(CommandMessage commandMessage);

        /// <summary>
        /// Reset the form
        /// </summary>
        void Reset();

        /// <summary>
        /// Reapply the resources to the stack
        /// </summary>
        /// <param name="cultureInfo">The new culture info</param>
        void ReapplyResources(CultureInfo cultureInfo);
    }
}
