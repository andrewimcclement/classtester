/********************************************************************************
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 *
 ********************************************************************************/

using System;
using System.Collections.Generic;

using Moq;

using NUnit.Framework;

using TheJoyOfCode.QualityTools.Tests.TestSubjects;

namespace TheJoyOfCode.QualityTools.Tests
{
    [TestFixture]
    public class PropertyTesterTest
    {
        [Test]
        public void TestProperties_BadProperty()
        {
            var tester = new PropertyTester(new DummyBadProperty());
            Assert.That(tester.TestProperties, Throws.TypeOf<PropertyTestException>());
        }

        [Test]
        public void TestProperties_BadPropertySkipped()
        {
            var tester = new PropertyTester(new DummyBadProperty());
            tester.IgnoredProperties.Add("SomeString");
            tester.TestProperties();
        }
        [Test]
        public void TestProperties_BadPropertySkippedLambda()
        {
            var subject = new DummyBadProperty();
            var tester = new PropertyTester(subject);
            tester.AddIgnoredProperty(() => subject.SomeString);
            tester.TestProperties();
        }

        [Test]
        public void TestProperties_CannotGenerateTwoDifferentValues()
        {
            var tester = new PropertyTester(new DummyUsesSameEquals());
            Assert.That(tester.TestProperties, Throws.InvalidOperationException);
        }

        [Test]
        public void TestProperties_ConstructorError()
        {
            Assert.That(() => new PropertyTester(null), Throws.ArgumentNullException);
        }

        [Test]
        public void TestProperties_GenericClass()
        {
            var tester = new PropertyTester(new DummyGeneric<int, int?, List<string>, string>());
            tester.TestProperties();
        }

        [Test]
        public void TestProperties_NoEvent()
        {
            var tester = new PropertyTester(new DummyNoEvent());
            Assert.That(tester.TestProperties, Throws.TypeOf<PropertyTestException>());
        }

        [Test]
        public void TestProperties_NoEventSkipped()
        {
            var tester = new PropertyTester(new DummyNoEvent());
            tester.IgnoredProperties.Add("SomeInt");
            tester.TestProperties();
        }

        [Test]
        public void TestProperties_NoEventSkippedLambda()
        {
            var subject = new DummyNoEvent();
            var tester = new PropertyTester(subject);
            tester.AddIgnoredProperty(() => subject.SomeInt);
            tester.TestProperties();
        }

        [Test]
        public void TestProperties_Pass()
        {
            var tester = new PropertyTester(new DummyGood());
            tester.TestProperties();
        }

        [Test]
        public void TestProperties_Self()
        {
            var tester = new PropertyTester(new PropertyTester(new object()));
            tester.TestProperties();
        }

        [Test]
        public void TestProperties_UsingMock()
        {
            var mock = new Mock<IBasicProperties>();
            mock.Setup(x => x.GetSetProperty).Returns("one");
            mock.Setup(x => x.GetOnlyProperty).Returns("anyoldthing");

            var typeFactory = new CustomStringTypeFactory();
            typeFactory.ReturnValues.Enqueue("one");
            typeFactory.ReturnValues.Enqueue("two");

            var tester = new PropertyTester(mock.Object, typeFactory);
            tester.IgnoredProperties.Add("ConstructorArguments");
            tester.TestProperties();
            mock.VerifyAll();
        }

        private class CustomStringTypeFactory : ITypeFactory
        {
            public CustomStringTypeFactory()
            {
                ReturnValues = new Queue<string>();
            }

            public Queue<string> ReturnValues { get; private set; }

            #region ITypeFactory Members

            public bool CanCreateInstance(Type type)
            {
                return type == typeof(string);
            }

            public object CreateRandomValue(Type type)
            {
                return ReturnValues.Dequeue();
            }

            #endregion
        }
    }
}