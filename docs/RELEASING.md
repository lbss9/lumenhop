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

O mesmo número precisa aparecer nos três lugares:

1. `<Version>` em `src/Lumenhop/Lumenhop.csproj` — o app lê isso no Sobre
2. Tag `v1.0.0` na `main` — dispara o Release
3. `--packVersion` do Velopack — o instalado compara com o feed do GitHub

Se um deles divergir, o update não encontra a versão certa.

O repositório do feed é `https://github.com/lbss9/lumenhop` (público, para o app baixar o Release sem token). O app de release só usa essa URL compilada. `LUMENHOP_REPO_URL` vale só no CI / `scripts/pack.ps1`.

## Uma vez

```powershell
dotnet tool install -g vpk
```

## Cortar uma versão

1. Trabalhe na `develop`.
2. Atualize `src/Lumenhop/Assets/changelog/pt-BR.md` e `en.md`.
3. Suba `<Version>` em `src/Lumenhop/Lumenhop.csproj` (ex.: `1.0.1`).
4. Abra o PR da `develop` para a `main`.
5. Na `main`, crie a tag `v1.0.1`.

O workflow `.github/workflows/release.yml` publica o build, empacota com Velopack e sobe os arquivos no Release.

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
