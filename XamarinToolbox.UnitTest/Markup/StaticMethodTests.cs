using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using XamarinToolbox.MarkupExtensions;

namespace XamarinToolbox.UnitTest.Markup
{
    [TestClass]
    public class StaticMethodTests : TestBase
    {
        private Type _staticType;
        private string _staticMethodName;
        private StaticMethodExtension _extension;

        private int[] _parameters;
        private object _method;
        private int _result;
        private Exception _exception;

        private void AType()
        {
            _staticType = typeof(StaticMethodProvider);
        }

        private void AMethodWithoutParameters()
        {
            _staticMethodName = nameof(StaticMethodProvider.Zero);
        }

        private Action AProposedMethodResultOf(int result) => () => StaticMethodProvider.Result = result;

        private void TheMarkup()
        {
            _extension = new StaticMethodExtension()
            {
                Type = _staticType,
                Method = _staticMethodName,
            };
        }

        private void TheStaticMethodIsReflected()
        {
            _exception = null;
            try
            {
                _method = _extension.ProvideValue(null);
            }
            catch (Exception exc)
            {
                _exception = exc;
            }
        }

        private void TheMethodHasNoParameters()
        {
            _method.Should().BeOfType<Func<int>>();
        }

        private Action CallingTheMethodReturns(int number) => () =>
        {
            _method.Should().BeAssignableTo<Delegate>();
            var asDelegate = (Delegate) _method;
            _result = (int) asDelegate.DynamicInvoke(_parameters);
        };

        [TestMethod]
        public void NoParameter()
        {
            Given(AType, AMethodWithoutParameters, AProposedMethodResultOf(7), TheMarkup);
            When(TheStaticMethodIsReflected);
            Then(TheMethodHasNoParameters, CallingTheMethodReturns(7));
        }
    }

    public static class StaticMethodProvider
    {
        public static int Result;
        public static int Zero() => Result;
        public static int One(int first) => first + Result;
        public static int Two(int first, int second) => first + second + Result;
    }
}