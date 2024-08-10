using System;
using TechTalk.SpecFlow;

namespace CheckoutPricing.Api.Tests.StepDefinitions
{
    [Binding]
    public class CheckoutStepDefinitions
    {
        [Given(@"the following pricing rules:")]
        public void GivenTheFollowingPricingRules(Table table)
        {
            throw new PendingStepException();
        }

        [When(@"I scan the following items:")]
        public void WhenIScanTheFollowingItems(Table table)
        {
            throw new PendingStepException();
        }

        [Then(@"the total price should be (.*)")]
        public void ThenTheTotalPriceShouldBe(int p0)
        {
            throw new PendingStepException();
        }
    }
}
