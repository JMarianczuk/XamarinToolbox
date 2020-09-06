using System;
using System.Collections;

namespace XamarinToolbox.UnitTest
{
    public abstract class TestBase
    {
        private void Multicall(Action[] actions)
        {
            foreach (var action in actions)
            {
                action();
            }
        }

        public void Given(params Action[] setup) => Multicall(setup);
        public void When(params Action[] conditions) => Multicall(conditions);
        public void Then(params Action[] assertments) => Multicall(assertments);
    }
}