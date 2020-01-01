namespace CrossRedNC.Test
{
    using System;
    using System.Data;
    using CrossRefNC;
    using NUnit.Framework;

    public class UnitTest1
    {
        private ViewModel viewModel;

        [SetUp]
        public void SetUp()
        {
            this.viewModel = new ViewModel();
        }

        [Test]
        public void ThrowsIfNoPathIsGiven()
        {
            Assert.Throws<NoNullAllowedException>(() => this.viewModel.ReadFiles());
        }
    }
}
