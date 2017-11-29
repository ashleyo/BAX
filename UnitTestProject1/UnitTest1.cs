using System.Linq;
using System.Collections.Generic;
using BankAccountExample;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        Customer TestCustomer = new Customer() { Name = "Test Customer" };

        [TestMethod]
        public void BasicAccountCreation()
        {
            Account AC = new CurrentAccount(TestCustomer);
            AC.Deposit(500M);
            AC.Withdraw(34.99M);
            Assert.AreEqual(AC.Balance, 465.01M);
        }

        [TestMethod]
        public void CurrentAccountCreationOverdraft()
        {
            CurrentAccount NAC;
            NAC = new CurrentAccount(TestCustomer) { OverdraftLimit = 200M };
            NAC.Withdraw(200M);
            NAC.Deposit(149.99M);
            Assert.AreEqual(NAC.Balance, -50.01M);
        }

        [TestMethod]
        public void SavingsAccountCreation()
        {
            SavingsAccount AC = new SavingsAccount(TestCustomer);
            AC.Deposit(149.99M);
            foreach (int i in Enumerable.Range(1, 3)) AC.Withdraw(50M);
            Assert.AreEqual(AC.Balance, 49.99M);
        }

        [TestMethod]
        public void PremiumSavingsAccountCreation()
        {
            PremiumSavingsAccount PSA;
            PSA = new PremiumSavingsAccount(
                new Customer()
                {
                    Name = "G Mugabe"
                }
            ){
                MinBalForInterest = 200M,
                InterestRate = 0.04M
            };
            PSA.Deposit(1000M);
            PSA.PayInterest();
            PSA.Withdraw(900M);
            PSA.PayInterest();
            Assert.AreEqual(PSA.Balance, 140M);
        }

        [TestMethod]
        public void MultiBeneficiaryAccountCreation()
        {
            CurrentAccount NAC = new CurrentAccount(
                                    new List<Customer>() {
                                        new Customer() {Name="Tom"},
                                        new Customer() {Name="Dick"},
                                        new Customer() {Name="Harry"}});
            NAC.Deposit(9999M);
            Assert.AreEqual(NAC.Balance, 9999M);
            Assert.AreEqual(NAC.PrimaryAccountHolder.Name, "Tom");
            Assert.AreEqual(NAC.AccountHolders.Count, 3);
        }
    }
}
