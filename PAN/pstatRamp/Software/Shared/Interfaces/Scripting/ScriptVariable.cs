/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.Scripting
{
    /// <summary>
    /// Script variable object
    /// </summary>
    public abstract class IScriptVariable
    {
        /// <summary>
        /// The variable type
        /// </summary>
        public abstract ScriptVariableType VariableType { get; set; }

        /// <summary>
        /// The variable value
        /// </summary>
        public abstract object Value { get; set; }
    }
}
