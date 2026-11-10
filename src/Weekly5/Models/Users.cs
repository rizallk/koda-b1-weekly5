namespace Weekly5.Models;

public sealed class User
{
  public int Id { get; }
  public string Name { get; }
  public string Email { get; }
  public string? Phone { get; }

  public User(int id, string name, string email, string? phone)
  {
    if (id <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(id), "Id tidak boleh 0 atau negatif");
    }
    if (string.IsNullOrWhiteSpace(name))
    {
      throw new ArgumentException("Nama wajib diisi", nameof(name));
    }
    if (string.IsNullOrWhiteSpace(email))
    {
      throw new ArgumentException("Email wajib diisi", nameof(phone));
    }

    Id = id;
    Name = name;
    Email = email;
    Phone = phone;
  }

  public User(string name, string email, string? phone)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      throw new ArgumentException("Nama wajib diisi", nameof(name));
    }
    if (string.IsNullOrWhiteSpace(email))
    {
      throw new ArgumentException("Email wajib diisi", nameof(phone));
    }

    Name = name;
    Email = email;
    Phone = phone;
  }
}