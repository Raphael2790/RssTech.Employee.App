# 👥 RssTech Employee Management API

## 📋 Finalidade do Projeto

Sistema de gerenciamento de funcionários desenvolvido para demonstrar uma aplicação .NET moderna seguindo princípios de Clean Architecture, Domain-Driven Design (DDD) e CQRS. O sistema permite cadastro, autenticação e gerenciamento hierárquico de funcionários com controle de acesso baseado em roles.

## 🛠️ Tecnologias e Pacotes Utilizados

### **Framework Base**
- **.NET 9.0** - Framework principal
- **ASP.NET Core** - Web API
- **C#** - Linguagem de programação

### **Banco de Dados**
- **PostgreSQL** - Banco de dados principal
- **Entity Framework Core 9.0.8** - ORM
- **Npgsql.EntityFrameworkCore.PostgreSQL 9.0.4** - Provider PostgreSQL

### **Autenticação e Segurança**
- **JWT Bearer Authentication** - Autenticação via tokens
- **System.IdentityModel.Tokens.Jwt 8.2.1** - Manipulação de tokens JWT

### **Arquitetura e Padrões**
- **MediatR 12.4.1** - Implementação do padrão Mediator para CQRS
- **FluentValidation 12.0.0** - Validação de dados

### **Logs e Monitoramento**
- **Serilog.AspNetCore 8.0.3** - Framework de logging estruturado
- **Serilog.Sinks.Elasticsearch 10.0.0** - Sink para Elasticsearch
- **Serilog.Enrichers.Environment 3.0.1** - Enriquecimento de logs
- **Serilog.Enrichers.Process 3.0.0** - Informações de processo
- **Serilog.Enrichers.Thread 4.0.0** - Informações de thread

### **Containerização e Deploy**
- **Docker** - Containerização da aplicação
- **Docker Compose** - Orquestração de serviços
- **Elasticsearch 8.15.0** - Armazenamento e indexação de logs
- **Kibana 8.15.0** - Visualização de logs e dashboards

### **Testes**
- **xUnit** - Framework de testes unitários
- **Projetos de teste separados** para Domain e Application

## 🏗️ Arquitetura do Projeto

O projeto segue os princípios da **Clean Architecture** com separação clara de responsabilidades:

```
RssTech.Employee.App/
├── 📁 RssTech.Employee.Api/           # Camada de Apresentação
│   ├── Endpoints/                     # Minimal APIs endpoints
│   ├── Extensions/                    # Extensões da API
│   └── Program.cs                     # Configuração da aplicação
│
├── 📁 RssTech.Employee.Application/   # Camada de Aplicação
│   ├── UseCases/                      # Casos de uso (CQRS)
│   │   ├── Auth/                      # Autenticação
│   │   └── Employee/                  # Operações de funcionários
│   └── Mappings/                      # Mapeamentos entre camadas
│
├── 📁 RssTech.Employee.Domain/        # Camada de Domínio
│   ├── Entities/                      # Entidades de domínio
│   ├── ValueObjects/                  # Objetos de valor
│   ├── Enums/                         # Enumerações
│   └── Validators/                    # Validadores de domínio
│
├── 📁 RssTech.Employee.Infrastructure/ # Camada de Infraestrutura
│   ├── Context/                       # DbContext do Entity Framework
│   ├── Configurations/                # Configurações do EF
│   ├── Repositories/                  # Implementações de repositórios
│   └── Extensions/                    # Extensões de infraestrutura
│
├── 📁 RssTech.Employee.Ioc/          # Injeção de Dependência
│   └── DependencyInjectionConfig.cs  # Configuração de DI
│
├── 📁 RssTech.Employee.Common/       # Utilitários Compartilhados
│   └── Shared utilities and helpers
│
├── 📁 test/                          # Testes
│   ├── RssTech.Employee.Domain.UnitTests/
│   └── RssTech.Employee.Application.UnitTests/
│
└── 📁 deploy/                        # Deploy e Docker
    ├── docker-compose-infra.yml      # Infraestrutura (BD + Logs)
    ├── docker-compose-full.yml       # Aplicação completa
    └── Dockerfile                    # Imagem da aplicação
```

### **Padrões Implementados**
- **Clean Architecture** - Separação em camadas com dependências direcionadas para dentro
- **Domain-Driven Design (DDD)** - Modelagem rica do domínio
- **CQRS** - Separação de comandos e queries usando MediatR
- **Repository Pattern** - Abstração do acesso a dados
- **Value Objects** - Encapsulamento de conceitos do domínio
- **Entity Validation** - Validação no nível da entidade

### **Funcionalidades Principais**
- ✅ **Autenticação JWT** com roles hierárquicas
- ✅ **CRUD de Funcionários** com validações de domínio
- ✅ **Controle Hierárquico** (gerentes podem gerenciar subordinados)
- ✅ **Logs Centralizados** com Elasticsearch e Kibana
- ✅ **Validação Robusta** com FluentValidation
- ✅ **API RESTful** com Minimal APIs
- ✅ **Containerização** com Docker

## 🚀 Como Executar Localmente

### **Pré-requisitos**
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)
- [Git](https://git-scm.com/)

### **1. Clonar o Repositório**
```bash
git clone <repository-url>
cd RssTech.Employee.App
```

### **2. Executar com Docker (Recomendado)**

#### **Opção A: Aplicação Completa**
```bash
# Executa aplicação + banco + logs
docker-compose -f deploy/docker-compose-full.yml up --build
```

#### **Opção B: Apenas Infraestrutura**
```bash
# Executa apenas banco + elasticsearch + kibana
docker-compose -f deploy/docker-compose-infra.yml up -d

# Execute a aplicação localmente
dotnet run --project RssTech.Employee.Api
```

### **3. Executar Localmente (Desenvolvimento)**

#### **3.1. Subir Infraestrutura**
```bash
docker-compose -f deploy/docker-compose-infra.yml up -d
```

#### **3.2. Restaurar Dependências**
```bash
dotnet restore
```

#### **3.3. Executar Migrations**
```bash
dotnet ef database update --project RssTech.Employee.Api
```

#### **3.4. Executar a Aplicação**
```bash
dotnet run --project RssTech.Employee.Api
```

### **4. Executar Testes**
```bash
# Todos os testes
dotnet test

# Testes específicos
dotnet test RssTech.Employee.Domain.UnitTests
dotnet test RssTech.Employee.Application.UnitTests
```

### **5. Acessar Serviços**

| Serviço | URL | Descrição |
|---------|-----|-----------|
| **API** | http://localhost:5000 | API principal |
| **Swagger** | http://localhost:5000/swagger | Documentação da API |
| **Kibana** | http://localhost:5601 | Dashboard de logs |
| **Elasticsearch** | http://localhost:9200 | API do Elasticsearch |
| **PostgreSQL** | localhost:5432 | Banco de dados |

### **6. Configurações de Desenvolvimento**

#### **appsettings.Development.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=EmployeeDb;Username=postgres;Password=postgres",
    "Elasticsearch": "http://localhost:9200"
  },
  "Jwt": {
    "SecretKey": "sua_chave_secreta_desenvolvimento_com_pelo_menos_32_caracteres",
    "Issuer": "RssTech.Employee.Api",
    "Audience": "RssTech.Employee.Client",
    "ExpirationMinutes": 60
  }
}
```

### **7. Comandos Úteis**

```bash
# Parar todos os containers
docker-compose -f deploy/docker-compose-full.yml down

# Ver logs da aplicação
docker-compose -f deploy/docker-compose-full.yml logs -f app

# Rebuild completo
docker-compose -f deploy/docker-compose-full.yml up --build --force-recreate

# Limpar volumes (⚠️ remove dados)
docker-compose -f deploy/docker-compose-full.yml down -v
```

### **8. Estrutura de Dados**

#### **Usuário Padrão (Seed)**
- **Email**: admin@rsstech.com
- **Senha**: Admin123!
- **Role**: Manager

## 📊 Monitoramento e Logs

O sistema inclui logging centralizado com:
- **Logs estruturados** enviados para Elasticsearch
- **Dashboards** no Kibana para análise
- **Índices diários** no formato `employee-logs-{yyyy.MM.dd}`
- **Enrichers** para contexto adicional (máquina, processo, thread)

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

---

**Desenvolvido com ❤️ pela equipe RssTech**