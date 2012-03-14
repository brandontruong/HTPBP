using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Security;
using AutoMapper;
using BP.Controllers;
using BP.Domain.Concrete;
using BP.Domain.Entities;
using BP.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using BP.Domain.Abstract;
using BP.ViewModels;
using System.Web.Mvc;
using Moq;

namespace BP.Tests
{
    
    
    /// <summary>
    ///This is a test class for AccountControllerTest and is intended
    ///to contain all AccountControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AccountControllerTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Register
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //public void RegisterTest()
        //{
        //    Mapper.CreateMap<RegisterViewModel, User>();
           

        //    var mockAccount = new Mock<IAccountRepository>();
           
        //    var mock = new Mock<IUnitOfWork>();
        //    mock.Setup(m => m.Accounts).Returns(mockAccount.Object);

        
        //    var target = new AccountController(mock.Object); 
        //    var model = new RegisterViewModel(); // TODO: Initialize to an appropriate value
         
        //    ActionResult actual = target.Register(model);
        //    Assert.IsInstanceOfType(actual, typeof(RedirectToRouteResult));
        //}

        [TestMethod()]
        public void LoginTest()
        {
            var mockAccount = new Mock<IAccountRepository>();

            mockAccount.Setup(m => m.IsValidLogin("admin", "11")).Returns(false);
          
            var mock = new Mock<IUnitOfWork>();
            mock.Setup(m => m.Accounts).Returns(mockAccount.Object);


            var target = new AccountController(mock.Object);
            var actual = target.Login(new LoginModel() { UserName = "admin", Password = "1234qwer" }, string.Empty);
            Assert.IsInstanceOfType(actual, typeof(ActionResult));
        }
    }
}
