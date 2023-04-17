using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Text_Editor.Models;

namespace Text_Editor.Controllers
{
    public class FileController : Controller
    {
        public IActionResult Index()
        {
            string? email = HttpContext.Session.GetString("uEmail");
            if (email == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<FileModel> files = new();
            SqlCommand cmd = new($"select name, lastAccessed from files where userID='{email}'", Connection.conn);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    files.Add(new FileModel
                    {
                        Name = reader.GetString(0),
                        LastAccessed = reader.GetDateTime(1)
                    });
                }
            }
            return View(files);
        }

        // GET: File/New?name=
        public IActionResult New(string name)
        {
            string? email = HttpContext.Session.GetString("uEmail");
            if (email == null)
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                // check if file name is already used
                SqlCommand cmd = new($"select count(*) from files where name='{name}' and userID='{email}'", Connection.conn);
                int? res = (int?)cmd.ExecuteScalar();
                if (res == null || res! == 0) // not used already
                {
                    cmd.CommandText = "newFile";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@userID", email);
                    cmd.ExecuteNonQuery();
                    return RedirectToAction("Edit", new { name });
                }
                else
                {
                    TempData["error"] = "File with the given name already exists";
                }
            }
            catch
            {
                TempData["error"] = "Unable to create file";
            }
            return RedirectToAction("Index");
        }

        // GE:T File/Edit?name=
        public IActionResult Edit(string name)
        {
            string? email = HttpContext.Session.GetString("uEmail");
            if (email == null)
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                SqlCommand cmd = new($"select * from files where name='{name}' and userID='{email}'", Connection.conn);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        FileModel model = new()
                        {
                            Name = reader.GetString(0),
                            UserID = reader.GetString(1),
                            LastAccessed = DateTime.Now,
                            Content = reader.GetString(3),
                        };
                        reader.Close();
                        cmd.CommandText = "updateLastAccessed";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@name", model.Name);
                        cmd.Parameters.AddWithValue("@userID", model.UserID);
                        cmd.ExecuteNonQuery();
                        return View(model);
                    }
                    else // File doesn't exist
                    {
                        TempData["error"] = "File doesn't exist";
                    }
                }
            }
            catch
            {
                TempData["error"] = "Unable to open file";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: File/Edit?name=
        public IActionResult Edit(FileModel model)
        {
			string? email = HttpContext.Session.GetString("uEmail");
			if (email == null)
			{
				return RedirectToAction("Login", "Account");
			}
			try
			{
				SqlCommand cmd = new("updateContent", Connection.conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@name", model.Name);
                cmd.Parameters.AddWithValue("@userID", email);
                cmd.Parameters.AddWithValue("@content", model.Content);
                cmd.ExecuteNonQuery();
				TempData["success"] = "File saved";
			}
			catch(Exception ex)
			{
                TempData["error"] = ex.Message;// "Unable to perform action";
			    return View(model);
            }
            return View(model);
        }
    }
}
