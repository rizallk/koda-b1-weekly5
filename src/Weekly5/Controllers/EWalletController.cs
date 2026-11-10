using Microsoft.AspNetCore.Mvc;
using Weekly5.DTOs;
using Weekly5.Services;
using Weekly5.ViewModels;

namespace Weekly5.Controllers;

[Route("")]
public sealed class EWalletController : Controller
{
  private readonly IUserService _userService;
  private readonly ITransactionService _transactionService;

  public EWalletController(IUserService userService, ITransactionService transactionService)
  {
    _userService = userService;
    _transactionService = transactionService;
  }

  // Home page
  [HttpGet("")]
  public async Task<IActionResult> Index(
      CancellationToken cancellationToken)
  {
    var users = await _userService.GetAllAsync(
        cancellationToken);

    return View("~/Views/Index.cshtml", users);
  }

  // Select user account
  [HttpGet("select-user/{id}")]
  public IActionResult SelectUser(int id)
  {
    HttpContext.Session.SetInt32("ActiveUserId", id);

    return RedirectToAction(nameof(Dashboard));
  }

  // Dashboard
  [HttpGet("dashboard")]
  public async Task<IActionResult> Dashboard(
      CancellationToken cancellationToken)
  {
    var activeUserId = HttpContext.Session.GetInt32("ActiveUserId");

    if (activeUserId is null)
    {
      TempData["AccessDenied"] =
          "Akses ditolak. Pilih akun terlebih dahulu.";
      return RedirectToAction(nameof(Index));
    }

    var user = await _userService.GetByIdAsync(activeUserId ?? 0,
        cancellationToken);

    if (user is null)
    {
      TempData["AccessDenied"] =
          "User tidak ditemukan.";
      return RedirectToAction(nameof(Index));
    }

    var userTransactions = await _transactionService.GetTransactionSummaryByWalletIdAsync(user.WalletId, cancellationToken);

    var viewModel = new DashboardViewModel
    {
      UserWallet = user,
      Transactions = userTransactions
    };

    return View("~/Views/Dashboard.cshtml", viewModel);
  }

  // Detail User
  [HttpGet("details/{id}")]
  public async Task<IActionResult> Details(
      int id,
      CancellationToken cancellationToken)
  {
    var user = await _userService.GetByIdAsync(
        id,
        cancellationToken);

    if (user is null)
    {
      return NotFound();
    }

    return View(user);
  }

  // Create New User (view)
  [HttpGet("create")]
  public IActionResult Create()
  {
    return View(new UserViewModel());
  }

  // Create New User (action)
  [HttpPost("create")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(
      UserViewModel model,
      CancellationToken cancellationToken)
  {
    if (!ModelState.IsValid)
    {
      return View(model);
    }

    try
    {
      var request = new UserRequest(
        model.Name,
        model.Email,
        model.Phone
      );

      var created = await _userService.CreateAsync(
          request,
          cancellationToken);

      if (created is null)
      {
        ModelState.AddModelError(
            nameof(model.Name),
            "User sudah digunakan.");

        return View(model);
      }

      TempData["Success"] =
          "User berhasil ditambahkan.";

      return RedirectToAction(
          nameof(Details),
          new { id = created.Id });
    }
    catch (ArgumentException ex)
    {
      ModelState.AddModelError(
          string.Empty,
          ex.Message);

      return View(model);
    }
  }

  // Edit User (view)
  [HttpGet("edit/{id}")]
  public async Task<IActionResult> Edit(
      int id,
      CancellationToken cancellationToken)
  {
    var user = await _userService.GetByIdAsync(
        id,
        cancellationToken);

    if (user is null)
    {
      return NotFound();
    }

    ViewData["Id"] = user.UserId;

    var model = new UserViewModel
    {
      Name = user.Name,
      Email = user.Email,
      Phone = user.Phone
    };

    return View(model);
  }

  // Edit User (action)
  [HttpPost("edit/{id}")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(
      int id,
      UserViewModel model,
      CancellationToken cancellationToken)
  {
    ViewData["Id"] = id;

    if (!ModelState.IsValid)
    {
      return View(model);
    }

    try
    {
      var request = new UserRequest
      (
        model.Name,
        model.Email,
        model.Phone
      );

      var updated = await _userService.UpdateAsync(
          id,
          request,
          cancellationToken);

      if (updated is null)
      {
        return NotFound();
      }

      TempData["Success"] =
          "User berhasil diperbarui.";

      return RedirectToAction(
          nameof(Details),
          new { id = updated.Id });
    }
    catch (ArgumentException ex)
    {
      ModelState.AddModelError(
          string.Empty,
          ex.Message);

      return View(model);
    }
  }

  // Top Up (view)
  [HttpGet("topup")]
  public async Task<IActionResult> TopUp(
  )
  {
    return View("~/Views/Wallets/TopUp.cshtml");
  }

  // Top Up (action)
  [HttpPost("topup")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> TopUp(
      int id,
      UserViewModel model,
      CancellationToken cancellationToken)
  {
    ViewData["Id"] = id;

    if (!ModelState.IsValid)
    {
      return View(model);
    }

    try
    {
      var request = new UserRequest
      (
        model.Name,
        model.Email,
        model.Phone
      );

      var updated = await _userService.UpdateAsync(
          id,
          request,
          cancellationToken);

      if (updated is null)
      {
        return NotFound();
      }

      TempData["Success"] =
          "User berhasil diperbarui.";

      return RedirectToAction(
          nameof(Details),
          new { id = updated.Id });
    }
    catch (ArgumentException ex)
    {
      ModelState.AddModelError(
          string.Empty,
          ex.Message);

      return View(model);
    }
  }

  // Transfer (view)

  // Transfer (action)

  // Transaction History

  // Exit
  [HttpGet("exit")]
  public IActionResult Exit()
  {
    HttpContext.Session.Remove("ActiveUserId");

    return RedirectToAction(nameof(Index));
  }
}