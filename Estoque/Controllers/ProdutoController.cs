using Estoque.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    public class ProdutoController : Controller
    {
        private EstoqueContext db = new EstoqueContext();

        public ActionResult Index(string busca, string filtro)
        {
            try
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
            catch (Exception ex)
            {
                Trace.TraceError("Index error: " + ex);
                TempData["Error"] = "Ocorreu um erro ao carregar os produtos.";
                return View(Enumerable.Empty<Produto>());
            }
        }
        // ===============================
        // GET: /Produto/Create
        // ===============================
        public ActionResult Create()
        {
            try
            {
                if (Session["UsuarioLogado"] == null)
                {
                    return RedirectToAction("Login", "Admin");
                }

                return View();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Create(GET) error: " + ex);
                TempData["Error"] = "Ocorreu um erro ao abrir o cadastro de produto.";
                return RedirectToAction("Index");
            }
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

                try
            {
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
            catch (Exception ex)
            {
                Trace.TraceError("Create error: " + ex);
                ModelState.AddModelError("", "Ocorreu um erro ao salvar o produto. Tente novamente.");
                return View(produto);
            }
        }

        // ===============================
        // GET: /Produto/Edit/5
        // ===============================
        public ActionResult Edit(int? id)
        {
            try
            {
                if (Session["UsuarioLogado"] == null)
                    return RedirectToAction("Login", "Admin");

                if (id == null)
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

                Produto produto = db.Produtos.Find(id);

                if (produto == null)
                    return HttpNotFound();

                // Agora usa a view Edit separada
                return View(produto);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Edit(GET) error: " + ex);
                TempData["Error"] = "Ocorreu um erro ao abrir o produto para edição.";
                return RedirectToAction("Index");
            }
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

            try
            {
                ModelState.Remove("ImagemUrl");

                if (ModelState.IsValid)
                {
                    var produtoOriginal = db.Produtos.Find(produto.Id);

                    if (produtoOriginal == null)
                    {
                        ModelState.AddModelError("", "Produto não encontrado.");
                        return View(produto);
                    }

                    // Atualiza dados
                    produtoOriginal.Nome = produto.Nome;
                    produtoOriginal.Preco = produto.Preco;
                    produtoOriginal.Descricao = produto.Descricao;
                    produtoOriginal.Quantidade = produto.Quantidade;

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

                return View(produto);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Edit(POST) error: " + ex);
                ModelState.AddModelError("", "Ocorreu um erro ao atualizar o produto. Tente novamente.");
                return View(produto);
            }
        }

        // ===============================
        // GET: /Produto/Delete/5
        // ===============================
        public ActionResult Delete(int? id)
        {
            try
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
            catch (Exception ex)
            {
                Trace.TraceError("Delete(GET) error: " + ex);
                TempData["Error"] = "Ocorreu um erro ao abrir o produto para remoção.";
                return RedirectToAction("Index");
            }
        }
        // ===============================
        // POST: /Produto/Delete/5
        // ===============================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                if (Session["UsuarioLogado"] == null)
                    return RedirectToAction("Login", "Admin");

                Produto produto = db.Produtos.Find(id);

                if (produto == null)
                {
                    TempData["Error"] = "Produto não encontrado.";
                    return RedirectToAction("Index");
                }

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
            catch (Exception ex)
            {
                Trace.TraceError("Delete(POST) error: " + ex);
                TempData["Error"] = "Ocorreu um erro ao remover o produto.";
                return RedirectToAction("Index");
            }
        }

        public ActionResult Entrada(int id)
        {
            try
            {
                if (Session["UsuarioLogado"] == null)
                    return RedirectToAction("Login", "Admin");

                var produto = db.Produtos.Find(id);
                return View(produto);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Entrada(GET) error: " + ex);
                TempData["Error"] = "Ocorreu um erro.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Entrada(int id, int quantidade)
        {
            try
            {
                if (Session["UsuarioLogado"] == null)
                    return RedirectToAction("Login", "Admin");

                var produto = db.Produtos.Find(id);

                if (produto == null)
                {
                    TempData["Error"] = "Produto não encontrado.";
                    return RedirectToAction("Index");
                }

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
            catch (Exception ex)
            {
                Trace.TraceError("Entrada(POST) error: " + ex);
                TempData["Error"] = "Ocorreu um erro ao registrar a entrada.";
                return RedirectToAction("Index");
            }
        }

        public ActionResult Saida(int id)
        {
            try
            {
                if (Session["UsuarioLogado"] == null)
                    return RedirectToAction("Login", "Admin");

                var produto = db.Produtos.Find(id);
                return View(produto);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Saida(GET) error: " + ex);
                TempData["Error"] = "Ocorreu um erro.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Saida(int id, int quantidade)
        {
            try
            {
                if (Session["UsuarioLogado"] == null)
                    return RedirectToAction("Login", "Admin");

                var produto = db.Produtos.Find(id);

                if (produto == null)
                {
                    TempData["Error"] = "Produto não encontrado.";
                    return RedirectToAction("Index");
                }

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
                else
                {
                    TempData["Error"] = "Quantidade insuficiente em estoque.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Trace.TraceError("Saida(POST) error: " + ex);
                TempData["Error"] = "Ocorreu um erro ao registrar a saída.";
                return RedirectToAction("Index");
            }
        }

        public ActionResult HistoricoEntrada()
        {
            try
            {
                if (Session["UsuarioLogado"] == null)
                    return RedirectToAction("Login", "Admin");

                var entradas = db.Movimentacoes
                    .Where(m => m.Tipo == "Entrada")
                    .ToList();

                return View(entradas);
            }
            catch (Exception ex)
            {
                Trace.TraceError("HistoricoEntrada error: " + ex);
                TempData["Error"] = "Ocorreu um erro ao carregar o histórico.";
                return RedirectToAction("Index");
            }
        }

        public ActionResult HistoricoSaida()
        {
            try
            {
                if (Session["UsuarioLogado"] == null)
                    return RedirectToAction("Login", "Admin");

                var saidas = db.Movimentacoes
                    .Where(m => m.Tipo == "Saida")
                    .ToList();

                return View(saidas);
            }
            catch (Exception ex)
            {
                Trace.TraceError("HistoricoSaida error: " + ex);
                TempData["Error"] = "Ocorreu um erro ao carregar o histórico.";
                return RedirectToAction("Index");
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
