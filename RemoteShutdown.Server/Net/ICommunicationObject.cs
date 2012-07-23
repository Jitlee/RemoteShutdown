using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteShutdown.Net
{
    /// <summary>
    /// The basic interface of communication objects
    /// </summary>
    public interface ICommunicationObject
    {
        /// <summary>
        /// Get the current state of the object-oriented communication.
        /// </summary>
        CommunicationState State
        {
            get;
        }

        /// <summary>
        /// Occur when the communication object is open for the first time.
        /// </summary>
        event EventHandler Opening;

        /// <summary>
        /// Occur when the communication object to complete the conversion from being open to open.
        /// </summary>
        event EventHandler Opened;

        /// <summary>
        /// Occur when the communication object first enters the state is shutting down.
        /// </summary>
        event EventHandler Closing;

        /// <summary>
        /// Occur when the communication object completes conversion from being shut down state to the closed state.
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Occur when the communication object first enters the error state.
        /// </summary>
        event EventHandler Faulted;

        IAsyncResult BeginOpen(AsyncCallback callback, object state);

        IAsyncResult BeginClose(AsyncCallback callback, object state);

        void EndOpen(IAsyncResult result);

        void EndClose(IAsyncResult result);

        /// <summary>
        /// Communication object from the created state into the opened state.
        /// </summary>
        void Open();

        /// <summary>
        /// Communication object from its current state of the conversion to the closed state.
        /// </summary>
        void Close();

        Exception GetLastError();
    }
}
