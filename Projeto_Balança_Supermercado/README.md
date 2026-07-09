# Projeto Balança Supermercado (FPGA)

Projeto de **balança de supermercado** implementado em **Verilog HDL** para FPGA **Artix-7** (`xc7a100t`), desenvolvido no **Xilinx ISE 14.7**.

**Código-fonte:** `Projeto_Balança/`

---

## O que precisa de instalar

| Requisito | Versão | Notas |
|-----------|--------|-------|
| **Xilinx ISE** | 14.7 | IDE para síntese, implementação e simulação |
| Toolchain Artix-7 | — | Incluído no ISE para a família Spartan/Artix |

> O ISE 14.7 é software legado da Xilinx/AMD. Requer licença académica ou de avaliação.

---

## Como abrir o projeto

1. Instale o **Xilinx ISE 14.7**.
2. Abra o ficheiro de projeto:

```
Projeto_Balança/Projeto_F1v2.xise
```

---

## Como executar

### Simulação comportamental

1. No ISE, defina o ficheiro de testbench como top de simulação, por exemplo:
   - `valores_esquema.v` — teste do esquema completo
   - `teste.v`, `testa_modulo_camara.v`, etc. — testes de módulos individuais
2. Execute **Simulate Behavioral Model**.
3. Analise as formas de onda no ISim.

### Síntese e implementação (FPGA)

1. Defina o top schematic: `esquema.sch`
2. Execute o fluxo **Synthesize → Implement → Generate Programming File**
3. Programe a FPGA com o ficheiro `.bit` gerado

---

## Módulos principais

| Módulo | Descrição |
|--------|-----------|
| `esquema.sch` | Top-level schematic |
| `somador.v` / `somador_preco.v` | Somadores de preço e peso |
| `multiplicador_11_9_bin_11.v` | Multiplicador |
| `separador_BCD_8_8.v` / `separador_BCD_4_12.v` | Conversão BCD para display |
| `valores_esquema.v` | Testbench do sistema completo |

---

## Estrutura do projeto

```
Projeto_Balança_Supermercado/
└── Projeto_Balança/
    ├── Projeto_F1v2.xise    ← abrir no ISE
    ├── esquema.sch
    ├── *.v                  ← módulos Verilog
    └── *_beh.prj            ← projetos de simulação
```

---

## Notas

- Este projeto **não corre em linha de comandos** — requer o Xilinx ISE.
- Os ficheiros `.xst`, `.prj`, `.cmd_log` são artefactos gerados pelo ISE durante síntese/simulação.
