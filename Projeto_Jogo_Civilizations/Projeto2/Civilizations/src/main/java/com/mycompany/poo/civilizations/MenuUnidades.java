/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package com.mycompany.poo.civilizations;

import java.util.*;
import java.util.InputMismatchException;

/**
 *
 * @author Diogo
 */
public class MenuUnidades implements IMenu{
    private GestorUnidades unidades; 
    private GestorMovimento movimento;
    private GestorAtaque ataque;
    private GestorConstrucao construcao;
    private Scanner s;
    private IMapa mapa;
    private Cidade cidade;
    private List<IUnidade> listaUnidades;
    
    public MenuUnidades(GestorUnidades unidades, IMapa mapa, Cidade cidade, 
            GestorMovimento movimento, GestorAtaque ataque, GestorConstrucao construcao) {
        this.unidades = unidades;
        this.s = new Scanner(System.in);
        this.mapa = mapa;
        this.cidade = cidade;
        this.movimento = movimento;
        this.ataque = ataque;
        this.construcao = construcao;
        this.listaUnidades = new ArrayList<>();
    }
    /**
    * Exibe as opções disponíveis no menu de unidades.
    *
    * Este método apresenta um menu com várias opções relacionadas às unidades, 
    * incluindo a produção de unidades, movimentação, ações militares, 
    * colonização de cidades e construção.
    */
   @Override
   public void exibirOpcoes() {
       GestorMensagens.mostrarMensagem("\nMenu Unidades");
       GestorMensagens.mostrarMensagem("1. Produzir unidade");
       GestorMensagens.mostrarMensagem("2. Mover unidade");
       GestorMensagens.mostrarMensagem("3. Acao Militar");
       GestorMensagens.mostrarMensagem("4. Colonizar cidade");
       GestorMensagens.mostrarMensagem("5. Construir");
       GestorMensagens.mostrarMensagem("0. Voltar");
       GestorMensagens.mostrarMensagem("Escolha uma opcao no Menu Unidades: ");
   }

   /**
    * Processa a escolha do jogador no menu de unidades.
    *
    * Este método exibe as opções disponíveis e executa a ação correspondente 
    * com base na escolha do jogador. O loop continua até que o jogador decida 
    * voltar ao menu anterior.
    *
    * @param opcao a opção escolhida pelo jogador
    */
   @Override
   public void processarEscolha(int opcao) {
       try {
           opcao = s.nextInt();
           s.nextLine(); // Limpa o buffer
           switch (opcao) {
               case 1:
                   GestorMensagens.mostrarMensagem("\nQue tipo de unidade: ");
                   exibirOpcoesUnidades();
                   processarEscolhaUnidade(opcao);
                   break;
               case 2:
                   GestorMensagens.mostrarMensagem("Escolha o tipo de unidade para mover:");
                   exibirOpcoesUnidades();
                   String unidade = movimento.processarEscolhaUnidade(opcao);
                   if (unidade.equals("")) { return; }
                   listaUnidades = exibirUnidades(unidade);
                   if (listaUnidades != null) { movimento.acaoUnidade(unidade, listaUnidades); }
                   break;
               case 3:
                   MenuAcaoMilitar();
                   break;    
               case 4:
                   realizarColonizacao("COLONO");
                   break;  
               case 5:
                   MenuTiposConstrucao();                   
                   break;    
               case 0:
                   break;
               default:
                   GestorMensagens.mostrarMensagem("\nOpcao invalida!");
                   break;
           }
       } catch (InputMismatchException e) {
           GestorMensagens.mostrarMensagem("Entrada invalida! Insira um numero.");
           s.nextLine(); // Limpa o buffer de entrada inválida
       }    
   }

   /**
    * Realiza a colonização de uma cidade utilizando uma unidade colono.
    *
    * Este método verifica se existem unidades do tipo colono disponíveis e 
    * solicita ao jogador que escolha uma unidade para colonizar a cidade 
    * correspondente à posição do colono.
    *
    * @param tipoUnidade o tipo da unidade a ser utilizada para colonização
    */
   private void realizarColonizacao(String tipoUnidade) {
       boolean existe = unidades.verificarUnidadeExistente(tipoUnidade);

       if (!existe) {
           GestorMensagens.mostrarMensagem("Nao existe nenhuma unidade do tipo " + 
                   tipoUnidade + " na cidade");
           return;
       }  
       List<IUnidade> colonosEmCidade = unidades.getColonosEmCidade();

       if (colonosEmCidade.isEmpty()) {
           GestorMensagens.mostrarMensagem("Nao existe nenhuma unidade Colono em posicao"
                   + " para colonizar.");
           return;
       }
       exibirColonos(colonosEmCidade);
       GestorMensagens.mostrarMensagem("Selecione o índice da unidade Colono para colonizar:");
       int indice = s.nextInt() - 1; // Converte para índice baseado em 0
       if (indice < 0 || indice >= colonosEmCidade.size()) {
           GestorMensagens.mostrarMensagem("Indice invalido.");
           return;
       }
       IUnidade colonoSelecionado = colonosEmCidade.get(indice);
       Cidade cidadeAColonizar = mapa.getCidadePorCoordenada(cidade.getCivilizacao(), 
               colonoSelecionado.getPosX(), colonoSelecionado.getPosY());
       unidades.colonizar(colonoSelecionado, cidadeAColonizar);
   }

   /**
    * Exibe as opções de construção disponíveis.
    *
    * Este método apresenta um menu com os tipos de construção que podem ser 
    * realizados e processa a escolha do jogador.
    */
    private void MenuTiposConstrucao() {
        GestorMensagens.mostrarMensagem("\nTipos de construcao");
        GestorMensagens.mostrarMensagem("1. Estrada");
        GestorMensagens.mostrarMensagem("0. Voltar");
        GestorMensagens.mostrarMensagem("Escolha uma opcao: ");
        int escolha = s.nextInt();
        s.nextLine(); // Limpa o buffer

        switch (escolha) {
            case 1: 
                listaUnidades = exibirUnidades("CONSTRUTOR");
                    if(listaUnidades != null) {
                        construcao.realizarConstrucao("CONSTRUTOR", listaUnidades);
                    }
                break;
            case 0:
                break;
            default:
                GestorMensagens.mostrarMensagem("Opcao invalida!");
                break;
        }
    }
    /**
    * Exibe o menu de ações militares disponíveis.
    *
    * Este método apresenta opções para o jogador realizar ações militares, 
    * como atacar ou capturar. O jogador pode escolher uma ação e, se 
    * necessário, as unidades militares disponíveis serão exibidas.
    */
   private void MenuAcaoMilitar() {
       GestorMensagens.mostrarMensagem("\nAcao Militar");
       GestorMensagens.mostrarMensagem("1. Atacar");
       GestorMensagens.mostrarMensagem("2. Capturar");
       GestorMensagens.mostrarMensagem("0. Voltar");
       GestorMensagens.mostrarMensagem("Escolha uma opcao: ");
       int escolha = s.nextInt();
       s.nextLine(); // Limpa o buffer

       switch (escolha) {
           case 1: 
               listaUnidades = exibirUnidades("MILITAR");
               if (listaUnidades != null) { ataque.acaoUnidade("MILITAR", listaUnidades); }
               break;
           case 2:
               listaUnidades = exibirUnidades("MILITAR");
               if (listaUnidades != null) { ataque.realizarCaptura(listaUnidades); }
               break;
           case 0:
               break;
           default:
               GestorMensagens.mostrarMensagem("Opcao invalida!");
               break;
       }
   }

   /**
    * Exibe a lista de colonos disponíveis e suas posições.
    *
    * Este método itera sobre a lista de unidades do tipo colono e exibe 
    * suas posições no console, indicando se estão ocupadas ou não.
    *
    * @param colonos a lista de unidades do tipo colono a serem exibidas
    */
   private void exibirColonos(List<IUnidade> colonos) {
       for (int i = 0; i < colonos.size(); i++) {
           IUnidade unidade = colonos.get(i);
           GestorMensagens.mostrarMensagem((i + 1) + ". Posicao: (" + unidade.getPosX() + ", " + unidade.getPosY() + ")"
                   + (unidade.isOcupada() ? " [OCUPADA]" : ""));
       }
   }

   /**
    * Exibe as unidades disponíveis do tipo especificado.
    *
    * Este método verifica se existem unidades do tipo selecionado e, se 
    * existirem, exibe suas posições. Retorna a lista de unidades do tipo 
    * especificado.
    *
    * @param tipoUnidade o tipo de unidade a ser exibido
    * @return a lista de unidades do tipo especificado ou null se não houver unidades
    */
   private List<IUnidade> exibirUnidades(String tipoUnidade) {
       // Verifica se existem unidades do tipo selecionado
       boolean existe = unidades.verificarUnidadeExistente(tipoUnidade);

       if (!existe) {
           GestorMensagens.mostrarMensagem("Nao existe nenhuma unidade do tipo " + 
                   tipoUnidade + " na cidade");
           return null;
       }       
       // Obtém a lista de unidades do tipo selecionado
       List<IUnidade> listaUnidades = unidades.getUnidades().get(tipoUnidade);

       if (listaUnidades.isEmpty()) {
           GestorMensagens.mostrarMensagem("Nao existem unidades do tipo " + tipoUnidade + ".");
           return null;
       }
       // Exibe as unidades com suas posições
       GestorMensagens.mostrarMensagem("Unidades disponiveis do tipo " + tipoUnidade + ":");
       for (int i = 0; i < listaUnidades.size(); i++) {
           IUnidade unidade = listaUnidades.get(i);
           GestorMensagens.mostrarMensagem((i + 1) + ". Posicao: (" + unidade.getPosX() + ", " + unidade.getPosY() + ")"
                   + (unidade.isOcupada() ? " [OCUPADA]" : ""));
       }
       return listaUnidades;
   }

   /**
    * Exibe as opções de tipos de unidades disponíveis para o jogador.
    *
    * Este método apresenta um menu com as opções de tipos de unidades que 
    * podem ser produzidas, permitindo que o jogador escolha um tipo específico.
    */
   public void exibirOpcoesUnidades() {
       GestorMensagens.mostrarMensagem("1. COLONO");
       GestorMensagens.mostrarMensagem("2. MILITAR");
       GestorMensagens.mostrarMensagem("3. CONSTRUTOR");
       GestorMensagens.mostrarMensagem("0. Voltar");
       GestorMensagens.mostrarMensagem("Escolha um tipo de unidade: ");
   }
    /**
    * Processa a escolha do jogador para produzir uma unidade.
    *
    * Este método exibe um menu que permite ao jogador escolher o tipo de 
    * unidade a ser produzida (COLONO, MILITAR ou CONSTRUTOR). O loop continua 
    * até que uma opção válida seja escolhida ou o jogador decida voltar.
    *
    * @param opcao a opção escolhida pelo jogador
    */
    public void processarEscolhaUnidade(int opcao) {  
        do {
            try {
                opcao = s.nextInt();
                switch (opcao) {
                    case 1:
                        unidades.produzirUnidade("COLONO");
                        break; 
                    case 2:
                        unidades.produzirUnidade("MILITAR");
                        break; 
                    case 3:
                        unidades.produzirUnidade("CONSTRUTOR");
                        break;     
                    case 0:
                        break;
                    default:
                        GestorMensagens.mostrarMensagem("\nOpcao invalida. Tente novamente.");
                        break;
                } 
            } catch (InputMismatchException e) {
                GestorMensagens.mostrarMensagem("Entrada invalida! Insira um numero.");
                s.nextLine(); // Limpa o buffer de entrada inválida
            }
            
        } while (!((opcao >= 0) && (opcao < 4)));    
    } 
}
