/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.View
{
    /// <summary>
    /// Data source holder for drag and drop
    /// </summary>
    internal class DataSourceHolder
    {
        /// <summary>
        /// The name of the data source
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Default contructor
        /// </summary>
        /// <param name="name">The name of the data source</param>
        public DataSourceHolder(string name)
        {
            Name = name;
        }
    }
}
