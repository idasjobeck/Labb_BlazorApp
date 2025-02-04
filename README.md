Min inlämningsuppgift för kursen "Programmering C#" (del av .NET-utvecklare YH-utbildningen på IT-Högskolan Stockholm).

# Blazor Labb
* Uppgiften är att skriva en webbapp med Ramverket Blazor i .NET<br/>
* Instruktioner finns på nästa sida se G samt ytterligare instruktioner för VG<br/>
* Uppgiften utförs individuellt och lämnas in som .zip<br/>

## Instruktioner för G
* Projektet ska versionshanteras med git och ha minst 2 branscher, en main branch och en development branch, samt minst 3 commits<br/>
* Webbappen ska ha 3 sidor som man kan navigera till via en meny<br/>
  * Sida 1 - Home:<br/>
    * har valfritt innehåll<br/>
  * Sida 2 - New User:<br/>
    * har text-inputs där vi fyller i användar-data (samma typ av data som används för sida 3)<br/>
    * har en knapp som 'sparar användar data' (ingen data behöver dock sparas…)<br/>
      * när vi trycker på spara-knappen så döljs formuläret och istället visas det användaren fyllt i,<br/>
        * Exempelvis: "Du har sparat följande användare: Namn: ifyllt_namn, Email: ifylld_email" osv…<br/>
  * Sida 3 - Users:<br/>
    * Asynkron ladding av sidan ska simuleras med Task.Delay med visst antal millisekunder<br/>
      * En placeholder ska visas medan användardata laddas<br/>
      * När användar-data laddats ska detta visas upp i en lista eller tabell<br/>
        * sidan visar de första 5 användarna sorterade på förnamn (LINQ ska användas)<br/>
        * när man trycker på knappen "visa alla" ska alla användare visas (minst 10)<br/>
    * Sidan kräver ett datalager:<br/>
      * Datalagret ska ha metoden GetUsers() som returnerar användarna<br/>
      * Följande användardata ska representeras av minst 3 klasser<br/>
        * id, name, email, adress (bestående av street, city, zipcode), company (name & catchphrase)<br/>
        * Metoden ska generera minst 10 användare, där ordningen inte är sorterad<br/>
* Koden lättläslig med bra namngivning och följa naming-conventions<br/>

## Instruktioner för VG
* Datalagret, ska beskrivas i form av ett interface, som sedan implementeras<br/>
  * Datalagret ska nu bytas ut så att data även kan läsas från följande API<br/>
    * https://jsonplaceholder.typicode.com/users<br/>
    * Sida 3 (Users):<br/>
      * ska visa upp användardata som hämtats från API:et<br/>
      * ska kunna sortera användarna så att vi kan visa en sorterad lista av användare<br/>
        * sorteringen ska vara stigande/fallande på (minst) id samt förnamn<br/>
      * En sökruta ska nnas där vi kan göra en text sökning efter en specik användare<br/>
        * endast matchande användare visas<br/>
* GUI:t ska vara intuitivt och koden ska vara genomtänkt, lättläslig med bra namngivning och följa namingconventions<br/>
