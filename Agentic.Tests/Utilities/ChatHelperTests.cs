using Agentic.Tools;
using Agentic.Utilities;

namespace Agentic.Tests.Utilities
{
    public class ChatHelperTests
    {
        [TestFixture]
        public class GetToolJsonExampleTests
        {
            [SetUp]
            public void Setup()
            {
            }

            [Test]
            public void SerializePwshToolCorrectly()
            {
                var pwshTool = new PwshTool();
                var toolJson = ChatHelpers.GetToolJsonExample(pwshTool);
                Assert.That(toolJson, Is.EqualTo("{\"Tool\":\"pwsh\",\"Script\":\"<Script>\"} (Executes a PowerShell script, file management, systems management, access external resources and anything else)"));
            }

            [Test]
            public void SerializeInlineToolCorrectly()
            {
                var tool = new InlineTool("NewTool", "Description!", () => "");

                var toolJson = ChatHelpers.GetToolJsonExample(tool);
                Assert.That(toolJson, Is.EqualTo("{\"Tool\":\"NewTool\"} (Description!)"));
            }

            [Test]
            public void SerializeInlineToolWithNullDescription()
            {
                var tool = new InlineTool("NewTool", "", () => "");

                var toolJson = ChatHelpers.GetToolJsonExample(tool);
                Assert.That(toolJson, Is.EqualTo("{\"Tool\":\"NewTool\"}"));
            }

            [Test]
            public void SerializeInlineToolWithEmptyDescription()
            {
                var tool = new InlineTool("NewTool", "", () => "");

                var toolJson = ChatHelpers.GetToolJsonExample(tool);
                Assert.That(toolJson, Is.EqualTo("{\"Tool\":\"NewTool\"}"));
            }

            [Test]
            public void SerializeInlineToolWithSimpleParameterCorrectly()
            {
                var tool = new InlineTool<string>("NewTool", "Description!", parameter => "");

                var toolJson = ChatHelpers.GetToolJsonExample(tool);
                Assert.That(toolJson, Is.EqualTo("{\"Tool\":\"NewTool\",\"Parameter\":\"<Parameter>\"} (Description!)"));
            }

            [Test]
            public void SerializeInlineToolWithMultipleParametersCorrectly()
            {
                var tool = new InlineTool<string, int>("NewTool", "Description!", (p1, p2) => "");

                var toolJson = ChatHelpers.GetToolJsonExample(tool);
                Assert.That(toolJson, Is.EqualTo("{\"Tool\":\"NewTool\",\"Parameter1\":\"<Parameter1>\",\"Parameter2\":0} (Description!)"));
            }

            [Test]
            public void SerializeInlineToolWithComplexParameterCorrectly()
            {
                var tool = new InlineTool<ComplexTestParameter>("NewTool", "Description!", parameter => "");

                var toolJson = ChatHelpers.GetToolJsonExample(tool);
                Assert.That(toolJson, Is.EqualTo("{\"Tool\":\"NewTool\",\"Parameter\":{\"Parameter1\":\"\",\"Parameter2\":0}} (Description!)"));
            }
        }

        [TestFixture]
        public class ParseToolsTests
        {
            [Test]
            public void ParsePwshInvocation()
            {
                var tools = new[] { new PwshTool() };
                var invocation = $"{{\"{nameof(ITool.Tool)}\":\"pwsh\",\"Script\":\"Write-Host \\\"Hello, World!\\\"\"}}";

                var tool = (PwshTool) ChatHelpers.ParseTools(tools, invocation).Single();

                Assert.That(tool.Tool, Is.EqualTo("pwsh"));
                Assert.That(tool.Script.Value, Is.EqualTo("Write-Host \"Hello, World!\""));
            }

            [Test]
            public void ParseInlineToolWithoutParameterInvocation()
            {
                var tools = new[] { new InlineTool("NewTool", "Description!", () => "") };
                var invocation = $"{{\"{nameof(ITool.Tool)}\":\"NewTool\"}}";

                var tool = (InlineTool) ChatHelpers.ParseTools(tools, invocation).Single();

                Assert.That(tool.Tool, Is.EqualTo("NewTool"));
            }

            [Test]
            public void ParseInlineToolWithStringInvocation()
            {
                var tools = new[] { new InlineTool<string>("NewTool", "Description!", i => "") };
                var invocation = $"{{\"{nameof(ITool.Tool)}\":\"NewTool\",\"Parameter\":\"abc\"}}";

                var tool = (InlineTool<string>) ChatHelpers.ParseTools(tools, invocation).Single();

                Assert.That(tool.Parameter.Value, Is.EqualTo("abc"));
            }

            [Test]
            public void ParseInlineToolWithDefaultStringParameterInvocation()
            {
                var tools = new[] { new InlineTool<string>("NewTool", "Description!", i => "") };
                var invocation1 = $"{{\"{nameof(ITool.Tool)}\":\"NewTool\",\"Parameter\":\"abc\"}}";
                var invocation2 = $"{{\"{nameof(ITool.Tool)}\":\"NewTool\"}}";

                // Call with parameter initially
                var tool = (InlineTool<string>)ChatHelpers.ParseTools(tools, invocation1).Single();

                // Call without parameter to verify that it's reset
                tool = (InlineTool<string>) ChatHelpers.ParseTools(tools, invocation2).Single();

                Assert.That(tool.Parameter.Value, Is.Null);
            }

            [Test]
            public void ParseInlineToolWithDefaultIntParameterInvocation()
            {
                var tools = new[] { new InlineTool<int>("NewTool", "Description!", i => "") };
                var invocation1 = $"{{\"{nameof(ITool.Tool)}\":\"NewTool\",\"Parameter\":5}}";
                var invocation2 = $"{{\"{nameof(ITool.Tool)}\":\"NewTool\"}}";

                // Call with parameter initially
                var tool = (InlineTool<int>)ChatHelpers.ParseTools(tools, invocation1).Single();

                // Call without parameter to verify that it's reset
                tool = (InlineTool<int>)ChatHelpers.ParseTools(tools, invocation2).Single();

                Assert.That(tool.Parameter.Value, Is.EqualTo(0));
            }

            [Test]
            public void ParseInlineToolWithIntegerInvocation()
            {
                var tools = new[] { new InlineTool<int>("NewTool", "Description!", i => "") };
                var invocation = $"{{\"{nameof(ITool.Tool)}\":\"NewTool\",\"Parameter\":5}}";

                var tool = (InlineTool<int>) ChatHelpers.ParseTools(tools, invocation).Single();

                Assert.That(tool.Parameter.Value, Is.EqualTo(5));
            }

            [Test]
            public void ParseInlineToolWithMultipleParamaterInvocation()
            {
                var tools = new[] { new InlineTool<string, int>("NewTool", "Description!", (p1, p2) => "") };
                var invocation = $"{{\"{nameof(ITool.Tool)}\":\"NewTool\",\"Parameter1\":\"abc\",\"Parameter2\":5}}";

                var tool = (InlineTool<string, int>) ChatHelpers.ParseTools(tools, invocation).Single();

                Assert.That(tool.Parameter1.Value, Is.EqualTo("abc"));
                Assert.That(tool.Parameter2.Value, Is.EqualTo(5));
            }

            [Test]
            public void ParseInlineToolWithComplexInvocation()
            {
                var tools = new[] { new InlineTool<ComplexTestParameter>("NewTool", "Description!", i => "") };
                var invocation = $"{{\"{nameof(ITool.Tool)}\":\"NewTool\",\"Parameter\":{{\"Parameter1\":\"abc\",\"Parameter2\":5}}}}";
            
                var tool = (InlineTool<ComplexTestParameter>) ChatHelpers.ParseTools(tools, invocation).Single();
            
                Assert.That(tool.Parameter.Value.Parameter1, Is.EqualTo("abc"));
                Assert.That(tool.Parameter.Value.Parameter2, Is.EqualTo(5));
            }
        }

        [TestFixture]
        public class GetToolJsonTests
        {
            [Test]
            public void GetJsonForPwshTool()
            {
                var pwshTool = new PwshTool();
                var toolJson = ChatHelpers.GetToolJson(pwshTool);
                Assert.That(toolJson, Is.EqualTo("{\"Tool\":\"pwsh\",\"Script\":\"\"}"));
            }

            [Test]
            public void GetJsonForPwshToolWithScript()
            {
                var pwshTool = new PwshTool();
                pwshTool.Script = new ToolParameter<string>("Write-Host \"Hello, World!\"");
                var toolJson = ChatHelpers.GetToolJson(pwshTool);
                Assert.That(toolJson, Is.EqualTo(@"{""Tool"":""pwsh"",""Script"":""Write-Host \""Hello, World!\""""}"));
            }

            [Test]
            public void GetJsonForInlineToolWithMultipleParamaters()
            {
                var tool = new InlineTool<string, int>("NewTool", "Description!", (p1, p2) => "");
                tool.Parameter1 = new ToolParameter<string>("abc");
                tool.Parameter2 = new ToolParameter<int>(5);

                var toolJson = ChatHelpers.GetToolJson(tool);
                Assert.That(toolJson, Is.EqualTo("{\"Tool\":\"NewTool\",\"Parameter1\":\"abc\",\"Parameter2\":5}"));
            }
        }

        public class ComplexTestParameter
        {
            public required string Parameter1 { get; set; }
            public int Parameter2 { get; set; }
        }
    }
}