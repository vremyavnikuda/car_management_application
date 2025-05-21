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

```sh
dotnet ef migrations add InitialCreate ––project document_management.csproj
```

### Обновляем(применяем) все отложенные изменения миграции базы данных

```sh
dotnet ef database update ––project document_management.csproj
```
