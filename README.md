Dokumentasjon

# ASP.NET Core MVC med Docker

## Prosjektbeskrivelse
Dette prosjektet er en ASP.NET Core MVC-applikasjon. 
Applikasjonen demonstrerer:
- Controller, ViewModel og View
- Responsive nettsider med dynamisk innhold
- Håndtering av GET og POST forespørsler
- Skjema hvor brukerdata sendes inn og vises på en resultatside
- Kart hvor bruker kan velge posisjon, som vises på en resultatside
- Kjøring i Docker-container


## Systemarkitektur
Applikasjonen følger MVC-mønsteret:

1. **Controller** håndterer forespørsler fra bruker.
2. **ViewModel** bærer data mellom Controller og View.
3. **View** viser dataene for brukeren i et responsivt grensesnitt.
4. **Docker** brukes til å pakke og kjøre applikasjonen.


### Krav
- [Docker](https://www.docker.com/) installert
- (Valgfritt) [.NET 6/7 SDK](https://dotnet.microsoft.com/en-us/download)

### Bygg og kjør i Docker
```bash
# Bygg image
docker build -t mvc-app .

# Start container
docker run -p 8080:80 mvc-app
