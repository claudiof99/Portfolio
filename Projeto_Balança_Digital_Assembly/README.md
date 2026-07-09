# Projeto Balança Digital (Assembly)

Implementação de uma balança digital em **assembly** para um processador educacional, com circuito digital no simulador **SEPE** (Simulador Especial Para Ensino).

---

## O que precisa de ter instalado

| Requisito | Obrigatório? | Notas |
|-----------|--------------|-------|
| **SEPE** | Sim | Simulador de circuitos digitais (versão 2.3.0 referenciada no projeto) |
| Toolchain de assembly | Sim | Montador/assembler do curso associado ao processador do SEPE |

> Este projeto foi desenvolvido no âmbito académico e depende do software SEPE disponibilizado pela instituição de ensino.

---

## Ficheiros do projeto

| Ficheiro | Descrição |
|----------|-----------|
| `Codigo_Assembly.asm` | Código assembly da balança (menus, peso, preço, registos) |
| `Processador.cir` | Circuito do processador no formato SEPE |
| `Memória.dat` | Dados de memória para o simulador |

---

## Como executar

### 1. Carregar o circuito no SEPE

1. Abra o **SEPE**.
2. Carregue o ficheiro `Processador.cir`.
3. Carregue `Memória.dat` se o simulador o solicitar.

### 2. Executar o código assembly

1. Abra ou importe `Codigo_Assembly.asm` no ambiente de assembly do curso.
2. Monte (assemble) o código.
3. Execute no simulador com o circuito carregado.

### 3. Periféricos simulados

O código usa endereços de E/S para botões, switches e display (definidos no início de `Codigo_Assembly.asm`):

- Botões: ON/OFF, OK, CHANGE, CANCEL
- Switches: seleção de menu e introdução de peso
- Display: saída de nome, peso, preço e total

---

## Funcionalidades

- Modo **Balança** — pesagem e cálculo de preço
- Modo **Registos** — visualização de registos
- Modo **Limpar** — limpeza de registos

---

## Notas

- Não existe compilador standalone: tudo corre dentro do ecossistema SEPE + toolchain do curso.
- Se o SEPE não estiver instalado, o projeto não pode ser executado localmente fora desse ambiente.
