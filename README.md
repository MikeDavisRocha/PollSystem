# PollSystem

Um sistema de votação em tempo real simples e robusto, construído com .NET 9.

## 🚀 Tecnologias

*   **C# / .NET 9**
*   **ASP.NET Core Minimal APIs**
*   **Entity Framework Core** (SQL Server)
*   **SignalR** (para atualizações em tempo real)

## 📂 Estrutura do Projeto

O projeto segue uma arquitetura de **Vertical Slice** simplificada, onde as funcionalidades são agrupadas por features em vez de camadas técnicas.

*   `PollSystem.API/Features/Polls`: Contém tudo relacionado a Enquetes (Modelos, Endpoints, Lógica).

## ⚙️ Como Rodar

### Pré-requisitos

*   [.NET 9 SDK](https://dotnet.microsoft.com/download)
*   SQL Server (ou LocalDB, que vem com o Visual Studio)

### Passos

1.  Clone o repositório ou navegue até a pasta do projeto.
2.  Restaure as dependências e compile:
    ```bash
    dotnet build
    ```
3.  Aplique as migrações do banco de dados (certifique-se de que a ConnectionString no `appsettings.json` está correta para o seu ambiente):
    ```bash
    cd PollSystem.API
    dotnet ef database update
    ```
4.  Execute a aplicação:
    ```bash
    dotnet run
    ```
    A API estará rodando em `http://localhost:5000` (ou a porta configurada).

## 🧪 Como Testar

Um arquivo `requests.http` foi incluído na raiz do projeto `PollSystem.API`. Você pode usá-lo com a extensão **REST Client** do VS Code ou diretamente no Visual Studio 2022+.

1.  Abra o arquivo `PollSystem.API/requests.http`.
2.  Envie a requisição **Create a new Poll** para criar uma enquete.
3.  Copie o `id` retornado.
4.  Use o `id` para buscar a enquete (**Get Poll**) ou votar (**Vote**).
5.  Ao votar, se houver um cliente SignalR conectado, ele receberá o evento `ReceiveVote` em tempo real.
