# F1WeatherStatus

App .NET MAUI (.NET 9) com navegação por **menu lateral**, **abas** e **pilha**, consumindo APIs públicas e organizado com POO.

## Como as APIs se integram

| De onde vem | O que passa | Para onde vai |
|---|---|---|
| IP (ip-api.io / ipwho.is) | região e coordenadas | **IBGE** (acha o seu estado) e **Open-Meteo** (clima de agora) |
| IBGE (estado / município) | nome do lugar | **Geocoding** (coordenadas) → **Open-Meteo** (clima da capital ou do município) |
| OpenF1 (corrida) | cidade do circuito e data | **Geocoding** → **Open-Meteo** (clima no dia da corrida) |
| OpenF1 + IP | circuito e sua posição | distância em km entre você e o circuito |
| Aba Clima | nome digitado | **Geocoding** → **Open-Meteo** |

## Telas

- **Menu lateral:** Tempo e Local, Estados (IBGE) e Fórmula 1.
- **Abas:** Localização e Clima (dentro de Tempo e Local).
- **Pilha:** Localização → Estado; Estados → Estado → município; Corridas → Detalhe da corrida.
- Tema sempre claro, com fundo branco.

## POO aplicada

- **Herança / classe abstrata:** `Services/ApiServiceBase.cs` (HTTP, JSON e erros) e `Views/BasePage.cs` (carregamento, erros e navegação).
- **Interfaces:** `Services/Interfaces.cs` (`ILocalizacaoService`, `IClimaService`, `IIbgeService`, `IFormula1Service`).
- **Polimorfismo:** `IpApiIoService` e `IpWhoIsService` implementam `ILocalizacaoService`; o `GerenciadorLocalizacao` tenta cada uma sem conhecer a classe concreta.
- **Encapsulamento:** classes `Dto` espelham o JSON das APIs; os `Models` são o que as telas usam.

## Como abrir

1. Visual Studio 2022 (17.12 ou mais novo) com a carga de trabalho **Desenvolvimento de interface do usuário de aplicativo multiplataforma do .NET**.
2. Abra `F1WeatherStatus.sln`.
3. Escolha **Windows Machine** e pressione F5.

Para usar a ip-api.io, coloque a chave em `ChaveApi` no arquivo `Services/IpApiIoService.cs`. Sem chave, o app usa a ipwho.is automaticamente.
