using System;
using System.Collections.Generic;
using System.Linq;

namespace BankAccountExampleInterfaces
{

    class ManageAccounts
    {
        static void Main(string[] args)
        {
            Customer C = new Customer { Name = "Test1" };
            IAccount AC = new CurrentAccount(C);
            AC.Deposit(500M);
            AC.Withdraw(34.99M);

            C = new Customer { Name = "Test2" };
            CurrentAccount NAC;
            NAC = new CurrentAccount(C);
            NAC.OverdraftLimit = 200M;
            NAC.Withdraw(200M);
            NAC.Deposit(149.99M);

            C = new Customer { Name = "Test2" };
            AC = new SavingsAccount(C);
            AC.Deposit(149.99M);
            foreach (int i in Enumerable.Range(1, 3)) AC.Withdraw(50M);

            PremiumSavingsAccount PSA;
            PSA = new PremiumSavingsAccount(new Customer() { Name = "G Mugabe" });
            PSA.Deposit(1000M);
            PSA.PayInterest();
            PSA.Withdraw(900M);
            PSA.PayInterest();

            NAC = new CurrentAccount(
                                    new List<Customer>() {
                                        new Customer() {Name="Tom"},
                                        new Customer() {Name="Dick"},
                                        new Customer() {Name="Harry"}});


            AllAccounts.ListAll();

            AllAccounts.ListOne(NAC);
            Console.ReadKey();
        }
    }

    class Customer
    {
        public string Name { get; set; }
    }

    static class AllAccounts
    {
        private static Action<string> defaultAction = Console.WriteLine;
        private static List<Account> all = new List<Account>();
        public static void Add(Account na) => all.Add(na);
        public static void ListAll()
        {
            Console.WriteLine("\nAccounts\n========\n");
            all.ForEach(a => Console.WriteLine($"{a.AccountNumber} {a.PrimaryAccountHolder.Name} {a.AccountType}: {a.Balance,0:C2}"));
        }

        public static void ListOne(Account A)
        {
            Console.WriteLine("\nAccount Details\n========\n");
            Console.Write($"A/C No {A.AccountNumber} {A.AccountType}" +
                $"\nPrimary Holder {A.PrimaryAccountHolder.Name}\n");
            Console.Write("Other Holders: ");
            foreach (Customer C in A.AccountHolders.Skip(1)) Console.Write($"{C.Name} ");
            Console.Write("\n");
            Console.Write($"{A.Balance,0:C2}\n");
        }

        public static void LogTransaction(Account A, Decimal amount, string type, string status = "Complete")
            => LogTransaction(A, amount, type, defaultAction, status);
        public static void LogTransaction(Account A, Decimal amount, string type, Action<string> todo, string status = "Complete")
        {
            todo($"{A.AccountNumber} {status} {type} {amount,0:C2}: new balance {A.Balance,0:C2}");
        }
    }

    //Interface/ABC split
    //The ABC should deal with things for which a concrete implementation works - name, account number
    //The interface should deal with required behaviours Deposit() Withdraw()
    //Balance is a midway case ....

    interface IAccount
    {
        void Deposit(decimal amount);
        void Withdraw(decimal amount);
        string GetAccountType { get; }
    }

    abstract class Account: IAccount
    {
        // Implemented
        private static int anb = 0;
        protected Account(Customer customer)
        {
            PrimaryAccountHolder = customer;
            AccountHolders.Add(PrimaryAccountHolder);
            AccountNumber = String.Format($"{++anb,0:D4}");
            Balance = 0;
            AllAccounts.Add(this);
        }

        public Customer PrimaryAccountHolder { get; set; }
        public List<Customer> AccountHolders = new List<Customer>();
        public string AccountNumber { get; set; }
        public Decimal Balance { get; set; }
        public string GetAccountType { get; }
        public void Deposit(decimal amount) => Balance += amount;
        public void Withdraw(decimal amount) => Balance -= amount;
    }

    class CurrentAccount : Account
    {
        public new string GetAccountType => "Current";
        public Decimal OverdraftLimit { get; set; }

        public CurrentAccount(Customer C) : base(C) { OverdraftLimit = 0M; }
        public CurrentAccount(List<Customer> customers) : this(customers[0])
        {
            AccountHolders.AddRange(customers.Skip(1));
        }

        public void Deposit(decimal amount)
        {
            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (Balance - amount + OverdraftLimit >= 0M)
            {
                Balance -= amount;
            }
            else
            {
                AllAccounts.LogTransaction(this, amount, "Withdrawal", "DECLINED");
            }
        }

    }

    class SavingsAccount : Account, IAccount
    {
        public string GetAccountType => "Savings";
        public SavingsAccount(Customer C) : base(C) { }
        public SavingsAccount(List<Customer> customers) : this(customers[0])
        {
            AccountHolders.AddRange(customers.Skip(1));
        }

        public void Deposit(decimal amount) => Balance += amount;

        public void Withdraw(decimal amount)
        {
            if (Balance - amount >= 0)
            {
                Balance -= amount;
            }
            else
            {
                AllAccounts.LogTransaction(this, amount, "Withdrawal", "DECLINED");
            }
        }

    }

    class PremiumSavingsAccount : Account, IAccount
    {
        public static decimal MinBalForInterest => 200M;
        public static decimal InterestRate => 0.04M;
        public string GetAccountType => "PremiumSavings";

        public PremiumSavingsAccount(Customer C) : base(C) { }
        public PremiumSavingsAccount(List<Customer> customers) : this(customers[0])
        {
            AccountHolders.AddRange(customers.Skip(1));
        }

        public void Deposit(decimal amount) => Balance += amount;

        public void Withdraw(decimal amount)
        {
            if (Balance - amount >= 0)
            {
                Balance -= amount;
            }
            else
            {
                AllAccounts.LogTransaction(this, amount, "Withdrawal", "DECLINED");
            }
        }

        public void PayInterest()
        {
            decimal interest = Balance >= MinBalForInterest ? Balance * InterestRate : 0M;
            if (interest > 0) Deposit(interest);
            AllAccounts.LogTransaction(this, interest, "Interest");
        }
    }


}
