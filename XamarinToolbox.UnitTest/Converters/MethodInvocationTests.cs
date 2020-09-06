using System;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamarinToolbox.Converters;

namespace XamarinToolbox.UnitTest.Converters
{
    [TestClass]
    public class MethodInvocationTests : ConverterTestBase
    {
        private object _value;
        private MethodProvider _methodProvider;
        private object _parameter;
        private int _result;
        private Exception _exception;

        private void AnObject()
        {
            _value = _methodProvider = new MethodProvider();
        }

        private void AnObjectOfWrongType()
        {
            _value = new ClassWithoutMethods();
        }

        private Action ANumberAsParameter(int number) => () => _parameter = number;
        private void NoParameter() => _parameter = null;

        private Action AMethodThatReturns(int number) => () =>
        {
            _methodProvider.ReturnValue = number;
            Converter = new MethodInvocationConverter<MethodProvider, int>()
            {
                Method = nameof(MethodProvider.Return)
            };
        };
        private void AMethodThatAddsTwoToANumber()
        {
            Converter = new MethodInvocationConverter<MethodProvider, int>()
            {
                Method = nameof(MethodProvider.AddTwo),
            };
        }
        private void AnInvalidMethodName()
        {
            Converter = new MethodInvocationConverter<MethodProvider, int>()
            {
                Method = "1"
            };
        }

        private void TheMethodIsInvoked()
        {
            _exception = null;
            try
            {
                _result = (int) Converter.Convert(_value, typeof(int), _parameter, CultureInfo.InvariantCulture);
            }
            catch (Exception exc)
            {
                _exception = exc;
            }
        }

        private Action TheResultIs(int number) => () => _result.Should().Be(number);

        public void TheInvocationThrowsAnException()
        {
            _exception.Should().NotBeNull();
        }

        [TestMethod]
        public void BasicMethodInvocation()
        {
            Given(AnObject, AMethodThatReturns(8), NoParameter);
            When(TheMethodIsInvoked);
            Then(TheResultIs(8));
        }

        [TestMethod]
        public void BasicMethodInvocationWithParameter()
        {
            Given(AnObject, AMethodThatAddsTwoToANumber, ANumberAsParameter(5));
            When(TheMethodIsInvoked);
            Then(TheResultIs(7));
        }

        [TestMethod]
        public void InvalidMethodName()
        {
            Given(AnObject, AnInvalidMethodName, NoParameter);
            When(TheMethodIsInvoked);
            Then(TheInvocationThrowsAnException);
        }

        [TestMethod]
        public void WrongValue()
        {
            Given(AnObjectOfWrongType, AMethodThatReturns(8), NoParameter);
            When(TheMethodIsInvoked);
            Then(TheInvocationThrowsAnException);
        }
    }

    public class MethodProvider
    {
        public int ReturnValue { get; set; }
        public int Return() => ReturnValue;
        public int AddTwo(int x) => x + 2;
    }

    public class ClassWithoutMethods
    {

    }
}