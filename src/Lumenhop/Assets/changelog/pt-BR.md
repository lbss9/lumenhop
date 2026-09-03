# 1.3.0

3 de setembro de 2026 · ligar e desligar todos de uma vez

- Botões **Iniciar todos** e **Parar todos** na tela inicial, ao lado de Adicionar — liga ou desliga todos os destinos num toque só.

---

# 1.2.1

3 de setembro de 2026 · importar/exportar nas configurações

- Importar e exportar agora ficam nas **Configurações**, ao lado de Sair, em vez do menu da tela inicial.

---

# 1.2.0

3 de setembro de 2026 · compartilhe seus destinos

- **Importar e exportar** sua lista de destinos, pelo menu `⋯` na tela inicial. A exportação gera um arquivo `.lumenhop` assinado — título, host, ícone e intervalo — que qualquer pessoa pode importar. A importação confere a assinatura e o checksum do arquivo, pula hosts que você já tem e nunca traz dados locais como imagens de ícone.

---

# 1.1.2

3 de setembro de 2026 · latência sem piscar

- O número da latência **não pisca mais a cada verificação**. Antes ele piscava para `…` entre as leituras; agora atualiza no lugar, e o "…" aparece só na primeira leitura. A bolinha também parou de piscar a cada ciclo.

---

# 1.1.1

3 de setembro de 2026 · a bolinha combina com o número

- A bolinha de status agora fica com a **mesma cor do número da latência**. Ela estava presa no cinza enquanto o número já vinha colorido pela faixa.

---

# 1.1.0

3 de setembro de 2026 · as cores são suas

- **Cores por latência configuráveis** em Configurações. Quatro faixas — ótimo, bom, regular e ruim — cada uma com um limite editável e seu próprio seletor de cor. A bolinha e o número da latência assumem a cor da faixa. Dá para restaurar o padrão quando quiser.

---

# 1.0.2

3 de setembro de 2026 · aviso de atualização que te encontra

- Agora o app **verifica atualizações também enquanto está aberto**, não só na abertura — uma checagem discreta a cada 6 horas.
- Quando sai uma versão nova, aparece uma **notificação na bandeja** — abra o Lumenhop para instalar. A janela de update não abre mais sozinha na sua frente.

---

# 1.0.1

3 de setembro de 2026 · acabamento e estabilidade

- A navegação lateral virou uma **barra de ícones fixa** — sem o botão de abrir e sem a animação de recolher que dava aquele tranco.
- Registro de falhas mais robusto: gravar o `crash.log` nunca mais mascara o erro original.

---

# 1.0.0

31 de agosto de 2026 · primeira versão pública

Monitor de ping para Windows. Flyout no canto da tela, cards vivos, ping contínuo.

Instalador: `Lumenhop-win.msi`.

## Destinos

- Card com **ícone**, **título**, **IP ou host** e **latência**
- Bolinha com pulso: `ciano` online · `âmbar` lento (≥ 200 ms) · `vermelho` offline · `cinza` desligado
- Toque na bolinha para ligar ou desligar
- Adicionar, editar e remover
- Intervalo próprio por destino, de 1 a 60 segundos
- Ícones Fluent ou uma imagem sua
- Aceita URL, host ou IP — o app normaliza sozinho
- Na primeira abertura: Cloudflare (`1.1.1.1`) e Google DNS (`8.8.8.8`)

## Janela

- Flyout WinUI 3, 400 × 600, sem maximizar
- Quatro cantos nas Configurações
- Fechar ou minimizar recolhe para a bandeja — o ping continua
- Clique simples ou duplo no ícone da bandeja abre o app; botão direito tem Abrir / Fechar
- Tema claro, escuro ou do sistema
- Acrílico e opacidade ajustáveis
- Idioma do sistema, Português (Brasil) ou English
- Abrir junto com o Windows

## Atualizações

- Aviso no app quando sai uma versão nova
- Changelog no idioma atual
- Instalar agora ou depois, em Configurações

Preferências em `%LOCALAPPDATA%\Lumenhop`.
