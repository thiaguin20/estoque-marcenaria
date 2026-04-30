using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Estoque.Helpers;
using Estoque.Models;
using Estoque.Models.ViewModels;

namespace Estoque.Controllers
{
    public class AdminController : Controller
    {
        private readonly EstoqueContext db = new EstoqueContext();

        public ActionResult Login()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Login(GET) error: " + ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                GarantirTabelaUsuarios();

                var adminEmail = ConfigurationManager.AppSettings["AdminEmail"];
                var storedHash = ConfigurationManager.AppSettings["AdminPasswordHash"];

                if (!string.IsNullOrWhiteSpace(adminEmail) &&
                    !string.IsNullOrWhiteSpace(storedHash) &&
                    model.Usuario == adminEmail &&
                    SecurityHelper.VerificarSenha(model.Senha ?? string.Empty, storedHash))
                {
                    Session["UsuarioLogado"] = adminEmail;
                    Session["EhAdminPrincipal"] = true;
                    return RedirectToAction("Index", "Produto");
                }

                var email = model.Usuario.Trim().ToLower();
                var senhaHash = SecurityHelper.HashSHA256(model.Senha ?? string.Empty);
                var usuarioBanco = db.Usuarios.FirstOrDefault(u => u.Email == email && u.SenhaHash == senhaHash);

                if (usuarioBanco != null)
                {
                    if (!usuarioBanco.PodeAcessarAdmin)
                    {
                        ViewBag.Erro = "Cadastro aguardando liberacao do administrador.";
                        return View();
                    }

                    Session["UsuarioLogado"] = usuarioBanco.Email;
                    Session["UsuarioNome"] = usuarioBanco.Nome;
                    Session["EhAdminPrincipal"] = false;
                    return RedirectToAction("Index", "Produto");
                }

                ViewBag.Erro = "Usuario ou senha invalidos.";
                return View(model);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Login error: " + ex);
                ViewBag.Erro = "Ocorreu um erro ao tentar fazer login. Contate o administrador.";
                return View(model);
            }
        }

        public ActionResult Cadastro()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Cadastro(GET) error: " + ex);
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cadastro(CadastroUsuarioViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var email = model.Email.Trim().ToLower();

                GarantirTabelaUsuarios();

                if (db.Usuarios.Any(u => u.Email == email))
                {
                    ViewBag.Erro = "Ja existe um usuario cadastrado com este email.";
                    return View(model);
                }

                var usuario = new Usuario
                {
                    Nome = model.Nome.Trim(),
                    Email = email,
                    SenhaHash = SecurityHelper.HashSHA256(model.Senha),
                    PodeAcessarAdmin = false,
                    DataCadastro = DateTime.Now
                };

                db.Usuarios.Add(usuario);
                db.SaveChanges();

                ViewBag.Sucesso = "Cadastro criado. Aguarde o admin liberar seu acesso.";
                ModelState.Clear();
                return View();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Cadastro error: " + ex);
                ViewBag.Erro = "Ocorreu um erro ao cadastrar o usuario.";
                return View(model);
            }
        }

        private void GarantirTabelaUsuarios()
        {
            db.Database.ExecuteSqlCommand(@"
IF OBJECT_ID('dbo.Usuarios', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Nome NVARCHAR(100) NOT NULL,
        Email NVARCHAR(150) NOT NULL,
        SenhaHash NVARCHAR(64) NOT NULL,
        PodeAcessarAdmin BIT NOT NULL,
        DataCadastro DATETIME NOT NULL
    )
END");
        }

        public ActionResult Logout()
        {
            try
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                Trace.TraceError("Logout error: " + ex);
                return RedirectToAction("Login");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
