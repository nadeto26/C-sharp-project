using System;
using System.Collections.Generic;
using System.IO;

public class BankAccount
{
    public int AccountNumber { get; set; }
    public double Balance { get; set; }

    public BankAccount(int accountNumber, double balance)
    {
        AccountNumber = accountNumber;
        Balance = balance;
    }

    public virtual void Deposit(double amount)
    {
        Balance += amount;
    }

    public virtual void Withdraw(double amount)
    {
        if (amount < Balance)
        {
            Balance -= amount;
        }
        else
        {
            Console.WriteLine("Нямате достатъчно пари");
        }

    }

    public void DisplayAccountInfo()
    {
        Console.WriteLine($"Номер на сметка: {AccountNumber}");
        Console.WriteLine($"Пари в сметката преди теглене: {Balance}");
    }
}

public class CurrentAccount : BankAccount
{
    public double OverdraftLimit { get; set; }

    public CurrentAccount(int accountNumber, double balance, double overdraftLimit)
        : base(accountNumber, balance)
    {
        OverdraftLimit = overdraftLimit;
    }

    public override void Withdraw(double amount)
    {
        if (amount < Balance + OverdraftLimit)
        {
            Balance -= amount;
        }
        else
        {
            Console.WriteLine("Нямате достатъчно пари");
        }
    }
}

public class SavingsAccount : BankAccount
{
    public double InterestRate { get; set; }

    public SavingsAccount(int accountNumber, double balance, double interestRate)
        : base(accountNumber, balance)
    {
        InterestRate = interestRate;
    }
}

public interface ITransactionable
{
    void MakeTransaction(double amount);
    void DisplayTransactionHistory();
}

public class Bank : ITransactionable
{
    private List<BankAccount> accounts = new List<BankAccount>();

    public void AddAccount(BankAccount account)
    {
        accounts.Add(account);
    }

    public void RemoveAccount(BankAccount account)
    {
        accounts.Remove(account);
    }

    public void DisplayAllAccounts()
    {
        Console.WriteLine("-------------------------------");

        foreach (var account in accounts)
        {
            account.DisplayAccountInfo();
            Console.WriteLine();
        }
    }

    public void DisplayTotalBalance()
    {
        double totalBalance = 0;
        foreach (var account in accounts)
        {
            totalBalance += account.Balance;
        }
        Console.WriteLine($"Общо пари в двете сметки преди теглене: {totalBalance}");
        Console.WriteLine("-------------------------------");

    }
    public void MakeTransaction(double amount)
    {
        foreach (var account in accounts)
        {
            account.Withdraw(amount);
        }
    }

    public void DisplayTransactionHistory()
    {
        foreach (var account in accounts)
        {
            Console.WriteLine($"Номер на сметка: {account.AccountNumber}");
            Console.WriteLine($"Пари в сметката след теглене: {account.Balance}");
            Console.WriteLine();
        }
    }

    public void SaveToFile(string filename)
    {
        using (StreamWriter writer = new StreamWriter(filename))
        {
            foreach (var account in accounts)
            {
                writer.WriteLine($"{account.AccountNumber},{account.Balance}");
            }
        }
    }

    public static Bank LoadFromFile(string filename)
    {
        Bank bank = new Bank();
        using (StreamReader reader = new StreamReader(filename))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(',');
                int accountNumber = int.Parse(parts[0]);
                double balance = double.Parse(parts[1]);
                BankAccount account = new BankAccount(accountNumber, balance);
                bank.AddAccount(account);
            }
        }
        return bank;
    }
}

public class InsufficientFundsException : Exception
{
    public InsufficientFundsException(string message) : base(message)
    {
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Въведете номер на сметка: ");
        int number = int.Parse(Console.ReadLine());
        Console.WriteLine("Въведете баланс по сметката: ");
        double balance = double.Parse(Console.ReadLine());
        Console.WriteLine("Въведете парите, които искате да изтеглите: ");
        double amount = double.Parse(Console.ReadLine());
        BankAccount account1 = new CurrentAccount(number, balance, amount);
        Console.WriteLine("Въведете номер на втора сметка: ");
        int number1 = int.Parse(Console.ReadLine());
        Console.WriteLine("Въведете баланс във втората сметка: ");
        double balance1 = double.Parse(Console.ReadLine());
        Console.WriteLine("Въведете парите, които искате да изтеглите: ");
        double amount1 = double.Parse(Console.ReadLine());
        BankAccount account2 = new SavingsAccount(number1, balance1, amount1);

        Bank bank = new Bank();
        bank.AddAccount(account1);
        bank.AddAccount(account2);

        bank.DisplayAllAccounts();

        bank.DisplayTotalBalance();

        bank.MakeTransaction(amount);
        bank.DisplayTransactionHistory();

        try
        {
            account1.Withdraw(amount);
        }
        catch (InsufficientFundsException e)
        {
            Console.WriteLine(e.Message);
        }
        bank.SaveToFile("bank_accounts.txt");

        Bank loadedBank = Bank.LoadFromFile("bank_accounts.txt");
        loadedBank.DisplayAllAccounts();
    }
}

