/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using IO.Model.Serializable;

namespace IO.Comms.Tests
{
    /// <summary>
    /// Configuration object for the test
    /// </summary>
    public class Configuration : IConfiguration
    {
        /// <summary>
        /// The available storage
        /// </summary>
        public override long Storage
        {
            get
            {
                return 0;
            }
        }
    }
}
