# Deploy da Aplicação Employee

Este diretório contém os arquivos necessários para fazer o deploy da aplicação Employee usando Docker.

## Pré-requisitos

- Docker
- Docker Compose

## Como executar

1. Navegue até o diretório deploy:
```bash
cd deploy
```

2. Execute o docker-compose:
```bash
docker-compose up -d
```

3. Para acompanhar os logs:
```bash
docker-compose logs -f
```

4. Para parar os serviços:
```bash
docker-compose down
```

## Serviços

### PostgreSQL
- **Container**: employee-postgres
- **Porta**: 5432
- **Database**: EmployeeDb
- **Usuário**: postgres
- **Senha**: postgres

### Aplicação Employee API
- **Container**: employee-app
- **Porta**: 8080
- **URL**: http://localhost:8080

## Variáveis de Ambiente

As seguintes variáveis de ambiente são configuradas para a aplicação:

- `DATABASE_URL`: String de conexão com o PostgreSQL
- `Jwt__SecretKey`: Chave secreta para JWT
- `Jwt__Issuer`: Emissor do token JWT
- `Jwt__Audience`: Audiência do token JWT
- `Jwt__ExpirationMinutes`: Tempo de expiração em minutos

## Health Checks

Ambos os serviços possuem health checks configurados:
- PostgreSQL: Verifica se o banco está respondendo
- Aplicação: Verifica se a API está respondendo na porta 8080

## Volumes

- `postgres_data`: Persiste os dados do PostgreSQL

## Rede

Os serviços estão conectados através da rede `employee-network`.