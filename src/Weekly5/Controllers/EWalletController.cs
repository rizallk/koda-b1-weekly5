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
  private readonly IWalletService _walletService;

  public EWalletController(IUserService userService, ITransactionService transactionService, IWalletService walletService)
  {
    _userService = userService;
    _transactionService = transactionService;
    _walletService = walletService;
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
  public async Task<IActionResult> TopUp(CancellationToken cancellationToken
  )
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

    var viewModel = new DashboardViewModel
    {
      UserWallet = user,
      TopUp = new TopUpViewModel
      {
        UserId = user.UserId,
        WalletNumber = user.WalletNumber
      }
    };

    return View("~/Views/Wallets/TopUp.cshtml", viewModel);
  }

  // Top Up (action)
  [HttpPost("topup")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> TopUp(
      [Bind(Prefix = "TopUp")] TopUpViewModel model,
      CancellationToken cancellationToken)
  {
    var activeUserId = HttpContext.Session.GetInt32("ActiveUserId") ?? model.UserId;

    if (!ModelState.IsValid)
    {
      var user = await _userService.GetByIdAsync(activeUserId, cancellationToken);

      var viewModel = new DashboardViewModel
      {
        UserWallet = user,
        TopUp = model
      };

      return View("~/Views/Wallets/TopUp.cshtml", viewModel);
    }

    try
    {
      var request = new WalletRequest
      (
        model.UserId,
        model.WalletNumber ?? "",
        model.Balance ?? 0
      );


      var updated = await _walletService.UpdateBalanceAsync(request.UserId, request, cancellationToken);

      if (updated is null)
      {
        return NotFound();
      }

      TempData["Success"] =
          "Top Up berhasil.";

      return RedirectToAction(nameof(Dashboard));
    }
    catch (ArgumentException ex)
    {
      ModelState.AddModelError(
          string.Empty,
          ex.Message);

      var user = await _userService.GetByIdAsync(activeUserId, cancellationToken);

      var viewModel = new DashboardViewModel
      {
        UserWallet = user,
        TopUp = model
      };

      return View(viewModel);
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