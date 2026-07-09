# Digital Scale Project (Assembly)

Digital scale implementation in **assembly** for an educational processor, with a digital circuit in the **SEPE** simulator (Simulador Especial Para Ensino).

---

## Requirements

| Requirement | Required? | Notes |
|-------------|-----------|-------|
| **SEPE** | Yes | Digital circuit simulator (version 2.3.0 referenced in the project) |
| Assembly toolchain | Yes | Course assembler tied to the SEPE processor |

This project was developed in an academic context and depends on the SEPE software provided by the educational institution.

---

## Project files

| File | Description |
|------|-------------|
| `Codigo_Assembly.asm` | Scale assembly code (menus, weight, price, records) |
| `Processador.cir` | Processor circuit in SEPE format |
| `Memória.dat` | Memory data for the simulator |

---

## How to run

### 1. Load the circuit in SEPE

1. Open **SEPE**.
2. Load `Processador.cir`.
3. Load `Memória.dat` if the simulator requests it.

### 2. Run the assembly code

1. Open or import `Codigo_Assembly.asm` in the course assembly environment.
2. Assemble the code.
3. Run it in the simulator with the circuit loaded.

### 3. Simulated peripherals

The code uses I/O addresses for buttons, switches, and display (defined at the top of `Codigo_Assembly.asm`):

- Buttons: ON/OFF, OK, CHANGE, CANCEL
- Switches: menu selection and weight input
- Display: name, weight, price, and total output

---

## Features

- **Scale** mode — weighing and price calculation
- **Records** mode — view stored records
- **Clear** mode — clear records

---

## Notes

- There is no standalone compiler: everything runs inside the SEPE + course toolchain ecosystem.
- Without SEPE installed, the project cannot be run locally outside that environment.
