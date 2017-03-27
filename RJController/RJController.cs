using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJController
{
    class RJController
    {
        
    }

    public class Person
    {
        private string _name;
        private int _age;
        private string _nick;
        private int _score;

        public Person(string name, int age, string nick, int score)
        {
            _name = name;
            _age = age;
            _nick = nick;
            _score = score;
        }

        public void AddScore(int score)
        {
            _score = _score + score;
        }

        public int Score
        {
            get
            {
                return _score;
            }

            set
            {
                _score = value;
            }
        }

    }
}
