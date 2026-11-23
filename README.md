## Modelagem do Banco de Dados (Diagrama ER)

A aplicação utiliza **SQLite** com **Entity Framework Core**, isolando todos os registros por `ProfissionalId` (multi-tenancy por profissional autenticada via JWT).

```mermaid
erDiagram
    PROFISSIONALS ||--o{ CLIENTES : "possui"
    PROFISSIONALS ||--o{ EXPEDIENTES : "define"
    PROFISSIONALS ||--o{ SERVICOS : "oferece"
    PROFISSIONALS ||--o{ AGENDAMENTOS : "realiza"
    CLIENTES ||--o{ AGENDAMENTOS : "participa"
    SERVICOS ||--o{ AGENDAMENTOS : "compõe"

    PROFISSIONALS {
        int Id PK
        string Nome
        string Login
        blob SenhaHash
        blob SenhaSalt
    }

    CLIENTES {
        int Id PK
        int ProfissionalId FK
        string Nome
        string Telefone
    }

    EXPEDIENTES {
        int Id PK
        int ProfissionalId FK
        int DiaSemana
        string HoraInicio
        string HoraFim
    }

    SERVICOS {
        int Id PK
        int ProfissionalId FK
        string Nome
        int DuracaoMinutos
        decimal Preco
    }

    AGENDAMENTOS {
        int Id PK
        int ProfissionalId FK
        int ClienteId FK
        int ServicoId FK
        datetime DataHoraInicio
        datetime DataHoraFim
        decimal PrecoCobrado
        int Status
    }
```

### Regras de Integridade e Histórico
* **Exclusão em Cascata (`Cascade`):** Ao remover uma profissional, seus clientes, expedientes e serviços vinculados são removidos automaticamente.
* **Exclusão Restrita (`Restrict`):** Clientes e serviços que possuem agendamentos vinculados não podem ser excluídos (`409 Conflict`), preservando a integridade do histórico.
* **Congelamento de Preço (`PrecoCobrado`):** O valor do serviço é copiado para o agendamento no momento da criação, garantindo que reajustes futuros na tabela de preços não alterem o histórico financeiro de atendimentos anteriores.
* **Índice Único de Expediente:** Cada profissional pode cadastrar apenas um expediente por dia da semana (`ProfissionalId + DiaSemana`).
