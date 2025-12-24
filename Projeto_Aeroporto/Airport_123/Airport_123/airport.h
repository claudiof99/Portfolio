#include <string>
#include <fstream>
#include <sstream>
using namespace std;

#ifndef AIRPORT
#define AIRPORT

// ESTRUTURAS
struct passageiro {
    string numeros_bilhete;
    string primeiro_nome;
    string segundo_nome;
    string nacionalidade;
    passageiro* next;
    passageiro* prev;
};

struct aviao {
    string voo;
    string modelo;
    string origem;
    string destino;
    int capacidade;
    passageiro* passageiros;
    passageiro* ultimo_passageiro;
    int numero_passageiros;
    aviao* next;
    aviao* prev;
};

struct aeroporto{
    aviao* avioes_aproximacao; // Lista de aviões em aproximação
    aviao* avioes_pista; // Lista de aviões na pista
    aviao* avioes_descolando; // Lista de aviões descolando
    aviao* ultimo_aviao_aproximacao; // Ponteiro para o último avião na lista de aproximação
    aviao* ultimo_aviao_pista; // Ponteiro para o último avião na lista de pista
    aviao* ultimo_aviao_descolando; // Ponteiro para o último avião na lista de descolagem
    int numero_avioes_partiram; // Número de aviões que descolaram
};

struct nodo_arvore_binaria {
    string item;
    passageiro* passageiros; // Lista de passageiros
    nodo_arvore_binaria * esq;
    nodo_arvore_binaria * dir;
};

struct nodo_lista_ligada {
    string item;
    nodo_lista_ligada * next;

    nodo_lista_ligada(string data) {
        item = data;
        next = nullptr;
    }
};


// GERAÇÃO DOS ARRAYS A PARTIR DOS FICHEIROS
int contador_linhas(string nome_ficheiro);
void carregar_dados(string nome_ficheiro, string array[], int numero_linhas);


// GERAÇÃO DE VALORES ALEATÓRIOS
string aleatorio_numero_bilhete();
passageiro passageiro_aleatorio(const string array_numero_bilhetes[], long long &numero_bilhetes_gerados,
                                const string array_primeiros_nomes[], const string array_segundos_nomes[],
                                const string array_nacionalidades[], nodo_arvore_binaria*& raiz);
bool numero_voo_unico(const aeroporto& aeroporto, const string& numero_voo);
aviao aviao_aleatorio(const string array_voos[], const string array_modelos[], const string array_origens[],
                      const string array_primeiros_nomes[], const string array_segundos_nomes[],
                      const string array_nacionalidades[], long long &numero_bilhetes_gerados, nodo_arvore_binaria*& raiz,
                      aeroporto& aeroporto);

// INICIALIZAÇÃO DO AEROPORTO
nodo_arvore_binaria * novo_nodo_arvore(string item);
nodo_arvore_binaria* inserir_nodo_arvore(nodo_arvore_binaria*& raiz, string item);
nodo_lista_ligada* lista_ligada_nacionalidades_estrangeiras(string array_nacionalidades[], int& numero_nacionalidades);
nodo_arvore_binaria** criar_arvores_nacionalidade(nodo_lista_ligada* head, int& numero_nacionalidades);
void inicializacao_aeroporto(aeroporto& aeroporto);


// GERENCIAMENTO DOS AVIÕES E PASSAGEIROS
void adicionar_passageiro(aviao& aviao, passageiro novo_passageiro);
void adicionar_aproximacao(aeroporto& aeroporto, const string array_modelos[], const string array_voos[], const string array_origens[],
                           const string array_primeiros_nomes[], const string array_segundos_nomes[], const string array_nacionalidades[],
                           int& numero_avioes_aproximacao, long long &numero_bilhetes_gerados, nodo_arvore_binaria*& raiz, int& numero_avioes_aeroporto,
                           int& numero_voos);
void mover_aproximacao_pista(aeroporto& aeroporto, int& numero_avioes_aproximacao, const string array_voos[],
                             const string array_destinos[], int& numero_avioes_pista, nodo_arvore_binaria**& arvores_nacionalidade,
                             int numero_nacionalidades, long long& numero_bilhetes_gerados, const string array_primeiros_nomes[],
                             const string array_segundos_nomes[], const string array_nacionalidades[], nodo_arvore_binaria*& raiz);
void mover_pista_descolagem(aeroporto& aeroporto, int& numero_avioes_pista, int& numero_avioes_descolando);
void remover_descolagem(aeroporto& aeroporto, int& permissao_remocao, int& numero_avioes_descolando, int& numero_avioes_aeroporto);
void esvaziarPassageiros(aviao& aviao);


//FUNÇÕES DISPONÍVEIS
aviao* encontrar_aviao_por_numero_voo(aviao* lista_avioes, const string& numero_voo);
void inverter_lista_pista(aeroporto& aeroporto);
void mostrar_passageiros_na_aproximacao(const aeroporto& aeroporto);
void mostrar_passageiros_na_pista(const aeroporto& aeroporto);
void mostrar_passageiros_na_descolagem(const aeroporto& aeroporto);
void pesquisar_passageiro_por_nome_chegadas(const aeroporto& aeroporto, const string& primeiro_nome);
void pesquisar_passageiro_por_nome_descolagens(const aeroporto& aeroporto, const string& primeiro_nome);
void editar_nacionalidade_por_nome(const aeroporto& aeroporto, const string& primeiro_nome, const string&segundo_nome , const string& nova_nacionalidade);
void ordena_passageiros_na_pista_por_nacionalidade(const string& nacionalidade, aviao* avioes_pista);
void mostrar_passageiros_na_pista_ordenados_alfabetica(const aeroporto& aeroporto, nodo_arvore_binaria** arvores_nacionalidade, int numero_nacionalidades);
nodo_arvore_binaria* criar_arvore_passageiros_por_nacionalidade(const string& nacionalidade, aviao* avioes_pista);
nodo_arvore_binaria* inserir_passageiro(nodo_arvore_binaria* raiz, passageiro* novo_passageiro );
void mostrar_passageiros_na_pista_arvore(const aeroporto& aeroporto, nodo_arvore_binaria** arvores_nacionalidade, int numero_nacionalidades);
void imprimeArvore(const string& prefix, nodo_arvore_binaria* no, bool esquerda);
void adicionarAviaoProximidade(aeroporto& aeroporto, aviao* novoAviao, int& numbAvioesAprox);
void adicionarAviaoPista(aeroporto& aeroporto, aviao* novoAviao, int& numbAvioesPista);
void adicionarAviaoDecolagem(aeroporto& aeroporto, aviao* novoAviao, int& numbAvioesDeslc);
void emergencia(aeroporto& aeroporto, string voo_de_emergencia, string voo_para_descolar, string array_modelos[], string array_voos[], string array_origens[],
                string array_primeiros_nomes[], string array_segundos_nomes[],
                string array_nacionalidades[],  int& numero_avioes_aproximacao, long long &numero_bilhetes_gerados, nodo_arvore_binaria*& raiz,
                int& numero_avioes_aeroporto, int& numero_voos, int& numero_avioes_pista, const string array_destinos[], int numero_avioes_descolando, int& permissao_remocao);
void fechar_abrir_aeroporto(int& numero_avioes_aproximacao, bool& aeroporto_fechado);

// ESTADO DO AEROPORTO
void estado_aeroporto(aeroporto & aeroporto, int& numero_avioes_aeroporto);

// SIMULAÇÃO DOS CICLOS
void simulacao_ciclo(int& dia, aeroporto& aeroporto, string array_modelos[], string array_voos[], string array_origens[],
                     string array_destinos[], string array_primeiros_nomes[], string array_segundos_nomes[],
                     string array_nacionalidades[],  int& numero_avioes_aproximacao, int& permissao_remocao,
                     int& numero_avioes_pista, int& numero_avioes_descolando, long long &numero_bilhetes_gerados, nodo_arvore_binaria*& raiz,
                     int& numero_avioes_aeroporto, int& numero_voos, nodo_arvore_binaria**& arvores_nacionalidade,
                     int numero_nacionalidades, bool aeroporto_fechado);

//GRAVAR E CARREGAR AEROPORTO
bool gravar_estado_aeroporto(aeroporto& aeroporto, const string& arquivo_aproximacao, const string& arquivo_pista, const string& arquivo_descolagem);
void carregarEstadoAeroporto(aeroporto& aeroporto, const string& arquivoProx, const string& arquivoPist, const string& arquivoDesc);

#endif