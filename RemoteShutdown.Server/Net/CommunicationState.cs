using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteShutdown.Net
{
    /// <summary>
    /// Define RemoteShutdown.Net.ICommunicationObject the state.
    /// </summary>
    public enum CommunicationState
    {
        /// <summary>
        /// Indicates that the communication object has been instantiated and can be configured, but not yet open or can not be used.
        /// </summary>
        Created = 0,
        /// <summary>
        /// Indicates that the communication object is converted from RemoteShutdown.Net.CommunicationState.Created state to RemoteShutdown.Net.CommunicationState.Opened state.
        /// </summary>
        Opening = 1,
        /// <summary>
        /// Indicate communication object is currently open and available for use at any time.
        /// </summary>
        Opened = 2,
        /// <summary>
        /// Indicates that the communication object is converted to RemoteShutdown.Net.CommunicationState.Closed state.
        /// </summary>
        Closing = 3,
        /// <summary>
        /// Indicate communication object has been closed and is no longer available.
        /// </summary>
        Closed = 4,
        /// <summary>
        /// Indicate communication object error has occurred, can not be recovered and is no longer available.
        /// </summary>
        Faulted = 5,
    }
}
