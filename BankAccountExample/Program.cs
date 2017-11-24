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
            Account AC = new CurrentAccount(C);
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

    abstract class Account
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

        //part delegated

        public virtual void Deposit(Decimal amount)
        {
            Balance += amount;
            AllAccounts.LogTransaction(this, amount, "Deposit");
        }
        public virtual void Withdraw(Decimal amount)
        {
            Balance -= amount;
            AllAccounts.LogTransaction(this, amount, "Withdrawal");
        }

        //delegated
        abstract public string AccountType { get; }
    }

    class CurrentAccount : Account
    {
        public override string AccountType => "Current";
        public Decimal OverdraftLimit { get; set; }

        public CurrentAccount(Customer C) : base(C) { OverdraftLimit = 0M; }
        public CurrentAccount(List<Customer> customers) : this(customers[0])
        {
            AccountHolders.AddRange(customers.Skip(1));
        }

        public override void Deposit(decimal amount)
        {
            base.Deposit(amount);
        }

        public override void Withdraw(decimal amount)
        {
            if (Balance - amount + OverdraftLimit >= 0M)
            {
                base.Withdraw(amount);
            }
            else
            {
                AllAccounts.LogTransaction(this, amount, "Withdrawal", "DECLINED");
            }
        }

    }

    class SavingsAccount : Account
    {
        public override string AccountType => "Savings";
        public SavingsAccount(Customer C) : base(C) { }
        public SavingsAccount(List<Customer> customers) : this(customers[0])
        {
            AccountHolders.AddRange(customers.Skip(1));
        }

        public override void Deposit(decimal amount) => base.Deposit(amount);

        public override void Withdraw(decimal amount)
        {
            if (Balance - amount >= 0)
            {
                base.Withdraw(amount);
            }
            else
            {
                AllAccounts.LogTransaction(this, amount, "Withdrawal", "DECLINED");
            }
        }

    }

    class PremiumSavingsAccount : Account
    {
        public static decimal MinBalForInterest => 200M;
        public static decimal InterestRate => 0.04M;
        public override string AccountType => "PremiumSavings";

        public PremiumSavingsAccount(Customer C) : base(C) { }
        public PremiumSavingsAccount(List<Customer> customers) : this(customers[0])
        {
            AccountHolders.AddRange(customers.Skip(1));
        }

        public override void Deposit(decimal amount) => base.Deposit(amount);

        public override void Withdraw(decimal amount)
        {
            if (Balance - amount >= 0)
            {
                base.Withdraw(amount);
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
