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
dotnet ef migrations add <migration_name> --project ../<ServiceName>.Infrastructure --startup-project . --output-dir Data/Migrations
```

### Format
```sh
dotnet format ./AppSolution.sln
```


### Testing
```sh
dotnet test --filter <ServiceName>~.UnitTests --no-build --verbosity normal
```


### CSS BEM
```css
.form { }
.form--theme-xmas { }
.form--simple { }
.form__input { }
.form__submit { }
.form__submit--disabled { }
```


### AI Prompt Engineering
1. AuthorWithLowCyclomaticComplexity: Ensure that the authored code maintains a very low cyclomatic complexity.
2. AuthorWithLowCognitiveComplexity: Ensure that the authored code maintains a very low cognitive complexity. 
3. AuthorWithLowDefectDensity: Ensure that the authored code maintains a low defect density.
4. AuthorWithLowCodeDuplication: Ensure that the authored code has low code duplication.
5. SanitizeUserInputs: Ensure that all user inputs are sanitized before rendering. Use libraries like DOMPurify.
6. AvoidDirectDOMManipulation: Use Vue.js virtual DOM instead of direct DOM manipulation to maintain security.
7. UseVHTMLSparingly: If using v-html, ensure content is sanitized with DOMPurify to prevent XSS attacks.
8. NeverHardcodeSecrets: Avoid harcoding API keys, tokens or sensitive data.
9. UseSecureStorage: Do not store sensitive data in localStorage, sessionStorage, IndexDB or similar.
10. ValidateAllProps : Use TypeScript or PropTypes to validate all props. Only render Props in SAFE HTML attributes.
11. EscapeDynamicContent: Ensure dynamic content passed through props is properly escaped to prevent injection attacks and only render Props in SAFE HTML attributes.
12. ValidateURLs: Validate and sanitize all URLs before rendering.
13. UseSafeURLLoadingPractices: Check that URLs being loaded are from trusted sources and apply Content Security Policies (CSP).
14. UseHTTPS: Ensure all API calls are made over HTTPS to prevent man-in-the-middle attacks.
15. NoClientSideAccessControl: Ensure that access control is not handled in client-side Vue.js, it must be server-side only.
16. AuditThirdPartyLibraries: Regularly check and update third-party libraries to prevent security vulnerabilities in the latest version.
17. LimitInlineStyles: Avoid using inline styles or ensure untrusted CSS values are escaped with CSS.escape() to prevent injection.
18. UseStaticVueTemplates: Ensure that Vue.js templates are static and not dynamically created to avoid template injection.