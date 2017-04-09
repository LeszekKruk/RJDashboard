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
    public class Test_LabelObject
    {
        [Fact]
        public void CreateNewLabelObject_Name_IsNotNull()
        {
            LabelObject lo = new LabelObject("Text static_1");

            var result = lo.Name;

            Assert.NotNull(result);
        }

        [Fact]
        public void CreateNewLabelObject_Name_ReturnPropertyName()
        {
            LabelObject lo = new LabelObject("Text static_1");

            var actual = lo.Name;
            string expected = "Text static_1";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddingNewContentsToLabelObject_ReturnPropertyCount()
        {
            LabelObject lo = new LabelObject("Text static_1");
            Content c1 = new Content("txtImieNazwisko", "Leszek Kruk", ContentType.Variable);
            Content c2 = new Content("txtImieNazwisko_2", "Leszek Kruk", ContentType.Variable);
            Content c3 = new Content("txtImieNazwisko_3", "Leszek Kruk", ContentType.Variable);

            lo.AddContent(c1);
            lo.AddContent(c2);
            lo.AddContent(c3);

            var actual = lo.Contents.Count;
            int expected = 3;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateNewLabelObject_With_Null_Name_Throws_ArgumentException()
        {
            Exception ex = Assert.Throws<ArgumentException>(() => new LabelObject(null));
            Assert.Equal("Parameter can't be null", ex.Message);
        }

        [Fact]
        public void AddingNewContentToLabelObject_ReturnPropertyType()
        {
            LabelObject lo = new LabelObject("Text static_1");
            Content c = new Content("txtImieNazwisko","Leszek Kruk", ContentType.Variable);

            lo.AddContent(c);

            var expected = typeof(Content);
            var actual = lo.Contents[0];

            Assert.IsType(expected, actual);
        }


        //do poprawy test - zgłasza wyjątek ale jak go opisać
        /*
        [Fact]
        public void GettingContentForLabelObject_WithWrongIndex_Throws_ArgumentException()
        {
            LabelObject lo = new LabelObject("Text static_1");
            Content c = new Content("txtImieNazwisko", "Leszek Kruk", ContentType.Variable);

            //lo.AddContent(c);
            Exception ex = Assert.Throws<ArgumentOutOfRangeException>(() => lo.Contents[1]);

            Assert.Equal("Index List<Content> is out of range", ex.Message);
        }
        */
    }
}
