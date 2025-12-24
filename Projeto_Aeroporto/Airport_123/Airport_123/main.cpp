//Grupo 23: João Calheta 2103423, Filipe Gonçalves 2141023, Maryna Mramornova 2163623, Cláudio Fernandes 2186622

#include <iostream>
#include "airport.h"
#include <fstream>
#include <string>
#include <sstream>
#include <stdlib.h>
#include <stdio.h>
#include <time.h>
#include <limits>
#include <algorithm>
#include <cstring>


using namespace std;

int main(int argc, char* argv[]) {
    srand(time(0));

    if (argc != 4) { // Verifica se o número correto de argumentos foi passado
        cerr << "Uso: " << argv[0] << " <arquivo_proximacao> <arquivo_pista> <arquivo_descolagem>" << endl;
        return 1;
    }
    // Obtém os nomes dos arquivos a partir dos argumentos da linha de comando
    string arquivo_proximacao = argv[1];
    string arquivo_pista = argv[2];
    string arquivo_descolagem = argv[3];

    // Declaração de variáveis e inicialização de ponteiros
    long long numero_bilhetes_gerados = 0;
    int numero_avioes_aeroporto = 0;
    nodo_arvore_binaria* raiz = nullptr;
    nodo_arvore_binaria** raizes = nullptr;
    nodo_lista_ligada* head = nullptr;

    // Inicialização da estrutura do aeroporto
    aeroporto aeroporto;
    inicializacao_aeroporto(aeroporto);

    // Declaração de variáveis para contagem de elementos e arrays para armazenar dados
    int numero_avioes_aproximacao = 0;
    int numero_avioes_pista = 0;
    int numero_avioes_descolando = 0;
    int permissao_remocao = 1;

    // Contagem do número de linhas em arquivos de dados e carregamento dos dados em arrays
    int numero_primeiros_nomes = contador_linhas("primeiro_nome.txt");
    string* array_primeiros_nomes = new string[numero_primeiros_nomes];
    carregar_dados("primeiro_nome.txt", array_primeiros_nomes, numero_primeiros_nomes);

    int numero_segundos_nomes = contador_linhas("segundo_nome.txt");
    string* array_segundos_nomes = new string[numero_segundos_nomes];
    carregar_dados("segundo_nome.txt", array_segundos_nomes, numero_segundos_nomes);

    int numero_nacionalidades = contador_linhas("nacionalidade.txt");
    string* array_nacionalidades = new string[numero_nacionalidades];
    carregar_dados("nacionalidade.txt", array_nacionalidades, numero_nacionalidades);

    int numero_voos = contador_linhas("voo.txt");
    string* array_voos = new string[numero_voos];
    carregar_dados("voo.txt", array_voos, numero_voos);

    int numero_modelos = contador_linhas("modelo.txt");
    string* array_modelos = new string[numero_modelos];
    carregar_dados("modelo.txt", array_modelos, numero_modelos);

    int numero_origens = contador_linhas("origem.txt");
    string* array_origens = new string[numero_origens];
    carregar_dados("origem.txt", array_origens, numero_origens);

    int numero_destinos = contador_linhas("destino.txt");
    string* array_destinos = new string[numero_destinos];
    carregar_dados("destino.txt", array_destinos, numero_destinos);

    // Criação de lista ligada para nacionalidades estrangeiras e árvores de nacionalidade
    nodo_lista_ligada * nacionalidades_estrangeiras = lista_ligada_nacionalidades_estrangeiras(array_nacionalidades,
                                                                                             numero_nacionalidades);
    nodo_arvore_binaria** arvores_nacionalidade = criar_arvores_nacionalidade(nacionalidades_estrangeiras, numero_nacionalidades);

    // Definição dos ciclos e limite de simulação
    int dia = 0;
    int fim = 365;

    // Declaração de variáveis para contagem de elementos e arrays para armazenar dados para a função que fecha o aeroporto
    bool aeroporto_fechado = false;
    int dias_fechadas = 0;
    int ciclos = 0;


    simulacao_ciclo(dia, aeroporto, array_modelos, array_voos, array_origens, array_destinos, array_primeiros_nomes,
                    array_segundos_nomes, array_nacionalidades, numero_avioes_aproximacao,permissao_remocao,
                    numero_avioes_pista, numero_avioes_descolando, numero_bilhetes_gerados,raiz,
                    numero_avioes_aeroporto, numero_voos, arvores_nacionalidade, numero_nacionalidades, aeroporto_fechado);



    while (dia < fim ) {
        string opcao_menu;
        cout << "(e)mergencias (o)pcoes (g)ravar\n"; // Solicitação de opções ao usuário

        // Impressão do estado atual do aeroporto
        estado_aeroporto(aeroporto, numero_avioes_aeroporto);
        cout << "Selecione a sua opcao:" << endl;
        cin >> opcao_menu;

        // Validação da opção escolhida pelo usuário
        bool again = false;
        while (opcao_menu != "e" && opcao_menu != "o" && opcao_menu != "g" && opcao_menu != "s") {
            cout << "\nOpcao invalida. Tente de novo." << endl;
            cout << "*********************************\n" << endl;
            cout << "(e)mergencias (o)pcoes (g)ravar" << endl;
            cout << "Selecione a sua opcao:" << endl;
            cin >> opcao_menu;
        }

        if (opcao_menu == "e") {
            cout << "Selecionou a opcao de emergencia." << endl;
            string voo_de_emergencia;
            string voo_para_descolar;
            cout << "Numero de voo de emergencia: " << endl;
            cin >> voo_de_emergencia;
            if (numero_avioes_pista == 7) {
                cout << "Numero de voo para descolar: " << endl;
                cin >> voo_para_descolar;
            }
            emergencia(aeroporto, voo_de_emergencia, voo_para_descolar, array_modelos, array_voos, array_origens, array_primeiros_nomes,
                       array_segundos_nomes, array_nacionalidades, numero_avioes_aproximacao,
                       numero_bilhetes_gerados,raiz,
                       numero_avioes_aeroporto, numero_voos, numero_avioes_pista, array_destinos, numero_avioes_descolando, permissao_remocao);
        }

        else if (opcao_menu == "o") {
            while (!again) {
                int opcao_valida;
                cout << "\n*********************************************" << endl;
                cout << "**********    OPCOES DISPONIVEIS   **********" << endl;
                cout << "*********************************************" << endl;
                cout << "(1) Mostrar os passageiros em pista " << endl;
                cout << "(2) Mostrar todos os passageiros em pista por:" << endl;
                cout << "(3) Pesquisa sobre os passageiros nas chegadas e partidas (pelo primeiro nome):" << endl;
                cout << "(4) Editar a nacionalidade de passageiro num voo das chegadas" << endl;
                cout << "(5) Inverter prioridade dos avioes na pista"<< endl;
                cout << "(6) Carregar dados guardados"<< endl;
                cout << "(7) Fechar o aeroporto" << endl;
                cout << "(8) Sair" << endl;
                cout << "\nSelecione a sua opcao:" << endl;
                cin >> opcao_valida;
                switch (opcao_valida) {
                    case 1: {
                        mostrar_passageiros_na_pista(aeroporto);  // nova
                        again = true;
                        break;
                    }
                    case 2: {
                        cout <<"Deseja visualizar os passageiros em pista por:"<< endl;
                        cout <<"(1) Nacionalidade, ordenados por ordem alfabetica"<< endl;
                        cout <<"(2) Nacionalidade, utilizando uma representacao visual para uma arvore de pesquisa binaria"<< endl;
                        int opcao;
                        cin>>opcao;
                        switch(opcao){
                            case 1:{
                                cout << "________________________________" << endl;
                                mostrar_passageiros_na_pista_ordenados_alfabetica(aeroporto, arvores_nacionalidade, numero_nacionalidades); //nova
                                again=true;
                                break;


                            }
                            case 2:{
                                cout << "________________________________" << endl;
                                mostrar_passageiros_na_pista_arvore(aeroporto, arvores_nacionalidade,numero_nacionalidades); //nova
                                again=true;
                                break;


                            }
                        }
                        break;
                    }
                    case 3: {
                        cout << "(1)Pesquisa sobre os passageiros nas chegadas"<< endl;
                        cout << "(2)Pesquisa sobre os passageiros nas partidas"<< endl;
                        int escolha;
                        cin>>escolha;
                        switch(escolha){
                            case 1:{
                                mostrar_passageiros_na_aproximacao(aeroporto); // nova
                                cout << "Escolheu a pesquisa sobre os passageiros nas chegadas"<< endl;

                                cout << "Diga qual o primeiro nome que quer pesquisar"<< endl;
                                string primeiro_nome;
                                cin>>primeiro_nome;
                                pesquisar_passageiro_por_nome_chegadas(aeroporto,primeiro_nome);
                                break;
                            }
                            case 2:{
                                    if (aeroporto.avioes_descolando != nullptr) {  // nova
                                        mostrar_passageiros_na_descolagem(aeroporto);
                                        cout <<"Escolheu a pesquisa sobre os passageiros nas partidas"<< endl;
                                        cout <<"Diga qual o primeiro nome que quer pesquisar"<< endl;
                                        string primeiro_nome;
                                        cin >> primeiro_nome;
                                        pesquisar_passageiro_por_nome_descolagens(aeroporto, primeiro_nome);
                                    } else {

                                        cout << "Nao ha voos nas descolagens" << endl<< endl;
                                    }
                                }
                        }

                        again=true;
                        break;
                    }
                    case 4: {

                        mostrar_passageiros_na_aproximacao(aeroporto);  //nova
                        cout <<"Diga qual o primeiro nome do passageiro que pretende mudar a nacionalidade :"<< endl;
                        string primeiro_nome;
                        cin>>primeiro_nome;
                        cout<< "Diga qual o seu segundo nome :"<< endl;
                        string segundo_nome;
                        cin>>segundo_nome;
                        cout <<"Para qual nacionalide gostaria de mudar :"<< endl;
                        string nova_nacionalidade;
                        cin>>nova_nacionalidade;
                        editar_nacionalidade_por_nome(aeroporto,primeiro_nome,segundo_nome, nova_nacionalidade);
                        again=true;
                        break;
                    }
                    case 5: {
                        cout << "Escolheu inverter a lista da pista" << endl;  // nova
                        inverter_lista_pista(aeroporto);
                        cout << "Dia-> "<< dia << endl;
                        estado_aeroporto(aeroporto, numero_avioes_aeroporto);
                        again=true;
                        break;
                    }
                    case 6: {
                        carregarEstadoAeroporto(aeroporto, arquivo_proximacao, arquivo_pista, arquivo_descolagem);
                        estado_aeroporto(aeroporto, numero_avioes_aeroporto);
                        again = true;
                        break;
                    }
                    case 7:{
                        cout << "Por quantos dias fechar? (maximo: 5 dias)" << endl;
                        cin >> ciclos;
                        if (ciclos > 5) {
                            cout << "Nao e possivel fechar o aeroporto para mais do que 5 dias." << endl;
                        }
                        else{
                            fechar_abrir_aeroporto(numero_avioes_aproximacao, aeroporto_fechado);
                        }
                        again=true;
                        break;
                    }
                    case 8:{
                        again=true;
                        break;
                    }
                    default: {
                        cout << "Opcao invalida." << endl;
                        break;
                    }
                }
            }
        }

        else if (opcao_menu == "g") {
            bool estado_gravacao = gravar_estado_aeroporto(aeroporto, arquivo_proximacao, arquivo_pista,arquivo_descolagem);
            if (!estado_gravacao) {
                cout << "Nao foi possivel gravar o estado do aeroporto." << endl;
            } else {
                cout << "Estado do aeroporto gravado com sucesso.\n" << endl;
            }
        }

        else if(opcao_menu == "s") {
            //para 3.6

            if (dias_fechadas < ciclos + 1 && aeroporto_fechado) {
                dias_fechadas += 1;
                cout << "-------Aeroporto fechado!-------" << endl;
            }

            if (dias_fechadas == (ciclos + 1) && aeroporto_fechado) {
                fechar_abrir_aeroporto(numero_avioes_aproximacao, aeroporto_fechado);
                dias_fechadas = 0;
                ciclos = 0;
            }
            //-----
            cout << "\nDias_fechado -> " << dias_fechadas << endl;
            cout << "\nDia -> " << dia + 1 << endl;
            ++dia;
            simulacao_ciclo(dia, aeroporto, array_modelos, array_voos, array_origens, array_destinos, array_primeiros_nomes,
                            array_segundos_nomes, array_nacionalidades, numero_avioes_aproximacao,permissao_remocao,
                            numero_avioes_pista, numero_avioes_descolando, numero_bilhetes_gerados,raiz,
                            numero_avioes_aeroporto, numero_voos, arvores_nacionalidade, numero_nacionalidades, aeroporto_fechado);
            cout << "numero_avioes_descolando: " << numero_avioes_descolando << endl;
        }
    }

    // Libertação de memória alocada dinamicamente
    delete[] array_primeiros_nomes;
    delete[] array_segundos_nomes;
    delete[] array_nacionalidades;
    delete[] array_voos;
    delete[] array_modelos;
    delete[] array_origens;
    delete[] array_destinos;

    return 0;
}