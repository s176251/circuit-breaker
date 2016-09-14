using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public class HalfOpenState : CircuitBreakerState
    {
        // Call the base class constructor
        public HalfOpenState(CircuitBreaker circuitBreaker) : base(circuitBreaker) { }


        // If an exception is thrown by the protected code, move to the Open state
        public override void ActUponException(Exception e)
        {
            base.ActUponException(e);
            circuitBreaker.MoveToOpenState();
        }

        // If no exception was thrown, move to the Closed state
        public override void ProtectedCodeHasBeenCalled()
        {
            base.ProtectedCodeHasBeenCalled();
            circuitBreaker.MoveToClosedState();
        }
    }
}
