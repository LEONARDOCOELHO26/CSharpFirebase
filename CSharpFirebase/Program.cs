using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CSharpFirebase
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            string opcao;

            Console.WriteLine("===== Sistema de Cadastro e Login =====");

            while (true)
            {
                Console.WriteLine("\nEscolha uma opção:");
                Console.WriteLine("1 - Cadastrar");
                Console.WriteLine("2 - Logar");
                Console.WriteLine("3 - Sair");

                Console.Write("\nOpção: ");
                opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine("===== Cadastro de Usuário =====");
                        Cadastro cadastro = new Cadastro();
                        cadastro.RealizarCadastro();
                        break;

                    case "2":
                        Console.Clear();
                        Console.WriteLine("===== Login de Usuário =====");
                        await RealizarLogin();
                        break;

                    case "3":
                        Console.WriteLine("\nEncerrando o programa...");
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("\nOpção inválida, tente novamente.");
                        break;
                }
            }
        }

        private static async Task RealizarLogin()
        {
            Console.Write("\nDigite o usuário : ");
            string email = Console.ReadLine();
            email = $"{email}@exemplo.com";

            Console.Write("Digite a senha: ");
            string senha = Console.ReadLine();

            try
            {
                string apiKey = "coloque sua api";
                var client = new HttpClient();
                var requestUri = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}";

                var content = new StringContent(
                    JsonConvert.SerializeObject(new
                    {
                        email,
                        password = senha,
                        returnSecureToken = true
                    }),
                    Encoding.UTF8, "application/json");

                var response = await client.PostAsync(requestUri, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseBody);

                Console.WriteLine("\nLogin realizado com sucesso!");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"\nErro ao fazer login: {e.Message}");
            }
        }
    }
}
