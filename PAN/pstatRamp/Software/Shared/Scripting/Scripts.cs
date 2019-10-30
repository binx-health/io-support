/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using IO.FileSystem;

namespace IO.Scripting
{
    /// <summary>
    /// Scripts interface
    /// </summary>
    [XmlRoot("Scripts")]
    public class Scripts : IScripts
    {
        /// <summary>
        /// The current path for the script files
        /// </summary>
        private string path = null;

        /// <summary>
        /// The list of variables
        /// </summary>
        private Dictionary<string, IScriptVariable> variables = null;

        /// <summary>
        /// The path from which scripts are loaded
        /// </summary>
        [XmlIgnore]
        public override string Path
        {
            get
            {
                return path;
            }
            set
            {
                if (path != value)
                {
                    // Set the path
                    path = value;

                    // Reload the scripts
                    Reload();
                }
            }
        }

        /// <summary>
        /// Enumeration of script variables
        /// </summary>
        [XmlIgnore]
        public override IDictionary<string, IScriptVariable> Variables
        {
            get
            {
                return variables;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Scripts()
        {
            // Initialise the variables
            variables = new Dictionary<string, IScriptVariable>()
            {
                { "v1", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "v2", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "v3", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "v4", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "v5", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "v6", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "v7", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "v8", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "v9", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "v10", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "v11", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "v12", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "v13", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "v14", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "v15", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "v16", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "v17", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "p1", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "p2", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "s1", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "m1", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "dpr1", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "l1", new ScriptVariable() { VariableType = ScriptVariableType.VirtualDevice } },
                { "l2", new ScriptVariable() { VariableType = ScriptVariableType.VirtualDevice } },
                { "l3", new ScriptVariable() { VariableType = ScriptVariableType.VirtualDevice } },
                { "l4", new ScriptVariable() { VariableType = ScriptVariableType.VirtualDevice } },
                { "l5", new ScriptVariable() { VariableType = ScriptVariableType.VirtualDevice } },
                { "ps1", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "ps2", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "ps3", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "ps4", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "therm1", new ScriptVariable() { VariableType = ScriptVariableType.VirtualDevice } },
                { "therm1.top", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "therm1.bottom", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "therm2", new ScriptVariable() { VariableType = ScriptVariableType.VirtualDevice } },
                { "therm2.top", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "therm2.bottom", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "therm3", new ScriptVariable() { VariableType = ScriptVariableType.VirtualDevice } },
                { "therm3.top", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "therm3.bottom", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "pstat1", new ScriptVariable() { VariableType = ScriptVariableType.VirtualDevice } },
                { "pstat2", new ScriptVariable() { VariableType = ScriptVariableType.VirtualDevice } },
                { "pstat3", new ScriptVariable() { VariableType = ScriptVariableType.VirtualDevice } },
                { "pstat4", new ScriptVariable() { VariableType = ScriptVariableType.VirtualDevice } },
                { "opto1", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "opto2", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "opto3", new ScriptVariable() { VariableType = ScriptVariableType.PhysicalDevice } },
                { "res1", new ScriptVariable() { VariableType = ScriptVariableType.VirtualDevice } },
                { "res2", new ScriptVariable() { VariableType = ScriptVariableType.VirtualDevice } },
                { "res3", new ScriptVariable() { VariableType = ScriptVariableType.VirtualDevice } },
            };
        }

        /// <summary>
        /// Reload the scripts from the path
        /// </summary>
        public override void Reload()
        {
            // Clear all of the current scripts
            Clear();

            // Load the scripts from the path
            LoadScripts();
        }

        /// <summary>
        /// Set the value for a script
        /// </summary>
        /// <param name="name">The name of the script</param>
        /// <param name="value">The new value or null to delete the script</param>
        public override IScript SetScript(string name, string value)
        {
            // get the existing script
            IScript script = this.Where(x => string.Compare(x.Name, name, true) == 0).FirstOrDefault();

            // Check for a new script
            if (script == null)
            {
                // Create a new script
                Add(script = new Script(name, this));
            }

            // Check for a null value
            if (value == null)
            {
                // Remove the script
                this.Remove(script);

                // Check the scripts for consistency
                CheckConsistency();
            }
            else if (value != script.Value)
            {
                // Set the script value
                script.Value = value;

                // Set the modified and loaded flags
                script.Modified = true;
                script.Loaded = false;

                // Check the scripts for consistency
                CheckConsistency();
            }

            // Return the script
            return script;
        }

        internal bool AddDeviceName(string name, string alias)
        {
            IScriptVariable variable;

            // Find the named variable 
            if (variables.TryGetValue(name, out variable))
            {
                // Check that it is a physical or virtual device
                if ((variable.VariableType == ScriptVariableType.PhysicalDevice) ||
                    (variable.VariableType == ScriptVariableType.VirtualDevice))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Load the scripts from the current path
        /// </summary>
        private void LoadScripts()
        {
            // Get all the script files on the path and create a named script object
            var files = ILocalFileSystem.Instance.GetFiles(path);

            if (files != null)
            {
                foreach (var fileName in files)
                {
                    Add(new Script(fileName, this));
                }
            }

            // Load the scripts from the files
            foreach (var script in this)
            {
                script.Value = ILocalFileSystem.Instance.ReadTextFile(path +
                    System.IO.Path.DirectorySeparatorChar + script.Name);
            }

            // Check the scripts for consistency
            CheckConsistency();
        }

        /// <summary>
        /// Check all scripts for consitency
        /// </summary>
        private void CheckConsistency()
        {
            // Clear all consistency errors
            foreach (Script script in this)
            {
                script.ClearConsistencyErrors();
            }

            // Create a distinct list of scripts called by other scripts (not themselves)
            IEnumerable<string> calledScripts = this.SelectMany(
                x => x.Children.Where(y => string.Compare(y, x.Name, true) != 0)).Distinct();

            // Find the root scripts (scripts not called by another script)
            var rootScripts = this.Select(x => x.Name).Except(calledScripts, 
                StringComparer.CurrentCultureIgnoreCase);

            // Loop through the root scripts checking consistency
            foreach (var scriptName in rootScripts)
            {
                // Create a new variables object from the base variables
                var allVariables = new Dictionary<string, IScriptVariable>(variables);

                // Call check consistency on this script
                CheckConsistency(this.Where(x => string.Compare(x.Name, scriptName, true) == 0).
                    FirstOrDefault(), allVariables);
            }
        }

        /// <summary>
        /// Check the consistency of a script reccursively
        /// </summary>
        /// <param name="script">The script to check</param>
        /// <param name="allVariables">The list of all variables</param>
        /// <param name="ancestry">The ancestry of this script</param>
        private void CheckConsistency(IScript script, Dictionary<string, IScriptVariable> allVariables,
            Stack<string> ancestry = null)
        {
            // We can only test know types of script
            if (script is Script)
            {
                // Get the underlying script object
                var scriptObject = (Script)script;

                // Create the ancestry if required
                if (ancestry == null)
                {
                    ancestry = new Stack<string>();
                }

                // Check for reentrant code
                if (ancestry.Contains(script.Name, StringComparer.CurrentCultureIgnoreCase))
                {
                    scriptObject.AddConsistencyError("Script is reentrant");
                    return;
                }

                // Create the list of new variables
                foreach (var variable in script.NewVariables)
                {
                    // Try to get an existing variable
                    IScriptVariable existingVariable;

                    if (allVariables.TryGetValue(variable.Key, out existingVariable))
                    {
                        // Compare the variables
                        if ((existingVariable.VariableType != variable.Value.VariableType) ||
                            (existingVariable.Value != variable.Value.Value))
                        {
                            scriptObject.AddConsistencyError("Script redefines variable '" + 
                                variable.Key + "'");
                        }
                    }
                    else
                    {
                        // Add this variable to the script
                        allVariables[variable.Key] = variable.Value;
                    }
                }

                // Check the used variables against all variables
                foreach (var variableName in script.UsedVariables)
                {
                    if (allVariables.ContainsKey(variableName) == false)
                    {
                        scriptObject.AddConsistencyError("Script uses undefined variable '" + 
                            variableName + "'");
                    }
                }

                // Push the script name onto the ancestry
                ancestry.Push(script.Name);

                // Loop through the child scripts checking them reccursively
                foreach (var scriptName in script.Children)
                {
                    // Get the child script
                    var childScript = this.Where(
                        x => string.Compare(x.Name, scriptName, true) == 0).FirstOrDefault();

                    // Check that the script exists
                    if (childScript == null)
                    {
                        scriptObject.AddConsistencyError("Script calls unknown script '" + scriptName + "'");
                    }
                    else
                    {
                        CheckConsistency(childScript, allVariables, ancestry);
                    }
                }

                // Pop the script name off the ancestry
                ancestry.Pop();
            }
        }
    }
}
