using System;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CommandAPI.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.AspNetCore.Hosting;
using CommandAPI.Controllers;

namespace CommandAPI.Tests
{
    public class CommandsControllerTests
    {
        DbContextOptionsBuilder<CommandContext> optionsBuilder;
        CommandContext dbContext;
        Mock<IHostingEnvironment> mockEnvironment;
        CommandsController Controller;
        public CommandsControllerTests()
        {
            optionsBuilder = new DbContextOptionsBuilder<CommandContext>();
            optionsBuilder.UseInMemoryDatabase("UnitTestInMemDB");
            dbContext = new CommandContext(optionsBuilder.Options);
            mockEnvironment = new Mock<IHostingEnvironment>();
            mockEnvironment.Setup(m => m.EnvironmentName).Returns("UnitTests");
            Controller = new CommandsController(dbContext, mockEnvironment.Object);

        }


        public void Dispose()
        {
            optionsBuilder = null;
            foreach (var cmd in dbContext.commandItems)

            {
                dbContext.commandItems.Remove(cmd);

                dbContext.SaveChanges(); dbContext.Dispose();
                mockEnvironment = null;
                Controller = null;
            }
        }



        [Fact]
        public void GetCommandItems_ReturnsZeroItems_WhenDBIsEmpty()
        {
            //Arrange         
            //DBContext            
            //Act
            var result = Controller.Get();
            //Assert
            Assert.Empty(result.Value);
        }

        [Fact]
        public void GetCommandItemsReturnsOneItemWhenDBHasOneObject()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.commandItems.Add(command);
            dbContext.SaveChanges();

            //Act
            var result = Controller.Get();
            //Assert
            Assert.Single(result.Value);
        }
        [Fact]
        public void GetCommandItemsReturnsTheCorrectType()
        {//Arrange,
         //Act
            var result = Controller.Get();
            //Assert
            Assert.IsType<ActionResult<IEnumerable<Command>>>(result);
        }
        [Fact]
        public void GetCommandItemReturnsNullResultWhenInvalidID()
        {
            //Arrange
            //Db should be empty any ID should be invalid
            //Act
            var result = Controller.Get(0);

            //Assert
            Assert.Null(result.Value);
        }
        [Fact]
        public void GetCommandItemReturns404NotFoundWhenInvalidID()
        {   //Arrange   //DB should be empty, any ID will be invalid 

            //Act
            var result = Controller.Get(0);

            //Assert   
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetCommandItemReturnsTheCorrectType()
        {
            //Arrange
            var Command = new Command
            {
                HowTo = "do something",
                Platform = "SomemPlatform",
                CommandLine = "SomeCommand"
            };

            dbContext.commandItems.Add(Command);
            dbContext.SaveChanges();
            var cmdId = Command.Id;

            //Act
            var result = Controller.Get(cmdId);
            //Assert
            Assert.IsType<ActionResult<Command>>(result);
        }

        [Fact]
        public void GetCommandItemReturnsTheCorrectResouce()
        {
            //Arrange
            var Command = new Command
            {
                HowTo = "do something",
                Platform = "SomemPlatform",
                CommandLine = "SomeCommand"
            };

            dbContext.commandItems.Add(Command);
            dbContext.SaveChanges();
            var cmdId = Command.Id;

            //Act
            var result = Controller.Get(cmdId);

            //Assert
            Assert.Equal(cmdId, result.Value.Id);
        }
        [Fact]
        public void PostCommandItemObjectCountIncrementWhenValidObject()
        {   //Arrange  
            var command = new Command
            { HowTo = "Do Somethting", Platform = "Some Platform", CommandLine = "Some Command" };
            var oldCount = dbContext.commandItems.Count();

            //Act  
            var result = Controller.PostCommandItem(command);

            //Assert   
            Assert.Equal(oldCount + 1, dbContext.commandItems.Count());
        }
        [Fact]
        public void PostCommandItemReturns201CreatedWhenValidObject()
        {   //Arrange  
            var command = new Command { HowTo = "Do Somethting", Platform = "Some Platform", CommandLine = "Some Command" };

            //Act 
            var result = Controller.PostCommandItem(command);
            //Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);

        }

        [Fact]
        public void PutCommandItem_AttributeUpdated_WhenValidObject()
        {
            //Arrange  
            var command = new Command { HowTo = "Do Somethting", Platform = "Some Platform", CommandLine = "Some Command" };
            dbContext.commandItems.Add(command); dbContext.SaveChanges();

            var cmdId = command.Id;

            command.HowTo = "UPDATED";

            //Act   
            Controller.PutCommandItem(cmdId, command);
            var result = dbContext.commandItems.Find(cmdId);

            //Assert   
            Assert.Equal(command.HowTo, result.HowTo);
        }

        [Fact]
        public void PutCommandItem_Returns204_WhenValidObject()
        {
            //Arrange   
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.commandItems.Add(command);
            dbContext.SaveChanges();

            var cmdId = command.Id;

            command.HowTo = "UPDATED";

            //Act   
            var result = Controller.PutCommandItem(cmdId, command);

            //Assert  
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void PutCommandItem_AttributeUnchanged_WhenInvalidObject()
        {
            //Arrange   
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };

            dbContext.commandItems.Add(command);
            dbContext.SaveChanges();

            var command2 = new Command
            { Id = command.Id, HowTo = "UPDATED", Platform = "UPDATED", CommandLine = "UPDATED" };

            //Act   
            Controller.PutCommandItem(command.Id + 1, command2);
            var result = dbContext.commandItems.Find(command.Id);
            //Assert 
            Assert.Equal(command.HowTo, result.HowTo);//silly makes me confused
        }

        [Fact]
        public void DeleteCommandItem_ObjectsDecrement_WhenValidObjectID()
        {
            //Arrange  
            var command = new Command { HowTo = "Do Somethting", Platform = "Some Platform", CommandLine = "Some Command" };
            dbContext.commandItems.Add(command);
            dbContext.SaveChanges();

            var cmdId = command.Id;
            var objCount = dbContext.commandItems.Count();

            //Act   
            Controller.DeleteCommandItem(cmdId);

            //Assert  
            Assert.Equal(objCount - 1, dbContext.commandItems.Count());
        }

        [Fact]
        public void DeleteCommandItem_Returns200OK_WhenValidObjectID()
        {
            //Arrange  
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.commandItems.Add(command);
            dbContext.SaveChanges();

            var cmdId = command.Id;

            //Act  
            var result = Controller.DeleteCommandItem(cmdId);

            //Assert  
            Assert.Null(result.Result);
        }

        [Fact]
        public void DeleteCommandItem_Returns404NotFound_WhenValidObjectID()
        {   //Arrange              
            //Act  
            var result = Controller.DeleteCommandItem(1);
            //Assert  
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }


        [Fact]
        public void DeleteCommandItem_ObjectCountNotDecremented_WhenValidObjectID()
        {
            //Arrange 
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.commandItems.Add(command);
            dbContext.SaveChanges();

            var cmdId = command.Id;
            var objCount = dbContext.commandItems.Count();

            //Act   
            var result = Controller.DeleteCommandItem(cmdId+1);  

            //Assert 
            Assert.Equal(objCount,dbContext.commandItems.Count());
        }
    }
}
