using System;
using Xunit;

namespace Spritely.ControlFlow.Test
{
    public class TryTest
    {
        [Fact]
        public void Until_hides_exception_from_caller()
        {
            Try.Running(() => { throw new Exception(); })
                .Until(() => true);
        }
    }
}
