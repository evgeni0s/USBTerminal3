using System.ComponentModel;
using InWit.Core.Tests.Utils;
using InWit.Core.Utils;
using NUnit.Framework;

namespace InWit.Core.Tests
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void NameOfPropertyTest()
        {
            Assert.That(Extensions.NameOfProperty((TestContent tc) => tc.Data), Is.EqualTo("Data"));
            Assert.That(Extensions.NameOfProperty((TestContent tc) => tc.Additional), Is.EqualTo("Additional"));
        }

        [Test]
        public void FirePropertyChangedTest()
        {
            var obj = new TestContent();

            bool fire = false;
            obj.PropertyChanged += (_,__) => fire = true;

            obj.Data = 3;
            Assert.True(fire);

            fire = false;

            obj.Additional = 5;
            Assert.True(fire);
        }

        [Test]
        public void IsPropertyTest()
        {
            var obj = new TestContent();

            PropertyChangedEventArgs args = null;
            obj.PropertyChanged += (_, e) => args = e;

            obj.Data = 3;
            Assert.True(args.IsProperty((TestContent tc) => tc.Data));

            obj.Additional = 5;
            Assert.True(args.IsProperty((TestContent tc) => tc.Additional));
        }
    }
}
