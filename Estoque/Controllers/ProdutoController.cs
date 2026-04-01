using Estoque.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    public class ProdutoController : Controller
    {
        private EstoqueContext db = new EstoqueContext();

        public ActionResult Index(string busca, string filtro)
        {
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Login", "Admin");

            var produtos = db.Produtos.AsQueryable();

            if (!string.IsNullOrEmpty(busca))
            {
                produtos = produtos.Where(p => p.Nome.Contains(busca));
            }

            // 🎯 FILTROS
            if (filtro == "baixo")
            {
                produtos = produtos.Where(p => p.Quantidade < 50);
            }
            else if (filtro == "medio")
            {
                produtos = produtos.Where(p => p.Quantidade >= 50 && p.Quantidade <= 100);
            }
            else if (filtro == "alto")
            {
                produtos = produtos.Where(p => p.Quantidade > 100);
            }

            return View(produtos.ToList());
        }
        // ===============================
        // GET: /Produto/Create
        // ===============================
        public ActionResult Create()
        {
            if (Session["UsuarioLogado"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            return View();
        }

        // ===============================
        // POST: /Produto/Create
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Produto produto, System.Web.HttpPostedFileBase fotoUpload)
        {
            if (Session["UsuarioLogado"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            if (ModelState.IsValid)
            {
                // 📸 Upload da imagem
                if (fotoUpload != null && fotoUpload.ContentLength > 0)
                {
                    string nomeArquivo = System.IO.Path.GetFileName(fotoUpload.FileName);

                    string caminho = System.IO.Path.Combine(
                        Server.MapPath("~/Content/images"),
                        nomeArquivo
                    );

                    fotoUpload.SaveAs(caminho);

                    produto.ImagemUrl = "~/Content/images/" + nomeArquivo;
                }
                else
                {
                    produto.ImagemUrl = "~/Content/images/default.jfif";
                }

                db.Produtos.Add(produto);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(produto);
        }

        // ===============================
        // GET: /Produto/Edit/5
        // ===============================
        public ActionResult Edit(int? id)
        {
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Login", "Admin");

            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            Produto produto = db.Produtos.Find(id);

            if (produto == null)
                return HttpNotFound();

            // 👇 PULO DO GATO (usa a mesma view do Create)
            return View("Create", produto);
        }

        // ===============================
        // POST: /Produto/Edit/5
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Produto produto, System.Web.HttpPostedFileBase fotoUpload)
        {
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Login", "Admin");

            ModelState.Remove("ImagemUrl");

            if (ModelState.IsValid)
            {
                var produtoOriginal = db.Produtos.Find(produto.Id);

                // Atualiza dados
                produtoOriginal.Nome = produto.Nome;
                produtoOriginal.Preco = produto.Preco;
                produtoOriginal.Descricao = produto.Descricao;

                // 📸 Nova imagem (se enviou)
                if (fotoUpload != null && fotoUpload.ContentLength > 0)
                {
                    string nomeArquivo = System.IO.Path.GetFileName(fotoUpload.FileName);

                    string caminho = System.IO.Path.Combine(
                        Server.MapPath("~/Content/images"),
                        nomeArquivo
                    );

                    fotoUpload.SaveAs(caminho);

                    produtoOriginal.ImagemUrl = "~/Content/images/" + nomeArquivo;
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View("Create", produto);
        }

        // ===============================
        // GET: /Produto/Delete/5
        // ===============================
        public ActionResult Delete(int? id)
        {
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Login", "Admin");

            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            Produto produto = db.Produtos.Find(id);

            if (produto == null)
                return HttpNotFound();

            return View(produto);
        }
        // ===============================
        // POST: /Produto/Delete/5
        // ===============================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Login", "Admin");

            Produto produto = db.Produtos.Find(id);

            // 🧹 (opcional) remove imagem do servidor
            if (!string.IsNullOrEmpty(produto.ImagemUrl) && produto.ImagemUrl != "~/Content/images/default.jpeg")
            {
                string caminho = Server.MapPath(produto.ImagemUrl);

                if (System.IO.File.Exists(caminho))
                {
                    System.IO.File.Delete(caminho);
                }
            }

            db.Produtos.Remove(produto);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Entrada(int id)
        {
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Login", "Admin");

            var produto = db.Produtos.Find(id);
            return View(produto);
        }

        [HttpPost]
        public ActionResult Entrada(int id, int quantidade)
        {
            var produto = db.Produtos.Find(id);

            produto.Quantidade += quantidade;

            db.Movimentacoes.Add(new Movimentacao
            {
                ProdutoId = produto.Id,
                NomeProduto = produto.Nome,
                Tipo = "Entrada",
                Quantidade = quantidade,
                Data = DateTime.Now
            });

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Saida(int id)
        {
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Login", "Admin");

            var produto = db.Produtos.Find(id);
            return View(produto);
        }

        [HttpPost]
        public ActionResult Saida(int id, int quantidade)
        {
            var produto = db.Produtos.Find(id);

            if (produto.Quantidade >= quantidade)
            {
                produto.Quantidade -= quantidade;

                db.Movimentacoes.Add(new Movimentacao
                {
                    ProdutoId = produto.Id,
                    NomeProduto = produto.Nome,
                    Tipo = "Saida",
                    Quantidade = quantidade,
                    Data = DateTime.Now
                });

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult HistoricoEntrada()
        {
            var entradas = db.Movimentacoes
                .Where(m => m.Tipo == "Entrada")
                .ToList();

            return View(entradas);
        }
       

       
        public ActionResult HistoricoSaida()
        {
            var saidas = db.Movimentacoes
                .Where(m => m.Tipo == "Saida")
                .ToList();

            return View(saidas);
        }
    }
}