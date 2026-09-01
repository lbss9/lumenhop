# Como gerar uma versão

O Lumenhop é um app WinUI 3 **unpackaged**. O instalador e o auto-update usam [Velopack](https://docs.velopack.io) a partir de **GitHub Releases**.

## Números (SemVer)

Um número só, em três partes: `MAJOR.MINOR.PATCH`.

| Parte | Quando sobe | Exemplo |
|---|---|---|
| **MAJOR** | Mudança que quebra o que o usuário já tem | `1.0.0` → `2.0.0` |
| **MINOR** | Função nova, sem quebrar o que existe | `1.0.0` → `1.1.0` |
| **PATCH** | Correção | `1.0.0` → `1.0.1` |

A primeira versão pública é **1.0.0**. Não use `0.x` daqui pra frente — isso sinaliza “ainda não é produto”. Também não use quatro números (`1.0.0.123`): o Windows preenche o quarto sozinho; a tag e o Velopack usam só três.

A `<Version>` em `src/Lumenhop/Lumenhop.csproj` é a fonte. Ao entrar na `main`, o workflow `Tag` cria `vX.Y.Z` se a tag ainda não existir. Essa tag dispara o Release (portable, MSI e o feed do Velopack).

Se a versão no csproj e a tag divergirem, o update não encontra o pacote certo.

O repositório do feed é `https://github.com/lbss9/lumenhop` (público, para o app baixar o Release sem token). O app de release só usa essa URL compilada. `LUMENHOP_REPO_URL` vale só no CI / `scripts/pack.ps1`.

## Uma vez

```powershell
dotnet tool install -g vpk
```

## Cortar uma versão

Tudo parte da `main`. Abra uma branch `feat/...` ou `fix/...`, depois PR para a `main`.

1. Atualize `src/Lumenhop/Assets/changelog/pt-BR.md` e `en.md`.
2. Suba `<Version>` em `src/Lumenhop/Lumenhop.csproj` (ex.: `1.0.1`).
3. Abra o PR para a `main` e faça o merge.

A tag `v1.0.1` e o Release saem sozinhos. Não crie a tag na mão.

## Empacotar na máquina

```powershell
pwsh scripts/pack.ps1
# ou
pwsh scripts/pack.ps1 -Version 1.0.1
```

Saída em `artifacts/release/`:

- `Lumenhop-win-Portable.zip` — sem instalar
- `Lumenhop-win-Setup.msi` — instalador Windows, atualiza sozinho
- `Lumenhop-win-Setup.exe` — instalador de um clique do Velopack
- pacote completo e delta da versão anterior, se houver

Para enviar ao GitHub:

```powershell
pwsh scripts/pack.ps1 -Version 1.0.1 -Upload
```

`GH_TOKEN` precisa de permissão para criar Releases.

## O que o usuário vê

Builds instalados pelo Setup avisam no app e em **Configurações → Atualizações**.  
`dotnet run` e o `.exe` da pasta `bin` não atualizam sozinhos — isso é esperado.
