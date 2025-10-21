# 安全性問題修復指南 (Security Fixes Guide)

## 🚨 發現的安全問題

### 1. **資料庫連線字串暴露** (CRITICAL)
- **檔案**: `appsettings.json`
- **問題**: 資料庫帳號密碼以明文儲存在版本控制中
- **風險**: 任何有權限存取 repository 的人都能看到資料庫憑證

### 2. **使用者密碼明文儲存** (CRITICAL)
- **檔案**: `Controllers/DBSongsController.cs`, `Models/User.cs`
- **問題**: 密碼沒有雜湊處理，直接以明文儲存和比對
- **風險**: 資料庫洩漏時所有使用者密碼會被曝光

---

## ✅ 已完成的修復

### 1. 建立 .gitignore 檔案
已建立 `.gitignore` 來防止敏感檔案被提交到版本控制：
- 排除 `appsettings.json`
- 排除 `bin/` 和 `obj/` 目錄
- 排除 `.env` 和其他敏感檔案

### 2. 建立配置檔案範本
已建立 `appsettings.example.json` 作為範本檔案。

---

## 🔧 需要手動完成的修復

### 修復 1: 保護資料庫連線字串

#### 方法 A: 使用 User Secrets (推薦用於開發環境)

1. 在專案目錄下執行:
```bash
cd s1121735_Final_Project
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:CmsContext" "YOUR_CONNECTION_STRING"
```

2. 更新 `appsettings.json`，移除連線字串:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

#### 方法 B: 使用環境變數 (推薦用於正式環境)

1. 設定環境變數:
```bash
export ConnectionStrings__CmsContext="YOUR_CONNECTION_STRING"
```

2. 或在 Azure App Service 的設定中加入:
- Key: `ConnectionStrings:CmsContext`
- Value: 你的連線字串

#### 方法 C: 使用 Azure Key Vault (最安全)

1. 安裝 NuGet 套件:
```bash
dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
dotnet add package Azure.Identity
```

2. 在 `Program.cs` 中加入:
```csharp
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());
```

---

### 修復 2: 實作密碼雜湊

#### 步驟 1: 安裝 ASP.NET Core Identity

```bash
cd s1121735_Final_Project
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
```

#### 步驟 2: 更新 User Model

修改 `Models/User.cs`:
```csharp
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser<int>
{
    public override int Id { get; set; }
    // Username 和 Password 已由 IdentityUser 提供
    // IdentityUser 會自動處理密碼雜湊
}
```

#### 步驟 3: 更新 DbContext

修改 `Data/CmsContext.cs`:
```csharp
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class CmsContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public CmsContext(DbContextOptions<CmsContext> options)
        : base(options)
    {
    }

    public DbSet<Songs> TableMusicDB1121735 { get; set; }
    // TableUserDB1121735 已由 IdentityDbContext 處理
}
```

#### 步驟 4: 更新 Program.cs

加入 Identity 服務:
```csharp
builder.Services.AddIdentity<User, IdentityRole<int>>()
    .AddEntityFrameworkStores<CmsContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // 密碼設定
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
});
```

#### 步驟 5: 更新 Login 邏輯

修改 `Controllers/DBSongsController.cs`:
```csharp
private readonly SignInManager<User> _signInManager;
private readonly UserManager<User> _userManager;

public DBSongsController(
    CmsContext context,
    SignInManager<User> signInManager,
    UserManager<User> userManager)
{
    _context = context;
    _signInManager = signInManager;
    _userManager = userManager;
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(string username, string password)
{
    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
    {
        TempData["message"] = "Please enter account and password!";
        return RedirectToAction("Login", "DBSongs");
    }

    var user = await _userManager.FindByNameAsync(username);
    if (user != null)
    {
        var result = await _signInManager.PasswordSignInAsync(
            user,
            password,
            isPersistent: false,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            HttpContext.Session.SetString("username", username);
            TempData["message"] = "Logged in!";
            return RedirectToAction("Index");
        }
    }

    TempData["message"] = "Login failed!";
    return RedirectToAction("Login", "DBSongs");
}
```

#### 步驟 6: 資料庫遷移

由於密碼結構改變，需要:

1. 建立新的遷移:
```bash
dotnet ef migrations add UpdateUserModelWithIdentity
```

2. 更新資料庫:
```bash
dotnet ef database update
```

3. **重要**: 所有現有使用者需要重設密碼，因為明文密碼無法直接轉換成雜湊值

---

### 修復 3: 從 Git 歷史中移除敏感資料

**警告**: 資料庫憑證已經在 Git 歷史中，需要:

1. **立即更改資料庫密碼** (在 `140.138.144.66\SQL1422` 伺服器上)

2. 從 Git 歷史中移除敏感檔案:
```bash
git filter-branch --force --index-filter \
  "git rm --cached --ignore-unmatch s1121735_Final_Project/appsettings.json" \
  --prune-empty --tag-name-filter cat -- --all

git filter-branch --force --index-filter \
  "git rm -r --cached --ignore-unmatch s1121735_Final_Project/bin" \
  --prune-empty --tag-name-filter cat -- --all
```

3. 強制推送 (小心使用):
```bash
git push origin --force --all
```

**或使用 BFG Repo-Cleaner (更簡單)**:
```bash
# 安裝 BFG
brew install bfg  # macOS
# 或從 https://rtyley.github.io/bfg-repo-cleaner/ 下載

# 移除敏感檔案
bfg --delete-files appsettings.json
bfg --delete-folders bin

# 清理
git reflog expire --expire=now --all
git gc --prune=now --aggressive
git push origin --force --all
```

---

## 📋 修復檢查清單

- [x] 建立 `.gitignore` 檔案
- [x] 建立 `appsettings.example.json` 範本
- [ ] 將連線字串移到 User Secrets 或環境變數
- [ ] 更新 `appsettings.json` 移除敏感資料
- [ ] 安裝 ASP.NET Core Identity
- [ ] 更新 User Model 使用 IdentityUser
- [ ] 更新登入邏輯使用密碼雜湊
- [ ] 執行資料庫遷移
- [ ] **更改資料庫密碼 (重要!)**
- [ ] 從 Git 歷史中移除敏感資料
- [ ] 通知所有使用者重設密碼

---

## 🔒 額外的安全建議

### 1. 啟用 HTTPS
確保 `Program.cs` 中有:
```csharp
app.UseHttpsRedirection();
```

### 2. 實作帳號鎖定
在 Identity 設定中加入:
```csharp
options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
options.Lockout.MaxFailedAccessAttempts = 5;
options.Lockout.AllowedForNewUsers = true;
```

### 3. 實作 CSRF 保護
已有 `[ValidateAntiForgeryToken]`，請確保所有表單都有:
```html
<form method="post">
    @Html.AntiForgeryToken()
    <!-- form fields -->
</form>
```

### 4. SQL Injection 防護
目前使用 Entity Framework 的 LINQ 查詢已經有基本防護，但要注意:
- 不要使用字串串接來建立 SQL 查詢
- 避免使用 `FromSqlRaw` 搭配未驗證的使用者輸入

### 5. Session 安全
在 `Program.cs` 中加強 Session 設定:
```csharp
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
```

### 6. 加入 Security Headers
```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    await next();
});
```

---

## 📚 參考資源

- [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [Safe storage of app secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Azure Key Vault](https://learn.microsoft.com/en-us/azure/key-vault/)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)

---

**注意**: 這些安全問題非常嚴重，建議立即處理，特別是更改資料庫密碼和實作密碼雜湊。
