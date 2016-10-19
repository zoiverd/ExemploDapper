using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace ExemploDapper
{
    class Program
    {
        static void Main(string[] args)
        {
            var strConn = ConfigurationManager.ConnectionStrings["dbDapper"].ConnectionString;

            //A conexão deve ser criada com using, para garantir que ela fechou
            using(var conn = new SqlConnection(strConn))
            {
                conn.Open();

                for(var i = 0; i < 10; i++)
                {
                    //Sempre utilize parâmetros NUNCA concatenar os valores como string
                    var strSql = "insert into cliente (nome, email, telefone) values (@nome, @email, @telefone)";

                    var cliente = new Cliente
                    {
                        Nome = $"Cliente {i}",
                        Email = $"emailcliente{i}@teste.com.br",
                        Telefone = 11999999999
                    };
                
                    //Cria o comando associado a conexão criada anteriormente e informa qual o script a ser executado
                    var command = conn.CreateCommand();
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = strSql;

                    //Adiciona os parametros que a query precisa ter
                    command.Parameters.AddWithValue("@nome", cliente.Nome);
                    command.Parameters.AddWithValue("@email", cliente.Email);
                    command.Parameters.AddWithValue("@telefone", cliente.Telefone);

                    //Executa o comando
                    command.ExecuteScalar();
                }

                //Aqui executa a seleção
                //Esta mágica de pegar os campos e já preencher no objeto é feita pelo dapper
                //Note que para isto funcionar, o nome ou alias da tabela deve ser igual ao nome do campo na classe CLIENTE
                var clientes = conn.Query<Cliente>("select id, nome, email, telefone from cliente");

                foreach(var cliente in clientes)
                {
                    Console.WriteLine($"Id: {cliente.Id} | Nome: {cliente.Nome} | Email: {cliente.Email} | Telefone: {cliente.Telefone}");
                }
            }

            Console.ReadLine();


        }
    }
}
