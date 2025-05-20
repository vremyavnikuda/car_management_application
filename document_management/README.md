### Важно!!!

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
