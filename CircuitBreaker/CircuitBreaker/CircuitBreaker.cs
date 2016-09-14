using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public class CircuitBreaker
    {
        private readonly object monitor = new object();
        private CircuitBreakerState state;

        public CircuitBreaker(int threshold, TimeSpan timeout)
        {
            if (threshold < 1)
            {
                throw new ArgumentOutOfRangeException("threshold", "Threshold should be greater than 0");
            }

            if (timeout.TotalMilliseconds < 1)
            {
                throw new ArgumentOutOfRangeException("timeout", "Timeout should be greater than 0");
            }

            Threshold = threshold;
            Timeout = timeout;
            MoveToClosedState();
        }

        public int Failures { get; private set; }
        public int Threshold { get; private set; }
        public TimeSpan Timeout { get; private set; }
        public bool IsClosed
        {
            get { return state.Update() is ClosedState; }
        }

        public bool IsOpen
        {
            get { return state.Update() is OpenState; }
        }

        public bool IsHalfOpen
        {
            get { return state.Update() is HalfOpenState; }
        }

        public double TimeoutRemaining()
        {
            OpenState os = state.Update() as OpenState;
            return (os == null) ? 0.0 : os.TimeoutRemaining();
        }

        internal CircuitBreakerState MoveToClosedState()
        {
            state = new ClosedState(this);
            return state;
        }

        internal CircuitBreakerState MoveToOpenState()
        {
            state = new OpenState(this);
            return state;
        }

        internal CircuitBreakerState MoveToHalfOpenState()
        {
            state = new HalfOpenState(this);
            return state;
        }

        internal void IncreaseFailureCount()
        {
            Failures++;
        }

        internal void ResetFailureCount()
        {
            Failures = 0;
        }

        public bool IsThresholdReached()
        {
            return Failures >= Threshold;
        }

        private Exception exceptionFromLastAttemptCall = null;

        public Exception GetExceptionFromLastAttemptCall()
        {
            return exceptionFromLastAttemptCall;
        }

        // Run the code
        public async Task<bool> AttemptCall(Func<Task> protectedCode)
        {
            this.exceptionFromLastAttemptCall = null;
            lock (monitor)
            {
                state.ProtectedCodeIsAboutToBeCalled();
                if (state is OpenState)
                {
                    return false; // Stop execution of this method
                }
            }

            try
            {
                await protectedCode();
            }
            catch (Exception e)
            {
                this.exceptionFromLastAttemptCall = e;
                lock (monitor)
                {
                    state.ActUponException(e);
                }
                return false; // Stop execution of this method
            }

            lock (monitor)
            {
                state.ProtectedCodeHasBeenCalled();
            }
            return true;
        }

        public void Close()
        {
            lock (monitor)
            {
                MoveToClosedState();
            }
        }

        public void Open()
        {
            lock (monitor)
            {
                MoveToOpenState();
            }
        }
    }
}
