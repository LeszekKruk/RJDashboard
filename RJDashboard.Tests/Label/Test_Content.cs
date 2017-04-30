using RJController.Enums;
using RJController.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RJDashboard.Tests.Label
{
    public class Test_Content 
    {

        
        //Content c2 = new Content("txtGit", "GitHub:", ContentType.Static);
        //Content c3 = new Content("txt", "https://github.com/LeszekKruk/RJDashboard", ContentType.Variable);

        [Fact]
        public void AddingNewContent_ReturnPropertyTypeFor_Name()
        {
            ObjectContent c1 = new ObjectContent("txtImieNazwisko", "Leszek Kruk", "Variable",-1,-1);

            var expected = typeof(ObjectContent);
            var actual = c1;

            Assert.IsType(expected, actual);
        }

        [Fact]
        public void AddingNewContent_ReturnProperty_Name_IsNotNull()
        {
            ObjectContent c1 = new ObjectContent("txtImieNazwisko", "Leszek Kruk", "Variable",-1,-1);

            var result = c1.ContentName;

            Assert.NotNull(result);
        }

        [Fact]
        public void AddingNewContent_With_Invalid_Name_Throws_ArgumentException()
        {
            Exception ex = Assert.Throws<ArgumentException>(() => new ObjectContent(null, "Leszek Kruk", "Variable",-1,-1));
            Assert.Equal("Parameter can't be null", ex.Message);
        }

        [Fact]
        public void AddingNewContent_ReturnPropertyValueFor_Name()
        {
            ObjectContent c1 = new ObjectContent("txtImieNazwisko", "Leszek Kruk", "Variable",-1,-1);

            string result = c1.ContentName;

            Assert.Equal("txtImieNazwisko", result);
        }

        [Fact]
        public void AddingNewContent_ReturnPropertyValueFor_Value()
        {
            ObjectContent c1 = new ObjectContent("txtImieNazwisko", "Leszek Kruk", "Variable",-1,-1);

            string result = c1.ContentValue;

            Assert.Equal("Leszek Kruk", result);
        }
    }




}
