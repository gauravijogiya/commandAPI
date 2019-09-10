using System;
using Xunit;
using CommandAPI.Models;

namespace CommandAPI.Tests
{
    public class CommandTests : IDisposable
    {
        Command testCommand;

        public CommandTests()
        {
            testCommand = new Command

            {
                HowTo = "Do something",
                Platform = "Some platform",
                CommandLine = "Some commandline"
            };
        }

        public void Dispose()
        {
            testCommand = null;
        }


        [Fact]

        public void CanChangeHowTo()
        {
            //Arrange
            
            //Act
            testCommand.HowTo = "Execute Unit test";

            //Assert
            Assert.Equal(testCommand.HowTo, "Execute Unit test");

        }

        [Fact]
        public void whichPlatform()
        {//Arrange
            
            //Act
            testCommand.Platform = "Xunit";
            //Assert
            Assert.Equal(testCommand.Platform, "Xunit");
        }

        [Fact]

        public void CanChangeCommand()
        {//Arrange
            //Act
            testCommand.CommandLine = "dotnet Test";
            //Assert
            Assert.Equal(testCommand.CommandLine, "dotnet Test");

        }
    }
}

