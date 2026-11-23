using FluentAssertions;
using Xunit;

namespace CSG.RbVision.Tests;

public class SmokeTests
{
    [Fact]
    public void Solution_Should_Load()
    {
        true.Should().BeTrue();
    }
}
