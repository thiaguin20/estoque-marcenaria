using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Estoque.Models;

namespace Estoque.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly EstoqueContext db = new EstoqueContext();

        public ActionResult Index()
        {
            try
            {
                if (!AdminPrincipalLogado())
                {
                    return RedirectToAction("Index", "Produto");
                }

                GarantirTabelaUsuarios();

                var usuarios = db.Usuarios
                    .OrderBy(u => u.PodeAcessarAdmin)
                    .ThenByDescending(u => u.DataCadastro)
                    .ToList();

                return View(usuarios);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Usuarios/Index error: " + ex);
                TempData["Erro"] = "Ocorreu um erro ao carregar os usuarios.";
                return RedirectToAction("Index", "Produto");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AlternarPermissao(int id)
        {
            if (!AdminPrincipalLogado())
            {
                return RedirectToAction("Index", "Produto");
            }

            try
            {
                GarantirTabelaUsuarios();

                var usuario = db.Usuarios.Find(id);

                if (usuario == null)
                {
                    TempData["Erro"] = "Usuario nao encontrado.";
                    return RedirectToAction("Index");
                }

                usuario.PodeAcessarAdmin = !usuario.PodeAcessarAdmin;
                db.SaveChanges();

                TempData["Sucesso"] = usuario.PodeAcessarAdmin
                    ? "Acesso liberado para o usuario."
                    : "Acesso bloqueado para o usuario.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Trace.TraceError("AlternarPermissao error: " + ex);
                TempData["Erro"] = "Ocorreu um erro ao alterar a permissao.";
                return RedirectToAction("Index");
            }
        }

        private bool AdminPrincipalLogado()
        {
            return Session["UsuarioLogado"] != null &&
                   Session["EhAdminPrincipal"] != null &&
                   (bool)Session["EhAdminPrincipal"];
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
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
    }
}
