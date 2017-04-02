using RJController.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RJDashboard.Tests.Label
{
    public class Test_ContentClass 
    {

        
        //Content c2 = new Content("txtGit", "GitHub:", ContentType.Static);
        //Content c3 = new Content("txt", "https://github.com/LeszekKruk/RJDashboard", ContentType.Variable);

        [Fact]
        public void AddingNewContent_ReturnPropertyTypeFor_Name()
        {
            Content c1 = new Content("txtImieNazwisko", "Leszek Kruk", ContentType.Variable);

            var expected = typeof(Content);
            var actual = c1;

            Assert.IsType(expected, actual);
        }

        [Fact]
        public void AddingNewContent_ReturnProperty_Name_IsNotNull()
        {
            Content c1 = new Content("txtImieNazwisko", "Leszek Kruk", ContentType.Variable);

            var result = c1.Name;

            Assert.NotNull(result);
        }

        [Fact]
        public void AddingNewContent_With_Invalid_Name_Throws_ArgumentException()
        {
            Exception ex = Assert.Throws<ArgumentException>(() => new Content(null, "Leszek Kruk", ContentType.Variable));
            Assert.Equal("Parameter can't be null", ex.Message);
        }

        [Fact]
        public void AddingNewContent_ReturnPropertyValueFor_Name()
        {
            Content c1 = new Content("txtImieNazwisko", "Leszek Kruk", ContentType.Variable);

            string result = c1.Name;

            Assert.Equal("txtImieNazwisko", result);
        }

        [Fact]
        public void AddingNewContent_ReturnPropertyValueFor_Value()
        {
            Content c1 = new Content("txtImieNazwisko", "Leszek Kruk", ContentType.Variable);

            string result = c1.Value;

            Assert.Equal("Leszek Kruk", result);
        }

        [Fact]
        public void AddingNewContent_ReturnPropertyValueFor_ContentType()
        {
            Content c1 = new Content("txtImieNazwisko", "Leszek Kruk", ContentType.Variable);

            ContentType result = c1.ContentType;

            Assert.Equal(ContentType.Variable, result);
        }

        [Fact]
        public void AddingNewContent_ReturnWrongValueFor_ContentType()
        {
            Content c1 = new Content("txtImieNazwisko", "Leszek Kruk", ContentType.Variable);

            ContentType result = c1.ContentType;

            Assert.NotEqual(ContentType.Static, result);
        }

        [Fact]
        public void AddingNewContent_ReturnTrueValueFor_NotImplementedType()
        {
            Content c1 = new Content("txtImieNazwisko", "Leszek Kruk");

            ContentType result = c1.ContentType;

            Assert.Equal(ContentType.NotImplemented, result);
        }
    }




}
