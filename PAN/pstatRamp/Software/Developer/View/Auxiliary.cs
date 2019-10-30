/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Windows.Forms;

namespace IO.View
{
    /// <summary>
    /// Auxiliary class containing extension methods
    /// </summary>
    internal static class Auxiliary
    {
        /// <summary>
        /// Extension method to add a tree node to a collection that is flagged as expanded
        /// </summary>
        /// <param name="treeNodeCollection">The collection to add the node to</param>
        /// <param name="key">The key for the node</param>
        /// <param name="text">The text for the node</param>
        /// <param name="imageIndex">The image index for the node</param>
        /// <returns>The appended node</returns>
        public static TreeNode AddExpanded(this TreeNodeCollection treeNodeCollection, string key, 
            string text, int imageIndex)
        {
            // Create the new node and a dummy child member
            var newNode = treeNodeCollection.Add(key, text);
            var dummyTreeNode = newNode.Nodes.Add(string.Empty);

            // Set the image and selected image index
            newNode.ImageIndex = newNode.SelectedImageIndex = imageIndex;

            // Expand the node and then remove the child
            newNode.Expand();
            newNode.Nodes.Remove(dummyTreeNode);

            return newNode;
        }
    }
}
