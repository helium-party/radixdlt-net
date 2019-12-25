using Shouldly;
using Xunit;

namespace HeliumParty.Utils.Tests
{
    public class TextTest
    {
        [Fact]
        public void ToUpperCamelCaseTest()
        {
            var expectedResult = "HelloWorld";
            Text.ChangeCase(expectedResult, Text.NamingCase.UpperCamelCase, Text.NamingCase.UpperCamelCase).ShouldBe(expectedResult);
            Text.ChangeCase("helloWorld", Text.NamingCase.lowerCamelCase, Text.NamingCase.UpperCamelCase).ShouldBe(expectedResult);
            Text.ChangeCase("HELLO_WORLD", Text.NamingCase.UPPER_CASE, Text.NamingCase.UpperCamelCase).ShouldBe(expectedResult);
            Text.ChangeCase("hello_world", Text.NamingCase.snake_case, Text.NamingCase.UpperCamelCase).ShouldBe(expectedResult);
        }

        [Fact]
        public void ToLowerCamelCaseTest()
        {
            var expectedResult = "helloWorld";
            Text.ChangeCase(expectedResult, Text.NamingCase.lowerCamelCase, Text.NamingCase.lowerCamelCase).ShouldBe(expectedResult);
            Text.ChangeCase("HelloWorld", Text.NamingCase.UpperCamelCase, Text.NamingCase.lowerCamelCase).ShouldBe(expectedResult);
            Text.ChangeCase("HELLO_WORLD", Text.NamingCase.UPPER_CASE, Text.NamingCase.lowerCamelCase).ShouldBe(expectedResult);
            Text.ChangeCase("hello_world", Text.NamingCase.snake_case, Text.NamingCase.lowerCamelCase).ShouldBe(expectedResult);
        }

        [Fact]
        public void ToUpperCaseTest()
        {
            var expectedResult = "HELLO_WORLD";
            Text.ChangeCase(expectedResult, Text.NamingCase.UPPER_CASE, Text.NamingCase.UPPER_CASE).ShouldBe(expectedResult);
            Text.ChangeCase("HelloWorld", Text.NamingCase.UpperCamelCase, Text.NamingCase.UPPER_CASE).ShouldBe(expectedResult);
            Text.ChangeCase("helloWorld", Text.NamingCase.lowerCamelCase, Text.NamingCase.UPPER_CASE).ShouldBe(expectedResult);
            Text.ChangeCase("hello_world", Text.NamingCase.snake_case, Text.NamingCase.UPPER_CASE).ShouldBe(expectedResult);
        }

        [Fact]
        public void ToSnakeCaseTest()
        {
            var expectedResult = "hello_world";
            Text.ChangeCase(expectedResult, Text.NamingCase.snake_case, Text.NamingCase.snake_case).ShouldBe(expectedResult);
            Text.ChangeCase("HelloWorld", Text.NamingCase.UpperCamelCase, Text.NamingCase.snake_case).ShouldBe(expectedResult);
            Text.ChangeCase("helloWorld", Text.NamingCase.lowerCamelCase, Text.NamingCase.snake_case).ShouldBe(expectedResult);
            Text.ChangeCase("HELLO_WORLD", Text.NamingCase.UPPER_CASE, Text.NamingCase.snake_case).ShouldBe(expectedResult);
        }
    }
}
