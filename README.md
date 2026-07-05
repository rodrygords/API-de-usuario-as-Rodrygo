# 🚀 API de Gerenciamento de Usuários

API REST completa para gerenciamento de usuários desenvolvida como projeto acadêmico da disciplina de Desenvolvimento Backend. Implementa operações CRUD com validações robustas, seguindo os princípios de Clean Architecture e aplicando padrões de projeto consolidados.

## 📋 Descrição

Este projeto consiste em uma API RESTful que permite o cadastro, consulta, atualização e remoção de usuários. A aplicação foi desenvolvida utilizando ASP.NET Core 10.0 com Minimal APIs, implementando as melhores práticas de desenvolvimento de software e separação de responsabilidades em camadas.

### 🎯 Funcionalidades Principais

- ✅ Cadastro de usuários com validação completa
- ✅ Listagem de usuários ativos
- ✅ Busca de usuário por ID
- ✅ Atualização de dados do usuário
- ✅ Remoção lógica (soft delete) de usuários
- ✅ Validação de email único
- ✅ Validação de idade mínima (18 anos)
- ✅ Validação de formato de telefone brasileiro
- ✅ Senhas armazenadas exclusivamente como hash PBKDF2 com salt aleatório
- ✅ Respostas da API sem exposição de senha ou hash
- ✅ Erros padronizados com Problem Details (RFC 9457)

> [!IMPORTANT]
> Esta API ainda não implementa autenticação, login, emissão de tokens ou autorização. O hash de senha protege os dados armazenados, mas não transforma os endpoints em endpoints autenticados.

## 🛠️ Tecnologias Utilizadas

- **[.NET 10.0](https://dotnet.microsoft.com/)** - Framework principal
- **[ASP.NET Core](https://docs.microsoft.com/aspnet/core)** - Web framework com Minimal APIs
- **[Entity Framework Core 10.0](https://docs.microsoft.com/ef/core)** - ORM para acesso a dados
- **[SQLite](https://www.sqlite.org/)** - Banco de dados relacional
- **[FluentValidation 12.1](https://docs.fluentvalidation.net/)** - Biblioteca de validação
- **PasswordHasher do ASP.NET Core** - Hash PBKDF2 de senhas
- **[Swagger/OpenAPI](https://swagger.io/)** - Documentação interativa da API

## 🏗️ Arquitetura e Padrões

O projeto segue os princípios de **Clean Architecture** com separação clara de responsabilidades:

### Padrões de Projeto Implementados

- **Repository Pattern**: Abstração da camada de acesso a dados
- **Service Pattern**: Encapsulamento da lógica de negócio
- **DTO Pattern**: Transferência de dados entre camadas
- **Dependency Injection**: Inversão de controle e baixo acoplamento

### Estrutura de Camadas

```
APIUsuarios/
├── Domain/                      # Camada de Domínio
│   └── Entities/
│       └── Usuario.cs          # Entidade principal
│
├── Application/                 # Camada de Aplicação
│   ├── DTOs/                   # Data Transfer Objects
│   │   └── UsuarioDtos.cs
│   ├── Interfaces/             # Contratos
│   │   ├── IUsuarioRepository.cs
│   │   ├── IUsuarioService.cs
│   │   └── IPasswordService.cs
│   ├── Services/               # Lógica de Negócio
│   │   └── UsuarioService.cs
│   └── Validators/             # Validações
│       ├── UsuarioCreateDtoValidator.cs
│       └── UsuarioUpdateDtoValidator.cs
│
├── Infrastructure/              # Camada de Infraestrutura
│   ├── Errors/                  # Tratamento global com Problem Details
│   ├── Persistence/
│   │   └── AppDbContext.cs     # Configuração do EF Core
│   ├── Repositories/
│   │   └── UsuarioRepository.cs # Implementação do Repository
│   └── Security/
│       └── PasswordService.cs   # Implementação do hash de senha
│
├── Migrations/                  # Migrações do EF Core
├── DateTimeConverter.cs         # Conversor de formatação de data
├── Program.cs                   # Configuração e Endpoints
├── appsettings.json            # Configurações da aplicação
└── APIUsuarioss.csproj         # Arquivo do projeto
```

## 📦 Modelo de Dados

### Entidade Usuario

| Campo | Tipo | Descrição | Validações |
|-------|------|-----------|------------|
| Id | int | Identificador único | PK, Auto-increment |
| Nome | string | Nome completo | Obrigatório, 3-100 caracteres |
| Email | string | Endereço de email | Obrigatório, formato válido, único |
| Senha | string | Recebida somente na criação e armazenada como hash PBKDF2 | Obrigatório, mínimo 6 caracteres; nunca retornada pela API |
| DataNascimento | DateTime | Data de nascimento | Obrigatório, idade >= 18 anos |
| Telefone | string | Telefone (opcional) | Formato: (XX) XXXXX-XXXX |
| Ativo | bool | Status do usuário | Padrão: true (soft delete) |
| DataCriacao | DateTime | Data de criação | Preenchido automaticamente |
| DataAtualizacao | DateTime? | Data de atualização | Atualizado automaticamente |

## 🔌 Endpoints da API

### Base URL
```
https://localhost:{5191}
```

### Documentação Interativa
```
https://localhost:{5191}/swagger
```

### Rotas Disponíveis

| Método | Endpoint | Descrição | Status de Sucesso |
|--------|----------|-----------|-------------------|
| `GET` | `/usuarios` | Lista todos os usuários ativos | 200 OK |
| `GET` | `/usuarios/{id}` | Busca usuário por ID | 200 OK |
| `POST` | `/usuarios` | Cria novo usuário | 201 Created |
| `PUT` | `/usuarios/{id}` | Atualiza usuário completo | 200 OK |
| `DELETE` | `/usuarios/{id}` | Remove usuário (soft delete) | 204 No Content |

### Códigos de Status

- **200 OK**: Requisição bem-sucedida
- **201 Created**: Recurso criado com sucesso
- **204 No Content**: Recurso removido com sucesso
- **400 Bad Request**: Dados inválidos
- **404 Not Found**: Recurso não encontrado
- **409 Conflict**: Conflito (ex: email duplicado)
- **500 Internal Server Error**: Erro no servidor

## 🚀 Como Executar o Projeto

### Pré-requisitos

- [.NET SDK 10.0 ou superior](https://dotnet.microsoft.com/download)
- Editor de código (VS Code, Visual Studio, Rider, etc.)
- [Git](https://git-scm.com/) (opcional)

### Passos para Execução

1. **Clone o repositório**
   ```bash
   git clone https://github.com/rodrygords/API-de-usuario-as-Rodrygo
   cd API-de-usuario-as-Rodrygo
   ```

2. **Restaure as dependências**
   ```bash
   dotnet restore
   ```

3. **Aplique as migrations (criar banco de dados)**
   ```bash
   dotnet ef database update
   ```

4. **Execute a aplicação**
   ```bash
   dotnet run
   ```

5. **Acesse a documentação Swagger**
   ```
   https://localhost:{5191}/swagger
   ```
   > A porta será exibida no terminal após executar o projeto

### Comandos Úteis

```bash
# Compilar o projeto
dotnet build

# Executar testes
dotnet test

# Criar nova migration
dotnet ef migrations add NomeDaMigration

# Reverter migration
dotnet ef database update NomeDaMigrationAnterior

# Limpar build
dotnet clean
```

## 🔒 Segurança

- O e-mail é normalizado com remoção de espaços externos e conversão para minúsculas antes de consultas e gravações.
- O índice único `IX_Usuarios_Email` reforça a unicidade no SQLite.
- A senha em texto puro existe apenas durante o processamento da requisição de criação; somente o hash PBKDF2 é persistido.
- Os DTOs de leitura não possuem campos de senha, e a propriedade de hash também é protegida contra serialização acidental.
- Erros inesperados retornam uma mensagem genérica em formato Problem Details. Detalhes técnicos são enviados apenas ao log da aplicação.
- A API não possui autenticação/login. Não deve ser publicada como serviço aberto antes da implementação de autenticação e autorização.
- Bancos criados por versões antigas podem conter senhas em texto puro. Como esta versão ainda não possui login, descarte esses bancos de desenvolvimento e recrie-os antes de continuar usando a aplicação.

## 📮 Exemplos de Requisições

### Criar Usuário (POST /usuarios)

**Request:**
```json
{
  "nome": "João Silva",
  "email": "joao@email.com",
  "senha": "senha123",
  "dataNascimento": "2000-01-15",
  "telefone": "(11) 98765-4321"
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "nome": "João Silva",
  "email": "joao@email.com",
  "dataNascimento": "2000-01-15T00:00:00",
  "telefone": "(11) 98765-4321",
  "ativo": true,
  "dataCriacao": "2025-12-03T18:51:00"
}
```

### Listar Usuários (GET /usuarios)

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "nome": "João Silva",
    "email": "joao@email.com",
    "dataNascimento": "2000-01-15T00:00:00",
    "telefone": "(11) 98765-4321",
    "ativo": true,
    "dataCriacao": "2025-12-03T18:51:00"
  }
]
```

### Buscar por ID (GET /usuarios/1)

**Response (200 OK):**
```json
{
  "id": 1,
  "nome": "João Silva",
  "email": "joao@email.com",
  "dataNascimento": "2000-01-15T00:00:00",
  "telefone": "(11) 98765-4321",
  "ativo": true,
  "dataCriacao": "2025-12-03T18:51:00"
}
```

### Atualizar Usuário (PUT /usuarios/1)

**Request:**
```json
{
  "nome": "João Silva Atualizado",
  "email": "joao@email.com",
  "dataNascimento": "2000-01-15",
  "telefone": "(11) 98765-4321",
  "ativo": true
}
```

**Response (200 OK):**
```json
{
  "id": 1,
  "nome": "João Silva Atualizado",
  "email": "joao@email.com",
  "dataNascimento": "2000-01-15T00:00:00",
  "telefone": "(11) 98765-4321",
  "ativo": true,
  "dataCriacao": "2025-12-03T18:51:00"
}
```

### Deletar Usuário (DELETE /usuarios/1)

**Response (204 No Content)**

> **Nota**: A exclusão é lógica (soft delete). O usuário é marcado como `ativo: false` no banco de dados, mas não é removido fisicamente.

### Erro de Validação (400 Bad Request)

**Request (email inválido):**
```json
{
  "nome": "Teste",
  "email": "emailinvalido",
  "senha": "123456",
  "dataNascimento": "2000-01-01"
}
```

**Response (400 Bad Request):**
```json
{
  "message": "Validação falhou",
  "errors": [
    {
      "field": "Email",
      "message": "Email deve ter formato válido"
    }
  ]
}
```

### Erro de Email Duplicado (409 Conflict)

**Response:**
```json
{
  "message": "Email já cadastrado"
}
```

## ✅ Validações Implementadas

### UsuarioCreateDto
- ✅ Nome: obrigatório, entre 3 e 100 caracteres
- ✅ Email: obrigatório, formato válido
- ✅ Senha: obrigatória, mínimo 6 caracteres
- ✅ DataNascimento: obrigatória, idade >= 18 anos
- ✅ Telefone: opcional, formato `(XX) XXXXX-XXXX`

### UsuarioUpdateDto
- ✅ Nome: obrigatório, entre 3 e 100 caracteres
- ✅ Email: obrigatório, formato válido, único (exceto o próprio usuário)
- ✅ DataNascimento: obrigatória, idade >= 18 anos
- ✅ Telefone: opcional, formato `(XX) XXXXX-XXXX`
- ✅ Ativo: obrigatório (para permitir reativação)

### Validações de Negócio
- ✅ Email único no banco de dados
- ✅ Email normalizado para lowercase
- ✅ Idade mínima de 18 anos
- ✅ Soft delete (marca `Ativo = false` ao invés de deletar)
- ✅ Data de criação e atualização automáticas
- ✅ Formatação customizada de DateTime (sem milissegundos)

## 🧪 Testando com Postman

O projeto inclui uma collection completa do Postman para facilitar os testes.

### Importar a Collection

1. Abra o Postman
2. Clique em **Import**
3. Selecione o arquivo `API-Usuarios.postman_collection.json`
4. Ajuste a variável `base_url` para a porta correta (http://localhost:5191)

### Executar Testes

A collection inclui testes automatizados para:
- ✅ Criação de usuário válido
- ✅ Validação de email duplicado
- ✅ Validação de dados inválidos
- ✅ Validação de idade mínima
- ✅ Listagem de usuários
- ✅ Busca por ID
- ✅ Atualização de dados
- ✅ Soft delete

## 📚 Decisões Técnicas

### Por que Clean Architecture?
Separação clara de responsabilidades, facilitando manutenção, testes e escalabilidade.

### Por que Repository Pattern?
Abstrai a camada de dados, permitindo trocar o banco sem impactar a lógica de negócio.

### Por que DTOs?
Evita exposição de dados sensíveis (ex: senha) e desacopla a API das entidades de domínio.

### Por que FluentValidation?
Validações mais legíveis, testáveis e reutilizáveis do que Data Annotations.

### Por que SQLite?
Simplicidade para desenvolvimento e entrega, sem necessidade de servidor de banco separado.

### Por que Soft Delete?
Mantém histórico, permite auditoria e possibilita recuperação de dados.

### Por que Conversor Customizado de DateTime?
Para garantir formato consistente e limpo nas respostas da API, removendo milissegundos e timezone desnecessários.

## 🎓 Aprendizados

- ✅ Implementação prática de Clean Architecture
- ✅ Aplicação de padrões de projeto em cenários reais
- ✅ Uso de Entity Framework Core com Code First
- ✅ Validações robusas com FluentValidation
- ✅ Injeção de dependências no ASP.NET Core
- ✅ Desenvolvimento de APIs RESTful seguindo boas práticas
- ✅ Documentação automática com Swagger/OpenAPI
- ✅ Customização de serialização JSON no .NET
- ✅ Implementação de soft delete para manutenção de histórico

## 📝 Licença

Este projeto foi desenvolvido para fins acadêmicos como parte da Avaliação Semestral da disciplina de Desenvolvimento Backend.

## 👤 Autor

**Rodrygo de Souza**
- Curso: [ADS]
- Disciplina: [Desenvolvimento Backend]
- Instituição: [Ulbra]

## 🎥 Vídeo Demonstrativo

**Assista à apresentação completa do projeto:**

[![Vídeo de Apresentação](https://img.youtube.com/vi/pDCNv2tcuE0/maxresdefault.jpg)](https://youtu.be/pDCNv2tcuE0)

🔗 **Link direto:** https://youtu.be/pDCNv2tcuE0

No vídeo você encontrará:
- Estrutura completa do projeto
- Explicação detalhada do código
- Demonstração de todos os endpoints
- Validações e tratamento de erros
- Verificação do soft delete no banco de dados

---
