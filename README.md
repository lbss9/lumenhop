<p align="center">
  <img src="src/Lumenhop/Assets/Lumenhop.png" width="128" alt="Lumenhop" />
</p>

<h1 align="center">Lumenhop</h1>

<p align="center">
  Monitor de ping para Windows.
</p>

<p align="center">
  <a href="https://github.com/lbss9/lumenhop/releases"><img src="https://img.shields.io/badge/versão-1.0.0-2EE6C7?style=flat-square" alt="1.0.0" /></a>
  <img src="https://img.shields.io/badge/Windows-10%20%2F%2011-0078D4?style=flat-square&logo=windows&logoColor=white" alt="Windows" />
  <img src="https://img.shields.io/badge/WinUI-3-59C8C8?style=flat-square" alt="WinUI 3" />
  <img src="https://img.shields.io/badge/.NET-8-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt=".NET 8" />
  <img src="https://img.shields.io/badge/idioma-pt--BR%20·%20en-2EE6C7?style=flat-square" alt="pt-BR e English" />
</p>

<p align="center">
  Flyout nativo no canto da tela. Cards vivos. Ping contínuo. Sem ruído.
</p>

---

## O que é

O Lumenhop é um monitor de ping só para Windows. Não é um dashboard. Não é um clone de nada. É um vidro acrílico noturno, com acento aurora ciano, que fica no canto que você escolher e mostra se o que importa está no ar.

Cada destino vira um card:

| Ícone | Destino | Pulso |
| :---: | :--- | :---: |
| o seu | título em cima<br />IP ou host embaixo, em mono | bolinha animada<br />`15ms` · `off` |

Toque na bolinha para ligar ou desligar. O menu do card edita ou remove.

<p align="center">
  <code>ciano</code> online &nbsp;·&nbsp; <code>âmbar</code> lento ≥ 200&nbsp;ms &nbsp;·&nbsp; <code>vermelho</code> offline &nbsp;·&nbsp; <code>cinza</code> desligado
</p>

Fechar ou minimizar recolhe para a bandeja. O ping continua.

---

## O que ele faz

| | |
| :--- | :--- |
| **Ping contínuo** | ICMP em loop, com intervalo próprio por destino (1–60 s). |
| **O canto que você escolhe** | Quatro quadrantes nas Configurações. A janela nasce onde você marcar. |
| **Bandeja silenciosa** | Some da tela. Continua medindo em segundo plano. |
| **A sua cara** | Ícones Fluent ou uma imagem sua. Título, host e intervalo editáveis. |
| **Dois idiomas** | Segue o sistema, ou trava em Português (Brasil) / English. |
| **Atualiza sozinho** | Quem instala pelo Setup recebe o aviso, lê o changelog e escolhe instalar. |

Na primeira abertura já vêm Cloudflare (`1.1.1.1`) e Google DNS (`8.8.8.8`).  
Tudo fica em `%LOCALAPPDATA%\Lumenhop`.

---

## Instalar

O instalador oficial sai nos [Releases](https://github.com/lbss9/lumenhop/releases) — é o `Lumenhop-win-Setup.exe`. Só ele habilita o auto-update.

`dotnet run` e o `.exe` da pasta `bin` são para desenvolvimento. Não atualizam sozinhos. Isso é esperado.

---

## Desenvolver

```powershell
dotnet test Lumenhop.sln
dotnet build src/Lumenhop/Lumenhop.csproj -c Debug
```

```
src/Lumenhop/bin/Debug/net8.0-windows10.0.19041.0/win-x64/Lumenhop.exe
```

Windows 10 1809+ · Windows 11 recomendado · .NET 8 SDK

Empacotar uma versão:

```powershell
pwsh scripts/pack.ps1
```

Guia completo: [docs/RELEASING.md](docs/RELEASING.md).

| Branch | Papel |
| :--- | :--- |
| `develop` | trabalho do dia |
| `main` | estável |
| tag `vX.Y.Z` | dispara o Setup |

---

<p align="center">
  <sub>Windows · WinUI 3 · pt-BR e English</sub>
</p>
