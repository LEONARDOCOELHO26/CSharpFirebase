using System;
using System.Text.RegularExpressions;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace CSharpFirebase
{
    public class Cadastro
    {
        public Cadastro()
        {
            // Inicializa o Firebase Admin SDK
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("C:/Users/leonardo.coelho/source/repos/CSharpFirebase/CSharpFirebase/chave/base-user-5182c-firebase-adminsdk-yexhn-0ab6dc123a.json"),
            });
        }

        public async void RealizarCadastro()
        {
            string nomeCompleto, primeiroNome, ultimoNome, senha, confirmarSenha, cpf, quest, usuario;
            bool admin;

            Console.WriteLine("Bem-vindo ao cadastro usando Firebase!");

            Console.WriteLine("O usuário é admin? (S) Sim ou (N) Não");
            quest = Console.ReadLine().ToUpper();

            Console.WriteLine("Digite o nome completo:");
            nomeCompleto = Console.ReadLine();

            string[] partesNome = nomeCompleto.Split(' ');

            // Define o primeiro e o último nome
            primeiroNome = partesNome[0];
            ultimoNome = partesNome[partesNome.Length - 1];

            // Gera o nome de usuário inicial
            usuario = $"{primeiroNome}.{ultimoNome}";

            Console.WriteLine("Digite o CPF:");
            cpf = Console.ReadLine();

            while (true)
            {
                Console.WriteLine("Digite a senha:");
                senha = Console.ReadLine();

                // Valida a senha
                if (ValidarSenha(senha))
                {
                    Console.WriteLine("Senha válida!");
                    break; // Sai do loop se a senha for válida
                }
                else
                {
                    Console.WriteLine("Senha inválida! A senha deve ter pelo menos 8 caracteres, com 1 maiúscula, 1 minúscula e 1 caractere especial. Tente novamente.");
                }
            }

            Console.WriteLine("Confirme a senha:");
            confirmarSenha = Console.ReadLine();

            admin = (quest == "S");

            if (senha == confirmarSenha)
            {
                try
                {
                    // Verifica se o nome de usuário já existe
                    int contador = 0;
                    string emailUsuario = $"{usuario}@exemplo.com";

                    while (await VerificarUsuarioExistente(emailUsuario))
                    {
                        contador++;
                        emailUsuario = $"{usuario}{contador}@exemplo.com"; // Incrementa o número no nome do usuário
                    }

                    // Cria o usuário no Firebase com o nome de usuário único
                    var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs()
                    {
                        Email = emailUsuario, // Email atualizado para garantir que é único
                        Password = senha,
                        DisplayName = nomeCompleto,
                    });

                    Console.WriteLine("Usuário criado com sucesso!");
                    Console.WriteLine($"Usuário: {emailUsuario}");
                    Console.WriteLine($"Senha: {senha}");
                    Console.WriteLine($"Admin: {(admin ? "Sim" : "Não")}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao criar usuário: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("As senhas não são iguais. O cadastro foi cancelado.");
            }
        }

        private async Task<bool> VerificarUsuarioExistente(string email)
        {
            try
            {
                var userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);
                return userRecord != null; // Se encontrou o usuário, o email já existe
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
                {
                    return false; // O usuário não existe
                }
                throw; // Se outro erro acontecer, lança a exceção
            }
        }

        private bool ValidarSenha(string senha)
        {
            // Verifica se a senha atende aos critérios
            if (senha.Length < 8) return false; // Verifica o tamanho

            // Verifica se contém pelo menos uma letra maiúscula, uma minúscula e um caractere especial
            bool temMaiuscula = Regex.IsMatch(senha, @"[A-Z]");
            bool temMinuscula = Regex.IsMatch(senha, @"[a-z]");
            bool temCaracterEspecial = Regex.IsMatch(senha, @"[!@#$%^&*(),.?""':;{}|<>]");

            return temMaiuscula && temMinuscula && temCaracterEspecial;
        }
    }
}
