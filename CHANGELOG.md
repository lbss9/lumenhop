# Changelog

O que muda em cada versão. No app, o mesmo texto aparece no idioma atual:

- [Português](src/Lumenhop/Assets/changelog/pt-BR.md)
- [English](src/Lumenhop/Assets/changelog/en.md)

---

## 1.1.0 — 31 de agosto de 2026

Portable e instalador MSI nos Releases.

- `Lumenhop-win-Portable.zip` — extrai e roda, sem instalar
- `Lumenhop-win-Setup.msi` — instalador do Windows, com atualização automática
- O `Setup.exe` do Velopack continua no pacote, para quem preferir um clique

## 1.0.0 — 31 de agosto de 2026

Primeira versão pública.

Monitor de ping para Windows. Flyout no canto da tela, cards vivos, ping contínuo.

### Destinos

- Card com **ícone**, **título**, **IP ou host** e **latência**
- Bolinha com pulso: `ciano` online · `âmbar` lento (≥ 200 ms) · `vermelho` offline · `cinza` desligado
- Toque na bolinha para ligar ou desligar
- Adicionar, editar e remover
- Intervalo próprio por destino, de 1 a 60 segundos
- Ícones Fluent ou uma imagem sua
- Aceita URL, host ou IP — o app normaliza sozinho
- Na primeira abertura: Cloudflare (`1.1.1.1`) e Google DNS (`8.8.8.8`)

### Janela

- Flyout WinUI 3, 400 × 600, sem maximizar
- Quatro cantos nas Configurações
- Fechar ou minimizar recolhe para a bandeja — o ping continua
- Tema claro, escuro ou do sistema
- Acrílico e opacidade ajustáveis
- Idioma do sistema, Português (Brasil) ou English
- Abrir junto com o Windows

### Atualizações

- Aviso no app quando sai uma versão nova
- Changelog no idioma atual
- Instalar agora ou depois, em Configurações

Preferências em `%LOCALAPPDATA%\Lumenhop`.
