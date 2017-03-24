using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeTask1
{
    public class User
    {
        private int _userAge;
        private double _countOfMoney;
        private string _userName;

        public int UserAge
        {
            get
            {
                return _userAge;
            }

            private set
            {
                _userAge = value;
            }
        }

        public double CountOfMoney
        {
            get
            {
                return _countOfMoney;
            }

            private set
            {
                _countOfMoney = value;
            }
        }

        public string UserName
        {
            get
            {
                return _userName;
            }

            private set
            {
                _userName = value;
            }
        }

        public User()
        {

        }

        public static User GetDefaultUser()
        {
            User practice = new User();

            practice.UserAge = 25;
            practice.UserName = "Boris Stanislavovich Filatov";
            practice.CountOfMoney = 200e+50;

            return practice;
        }

        public override string ToString()
        {
            StringBuilder temp = new StringBuilder("User " + UserName);
            temp.Append(string.Format(" has {0:E5} amount of money.", _countOfMoney));
            temp.Append(string.Format(" He is {0} years old.", _userAge));
            return temp.ToString();
        }
    }
}
