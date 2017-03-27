using RJController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RJDashboard.Tests
{

    public class TestPerson
    {
        
        Person p2 = new Person("Dominika", 20, "disia", 0);

        [Fact]
        public void AddingPlusScore_ReturnsPropertyValue()
        {
            //ARRANGE - przygotowanie środowiska do testów
            Person p1 = new Person("Leszek", 25, "leszekk", 0);

            //ACT - wykonanie logiki którą testujemy
            p1.AddScore(10);
            int result = p1.Score;

            //ASSERT - weryfikacja efektów testowanej logiki
            Assert.Equal(10, result);
        }

        [Fact]
        public void AddingMinusScore_ReturnsPropertyValue()
        {
            //ARRANGE - przygotowanie środowiska do testów
            Person p1 = new Person("Leszek", 25, "leszekk", 0);

            //ACT - wykonanie logiki którą testujemy
            p1.AddScore(-5);
            int result = p1.Score;

            //ASSERT - weryfikacja efektów testowanej logiki
            Assert.Equal(-5, result);
        }

        [Theory]
        [InlineData(5,5)]
        [InlineData(-5,-5)]
        [InlineData(8,8)]
        public void AddingScore_ReturnsPropertyValue(int score, int expectedResult)
        {
            //ARRANGE - przygotowanie środowiska do testów
            Person p1 = new Person("Leszek", 25, "leszekk", 0);

            //ACT - wykonanie logiki którą testujemy
            p1.AddScore(score);
            int result = p1.Score;

            //ASSERT - weryfikacja efektów testowanej logiki
            Assert.Equal(expectedResult, result);
        }




    }

    public class TestClass
    { 
        [Fact]
        public void PassingFirstTest()
        {
            Assert.Equal(true, isEquals(2, 2));
        }

        [Fact]
        public void FailingFirstTest()
        {
            Assert.Equal(true, isEquals(5, 2));
        }

        bool isEquals(int a, int b)
        {
            if (a == b) {
                return true;
            } else {
                return false;
            }
        }

        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(6)]
        public void MyFirstTheory(int value)
        {
            Assert.True(IsOdd(value));
        }

        bool IsOdd(int value)
        {
            return value % 2 == 1;
        }

    }
}
