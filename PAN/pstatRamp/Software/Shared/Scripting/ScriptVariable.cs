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
    public class ScriptVariable : IScriptVariable
    {
        /// <summary>
        /// The variable type
        /// </summary>
        public override ScriptVariableType VariableType { get; set; }

        /// <summary>
        /// The variable value
        /// </summary>
        public override object Value { get; set; }
    }
}
