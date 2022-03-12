using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.UnitTestProvider;

namespace NUnitRetry.SpecFlowPlugin
{
    public class TestGeneratorProvider : NUnit3TestGeneratorProvider
    {
        protected internal const string RETRY_ATTR = "NUnit.Framework.Retry";

        private readonly Configuration _configuration;

        public TestGeneratorProvider(CodeDomHelper codeDomHelper, Configuration configuration) : base(codeDomHelper)
        {
            _configuration = configuration;
        }

        // Called for scenario outlines, even when it has no tags.
        // We don't yet have access to tags against the scenario at this point, but can handle feature tags now.
        public override void SetRowTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            base.SetRowTest(generationContext, testMethod, scenarioTitle);

            // We don't want to add tag for scenario outline, as it should be added with SetTestMethod
            /*string[] featureTags = generationContext.Feature.Tags.Select(t => StripLeadingAtSign(t.Name)).ToArray();
            ApplyRetry(featureTags, Enumerable.Empty<string>(), testMethod);*/
        }

        // Called for scenarios, even when it has no tags.
        // We don't yet have access to tags against the scenario at this point, but can handle feature tags now.
        public override void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string friendlyTestName)
        {
            base.SetTestMethod(generationContext, testMethod, friendlyTestName);

            string[] featureTags = generationContext.Feature.Tags.Select(t => StripLeadingAtSign(t.Name)).ToArray();

            ApplyRetry(featureTags, Enumerable.Empty<string>(), testMethod);
        }

        // Called for both scenarios & scenario outlines, but only if it has tags
        public override void SetTestMethodCategories(TestClassGenerationContext generationContext,
            CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
        {
            // Optimisation: Prevent multiple enumerations
            scenarioCategories = scenarioCategories as string[] ?? scenarioCategories.ToArray();

            base.SetTestMethodCategories(generationContext, testMethod, scenarioCategories);

            // Feature tags will have already been processed in one of the methods above, which are executed before this
            IEnumerable<string> featureTags = generationContext.Feature.Tags.Select(t => StripLeadingAtSign(t.Name));

            ApplyRetry((string[])scenarioCategories, featureTags, testMethod);
        }

        

        private static string StripLeadingAtSign(string s) => s.StartsWith("@") ? s.Substring(1) : s;

        private static bool IsIgnoreTag(string tag) => tag.Equals(Constants.IGNORE_TAG, StringComparison.OrdinalIgnoreCase);

        private static string GetRetryTag(IEnumerable<string> tags) =>
            tags.FirstOrDefault(t =>
                t.StartsWith(Constants.RETRY_TAG, StringComparison.OrdinalIgnoreCase) &&
                // Is just "retry", or is "retry("... for params
                (t.Length == Constants.RETRY_TAG.Length || t[Constants.RETRY_TAG.Length] == '('));

        /// <summary>
        /// Apply retry tags to the current test
        /// </summary>
        /// <param name="tags">Tags that haven't yet been processed. If the test has just been created these will be for the feature, otherwise for the scenario</param>
        /// <param name="processedTags">Tags that have already been processed. If the test has just been created this will be empty, otherwise they will be the feature tags</param>
        /// <param name="testMethod">Test method we are applying retries for</param>
        private void ApplyRetry(IList<string> tags, IEnumerable<string> processedTags, CodeMemberMethod testMethod)
        {
            // Do not add retries to skipped tests (even if they have the retry attribute) as retrying won't affect the outcome.
            if (tags.Any(IsIgnoreTag) || processedTags.Any(IsIgnoreTag))
            {
                return;
            }
          
            // At feature level - if no retry tags are found - skip
            // At scenario level - if no retry tags are found - skip
            string strRetryTag = GetRetryTag(tags);
            if (strRetryTag == null)
            {
                // Apply Retry tag if ApplyGlobally in the config is set to true
                ApplyGlobalRetry(testMethod);

                return;
            }

            Parsers.RetryTag retryTag = new Parsers.RetryTagParser().Parse(strRetryTag);

            // Delete Retry attribute here; if we want to apply Retry attribute when attr from global is present, we must delete Global's one
            DeleteRetryAttribute(testMethod);

            // Apply Retry attribute which is based on Feature-tag
            var attribute = new CodeAttributeDeclaration(
                "NUnit.Framework.Retry",
                new CodeAttributeArgument(new CodePrimitiveExpression(retryTag.MaxRetries ?? _configuration.MaxRetries)));

            testMethod.CustomAttributes.Add(attribute);
        }

        /// <summary>
        /// Apply retry tag, which max retry is based on specflow.json
        /// </summary>
        private void ApplyGlobalRetry(CodeMemberMethod testMethod)
        {
            if (_configuration.ApplyGlobally)
            {
                var attribute = new CodeAttributeDeclaration(
                    "NUnit.Framework.Retry",
                    new CodeAttributeArgument(new CodePrimitiveExpression(_configuration.MaxRetries)));
                
                testMethod.CustomAttributes.Add(attribute);
            }
        }

        private void DeleteRetryAttribute(CodeMemberMethod testMethod)
        {        
            // Remove the original fact or theory attribute
            CodeAttributeDeclaration originalAttribute = testMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                .FirstOrDefault(a =>
                    a.Name == RETRY_ATTR);
            if (originalAttribute == null)
            {
                return;
            }
            testMethod.CustomAttributes.Remove(originalAttribute);
        }
    }
}
