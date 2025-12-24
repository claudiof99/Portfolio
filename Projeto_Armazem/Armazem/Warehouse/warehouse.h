#ifndef WAREHOUSE
#define WAREHOUSE

#include <string>
using namespace std;

///GERAÇÃO DE VALORES ALEATÓRIOS
int aleatorio_capacidade();
int aleatorio_numero_seccoes();
int aleatorio_numero_serie();
int aleatorio_preco();
int aleatorio_probabilidade_venda();


///GERAÇÃO DOS ARRAYS A PARTIR DOS FICHEIROS
void categorias(string categoria[], int numero_categorias);
int contador_linhas_categorias();
void marcas(string marca[], int numero_marcas);
int contador_linhas_marcas();


///INICIALIZAÇÃO DAS SECÇÕES, CATEGORIAS E LISTA DE CHEGADA
struct peca {
    string marca;
    string categoria;
    int numero_serie;
    float preco;
    int probabilidade_venda;
    bool peca_em_promocao;
};

struct seccao {
    char ID;
    string categoria;
    int capacidade;
    int quantidade;
    int faturacao;
    bool seccao_em_promocao;
    int duracao_promocao;
    int valor_promocao;
    peca ** pecas;
};

peca * inicializacao_pecas(int numero_pecas, seccao * seccoes, int numero_seccoes, int numeros_serie[], int& numeros_serie_gerados, int numeros_serie_disponiveis);
seccao * inicializacao_seccoes(int numero_seccoes);
void inicializacao_lista_chegada(peca *& lista_chegada, int capacidade_maxima, int & primeiro_livre, const peca & nova_peca, peca *& lista_de_espera, int &capacidade_espera, int & primeiro_livre_secundario);


///FUNCIONAMENTO
void venda_pecas(seccao* seccoes, int numero_seccoes, float & total_faturacao_armazem);
void transferencia_pecas(peca * lista_chegada, int & primeiro_livre, seccao * seccoes, int numero_seccoes);

///GESTÃO
void venda_manual(seccao* seccoes, int numero_seccoes, peca* lista_chegada, int& primeiro_livre, float &total_faturacao);
void adicionar_promocao(seccao* seccoes, int numero_seccoes, int dia_atual);
void aplicar_promocao(seccao* seccoes, int numero_seccoes, int dia_atual);
bool categoria_existe(string categoria_nova, int numero_categorias);
void adicionar_categoria(string categoria_nova);
void alterar_categoria(seccao * seccoes, string categoria_nova, int numero_seccoes);
void adicionar_seccao(seccao*&seccoes, int &numero_seccoes);
void armazem_alfabetica(seccao* seccoes, peca * lista_chegada, int &primeiro_livre, int &numero_seccoes);
void armazem_preco(seccao* seccoes, peca * lista_chegada, int &primeiro_livre, int &numero_seccoes);


void gravar_estado_armazem(seccao* seccoes, peca* lista_chegada, int numero_seccoes, float total_faturacao, int dia,
                           int primeiro_livre, int numeros_serie_gerados, int capacidade_espera, bool & estado_gravacao,
                           int primeiro_livre_secundario, peca * lista_de_espera);

void carregar_estado_armazem(seccao* &seccoes, peca* &lista_chegada, int &numero_seccoes, float &total_faturacao, int &dia,
                             int &primeiro_livre, int & numeros_serie_gerados, int &capacidade_espera, bool & estado_gravacao,
                             int &primeiro_livre_secundario, peca * &lista_de_espera);

#endif