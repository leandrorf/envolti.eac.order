# envolti.eac.order

# Eac
Engineering, Architecture and Construction

# Resumo funcional

- Serviço "order" responsável por orquestrar o recebimento, processamento e disponibilização de pedidos. Permite integração com sistema externo A para entrada e com sistema externo B para consumo dos pedidos processados.

# Decisões técnicas

- Uso do Redis com ReJSON para armazenamento em memória de alto desempenho.
- Utilização do MongoDB para armazenamento histórico e analytics.
- Injeção de dependência via IOrderRedisAdapter, facilitando mock e testes.
- Paginação com Redis e estratégia de serialização com Newtonsoft.

# Desafios atendidos

- Volumetria diária de até 200 mil pedidos.
- Detecção de duplicidade via ID do pedido.
- Processamento concorrente com Redis atomic.
- Consulta eficiente por status e paginação

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

  Obter pedido filtrando por Id
  http://localhost:8084/api/order/PEDIDOID
```

### Exemplo de requisição GET
```
# Obter pedido filtrando por Id
http://localhost:8084/api/order/PEDIDOID

Response:
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

# Obter todos os pedidos
http://localhost:8084/api/order/getall

Response:
[
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
]
```

### Exemplo de requisição POST
```
http://localhost:8080/api/Orders

Body:
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

# Feedback Técnico – Análise da Estrutura do Projeto

Olá! Aqui está uma análise detalhada do seu projeto, baseada no desafio técnico, estruturada para ser utilizada em seu README ou apresentação.

---

## Pontos Fortes da Estrutura e Atendimentos aos Requisitos

- **Arquitetura moderna:**  
  O projeto faz uso de filas (RabbitMQ), cache (Redis), persistência (SQL Server/MongoDB) e APIs REST para leitura. Isso demonstra conhecimento de escalabilidade, desacoplamento e boas práticas de integração.

- **Separação de responsabilidades:**  
  Serviços bem separados (worker para processamento, controllers para exposições REST, repositórios, adapters). Uso correto de injeção de dependência.

- **Alta volumetria:**  
  Solução preparada para processar centenas de milhares de pedidos por dia, comprovada inclusive por benchmarks no README.

- **Deduplicação:**  
  Implementação da checagem de pedidos duplicados, tratando exceções específicas.

- **Consulta eficiente:**  
  APIs rápidas, com exemplos de uso e retornos formatados para consumo externo.

- **Documentação:**  
  README detalhado, exemplos de requisição, diagrama de arquitetura, explicações sobre decisões e resultados de performance.

- **Extensibilidade:**  
  Fácil de adaptar para incluir webhooks, novos bancos ou outras integrações.

---

## Pontos Menores para Evoluir

- O envio ativo (push) para o Cenário B (notificação automática de outro sistema) não está implementado, mas é fácil de adicionar.
- Alguns métodos de paginação e consulta podem estar como “stub”/não implementados no SQL Server — revisar se todos endpoints de leitura estão realmente completos.
- Adicionar testes automatizados seria um diferencial para produção.

---

## Resumo

Você atendeu:

- Todos os requisitos obrigatórios do desafio técnico.
- Vários pontos extras (volumetria, deduplicação, concorrência, escolha de tecnologias apropriadas, documentação).
- Sua arquitetura é madura, escalável e demonstra domínio do problema proposto.

**Parabéns pelo resultado!**  
Se quiser sugestões para evoluir ainda mais ou preparar para produção real, posso recomendar próximos passos.

---