using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;
using System.Data.Common;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Beneficiarios()
        {

            return View();
        }

        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente bo = new BoCliente();

            var cpf = RetornaCPFseValido(model.CPF);

            if (cpf == null)
            {
                Response.StatusCode = 400;
                return Json("CPF inválido. Verifique e tente novamente.");
            }
            if (CPFcadastrado(cpf))
            {
                return Json("Já existe um usuário cadastrado com esse CPF.");
            }

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {

                model.Id = bo.Incluir(new Cliente()
                {
                    CEP = model.CEP,
                    CPF = cpf,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone
                });


                return Json("Cadastro efetuado com sucesso");
            }
        }

        public JsonResult IncluirBeneficiario(BeneficiariosModel model)
        {
            BoBeneficiario bo = new BoBeneficiario();

            var cpf = RetornaCPFseValido(model.CPF);

            if (cpf == null)
            {
                Response.StatusCode = 400;
                return Json("CPF inválido. Verifique e tente novamente.");
            }

            List<Beneficiarios> cpfCadastrados = new BoBeneficiario().Pesquisa(model.IdCliente);

           if( cpfCadastrados.Any(c => c.CPF == cpf))
                return Json("Já existe um usuário cadastrado com esse CPF.");

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {

                model.Id = bo.Incluir(new Beneficiarios()
                {
                    CPF = cpf,
                    Nome = model.Nome,
                    IdCliente = model.IdCliente
                });


                return Json("Cadastro efetuado com sucesso");
            }
        }


        [HttpPost]
        //aqui
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();

            var cpf = RetornaCPFseValido(model.CPF);

            if (cpf == null)
            {
                Response.StatusCode = 400;
                return Json("CPF inválido. Verifique e tente novamente.");
            }

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                bo.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CPF = cpf,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone
                });

                return Json("Cadastro alterado com sucesso");
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CPF = cliente.CPF,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone
                };


            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        public bool CPFcadastrado(string cpf, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = "Nome ASC")
        {
            List<Cliente> clientes = new BoCliente().Pesquisa(0, 5, "CPF", true, out int qtd);

            bool cpfCadastrado = clientes.Any(c => c.CPF == cpf);

            return cpfCadastrado;

        }
        public JsonResult BeneficiarioList(long id)
        {

            List<Beneficiarios> clientes = new BoBeneficiario().Pesquisa(id);
            int qtd = clientes.Count;

            // Return result to jTable
            return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
        }

        public String RetornaCPFseValido(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
            {
                return null;
            }
            cpf = cpf.Replace(".", "");

            cpf = cpf.Replace("-", "");

            // Remove any non-numeric characters from the input string


            // CPF must have exactly 11 digits
            if (cpf.Length != 11)
            {
                return null;
            }

            //// Check for known invalid CPF numbers
            string[] invalidCpfs = { "00000000000", "11111111111", "22222222222", "33333333333", "44444444444", "55555555555", "66666666666", "77777777777", "88888888888", "99999999999" };
            if (invalidCpfs.Contains(cpf))
            {
                return null;
            }

            // Calculate the verification digits
            int sum1 = 0, sum2 = 0;
            for (int i = 0; i < 9; i++)
            {
                sum1 += int.Parse(cpf[i].ToString()) * (10 - i);
                sum2 += int.Parse(cpf[i].ToString()) * (11 - i);
            }
            int dv1 = (sum1 * 10) % 11;
            if (dv1 == 10)
            {
                dv1 = 0;
            }
            int dv2 = ((sum2 + dv1 * 2) * 10) % 11;
            if (dv2 == 10)
            {
                dv2 = 0;
            }

            // Compare the calculated verification digits to the input digits
            if (cpf[9] == dv1.ToString()[0] && cpf[10] == dv2.ToString()[0])
                return cpf;
            else
                return null;
        }

    }
}