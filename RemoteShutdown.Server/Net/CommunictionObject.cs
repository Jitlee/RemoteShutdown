using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteShutdown.Net
{
    public abstract class CommunictionObject : ICommunicationObject
    {
        private ILogger _logger;

        private Exception _exception;

        /// <summary>
        /// Initialize the System.ServiceModel.Channels.CommunicationObject class new instance.
        /// </summary>
        /// <remarks>protected</remarks>
        /// <returns>the new instance</returns>
        protected CommunictionObject() : this(new object())
        {
        }

        /// <summary>
        /// Mutually exclusive lock initialization RemoteShutdown.Net.CommunicationObject class of new instances to protect the state transitions specified.
        /// </summary>
        /// <remarks>protected</remarks>
        /// <param name="mutex">Mutually exclusive lock protects the class instance in the process of state transitions.</param>
        /// <returns>the new instance</returns>
        protected CommunictionObject(object mutex)
        {
            this.ThisLock = mutex;
            this.eventSender = this;
        }

        private object eventSender;

        /// <summary>
        /// Gets a value indicating whether the communication object has been released.
        /// </summary>
        protected bool IsDisposed 
        {
            get;
            private set;
        }
 
        /// <summary>
        /// Get the current state of the object-oriented communication.
        /// </summary>
        public CommunicationState State
        {
            get;
            protected set;
        }

        protected object ThisLock
        {
            get;
            private set;
        }

        /// <summary>
        /// Occur when the communication object is open for the first time.
        /// </summary>
        public event EventHandler Opening;

        /// <summary>
        /// Occur when the communication object to complete the conversion from being open to open.
        /// </summary>
        public event EventHandler Opened;

        /// <summary>
        /// Occur when the communication object first enters the state is shutting down.
        /// </summary>
        public event EventHandler Closing;

        /// <summary>
        /// Occur when the communication object completes conversion from being shut down state to the closed state.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Occur when the communication object first enters the error state.
        /// </summary>
        public event EventHandler Faulted;

        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            try
            {
                State = CommunicationState.Opening;
                OnOpenning();
                return OnBeginOpen(callback, state);
            }
            catch (Exception ex)
            {
                GetLogger().Debug("[BeginOpen] Called : {0}", ex.Message);
                Fault(ex);
                return null;
            }
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            try
            {
                State = CommunicationState.Closed;
                OnClosing();
                return OnBeginClose(callback, state);
            }
            catch (Exception ex)
            {
                GetLogger().Debug("[BeginClose] Called : {0}", ex.Message);
                Fault(ex);
                return null;
            }
        }

        public void EndOpen(IAsyncResult result)
        {
            try
            {
                OnEndOpen(result);
                State = CommunicationState.Opened;
                OnOpened();
            }
            catch (Exception ex)
            {
                GetLogger().Debug("[EndOpen] Called : {0}", ex.Message);
                Fault(ex);
            }
        }

        public void EndClose(IAsyncResult result)
        {
            try
            {
                OnEndClose(result);
                OnClosed();
            }
            catch (Exception ex)
            {
                GetLogger().Debug("[EndClose] Called : {0}", ex.Message);
                Fault(ex);
            }
        }

        public Exception GetLastError()
        {
            return GetLastError(_exception);
        }

        private Exception GetLastError(Exception ex)
        {
            if (null != ex.InnerException)
            {
                return GetLastError(ex.InnerException);
            }
            return ex;
        }

        protected abstract void OnOpen();

        protected abstract void OnClose();

        protected abstract IAsyncResult OnBeginOpen(AsyncCallback callback, object state);

        protected abstract IAsyncResult OnBeginClose(AsyncCallback callback, object state);

        protected abstract void OnEndOpen(IAsyncResult result);

        protected abstract void OnEndClose(IAsyncResult result);

        /// <summary>
        /// Communication object from the created state into the opened state.
        /// </summary>
        public void Open()
        {
            try
            {
                State = CommunicationState.Opening;
                OnOpenning();
                OnOpen();

                lock (ThisLock)
                {
                    if (State == CommunicationState.Faulted
                        || State != CommunicationState.Opening)
                    {
                        return;
                    }

                    State = CommunicationState.Opened;
                }
                OnOpened();
            }
            catch (Exception ex)
            {
                //GetLogger().Debug("[Open] Called : {0}", ex.Message);
                //Fault(ex);
                throw ex;
            } 
        }

        /// <summary>
        /// Communication object from its current state of the conversion to the closed state.
        /// </summary>
        public void Close()
        {
            try
            {
                if (State == CommunicationState.Closed)
                {
                    return;
                }

                State = CommunicationState.Closing;

                OnClosing();

                OnClose();

                if (State != CommunicationState.Closing)
                {
                    return;
                }
                State = CommunicationState.Closed;

                OnClosed();
            }
            catch (Exception ex)
            {
                //GetLogger().Debug("[Close] Called : {0}", ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Communication object from the current state of the transition to an error state.
        /// </summary>
        protected void Fault()
        {
            //lock (ThisLock)
            //{
                if (((State == CommunicationState.Closed) || (State == CommunicationState.Closing)) || (State == CommunicationState.Faulted))
                {
                    return;
                }
                this.State = CommunicationState.Faulted;
            //}
            OnFaulted();
        }

        protected void Fault(Exception ex)
        {
            _exception = ex;
            Fault();
        }

        /// <summary>
        /// Communication object into the opened state of the process is called.
        /// </summary>
        protected virtual void OnOpened()
        {
            if (null != Opened)
            {
                Opened(this.eventSender, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Communication object conversion process is open call.
        /// </summary>
        protected virtual void OnOpenning()
        {
            if (null != Opening)
            {
                Opening(this.eventSender, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Communication object is converted to the state is shutting down the process of call.
        /// </summary>
        protected virtual void OnClosed()
        {
            if (null != Closed)
            {
                Closed(this.eventSender, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Communication object is converted to the state is shutting down the process of call.
        /// </summary>
        protected virtual void OnClosing()
        {
            if (null != Closing)
            {
                Closing(this.eventSender, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Call to a synchronization error operation, which led to the communication object is converted to an error state, the method is to insert the handle of the communications object.
        /// </summary>
        /// <remarks>Call to a synchronization error operation, which led to the communication object is converted to an error state, the method is to insert the handle of the communications object.</remarks>
        protected virtual void OnFaulted()
        {
            if (null != Faulted)
            {
                try
                {
                    Faulted(this.eventSender, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    GetLogger().Debug("[OnFaulted] Called : {0}.", ex.Message);
                }
            }
        }

        private ILogger GetLogger()
        {
            return _logger ?? (_logger = LoggerFactory.GetLogger(typeof(CommunictionObject).FullName));
        }
    }
}
