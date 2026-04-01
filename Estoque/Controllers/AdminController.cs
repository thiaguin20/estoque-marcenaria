using System.Web.Mvc;

namespace Estoque.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string usuario, string senha)
        {
            if (usuario == "admin" && senha == "123")
            {
                Session["UsuarioLogado"] = usuario;
                return RedirectToAction("Index", "Produto");
            }

            ViewBag.Erro = "Usuário ou senha inválidos";
            return View();
        }

        public ActionResult Logout()
        {
            Session["UsuarioLogado"] = null;
            return RedirectToAction("Login");
        }
    }
}