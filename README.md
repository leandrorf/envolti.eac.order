# envolti.eac.order

# Eac
Architecture, Engineering and Construction

# Preparando ambiente

## Ambiente docker

### SqlSever
```
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=L3@ndr0rf" -p 1433:1433 --name sqlServer --hostname sqlServer -d mcr.microsoft.com/mssql/server:2025-latest
```

### RabbitMQ
```
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 --restart=always rabbitmq:3-management
```

### Redis
```
docker run -d --name redis --restart=always -p 6379:6379 redis
```

## Configurações

### Configuração do SqlServer

*Acessar o caminho:*
envolti.eac.order\src\adapters\driving\envolti.api.order.driving

Executar os comandos:
```
dotnet ef migrations add InitialCreate --project ..\..\driven\envolti.lib.data.sqlserver
dotnet ef database update --project ..\..\driven\envolti.lib.data.sqlserver
```

## Executar os projetos

* envolti.api.order.driving
* envolti.api.order.reading.driving

## Instruções de uso

### Exemplo de requisição POST
```
{
  "orderIdExternal": 1,
  "products": [
    {
      "productIdExternal": 1354,
      "name": "Produto 1",
      "price": 15.90
    },
    {
      "productIdExternal": 6599,
      "name": "Produto 2",
      "price": 102.99
    },
    {
      "productIdExternal": 5536,
      "name": "Produto 3",
      "price": 66.59
    }
  ]
}
```