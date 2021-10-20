# RabbitMqNetCore
***Projeto .NET Core*** - Enviando e recebendo mensagens do RabbitMQ

O objetivo do projeto é consumir os serviços de envio e recebimento usando .NET Core 5.0.
Foi aplicado nesse projeto o uso do ***try..catch***, ***Parallel.ForEach*** e do ***Docker*** com uma instância do RabbitMQ.
Além disso, usamos a biblioteca do [Bogus](https://github.com/bchavez/Bogus/wiki/Bogus-Premium) para gerar dados fake.



Foi utilizado o Docker Desktop para Windows para rodar uma instância do RabbitMQ.
```sh
docker run -d --hostname rabbitserver --name rabbitmq-server -p 15672:15672 -p 5672:5672 rabbitmq:3-management
```

See the [open issues](https://github.com/github_username/repo_name/issues) for a full list of proposed features (and known issues).
