using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public abstract class CircuitBreakerState
    {
        // Type reference to the CurcuitBreaker class
        protected readonly CircuitBreaker circuitBreaker;


        // Constructor receives an instance of a CircuitBreaker class
        protected CircuitBreakerState(CircuitBreaker circuitBreaker)
        {
            this.circuitBreaker = circuitBreaker;
        }

        // Call an instance of the CircuitBreaker class
        public virtual CircuitBreaker ProtectedCodeIsAboutToBeCalled()
        {
            return this.circuitBreaker;
        }

        // Protected code has been called (it is completed, either successfully or not
        public virtual void ProtectedCodeHasBeenCalled() { }

        // When the protected code raises an exception, increment the failure count on the CircuitBreaker class
        public virtual void ActUponException(Exception e) { circuitBreaker.IncreaseFailureCount(); }

        // retrun the current state class instance
        public virtual CircuitBreakerState Update()
        {
            return this;
        }
    }
}
