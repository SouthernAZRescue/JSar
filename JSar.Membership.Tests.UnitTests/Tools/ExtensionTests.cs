﻿using Xunit;
using JSar.Web.UI;
using JSar.Web.UI.Extensions;

namespace JSar.Membership.Tests.UnitTests.Tools
{
    public class ExtensionTests
    {
        [Theory]
        [InlineData("HelloWorld")]
        [InlineData("Hello world")]
        [InlineData("Hello world.")]
        public void IsNullOrWhiteSpace_GivenPopulatedString_ReturnsFalse(string str)
        {
            Assert.False(str.IsNullOrWhiteSpace());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("     ")]
        [InlineData(" ")]
        [InlineData("\n")]
        [InlineData("")]
        public void IsNullOrWhiteSpace_GivenWhiteSpaceOrEmpty_ReturnsTrue(string str)
        {
            Assert.True(str.IsNullOrWhiteSpace());
        }

    }
}
