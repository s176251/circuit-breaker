using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public class OpenState : CircuitBreakerState
    {
        private readonly DateTime openDateTime;

        // Set the openDateTime to the current UTC time
        public OpenState(CircuitBreaker circuitBreaker)
            : base(circuitBreaker)
        {
            openDateTime = DateTime.UtcNow;
        }

        // Call the protected code
        public override CircuitBreaker ProtectedCodeIsAboutToBeCalled()
        {
            base.ProtectedCodeIsAboutToBeCalled();
            this.Update();
            return base.circuitBreaker;
        }

        // If the timeout period has passed, move the state to half open
        public override CircuitBreakerState Update()
        {
            base.Update();
            if (DateTime.UtcNow >= openDateTime + base.circuitBreaker.Timeout)
            {
                return circuitBreaker.MoveToHalfOpenState();
            }
            return this;
        }

        public double TimeoutRemaining()
        {
            TimeSpan t = (openDateTime + base.circuitBreaker.Timeout) - DateTime.UtcNow;
            return Math.Max(t.TotalMilliseconds, 0);
        }
    }
}
