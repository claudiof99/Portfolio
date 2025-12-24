#include "airport.h"
#include <iostream>
#include <fstream>
#include <cstring>
#include <string>
#include <stdlib.h>
#include <ctime>
#include <locale>
using namespace std;
#include <algorithm>
#include <sstream>
#include <map>

// ÁRVORES BINÁRIAS

/**
 * Função que recebe uma string e cria um novo nó para a árvore binária
 * @param item - string a ser armazenada no nó
 * @return ponteiro para o novo nó criado
 */
nodo_arvore_binaria * novo_nodo_arvore(string item) {
    nodo_arvore_binaria* novo = new nodo_arvore_binaria;
    novo -> item = item;
    novo -> esq = nullptr; // Filho esquerdo nulo
    novo -> dir = nullptr; // Filho direito nulo
    return novo;
}

/**
 * Função que recebe uma string e um ponteiro para o nó raiz da árvore binária
 * Insere um novo nó em uma árvore binária
 * @param raiz - ponteiro para a raiz da árvore binária onde o item será inserido
 * @param item - string a ser inserida no novo nó
 * @return ponteiro para a raiz da árvore após a inserção
 */
nodo_arvore_binaria* inserir_nodo_arvore(nodo_arvore_binaria*& raiz, string item){
    // Cria um ponteiro auxiliar para percorrer a árvore e um ponteiro para armazenar o nó anterior
    nodo_arvore_binaria* aux = raiz;
    nodo_arvore_binaria* prev = nullptr;

    // Se a raiz for NULL, cria um novo nó como raiz contendo o item fornecido
    if (raiz == nullptr)
        raiz = novo_nodo_arvore(item);

    // Caso contrário, percorre a árvore até encontrar a posição correta para inserir o item
    else{
        while (aux != nullptr){
            prev = aux;
            aux = (aux->item > item ? aux->esq : aux->dir );
        }

        // Insere o novo nó na posição encontrada
        if (prev->item < item)
            prev->dir = novo_nodo_arvore(item);
        else
            prev->esq = novo_nodo_arvore(item);
    }
    return raiz;
}


/**
 * Função que recebe uma string e um ponteiro para o nó raiz da árvore binária
 * Realiza uma pesquisa em uma árvore binária (BST) para verificar se o item está presente
 * @param raiz - ponteiro para o nó raiz da árvore binária
 * @param item - elemento a ser pesquisado na árvore
 * @return true se o item estiver presente na árvore
 *         false caso contrário
 */
bool pesquisar_nodo_arvore(nodo_arvore_binaria* raiz, string item){
    if (raiz == NULL) // Árvore está vazia ou que o item não foi encontrado
        return false;
    if (item == raiz->item) // Item procurado é igual ao item armazenado no nó atual
        return true;
    else if (item < raiz->item) // Pesquisa na subárvore esquerda (item procurado é menor que o item no nó atual)
        return pesquisar_nodo_arvore(raiz->esq, item);
    else // Pesquisa na subárvore esquerda (item procurado é maior que o item no nó atual)
        return pesquisar_nodo_arvore(raiz->dir, item);
}




// GERAÇÃO DOS ARRAYS A PARTIR DOS FICHEIROS

/**
 * Função para contar o número de linhas de um arquivo em questão
 * @return inteiro com o número de linhas do arquivo
 *         1, em caso de erro ao abrir o arquivo
 */
int contador_linhas(string nome_ficheiro) {
    ifstream ficheiro(nome_ficheiro);
    if (ficheiro.is_open()) {
        int numero_linhas = 0;
        string linha;
        while (getline(ficheiro, linha)) {
            numero_linhas++;
        }
        ficheiro.close();
        return numero_linhas;
    } else {
        cerr << "Erro ao abrir o ficheiro: " << nome_ficheiro << endl;
        return -1;
    }
}

/**
 * Função que recebe o nome do ficheiro a ser lido, um array de strings e o número de linhas a serem lidas do arquivo em questão
 * Lê o arquivo e armazena em um array de strings
 * @param nome_ficheiro - string com o nome do ficheiro a carregar o conteúdo
 * @param array - array que vai armazenar o conteúdo do ficheiro
 * @param numero_linhas - inteiro com o número de linhas do ficheiro
 */
void carregar_dados(string nome_ficheiro, string array[], int numero_linhas) {
    ifstream ficheiro(nome_ficheiro, ios::in | ios::binary);
    if (ficheiro.is_open()) {
        for (int i = 0; i < numero_linhas; i++) {
            getline(ficheiro, array[i]);
        }
        ficheiro.close();
    } else {
        cerr << "Erro ao abrir o ficheiro: " << nome_ficheiro << endl;
        exit(1);
    }
}


// GERAÇÃO DE VALORES ALEATÓRIOS

/**
 * Função que retorna uma string TK + parte númerica aleatória entre 0000000000 e 9999999999
 * @return - string com os números do bilhete
 */
string numero_bilhete_aleatorio() {
    string numeros_bilhete = "TK";
    int digitos;
    for (int i = 0; i < 10; ++i) {
        digitos = rand() % 10;
        numeros_bilhete += to_string(digitos); // Junta o TK aos restantes digitos do bilhete através do to_string para a parte inteira
    }
    return numeros_bilhete;
}

/**
 * Função que gera um passageiro aleatório selecionando aleatoriamente os detalhes do passageiro (números do bilhete, primeiro nome, segundo nome e nacionalidade)
 * Garante que os números do bilhete não se repetem
 * @param array_primeiros_nomes - array de strings que contém os primeiros nomes dos passageiros
 * @param array_segundos_nomes - array de strings que contém os primeiros nomes dos passageiros
 * @param array_nacionalidades - array de strings que contém os primeiros nomes dos passageiros
 * @param numero_bilhetes_gerados - referência para inteiro com o número total de bilhetes gerados
 * @param raiz - referência para o ponteiro para o nó raiz da árvore binária
 * @param numero_bilhetes_gerados - referência para um inteiro com o número de bilhetes gerados
 * @return estrutura do tipo passageiro que contém os detalhes atribuídos ao passageiro gerado aleatoriamente
 */
passageiro passageiro_aleatorio( long long& numero_bilhetes_gerados, const string array_primeiros_nomes[],
                                 const string array_segundos_nomes[], const string array_nacionalidades[], nodo_arvore_binaria*& raiz) {

    string numeros_bilhete;
    const long long numero_bilhetes_diferentes = 10000000000;

    // Verifica se ainda existem bilhetes diferentes disponíveis
    if (numero_bilhetes_gerados >= numero_bilhetes_diferentes) {
        cout << "Nao existem mais bilhetes disponiveis." << endl;
        exit(1);
    }

    // Gera um bilhete aleatório único e insere-o numa árvore binária
    do {
        numeros_bilhete = numero_bilhete_aleatorio();
    } while (pesquisar_nodo_arvore(raiz, numeros_bilhete));
    raiz = inserir_nodo_arvore(raiz, numeros_bilhete);
    ++numero_bilhetes_gerados;

    // Criação do novo passageiro
    passageiro novo_passageiro;
    novo_passageiro.numeros_bilhete = numeros_bilhete;
    novo_passageiro.primeiro_nome = array_primeiros_nomes[rand() % contador_linhas("primeiro_nome.txt")];
    novo_passageiro.segundo_nome = array_segundos_nomes[rand() % contador_linhas("segundo_nome.txt")];
    novo_passageiro.nacionalidade = array_nacionalidades[rand() % contador_linhas("nacionalidade.txt")];

    return novo_passageiro;
}

/**
 * Função que verifica se um número de voo é único no aeroporto
 * Garante que não se repetem números de voos nas listas ligadas de aviões (aproximação, pista e descolagem)
 * @param aeroporto - referência constante para uma estrutura do tipo aeroporto
 * @param numero_voo - referência para string consante que contém o número de voo a ser verificado
 * @return true se o número de voo for único (não estiver presente em nenhuma lista de aviões)
 *         false se o número de voo não for único (estiver presente em alguma lista de aviões)
 */
bool numero_voo_unico(const aeroporto& aeroporto, const string& numero_voo) {

    // Verifica na lista de aproximação
    aviao* atual = aeroporto.avioes_aproximacao;
    while (atual != nullptr) {
        if (atual->voo == numero_voo) {
            return true; //Se o número de voo foi encontrado
        }
        atual = atual->next;
    }

    // Verifica na lista da pista
    atual = aeroporto.avioes_pista;
    while (atual != nullptr) {
        if (atual->voo == numero_voo) {
            return true; //Se o número de voo foi encontrado
        }
        atual = atual->next;
    }

    // Verifica na lista de descolagem
    atual = aeroporto.avioes_descolando;
    while (atual != nullptr) {
        if (atual->voo == numero_voo) {
            return true; //Se o número de voo foi encontrado
        }
        atual = atual->next;
    }

    return false; //Se o número de voo não foi encontrado
}

/**
 * Função que gera um avião aleatório selecionando aleatoriamente os detalhes do avião (número, modelo, capacidade, origem e número de passageiros)
 * Chama a função passageiro_aleatorio para gerar os passageiros necessários para o avião
 * @param array_voos - array de strings que contém os números dos voos
 * @param array_modelos - array de strings que contém os modelos dos aviões
 * @param array_origens - array de strings que contém as origens dos aviões
 * @param array_primeiros_nomes - array de strings que contém os primeiros nomes dos passageiros
 * @param array_segundos_nomes - array de strings que contém os primeiros nomes dos passageiros
 * @param array_nacionalidades - array de strings que contém os primeiros nomes dos passageiros
 * @param numero_bilhetes_gerados - referência para um inteiro com o número de bilhetes gerados
 * @param raiz - referência para o ponteiro para o nó raiz da árvore binária
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 * @return estrutura do tipo aviao que contém os detalhes atribuídos ao aviao gerado aleatoriamente
 */
aviao aviao_aleatorio(const string array_voos[], const string array_modelos[], const string array_origens[],
                      const string array_primeiros_nomes[], const string array_segundos_nomes[], const string array_nacionalidades[],
                      long long& numero_bilhetes_gerados, nodo_arvore_binaria*& raiz, aeroporto& aeroporto){

    string numero_voo;

    //Gera um número de voo aleatório único no aeroporto
    do {
        numero_voo = array_voos[rand() % contador_linhas("voo.txt")];
    } while (numero_voo_unico(aeroporto, numero_voo));

    // Criação do novo avião
    aviao novo_aviao;
    novo_aviao.voo = numero_voo;
    novo_aviao.modelo = array_modelos[rand() % contador_linhas("modelo.txt")];
    novo_aviao.origem = array_origens[rand() % contador_linhas("origem.txt")];
    novo_aviao.destino = "Aeroporto EDA";
    novo_aviao.capacidade = 5 + rand() % 11;
    novo_aviao.numero_passageiros = 0;
    novo_aviao.passageiros = nullptr;

    for (int i = 0; i < novo_aviao.capacidade; i++) {
        passageiro novo_passageiro = passageiro_aleatorio(numero_bilhetes_gerados, array_primeiros_nomes, array_segundos_nomes, array_nacionalidades, raiz);
        // Adiciona o novo passageiro à lista ligada do 'novo_avião'
        adicionar_passageiro(novo_aviao, novo_passageiro);
        novo_aviao.numero_passageiros++;
    }

    return novo_aviao;
}




// INICIALIZAÇÃO DO AEROPORTO

/**
 * Função para criar uma lista ligada com os estrangeiros que podem aparecer no aeroporto a partir de um array de strings com as nacionalidades
 * Recebe um array de strings contendo as nacionalidades e uma referência para um inteiro com o número total de nacionalidades no array
 *
 * @param array_nacionalidades - array de strings que contém as nacionalidades
 * @param numero_nacionalidades - referência para um inteiro com o número de nacionalidades
 * @return ponteiro para o primeiro elemento da lista ligada com os estrangeiros que podem aparecer no aeroporto
*/
nodo_lista_ligada* lista_ligada_nacionalidades_estrangeiras(string array_nacionalidades[], int& numero_nacionalidades) {

    // Cria o primeiro nó da lista ligada de nacionalidades estrangeiras
    nodo_lista_ligada* head = nullptr;
    nodo_lista_ligada* current = nullptr;

    // Itera sobre o array de nacionalidades
    for (int i = 0; i < numero_nacionalidades; i++) {

        // Ignora a nacionalidade portuguesa
        if (array_nacionalidades[i] == "Portuguese") {
            continue; // Passa para a próxima iteração sem executar o resto do código do loop
        }

        // Se o head for nullptr, cria o primeiro nó da lista ligada
        if (head == nullptr) {
            head = new nodo_lista_ligada(array_nacionalidades[i]);
            current = head;
        }

        // Se o head não for nullptr, cria um novo nó e liga ao nó atual
        else {
            current->next = new nodo_lista_ligada(array_nacionalidades[i]);
            current = current->next;
        }

    }
    return head; // Retorna um ponteiro para o primeiro nó da lista ligada
}

/**
 * Função para criar uma árvore binária com raiz em cada nacionalidade da lista ligada de nacionalidades estrangeiras
 * @param head - ponteiro para o primeiro elemento da lista ligada de nacionalidades estrangeiras
 * @param numero_nacionalidades - referência para um inteiro com o número de nacionalidades
 * @return um array de ponteiros para a raiz de cada árvore binária criada
 */
nodo_arvore_binaria** criar_arvores_nacionalidade(nodo_lista_ligada* head, int& numero_nacionalidades) {

    nodo_arvore_binaria** raizes = new nodo_arvore_binaria*[numero_nacionalidades-1];

    int i = 0;
    nodo_lista_ligada * current = head;
    while (current != nullptr) {
        raizes[i] = nullptr;
        raizes[i] = inserir_nodo_arvore(raizes[i], current->item);
        current = current->next;
        i++;
    }
    return raizes;
}

/**
 * Função que inicia o aeroporto vazio, sem nenhum avião em aproximação, em pista ou descolando
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 */
void inicializacao_aeroporto(aeroporto& aeroporto) {
    aeroporto.avioes_aproximacao = nullptr;
    aeroporto.avioes_pista = nullptr;
    aeroporto.avioes_descolando = nullptr;
    aeroporto.ultimo_aviao_aproximacao = nullptr;
    aeroporto.ultimo_aviao_pista = nullptr;
    aeroporto.ultimo_aviao_descolando = nullptr;
    aeroporto.numero_avioes_partiram = 0;
}




// GERENCIAMENTO DOS AVIÕES E PASSAGEIROS

/**
 * Função responsável por adicionar um passageiro à lista ligada dos passageiros que se encontram em um dado avião
 * @param aviao - referência para uma estrutura do tipo avião
 * @param novo_passageiro - parâmetro que representa o novo passageiro a ser adicionado à lista ligada
 */
void adicionar_passageiro(aviao& aviao, passageiro novo_passageiro){

    // Alocação de memória para o novo passageiro e atribuição de informações aleatórias ao novo passageiro
    passageiro* novo_no_passageiro = new passageiro;
    *novo_no_passageiro = novo_passageiro;
    novo_no_passageiro->next = nullptr; // O apontador do novo nó de passageiro será NULL

    // Se a lista de passageiros estiver vazia, o novo passageiro será o primeiro e o último na lista
    if (aviao.passageiros == nullptr) {
        aviao.passageiros = novo_no_passageiro;
    }

        // Se a lista não estiver vazia, o novo passageiro será o último da lista
    else {
        // Encontra o último nó na lista de passageiros
        passageiro* ultimo = aviao.passageiros;
        while (ultimo->next != nullptr) {
            ultimo = ultimo->next;
        }
        // Adiciona o novo nó como o próximo do último nó
        ultimo->next = novo_no_passageiro;
    }
    aviao.ultimo_passageiro = novo_no_passageiro;
}

/**
 * Função responsável por adicionar um avião à lista ligada dos aviões que estão se aproximando do aeroporto
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 * @param array_voos - array de strings que contém os números dos voos
 * @param array_modelos - array de strings que contém os modelos dos aviões
 * @param array_origens - array de strings que contém as origens dos aviões
 * @param array_primeiros_nomes - array de strings que contém os primeiros nomes dos passageiros
 * @param array_segundos_nomes - array de strings que contém os primeiros nomes dos passageiros
 * @param array_nacionalidades - array de strings que contém os primeiros nomes dos passageiros
 * @param numero_bilhetes_gerados - referência para um inteiro com o número de bilhetes gerados
 * @param raiz - referência para o ponteiro para o nó raiz da árvore binária
 * @param numero_avioes_aproximacao - referência para um inteiro com o número de aviões em aproximação
 * @param numero_avioes_aeroporto - referência para um inteiro que contém o número total de aviões do aeroporto
 * @param numero_voos - referência para um inteiro com o número de voos
 */
void adicionar_aproximacao(aeroporto& aeroporto, const string array_modelos[], const string array_voos[], const string array_origens[],
                           const string array_primeiros_nomes[], const string array_segundos_nomes[], const string array_nacionalidades[],
                           int& numero_avioes_aproximacao, long long &numero_bilhetes_gerados, nodo_arvore_binaria*& raiz, int& numero_avioes_aeroporto,
                           int& numero_voos){


    // Alocação de memória para o novo avião e atribuição de informações aleatórias ao novo aviao
    aviao* novo_aviao = new aviao;
    *novo_aviao = aviao_aleatorio(array_voos, array_modelos, array_origens, array_primeiros_nomes, array_segundos_nomes,
                                  array_nacionalidades,numero_bilhetes_gerados, raiz, aeroporto);

    // Se a lista estiver vazia, o novo avião será o primeiro e o último na lista
    if (aeroporto.avioes_aproximacao == nullptr) {
        aeroporto.avioes_aproximacao = novo_aviao;
        aeroporto.ultimo_aviao_aproximacao = novo_aviao;
        novo_aviao->next = nullptr;
        novo_aviao->prev = nullptr;
    }

        // Se a lista não estiver vazia, o novo avião será o ultimo da lista
    else {
        aeroporto.ultimo_aviao_aproximacao->next = novo_aviao;
        novo_aviao->prev = aeroporto.ultimo_aviao_aproximacao;
        aeroporto.ultimo_aviao_aproximacao = novo_aviao;
        novo_aviao->next = nullptr;
    }

    numero_avioes_aproximacao += 1; // Incrementa o número de aviões em aproximação
    numero_avioes_aeroporto += 1; // Incrementa o número de aviões no aeroporto

    // Se o número de voos for inferior ao número de aviões do aeroporto
    if (numero_avioes_aeroporto>=numero_voos){
        cout << "Nao existem mais voos diferentes para o numero de avioes do aeroporto." << endl;
        exit (1);
    }
}

/**
 * Função responsável por remover os passageiros dos aviões que chegaram à lista ligada da pista
 * @param aviao - referência para uma estrutura do tipo aviao
 */
void esvaziarPassageiros(aviao& aviao) {
    nodo_lista_ligada* atual = reinterpret_cast<nodo_lista_ligada *>(aviao.passageiros);
    while (atual != nullptr) {
        nodo_lista_ligada* proximo = atual->next;
        delete atual; // Libera a memória ocupada pelo nó atual
        atual = proximo; // Move para o próximo nó
    }
    aviao.passageiros = nullptr; // Atualiza a cabeça da lista para indicar que está vazia
}

/**
 * Função responsável por mover um avião da lista ligada de aproximação para a lista ligada da pista
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 * @param numero_avioes_aproximacao - referência para um inteiro com o número de aviões em aproximação
 * @param array_voos - array de strings que contém os números dos voos
 * @param array_destinos - array de strings que contém os destinos dos voos
 * @param numero_avioes_pista - referência para um inteiro com o número de aviões em pista
 * @param arvores_nacionalidade - ponteiro que aponta para um array de ponteiros para o nó raiz da árvore binária das nacionalidades
 * @param numero_nacionalidades - referência para um inteiro com o número de nacionalidades
 */
void mover_aproximacao_pista(aeroporto& aeroporto, int& numero_avioes_aproximacao, const string array_voos[],
                             const string array_destinos[], int& numero_avioes_pista, nodo_arvore_binaria**& arvores_nacionalidade,
                             int numero_nacionalidades) {

    // Verifica se há pelo menos um avião na lista de aproximação e se está vazia
    if (numero_avioes_aproximacao >= 1 && aeroporto.avioes_aproximacao != nullptr) {
        // Encontra o primeiro avião na lista de aproximação
        aviao* aviao_aproximacao = aeroporto.avioes_aproximacao;

        // Encontra o último avião na lista da pista
        aviao* ultimo_aviao_pista = aeroporto.ultimo_aviao_pista;

        // Remove o avião da lista de aproximação
        if (aviao_aproximacao->next != nullptr) {
            aviao_aproximacao->next->prev = nullptr;
            aeroporto.avioes_aproximacao = aviao_aproximacao->next;
        }
        else {
            aeroporto.avioes_aproximacao = nullptr;
            aeroporto.ultimo_aviao_aproximacao = nullptr;
        }

        // Adiciona o avião no final da lista da pista
        if (ultimo_aviao_pista == nullptr) {
            aeroporto.avioes_pista = aviao_aproximacao;
            aeroporto.ultimo_aviao_pista = aviao_aproximacao;
        } else {
            ultimo_aviao_pista->next = aviao_aproximacao;
            aviao_aproximacao->prev = ultimo_aviao_pista;
        }
        aeroporto.ultimo_aviao_pista = aviao_aproximacao;
        aviao_aproximacao->next = nullptr;

        // Para cada passageiro do avião, adiciona à árvore binária correspondente à sua nacionalidade
        passageiro* passageiro_atual = aviao_aproximacao->passageiros;
        while (passageiro_atual != nullptr) {
            // Encontra a árvore binária correspondente à nacionalidade do passageiro
            for (int i = 0; i < numero_nacionalidades-1; ++i) {
                if (passageiro_atual->nacionalidade == arvores_nacionalidade[i]->item) {
                    // Insere o passageiro na árvore binária correspondente
                    arvores_nacionalidade[i] = inserir_nodo_arvore(arvores_nacionalidade[i], passageiro_atual->primeiro_nome);
                    break;
                }
            }
            passageiro_atual = passageiro_atual->next;
        }

        /*

        esvaziarPassageiros(*aviao_aproximacao); // esvaziar aviao

        // Gerar novos passageiros e adicionar ao avião
        for (int i = 0; i < aviao_aproximacao->capacidade; ++i) {
                // Gerar passageiro aleatório
                passageiro novo_passageiro = passageiro_aleatorio(numero_bilhetes_gerados, array_primeiros_nomes, array_segundos_nomes, array_nacionalidades, raiz);
                adicionar_passageiro(*aviao_aproximacao, novo_passageiro);
            }

         */

        // Atualiza os detalhes do avião e o número de aviões nas listas
        aviao_aproximacao->voo = array_voos[rand() % contador_linhas("voo.txt")];
        aviao_aproximacao->origem = "Aeroporto EDA";
        aviao_aproximacao->destino = array_destinos[rand() % contador_linhas("destino.txt")];
        numero_avioes_aproximacao -= 1; // Decrementa o número de aviões em aproximação
        numero_avioes_pista += 1; // Incrementa o número de aviões em pista
    }
}

/**
 * Função responsável por mover um avião da lista ligada da pista para a lista ligada descolando
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 * @param numero_avioes_pista - referência para um inteiro com o número de aviões em pista
 * @param numero_avioes_descolando - referência para um inteiro com o número de aviões descolando
 */
void mover_pista_descolagem(aeroporto& aeroporto, int& numero_avioes_pista, int& numero_avioes_descolando) {

    //Verifica de a lista da pista está vazia
    if (aeroporto.avioes_pista != nullptr) {
        // Encontra o primeiro avião na lista da pista
        aviao* aviao_pista = aeroporto.avioes_pista;

        // Encontra o último avião na lista de decolagem
        aviao* ultimo_aviao_descolagem = aeroporto.ultimo_aviao_descolando;

        // Remove o avião da lista da pista
        if (aviao_pista->next != nullptr) {
            aviao_pista->next->prev = nullptr;
            aeroporto.avioes_pista = aviao_pista->next;
        } else {
            aeroporto.avioes_pista = nullptr;
        }

        // Adiciona o avião no final da lista de descolagem
        if (ultimo_aviao_descolagem == nullptr) {
            aeroporto.avioes_descolando = aviao_pista;
        } else {
            ultimo_aviao_descolagem->next = aviao_pista;
            aviao_pista->prev = ultimo_aviao_descolagem;
        }
        aeroporto.ultimo_aviao_descolando = aviao_pista;
        aviao_pista->next = nullptr;

        // Atualiza o número de aviões nas listas
        numero_avioes_pista -= 1; // Decrementa o número de aviões em pista
        numero_avioes_descolando += 1; // Incrementa o número de aviões descolando
    }
}

/**
 * Função responsável por remover um avião da descolagem
 * @param permissao_remocao - referência para um inteiro que indica a permissao de remocao
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 * @param numero_avioes_descolando - referência para um inteiro com o número de aviões descolando
 * @param numero_avioes_aeroporto - referência para um inteiro que contém o número total de aviões do aeroporto
 */
void remover_descolagem(aeroporto& aeroporto, int& permissao_remocao, int& numero_avioes_descolando, int& numero_avioes_aeroporto) {
    if (aeroporto.avioes_descolando != nullptr && permissao_remocao == 1) {
        aviao* proximo_descolar = aeroporto.avioes_descolando->next;
        delete aeroporto.avioes_descolando;
        aeroporto.avioes_descolando = proximo_descolar;
        permissao_remocao = 0;
        numero_avioes_descolando -= 1; //Decrementa o número de aviões descolando
        numero_avioes_aeroporto -= 1; //Decrementa o número de aviões no aeroporto
    }
}





// ESTADO DO AEROPORTO

/**
 * Função que recebe duas referência para uma estrutura e para um inteiro
 * Exibe o estado atual do aeroporto, incluindo informações sobre os aviões em aproximação, na pista e a descolar
 * (número de voo, modelo, origem, destino, capacidade, primeiro e segundo nome e número de bilhete dos passageiros)
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 * @param numero_avioes_aeroporto - referência para um inteiro que contém o número de aviões do aeroporto
 */
void estado_aeroporto(aeroporto & aeroporto, int& numero_avioes_aeroporto) {
    cout << "Total de avioes: " << numero_avioes_aeroporto << endl;
    cout << "---------------" << endl;
    cout << "Em aproximacao" << endl;
    cout << "---------------" << endl;
    aviao* atual = aeroporto.ultimo_aviao_aproximacao;
    aviao* inicio_aviao = aeroporto.avioes_aproximacao;

    for (; atual != nullptr; atual = atual->prev) {
        cout << "Voo " << atual->voo << endl;
        cout << "Modelo: " << atual->modelo << endl;
        cout << "Origem: " << atual->origem << endl;
        cout << "Destino: " << atual->destino << endl;
        cout << "Capacidade: " << atual->capacidade << endl;
        cout << "Passageiros: ";
        passageiro* passageiro_atual = atual->passageiros;
        while (passageiro_atual != nullptr) {
            cout << passageiro_atual->primeiro_nome << " "
                 << passageiro_atual->segundo_nome << " ("
                 << passageiro_atual->numeros_bilhete << " | "
                 << passageiro_atual->nacionalidade << ")";
            if (passageiro_atual->next != nullptr) cout << ", ";
            passageiro_atual = passageiro_atual->next;
        }
        cout << endl;
        cout << "." << endl;
        cout << "." << endl;
        cout << "." << endl;

        if (atual == inicio_aviao) break;
    }

    cout << "---------------" << endl;
    cout << "Na pista" << endl;
    cout << "---------------" << endl;
    atual = aeroporto.ultimo_aviao_pista;
    inicio_aviao = aeroporto.avioes_pista;
    for (; atual != nullptr; atual = atual->prev) {
        cout << "Voo " << atual->voo << endl;
        cout << "Modelo: " << atual->modelo << endl;
        cout << "Origem: " << atual->origem << endl;
        cout << "Destino: " << atual->destino << endl;
        cout << "Capacidade: " << atual->capacidade << endl;
        cout << "Passageiros: ";
        passageiro* passageiro_atual = atual->passageiros;
        while (passageiro_atual != nullptr) {
            cout << passageiro_atual->primeiro_nome << " "
                 << passageiro_atual->segundo_nome << " ("
                 << passageiro_atual->numeros_bilhete << " | "
                 << passageiro_atual->nacionalidade << ")";
            if (passageiro_atual->next != nullptr) cout << ", ";
            passageiro_atual = passageiro_atual->next;
        }
        cout << endl;
        cout << "." << endl;
        cout << "." << endl;
        cout << "." << endl;

        if (atual == inicio_aviao) break;
    }

    cout << "---------------" << endl;
    cout << "A descolar" << endl;
    cout << "---------------" << endl;
    atual = aeroporto.ultimo_aviao_descolando;
    inicio_aviao = aeroporto.avioes_descolando;
    for (; atual != nullptr; atual = atual->prev) {
        cout << "Voo " << atual->voo << endl;
        cout << "Modelo: " << atual->modelo << endl;
        cout << "Origem: " << atual->origem << endl;
        cout << "Destino: " << atual->destino << endl;
        cout << "Capacidade: " << atual->capacidade << endl;
        cout << "Passageiros: ";
        passageiro* passageiro_atual = atual->passageiros;
        while (passageiro_atual != nullptr) {
            cout << passageiro_atual->primeiro_nome << " "
                 << passageiro_atual->segundo_nome << " ("
                 << passageiro_atual->numeros_bilhete << " | "
                 << passageiro_atual->nacionalidade << ")";
            if (passageiro_atual->next != nullptr) cout << ", ";
            passageiro_atual = passageiro_atual->next;
        }
        cout << endl;
        cout << "." << endl;
        cout << "." << endl;
        cout << "." << endl;

        if (atual == inicio_aviao) break;
    }
}



//FUNÇÕES DISPONÍVEIS

/**
 *  Função que rece um ponteiro para uma lista ligada e uma referência para um string com o número de voo
 *  Procura um dado avião com base no número de voo
 *
 * @param lista_avioes - ponteiro para o início da lista ligada de aviões
 * @param numero_voo - referência para uma string com o número do voo que se pretende encontrar
 * @return atual (ponteiro para o avião com o número de voo desejado) ou nullptr (caso não tenha encontrado o voo pretendido)
 */
aviao* encontrar_aviao_por_numero_voo(aviao* lista_avioes, const string& numero_voo) {
    aviao* atual = lista_avioes;
    while (atual != nullptr) {
        if (atual->voo == numero_voo) {
            return atual;
        }
        atual = atual->next;
    }
    return nullptr;
}

/**
 * Função que recebe uma referência para uma estrutura
 * Exibe todos os passageiros existentes na lista ligada de aproximação
 * Inclui as seguintes informações dos passageiros: primeiro e segundo nome, número de bilhete e nacionalidade
 *
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 */
void mostrar_passageiros_na_aproximacao(const aeroporto& aeroporto) {
    cout << "Lista de passageiros na lista de aproximacao :" << endl<< endl;

    aviao* aviao_atual = aeroporto.avioes_aproximacao;
    while (aviao_atual != nullptr) {
        passageiro* passageiro_atual = aviao_atual->passageiros;
        while (passageiro_atual != nullptr) {
            cout << passageiro_atual->primeiro_nome << " "<< passageiro_atual->segundo_nome << " ("
                 << passageiro_atual->numeros_bilhete << " | " << passageiro_atual->nacionalidade << ")" << endl;
            passageiro_atual = passageiro_atual->next;
        }

        aviao_atual = aviao_atual->next;
    }
    cout <<"_________________" << endl;
}

/**
 * Função que recebe uma referência para uma estrutura
 * Exibe todos os passageiros existentes na lista ligada da pista
 * Inclui as seguintes informações dos passageiros: primeiro e segundo nome, número de bilhete e nacionalidade
 *
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 */
void mostrar_passageiros_na_pista(const aeroporto& aeroporto) {

    aviao *aviao_atual = aeroporto.avioes_pista;
    if (aviao_atual == nullptr) {
        cout << "Nao ha passageiros na pista." << endl;
    }
    cout << "Lista de passageiros na lista da pista :" << endl;
    while (aviao_atual != nullptr) {

        passageiro *passageiro_atual = aviao_atual->passageiros;
        while (passageiro_atual != nullptr) {
            cout << passageiro_atual->primeiro_nome << " " << passageiro_atual->segundo_nome << " ("
                 << passageiro_atual->numeros_bilhete << " | " << passageiro_atual->nacionalidade << ")" << endl;
            passageiro_atual = passageiro_atual->next;
        }

        aviao_atual = aviao_atual->next;
    }
    cout << "_________________" << endl;

}

/**
 * Função que recebe uma referência para uma estrutura
 * Exibe todos os passageiros existentes na lista ligada da descolagem
 * Inclui as seguintes informações dos passageiros: primeiro e segundo nome, número de bilhete e nacionalidade
 *
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 */
void mostrar_passageiros_na_descolagem(const aeroporto& aeroporto) {

    aviao* aviao_atual = aeroporto.avioes_descolando;
    if(aviao_atual== nullptr){
        cout << "Nao ha avioes nas descolagens"<< endl;
        return;
    }
    cout << "Lista de passageiros na lista de descolagem :" << endl<< endl;
    while (aviao_atual != nullptr) {
        passageiro* passageiro_atual = aviao_atual->passageiros;
        while (passageiro_atual != nullptr) {
            cout << passageiro_atual->primeiro_nome << " " << passageiro_atual->segundo_nome << " ("
                 << passageiro_atual->numeros_bilhete << " | " << passageiro_atual->nacionalidade << ")" << endl;
            passageiro_atual = passageiro_atual->next;
        }

        aviao_atual = aviao_atual->next;
    }
    cout <<"_________________" << endl;
}

/**
 * Função que recebe duas referências para uma estrutura e para uma string
 * Pesquisa por um passageiro na lista ligada de aproximação com base no primeiro nome que é nos fornecido
 *
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 * @param primeiro_nome - referência para uma string com o primeiro nome de um passageiro
 */
void pesquisar_passageiro_por_nome_chegadas(const aeroporto& aeroporto, const string& primeiro_nome) {

    bool passageiro_encontrado = false;

    // Percorre a lista de aviões em aproximação
    aviao* aviao_chegada_atual = aeroporto.avioes_aproximacao;
    while (aviao_chegada_atual != nullptr) {
        // Percorre a lista de passageiros do avião
        passageiro* passageiro_atual = aviao_chegada_atual->passageiros;
        while (passageiro_atual != nullptr) {
            // Verifica se o primeiro nome corresponde ao nome fornecido
            if (passageiro_atual->primeiro_nome == primeiro_nome) {

                // Imprime os detalhes do passageiro encontrado
                cout << "Passageiro " << primeiro_nome << " encontrado no voo de chegada " << aviao_chegada_atual->voo << "." << endl;
                cout << "Nacionalidade: " << passageiro_atual->nacionalidade << endl;
                cout << "Numero de bilhete: " << passageiro_atual->numeros_bilhete << endl;
                cout <<"__________________" << endl;
                passageiro_encontrado = true; // Indica que o passageiro foi encontrado
            }
            passageiro_atual = passageiro_atual->next; // Avança para o próximo passageiro na lista
        }
        aviao_chegada_atual = aviao_chegada_atual->next; // Avança para o próximo avião na lista de aproximação
    }
    // Se o passageiro não for encontrado
    if (!passageiro_encontrado) {
        cout << "Passageiro com o primeiro nome " << primeiro_nome << " nao encontrado em nenhum voo." << endl;
    }
}

/**
 * Função que recebe duas referências para uma estrutura e para uma string
 * Pesquisa por um passageiro na lista ligada da descolagem com base no primeiro nome que é nos fornecido
 *
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 * @param primeiro_nome - referência para uma string com o primeiro nome de um passageiro
 */
void pesquisar_passageiro_por_nome_descolagens(const aeroporto& aeroporto, const string& primeiro_nome) {
    bool passageiro_encontrado = false;
    // Percorre a lista de aviões em descolagem
    aviao* aviao_partida_atual = aeroporto.avioes_descolando;
    while (aviao_partida_atual != nullptr) {
        // Percorre a lista de passageiros do avião
        passageiro* passageiro_atual = aviao_partida_atual->passageiros;
        while (passageiro_atual != nullptr) {
            // Verifica se o primeiro nome corresponde ao nome fornecido
            if (passageiro_atual->primeiro_nome == primeiro_nome) {
                // Imprime os detalhes do passageiro encontrado
                cout << "Passageiro " << primeiro_nome << " encontrado no voo de partida " << aviao_partida_atual->voo << "." << endl;
                cout << "Nacionalidade: " << passageiro_atual->nacionalidade << endl;
                cout << "Numero de bilhete: " << passageiro_atual->numeros_bilhete << endl;
                cout <<"__________________" << endl;
                passageiro_encontrado = true; // Indica que o passageiro foi encontrado
            }
            passageiro_atual = passageiro_atual->next; // Avança para o próximo passageiro na lista
        }
        aviao_partida_atual = aviao_partida_atual->next; // Avança para o próximo avião na lista de aproximação
    }
    // Se o passageiro não for encontrado
    if (!passageiro_encontrado) {
        cout << "Passageiro com o primeiro nome " << primeiro_nome << " nao encontrado em nenhum voo." << endl;
    }
}

/**
 * Função que recebe três referências para uma estrutura e para três strings
 * Edita a nacionalidade de um dado passageiro para uma nacionalidade fornecida pelo utilizador
 *
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 * @param primeiro_nome - referência para uma string com o primeiro nome de um passageiro
 * @param segundo_nome - referência para uma string com o segundo nome de um passageiro
 * @param nova_nacionalidade - referência para uma string com uma dada nacionalidade
 */
void editar_nacionalidade_por_nome(const aeroporto& aeroporto, const string& primeiro_nome, const string&segundo_nome , const string& nova_nacionalidade) {

    //Percorre a lista de aviões em aproximação
    aviao* aviao_atual = aeroporto.avioes_aproximacao;
    while (aviao_atual != nullptr) {
        //Percorre a lista de passageiros do avião
        passageiro* passageiro_atual = aviao_atual->passageiros;
        while (passageiro_atual != nullptr) {
            // Verifica se o nome completo do passageiro corresponde ao nome fornecido
            if ((passageiro_atual->primeiro_nome == primeiro_nome) && (passageiro_atual->segundo_nome==segundo_nome)) {
                // Atualiza a nacionalidade do passageiro
                passageiro_atual->nacionalidade = nova_nacionalidade;
                cout << "Nacionalidade do passageiro " << primeiro_nome << " " << segundo_nome << " foi atualizada para " << nova_nacionalidade << "." << endl;
                cout << "O passageiro "<< primeiro_nome<< " " << segundo_nome << " esta no voo em aproximacao -> " << aviao_atual->voo<< endl;
                cout <<"____________________" << endl;
                return;
            }
            passageiro_atual = passageiro_atual->next; // Avança para o próximo passageiro na lista
        }
        aviao_atual = aviao_atual->next; // Avança para o próximo avião na lista de aproximação
    }
    // Se o passageiro não for encontrado
    cout << "Passageiro com nome " << primeiro_nome  << " " << segundo_nome << " nao encontrado em nenhum voo das chegadas." << endl;
}

/**
 * Função que recebe uma referência para uma string e um apontador para o início de uma lista ligada
 * Organiza os passageiros por nacionalidade e os ordena alfabeticamente
 *
 * @param nacionalidade - referência para uma string com uma nacionalidade
 * @param avioes_pista - apontador para o início da lista ligada da pista
 */
void ordena_passageiros_na_pista_por_nacionalidade(const string& nacionalidade, aviao* avioes_pista) {
    passageiro* primeiro_passageiro = nullptr;// Ponteiro para o início da lista de passageiros que vao ser ordenados
    // Percorre a lista de aviões na pista
    aviao* aviao_atual = avioes_pista;
    while (aviao_atual != nullptr) {
        //Percorre a lista de passageiros do avião atual
        passageiro* passageiro_atual = aviao_atual->passageiros;
        while (passageiro_atual != nullptr) {
            // Verifica se a nacionalidade do passageiro corresponde à nacionalidade fornecida
            if (passageiro_atual->nacionalidade == nacionalidade) {
                // Cria um novo nó de passageiro com os dados do passageiro atual
                passageiro* novo_passageiro = new passageiro(*passageiro_atual);
                novo_passageiro->next = nullptr;
                // Insere no início da lista se for o primeiro ou se o nome for menor que o primeiro nome atual
                if (primeiro_passageiro == nullptr || novo_passageiro->primeiro_nome < primeiro_passageiro->primeiro_nome) {
                    novo_passageiro->next = primeiro_passageiro;
                    primeiro_passageiro = novo_passageiro;
                } else { // Insere no meio ou no final da lista
                    passageiro* atual = primeiro_passageiro;
                    while (atual->next != nullptr && atual->next->primeiro_nome < novo_passageiro->primeiro_nome) {
                        atual = atual->next;
                    }
                    novo_passageiro->next = atual->next;
                    atual->next = novo_passageiro;
                }
            }
            passageiro_atual = passageiro_atual->next; // Avança para o próximo passageiro
        }
        aviao_atual = aviao_atual->next; // Avança para o próximo avião
    }

    // Imprime a lista de passageiros ordenados
    if (primeiro_passageiro != nullptr) {
        cout << "Nacionalidade " << nacionalidade << ":" << endl;
        passageiro* atual = primeiro_passageiro;
        while (atual != nullptr) {
            cout << "- " << atual->primeiro_nome << " " << atual->segundo_nome << endl;
            passageiro* temp = atual;
            atual = atual->next;
            delete temp;
        }
        cout << "________________________________";
        cout << endl;
    }
}

/**
 * Função que recebe uma referência para uma estrutura, um apontador para um array de apontadores e um inteiro
 * Mostra todos os passageiros em pista, ordenados por nacionalidade e ordem alfabética
 *
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 * @param arvores_nacionalidade - apontador para um array de apontadores para o nó de uma árvore binária
 * @param numero_nacionalidades - inteiro com o número de nacionalidades existentes
 */
void mostrar_passageiros_na_pista_ordenados_alfabetica(const aeroporto& aeroporto, nodo_arvore_binaria** arvores_nacionalidade, int numero_nacionalidades) {

    for (int i = 0; i < numero_nacionalidades; ++i) {
        nodo_arvore_binaria* raiz_nacionalidade = arvores_nacionalidade[i];
        if (raiz_nacionalidade != nullptr) {
            ordena_passageiros_na_pista_por_nacionalidade(raiz_nacionalidade->item, aeroporto.avioes_pista);
        }
    }
}

/**
 * Função que recebe uma referência para uma estrutura
 * Inverte a lista ligada da pista
 * O avião que estaria na última posição da fila que seria o último a descolar passa para o início da e será o próximo a descolar
 * (esta prioridade repete-se com os restantes avioes em pista)

 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 */
void inverter_lista_pista(aeroporto& aeroporto) {
    // Verifica se a lista está vazia ou tem apenas um elemento
    if (aeroporto.avioes_pista == nullptr || aeroporto.avioes_pista->next == nullptr) {
        return; // Não há necessidade de inverter
    }

    aviao* atual = aeroporto.avioes_pista;
    aviao* temp = nullptr;

    while (atual != nullptr) {
        temp = atual->prev; // Armazena o ponteiro 'prev' do avião atual em 'temp'
        atual->prev = atual->next; // O ponteiro 'prev' do avião atual passa a apontar para 'next'
        atual->next = temp; // O ponteiro 'next' do avião atual passa a apontar para 'temp'
        atual = atual->prev; // Avança para o próximo avião na lista
    }
    
    if (temp != nullptr) {
        aeroporto.avioes_pista = temp->prev; // Atualiza o início da lista para ser o primeiro avião da lista invertida
    }
    
    aeroporto.ultimo_aviao_pista = aeroporto.avioes_pista; // Atualiza o ponteiro 'ultimo_aviao_pista' para apontar para o último avião da lista invertida
    while (aeroporto.ultimo_aviao_pista->next != nullptr) {
        aeroporto.ultimo_aviao_pista = aeroporto.ultimo_aviao_pista->next;
    }
}

/**
 * Função que recebe dois ponteiros
 * Insere um novo passageito numa árvore de pesquisa binária
 * 
 * @param raiz - ponteiro para a raiz da árvore de pesquisa binária
 * @param novo_passageiro - ponteiro para um objeto 'passageiro'
 * @return ponteiro para um nó da árvore binária
 */
nodo_arvore_binaria* inserir_passageiro(nodo_arvore_binaria* raiz, passageiro* novo_passageiro) {
    if (raiz == nullptr) {
        raiz = new nodo_arvore_binaria();
        raiz->item = novo_passageiro->nacionalidade;
        raiz->passageiros = novo_passageiro;
        raiz->esq = nullptr;
        raiz->dir = nullptr;
    } else {
        if (novo_passageiro->primeiro_nome + " " + novo_passageiro->segundo_nome <
            raiz->passageiros->primeiro_nome + " " + raiz->passageiros->segundo_nome) {
            raiz->esq = inserir_passageiro(raiz->esq, novo_passageiro);
            } else {
                raiz->dir = inserir_passageiro(raiz->dir, novo_passageiro);
            }
    }
    return raiz;
}

/**
 * Função que recebe uma referência para uma string e um apontador para o início de uma lista ligada
 * Cria uma árvore de passageiros por nacionalidade
 * 
 * @param nacionalidade - referência para uma string com uma nacionalidade
 * @param avioes_pista - apontador para o início da lista ligada da pista
 * @return ponteiro para a raiz de uma árvore de pesquisa binária de passageiros
 */
nodo_arvore_binaria* criar_arvore_passageiros_por_nacionalidade(const string& nacionalidade, aviao* avioes_pista) {
    nodo_arvore_binaria* raiz = nullptr;
    aviao* aviao_atual = avioes_pista;

    // Percorre a lista de aviões na pista
    while (aviao_atual != nullptr) {
        passageiro* passageiro_atual = aviao_atual->passageiros;
        // Percorrer a lista de passageiros do avião atual
        while (passageiro_atual != nullptr) {
            if (passageiro_atual->nacionalidade == nacionalidade) { // Verifica se a nacionalidade do passageiro é igual à nacionalidade fornecida
                raiz = inserir_passageiro(raiz, passageiro_atual); // Insere o passageiro na árvore
            }
            passageiro_atual = passageiro_atual->next; // Avança para o próximo passageiro na lista
        }
        aviao_atual = aviao_atual->next; // Avança para o próximo avião na lista
    }
    return raiz;
}

/**
 * Função que recebe uma referência para uma estrutura, um apontador para um array de apontadores e um inteiro
 * Mostra os passageiros na pista, organizados por nacionalidade, usando árvores de pesquisa binária
 *
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 * @param arvores_nacionalidade - apontador para um array de apontadores para o nó de uma árvore binária
 * @param numero_nacionalidades - inteiro com o número de nacionalidades existentes
 */
void mostrar_passageiros_na_pista_arvore(const aeroporto& aeroporto, nodo_arvore_binaria** arvores_nacionalidade, int numero_nacionalidades) {
    for (int i = 0; i < numero_nacionalidades; ++i) {
        nodo_arvore_binaria* raiz_nacionalidade = arvores_nacionalidade[i]; // Obtém a raiz da árvore de nacionalidade atual
        if (raiz_nacionalidade != nullptr) {
            // Cria uma árvore de passageiros por nacionalidade para os passageiros na pista
            nodo_arvore_binaria* arvore_passageiros = criar_arvore_passageiros_por_nacionalidade(raiz_nacionalidade->item, aeroporto.avioes_pista);
            if (arvore_passageiros != nullptr) {
                cout << "Nacionalidade " << raiz_nacionalidade->item << ":" << endl << endl; // Imprime a nacionalidade atual
                imprimeArvore("", arvore_passageiros, false); // Imprime a árvore de passageiros por nacionalidade
                cout << "________________________________" << endl;
            }
        }
    }
}

/**
 * Função que recebe uma referência para uma string, um apontador para um nó e um booleano
 * Responsável pela impressão de uma árvore binária de passageiros
 *
 * @param prefix - referência para uma string que representa o prefixo a ser impresso antes de cada nó da árvore
 * @param no - apontador para o nó da árvore de pesquisa binária de passageiros
 * @param esquerda - booleano que indica se o nó atual é o filho à esquerda ou à direita do seu pai
 */
void imprimeArvore(const string& prefix, nodo_arvore_binaria* no, bool esquerda) {
    if (no != nullptr) {
        cout << prefix;
        cout << (esquerda ? "- " : "|- ");
        cout << no->passageiros->primeiro_nome + " " + no->passageiros->segundo_nome << endl;
        imprimeArvore(prefix + (esquerda ? "|   " : "    "), no->esq, true);
        imprimeArvore(prefix + (esquerda ? "|   " : "    "), no->dir, false);
    }
}

// SIMULAÇÃO DOS CICLOS

/**
 * Função que recebe sete arrays, sete referências para seis inteiros e uma esturtura e um ponteiro para o nó da raiz da árvore binária
 * Realiza um ciclo de simulação para o aeroporto, gerenciando a chegada e partida de aviões:
 * - Adição de aviões em aproximação no primeiro dia de operação até atingir 10 aviões
 * - Remoção de aviões da lista de descolagem quando atinge o limite de 5 aviões
 * - Movimento de aviões da lista de pista para a lista de descolagem quando atinge o limite de 7 aviões
 * - Movimento de aviões da lista de aproximação para a lista de pista quando atinge o limite de 10 aviões em aproximação
 * Chama a funcao aviao_aleatorio que atribui informações de forma aleatória aos aviões gerados nos ciclos
 * @param dia - referência para um inteiro que contém o dia desde o início do funcionamento do aeroporto
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 * @param array_voos - array de strings que contém os números dos voos
 * @param array_modelos - array de strings que contém os modelos dos aviões
 * @param array_origens - array de strings que contém as origens dos aviões
 * @param array_destinos - array de strings que contém os destinos dos voos
 * @param array_primeiros_nomes - array de strings que contém os primeiros nomes dos passageiros
 * @param array_segundos_nomes - array de strings que contém os primeiros nomes dos passageiros
 * @param array_nacionalidades - array de strings que contém os primeiros nomes dos passageiros
 * @param numero_avioes_aproximacao - referência para um inteiro com o número de aviões em aproximação
 * @param permissao_remocao - referência para um inteiro que indica a permissao de remocao
 * @param numero_avioes_pista - referência para um inteiro com o número de aviões em pista
 * @param numero_avioes_descolando - referência para um inteiro com o número de aviões descolando
 * @param numero_bilhetes_gerados - referência para inteiro com o número total de bilhetes gerados
 * @param raiz - ponteiro para o nó raiz da árvore binária
 * @param numero_avioes_aeroporto - referência para um inteiro que contém o número total de aviões do aeroporto
 * @param numero_voos - referência para um inteiro com o número de voos
 * @param arvores_nacionalidade - ponteiro que aponta para um array de ponteiros para o nó raiz da árvore binária das nacionalidades
 * @param numero_nacionalidades - referência para um inteiro com o número de nacionalidades
 */
void simulacao_ciclo(int& dia, aeroporto& aeroporto, string array_modelos[], string array_voos[], string array_origens[],
                     string array_destinos[], string array_primeiros_nomes[], string array_segundos_nomes[],
                     string array_nacionalidades[],  int& numero_avioes_aproximacao, int& permissao_remocao,
                     int& numero_avioes_pista, int& numero_avioes_descolando, long long &numero_bilhetes_gerados, nodo_arvore_binaria*& raiz,
                     int& numero_avioes_aeroporto, int& numero_voos, nodo_arvore_binaria**& arvores_nacionalidade,
                     int numero_nacionalidades, bool aeroporto_fechado) {

    // Para o primeiro dia: adiciona aviões em aproximação até atingir 10
    if (dia == 0) {
        while (numero_avioes_aproximacao < 10) {
            aviao* aviao_ciclo = new aviao;
            *aviao_ciclo = aviao_aleatorio(array_voos, array_modelos, array_origens, array_primeiros_nomes,
                                     array_segundos_nomes, array_nacionalidades,numero_bilhetes_gerados, raiz, aeroporto);
            adicionar_aproximacao(aeroporto, array_modelos, array_voos, array_origens, array_primeiros_nomes,
                                  array_segundos_nomes,array_nacionalidades, numero_avioes_aproximacao,
                                 numero_bilhetes_gerados,raiz, numero_avioes_aeroporto, numero_voos);
        }
    }
    // Para os dias seguintes: faz o gerenciamento conforme as listas vão enchendo
    else {

        if (aeroporto_fechado == false) {

            if (numero_avioes_aproximacao == 10) {
                //Remove um avião ao conjunto dos aviões a descolar
                if (numero_avioes_descolando >= 5) remover_descolagem(aeroporto, permissao_remocao, numero_avioes_descolando, numero_avioes_aeroporto);
                //Move um avião do conjunto de aviões em pista para o conjunto de aviões a descolar
                if (numero_avioes_pista == 7) mover_pista_descolagem(aeroporto, numero_avioes_pista, numero_avioes_descolando);
                //Move um avião do conjunto de aviões em aproximação para o conjunto de aviões em pista
                mover_aproximacao_pista(aeroporto, numero_avioes_aproximacao, array_voos, array_destinos, numero_avioes_pista, arvores_nacionalidade,numero_nacionalidades);
            }

            if (numero_avioes_aproximacao > 10) {
                for (int i = 0; i < 2; i++) {
                    mover_aproximacao_pista(aeroporto, numero_avioes_aproximacao, array_voos, array_destinos,
                                            numero_avioes_pista, arvores_nacionalidade, numero_nacionalidades);
                    if (numero_avioes_pista >= 7) mover_pista_descolagem(aeroporto, numero_avioes_pista, numero_avioes_descolando);
                    //Remove um avião ao conjunto dos aviões a descolar
                    if (numero_avioes_descolando >= 5) remover_descolagem(aeroporto, permissao_remocao, numero_avioes_descolando, numero_avioes_aeroporto);
                    permissao_remocao = 1;
                }
            }
        }
        //Adiciona um avião ao conjunto dos aviões em aproximação.
        aviao* aviao_ciclo = new aviao;
        *aviao_ciclo = aviao_aleatorio(array_voos, array_modelos, array_origens, array_primeiros_nomes, array_segundos_nomes,
                                       array_nacionalidades,numero_bilhetes_gerados, raiz, aeroporto);
        adicionar_aproximacao(aeroporto, array_modelos, array_voos, array_origens, array_primeiros_nomes, array_segundos_nomes,
                              array_nacionalidades, numero_avioes_aproximacao,numero_bilhetes_gerados, raiz, numero_avioes_aeroporto, numero_voos);
        permissao_remocao = 1;
    }
}


// GRAVAR E CARREGAR ESTADO AEROPORTO

/**
 * Função que recebe uma referência para uma estrutura de dados representando o estado atual do aeroporto
 * Grava o estado atual do aeroporto em três arquivos para cada lista (aproximação, pista e descolagem)
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 * @param arquivo_aproximacao - caminho para o arquivo de saída que irá armazenar os aviões em aproximação
 * @param arquivo_pista - caminho para o arquivo de saída que irá armazenar os aviões em pista
 * @param arquivo_descolagem - caminho para o arquivo de saída que irá armazenar os aviões descolando
 * @return true se a gravação for bem-sucedida
 *         falso caso contrário
 */
bool gravar_estado_aeroporto(aeroporto& aeroporto, const string& arquivo_aproximacao, const string& arquivo_pista, const string& arquivo_descolagem) {

    // Abre os arquivos de saída para cada lista de aviões
    ofstream out_aproximacao(arquivo_aproximacao), out_pista(arquivo_pista), out_descolagem(arquivo_descolagem);

    // Array contendo as três listas ligadas de aviões (aproximação, pista e descolagem)
    aviao* listas[3] = {aeroporto.avioes_aproximacao, aeroporto.avioes_pista, aeroporto.avioes_descolando};

    // Itera sobre as três listas de aviões
    for (int i = 0; i < 3; ++i) {

        // Determina qual arquivo de saída usar com base no índice utilizando o operador ternário
        ofstream& out = (i == 0) ? out_aproximacao : (i == 1) ? out_pista : out_descolagem;

        // Itera sobre os aviões da lista
        for (aviao* gravar_aviao = listas[i]; gravar_aviao != nullptr; gravar_aviao = gravar_aviao->next) {

            // Escreve os detalhes do avião no arquivo de saída
            out << gravar_aviao->voo << "|" << "\""
                << gravar_aviao->modelo << "\"" << "|" << "\""
                << gravar_aviao->origem << "\"" << "|" << "\""
                << gravar_aviao->destino << "\"" << "|"
                << gravar_aviao->capacidade << "|"
                << gravar_aviao->numero_passageiros << "\n";

            // Itera sobre os passageiros dos aviões da lista
            passageiro* gravar_passageiro = gravar_aviao->passageiros;
            while (gravar_passageiro != nullptr) {
                // Escreve os detalhes do passageiro no arquivo de saída
                out << gravar_passageiro->numeros_bilhete << "|"
                    << gravar_passageiro->primeiro_nome << "|"
                    << gravar_passageiro->segundo_nome << "|"
                    << gravar_passageiro->nacionalidade << "\n";

                // Move para o próximo passageiro na lista ligada
                gravar_passageiro = gravar_passageiro->next;
            }
        }
    }

    // Verifica se existiram falhas na gravação
    if (out_aproximacao.fail() || out_pista.fail() || out_descolagem.fail()) {
        return false; // Se existir alguma falha
    }
    return true; // Se não existir falhas
}

/**
 * Função que recebe quatro referências para uma estrutura e para três strings
 * Efetua o carregamento do estado de um aeroporto gravado previamente
 *
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 * @param arquivoProx - referência para uma string com o nome do ficheiro da lista ligada de aproximação gravado
 * @param arquivoPist - referência para uma string com o nome do ficheiro da lista ligada da pista gravado
 * @param arquivoDesc - referência para uma string com o nome do ficheiro da lista ligada da descolagem gravado
 */
void carregarEstadoAeroporto(aeroporto& aeroporto, const string& arquivoProx, const string& arquivoPist, const string& arquivoDesc) {
    ifstream inFiles[3] = {ifstream(arquivoProx), ifstream(arquivoPist), ifstream(arquivoDesc)};
    inicializacao_aeroporto(aeroporto);
    int numbAvioesAprox = 0;
    int numbAvioesPista = 0;
    int numbAvioesDeslc = 0;

    for (int i = 0; i < 3; ++i) {
        if (!inFiles[i]) {
            cerr << "Erro ao abrir arquivo para leitura." << std::endl;
            continue;
        }

        string linha;
        while (getline(inFiles[i], linha)) {
            if (linha.empty()) continue;

            linha.erase(remove(linha.begin(), linha.end(), '"'), linha.end());

            aviao* novoAviao = new aviao();
            stringstream ll(linha);

            getline(ll, novoAviao->voo, '|');
            getline(ll, novoAviao->modelo, '|');
            getline(ll, novoAviao->origem, '|');
            getline(ll, novoAviao->destino, '|');
            ll >> novoAviao->capacidade;
            ll.ignore();
            ll >> novoAviao->numero_passageiros;
            ll.ignore();

            novoAviao->passageiros = nullptr; // Inicializa a lista de passageiros como vazia

            // Itera sobre os passageiros do avião e os adiciona à lista ligada
            for (int j = 0; j < novoAviao->numero_passageiros; ++j) {
                getline(inFiles[i], linha);
                // Remove double quotes
                linha.erase(remove(linha.begin(), linha.end(), '"'), linha.end());

                passageiro* novoPassageiro = new passageiro();
                stringstream ssPass(linha);
                getline(ssPass, novoPassageiro->numeros_bilhete, '|');
                getline(ssPass, novoPassageiro->primeiro_nome, '|');
                getline(ssPass, novoPassageiro->segundo_nome, '|');
                getline(ssPass, novoPassageiro->nacionalidade, '|');

                // Adiciona o novo passageiro à lista ligada de passageiros do avião
                adicionar_passageiro(*novoAviao, *novoPassageiro);
            }

            // Adiciona o avião à lista apropriada do aeroporto
            switch (i) {
                case 0:
                    adicionarAviaoProximidade(aeroporto, novoAviao, numbAvioesAprox);
                    break;
                case 1:
                    adicionarAviaoPista(aeroporto, novoAviao, numbAvioesPista);
                    break;
                case 2:
                    adicionarAviaoDecolagem(aeroporto, novoAviao, numbAvioesDeslc);
                    break;
                default:
                    cerr << "Tipo de lista invalido." << endl;
                    break;
            }
        }

        inFiles[i].close();
    }
}


//FUNÇÕES DISPONÍVEIS

/**
 * Função que recebe duas referências para uma estrutura e para um inteiro e um apontador para um objeto
 * Adiciona um novo avião à lista de proximidade do aeroporto
 *
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 * @param novoAviao - apontador para um objeto do tipo 'aviao'
 * @param numbAvioesAprox - referência para um inteiro com o número de aviões na lista ligada de aproximação
 */
void adicionarAviaoProximidade(aeroporto& aeroporto, aviao* novoAviao, int& numbAvioesAprox) {
    if (aeroporto.avioes_aproximacao == nullptr) {
        aeroporto.avioes_aproximacao = novoAviao;
        aeroporto.ultimo_aviao_aproximacao = novoAviao;
        novoAviao->next = nullptr;
    } else {
        aeroporto.ultimo_aviao_aproximacao->next = novoAviao;
        novoAviao->prev = aeroporto.ultimo_aviao_aproximacao;
        aeroporto.ultimo_aviao_aproximacao = novoAviao;
        novoAviao->next = nullptr;
    }
    numbAvioesAprox += 1;

}

/**
 * Função que recebe duas referências para uma estrutura e para um inteiro e um apontador para um objeto
 * Adiciona um novo avião à lista da pista do aeroporto
 *
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 * @param novoAviao - apontador para um objeto do tipo 'aviao'
 * @param numbAvioesPista - referência para um inteiro com o número de aviões na lista ligada da pista
 */
void adicionarAviaoPista(aeroporto& aeroporto, aviao* novoAviao, int& numbAvioesPista) {
    if (aeroporto.avioes_pista == nullptr) {
        aeroporto.avioes_pista = novoAviao;
        aeroporto.ultimo_aviao_pista = novoAviao;
        novoAviao->next = nullptr;
    } else {
        aeroporto.ultimo_aviao_pista->next = novoAviao;
        novoAviao->prev = aeroporto.ultimo_aviao_pista;
        aeroporto.ultimo_aviao_pista = novoAviao;
        novoAviao->next = nullptr;
    }
    numbAvioesPista += 1;
}

/**
 * Função que recebe duas referências para uma estrutura e para um inteiro e um apontador para um objeto
 * Adiciona um novo avião à lista de descolagem do aeroporto
 *
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 * @param novoAviao - apontador para um objeto do tipo 'aviao'
 * @param numbAvioesDeslc - referência para um inteiro com o número de aviões na lista ligada da descolagem
 */
void adicionarAviaoDecolagem(aeroporto& aeroporto, aviao* novoAviao, int& numbAvioesDeslc) {
    if (aeroporto.avioes_descolando == nullptr) {
        aeroporto.avioes_descolando = novoAviao;
        aeroporto.ultimo_aviao_descolando = novoAviao;
        novoAviao->next = nullptr;
    } else {
        aeroporto.ultimo_aviao_descolando->next = novoAviao;
        novoAviao->prev = aeroporto.ultimo_aviao_descolando;
        aeroporto.ultimo_aviao_descolando = novoAviao;
        novoAviao->next = nullptr;
    }
    numbAvioesDeslc += 1;
}

/**
 * Função que recebe oito referências para uma estrutura, seis inteiros e um apontador para um nó de uma árvore, duas string, sete arrays de strings e um inteiro
 * Simula uma emergência de um avião que está na lista ligada de aproximação
 * Transfere um dado avião da lista ligada de pista para a de descolagem quando não há espaço suficiente para "aterrar" o avião em emergência
 *
 * @param aeroporto - referência para uma estrutura do tipo aeroporto
 * @param voo_de_emergencia - string com o número do voo que se encontra em emergência
 * @param voo_para_descolar - string com o número do voo que será transferido para a descolagem
 * @param array_modelos - array de strings que contém os modelos dos aviões
 * @param array_voos - array de strings que contém os números dos voos
 * @param array_origens - array de strings que contém as origens dos aviões
 * @param array_primeiros_nomes - array de strings que contém os primeiros nomes dos passageiros
 * @param array_segundos_nomes - array de strings que contém os primeiros nomes dos passageiros
 * @param array_nacionalidades - array de strings que contém os primeiros nomes dos passageiros
 * @param numero_avioes_aproximacao - referência para um inteiro com o número de aviões em aproximação
 * @param numero_bilhetes_gerados - referência para inteiro com o número total de bilhetes gerados
 * @param raiz - ponteiro para o nó raiz da árvore binária
 * @param numero_avioes_aeroporto - referência para um inteiro que contém o número total de aviões do aeroporto
 * @param numero_voos - referência para um inteiro com o número de voo
 * @param numero_avioes_pista - referência para um inteiro com o número de aviões em pista
 * @param array_destinos - array de strings que contém os destinos dos voos
 * @param numero_avioes_descolando - referência para um inteiro com o número de aviões descolando
 * @param permissao_remocao - referência para um inteiro para permitir a remoção (1) ou não permitir a remoção (0)
 */
void emergencia(aeroporto& aeroporto, string voo_de_emergencia, string voo_para_descolar, string array_modelos[], string array_voos[],
                string array_origens[], string array_primeiros_nomes[], string array_segundos_nomes[], string array_nacionalidades[],
                int& numero_avioes_aproximacao, long long &numero_bilhetes_gerados, nodo_arvore_binaria*& raiz, int& numero_avioes_aeroporto,
                int& numero_voos, int& numero_avioes_pista, const string array_destinos[], int numero_avioes_descolando, int& permissao_remocao){

    aviao *aviao_descolar = nullptr;
    aviao *aviao_emergencia = nullptr;
    aviao *aviao_em_pista = aeroporto.avioes_pista;
    aviao *aviao_em_aproximacao = aeroporto.avioes_aproximacao;
    aviao* ultimo_aviao_pista = aeroporto.ultimo_aviao_pista;
    aviao* ultimo_aviao_aproximacao = aeroporto.ultimo_aviao_aproximacao;
    aviao* ultimo_aviao_descolando = aeroporto.ultimo_aviao_descolando;

    // Encontra o número do voo
    while (aviao_em_pista != nullptr) {
        if (aviao_em_pista->voo == voo_para_descolar) {
            aviao_descolar = aviao_em_pista;
            break;
        }
        aviao_em_pista = aviao_em_pista->next;
    }

    while (aviao_em_aproximacao != nullptr) {
        if (aviao_em_aproximacao->voo == voo_de_emergencia) {
            aviao_emergencia = aviao_em_aproximacao;
            break;
        }
        aviao_em_aproximacao = aviao_em_aproximacao->next;
    }

    //Se não estiver espaço suficiente para o avião em emergência
    if (numero_avioes_pista == 7) {
        //Remove 'aviao_descolar' da lista ligada da pista
        if (aviao_descolar != nullptr) {
            if (aviao_descolar->prev != nullptr) {
                aviao_descolar->prev->next = aviao_descolar->next;
            } else {
                aeroporto.avioes_pista = aviao_descolar->next;
            }
            if (aviao_descolar->next != nullptr) {
                aviao_descolar->next->prev = aviao_descolar->prev;
            }
        }
        numero_avioes_pista -= 1;

        if (numero_avioes_descolando == 5){
            remover_descolagem(aeroporto, permissao_remocao, numero_avioes_descolando, numero_avioes_aeroporto);
            permissao_remocao = 1;
        }

        //Coloca o avião a descolar (que "dará" lugar ao avião em emergência) na lista ligada da descolagem
        if (ultimo_aviao_descolando == nullptr) {
            aeroporto.avioes_descolando = aviao_descolar;
        } else {
            ultimo_aviao_descolando->next = aviao_descolar;
            aviao_descolar->prev = ultimo_aviao_descolando;
        }
        aeroporto.ultimo_aviao_descolando = aviao_descolar;
        aviao_descolar->next = nullptr;
        numero_avioes_descolando += 1;
    }

    //Remove o avião em emergência da lista ligada de aproximação
    if (aviao_emergencia != nullptr) {
        if (aviao_emergencia->prev != nullptr) {
            aviao_emergencia->prev->next = aviao_emergencia->next;
        } else {
            aeroporto.avioes_aproximacao = aviao_emergencia->next;
        }
        if (aviao_emergencia->next != nullptr) {
            aviao_emergencia->next->prev = aviao_emergencia->prev;
        }
        else {
            aeroporto.ultimo_aviao_aproximacao = aviao_emergencia->prev;
        }
        numero_avioes_aproximacao -= 1;
    }

    aviao_emergencia->voo = array_voos[rand() % contador_linhas("voo.txt")];
    aviao_emergencia->origem = "Aeroporto EDA";
    aviao_emergencia->destino = array_destinos[rand() % contador_linhas("destino.txt")];

    //Insere o 'aviao_emergencia' em 'avioes_pista'
    if (ultimo_aviao_pista == nullptr) {
        aeroporto.avioes_pista = aviao_emergencia;
    } else {
        ultimo_aviao_pista->next = aviao_emergencia;
        aviao_emergencia->prev = ultimo_aviao_pista;
    }
    aeroporto.ultimo_aviao_pista = aviao_emergencia;
    aviao_emergencia->next = nullptr;
    numero_avioes_pista += 1;

    //Adiciona um novo avião à lista ligada de aproximação
    adicionar_aproximacao(aeroporto, array_modelos, array_voos, array_origens, array_primeiros_nomes, array_segundos_nomes,
                          array_nacionalidades, numero_avioes_aproximacao,numero_bilhetes_gerados, raiz, numero_avioes_aeroporto, numero_voos);
}

/**
 * Função que recebe duas referências para um inteiro e para um booleano
 * Responsável por abrir e fechar o aeroporto
 * Esta função altera o estado do aeroporto (aberto ou fechado) com base no número de aviões
 * em aproximação. Se o aeroporto está fechado e há mais de 10 aviões a se aproximar,
 * o número de aviões é decrementado em 1 e o aeroporto é aberto. Se o aeroporto está aberto
 * e há exatamente 10 aviões a se aproximar, o número de aviões é incrementado em 1 e o
 * aeroporto é fechado.
 *
 * @param numero_avioes_aproximacao - referência para um inteiro com o número de aviões em aproximação
 * @param aeroporto_fechado - booleano relativo ao estado do aeroporto (se está, ou não, fechado)
 */
void fechar_abrir_aeroporto(int& numero_avioes_aproximacao, bool& aeroporto_fechado){
    if (aeroporto_fechado) {
        if (numero_avioes_aproximacao > 10) {
                numero_avioes_aproximacao -= 1;
            }
            aeroporto_fechado = false;
    }
    else {
        if (numero_avioes_aproximacao == 10) {
        numero_avioes_aproximacao += 1;
    }
    aeroporto_fechado = true;
    }
}