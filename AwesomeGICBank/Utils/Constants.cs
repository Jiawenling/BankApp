using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwesomeGICBank.Utils
{
    
    public static class Constants
    {
        public static string MESSAGE_WELCOME = "Welcome to AwesomeGIC Bank! What would you like to do?" + Environment.NewLine + "[T] Input transactions" + Environment.NewLine + "[I] Define interest rules" + Environment.NewLine + "[P] Print statement" + Environment.NewLine + "[Q] Quit";
        public static string MESSAGE_NEXTACTION = "Is there anything else you'd like to do?" + Environment.NewLine + "[T] Input transactions" + Environment.NewLine + "[I] Define interest rules" + Environment.NewLine + "[P] Print statement" + Environment.NewLine + "[Q] Quit";
        public static string MESSAGE_TRANSACTIONS = "Please enter transaction details in <Date> <Account> <Type> <Amount> format (or enter blank to go back to main menu):";
        public static string MESSAGE_INTERESTRATES = "Please enter interest rules details in <Date> <RuleId> <Rate in %> format (or enter blank to go back to main menu):";
        public static string MESSAGE_STATEMENT = "Please enter account and month to generate the statement <Account> <Year><Month>(or enter blank to go back to main menu):";
        public static string MESSAGE_QUIT = "Thank you for banking with AwesomeGIC Bank."+Environment.NewLine +"Have a nice day!";
        public static string MESSAGE_INVALIDINPUT = "Invalid input: ";

        public static string DATEFORMAT = "yyyyMMdd";

        public static string ACTION_QUIT = "q";
        public static string ACTION_INPUTTRANSACTIONS = "t";
        public static string ACTION_INTERESTRULES = "i";
        public static string ACTION_PRINTSTATEMENT = "p";

        public static char WITHDRAW = 'W';

        
    }
}