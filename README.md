# envolti.eac.order

# Eac
Engineering, Architecture and Construction

# Sobre o ambiente ambiente

O ambiente utiliza recursos externos como ferramentas para auxiliar no proprosito da aplicação sendo eles:

- RabbitMQ
- Redis
- Grafana
- Loki
- Promtail
- MongoDb

## Ambiente docker

### Docker-compose
```
docker-compose up -d
```

> [!NOTE]
> A estrutura do banco de dados executa automaticamente a [Migration].

## Executar os projetos

* envolti.api.order.reading.driving
* envolti.api.order.written.driving
* envolti.service.order.written.driving

## Instruções de uso

### Exemplo de requisição GET
```
  Obter todos os pedidos
  http://localhost:8084/api/order/getall

  Obter pedidos filtrando por status
  http://localhost:8084/api/order/GetByStatus

  Obter pedido filtrando por Id
  http://localhost:8084/api/order/PEDIDOID
```

### Exemplo de requisição GET
```
{
    "orderIdExternal": 7000000,
    "totalPrice": 185.48,
    "createdAt": "2025-06-18T19:51:47.4055155+00:00",
    "processedIn": "2025-06-18T19:51:47.4062668+00:00",
    "status": 1,
    "products": [
        {
            "productIdExternal": 1354,
            "name": "Produto 1",
            "price": 15.9
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

### Tempo de requisição POST

- Tempo total da requisição: 1 ms

### Tempo de requisição GET
- Tempo total da requisições por status dos pedidos: 3 ms
- Tempo total da requisições por todos os pedidos: 3 ms
- Tempo total da requisições do pedido por Id: 3 ms

### Exemplo de performance

Rodar 200 mil requisições por dia (ou ~2,3 requisições por segundo em média)

- Tempo total de requisições: 2.087 segundos (34 × 60 + 47 = 2.087 segundos)
- Tempo médio de requisição: 10,4 ms (2087 segundos ÷ 200.000 requisições ≈ 0,0104 segundos por requisição)

![Sou uma imagem](Tests.png)

### Tempo de processamento de pedidos

- Primeiro, calculamos o tempo total entre 15:11:54 e 17:04:27:
- Isso dá 1 hora, 52 minutos e 33 segundos.
- Convertendo tudo para segundos:
1 hora = 3600 s
52 minutos = 3120 s
33 segundos = 33 s
Total = 3600 + 3120 + 33 = 6753 segundos
- Agora dividimos o tempo total pelo número de execuções:
( 6753 ÷ 200.000 = 0,033765 ) segundos por execução
Ou seja, cada execução levou em média aproximadamente 0,0338 segundos, ou cerca de 33,8 milissegundos.

![Processamento de pedidos](processamento-pedidos.png)

### Desenho da Arquitetura

![Sou uma imagem](Arquitetura.jpg)
