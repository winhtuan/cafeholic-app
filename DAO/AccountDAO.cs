using CAFEHOLIC.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CAFEHOLIC.DAO;

public class AccountDAO
{
    private readonly CafeholicContext _context;
    private readonly ILogger<AccountDAO> _logger;

    public AccountDAO(ILogger<AccountDAO> logger)
    {
        _context = new CafeholicContext();
        _logger = logger;
    }

    public string GenerateOTP(string phoneNumber)
    {
        var random = new Random();
        return string.Concat(Enumerable.Range(0, 6).Select(_ => random.Next(10).ToString()));
    }

    public Account? GetAccountByPhone(string phoneNumber)
    {
        try
        {
            return _context.Accounts.FirstOrDefault(a => a.PhoneNumber == phoneNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding account by phone.");
            return null;
        }
    }

    public Account? CheckLogin(string phone, string password)
    {
        try
        {
            return _context.Accounts.Include(a => a.Role).FirstOrDefault(a => a.PhoneNumber == phone && a.PasswordHash == password && a.IsVerified == true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking login.");
            return null;
        }
    }

    public Account? GetAccountById(int accId)
    {
        try
        {
            return _context.Accounts.Find(accId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account information.");
            return null;
        }
    }

    public Account? CreateAccount(string phone, string password, int userId)
    {
        try
        {
            var account = new Account
            {
                PhoneNumber = phone,
                PasswordHash = password,
                RegistDate = DateTime.Now,
                IsVerified = true,
                UserId = userId,
                RoleId = 1
            };
            _context.Accounts.Add(account);
            _context.SaveChanges();
            return account;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating new account.");
            return null;
        }
    }

    public bool UpdatePasswordByPhone(string phoneNumber, string newPassword)
    {
        try
        {
            var acc = _context.Accounts.FirstOrDefault(a => a.PhoneNumber == phoneNumber);
            if (acc != null)
            {
                acc.PasswordHash = newPassword;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating password.");
            return false;
        }
    }

    public bool IsPhoneNumberExists(string phoneNumber)
    {
        try
        {
            return _context.Accounts.Any(a => a.PhoneNumber == phoneNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking phone number existence.");
            return false;
        }
    }
}