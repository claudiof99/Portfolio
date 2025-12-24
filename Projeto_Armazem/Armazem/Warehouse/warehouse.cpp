#include "warehouse.h"
#include <iostream>
#include <fstream>
#include <stdlib.h>
#include <ctime>
using namespace std;
#include <algorithm>


///GERAÇÃO DE VALORES ALEATÓRIOS

/**
 * Função que retorna um número inteiro aleatório entre 3 e 6
 * @return - inteiro com a capacidade da secção
 */
int aleatorio_capacidade() {
    int capacidade = rand() % 4 + 3;
    return capacidade;
}

/**
 * Função que retorna um número inteiro aleatório entre 7 e 10
 * @return - inteiro com o número de secções
 */
int aleatorio_numero_seccoes() {
    int numero_seccoes = rand() % 4 + 7;
    return numero_seccoes;
}

/**
 * Função que retorna um número inteiro aleatório entre 1000 e 9999
 * @return - inteiro com o número de série da peça
 */
int aleatorio_numero_serie() {
    int numero_serie = rand() % 9000 + 1000;
    return numero_serie;
}

/**
 * Função que retorna um número inteiro aleatório múltiplo de 5 entre 10 e 900
 * @return - inteiro com o preço (EUR) da peça
 */
int aleatorio_preco() {
    int preco = ((rand() % 179 + 2) * 5);
    return preco;
}

/**
 * Função que retorna um número inteiro aleatório entre 5 e 50
 * @return - inteiro com a probabilidade (%) de venda da peça
 */
int aleatorio_probabilidade_venda() {
    int probabilidade_venda = rand() % 46 + 5;
    return probabilidade_venda;
}



///GERAÇÃO DE ARRAYS A PARTIR DOS FICHEIROS

/**
 * Função para contar o número de linhas do arquivo "categorias.txt"
 * @return inteiro com o número de categorias do arquivo
 *         1, em caso de erro ao abrir o arquivo
 */
int contador_linhas_categorias() {

    ifstream ficheiro("categorias.txt");
    if (ficheiro.is_open()) {

        int numero_categorias = 0;
        string linha;

        while (getline(ficheiro, linha)){
            numero_categorias++;
        }

        ficheiro.close();
        return numero_categorias;
    }

    else {
        cerr << "Erro ao abrir o ficheiro." << endl;
        return 1;
    }
}

/**
 * Função que recebe um array de strings e o número de linhas a serem lidas do arquivo "categorias.txt"
 * @param categoria[] - array que vai armazenar as categorias
 * @param numero_categorias - inteiro com o número de categorias
 */
void categorias(string categoria[], int numero_categorias) {

    ifstream ficheiro("categorias.txt");
    if (ficheiro.is_open()) {

        for (int i = 0; i < numero_categorias; i++) {
            getline(ficheiro, categoria[i]);
        }

        ficheiro.close();
    }

    else {
        cerr << "Erro ao abrir o ficheiro." << endl;
        exit(1);
    }
}

/**
 * Função para contar o número de linhas do arquivo "marcas.txt"
 * @return - inteiro com o número de marcas do arquivo
 *           1, em caso de erro ao abrir o arquivo
 */
int contador_linhas_marcas() {

    ifstream ficheiro("marcas.txt");
    if (ficheiro.is_open()) {

        int numero_marcas = 0;
        string linha;

        while (getline(ficheiro, linha)){
            numero_marcas++;
        }

        ficheiro.close();
        return numero_marcas;
    }

    else {
        cerr << "Erro ao abrir o ficheiro." << endl;
        return 1;
    }
}

/**
 * Função que recebe um array de strings e o número de linhas a serem lidas do arquivo "marcas.txt"
 * @param marca[] - array que vai armazenar as marcas
 * @param numero_marcas - inteiro com o número de marcas
 */
void marcas(string marca[], int numero_marcas) {

    ifstream ficheiro("marcas.txt");
    if (ficheiro.is_open()) {

        for (int i = 0; i < numero_marcas; i++) {
            getline(ficheiro, marca[i]);
        }

        ficheiro.close();
    }

    else {
        cerr << "Erro ao abrir o ficheiro." << endl;
        exit(1);
    }
}



///INICIALIZAÇÃO DAS SECÇÕES, PEÇAS E LISTA DE CHEGADA

/**
 * Função que recebe um número inteiro e retorna um ponteiro para um array de estruturas
 * Atribui ID, categoria, capacidade, quantidade e faturação; Controla as promoções das secções e as peças inseridas na secção;
 * @param numero_seccoes - inteiro com o número de secções
 * @return - ponteiro para um array de estruturas 'seccao' com as secções com ID, categoria, capacidade, quantidade, faturação, estado valor e duração da promoção atribuídos e array das peças das secções
 */
seccao * inicializacao_seccoes(int numero_seccoes) {

    int numero_categorias = contador_linhas_categorias();
    string categoria[numero_categorias];
    categorias(categoria, numero_categorias);

    seccao * seccoes = new seccao[numero_seccoes];

    for (int i = 0; i < numero_seccoes; i++) {
        seccoes[i].ID = 'A' + i;
        seccoes[i].categoria = categoria[rand() % numero_categorias];
        seccoes[i].capacidade = aleatorio_capacidade();
        seccoes[i].quantidade = 0;
        seccoes[i].faturacao = 0;
        seccoes[i].seccao_em_promocao = false;
        seccoes[i].duracao_promocao = 0;
        seccoes[i].valor_promocao = 0;

        ///Peças que vão estar nas secções
        seccoes[i].pecas = new peca * [seccoes[i].capacidade];
        for (int j = 0; j < seccoes[i].capacidade; j++) {
            seccoes[i].pecas[j] = nullptr;
        }
    }

    return seccoes;
}

/**
 * Função que recebe um ponteiro para um array de estruturas e dois números inteiros. Retorna um ponteiro para um array de estruturas
 * Atribui categoria, marca, número de série, preço, probabilidade de venda e estado de promoção a cada peça
 * Garante a geração de um número de série único
 * Garante que a categoria das peças geradas está presente nas secções
 * @param numero_pecas - inteiro com o número de peças
 * @param seccoes - ponteiro para um array de estruturas 'seccao' com as secções com ID, categoria e capacidade atribuídos
 * @param numero_seccoes - inteiro com o número de secções
 * @param numeros_serie - array de inteiros contendo os números de série gerados
 * @param numeros_serie_gerados - referência a um inteiro com o número de números de série já gerados
 * @param numeros_serie_disponiveis - inteiro com o numero de números de série disponíveis
 * @return - ponteiro para um array de estruturas 'peca' com as peças com categoria, marca, número de série, preço, probabilidade de venda e estado de promoção atribuídos
 */
peca * inicializacao_pecas(int numero_pecas, seccao * seccoes, int numero_seccoes, int numeros_serie[], int& numeros_serie_gerados, int numeros_serie_disponiveis){

    int numero_categorias = contador_linhas_categorias();
    string categoria[numero_categorias];
    categorias(categoria, numero_categorias);

    int numero_marcas = contador_linhas_marcas();
    string marca[numero_marcas];
    marcas(marca, numero_marcas);

    peca * pecas = new peca[numero_pecas];

    for (int i = 0; i < numero_pecas; i++) {
        int numero_serie;
        bool numero_repetido = true;

        ///Geração de um número de série único
        if (numeros_serie_gerados >= numeros_serie_disponiveis) {
            break;
        }
        while (numero_repetido) {
            numero_serie = aleatorio_numero_serie();
            numero_repetido = false;

            for (int j = 0; j < numeros_serie_gerados; j++) {
                if (numeros_serie[j] == numero_serie) {
                    numero_repetido = true;
                    break;
                }
            }
        }
        numeros_serie[numeros_serie_gerados] = numero_serie;
        numeros_serie_gerados++;

        ///Geração de categoria para as peças que esteja presente nas secções
        bool categoria_valida = false;
        while (!categoria_valida) {
            string categoria_aleatoria = categoria[rand() % numero_categorias];

            for (int j = 0; j < numero_seccoes; j++) {
                if (seccoes[j].categoria == categoria_aleatoria) {
                    categoria_valida = true;
                    break;
                }
            }

            if (categoria_valida) {
                pecas[i].categoria = categoria_aleatoria;
                pecas[i].marca = marca[rand() % numero_marcas];
                pecas[i].numero_serie = numero_serie;
                pecas[i].preco = aleatorio_preco();
                pecas[i].probabilidade_venda = aleatorio_probabilidade_venda();
                pecas[i].peca_em_promocao = false;
            }
        }
    }

    return pecas;
}

/**
 * Função que recebe dois ponteiros para a estrutura 'peca', um número inteiro, três referências para um inteiro e uma referência constante para a estrutura 'peca'
 * Limita a lista de chegada a 50 peças e coloca peças da mesma categoria juntas por ordem de chegada, mantendo a ordem natural da lista de chegada
 * Transfere peças recebidas por referência para a lista de espera enquanto a lista de chegada está cheia
 * Aloca memória dinamicamente para a lista de espera quando esta está cheia e é necessário armazenar peças enquanto a lista de chegada também está cheia
 * Prioriza a transferência de peças existentes na lista de espera para a lista de chegada, em relação à transferência de peças recebidas por referência para essa mesma lista
 *
 * @param lista_chegada - ponteiro para a estrutura 'peca' com a lista de chegada
 * @param capacidade_maxima - inteiro com a capacidade máxima da lista de chegada
 * @param primeiro_livre - referência para um inteiro com a posição do primeiro espaço livre da lista de chegada
 * @param nova_peca - referência constante para a estrutura 'peca' com a nova peça a ser inserida na lista de chegada
 * @param lista_de_espera - ponteiro para a estrutura 'peca' com a lista de espera
 * @param capacidade_espera - referência para um inteiro com a capacidade máxima da lista de espera
 * @param primeiro_livre_secundario - referência para um inteiro com a posição do primeiro espaço livre da lista de espera
 */
void inicializacao_lista_chegada(peca *& lista_chegada, int capacidade_maxima, int & primeiro_livre, const peca & nova_peca, peca *& lista_de_espera, int & capacidade_espera, int & primeiro_livre_secundario) {

    ///Verifica se a lista de chegada principal e secundária estão cheias, e se estiverem, aloca memória dinamicamente para a lista de chegada, para inserir a peça
    if (primeiro_livre_secundario >= capacidade_espera) {
        ///Aloca memória dinamicamente
        capacidade_espera += 5;
        peca *lista_de_espera_nova = new peca[capacidade_espera];

        ///Copia os elementos da lista de espera original para a nova (que foi criada)
        for (int i = 0; i < primeiro_livre_secundario; i++) {
            lista_de_espera_nova[i] = lista_de_espera[i];
        }

        ///Liberta a memória da lista de espera original e substitui a lista de espera nova pela lista de espera original
        delete[] lista_de_espera;
        lista_de_espera = lista_de_espera_nova;

        ///Adiciona a peça na lista de espera criada
        if (primeiro_livre_secundario < capacidade_espera) {
            lista_de_espera[primeiro_livre_secundario++] = nova_peca;
        }
    }
    else if (primeiro_livre >= capacidade_maxima && primeiro_livre_secundario < capacidade_espera) {
        ///Se a lista de chegada estiver cheia, e a lista de espera não, inserir a peça na lista de espera
        if (primeiro_livre_secundario < capacidade_espera) {
            lista_de_espera[primeiro_livre_secundario++] = nova_peca;
        }
    } else if (primeiro_livre < capacidade_maxima) { ///Se existir uma "vaga" na lista de chegada, há a inserção de uma peça nela (lista de chegada)

        ///Transfere as peças da lista de espera para a lista de chegada, enquanto houver espaço nela, antes de inserir diretamente a peça recebida nesta
        while (primeiro_livre < capacidade_maxima && primeiro_livre_secundario > 0) {
            int posicao_inserir = -1;

            ///Encontra a posição correta para inserir a peça na lista de chegada, com base na metodologia FIFO (First In, First Out)
            for (int i = 0; i < primeiro_livre; i++) {
                if (lista_de_espera[0].categoria == lista_chegada[i].categoria && (i + 1 == primeiro_livre || lista_chegada[i + 1].categoria != lista_de_espera[0].categoria)) {
                    posicao_inserir = i + 1;
                    break;
                }
            }
            cout << posicao_inserir << endl;

            ///Verifica se existe espaço para inserir uma nova peça
            if (posicao_inserir != -1) {
                ///Move as peças da lista de chegada para a direita, de forma a "abrir" espaço para a nova peça
                for (int i = primeiro_livre; i > posicao_inserir; i--) {
                    if (i > 0) {
                        lista_chegada[i] = lista_chegada[i - 1];
                    }
                }

                ///Inserção da peça na posição correta
                lista_chegada[posicao_inserir] = lista_de_espera[0];

                ///Reorganizar a lista de espera
                for (int i = 0; i < primeiro_livre_secundario - 1; i++) {
                    lista_de_espera[i] = lista_de_espera[i + 1];
                }

                primeiro_livre++;
                primeiro_livre_secundario--;
            } else {
                break;
            }
        }

        if (primeiro_livre_secundario == 0) { ///Se não existir mais peças na lista de espera, inserir a peça recebida na lista de chegada
            int posicao_inserir = primeiro_livre;

            ///Percorre a lista de chegada da esquerda para a direita
            for (int i = 0; i < primeiro_livre - 1; i++) {

                if (lista_chegada[i].categoria == nova_peca.categoria && lista_chegada[i+1].categoria != nova_peca.categoria) {
                    posicao_inserir = i + 1; ///Define a posição de inserção após a primeira peça com categoria equivalente encontrada, e que a próxima a essa seja de diferente categoria
                    break;
                }
                else {
                    posicao_inserir = primeiro_livre;
                }
            }
            ///Move as peças na lista de chegada da esquerda para a direita até à posição inserir para "abrir" espaço para a nova peça
            for (int i = primeiro_livre; i > posicao_inserir; i--) {
                lista_chegada[i] = lista_chegada[i - 1];
            }

            ///Insere a nova peça na posição inserir da lista
            lista_chegada[posicao_inserir] = nova_peca;

            ///Incrementa o primeiro livre
            primeiro_livre++;
        }
    }
}



///FUNCIONAMENTO
/**
 * Função que recebe um ponteiro para um array de estruturas, um número inteiro e uma referência para um número inteiro
 * Responsável pela venda de peças em cada secção do armazém
 * Atualiza o total de faturação do armazém e a faturação de cada secção
 *
 * @param seccoes - ponteiro para um array de estruturas 'seccao' que representa as secções do armazém
 * @param numero_seccoes - inteiro com o número de secções do armazém
 * @param total_faturacao - referência a um float com a faturação total do armazém
 */
void venda_pecas(seccao * seccoes, int numero_seccoes, float & total_faturacao) {
    for (int i = 0; i < numero_seccoes; i++) {
        for (int j = 0; j < seccoes[i].quantidade; j++) {

            ///Se existirem peças nas secções
            if (seccoes[i].pecas[j] != nullptr) {

                ///Se a probabilidade da peça da secção ser vendida for cumprida
                int numero_aleatorio = rand() % 100 + 1;
                if (numero_aleatorio <= seccoes[i].pecas[j]->probabilidade_venda) {

                    ///Atualiza o total de faturação do armazém e da secção
                    total_faturacao += seccoes[i].pecas[j]->preco;
                    seccoes[i].faturacao += seccoes[i].pecas[j]->preco;

                    ///Remove a peça vendida da secção
                    for (int k = j; k < seccoes[i].quantidade - 1; k++) {
                        seccoes[i].pecas[k] = seccoes[i].pecas[k + 1];
                    }

                    ///Decrementa a quantidade de peças na secção
                    seccoes[i].quantidade--;

                }
            }
        }
    }
}

/**
 * Função que recebe dois ponteiros para um array de estruturas, um número inteiro e uma referência para um número inteiro
 * Responsável pela transferência de peças da lista de chegada para as secções do armazém
 * @param lista_chegada - ponteiro para um array de estruturas 'peca' que representa a lista de chegada
 * @param primeiro_livre - referência a um inteiro que representa o índice do primeiro espaço livre na lista de chegada
 * @param seccoes - ponteiro para um array de estruturas 'seccao' que representa as secções do armazém
 * @param numero_seccoes - inteiro com o número de secções do armazém
 */
void transferencia_pecas(peca * lista_chegada, int & primeiro_livre, seccao * seccoes, int numero_seccoes) {

    ///Se a lista de chegada estiver vazia encerra a função
    if (primeiro_livre == 0) {
        return;
    }

    int pecas_transferidas = 0; ///Número de peças transferidas

    ///Percorre até ao fim ou até ao oitavo elemento da lista de chegada
    for (int i = 0; i < primeiro_livre && pecas_transferidas < 8; i++) {

        int seccao_livre = -1;

            ///Percorre as seccções
            for (int j = 0; j < numero_seccoes; j++) {

                ///Verifica se existe uma secção com categoria equivalente à da peça e espaço livre
                if (seccoes[j].quantidade < seccoes[j].capacidade && lista_chegada[i].categoria == seccoes[j].categoria) {
                    seccao_livre = j;
                    break;
                }
            }

            ///Transfere a peça da lista de chegada para a secção correspondente
            if (seccao_livre != -1) {
                seccoes[seccao_livre].pecas[seccoes[seccao_livre].quantidade] = new peca;
                * seccoes[seccao_livre].pecas[seccoes[seccao_livre].quantidade] = lista_chegada[i];
                seccoes[seccao_livre].quantidade++;

                ///Remove a peça transferida
                for (int k = i; k < primeiro_livre - 1; k++) {
                    lista_chegada[k] = lista_chegada[k + 1];
                }
                primeiro_livre--;
                i--; ///Atualiza a posição da peça a ser analisada
                pecas_transferidas++; ///Incrementa o número de peças transferidas
            }
    }
}



///GESTÃO
/**
 * Função que recebe dois ponteiros para um array de estruturas, um número inteiro e duas referências para um número inteiro
 * Responsável pela venda manual de peças em uma seção específica do armazém
 * Atualiza a faturação total do armazém e da seção
 * Repõe automaticamente uma peça na seção, caso exista uma peça da mesma categoria na lista de chegada
 *
 * @param seccoes - ponteiro para um array de estruturas 'seccao' que representa as secções do armazém
 * @param numero_seccoes - inteiro com o número de secções do armazém
 * @param lista_chegada - ponteiro para um array de estruturas 'peca' que representa a lista de chegada
 * @param primeiro_livre - referência a um inteiro que representa o índice do primeiro espaço livre na lista de chegada
 * @param total_faturacao - referência a um float que representa o total de faturação do armazém
 */
void venda_manual(seccao * seccoes, int numero_seccoes, peca * lista_chegada, int & primeiro_livre, float & total_faturacao) {

    ///Mostra as secções existentes com o ID e categoria correspondentes
    cout << "\nSeccoes existentes:" << endl;
    for (int i = 0; i < numero_seccoes; ++i) {
        cout << i + 1 << ". " << "Seccao " << seccoes[i].ID << " - " << seccoes[i].categoria << endl; ///Percorre todas as secções existentes
    }

    ///Seleção da secção
    int seccao_selecionada;
    do {
        cout << "\nSelecione a seccao desejada:" << endl;
        cin >> seccao_selecionada;
        seccao_selecionada--; ///Ajusta o índice do array

        if (seccao_selecionada < 0 || seccao_selecionada >= numero_seccoes) { ///Verifica se a secção selecionada é válida
            cout << "Seccao invalida. Por favor, selecione uma seccao valida.\n" << endl;
            cin.clear();
            cin.ignore();
        }
    } while (seccao_selecionada < 0 || seccao_selecionada >= numero_seccoes); ///Loop até ser fornecido uma secção válida

    ///Mostra as peças existentes na secção selecionada anteriormente
    cout << "\nPecas existentes na seccao " << seccoes[seccao_selecionada].ID << ":\n";
    if (seccoes[seccao_selecionada].quantidade == 0) {
        cout << " 0" << endl;
        cout << "Nao existem pecas nesta seccao. Por favor, selecione uma seccao valida para poder efetuar uma venda manual." << endl;
        return;
    } else {
        for (int i = 0; i < seccoes[seccao_selecionada].quantidade; ++i) {
            cout << "    " << i + 1 << ". " << seccoes[seccao_selecionada].pecas[i]->marca << " | "
                 << seccoes[seccao_selecionada].pecas[i]->categoria <<endl;
        }
    }

    ///Seleção da peça
    int peca_selecionada;
    do {
        cout << "\nSelecione a peca desejada:" << endl;
        cin >> peca_selecionada;
        peca_selecionada--; ///Ajusta o índice do array

        if (peca_selecionada < 0 || peca_selecionada >= seccoes[seccao_selecionada].quantidade) { ///Verifica se a peça selecionada é válida
            cout << "Peca invalida. Por favor, selecione uma peca valida." << endl;
            cin.clear();
            cin.ignore();
        }
    } while (peca_selecionada < 0 || peca_selecionada >= seccoes[seccao_selecionada].quantidade); ///Loop até ser fornecido uma peça válida

    ///Realiza a venda da peça selecionada
    seccoes[seccao_selecionada].faturacao += seccoes[seccao_selecionada].pecas[peca_selecionada]->preco;
    total_faturacao += seccoes[seccao_selecionada].pecas[peca_selecionada]->preco;
    cout << "\nVenda manual realizada com sucesso." << endl;

    ///Se existir alguma peça da mesma categoria que a peça vendida na lista de chegada
    bool peca_encontrada = false;
    for (int i = 0; i < primeiro_livre; ++i) {
        if (lista_chegada[i].categoria == seccoes[seccao_selecionada].pecas[peca_selecionada]->categoria) {
            peca_encontrada = true;

            ///Transfere a peça da lista de chegada para a secção na posição em que a peça vendida se encontrava
            * (seccoes[seccao_selecionada].pecas[peca_selecionada]) = lista_chegada[i];

            ///Remove a peça da lista de chegada
            for (int k = i; k < primeiro_livre - 1; ++k) {
                lista_chegada[k] = lista_chegada[k + 1];
            }
            --primeiro_livre;

            cout << "A peca foi reposta: foi transferida uma peca da lista de chegada com a mesma categoria da peca vendida para a seccao " << seccao_selecionada + 1 << "." << endl;
            break;
        }
    }

    ///Se não existir nenhuma peça da mesma categoria que a peça vendida na lista de chegada
    if (!peca_encontrada) {

        ///Remove a peça, ajusta e atualiza o estado da secção
        delete seccoes[seccao_selecionada].pecas[peca_selecionada];
        for (int i = peca_selecionada; i < seccoes[seccao_selecionada].quantidade - 1; ++i) {
            seccoes[seccao_selecionada].pecas[i] = seccoes[seccao_selecionada].pecas[i + 1];
        }
        seccoes[seccao_selecionada].quantidade--;

        cout << "A peca nao foi reposta: nao havia nenhuma peca na lista de chegada com a mesma categoria da peca vendida." << endl;
    }
}


/**
 * Função que recebe um ponteiro para um array de estruturas e dois inteiros
 * Responsável pela identificação da secção a qual devemos aplicar a promoção e as características relacionadas a esse evento (duração e valor, em %, da promoção)
 * Atualiza o estado da promoção da secção escolhida pelo utilizador e o valor da promoção nessa secção
 * Calcula o dia de término da promoção, a partir do número de ciclos (dias) e o dia atual
 *
 * @param seccoes - ponteiro para um array de estruturas 'seccao' que representa as secções do armazém
 * @param numero_seccoes - inteiro com o número de secções do armazém
 * @param dia_atual - inteiro com o número do presente dia
 */
void adicionar_promocao(seccao * seccoes, int numero_seccoes, int dia_atual) {

    bool seccao_promocao_valido = false;
    bool valor_promocao_valida = false;
    bool duracao_promocao_valida = false;
    int seccao_promocao, valor_promocao;

    while (!seccao_promocao_valido) {

        ///Mostra as secções existentes com o ID e categoria correspondentes
        cout << "\nSeccoes existentes:" << endl;
        for (int i = 0; i < numero_seccoes; ++i) {
            cout << i + 1 << ". " << "Seccao " << seccoes[i].ID << " - " << seccoes[i].categoria << endl; ///Percorre todas as secções existentes
        }

        cout << "\nSelecione a seccao que deseja adicionar a promocao:" << endl;
        cin >> seccao_promocao;

        ///Verifica se a escolha é válida
        if (seccao_promocao >= 1 && seccao_promocao <= numero_seccoes) {
            seccao_promocao_valido = true;
        }

        if (!seccao_promocao_valido) {
            cout << "\nSeccao invalida. Por favor, selecione uma seccao valida." << endl;
            cin.clear();
            cin.ignore();
        }
    }

    while (!valor_promocao_valida) {

        cout << "\nDigite o valor da promocao (em %):" << endl;
        cin >> valor_promocao;

        ///Verifica se o valor da promoção é válido (se é entre 0 e 100, sem incluir o valor 0)
        if (valor_promocao > 0 && valor_promocao <= 100){
            valor_promocao_valida = true;
        }

        if (!valor_promocao_valida) {
            cout << "\nValor para promocao invalido. Por favor, digite um valor valido." << endl;
            cin.clear();
            cin.ignore();
        }
    }

    ///Procura a secção escolhida para adicionar a promoção
    for (int i = 0; i < numero_seccoes; ++i) {
        if (seccoes[i].ID == char('A' + seccao_promocao - 1)) { ///Transforma a opção escolhida em letra para identificar a secção escolhida

            while (!duracao_promocao_valida) {
                cout << "\nDigite a duracao da promocao (em dias):" << endl;
                int duracao_promocao;
                cin >> duracao_promocao;

                ///Verifica se o valor da duração da promoção é válido (se é entre 0 e 100, sem incluir o valor 0)
                if (duracao_promocao > 0 && duracao_promocao <= 100){
                    duracao_promocao_valida = true;
                }

                if (!duracao_promocao_valida) {
                    cout << "\nValor para promocao invalido. Por favor, digite um valor valido." << endl;
                    cin.clear();
                    cin.ignore();
                }
                else {
                    seccoes[i].seccao_em_promocao = true;
                    seccoes[i].duracao_promocao = dia_atual + duracao_promocao; ///Define o dia de término da promoção
                    seccoes[i].valor_promocao = valor_promocao; ///Define o valor da promoção a aplicar à secção
                    cout << "Promocao adicionada com sucesso." << endl;
                }
            }
        }
    }
}

/**
 * Função que recebe um ponteiro para um array de estruturas e dois inteiros
 * Responsável pela aplicação da promoção, caso haja um "rótulo" na secção o qual nos indica que esta se encontra em promoção
 * Altera o estado da promoção na(s) secção(ões), tendo por base o dia de término da promoção e o dia atual
 * Atualiza o preço da(s) peça(s) presente(s) na(s) secção(ões), como também a sua probabilidade de venda, mediante o estado da promoção na secção em questão
 *
 * @param seccoes - ponteiro para um array de estruturas 'seccao' que representa as secções do armazém
 * @param numero_seccoes - inteiro com o número de secções do armazém
 * @param dia_atual - inteiro com o número do presente dia
 */
void aplicar_promocao(seccao* seccoes, int numero_seccoes, int dia_atual) {
    for (int i = 0; i < numero_seccoes; ++i) {

        ///Início de promoção
        if (seccoes[i].seccao_em_promocao && dia_atual < seccoes[i].duracao_promocao) {
            cout << "Existe uma promocao ativa na seccao " << seccoes[i].ID << " (" << seccoes[i].valor_promocao << "%)." << endl;
            for (int j = 0; j < seccoes[i].quantidade; ++j) {
                if (!seccoes[i].pecas[j]->peca_em_promocao){
                    ///Aumento da probabilidade de venda em 15%
                    seccoes[i].pecas[j]->probabilidade_venda += 15;
                    /// Redução do preço conforme o valor da promoção
                    float desconto = seccoes[i].valor_promocao / 100.0; ///Converção da percentagem da promoção em decimal
                    seccoes[i].pecas[j]->preco *= (1.0 - desconto); ///Aplicar o desconto na(s) peça(s) da secção
                    seccoes[i].pecas[j]->peca_em_promocao = true;
                }
            }
        }

            ///Fim de promoção
        else if (seccoes[i].seccao_em_promocao && dia_atual >= seccoes[i].duracao_promocao) {
            seccoes[i].seccao_em_promocao = false;
            for (int j = 0; j < seccoes[i].quantidade; ++j) {
                ///Diminuição da probabilidade de venda em 15%
                seccoes[i].pecas[j]->probabilidade_venda -= 15;
                ///Ajuste do preço para voltar ao valor original
                float preco_original = seccoes[i].pecas[j]->preco / (1.0 - (seccoes[i].valor_promocao / 100.0)); ///Cálculo do valor original da peça (antes da promoção)
                seccoes[i].pecas[j]->preco = preco_original;
                seccoes[i].pecas[j]->peca_em_promocao = false;
            }
            seccoes[i].valor_promocao = 0;
            cout << "A promocao na seccao " << seccoes[i].ID << " terminou." << endl;
        }

    }
}

/**
 * Função que recebe uma string e um número inteiro
 * Verifica se uma determinada categoria existe no arquivo "categorias.txt"
 * @param nova_categoria - string que contém a categoria a ser verificada
 * @param numero_categorias - inteiro com o número de categorias do arquivo
 * @return
 */
bool categoria_existe(string nova_categoria, int numero_categorias) {
    string categoria[numero_categorias];
    categorias(categoria, numero_categorias);
    bool resultado = false;

    for (int i = 0; i < numero_categorias; i++) {
        if (categoria[i] == nova_categoria) {
            resultado = true;
        }
    }
    return resultado;
}

/**
 * Função que recebe uma string
 * Adiciona uma nova categoria ao arquivo "categorias.txt"
 * @param nova_categoria - string que contém a categoria a ser verificada
 */
void adicionar_categoria(string nova_categoria) {
    ofstream ficheiro("categorias.txt", ios::app);

    if (ficheiro.is_open()) {
        ficheiro << endl;
        ficheiro << nova_categoria;
        ficheiro.close();
    }

    else {
        cerr << "Erro ao abrir o ficheiro." << endl;
        exit(1);
    }
}

/**
 * Função que recebe um ponteiro para um array, uma string e um inteiro
 * Responsável por alterar a categoria de uma secção
 * @param seccoes - ponteiro para um array de estruturas 'seccao' que representa as secções do armazém
 * @param nova_categoria - string que contém a categoria a ser verificada
 * @param numero_seccoes - inteiro com o numero de secções do armazém
 */
void alterar_categoria(seccao * seccoes, string nova_categoria, int numero_seccoes) {

    ///Mostra as secções existentes com o ID e categoria correspondentes
    cout << "\nSeccoes existentes:" << endl;
    for (int i = 0; i < numero_seccoes; ++i) {
        cout << i + 1 << ". " << "Seccao " << seccoes[i].ID << " - " << seccoes[i].categoria << endl; ///Percorre todas as secções existentes
    }

    ///Seleção da secção
    int seccao_selecionada;
    do {
        cout << "\nSelecione a seccao desejada:" << endl;
        cin >> seccao_selecionada;
        seccao_selecionada--; ///Ajusta o índice do array

        if (seccao_selecionada < 0 || seccao_selecionada >= numero_seccoes) { ///Verifica se a secção selecionada é válida
            cout << "Seccao invalida. Por favor, selecione uma seccao valida." << endl;
            cin.clear();
            cin.ignore();
        }
    } while (seccao_selecionada < 0 || seccao_selecionada >= numero_seccoes); ///Loop até ser fornecido uma secção válida

    cout << "\nIndique a nova categoria da seccao:" << endl;
    cin >> nova_categoria;

    ///Se a categoria ainda não existir
    if (!categoria_existe(nova_categoria, contador_linhas_categorias())){
        adicionar_categoria(nova_categoria); ///Adiciona a categoria ao ficheiro
    }

    ///Remoção dos produtos da secção
    if (seccoes[seccao_selecionada].pecas != nullptr) {
        delete[] seccoes[seccao_selecionada].pecas;
        seccoes[seccao_selecionada].quantidade = 0;
    }

    ///Alteração da categoria
    for (int i = 0; i < numero_seccoes; i++) {
        if (seccoes[i].ID == char('A' + seccao_selecionada)) {
            seccoes[i].categoria = nova_categoria;
            cout << "A categoria foi alterada com sucesso." << endl;
            return;
        }
    }

    cout << "Erro: a categoria da seccao nao foi alterada com sucesso." << endl;
}


/**
 * Função que recebe uma referência de apontador e uma referência para um inteiro
 *
 * @param seccoes - Referência de um ponteiro para um array de estruturas 'seccao' que representa as secções do armazém
 * @param numero_seccoes - Referência a um inteiro com o número de seções do armazém
 */

void adicionar_seccao(seccao*&seccoes, int &numero_seccoes) {
    char ID_introduzido;
    string nova_categoria;
    int nova_capacidade;

    cout << "\nIndique um ID para a nova seccao:" << endl;
    cin >> ID_introduzido;

    ///Verifica se o ID já existe
    int i = 0;
    while (i < numero_seccoes) {
        if (seccoes[i].ID == ID_introduzido) {
            cout << "Este ID ja existe. Por favor, insira outro ID:" << endl;
            cin >> ID_introduzido;
            i = 0;
        } else {
            i++;
        }
    }

    cout << "\nIndique uma categoria para a nova seccao:" << endl;
    cin >> nova_categoria;

    ///Adiciona a categoria ao arquivo se ainda não existir no mesmo
    if (!categoria_existe(nova_categoria, contador_linhas_categorias())) {
        adicionar_categoria(nova_categoria);
    }

    cout << "\nIndique uma capacidade para nova seccao:" << endl;
    cin >> nova_capacidade;

    ///Cria uma nova secção com os valores fornecidos
    seccao nova_seccao;
    nova_seccao.ID = ID_introduzido;
    nova_seccao.categoria = nova_categoria;
    nova_seccao.capacidade = nova_capacidade;
    nova_seccao.quantidade = 0;
    nova_seccao.faturacao = 0;
    nova_seccao.seccao_em_promocao = false;
    nova_seccao.duracao_promocao = 0;
    nova_seccao.pecas = new peca*[nova_capacidade];
    for (int i = 0; i < nova_capacidade; i++) {
        nova_seccao.pecas[i] = nullptr;
    }

    ///Atualiza as secções para a incluir a nova secção
    seccao * novas_seccoes = new seccao[numero_seccoes + 1];
    for (int i = 0; i < numero_seccoes; i++) {
        novas_seccoes[i] = seccoes[i];
    }
    novas_seccoes[numero_seccoes] = nova_seccao;
    delete[] seccoes;
    seccoes = novas_seccoes;
    numero_seccoes++;
    cout << "\nSeccao adicionada com sucesso." << endl;
}

/**
 * Função que recebe dois ponteiros para um array e duas referências para um número inteiro
 * Responsável por ordenar todas as peças do armazém por ordem alfabética de marca
 * @param seccoes - ponteiro para um array de estruturas 'seccao' que representa as secções do armazém
 * @param lista_chegada - ponteiro para um array de estruturas 'peca' que representa a lista de chegada
 * @param primeiro_livre - referência a um inteiro que representa o índice do primeiro espaço livre na lista de chegada
 * @param numero_seccoes - referência a um inteiro que representa o número de secções
 */
void armazem_alfabetica(seccao* seccoes, peca * lista_chegada, int &primeiro_livre, int &numero_seccoes){

    ///Número de peças do armazém (secções + lista de chegada)
    int numero_pecas_armazem = 0;
    for (int i = 0; i < numero_seccoes; ++i) {
        numero_pecas_armazem += seccoes[i].quantidade;
    }
    numero_pecas_armazem += primeiro_livre;

    ///Cria um array com todas as peças do armazém
    peca *pecas_armazem = new peca[numero_pecas_armazem];
    int contador_pecas = 0;
    for (int i = 0; i < numero_seccoes; ++i) {
        for (int j = 0; j < seccoes[i].quantidade; ++j) {
            pecas_armazem[contador_pecas++] = *seccoes[i].pecas[j];
        }
    }
    for (int i = 0; i < primeiro_livre; ++i) {
        pecas_armazem[contador_pecas++] = lista_chegada[i];
    }

    ///Ordena as peças do armazém por ordem alfabética da marca
    sort(pecas_armazem, pecas_armazem + numero_pecas_armazem, [](const peca &a, const peca &b) {
        return a.marca < b.marca;
    });

    ///Imprime as peças do armazém
    for (int i = 0; i < numero_pecas_armazem; ++i) {
        cout << pecas_armazem[i].marca << " | " << pecas_armazem[i].categoria << " | " << pecas_armazem[i].numero_serie << " | " << pecas_armazem[i].preco << " EUR" << endl;
    }

    delete[] pecas_armazem;
}

/**
 * Função que recebe dois ponteiros para um array e duas referências para um número inteiro
 * Responsável por ordenar todas as peças do armazém por preço
 * @param seccoes - ponteiro para um array de estruturas 'seccao' que representa as secções do armazém
 * @param lista_chegada - ponteiro para um array de estruturas 'peca' que representa a lista de chegada
 * @param primeiro_livre - referência a um inteiro que representa o índice do primeiro espaço livre na lista de chegada
 * @param numero_seccoes - referência a um inteiro que representa o número de secções
 */
void armazem_preco(seccao* seccoes, peca * lista_chegada, int &primeiro_livre, int &numero_seccoes){

    ///Número de peças do armazém (secções + lista de chegada)
    int numero_pecas_armazem = 0;
    for (int i = 0; i < numero_seccoes; ++i) {
        numero_pecas_armazem += seccoes[i].quantidade;
    }
    numero_pecas_armazem += primeiro_livre;

    ///Cria um array com todas as peças do armazém
    peca *pecas_armazem = new peca[numero_pecas_armazem];
    int contador_pecas = 0;
    for (int i = 0; i < numero_seccoes; ++i) {
        for (int j = 0; j < seccoes[i].quantidade; ++j) {
            pecas_armazem[contador_pecas++] = *seccoes[i].pecas[j];
        }
    }
    for (int i = 0; i < primeiro_livre; ++i) {
        pecas_armazem[contador_pecas++] = lista_chegada[i];
    }

    ///Ordena as peças do armazém por preço
    sort(pecas_armazem, pecas_armazem + numero_pecas_armazem, [](const peca &a, const peca &b) {
        return a.preco < b.preco;
    });

    ///Imprime as peças do armazém
    for (int i = 0; i < numero_pecas_armazem; ++i) {
        cout << pecas_armazem[i].marca << " | " << pecas_armazem[i].categoria << " | " << pecas_armazem[i].numero_serie << " | " << pecas_armazem[i].preco << " EUR" << endl;
    }

    delete[] pecas_armazem;
}



/**
 * Função que recebe três ponteiros para dois arrays, uma referência para um inteiro, um float e sete inteiros
 * Responsável por gravar o estado do armazém num ficheiro
 * @param seccoes - ponteiro para um array de estruturas 'seccao' que representa as secções do armazém
 * @param lista_chegada - ponteiro para a estrutura 'peca' com a lista de chegada
 * @param numero_seccoes - inteiro com o numero de seccoes do armazem
 * @param total_faturacao - float com o total da faturacao
 * @param dia - inteiro com o dia do armazém
 * @param primeiro_livre - inteiro com a primeira posição livre na lista de chegada
 * @param numeros_serie_gerados - referência a um inteiro com o número de números de série já gerados
 * @param capacidade_espera - referência para um inteiro com a capacidade máxima da lista de espera
 * @param estado_gravacao - referência para um booleano que indica se a gravação foi bem-sucedida ou não
 * @param primeiro_livre_secundario - inteiro com a primeira posição livre na lista de espera
 * @param lista_de_espera - ponteiro para a estrutura 'peca' com a lista de espera
 */
void gravar_estado_armazem(seccao* seccoes, peca* lista_chegada, int numero_seccoes, float total_faturacao, int dia,
                           int primeiro_livre, int numeros_serie_gerados, int capacidade_espera, bool & estado_gravacao,
                           int primeiro_livre_secundario, peca * lista_de_espera){

    ofstream arquivo("estado_armazem.txt");

    if (arquivo.is_open()) {

        arquivo << numero_seccoes << std::endl;

        ///Escreve as informações de cada secção
        for (int i = 0; i < numero_seccoes; i++) {
            arquivo << seccoes[i].ID << " "
                    << seccoes[i].categoria << " "
                    << seccoes[i].capacidade << " "
                    << seccoes[i].quantidade << " "
                    << seccoes[i].seccao_em_promocao << " "
                    << seccoes[i].duracao_promocao << " "
                    << seccoes[i].valor_promocao << " "
                    << seccoes[i].faturacao << endl;

            ///Escreve as informações das peças de cada secção
            for (int j = 0; j < seccoes[i].quantidade; j++) {
                arquivo << seccoes[i].pecas[j]->marca << " "
                        << seccoes[i].pecas[j]->categoria << " "
                        << seccoes[i].pecas[j]->numero_serie << " "
                        << seccoes[i].pecas[j]->probabilidade_venda << " "
                        << seccoes[i].pecas[j]->peca_em_promocao << " "
                        << seccoes[i].pecas[j]->preco << endl;
            }
        }

        arquivo << primeiro_livre << endl;
        cout << primeiro_livre;

        ///Escreve as informações da lista de chegada
        for (int i = 0; i < primeiro_livre; i++) {
            arquivo << lista_chegada[i].marca << " "
                    << lista_chegada[i].categoria << " "
                    << lista_chegada[i].numero_serie << " "
                    << lista_chegada[i].probabilidade_venda << " "
                    << lista_chegada[i].peca_em_promocao << " "
                    << lista_chegada[i].preco << endl;
        }

        arquivo << total_faturacao << endl;
        arquivo << dia << endl;
        arquivo << numeros_serie_gerados << endl;
        arquivo << capacidade_espera << endl;
        arquivo << primeiro_livre_secundario << endl;

        ///Escreve as informações da lista de espera
        for (int i = 0; i < primeiro_livre_secundario; i++) {
            arquivo << lista_de_espera[i].marca << " "
                    << lista_de_espera[i].categoria << " "
                    << lista_de_espera[i].numero_serie << " "
                    << lista_de_espera[i].probabilidade_venda << " "
                    << lista_de_espera[i].peca_em_promocao << " "
                    << lista_de_espera[i].preco << endl;
        }

        estado_gravacao = true;
        arquivo.close();
    } else {
        bool estado_gravacao = false;
        cerr << "Erro ao abrir o arquivo para escrita." << endl;
    }

}

/**
 * Função que recebe três ponteiros para dois arrays, uma referência para um inteiro, um float e sete inteiros
 * Responsável por carregar um estado do armazém antigo gravado num ficheiro
 * @param seccoes - ponteiro para um array de estruturas 'seccao' que representa as secções do armazém
 * @param lista_chegada - ponteiro para a estrutura 'peca' com a lista de chegada
 * @param numero_seccoes - inteiro com o numero de seccoes do armazem
 * @param total_faturacao - float com o total da faturacao
 * @param dia - inteiro com o dia do armazém
 * @param primeiro_livre - inteiro com a primeira posição livre na lista de chegada
 * @param numeros_serie_gerados - referência a um inteiro com o número de números de série já gerados
 * @param capacidade_espera - referência para um inteiro com a capacidade máxima da lista de espera
 * @param estado_gravacao - referência para um booleano que indica se a gravação foi bem-sucedida ou não
 * @param primeiro_livre_secundario - inteiro com a primeira posição livre na lista de espera
 * @param lista_de_espera - ponteiro para a estrutura 'peca' com a lista de espera
 */
void carregar_estado_armazem(seccao* &seccoes, peca* &lista_chegada, int &numero_seccoes, float &total_faturacao, int &dia,
                             int &primeiro_livre, int & numeros_serie_gerados, int &capacidade_espera, bool & estado_gravacao,
                             int &primeiro_livre_secundario, peca * &lista_de_espera){

    ifstream arquivo("estado_armazem.txt"); // Abrir arquivo para leitura

    if (arquivo.is_open()) {

        if (!estado_gravacao) {
            ofstream arquivo("estado_armazem.txt");
            arquivo.close();
            cout << "Sem nenhum estado do armazem para apresentar." << endl;
        }
        else {

            arquivo >> numero_seccoes;
            seccoes = new seccao[numero_seccoes];

            ///Recebe novos dados das secções e substitui os atuais
            for (int i = 0; i < numero_seccoes; i++) {
                arquivo >> seccoes[i].ID >> seccoes[i].categoria >> seccoes[i].capacidade
                        >> seccoes[i].quantidade >> seccoes[i].seccao_em_promocao >> seccoes[i].duracao_promocao >> seccoes[i].valor_promocao >> seccoes[i].faturacao;
                seccoes[i].pecas = new peca*[seccoes[i].capacidade];

                ///Recebe novos dados das peças das secções e substitui os atuais
                for (int j = 0; j < seccoes[i].quantidade; j++) {
                    seccoes[i].pecas[j] = new peca;
                    arquivo >> seccoes[i].pecas[j]->marca
                            >> seccoes[i].pecas[j]->categoria
                            >> seccoes[i].pecas[j]->numero_serie
                            >> seccoes[i].pecas[j]->probabilidade_venda
                            >> seccoes[i].pecas[j]->peca_em_promocao
                            >> seccoes[i].pecas[j]->preco;
                }
            }

            ///Recebe novos dados da lista de chegada e substitui os atuais
            int primeiro_livre;
            arquivo >> primeiro_livre;
            lista_chegada = new peca[primeiro_livre];
            cout << primeiro_livre;
            for (int i = 0; i < primeiro_livre; i++) {
                arquivo >> lista_chegada[i].marca
                        >> lista_chegada[i].categoria
                        >> lista_chegada[i].numero_serie
                        >> lista_chegada[i].probabilidade_venda
                        >> lista_chegada[i].peca_em_promocao
                        >> lista_chegada[i].preco;
            }

            arquivo >> total_faturacao >> dia >> numeros_serie_gerados >> capacidade_espera >> primeiro_livre_secundario;

            ///Recebe novos dados da lista de espera e substitui os atuais
            int primeiro_livre_secundario;
            int capacidade_espera;
            if (primeiro_livre_secundario > 0) {
                lista_de_espera = new peca[capacidade_espera];
                for (int i = 0; i < primeiro_livre_secundario; i++) {
                    arquivo >> lista_de_espera[i].marca
                            >> lista_de_espera[i].categoria
                            >> lista_de_espera[i].numero_serie
                            >> lista_de_espera[i].probabilidade_venda
                            >> lista_de_espera[i].peca_em_promocao
                            >> lista_de_espera[i].preco;
                }
            }

            ///"Imprime" o armazém do dia gravado
            cout << "***********************************************************************************************" << endl;
            cout << "\n";
            cout << "*********************************************************" << endl;
            cout << "                   Lista de Espera                     " << endl;
            cout << "*********************************************************" << endl;

            for (int i = 0; i < primeiro_livre_secundario; i++) {
                cout << lista_de_espera[i].marca << " | "
                     << lista_de_espera[i].categoria << " | "
                     << lista_de_espera[i].numero_serie << " | "
                     << lista_de_espera[i].preco << " EUR" << endl;
            }

            cout << "\nDia -> " << dia << endl;
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
        arquivo.close();
    } else {
        cerr << "Erro ao abrir o arquivo para leitura." << endl;
    }
}