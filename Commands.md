### CI/CD
```sh
docker build -t <user_name_>/<image_name_>:latest -f ./src/<ServiceName>/Dockerfile .
```

```sh
docker push <user_name_>/<image_name_>:latest
```

### Application Inspector

```sh
appinspector analyze -s ./ -o appinspector.html -g **/.git/** -l ./appinspector.log
```

```sh
appinspector analyze -s ./ -o appinspector.sarif -f sarif -g **/.git/** -l ./appinspector.log
```
```sh
appinspector analyze -s ./ -o appinspector.json -f json -g **/.git/** -l ./appinspector.log
```

### Code Coverage
```sh
dotnet test --collect:"XPLat Code Coverage;Format=json"

reportgenerator -reports:".\TestResults\5564f18e-5274-404b-ae77-c7b850c680b8\coverage.json" -targetdir:"coverageresults" -reporttypes:Html

dotnet stryker
```

### Create Migration
```sh	
dotnet ef migrations add <migration_name> --project ../ModularMonolith.Users.Infrastructure --startup-project . --output-dir Data/Migrations
```

```sh	
dotnet ef migrations add <migration_name> --output-dir Data/Migrations
```

```sh 
 dotnet ef migrations bundle --project ModularMonolith.Users.Infrastructure --startup-project  ModularMonolith.Web --output efbundle
```

```sh 
 dotnet ef migrations script --project ModularMonolith.Users.Infrastructure --startup-project  ModularMonolith.Web --output migrations.sql --idempotent

 ./efbundle --connection "${{secrets.CONNECTION_STRINGS_DATABASE}}"
```

### Format
```sh
dotnet format ./AppSolution.sln
```


### Testing
```sh
dotnet test --filter <ServiceName>~.UnitTests --no-build --verbosity normal
```



