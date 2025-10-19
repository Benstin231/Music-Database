using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using s1121735_Final_Project.Data;
using s1121735_Final_Project.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using X.PagedList;
using X.PagedList.Extensions;

namespace s1121735_Final_Project.Controllers
{
    public class DBSongsController : Controller
    {
        private readonly CmsContext _context;
        public DBSongsController(CmsContext context)
        {
            _context = context;
        }

        //Index with pages
        public async Task<IActionResult> Index(int ? page = 1)
        {
            //Session Check
            if (HttpContext.Session.GetString("username") == null)
            {
                TempData["message"] = "Please Login!";
                return RedirectToAction("Login", "DBSongs");
            }

            //每頁幾筆
            const int pageSize = 10;

            //處理頁數
            ViewBag.usersModel = GetPagedProcess(page, pageSize);

            //填入頁面資料
            return View(await _context.TableMusicDB1121735.Skip<Songs>(pageSize * ((page ?? 1) - 1)).Take(pageSize).ToListAsync());
        }
        protected IPagedList<Songs> GetPagedProcess(int? page, int pageSize)
        {
            //過濾從clint傳送過來有問題頁數
            if (page.HasValue && page < 1)
            {
                return null;
            }

            //從資料庫取得資料
            var listUnpaged = GetStuffFromDatabase();
            IPagedList<Songs> pagelist = listUnpaged.ToPagedList(page ?? 1, pageSize);

            //過濾從client傳送過來有問題頁數，包含判斷有問題的頁數邏輯
            if (pagelist.PageNumber != 1 && page.HasValue && page > pagelist.PageCount)
            {
                return null;
            }
            return pagelist;
        }
        protected IQueryable<Songs> GetStuffFromDatabase()
        {
            return _context.TableMusicDB1121735;
        }

        //Create
        [HttpGet]
        public IActionResult Create()
        {
            //Session Check
            if (HttpContext.Session.GetString("username") == null)
            {
                TempData["message"] = "Please Login!";
                return RedirectToAction("Login", "DBSongs");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SongID,Title,Artist,Album,Genre,Duration,ReleaseYear,ImageUrl,CreatedDate")] Songs song)
        {
            //用ModelState.IsValid判斷資料是否通過驗證
            if (ModelState.IsValid)
            {
                //將entity加入DbSet
                _context.TableMusicDB1121735.Add(song);

                //將資料異動儲存到資料庫
                await _context.SaveChangesAsync();

                //導向智Index動作方法
                return RedirectToAction(nameof(Index));
            }

            return View(song);
        }

        //Details
        public async Task<IActionResult> Details(int? id)
        {
            //Session Check
            if (HttpContext.Session.GetString("username") == null)
            {
                TempData["message"] = "Please Login!";
                return RedirectToAction("Login", "DBSongs");
            }
            //檢查是否有歌曲Id
            if (id == null || _context.TableMusicDB1121735 == null)
            {
                var msgObject = new
                {
                    statuscode = StatusCodes.Status400BadRequest,
                    error = "無效的請求，必須提供Id編號",
                    url = "The url example : /Songs/Details/5"
                };

                return new BadRequestObjectResult(msgObject);
            }

            //以Id尋找歌曲資料
            var song_id = await _context.TableMusicDB1121735.FirstOrDefaultAsync(m => m.SongID == id);

            //如果沒有找到歌曲，回傳NotFound
            if (song_id == null)
            {
                return NotFound();
            }

            return View(song_id);
        }

        //Edit
        public async Task<IActionResult> Edit(int? id)
        {
            //Session Check
            if (HttpContext.Session.GetString("username") == null)
            {
                TempData["message"] = "Please Login!";
                return RedirectToAction("Login", "DBSongs");
            }

            if (id == null || _context.TableMusicDB1121735 == null)
            {
                return NotFound();
            }

            var song = await _context.TableMusicDB1121735.FindAsync(id);

            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SongID,Title,Artist,Album,Genre,Duration,ReleaseYear,ImageUrl,CreatedDate")] Songs song)
        {
            //檢查編輯id與Entity的Id是否相等
            if (id != song.SongID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //更新song實體
                    _context.TableMusicDB1121735.Update(song);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongExists(song.SongID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(song);
        }

        private bool SongExists(int id)
        {
            return _context.TableMusicDB1121735.Any(e => e.SongID == id);
        }


        //Delete
        public async Task<IActionResult> Delete(int? id)
        {
            //Session Check
            if (HttpContext.Session.GetString("username") == null)
            {
                TempData["message"] = "Please Login!";
                return RedirectToAction("Login", "DBSongs");
            }

            // 檢查參數
            if (id == null || _context.TableMusicDB1121735 == null)
            {
                return NotFound();
            }

            // 查找特定會員
            var song = await _context.TableMusicDB1121735
                                .FirstOrDefaultAsync(m => m.SongID == id);

            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TableMusicDB1121735 == null)
            {
                return Problem("Entity set 'CmsContext.Songs is null.");
            }

            //以Id找尋Entity然後刪除
            var song = await _context.TableMusicDB1121735.FindAsync(id);

            if (song != null)
            {
                //將該筆資料移除
                _context.TableMusicDB1121735.Remove(song);
                await _context.SaveChangesAsync(); //將資料異動儲存到資料庫
            }

            return RedirectToAction(nameof(Index));
        }

        //Select
        [HttpGet]
        public async Task<IActionResult> SelectQuery()
        {
            //Session Check
            if (HttpContext.Session.GetString("username") == null)
            {
                TempData["message"] = "Please Login!";
                return RedirectToAction("Login", "DBSongs");
            }

            var song_title = await (from p in _context.TableMusicDB1121735
                               orderby p.SongID
                               select p.Title).Distinct().ToListAsync();
            ViewBag.Mylist = song_title;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SelectName(string fTitle)
        {
            var songs = await (from p in _context.TableMusicDB1121735
                               where p.Title == fTitle
                               orderby p.SongID
                               select p).Distinct().ToListAsync();
            return View(songs);
        }


        //Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (username == null && password == null)
            {
                TempData["message"] = "Please enter account and password!";
                return RedirectToAction("Login", "DBSongs");
            }


            var users = await(from p in _context.TableUserDB1121735
                              where p.Username == username && p.Password == password
                              orderby p.UserID
                              select p).ToListAsync();

            if (users.Count != 0)
            {
                HttpContext.Session.SetString("username", username);
                TempData["message"] = "Logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Login failed!";
                return RedirectToAction("Login", "DBSongs");
            }

        }


        ///  Login Session

        [HttpGet]
        public IActionResult Login_Session()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login_Session(string username, string Password)
        {
            if (username == null || Password == null)
            {
                TempData["message"] = "Please enter account and password!";
                return RedirectToAction("Login_Session", "DBSongs");
            }


            var users = await (from p in _context.TableUserDB1121735
                               where p.Username == username && p.Password == Password
                               orderby p.UserID
                               select p).ToListAsync();

            if (users.Count != 0)
            {
                HttpContext.Session.SetString("username", username);
                TempData["message"] = "Logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Login failed!";
                return RedirectToAction("Login_Session", "DBSongs");
            }

        }


    }
}
