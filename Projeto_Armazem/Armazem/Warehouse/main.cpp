#include "warehouse.h"
#include <iostream>
#include <stdlib.h>
#include <time.h>
#include <limits>
#include <algorithm>

int main() {
    srand(time(0));
    int numeros_serie_gerados = 0; ///Número de números de série já utilizados
    const int numeros_serie_disponiveis = 8999; ///Número de números de série ainda não utilizados
    int numeros_serie[numeros_serie_disponiveis];
    float total_faturacao = 0; ///Faturação total do armazém
    int numero_seccoes = aleatorio_numero_seccoes(); ///Geração de um número aleatório de secções
    seccao * seccoes = inicializacao_seccoes(numero_seccoes);
    string nova_categoria; ///Categoria adicionada pelo utilizador na gestão manual

    cout << "***********************************************************************************************" << endl;
    cout << "                            Armazem EDA | Total Faturacao: " << total_faturacao << " EUR" << endl;
    cout << "***********************************************************************************************" << endl;

    ///Inicialização das secções ainda sem peças
    for (int i = 0; i < numero_seccoes; i++) {
        cout << "Seccao: " << seccoes[i].ID
             << " | Categoria: " << seccoes[i].categoria
             << " | Capacidade: " << seccoes[i].capacidade
             << " | Quantidade: " << seccoes[i].quantidade
             << " | Faturacao: " << seccoes[i].faturacao << endl;
    }

    cout << "***********************************************************************************************" << endl;
    cout << "\n";
    cout << "*********************************************************" << endl;
    cout << "                   Lista de Chegada                      " << endl;
    cout << "*********************************************************" << endl;

    const int capacidade_maxima = 50; ///Capacidade máxima da lista de chegada
    int capacidade_espera = 5;
    peca * lista_chegada = new peca[capacidade_maxima];
    peca * lista_de_espera = new peca[capacidade_espera];
    int primeiro_livre = 0; ///Índice do primeiro espaço livre na lista de chegada
    int primeiro_livre_secundario = 0;
    int pecas_adicionar = 10; ///Peças a adicionar inicialmente na lista de chegada
    peca * pecas = inicializacao_pecas(pecas_adicionar, seccoes, numero_seccoes, numeros_serie, numeros_serie_gerados, numeros_serie_disponiveis);

    for (int i = 0; i < pecas_adicionar; i++) {
        inicializacao_lista_chegada(lista_chegada, capacidade_maxima, primeiro_livre, pecas[i], lista_de_espera, capacidade_espera, primeiro_livre_secundario);
    }

    ///Inicialização da lista de chegada
    for (int i = 0; i < primeiro_livre; i++) {
        cout << lista_chegada[i].marca << " | "
             << lista_chegada[i].categoria << " | "
             << lista_chegada[i].numero_serie << " | "
             << lista_chegada[i].preco << " EUR" << endl;
    }

    bool estado_gravacao = false;
    int dia = 0;
    int fim = 100;

    bool opcao_valida = false;
    while (dia < fim) {
        string opcao;

        ///Menu para o utilizador selecionar gerar um ciclo ou gerir o armazém manualmente
        cout << "*********************************************************\n" << endl;
        cout << "Dia (s)eguinte ******************* (g)estao" << endl;
        cout << "Selecione a sua opcao:" << endl;
        cin >> opcao;

        bool again = false;
        while (opcao != "s" && opcao != "g") {
            cout << "\nOpcao invalida. Tente de novo."<< endl;
            cout << "*********************************************************\n" << endl;
            cout << "Dia (s)eguinte ******************* (g)estao" << endl;
            cout << "Selecione a sua opcao: " << endl;
            cin >> opcao;
        }

        ///Se o utilizador escolher gerir o armazém manualmente
        if (opcao == "g") {
            while(!again) {
                int opccao;
                cout << "\n*******************************************" << endl;
                cout << "**********    Bem-vindo Gestor   **********" << endl;
                cout << "*******************************************" << endl;
                cout << "(1) Venda Manual" << endl;
                cout << "(2) Promocao" << endl;
                cout << "(3) Alterar Categoria" << endl;
                cout << "(4) Adicionar Seccao" << endl;
                cout << "(5) Gravar Armazem" << endl;
                cout << "(6) Carregar Armazem" << endl;
                cout << "(7) Imprimir Armazem" << endl;
                cout << "(8) Sair" << endl;
                cout << "\nSelecione a sua opcao:" << endl;
                cin >> opccao;
                switch (opccao) {

                    ///Venda Manual
                    case 1:
                        venda_manual(seccoes, numero_seccoes, lista_chegada, primeiro_livre, total_faturacao);
                        again=true;
                        break;

                        ///Promocao
                    case 2:
                        adicionar_promocao(seccoes, numero_seccoes, dia);
                        again=true;
                        break;

                        ///Alterar Categoria
                    case 3:
                        alterar_categoria(seccoes, nova_categoria, numero_seccoes);
                        again=true;
                        break;

                        ///Adicionar Seccao
                    case 4:
                        adicionar_seccao(seccoes, numero_seccoes);
                        again=true;
                        break;

                        ///Gravar Armazem
                    case 5:
                        gravar_estado_armazem(seccoes, lista_chegada, numero_seccoes, total_faturacao, dia, primeiro_livre,
                                              numeros_serie_gerados, capacidade_espera, estado_gravacao, primeiro_livre_secundario, lista_de_espera);
                        again=true;
                        break;

                        ///Carregar Armazem
                    case 6:

                        carregar_estado_armazem(seccoes, lista_chegada, numero_seccoes, total_faturacao, dia, primeiro_livre,
                                                numeros_serie_gerados, capacidade_espera, estado_gravacao, primeiro_livre_secundario, lista_de_espera);

                        again=true;
                        break;

                        ///Imprimir Armazem
                    case 7:

                        int escolha;

                        opcao_valida = false;
                        while (!opcao_valida) {
                            cout << "\nDeseja imprimir de que forma?" << endl;
                            cout << "  1 - Por ordem alfabetica da marca da peca." << endl;
                            cout << "  2 - Por preco." << endl;
                            cin >> escolha;

                            switch (escolha) {

                                ///Organiza as peças do armazém por ordem alfabética da marca da peça
                                case 1:
                                    armazem_alfabetica(seccoes, lista_chegada, primeiro_livre, numero_seccoes);
                                    opcao_valida = true;
                                    break;

                                    ///Organiza as peças do armazém por preço
                                case 2:
                                    armazem_preco(seccoes, lista_chegada, primeiro_livre, numero_seccoes);
                                    opcao_valida = true;
                                    break;

                                default:
                                    cout << "Opcao invalida. Tente de novo." << endl;
                            }
                        }
                        again=true;
                        break;

                        ///Sair da gestão manual
                    case 8:
                        again = true;
                        break;

                    default:
                        cout << "Opcao invalida. Tente de novo." << endl;
                        cin.clear();
                        cin.ignore();
                        break;
                }
            }
        }

            ///Se o utilizador escolher gerar um ciclo
        else if(opcao == "s") {
            cout << "\nDia -> " << dia + 1 << endl;

            aplicar_promocao(seccoes, numero_seccoes, dia);
            venda_pecas(seccoes, numero_seccoes, total_faturacao);
            int pecas_adicionar = 5;
            peca * pecas = inicializacao_pecas(pecas_adicionar, seccoes, numero_seccoes, numeros_serie, numeros_serie_gerados, numeros_serie_disponiveis);
            for (int i = 0; i < pecas_adicionar; i++) {
                inicializacao_lista_chegada(lista_chegada, capacidade_maxima, primeiro_livre, pecas[i], lista_de_espera, capacidade_espera, primeiro_livre_secundario);
            }
            transferencia_pecas(lista_chegada, primeiro_livre, seccoes, numero_seccoes);
            dia++;

            cout << "***********************************************************************************************" << endl;
            cout << "                            Armazem EDA | Total Faturacao: " << total_faturacao << " EUR" << endl;
            cout << "***********************************************************************************************" << endl;

            for (int i = 0; i < numero_seccoes; i++) {
                cout << "Seccao: " << seccoes[i].ID
                     << " | Categoria: " << seccoes[i].categoria
                     << " | Capacidade: " << seccoes[i].capacidade
                     << " | Quantidade: " << seccoes[i].quantidade
                     << " | Faturacao: " << seccoes[i].faturacao << endl;
                for (int j = 0; j < seccoes[i].quantidade; j++) {
                    cout << "        " << seccoes[i].pecas[j]->marca << " | "
                         << seccoes[i].pecas[j]->categoria << " | "
                         << seccoes[i].pecas[j]->numero_serie << " | "
                         << seccoes[i].pecas[j]->preco << " EUR" << endl;
                }
            }

            cout << "***********************************************************************************************" << endl;
            cout << "\n";
            cout << "*********************************************************" << endl;
            cout << "                   Lista de Chegada                      " << endl;
            cout << "*********************************************************" << endl;

            for (int i = 0; i < primeiro_livre; i++) {
                cout << lista_chegada[i].marca << " | "
                     << lista_chegada[i].categoria << " | "
                     << lista_chegada[i].numero_serie << " | "
                     << lista_chegada[i].preco << " EUR" << endl;
            }
        }
    }

    delete[] seccoes;
    delete[] pecas;
    delete[] lista_chegada;

    return 0;
}