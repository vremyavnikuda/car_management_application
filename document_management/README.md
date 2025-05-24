### Важно!!!

[TECHNICAL_DEBT(ENG)](TECHNICAL_DEBT.md)
[TECHNICAL_DEBT(RU)](ТЕХ_ДОЛГ.md)

не забудьте ,что нужно создать `appsettings.json`, где необходимо указать:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host={Host};Database={name_db};Username={user};Password={pass_db}"
  },
  // Опционально
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Выполняем миграцию базы данных

Выполнять миграцию перед запуском app не нужно , так как она применяется `автоматически` + перед этим создается зеркало 
для восставновления 
