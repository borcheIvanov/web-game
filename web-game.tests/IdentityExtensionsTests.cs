using System;
using System.Security.Claims;
using System.Security.Principal;
using NUnit.Framework;
using web_game.Identity;
using FluentAssertions;

namespace web_game.tests
{
    
    public class IdentityExtensionsTests
    {
        private GenericIdentity _genericIdentity;

        [SetUp]
        public void SetUp(){
            _genericIdentity = new GenericIdentity("borche");
        }

        [Test]
        public void TestGetUserName(){
            _genericIdentity.AddClaim(new Claim(IdentityConstants.NameType, "Borche Ivanov"));

            var name = _genericIdentity.GetName();

            name.Should().Be("Borche Ivanov");
        }

        [Test]
        public void TestGetEmail(){
            _genericIdentity.AddClaim(new Claim(IdentityConstants.EmailType, "borche@ivanov.com"));

            var email = _genericIdentity.GetEmail();

            email.Should().Be("borche@ivanov.com");
        }

        [Test]
        public void GetEmailShouldThrowIfNonExistent(){
            
            try{
                var email = _genericIdentity.GetEmail();
                Assert.Fail();
            }
            catch(Exception e)
            {
                e.Message.Should().Be("email not provided");    
            }
        }
    }
}