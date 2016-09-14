using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    class ClosedState : CircuitBreakerState
    {
        // Call the Base Class Constructor and reset the failure count to 0
        public ClosedState(CircuitBreaker circuitBreaker)
            : base(circuitBreaker)
        {
            circuitBreaker.ResetFailureCount();
        }

        // If an exception is raised by the protcted code, and the failure
        // threshold has been reached, move to the Open state
        public override void ActUponException(Exception e)
        {
            base.ActUponException(e);
            if (circuitBreaker.IsThresholdReached())
            {
                circuitBreaker.MoveToOpenState();
            }
        }
    }
}
