# 🎵 音樂資料庫管理系統 (Music Database Management System)

一個基於 ASP.NET Core MVC 8.0 開發的音樂收藏管理系統,提供完整的 CRUD 功能與優雅的使用者介面。

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![SQL Server](https://img.shields.io/badge/database-SQL%20Server-red.svg)

## 📋 目錄

- [功能特色](#功能特色)
- [系統需求](#系統需求)
- [使用說明](#使用說明)
- [專案結構](#專案結構)
- [技術架構](#技術架構)
- [資料庫結構](#資料庫結構)
- [螢幕截圖](#螢幕截圖)
- [安全性考量](#安全性考量)
- [已知問題與限制](#已知問題與限制)
- [未來計畫](#未來計畫)

## ✨ 功能特色

### 核心功能
- 🎼 **歌曲管理** - 新增、編輯、刪除、檢視歌曲資訊
- 🔍 **智慧搜尋** - 依歌曲名稱快速搜尋
- 📄 **分頁顯示** - 每頁顯示 10 筆資料,提升瀏覽效率
- 🔐 **使用者認證** - Session 登入機制保護資料安全
- 🎨 **響應式設計** - 支援桌面、平板、手機等多種裝置

### 歌曲資訊管理
- 歌曲名稱 (Title)
- 演唱者/藝人 (Artist)
- 專輯名稱 (Album)
- 音樂類型 (Genre)
- 播放時長 (Duration)
- 發行年份 (Release Year)
- 專輯封面圖片 (Image URL)
- 加入日期 (Created Date)

### 使用者介面特色
- 🎭 **玻璃擬態設計** - 現代化的半透明介面
- 🌈 **漸層色彩** - 紫色系主題配色
- 🎶 **動態音符動畫** - 增添音樂氛圍
- ⚡ **流暢互動** - 即時預覽與驗證回饋
- 📱 **完全響應式** - 完美適配各種螢幕尺寸

## 💻 系統需求

- **.NET SDK 8.0** 或更高版本
- **Visual Studio 2022** (建議使用 17.13 或更新版本)
- **SQL Server 2019** 或更高版本
- **Windows 10/11** 或 **macOS** 或 **Linux**

## 📖 使用說明

### 首次使用

1. **登入系統**
   - 前往 `/DBSongs/Login`
   - 輸入使用者名稱和密碼
   - 成功登入後將導向音樂庫首頁

2. **新增歌曲**
   - 點擊「新增歌曲」按鈕
   - 填寫歌曲資訊
   - 可選擇性上傳專輯封面圖片
   - 點擊「儲存」完成新增

3. **管理歌曲**
   - 在音樂庫列表中瀏覽所有歌曲
   - 使用「編輯」功能修改歌曲資訊
   - 使用「詳情」查看完整資訊
   - 使用「刪除」移除不需要的歌曲

4. **搜尋歌曲**
   - 點擊「搜尋」按鈕
   - 從下拉選單選擇歌曲名稱
   - 查看符合條件的搜尋結果

## 📁 專案結構
```
s1121735_Final_Project/
├── Controllers/
│   ├── DBSongsController.cs      # 歌曲管理控制器
│   └── HomeController.cs         # 首頁控制器
├── Data/
│   └── CmsContext.cs             # 資料庫上下文
├── Migrations/                    # EF Core 遷移檔案
├── Models/
│   ├── Songs.cs                  # 歌曲模型
│   ├── User.cs                   # 使用者模型
│   └── ErrorViewModel.cs         # 錯誤視圖模型
├── Views/
│   ├── DBSongs/
│   │   ├── Index.cshtml          # 音樂庫首頁
│   │   ├── Create.cshtml         # 新增歌曲
│   │   ├── Edit.cshtml           # 編輯歌曲
│   │   ├── Details.cshtml        # 歌曲詳情
│   │   ├── Delete.cshtml         # 刪除確認
│   │   ├── Login.cshtml          # 登入頁面
│   │   ├── SelectQuery.cshtml    # 搜尋頁面
│   │   └── SelectName.cshtml     # 搜尋結果
│   ├── Shared/
│   │   ├── _Layout.cshtml        # 版面配置
│   │   └── Error.cshtml          # 錯誤頁面
│   ├── _ViewImports.cshtml
│   └── _ViewStart.cshtml
├── wwwroot/
│   ├── css/                      # 樣式檔案
│   ├── js/                       # JavaScript 檔案
│   └── images/                   # 圖片資源
├── appsettings.json              # 應用程式設定
├── Program.cs                    # 程式進入點
└── s1121735_Final_Project.csproj # 專案檔
```

## 🔧 技術架構

### 後端技術
- **ASP.NET Core MVC 8.0** - 網頁應用程式框架
- **Entity Framework Core 8.0** - ORM 框架
- **SQL Server** - 關聯式資料庫
- **X.PagedList** - 分頁功能套件

### 前端技術
- **Bootstrap 5.3** - 響應式 UI 框架
- **Font Awesome 6.4** - 圖示庫
- **Vanilla JavaScript** - 互動功能
- **CSS3 動畫** - 視覺效果

### 設計模式
- **MVC 架構** - 關注點分離
- **Repository Pattern** - 資料存取抽象化
- **Dependency Injection** - 相依性注入

## 🗄️ 資料庫結構

### TableMusicDB1121735 (歌曲資料表)

| 欄位名稱 | 資料型別 | 說明 | 限制 |
|---------|---------|------|-----|
| SongID | int | 主鍵 | 自動遞增 |
| Title | nvarchar(100) | 歌曲名稱 | NOT NULL |
| Artist | nvarchar(100) | 演唱者 | NOT NULL |
| Album | nvarchar(100) | 專輯名稱 | NOT NULL |
| Genre | nvarchar(50) | 音樂類型 | NOT NULL |
| Duration | int | 時長(秒) | NULL |
| ReleaseYear | int | 發行年份 | NULL |
| ImageUrl | nvarchar(255) | 封面圖片 | NOT NULL |
| CreatedDate | datetime2 | 建立日期 | NOT NULL |

### TableUserDB1121735 (使用者資料表)

| 欄位名稱 | 資料型別 | 說明 | 限制 |
|---------|---------|------|-----|
| UserID | int | 主鍵 | 自動遞增 |
| Username | nvarchar(max) | 使用者名稱 | NOT NULL |
| Password | nvarchar(max) | 密碼 | NOT NULL |

## 📸 螢幕截圖

### 登入頁面
- 現代化的玻璃擬態設計
- 動態音符背景動畫
- 友善的錯誤提示

### 音樂庫首頁
- 統計資訊卡片
- 響應式表格設計
- 分頁導航功能

### 新增/編輯頁面
- 即時預覽功能
- 表單驗證回饋
- 直覺的操作介面

### 搜尋功能
- 下拉選單選擇
- 搜尋結果列表
- 完整的歌曲資訊展示


---

⭐ 如果這個專案對你有幫助,歡迎給個星星!

📧 如有問題或建議,歡迎提出 Issue 或 Pull Request。
