using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankAccountExample
{
    class ManageAccounts
    {
        static void Main(string[] args)
        {
            Customer C = new Customer { Name = "Test1" };
            BaseAccount AC = new BaseAccount(C);
            AC.Deposit(500M);
            AC.Withdraw(34.99M);

            C = new Customer { Name = "Test2" };
            AC = new BaseAccount(C);
            AC.Withdraw(200M);
            AC.Deposit(149.99M);

            AllAccounts.List();
            Console.ReadKey();
        }
    }

    class Customer
    {
        public string Name { get; set; }
    }

    static class AllAccounts
    {
        private static List<BaseAccount> all = new List<BaseAccount>();
        public static void Add(BaseAccount na) => all.Add(na);
        public static void List()
        {
            Console.WriteLine("Accounts\n========\n");
            all.ForEach(a => Console.WriteLine($"{a.AccountNumber} {a.AccountHolder}: £{a.Balance}"));
        }
    }

    class BaseAccount
    {
        private static int anb = 0;
        public BaseAccount(Customer customer)
        {
            AccountHolder = customer;
            AccountNumber = String.Format($"{++anb,0:D4}");
            Balance = 0;
            AllAccounts.Add(this);
        }
        public Customer AccountHolder { get; set; }
        public string AccountNumber { get; set; }
        public Decimal Balance { get; set; }
        public void Deposit(Decimal amount) => Balance += amount;
        public void Withdraw(Decimal amount) => Balance -= amount;
    }


}
