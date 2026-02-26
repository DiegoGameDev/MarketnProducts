# MarketnProducts
Sistema web para gerenciamento de mercados e produtos, com autenticação via Identity, confirmação de e-mail, controle de roles e operações CRUD. Desenvolvido com ASP.NET Core, Entity Framework e SQL Server.


🚀 Tecnologias utilizadas

ASP.NET Core

Entity Framework Core

SQL Server

Identity

⚙️ Como executar o projeto
📌 Pré-requisitos

.NET SDK (10.0)

SQL Server (Express ou Developer)

Visual Studio ou VS Code


1️⃣ Clone o repositório
git clone <url-do-repositorio>

2️⃣ Configure a Connection String

Abra o arquivo:

appsettings.json


Configure a connection string de acordo com sua instância local do SQL Server. Exemplo:

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=MarketDB;Trusted_Connection=True;TrustServerCertificate=True;"
}


Caso utilize SQL Express:

Server=localhost\\SQLEXPRESS;

3️⃣ Execute as migrations

No terminal, dentro da pasta do projeto:

dotnet ef database update


Ou pelo Package Manager Console:

Update-Database


Isso criará automaticamente o banco de dados e as tabelas.

4️⃣ Execute o projeto
dotnet run


Ou pressione F5 no Visual Studio.

Acesse a URL exibida no terminal (ex: https://localhost:xxxx
).

O programa já vai criar um usuario Admin para você já conseguir acessar
Adicione a role de Reviewer ao usuário para ter receber email de solicitação. ltere o email do usuário para um existente (opcional)
*Não esqueça de colocar seu email e a sua passKey para o programa ter acesso ao envio de mensagem a partir do seu email! 

Faça bom uso e fique a vontade para comentar sobre o código
