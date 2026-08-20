# PDV C# — Supermarket

Ponto de venda desktop em WPF. O banco vai junto no arquivo `pdv.db` (SQLite). Não precisa de MySQL, Docker nem Visual Studio para usar o programa pronto.

## Usar o aplicativo (pasta `distri`)

```powershell
cd C:\PDV-CSharp
.\publicar.cmd
```

Depois abra `distri\PDVCSharp.WPF.exe` ou rode `distri\Instalar.cmd` para criar atalho na Área de Trabalho.

Login: `caixa` / `caixa` ou `admin` / `admin`.

## Desenvolver

```powershell
dotnet run --project PDVCSharp.WPF
```

## Estrutura

- `PDVCSharp.Domain` — entidades
- `PDVCSharp.Data` — SQLite/MySQL + repositórios
- `PDVCSharp.Application` — regras de negócio
- `PDVCSharp.WPF` — telas do caixa
- `PDVCSharp.Tests` — testes
- `distri` — pasta de distribuição do executável
