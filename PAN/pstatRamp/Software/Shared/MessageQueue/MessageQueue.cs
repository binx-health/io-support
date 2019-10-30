/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Threading;
using System.Collections.Generic;

namespace IO.MessageQueue
{
    /// <summary>
    /// Generic message queue object
    /// </summary>
    /// <typeparam name="T">The object type representing the message which must be nullable</typeparam>
    public class MessageQueue<T> : EventWaitHandle
        where T: class
    {
        /// <summary>
        /// The underlying generic queue object
        /// </summary>
        private Queue<T> messageQueue = new Queue<T>();

        /// <summary>
        /// The queue length
        /// </summary>
        public int Length
        {
            get
            {
                return messageQueue.Count;
            }
        }

        /// <summary>
        /// Default constructor initialises the event
        /// </summary>
        public MessageQueue()
            : base(false, EventResetMode.AutoReset)
        {
        }

        /// <summary>
        /// Push a message onto the end of the queue (threadsafe)
        /// </summary>
        /// <param name="message">The message object</param>
        public void Push(T message)
        {
            // Lock the message queue and queue the message
            lock (messageQueue)
            {
                messageQueue.Enqueue(message);
            }

            // Set the event
            Set();
        }

        /// <summary>
        /// Pop a message from the head of the queue (threadsafe)
        /// </summary>
        /// <returns>The object formerly at the head of the queue or null if the queue is empty</returns>
        public T Pop()
        {
            // Lock the message queue and conditionally dequeue the message
            lock (messageQueue)
            {
                return (messageQueue.Count > 0) ? messageQueue.Dequeue() : (T)null;
            }
        }
    }
}
